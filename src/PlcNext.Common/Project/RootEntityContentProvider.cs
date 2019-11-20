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
using System.Text.RegularExpressions;
using PlcNext.Common.Build;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Project
{
    internal class RootEntityContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateIdentifierRepository identifierRepository;
        private readonly IFileSystem fileSystem;
        private readonly IParser parser;
        private readonly ExecutionContext executionContext;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;
        private readonly IBuildInformationService informationService;

        public RootEntityContentProvider(ITemplateRepository templateRepository, ITemplateIdentifierRepository identifierRepository, IFileSystem fileSystem, IParser parser, ExecutionContext executionContext, ITargetParser targetParser, ISdkRepository sdkRepository, IBuildInformationService informationService)
        {
            this.templateRepository = templateRepository;
            this.identifierRepository = identifierRepository;
            this.fileSystem = fileSystem;
            this.parser = parser;
            this.executionContext = executionContext;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
            this.informationService = informationService;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.RootKey ||
                   key == EntityKeys.EscapeProjectNameFormatKey && owner.Type == EntityKeys.FormatKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            if (key == EntityKeys.EscapeProjectNameFormatKey && 
                owner.Type == EntityKeys.FormatKey)
            {
                return EscapeProjectName();
            }
            Entity rootEntity = CreateRootEntity();
            rootEntity.SetMetaData(true, EntityKeys.IsRoot);
            return rootEntity;

            Entity EscapeProjectName()
            {
                string value = owner.Owner?.Value<string>();
                if (string.IsNullOrEmpty(value))
                {
                    return owner.Create(key, string.Empty);
                }

                value = Regex.Replace(value, @"[^a-zA-Z0-9_\.]", "_"); //Replace not allowed values with _
                int length, newLength;
                do //Remove double _ and .
                {
                    length = value.Length;
                    value = value.Replace("__", "_");
                    value = value.Replace("..", ".");
                    newLength = value.Length;
                } while (length != newLength);

                value = value.TrimEnd('.'); //Remove trailing dot

                string prefix = string.Empty;
                int lastPoint = value.LastIndexOf('.');
                if (lastPoint >= 0)
                {
                    prefix = value.Substring(0, lastPoint);
                    value = value.Substring(lastPoint + 1);
                }

                if (value[0] == '_' && value.Length > 1)
                {
                    value = "_" + new string(char.ToUpperInvariant(value[1]), 1) + value.Substring(2);
                }
                else
                {
                    value = new string(char.ToUpperInvariant(value[0]), 1) + value.Substring(1);
                }
                if (!Regex.IsMatch(value, @"^_?[A-Z]"))
                {//Start with uppercase letter
                    value = "Library";
                }

                if (!string.IsNullOrEmpty(prefix))
                {
                    value = prefix + "." + value;
                }

                return owner.Create(key, value);
            }

            Entity CreateRootEntity()
            {
                if (HasRootInHierarchy(out Entity hierarchyRoot))
                {
                    return hierarchyRoot;
                }

                if (owner.IsCommand())
                {
                    string rootFilePath = owner.GetPathCommandArgument();
                    VirtualDirectory baseDirectory = fileSystem.FileExists(rootFilePath)
                                                         ? fileSystem.GetFile(rootFilePath).Parent
                                                         : fileSystem.DirectoryExists(rootFilePath)
                                                            ? fileSystem.GetDirectory(rootFilePath)
                                                            : throw new FormattableException($"The path {rootFilePath} does not exist.");
                    if (owner.HasTemplate())
                    {
                        return CreateRootBasedOnTemplate(baseDirectory);
                    }

                    return CreateRootBasedOnAllRootTemplates(baseDirectory);
                }

                if (owner.IsTemplateOnly())
                {
                    TemplateDescription template = owner.Template();
                    TemplateDescription rootTemplate = FindRootTemplate(template);
                    if (rootTemplate == null)
                    {
                        throw new RootTemplateNotFoundException(template.name);
                    }

                    return owner.Create(rootTemplate.name);
                }
                
                return CreateFallback();

                ICodeModel ParseCodeModel(VirtualDirectory virtualDirectory, Entity root)
                {
                    try
                    {
                        ICodeModel model = parser.Parse(GetSourceDirectories(virtualDirectory),
                                                        GetIncludeDirectories(virtualDirectory),
                                                        out IEnumerable<CodeSpecificException> loggableExceptions);
                        bool firstException = true;
                        foreach (CodeSpecificException loggableException in loggableExceptions)
                        {
                            if (firstException)
                            {
                                executionContext.WriteInformation(
                                    "The following code errors were found inside the parsed include files. " +
                                    "Containing types cannot be used as port types.", false);
                                firstException = false;
                            }
                            loggableException.CompleteCodeExceptions(virtualDirectory);
                            executionContext.WriteError(loggableException.ToString(), false);
                        }
                        return model;
                    }
                    catch (Exception exception)
                    {
                        exception.CompleteCodeExceptions(virtualDirectory);
                        throw;
                    }

                    IDictionary<string, VirtualDirectory> GetIncludeDirectories(VirtualDirectory baseDirectory)
                    {
                        IEnumerable<string> includes = HasIncludeDirectoriesCommandArgument(owner)
                                                           ? GetIncludeDirectoriesCommandArgument(owner)
                                                           : Enumerable.Empty<string>();
                        Target[] projectTargets = GetProjectTargets();

                        if (projectTargets.Any())
                        {
                            if (!includes.Any() && HasNoIncludeDetectionCommandArgument(owner) && !GetNoIncludeDetectionCommandArgument(owner))
                            {
                                try
                                {
                                    includes = informationService.RetrieveBuildSystemProperties(root, projectTargets[0], executionContext.Observable).IncludePaths;
                                }
                                catch (Exception e)
                                {
                                    if (e is FormattableException || e is AggregateException)
                                    {
                                        executionContext.WriteWarning($"Automatic include detection via cmake could not be executed. See log for details.");
                                        executionContext.WriteError(e.ToString(), false);
                                    }
                                    else
                                        throw;
                                }
                            }
                        }
                        else
                        {
                            executionContext.WriteWarning($"The project in {baseDirectory.FullName} does not contain a valid target. " +
                                                          $"Without a valid target port structures from within the SDK can not be generated " +
                                                          $"and automatic include detection will not work as well.");
                        }
                        includes = includes.Concat(new[]
                        {
                            Path.Combine(Constants.IntermediateFolderName, Constants.GeneratedCodeFolderName)
                        });

                        includes = includes.Concat(GetTargetIncludes());
                        
                        IDictionary<string, VirtualDirectory> includeDirectories = includes.Distinct().ToDictionary(x => x, GetIncludeDirectory);
                        return includeDirectories;

                        IEnumerable<string> GetTargetIncludes()
                        {
                            SdkInformation[] projectSdks = projectTargets.Select(sdkRepository.GetSdk)
                                                              .Distinct()
                                                              .ToArray();

                            return projectSdks.SelectMany(s => s.IncludePaths.Concat(s.CompilerInformation.IncludePaths));
                        }

                        VirtualDirectory GetIncludeDirectory(string path)
                        {
                            if (fileSystem.DirectoryExists(path, baseDirectory.FullName))
                            {
                                return fileSystem.GetDirectory(path, baseDirectory.FullName);
                            }

                            executionContext.WriteWarning($"The include path {path} was not found and will not be used.",false);
                            return null;
                        }

                        Target[] GetProjectTargets()
                        {
                            ProjectEntity project = ProjectEntity.Decorate(root);
                            TargetsResult targetsResult = targetParser.Targets(project, false);
                            Target[] availableTargets = sdkRepository.GetAllTargets().ToArray();
                            Target[] targets = targetsResult.ValidTargets
                                                                   .Select(tr => availableTargets.FirstOrDefault(
                                                                               t => t.Name == tr.Name &&
                                                                                    t.LongVersion == tr.LongVersion))
                                                                   .Where(t => t != null)
                                                                   .ToArray();
                            return targets;
                        }
                    }

                    ICollection<VirtualDirectory> GetSourceDirectories(VirtualDirectory baseDirectory)
                    {
                        IEnumerable<string> sources = HasSourceDirectoriesCommandArgument(owner)
                                                          ? GetSourceDirectoriesCommandArgument(owner)
                                                          : Enumerable.Empty<string>();
                        ICollection<VirtualDirectory> sourceDirectories = sources.Select(s => fileSystem.DirectoryExists(s, baseDirectory.FullName)
                                                                                                  ? fileSystem.GetDirectory(s, baseDirectory.FullName)
                                                                                                  : throw new FormattableException(
                                                                                                        $"The path {s} does not exist."))
                                                                                 .ToArray();
                        if (!sourceDirectories.Any())
                        {
                            sourceDirectories = baseDirectory.Directories.Any(d => d.Name == Constants.SourceFolderName)
                                                    ? new[] {baseDirectory.Directory(Constants.SourceFolderName) }
                                                    : new[] {baseDirectory};
                        }

                        return sourceDirectories;
                    }

                    IEnumerable<string> GetSourceDirectoriesCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<MultipleValueArgument>(EntityKeys.SourceDirectoryKey)
                                    ?.Values
                               ?? entity.Value<CommandArgs>()
                                       ?.PropertyValue<IEnumerable<string>>(EntityKeys.SourceDirectoryKey)
                               ?? Enumerable.Empty<string>();
                    }

                    bool HasSourceDirectoriesCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<MultipleValueArgument>(EntityKeys.SourceDirectoryKey)
                               != null
                               || entity.Value<CommandArgs>()
                                       ?.HasPropertyValue(EntityKeys.SourceDirectoryKey, typeof(IEnumerable<string>))
                               == true;
                    }

                    IEnumerable<string> GetIncludeDirectoriesCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<MultipleValueArgument>(EntityKeys.IncludeDirectoryKey)
                                    ?.Values
                               ?? entity.Value<CommandArgs>()
                                       ?.PropertyValue<IEnumerable<string>>(EntityKeys.IncludeDirectoryKey)
                               ?? Enumerable.Empty<string>();
                    }

                    bool HasIncludeDirectoriesCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<MultipleValueArgument>(EntityKeys.IncludeDirectoryKey)
                               != null
                               || entity.Value<CommandArgs>()
                                       ?.HasPropertyValue(EntityKeys.IncludeDirectoryKey, typeof(IEnumerable<string>))
                               == true;
                    }

                    bool GetNoIncludeDetectionCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<BoolArgument>(Constants.NoIncludePathDetection)
                                    ?.Value
                               ?? entity.Value<CommandArgs>()
                                       ?.PropertyValue<bool>(Constants.NoIncludePathDetection.ToPropertyName())
                               ?? false;
                    }

                    bool HasNoIncludeDetectionCommandArgument(Entity entity)
                    {
                        return entity.Value<CommandDefinition>()
                                    ?.Argument<BoolArgument>(Constants.NoIncludePathDetection)
                               != null
                               || entity.Value<CommandArgs>()
                                       ?.HasPropertyValue(Constants.NoIncludePathDetection.ToPropertyName(), typeof(bool))
                               == true;
                    }
                }

                ICodeModel CreateCodeModel(Entity root)
                {
                    VirtualDirectory rootDirectory = fileSystem.GetDirectory(root.Path);
                    return ParseCodeModel(rootDirectory, root);
                }

                Entity CreateFallback(VirtualDirectory baseDirectory = null)
                {
                    Entity fallbackEntity = null;
                    try
                    {
                        if (owner.HasPathCommandArgument())
                        {
                            VirtualDirectory rootDirectory = fileSystem.GetDirectory(owner.GetPathCommandArgument(), createNew: false);
                            if (rootDirectory != null && fileSystem.DirectoryExists(rootDirectory.FullName))
                            {
                                IEnumerable<VirtualFile> possibleComponents = rootDirectory.Files(searchString: "*.hpp", searchRecursive: true);
                                bool result = possibleComponents.Where(c =>
                                {
                                    using (Stream fileStream = c.OpenRead())
                                    using (StreamReader streamReader = new StreamReader(fileStream))
                                    {
                                        string content = streamReader.ReadToEnd();
                                        if (content.Contains("MetaComponentBase"))
                                        {
                                            fallbackEntity = owner.Create("acfproject");
                                        }
                                    }
                                    return false;
                                }).Any();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        executionContext.WriteVerbose($"Error while creating fallback root entity.{Environment.NewLine}{exception}");
                    }
                    if (fallbackEntity == null)
                    {
                        fallbackEntity = owner.Create("project");
                    }
                    fallbackEntity.AddValue(() => CreateCodeModel(fallbackEntity));

                    if (baseDirectory != null)
                    {
                        fallbackEntity.AddValue(baseDirectory);
                    }
                    return fallbackEntity;
                }

                bool HasRootInHierarchy(out Entity foundRoot)
                {
                    foundRoot = owner.EntityHierarchy().FirstOrDefault(HasRootTemplate);
                    return foundRoot != null;

                    bool HasRootTemplate(Entity possibleRoot)
                    {
                        return possibleRoot.HasTemplate() && possibleRoot.Template().isRoot;
                    }
                }

                TemplateDescription FindRootTemplate(TemplateDescription template)
                {
                    Stack<TemplateDescription> unvisited = new Stack<TemplateDescription>(new[] { template });
                    List<TemplateDescription> visited = new List<TemplateDescription>();
                    while (unvisited.Any())
                    {
                        TemplateDescription current = unvisited.Pop();
                        if (current.isRoot)
                        {
                            return current;
                        }

                        visited.Add(current);

                        foreach (templateRelationship relationship in current.Relationship)
                        {
                            TemplateDescription description = templateRepository.Template(relationship.type);
                            if (description != null && !visited.Contains(description))
                            {
                                unvisited.Push(description);
                            }
                        }
                    }

                    return null;
                }

                Entity CreateRootBasedOnAllRootTemplates(VirtualDirectory baseDirectory)
                {
                    return IdentifyRoot(templateRepository.Templates.Where(t => t.isRoot), baseDirectory);
                }

                Entity IdentifyRoot(IEnumerable<TemplateDescription> templateDescriptions, VirtualDirectory baseDirectory)
                {
                    Entity root = null;
                    foreach (TemplateDescription possibleTemplate in templateDescriptions.OrderByDescending(Depth))
                    {
                        root = identifierRepository.FindAllEntities(possibleTemplate.name, owner,
                                                                    possibleTemplate.identifier)
                                                    .FirstOrDefault();
                        if (root != null)
                        {
                            break;
                        }
                    }

                    if (root != null)
                    {
                        root.AddValue(() => CreateCodeModel(root));
                    }
                    else
                    {
                        root = CreateFallback(baseDirectory);
                    }
                    return root;

                    int Depth(TemplateDescription templateDescription)
                    {
                        int depth = 0;
                        while (!string.IsNullOrEmpty(templateDescription?.basedOn))
                        {
                            templateDescription = templateRepository.Template(templateDescription.basedOn);
                            depth++;
                        }

                        return depth;
                    }
                }

                Entity CreateRootBasedOnTemplate(VirtualDirectory baseDirectory)
                {
                    TemplateDescription template = owner.Template();
                    IEnumerable<TemplateDescription> possibleTemplates = FindAllRootTemplates();
                    return IdentifyRoot(possibleTemplates, baseDirectory);

                    IEnumerable<TemplateDescription> FindAllRootTemplates()
                    {
                        TemplateDescription rootTemplate = FindRootTemplate(template);
                        if (rootTemplate == null)
                        {
                            throw new RootTemplateNotFoundException(template.name);
                        }

                        List<TemplateDescription> result = new List<TemplateDescription>();
                        Stack<TemplateDescription> unvisited = new Stack<TemplateDescription>();
                        unvisited.Push(rootTemplate);
                        while (unvisited.Any())
                        {
                            TemplateDescription current = unvisited.Pop();
                            result.Add(current);
                            foreach (TemplateDescription description in templateRepository.Templates
                                                                                          .Where(t => t.basedOn
                                                                                                      ?.Equals(current.name,
                                                                                                               StringComparison
                                                                                                                  .OrdinalIgnoreCase)
                                                                                                      == true))
                            {
                                unvisited.Push(description);
                            }
                        }

                        return result;
                    }
                }
            }
        }
    }
}
