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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
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

        public TemplateFileGenerator(ITemplateResolver resolver, ITemplateRepository repository, IFileSystem fileSystem)
        {
            this.resolver = resolver;
            this.repository = repository;
            this.fileSystem = fileSystem;
        }

        public async Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable)
        {
            bool forced = dataModel.Value<CommandDefinition>()
                                   .Argument<BoolArgument>(TemplateCommandBuilder.ForcedArgumentName)
                                   .Value;
            TemplateDescription template = dataModel.Template();

            List<VirtualFile> generatedFiles = new List<VirtualFile>(await InitalizeFiles());
            generatedFiles.AddRange(await InitalizeSubTemplates());
            
            Exception e = dataModel.GetCodeExceptions();
            if (e != null)
            {
                e.CompleteCodeExceptions(fileSystem.GetDirectory(dataModel.Root.Path));
                throw e;
            }

            return generatedFiles;

            async Task<IEnumerable<VirtualFile>> InitalizeFiles()
            {
                string basePath = dataModel.Root.Path;
                HashSet<VirtualFile> files = new HashSet<VirtualFile>();

                foreach (templateFile file in template.File)
                {
                    (string content, Encoding encoding) = await GetResolvedTemplateContent(dataModel, file, template);

                    VirtualFile destination = await GetFile(dataModel, file, forced, basePath, template);
                    observable.OnNext(new Change(() => destination.Restore(),
                                                 $"Create file {Path.GetFileName(destination.Name)} for template " +
                                                 $"{template.name} in {destination.Parent.FullName}."));

                    using (Stream fileStream = destination.OpenWrite())
                    using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                    {
                        fileStream.SetLength(0);
                        await writer.WriteAsync(content);
                    }

                    files.Add(destination);
                }

                return files;
            }

            async Task<IEnumerable<VirtualFile>> InitalizeSubTemplates()
            {
                List<VirtualFile> files = new List<VirtualFile>();

                foreach (templateReference reference in SortByRelationship(template.AddTemplate??Enumerable.Empty<templateReference>()))
                {
                    if (repository.Template(reference.template) == null)
                    {
                        throw new TemplateReferenceNotDefinedException(reference.template);
                    }

                    CommandDefinitionBuilder pseudoDefiniton = CommandDefinitionBuilder.Create()
                                                                                       .SetName(reference.template);
                    foreach (templateArgumentInstance argumentInstance in reference.Arguments)
                    {
                        AddArgument(argumentInstance, pseudoDefiniton);
                    }

                    IEnumerable<IGrouping<string, templateRelationshipInstance>> grouped = (reference.Relationship ?? Enumerable.Empty<templateRelationshipInstance>()).GroupBy(r => r.name);
                    foreach (IGrouping<string, templateRelationshipInstance> relationshipInstances in grouped)
                    {
                        AddRelationships(relationshipInstances, pseudoDefiniton, reference);
                    }

                    pseudoDefiniton.CreateArgument()
                                   .SetName(TemplateCommandBuilder.ForcedArgumentName)
                                   .SetValue(forced)
                                   .Build();
                    Entity referencedTemplateEntity = dataModel.Create(reference.template, pseudoDefiniton.Build());
                    dataModel.AddEntity(referencedTemplateEntity);
                    files.AddRange(await InitalizeTemplate(referencedTemplateEntity, observable));
                }

                return files;

                void AddArgument(templateArgumentInstance templateArgumentInstance, CommandDefinitionBuilder commandDefinitionBuilder)
                {
                    string tempalteArgumentValue = resolver.Resolve(templateArgumentInstance.value, dataModel);
                    bool argumentHasNoValue = bool.TryParse(tempalteArgumentValue, out bool boolValue);
                    string[] argumentSplit = tempalteArgumentValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    bool isMultiArgument = argumentSplit.Length > 1;
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
                                                .SetValue(argumentSplit)
                                                .Build();
                    }
                    else
                    {
                        commandDefinitionBuilder.CreateArgument()
                                                .SetName(templateArgumentInstance.name)
                                                .SetValue(tempalteArgumentValue)
                                                .Build();
                    }
                }

                void AddRelationships(IGrouping<string, templateRelationshipInstance> relationshipInstances,
                                      CommandDefinitionBuilder commandDefinitionBuilder, templateReference reference)
                {
                    templateRelationship relationshipDefintion = repository.Template(reference.template).Relationship
                                                                          ?.FirstOrDefault(r => r.name ==
                                                                                                relationshipInstances.Key);
                    if (relationshipDefintion == null)
                    {
                        throw new TemplateRelationshipNotFoundException(reference.template, relationshipInstances.Key);
                    }

                    bool multipleValues = relationshipDefintion.multiplicity == multiplicity.OneOrMore;
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
                        if (cycleChecker == null)
                        {
                            cycleChecker = new CycleChecker<templateReference>(ExceptionTexts.TemplateRelationshipCycle);
                        }

                        cycleChecker.AddItem(current);
                        List<templateReference> dependend = new List<templateReference>();
                        foreach (templateRelationshipInstance relationshipInstance in current.Relationship ?? Enumerable.Empty<templateRelationshipInstance>())
                        {
                            templateReference reference = unsorted.FirstOrDefault(r => HasRelationship(r, relationshipInstance));
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
                                dependend.Add(reference);
                            }
                        }

                        int skipping = dependend.Any() ? dependend.Select(d => sorted.IndexOf(d)).Max() : -1;
                        int index = skipping + 1;
                        sorted.Insert(index, current);
                        unsorted.Remove(current);

                        bool HasRelationship(templateReference currentReference, templateRelationshipInstance relationshipInstance)
                        {
                            templateArgumentInstance instance = currentReference.Arguments
                                                                                .FirstOrDefault(a => a.name
                                                                                                      .Equals(EntityKeys.NameKey,
                                                                                                              StringComparison.OrdinalIgnoreCase));
                            return instance?.value?.Equals(relationshipInstance.value) == true;
                        }
                    }
                }
            }
        }

        private async Task<(string content, Encoding encoding)> GetResolvedTemplateContent(Entity dataModel, templateFile file, TemplateDescription template)
        {
            string content;
            Encoding encoding;
            using (Stream fileStream = fileSystem.GetFile(file.template, repository.GetTemplateBase(template)).OpenRead())
            using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                encoding = reader.CurrentEncoding;
                content = await reader.ReadToEndAsync();
            }

            content = await resolver.ResolveAsync(content, dataModel);
            return (content, encoding);
        }

        private async Task<VirtualFile> GetFile(Entity dataModel, templateFile file, bool forced, string basePath,
                                   TemplateDescription template)
        {
            string path = await resolver.ResolveAsync(file.path ?? string.Empty, dataModel);
            string name = await resolver.ResolveAsync(file.name, dataModel);
            if (!forced && fileSystem.FileExists(Path.Combine(path, name), basePath))
            {
                throw new FileExistsException(name, template.name);
            }

            VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), basePath);
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
                await GenerateFilesForEntity(generatableEntity);
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

                foreach (templateGeneratedFile file in template.GeneratedFile)
                {
                    if (generator != "all" && !file.generator.Equals(generator, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    (string content, Encoding encoding) = await GetResolvedTemplateContent(generatableEntity, file, template);

                    VirtualDirectory baseDirectory = GetBaseDirectory(file);
                    VirtualFile destination = await GetFile(generatableEntity, file, true, baseDirectory.FullName, template);
                    rootDirectories.Add(baseDirectory);
                    generatedFiles.Add(destination);

                    observable.OnNext(new Change(() => destination.Restore(), $"Generated the file {destination.FullName}."));

                    using (Stream fileStream = destination.OpenComparingWriteStream())
                    using (StreamWriter writer = new StreamWriter(fileStream, encoding))
                    {
                        await writer.WriteAsync(content);
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
            }
        }
    }
}
