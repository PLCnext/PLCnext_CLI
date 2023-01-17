#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac.Features.AttributeFilters;
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlcNext.Common.Generate
{
    internal class SharedNativeGenerateEngine : ITemplateFileGenerator
    {
        private readonly ITemplateFileGenerator templateFileGenerator;
        private readonly IBinariesLocator binariesLocator;
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;
        private readonly ISettingsProvider settingsProvider;
        private readonly IProcessManager processManager;
        private readonly ExecutionContext executionContext;
        private readonly ILog log;

        public SharedNativeGenerateEngine([KeyFilter("DefaultGenerateEngine")] ITemplateFileGenerator templateFileGenerator, 
                                          IBinariesLocator binariesLocator, IFileSystem fileSystem,
                                          IEnvironmentService environmentService, ISettingsProvider settingsProvider,
                                          IProcessManager processManager, ExecutionContext executionContext, ILog log)
        {
            this.templateFileGenerator = templateFileGenerator;
            this.binariesLocator = binariesLocator;
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.settingsProvider = settingsProvider;
            this.processManager = processManager;
            this.executionContext = executionContext;
            this.log = log;
        }
        public Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable)
        {
            return templateFileGenerator.InitalizeTemplate(dataModel, observable);
        }

        public async Task GenerateFiles(Entity rootEntity, string generator, string output, bool outputDefined, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(rootEntity);
            string projectPath = project.CSharpProjectPath;
            if (!Path.IsPathRooted(projectPath))
            {
                projectPath = Path.Combine(project.Path, projectPath);
            }

            await BuildCSharpProject(rootEntity, projectPath, project.Path).ConfigureAwait(false);
            
            //await GenerateCppFiles(projectPath).ConfigureAwait(false);
            //SyncFiles();

        }

        private async Task BuildCSharpProject(Entity entity, string csharpProjectPath, string cppProjectPath)
        {
            string msbuildLocation = MSBuildFinder.FindMsBuild(environmentService, fileSystem, settingsProvider, binariesLocator, entity);

            StringBuilderUserInterface userInterface = new StringBuilderUserInterface(log, writeInformation: true, writeError: true);
            string arguments = $"-target:GetEclrVersion \"{csharpProjectPath}\"";
            using (IProcess process = processManager.StartProcess(msbuildLocation, arguments, userInterface))
            {
                await process.WaitForExitAsync().ConfigureAwait(false);
                if (process.ExitCode == 0 && !EclrVersionIsSupported(userInterface.Information))
                {
                    throw new FormattableException("This version of the PLCnCLI supports only eCLR version 3.3.0");
                } 
            }

            CommandEntity command = CommandEntity.Decorate(entity.Origin);
            string configuration = command.GetSingleValueArgument("buildtype");
            arguments  = string.IsNullOrEmpty(configuration) ? $"\"{csharpProjectPath}\"" : $" -p:Configuration={configuration};CppProjectPath=\"{cppProjectPath}\" \"{csharpProjectPath}\"";
            using (IProcess process = processManager.StartProcess(msbuildLocation, arguments, executionContext, showOutput: false))
            {
                await process.WaitForExitAsync().ConfigureAwait(false);
                if (process.ExitCode != 0)
                {
                    throw new FormattableException("Generate step failed! See log for details.");
                }
            }
        }

        private bool EclrVersionIsSupported(string information)
        {
            Match match = Regex.Match(information, @"@begineclrversion(?<eclrversion>.*)@endeclrversion");
            if (match.Success)
            {
                return match.Groups["eclrversion"].Value.Equals("eCLR3.3", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
