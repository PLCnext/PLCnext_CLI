﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Generate;
using PlcNext.Common.Project;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using Entity = PlcNext.Common.DataModel.Entity;

namespace PlcNext.Common.Templates
{
    internal class TemplateFileGenerator : ITemplateFileGenerator
    {
        private readonly ITemplateResolver resolver;
        private readonly ITemplateRepository repository;
        private readonly IFileSystem fileSystem;
        private readonly ExecutionContext executionContext;
        private readonly FileGenerationHelper generationHelper;

        public TemplateFileGenerator(ITemplateResolver resolver, ITemplateRepository repository, IFileSystem fileSystem, ExecutionContext executionContext)
        {
            this.resolver = resolver;
            this.repository = repository;
            this.fileSystem = fileSystem;
            this.executionContext = executionContext;
            generationHelper = new FileGenerationHelper(resolver, repository, fileSystem);
        }

        public async Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable)
        {
            bool forced = dataModel.Value<CommandDefinition>()
                .Argument<BoolArgument>(TemplateCommandBuilder.ForcedArgumentName)
                .Value;
            bool empty = dataModel.Value<CommandDefinition>()
                .Argument<BoolArgument>(TemplateCommandBuilder.EmptyArgumentName)
                .Value;
            TemplateDescription template = dataModel.Template();
            CheckCompatibility(template, dataModel);

            List<VirtualFile> generatedFiles = new List<VirtualFile>(await InitializeFiles().ConfigureAwait(false));
            List<VirtualDirectory> generatedDirectories = new List<VirtualDirectory>(await InitializeDirectories().ConfigureAwait(false));
            generatedFiles.AddRange(await InitializeSubTemplates().ConfigureAwait(false));
            
            Exception e = dataModel.GetCodeExceptions();
            if (e != null)
            {
                e.CompleteCodeExceptions(fileSystem.GetDirectory(dataModel.Root.Path));
                throw e;
            }

            return generatedFiles;

            async Task<IEnumerable<VirtualFile>> InitializeFiles()
            {
                string basePath = dataModel.Root.Path;
                HashSet<VirtualFile> files = new HashSet<VirtualFile>();

                foreach (templateFile file in template.File.Where(f => !f.excluded && (!empty || !f.excludedForEmptyTemplate)))
                {
                    (string content, Encoding encoding) = await generationHelper.GetResolvedTemplateContent(dataModel, file, template).ConfigureAwait(false);

                    if (Equals(content, default) && Equals(encoding,default))
                    {
                        //skip file
                        executionContext.WriteWarning($"The template file {file.template} of the template {template.name} was not found. Creation will be skipped.");
                        continue;
                    }

                    VirtualFile destination = await GetFile(dataModel, file, forced, basePath, template).ConfigureAwait(false);
                    observable.OnNext(new Change(() => destination.Delete(),
                                                 $"Create file {Path.GetFileName(destination.Name)} for template " +
                                                 $"{template.name} in {destination.Parent.FullName}."));

                    using (Stream fileStream = destination.OpenWrite())
                    using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                    {
                        fileStream.SetLength(0);
                        await writer.WriteAsync(content).ConfigureAwait(false);
                    }

                    files.Add(destination);
                }

                return files;
            }
            async Task<IEnumerable<VirtualDirectory>> InitializeDirectories()
            {
                string basePath = dataModel.Root.Path;
                HashSet<VirtualDirectory> directories = new HashSet<VirtualDirectory>();
                if (template.Folder == null) return directories;
                
                foreach (templateDirectory folder in template.Folder.Where(f => !f.excluded))
                {
                    VirtualDirectory destination =
                        await GetDirectory(dataModel, folder, basePath).ConfigureAwait(false);
                    observable.OnNext(new Change(() => destination.DeleteIfEmpty(),
                        $"Create directory {Path.GetFileName(destination.Name)} for template " +
                        $"{template.name} in {destination.Parent.FullName}."));
                    directories.Add(destination);
                }

                return directories;
            }

            async Task<IEnumerable<VirtualFile>> InitializeSubTemplates()
            {
                List<VirtualFile> files = new List<VirtualFile>();

                foreach (templateReference reference in SortByRelationship(
                             template.AddTemplate?.Where(tmpl=> !tmpl.excluded)?? []))
                {
                    TemplateDescription templateDescription = repository.Template(reference.template);
                    if (templateDescription == null)
                    {
                        throw new TemplateReferenceNotDefinedException(reference.template);
                    }

                    CommandDefinitionBuilder pseudoDefinition = CommandDefinitionBuilder.Create()
                                                                                       .SetName(reference.template);
                    foreach (templateArgumentInstance argumentInstance in reference.Arguments)
                    {
                        AddArgument(argumentInstance, pseudoDefinition, templateDescription);
                    }

                    IEnumerable<IGrouping<string, templateRelationshipInstance>> grouped = (reference.Relationship ?? Enumerable.Empty<templateRelationshipInstance>()).GroupBy(r => r.name);
                    foreach (IGrouping<string, templateRelationshipInstance> relationshipInstances in grouped)
                    {
                        AddRelationships(relationshipInstances, pseudoDefinition, reference);
                    }

                    pseudoDefinition.CreateArgument()
                                   .SetName(TemplateCommandBuilder.ForcedArgumentName)
                                   .SetValue(forced)
                                   .Build();
                    pseudoDefinition.CreateArgument()
                        .SetName(TemplateCommandBuilder.EmptyArgumentName)
                        .SetValue(empty)
                        .Build();
                    Entity referencedTemplateEntity = dataModel.Create(reference.template, pseudoDefinition.Build());
                    dataModel.AddEntity(referencedTemplateEntity);
                    files.AddRange(await InitalizeTemplate(referencedTemplateEntity, observable).ConfigureAwait(false));
                }

                return files;

                void AddArgument(templateArgumentInstance templateArgumentInstance,
                    CommandDefinitionBuilder commandDefinitionBuilder, TemplateDescription templateDescription)
                {
                    var argumentDefinition = templateDescription.Arguments.FirstOrDefault(arg => arg.name == templateArgumentInstance.name);
                    string templateArgumentValue = resolver.Resolve(templateArgumentInstance.value, dataModel);
                    bool argumentHasNoValue;
                    bool boolValue = false;
                    bool isMultiArgument;
                    string[] templateArgumentValues;
                    string separator = "|";
                    if (argumentDefinition == null)
                    {
                        argumentHasNoValue = bool.TryParse(templateArgumentValue, out boolValue);
                        templateArgumentValues = templateArgumentValue?.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                        isMultiArgument = templateArgumentValues?.Length > 1;
                    }
                    else
                    {
                        argumentHasNoValue = !argumentDefinition.hasvalue;
                        if (argumentHasNoValue)
                        {
                            if (!bool.TryParse(templateArgumentValue, out boolValue))
                            {
                                boolValue = false;
                            }
                        }
                        isMultiArgument  = argumentDefinition.multiplicity == multiplicity.OneOrMore;
                        templateArgumentValues = new [] { templateArgumentValue };
                        separator = argumentDefinition.separator;
                        if (isMultiArgument)
                        {
                            templateArgumentValues = templateArgumentValue?.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                    
                    if (argumentHasNoValue)
                    {
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(templateArgumentInstance.name)
                                                .SetValue(boolValue)
                                                .Build();
                    }
                    else if (isMultiArgument)
                    {
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(templateArgumentInstance.name)
                                                .SetValue(templateArgumentValues)
                                                .Build();
                    }
                    else
                    {
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(templateArgumentInstance.name)
                                                .SetValue(templateArgumentValue)
                                                .Build();
                    }
                }

                void AddRelationships(IGrouping<string, templateRelationshipInstance> relationshipInstances,
                                      CommandDefinitionBuilder commandDefinitionBuilder, templateReference reference)
                {
                    templateRelationship relationshipDefinition = repository.Template(reference.template).Relationship
                                                                          ?.FirstOrDefault(r => r.name ==
                                                                                                relationshipInstances.Key);
                    if (relationshipDefinition == null)
                    {
                        throw new TemplateRelationshipNotFoundException(reference.template, relationshipInstances.Key);
                    }

                    bool multipleValues = relationshipDefinition.multiplicity == multiplicity.OneOrMore;
                    if (multipleValues)
                    {
                        string[] relationships = relationshipInstances.Select(r => resolver.Resolve(r.value, dataModel))
                                                                      .ToArray();
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(relationshipInstances.Key)
                                                .SetValue(relationships)
                                                .Build();
                    }
                    else
                    {
                        if (relationshipInstances.Count() != 1)
                        {
                            throw new RelationshipMultiplicityMismatchException(relationshipInstances.Key,reference.template);
                        }
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(relationshipInstances.Key)
                                                .SetValue(resolver.Resolve(relationshipInstances.Single().value, dataModel))
                                                .Build();
                    }
                }

                IEnumerable<templateReference> SortByRelationship(IEnumerable<templateReference> references)
                {
                    List<templateReference> unsorted = references.ToList();
                    List<templateReference> sorted = new List<templateReference>();

                    while (unsorted.Any())
                    {
                        Insert(unsorted[0]);
                    }

                    return sorted;

                    void Insert(templateReference current, CycleChecker<templateReference> cycleChecker = null)
                    {
                        using (cycleChecker = cycleChecker?.SpawnChild() ?? new CycleChecker<templateReference>(
                                                  ExceptionTexts.TemplateRelationshipCycle,
                                                  () => cycleChecker = null))
                        {
                            cycleChecker.AddItem(current);
                            List<templateReference> dependent = new List<templateReference>();
                            foreach (templateRelationshipInstance relationshipInstance in current.Relationship ??
                                                                                          Enumerable
                                                                                             .Empty<
                                                                                                  templateRelationshipInstance
                                                                                              >())
                            {
                                templateReference reference =
                                    unsorted.FirstOrDefault(r => HasRelationship(r, relationshipInstance));
                                if (reference != null)
                                {
                                    Insert(reference, cycleChecker);
                                }
                                else
                                {
                                    reference = sorted.FirstOrDefault(r => HasRelationship(r, relationshipInstance));
                                }

                                if (reference != null)
                                {
                                    dependent.Add(reference);
                                }
                            }

                            int skipping = dependent.Any() ? dependent.Select(d => sorted.IndexOf(d)).Max() : -1;
                            int index = skipping + 1;
                            sorted.Insert(index, current);
                            unsorted.Remove(current);

                            bool HasRelationship(templateReference currentReference,
                                                 templateRelationshipInstance relationshipInstance)
                            {
                                templateArgumentInstance instance = currentReference.Arguments
                                                                                    .FirstOrDefault(a => a.name
                                                                                                          .Equals(
                                                                                                               EntityKeys
                                                                                                                  .NameKey,
                                                                                                               StringComparison
                                                                                                                  .OrdinalIgnoreCase));
                                return instance?.value?.Equals(relationshipInstance.value,
                                                               StringComparison.OrdinalIgnoreCase) == true;
                            }
                        }
                    }
                }
            }
        }

        private void CheckCompatibility(TemplateDescription template, Entity dataModel)
        {
            IEnumerable<TemplateDescription> roots = FindCompatibleRoots();
            TemplateEntity templateEntity = TemplateEntity.Decorate(dataModel.Root);
            List<TemplateDescription> dataModelTemplates = GetTemplateHierarchy();
            if (templateEntity.HasTemplate &&
                !roots.Any(dataModelTemplates.Contains))
            {
                throw new TemplateIncompatibleException(template.name, templateEntity.Template.name);
            }

            List<TemplateDescription> GetTemplateHierarchy()
            {
                List<TemplateDescription> result = new List<TemplateDescription>();
                if (!templateEntity.HasTemplate)
                {
                    return result;
                }

                TemplateDescription current = templateEntity.Template;
                result.Add(current);
                while (!string.IsNullOrEmpty(current?.basedOn))
                {
                    current = repository.Template(current.basedOn);
                    result.Add(current);
                }

                return result;
            }

            IEnumerable<TemplateDescription> FindCompatibleRoots()
            {
                List<TemplateDescription> unvisited = new List<TemplateDescription>(new []{template});
                List<TemplateDescription> visited = new List<TemplateDescription>();
                List<TemplateDescription> result = new List<TemplateDescription>();
                while (unvisited.Except(visited).Any())
                {
                    TemplateDescription visiting = unvisited.Except(visited).First();
                    visited.Add(visiting);
                    if (visiting.isRoot)
                    {
                        result.Add(visiting);
                    }
                    else
                    {
                        unvisited.AddRange(visiting.Relationship
                                                   .Where(relationship => repository.Template(relationship.type) != null)
                                                   .Select(relationship => repository.Template(relationship.type)));
                    }
                }

                return result.Distinct();
            }
        }

        private async Task<VirtualFile> GetFile(Entity dataModel, templateFile file, bool forced, string basePath,
            TemplateDescription template)
        {
            string path = await resolver.ResolveAsync(file.path ?? string.Empty, dataModel).ConfigureAwait(false);
            string name = await resolver.ResolveAsync(file.name, dataModel).ConfigureAwait(false);
            if (!forced && fileSystem.FileExists(Path.Combine(path, name), basePath))
            {
                throw new FileExistsException(name, template.name);
            }

            VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), basePath);
            return destination;
        }

        private async Task<VirtualDirectory> GetDirectory(Entity dataModel, templateDirectory file, string basePath)
        {
            string path = await resolver.ResolveAsync(file.path ?? string.Empty, dataModel).ConfigureAwait(false);
            string name = await resolver.ResolveAsync(file.name, dataModel).ConfigureAwait(false);
            VirtualDirectory destination = fileSystem.GetDirectory(Path.Combine(path, name), basePath, true);
            return destination;
        }
        
        public async Task GenerateFiles(Entity rootEntity, string generator, string output, bool outputDefined,
                                        ChangeObservable observable)
        {
            IEnumerable<Entity> generatableEntities = rootEntity.Hierarchy();
            HashSet<VirtualFile> generatedFiles = new HashSet<VirtualFile>();
            HashSet<VirtualDirectory> rootDirectories = new HashSet<VirtualDirectory>();
            foreach (Entity generatableEntity in generatableEntities)
            {
                await GenerateFilesForEntity(generatableEntity).ConfigureAwait(false);
            }

            Exception e = rootEntity.GetCodeExceptions();
            if (e != null)
            {
                e.CompleteCodeExceptions(fileSystem.GetDirectory(rootEntity.Path));
                throw e;
            }

            foreach (VirtualDirectory rootDirectory in rootDirectories)
            {
                DeleteRedundantFiles(rootDirectory);
            }

            void DeleteRedundantFiles(VirtualDirectory rootDirectory)
            {
                foreach (VirtualFile file in rootDirectory.Files(searchRecursive:true).Where(f => !generatedFiles.Contains(f)).ToArray())
                {
                    VirtualDirectory current = file.Parent;
                    file.Delete();
                    while (current != null && !current.Entries.Any())
                    {
                        VirtualDirectory next = current.Parent;
                        current.Delete();
                        current = next;
                    }
                }
            }

            async Task GenerateFilesForEntity(Entity generatableEntity)
            {
                TemplateDescription template = generatableEntity.Template();
                if (template == null)
                {
                    return;
                }

                foreach (templateGeneratedFile file in template.GeneratedFile?.Where(f => !f.excluded)
                                                       ??Enumerable.Empty<templateGeneratedFile>())
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

                    if (Equals(content, default) && Equals(encoding,default))
                    {
                        //skip file
                        executionContext.WriteWarning($"The template file {file.template} of the template {template.name} was not found. Creation will be skipped.");
                        continue;
                    }
                    
                    VirtualDirectory baseDirectory = GetBaseDirectory(file);
                    VirtualFile destination = await GetFile(generatableEntity, file, true, baseDirectory.FullName, template).ConfigureAwait(false);
                    rootDirectories.Add(baseDirectory);
                    generatedFiles.Add(destination);

                    observable.OnNext(new Change(() => destination.Restore(), $"Generated the file {destination.FullName}."));

                    using (Stream fileStream = destination.OpenComparingWriteStream())
                    using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                    {
                        await writer.WriteAsync(content).ConfigureAwait(false);
                    }
                }

                VirtualDirectory GetBaseDirectory(templateGeneratedFile file)
                {
                    string generatorPath = output;
                    if (!outputDefined)
                    {
                        generatorPath = Path.Combine(Constants.IntermediateFolderName, file.generator.ToLowerInvariant());
                    }
                    else if(generator == "all")
                    {
                        generatorPath = Path.Combine(generatorPath, file.generator.ToLowerInvariant());
                    }
                    return fileSystem.GetDirectory(generatorPath, rootEntity.Path);
                }

                bool CheckProjectVersionRestrictions(templateGeneratedFile file)
                {
                    if (!string.IsNullOrEmpty(file.maxversion))
                    {
                        if (!string.IsNullOrEmpty(file.minversion) || !string.IsNullOrEmpty(file.equalsversion))
                        {
                            throw new FormattableException($"Error in template {template.name}: " +
                                "Only one of the attributes minversion, maxversion, equalsversion can be used per file. ");
                        }

                        ProjectEntity project = ProjectEntity.Decorate(generatableEntity.IsRoot() ? generatableEntity : generatableEntity.Root);
                        if (!project.Settings.IsPersistent || project.Version > new Version(file.maxversion))
                        {
                            return false;
                        }
                        return true;
                    }

                    if (!string.IsNullOrEmpty(file.minversion))
                    {
                        if (!string.IsNullOrEmpty(file.equalsversion))
                        {
                            throw new FormattableException($"Error in template {template.name}: " +
                                "Only one of the attributes minversion, maxversion, equalsversion can be used per file. ");
                        }
                        ProjectEntity project = ProjectEntity.Decorate(generatableEntity.IsRoot() ? generatableEntity : generatableEntity.Root);
                        if (project.Settings.IsPersistent && project.Version < new Version(file.minversion))
                        {
                            return false;
                        }
                        return true;
                    }

                    if (!string.IsNullOrEmpty(file.equalsversion))
                    {
                        ProjectEntity project = ProjectEntity.Decorate(generatableEntity.IsRoot() ? generatableEntity : generatableEntity.Root);
                        if (!project.Settings.IsPersistent || project.Version != new Version(file.equalsversion))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
    }
}
