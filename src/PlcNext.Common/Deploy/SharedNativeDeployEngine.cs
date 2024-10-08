﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac.Features.AttributeFilters;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Tools.MSBuild;

namespace PlcNext.Common.Deploy
{
    internal class SharedNativeDeployEngine : IDeployService
    {
        private readonly IDeployService defaultDeployService;
        private readonly IFileSystem fileSystem;
        private readonly ExecutionContext executionContext;
        private readonly IDirectoryPackService directoryPackService;
        private readonly IProcessManager processManager;
        private readonly ILog log;
        private readonly IMSBuildFinder msBuildFinder;

        public SharedNativeDeployEngine([KeyFilter("DefaultDeployEngine")] IDeployService defaultDeployService, IFileSystem fileSystem,
                                        ExecutionContext executionContext, IDirectoryPackService directoryPackService,
                                        IProcessManager processManager, ILog log, IMSBuildFinder msBuildFinder)
        {
            this.defaultDeployService = defaultDeployService;
            this.fileSystem = fileSystem;
            this.executionContext = executionContext;
            this.directoryPackService = directoryPackService;
            this.processManager = processManager;
            this.log = log;
            this.msBuildFinder = msBuildFinder;
        }

        public void DeployFiles(Entity dataModel)
        {
            defaultDeployService.DeployFiles(dataModel);

            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            string csharpProjectPath = project.CSharpProjectPath;
            if (!Path.IsPathRooted(csharpProjectPath))
            {
                if(csharpProjectPath == null)
                {
                    csharpProjectPath = project.Path;
                    log.LogWarning("CSharp project path not found. Using project path instead.");
                }
                else
                {
                    csharpProjectPath = Path.Combine(project.Path, csharpProjectPath);
                }
            }

            string csharpProjectOutputFolder = FindCSharpProjectOutputFolder();

            if (fileSystem.DirectoryExists(csharpProjectOutputFolder, csharpProjectPath))
            {
                CommandEntity command = CommandEntity.Decorate(dataModel);
                string basePath = project.Path;
                VirtualDirectory destinationDirectory;
                if (!command.IsCommandArgumentSpecified(Constants.OutputArgumentName))
                {
                    destinationDirectory = fileSystem.GetDirectory(Path.Combine(basePath, 
                                                                                Constants.LibraryFolderName, 
                                                                                Constants.ConfigIndependentFiles));
                }
                else
                {
                    destinationDirectory = fileSystem.GetDirectory(Path.Combine(basePath, 
                                                                                command.GetSingleValueArgument(Constants.OutputArgumentName), 
                                                                                Constants.ConfigIndependentFiles));
                }

                CopyDlls();
                CopyProjectItems();
                CopyHelpFiles();

                void CopyDlls()
                {
                    VirtualDirectory sourceDirectory = fileSystem.GetDirectory(csharpProjectOutputFolder,
                                                                               csharpProjectPath,
                                                                               false);
                    foreach (VirtualFile deployableFile in sourceDirectory.Files("*.dll"))
                    {
                        VirtualFile destination = fileSystem.GetFile(deployableFile.Name, destinationDirectory.FullName);

                        using (Stream source = deployableFile.OpenRead(true))
                        using (Stream dest = destination.OpenWrite())
                        {
                            dest.SetLength(0);
                            source.CopyTo(dest);
                        }

                        executionContext.WriteVerbose($"Deployed file {deployableFile.FullName} to {destination.FullName}.");
                    }
                }

                void CopyProjectItems()
                {
                    VirtualDirectory sourceDirectory = fileSystem.GetDirectory(Constants.CSharpProjectItemsFolderName,
                                                                               csharpProjectPath, false);

                    foreach (VirtualFile deployableFile in sourceDirectory.Files(searchRecursive: true))
                    {
                        string relativePath = deployableFile.FullName.GetRelativePath(sourceDirectory.FullName);

                        VirtualFile destination = fileSystem.GetFile(relativePath, destinationDirectory.FullName);

                        using (Stream source = deployableFile.OpenRead(true))
                        using (Stream dest = destination.OpenWrite())
                        {
                            dest.SetLength(0);
                            source.CopyTo(dest);
                        }

                        executionContext.WriteVerbose($"Deployed file {relativePath} to {destination.FullName}.");
                    }
                }

                void CopyHelpFiles()
                {
                    if (!fileSystem.DirectoryExists(Constants.CSharpHelpFolderName, csharpProjectPath))
                    {
                        return;
                    }
                    VirtualDirectory sourceDirectory = fileSystem.GetDirectory(Constants.CSharpHelpFolderName,
                                                                               csharpProjectPath, false);
                    
                    const string helpFileSuffix = "FBFun.chm";
                    List<string> usedCultures = new List<string>();
                    string standardHelpFileName = project.Name + "_" + helpFileSuffix;
                    string[] libraryNameTokens = project.Name.Split('_');

                    foreach (VirtualFile deployableFile in sourceDirectory.Files(searchRecursive: true))
                    {
                        string relativePath = deployableFile.FullName.GetRelativePath(sourceDirectory.FullName);

                        if (!relativePath.StartsWith("HTML5", StringComparison.InvariantCulture))
                        {
                            HandleChmHelp(deployableFile, relativePath);
                        }

                    }
                    HandleHtmlHelp();

                    void HandleHtmlHelp()
                    {
                        string htmlDirectory = "HTML5";
                        if (!sourceDirectory.DirectoryExists(htmlDirectory))
                        {
                            return;
                        }
                        IEnumerable<string> cultures = sourceDirectory.Directory(htmlDirectory).Directories.Select(d => d.Name)
                                                           .Where(d => !string.IsNullOrEmpty(d) && !d.Equals("images", StringComparison.OrdinalIgnoreCase));

                        //no culture check is performed when deploying

                        VirtualDirectory deployHelpDirectory = fileSystem.GetDirectory(Constants.DeployHelpDirectoryName, destinationDirectory.FullName);
                        deployHelpDirectory.Clear();

                        foreach (string culture in cultures)
                        {
                            VirtualDirectory destinationDir = deployHelpDirectory;
                            if (!culture.Equals("default", StringComparison.OrdinalIgnoreCase))
                            {
                                destinationDir = deployHelpDirectory.Directory(culture);
                            }

                            string destination = Path.Combine(destinationDir.FullName, project.Name + "_FBfun.Help.zip");
                            
                            directoryPackService.Pack((sourceDirectory.Directory(htmlDirectory).Directory(culture)), destination);
                            executionContext.WriteVerbose($"Deployed help culture {culture} to {destination}.");
                        }
                    }

                    void HandleChmHelp(VirtualFile deployableFile, string relativePath)
                    {
                        bool checkLibName(string[] fileTokens)
                        {
                            bool result = false;
                            if (libraryNameTokens.Length <= fileTokens.Length)
                            {
                                result = true;
                                for (int i = 0; i < libraryNameTokens.Length; i++)
                                {
                                    if (!string.Equals(libraryNameTokens[i], fileTokens[i], StringComparison.OrdinalIgnoreCase))
                                    {
                                        result = false;
                                    }
                                }
                            }
                            return result;
                        }

                        bool isFileNameOK = false;
                        string fileName = deployableFile.Name;
                        string culture = "";
                        string[] fileTokens = fileName.Split('_');
                        if (fileTokens.Length == libraryNameTokens.Length + 1)
                        {
                            // format is Libname_FBFun
                            if (checkLibName(fileTokens) && fileTokens[fileTokens.Length - 1].Equals(helpFileSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                isFileNameOK = true;
                            }
                        }
                        else if (fileTokens.Length == libraryNameTokens.Length + 2)
                        {
                            // format is LibName_culture_FBFun where the culture can be of one part (like "en", "de") or of more than one part (like "en-US", "de-DE")
                            if (checkLibName(fileTokens) && fileTokens[fileTokens.Length - 1].Equals(helpFileSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                culture = fileTokens[fileTokens.Length - 2];

                                if (usedCultures.Contains(culture))
                                {
                                    executionContext.WriteError($"The culture {culture} is used more than once");
                                    return;
                                }

                                usedCultures.Add(culture);

                                isFileNameOK = true;
                            }
                        }

                        if (!isFileNameOK)
                        {
                            executionContext.WriteError($"{fileName} has illegal help file format. Help files must have the format <library name>[_culture]_{helpFileSuffix}");
                            return;
                        }

                        string basePath = string.IsNullOrEmpty(culture) ? 
                                            Path.Combine(destinationDirectory.FullName, Constants.DeployHelpDirectoryName) :
                                            Path.Combine(destinationDirectory.FullName, Constants.DeployHelpDirectoryName, culture);

                        VirtualFile destination = fileSystem.GetFile(standardHelpFileName, basePath);

                        using (Stream source = deployableFile.OpenRead(true))
                        using (Stream dest = destination.OpenWrite())
                        {
                            dest.SetLength(0);
                            source.CopyTo(dest);
                        }

                        executionContext.WriteVerbose($"Deployed file {relativePath} to {destination.FullName}.");
                    }
                }
            }

            string FindCSharpProjectOutputFolder()
            {
                MSBuildData msbuild = msBuildFinder.FindMSBuild(dataModel.Root);

                return MSBuildProjectInformationProvider.GetCSharpProjectOutputPath(processManager, log, csharpProjectPath, msbuild);
            }
        }
    }
}
