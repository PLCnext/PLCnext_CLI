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
using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Project
{
    internal class RootEntityContentProvider : IEntityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateIdentifierRepository identifierRepository;
        private readonly IFileSystem fileSystem;
        private readonly IParser parser;
        private readonly ExecutionContext executionContext;

        public RootEntityContentProvider(ITemplateRepository templateRepository, ITemplateIdentifierRepository identifierRepository, IFileSystem fileSystem, IParser parser, ExecutionContext executionContext)
        {
            this.templateRepository = templateRepository;
            this.identifierRepository = identifierRepository;
            this.fileSystem = fileSystem;
            this.parser = parser;
            this.executionContext = executionContext;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.RootKey ||
                   key == EntityKeys.EscapeProjectNameFormatKey && owner.Type == EntityKeys.FormatKey;
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
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

                ICodeModel ParseCodeModel(VirtualDirectory virtualDirectory)
                {
                    try
                    {
                        return parser.Parse(GetSourceDirectories(virtualDirectory));
                    }
                    catch (Exception exception)
                    {
                        exception.CompleteCodeExceptions(virtualDirectory);
                        throw;
                    }

                    IEnumerable<VirtualDirectory> GetSourceDirectories(VirtualDirectory baseDirectory)
                    {
                        IEnumerable<string> sources = owner.HasSourceDirectoriesCommandArgument()
                                                          ? owner.GetSourceDirectoriesCommandArgument()
                                                          : Enumerable.Empty<string>();
                        IEnumerable<VirtualDirectory> sourceDirectories = sources.Select(s => fileSystem.DirectoryExists(s, baseDirectory.FullName)
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
                }

                ICodeModel CreateCodeModel(Entity root)
                {
                    VirtualDirectory rootDirectory = fileSystem.GetDirectory(root.Path);
                    return ParseCodeModel(rootDirectory);
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
                    }catch(Exception exception)
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
