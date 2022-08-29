#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.Deploy
{
    internal class DeployCommandContentProvider : PriorityContentProvider
    {
        public override SubjectIdentifier LowerPrioritySubject { get; } = new SubjectIdentifier(nameof(CommandDefinitionContentProvider));
        private readonly IFileSystem fileSystem;

        public DeployCommandContentProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return (key == Constants.OutputArgumentName &&
                    CommandEntity.Decorate(owner).CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)) ||
                   (key == EntityKeys.InternalDeployPathKey &&
                   TargetEntity.Decorate(owner).HasFullName);
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.InternalDeployPathKey:
                    VirtualDirectory deployRoot = GetDeployRoot();
                    return owner.Create(key, deployRoot.FullName, deployRoot);

                case Constants.OutputArgumentName:
                default:
                    string outputDirectory = GetOutput();
                    return owner.Create(key, outputDirectory);
            }

            string GetOutput()
            {
                CommandEntity command = CommandEntity.Decorate(owner);
                FileEntity projectFileEntity = FileEntity.Decorate(owner.Root);
                string outputDirectory = command.GetSingleValueArgument(Constants.OutputArgumentName);

                if (string.IsNullOrEmpty(outputDirectory))
                {
                    outputDirectory = projectFileEntity.Directory
                                                       .Directory(Constants.LibraryFolderName)
                                                       .FullName;
                }
                return outputDirectory;
            }

            VirtualDirectory GetDeployRoot()
            {
                TargetEntity targetEntity = TargetEntity.Decorate(owner);
                BuildEntity buildEntity = BuildEntity.Decorate(owner);
                Entity project = owner.Root;
                CommandEntity commandOrigin = CommandEntity.Decorate(owner.Origin);
                VirtualDirectory outputRoot = fileSystem.GetDirectory(commandOrigin.Output, project.Path);
                VirtualDirectory deployRoot = outputRoot.Directory(targetEntity.FullName.Replace(',', '_'),
                                                                   buildEntity.BuildType);
                return deployRoot;
            }            
        }
    }
}
