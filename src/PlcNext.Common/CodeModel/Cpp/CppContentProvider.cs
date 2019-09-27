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
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.CodeModel.Cpp
{
    internal class CppContentProvider : PriorityContentProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ITemplateResolver resolver;
        private readonly Regex splitRegex = new Regex(@"[\/\\](?<split>.*)?", RegexOptions.Compiled);
        private readonly Regex relativeDirectoryRegex;

        public CppContentProvider(IFileSystem fileSystem, ITemplateResolver resolver)
        {
            this.fileSystem = fileSystem;
            this.resolver = resolver;
            string pathSeparator = Regex.Escape(new string(Path.DirectorySeparatorChar, 1));
            relativeDirectoryRegex = new Regex(@"[^" + pathSeparator + @"]+(?:[" + pathSeparator + @"](?<split>.*))?",
                                               RegexOptions.Compiled);
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.IncludeKey && owner.Type == EntityKeys.FormatKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            if (key == EntityKeys.IncludeKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveInclude();
            }
            throw new ContentProviderException(key, owner);

            Entity ResolveInclude()
            {
                IEntityBase templateOrigin = GetTemplateOrigin(out IEntityBase formatOrigin);
                CommandEntity commandEntity = CommandEntity.Decorate(owner.Origin);
                string basePath = string.Empty;
                if (templateOrigin.HasContent(EntityKeys.BaseDirectoryKey) &&
                    !string.IsNullOrEmpty(templateOrigin[EntityKeys.BaseDirectoryKey].Value<string>()))
                {
                    string path = templateOrigin.Path;
                    string relative = path.Split(new[] {templateOrigin[EntityKeys.BaseDirectoryKey].Value<string>()},
                                                 StringSplitOptions.None)
                                          .Last();
                    Match match = splitRegex.Match(relative);
                    if (match.Success && match.Groups["split"].Success)
                    {
                        relative = match.Groups["split"].Value;
                    }

                    basePath = relative.Replace('\\', '/');
                }
                else if (commandEntity.IsCommandArgumentSpecified(EntityKeys.OutputKey))
                {
                    string output = commandEntity.Output;
                    string rootPath = templateOrigin.Root.Path;
                    string outputDirectory = fileSystem.GetDirectory(output, rootPath, false).FullName;
                    if (outputDirectory.StartsWith(rootPath, StringComparison.Ordinal) && outputDirectory != rootPath)
                    {
                        string partialPath = outputDirectory.Substring(rootPath.Length + 1).CleanPath();
                        Match match = relativeDirectoryRegex.Match(partialPath);
                        if (match.Success && match.Groups["split"].Success)
                        {
                            basePath = match.Groups["split"].Value;
                        }
                    }
                }

                string result = resolver.Resolve(formatOrigin.Name, templateOrigin);
                if (!string.IsNullOrEmpty(basePath))
                {
                    if (!basePath.EndsWith("/", StringComparison.Ordinal))
                    {
                        basePath += "/";
                    }

                    result = basePath + result;
                }

                return owner.Create(key, result);

                IEntityBase GetTemplateOrigin(out IEntityBase realOrigin)
                {
                    realOrigin = TemplateEntity.Decorate(owner).FormatOrigin;
                    IEntityBase templateParent = realOrigin;
                    while (templateParent != null &&
                           templateParent.Type != EntityKeys.TemplateKey)
                    {
                        templateParent = templateParent.Owner;
                    }

                    return templateParent?.Owner ?? realOrigin;
                }
            }
        }
    }
}
