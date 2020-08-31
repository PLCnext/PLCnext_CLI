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
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Deploy
{
    internal class CopyDependenciesDeployStep : IDeployStep
    {
        private readonly ExecutionContext executionContext;
        private readonly IFileSystem fileSystem;

        public CopyDependenciesDeployStep(ExecutionContext executionContext, IFileSystem fileSystem)
        {
            this.executionContext = executionContext;
            this.fileSystem = fileSystem;
        }

        public string Identifier => "CopyDependenciesDeployStep";

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            
            IEnumerable<Entity> targets = project.Targets.ToArray();
            if (!targets.Any())
            {
                throw new FormattableException("Please use --target to specify for which targets the library shall be generated.");
            }

            foreach (Entity target in targets)
            {
                VirtualDirectory deployRoot = DeployEntity.Decorate(target).DeployDirectory;
                VirtualDirectory deployDirectory = deployRoot.Directory("lib", project.Name);
                BuildEntity buildEntity = BuildEntity.Decorate(target);
                if (!buildEntity.HasBuildSystem)
                {
                    TargetEntity targetEntity = TargetEntity.Decorate(target);
                    executionContext.WriteWarning(new CMakeBuildSystemNotFoundException(targetEntity.FullName, buildEntity.BuildType).Message);
                    return;
                }
                
                foreach (string externalLibrary in buildEntity.BuildSystem.ExternalLibraries)
                {
                    VirtualFile newFile = fileSystem.GetFile(externalLibrary).CopyTo(deployDirectory);
                    executionContext.Observable.OnNext(new Change(() => newFile.Delete(), $"Copied {externalLibrary} to {newFile.FullName}."));
                }
            }
        }
    }
}
