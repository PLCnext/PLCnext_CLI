#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac.Features.AttributeFilters;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.MSBuild;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Generate
{
    internal class SharedNativeGenerateEngine : ITemplateFileGenerator
    {
        private readonly ITemplateResolver resolver;
        private readonly ITemplateFileGenerator templateFileGenerator;
        private readonly IFileSystem fileSystem;
        private readonly IProcessManager processManager;
        private readonly ExecutionContext executionContext;
        private readonly ILog log;
        private readonly FileGenerationHelper generationHelper;
        private readonly IMSBuildFinder msBuildFinder;

        public SharedNativeGenerateEngine([KeyFilter("DefaultGenerateEngine")] ITemplateFileGenerator templateFileGenerator, 
                                          IFileSystem fileSystem,
                                          IProcessManager processManager, ExecutionContext executionContext, ILog log,
                                          ITemplateResolver resolver, ITemplateRepository repository, IMSBuildFinder msBuildFinder)
        {
            this.templateFileGenerator = templateFileGenerator;
            this.fileSystem = fileSystem;
            this.processManager = processManager;
            this.executionContext = executionContext;
            this.log = log;
            this.resolver = resolver;
            this.msBuildFinder = msBuildFinder;

            generationHelper = new FileGenerationHelper(resolver, repository, fileSystem);
        }
        public Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable)
        {
            return templateFileGenerator.InitalizeTemplate(dataModel, observable);
        }

        public async Task GenerateFiles(Entity rootEntity, string generator, string output, bool outputDefined, ChangeObservable observable)
        {
            await GenerateTemplateFiles(rootEntity).ConfigureAwait(false);

            if (generator != "all" && generator != "code")
            {
                return;
            }

            ProjectEntity project = ProjectEntity.Decorate(rootEntity);
            string csharpProjectPath = project.CSharpProjectPath;
            if (!Path.IsPathRooted(csharpProjectPath))
            {
                if (csharpProjectPath == null)
                {
                    csharpProjectPath = project.Path;
                    log.LogWarning("CSharp project path not found. Using project path instead.");
                }
                else
                {
                    csharpProjectPath = Path.Combine(project.Path, csharpProjectPath);
                }
            }

            MSBuildData msbuild = msBuildFinder.FindMSBuild(rootEntity);
            string niBuilderOutputPath = MSBuildProjectInformationProvider.GetCSharpProjectOutputPath(processManager, log, csharpProjectPath, msbuild);

            IEnumerable<VirtualFile> outputFolderFiles = Enumerable.Empty<VirtualFile>();
            if (fileSystem.DirectoryExists(niBuilderOutputPath, csharpProjectPath))
            {
                outputFolderFiles = fileSystem.GetDirectory(niBuilderOutputPath, csharpProjectPath).Files();
            }
            Dictionary<VirtualFile, DateTime> filesBefore = outputFolderFiles.ToDictionary(f => f, f => f.LastWriteTime);
                

            await BuildCSharpProject(rootEntity, csharpProjectPath, project.Path, msbuild).ConfigureAwait(false);

            CopyNIBuilderFilesToCpp();

            async Task GenerateTemplateFiles(Entity generatableEntity)
            {
                TemplateDescription template = generatableEntity.Template();
                if (template == null)
                {
                    return;
                }

                foreach (templateGeneratedFile file in template.GeneratedFile?.Where(f => !f.excluded)
                                                       ?? Enumerable.Empty<templateGeneratedFile>())
                {
                    if (generator != "all" && !file.generator.Equals(generator, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(file.condition))
                    {
                        string conditionString = await resolver.ResolveAsync(file.condition, generatableEntity).ConfigureAwait(false);
                        bool condition = conditionString.ResolveCondition();
                        if (!condition)
                        {
                            continue;
                        }
                    }

                    if (!CheckProjectVersionRestrictions(file))
                        continue;

                    (string content, Encoding encoding) = await generationHelper.GetResolvedTemplateContent(generatableEntity, file, template).ConfigureAwait(false);

                    if (Equals(content, default) && Equals(encoding, default))
                    {
                        //skip file
                        executionContext.WriteWarning($"The template file {file.template} of the template {template.name} was not found. Creation will be skipped.");
                        continue;
                    }

                    VirtualDirectory baseDirectory = GetBaseDirectory(file.generator);
                    string path = await resolver.ResolveAsync(file.path ?? string.Empty, generatableEntity).ConfigureAwait(false);
                    string name = await resolver.ResolveAsync(file.name, generatableEntity).ConfigureAwait(false);
                    if (fileSystem.FileExists(Path.Combine(path, name), baseDirectory.FullName))
                    {
                        executionContext.WriteVerbose($"The template file {file.template} of the template {template.name} exists already. Creation will be skipped.");
                        continue;
                    }

                    VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), baseDirectory.FullName);
                    
                    observable.OnNext(new Change(() => destination.Restore(), $"Generated the file {destination.FullName}."));

                    using (Stream fileStream = destination.OpenComparingWriteStream())
                    using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                    {
                        await writer.WriteAsync(content).ConfigureAwait(false);
                    }
                }
                bool CheckProjectVersionRestrictions(templateGeneratedFile file)
                {
                    if (!string.IsNullOrEmpty(file.maxversion)
                        || !string.IsNullOrEmpty(file.minversion)
                        || !string.IsNullOrEmpty(file.equalsversion))
                    {
                        throw new NotImplementedException();
                    }
                    return true;
                }
            }
            
            VirtualDirectory GetBaseDirectory(string generator)
            {
                string generatorPath = output;
                if (!outputDefined)
                {
                    generatorPath = Path.Combine(Constants.IntermediateFolderName, generator.ToLowerInvariant());
                }
                else if (generator == "all")
                {
                    generatorPath = Path.Combine(generatorPath, generator.ToLowerInvariant());
                }
                return fileSystem.GetDirectory(generatorPath, rootEntity.Path);
            }

            void CopyNIBuilderFilesToCpp()
            {
                VirtualDirectory baseDirectory = GetBaseDirectory("code");
                VirtualDirectory sourceDirectory = fileSystem.GetDirectory(Constants.SourceFolderName, project.Path);
                VirtualDirectory niBuilderOutputDirectory = fileSystem.GetDirectory(niBuilderOutputPath, csharpProjectPath, createNew: false);

                IEnumerable<VirtualFile> filesAfter = niBuilderOutputDirectory?.Files() ?? Enumerable.Empty<VirtualFile>();

                RenameLibraryFile();

                IEnumerable <VirtualFile> newFiles = filesAfter.Where(f => !filesBefore.Keys.Select(file => file.Name).Contains(f.Name) 
                                                                          || filesBefore[filesBefore.Keys.Where(file => file.Name == f.Name).FirstOrDefault()].CompareTo(f.LastWriteTime) != 0)
                                                                .Where(f => f.Name.EndsWith(".cpp", StringComparison.InvariantCultureIgnoreCase) 
                                                                            || f.Name.EndsWith(".hpp", StringComparison.InvariantCultureIgnoreCase)
                                                                            || f.Name.EndsWith(".h", StringComparison.InvariantCultureIgnoreCase));

                List<VirtualFile> copiedFiles = new();

                CopyFilesToOverwrite();
                CopyFilesToNotOverwrite();
                WarnIfFilesFoundWhichShouldBeDeleted();

                void RenameLibraryFile()
                {
                    string oldName = $"{project.Name}-template3.h";
                    string newName = $"{project.Name}-template32.h";
                    VirtualFile fileToRename = filesAfter.Where(file => file.Name == oldName).FirstOrDefault();
                    if (fileToRename == null || !fileToRename.Exists)
                    {
                        return;
                    }

                    VirtualFile newFile = niBuilderOutputDirectory.File(newName);
                    
                    using (Stream sourceStream = fileToRename.OpenRead())
                    using (Stream destinationStream = newFile.OpenWrite())
                    {
                        destinationStream.SetLength(0);
                        sourceStream.CopyTo(destinationStream);
                    }
                    newFile.Touch();
                    observable.OnNext(new Change(() => newFile.Delete(), $"Renamed the file {newFile.FullName}."));

                    VirtualFile file = niBuilderOutputDirectory.File(oldName);
                    file.Delete();
                    observable.OnNext(new Change(() => file.Restore(), $"Deleted the file {file.FullName}."));
                    filesAfter = filesAfter.Where(f => f.Name != newFile.Name).Concat(new[] { newFile });
                }

                void CopyFilesToOverwrite()
                {
                    string fileName_Version = $"{project.Name}-version.h"; 
                    string fileName_cli3 = $"{project.Name}-cli3.cpp";
                    string fileName_cli32 = $"{project.Name}-cli32.cpp";
                    string fileName_cli64 = $"{project.Name}-cli64.cpp";

                    CopyOverwritableFile(fileName_Version, fileName_Version);
                    CopyOverwritableFile(fileName_cli3, fileName_cli32);
                    CopyOverwritableFile(fileName_cli64, fileName_cli64);
                    
                    void CopyOverwritableFile(string fileName, string newFileName)
                    {
                        VirtualFile source = filesAfter.Where(f => f.Name == fileName).FirstOrDefault();
                        if (source != null)
                        {
                            if (baseDirectory.FileExists(newFileName)) 
                            {
                                VirtualFile file = baseDirectory.File(newFileName);
                                file.Delete();
                                observable.OnNext(new Change(() => file.Restore(), $"Deleted the file {file.FullName}."));
                            }

                            VirtualFile newFile = baseDirectory.File(newFileName);
                            using (Stream sourceStream = source.OpenRead())
                            using (Stream destinationStream = newFile.OpenWrite())
                            {
                                destinationStream.SetLength(0);
                                sourceStream.CopyTo(destinationStream);
                            }
                            observable.OnNext(new Change(() => newFile.Delete(), $"Generated the file {newFile.FullName}."));
                            copiedFiles.Add(source);
                        }
                    }
                }

                void CopyFilesToNotOverwrite() 
                {
                    foreach (VirtualFile file in newFiles.Where(file => !copiedFiles.Contains(file)))
                    {
                        string newFileName = RemoveTemplateFromFileName(file.Name);
                        CopyNotOverwritableFile(file.Name, newFileName);
                    }

                    void CopyNotOverwritableFile(string fileName, string newFileName)
                    {
                        VirtualFile source = filesAfter.Where(f => f.Name == fileName).FirstOrDefault();
                        if (source != null && !sourceDirectory.FileExists(newFileName))
                        {
                            VirtualFile newFile = sourceDirectory.File(newFileName);
                            using (Stream sourceStream = source.OpenRead())
                            using (Stream destinationStream = newFile.OpenWrite())
                            {
                                destinationStream.SetLength(0);
                                sourceStream.CopyTo(destinationStream);
                            }
                            observable.OnNext(new Change(() => newFile.Delete(), $"Generated the file {newFile.FullName}."));
                        }
                        else if(source != null)
                        {
                            executionContext.WriteVerbose($"The file {newFileName} exists already. Copying will be skipped.");
                            if (CheckForUpdateNecessary(source, sourceDirectory.File(newFileName)))
                            {
                                executionContext.WriteWarning($"The file {newFileName} differs from " +
                                    $"{source.FullName.GetRelativePath(Path.Combine(csharpProjectPath, ".."))} and needs to be updated manually.");
                            }
                        }
                    }
                }

                string RemoveTemplateFromFileName(string fileName)
                {
                    int startIndex = fileName.LastIndexOf("-template", StringComparison.InvariantCultureIgnoreCase);
                    int length = 9;
                    if (startIndex > 0)
                    {
                        fileName = fileName.Remove(startIndex, length);
                        fileName = fileName.Insert(startIndex, "-cli");
                        return fileName;
                    }
                    return fileName;
                }

                bool CheckForUpdateNecessary(VirtualFile fileA, VirtualFile fileB)
                {
                    using (Stream fileStreamA = fileA.OpenRead())
                    using (StreamReader readerA = new StreamReader(fileStreamA))
                    using (Stream fileStreamB = fileB.OpenRead())
                    using (StreamReader readerB = new StreamReader(fileStreamB))
                    {
                        while (!readerA.EndOfStream)
                        {
                            string contentA = readerA.ReadLine();
                            string contentB = readerB.EndOfStream ? string.Empty : readerB.ReadLine();
                            if (contentA != contentB)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                void WarnIfFilesFoundWhichShouldBeDeleted() 
                {
                    TemplateEntity template = TemplateEntity.Decorate(rootEntity);
                    IEnumerable<CodeEntity> entities = template.EntityHierarchy.Select(e =>
                    {
                        CodeEntity codeEntity = CodeEntity.Decorate(e);
                        return codeEntity;
                    }).Where(entity => !entity.Type.Contains("project", StringComparison.Ordinal));
                    
                    foreach (CodeEntity codeEntity in entities)
                    {
                        if (!newFiles.Where(file => Path.GetFileNameWithoutExtension(file.Name) == codeEntity.Name+"-template").Any())
                        {
                            executionContext.WriteWarning($"The entity {codeEntity.Name} is no longer available in the c# counterpart and probably can be deleted.");
                        }
                    }
                }
            }
        }

        private async Task BuildCSharpProject(Entity entity, string csharpProjectPath, string cppProjectPath, MSBuildData msbuild)
        {
            CommandEntity command = CommandEntity.Decorate(entity.Origin);
            string configuration = command.GetSingleValueArgument(EntityKeys.BuildTypeKey);
            string arguments = string.IsNullOrEmpty(configuration) ? $"\"{csharpProjectPath}\"" : $" -v:normal -p:Configuration={configuration};CppProjectPath=\"{cppProjectPath}\" \"{csharpProjectPath}\"";
            arguments = (string.IsNullOrEmpty(msbuild.Parameters) ? string.Empty : msbuild.Parameters + " ")+arguments;
            using (IProcess process = processManager.StartProcess(msbuild.FullName, arguments, executionContext))
            {
                await process.WaitForExitAsync().ConfigureAwait(false);
                if (process.ExitCode != 0)
                {
                    throw new FormattableException("Generate step failed! See log for details.");
                }
            }
        }
    }
}
