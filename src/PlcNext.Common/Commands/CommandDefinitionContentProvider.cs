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
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Commands
{
    internal class CommandDefinitionContentProvider : IEntityContentProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ITemplateResolver templateResolver;
        private readonly ITemplateRepository templateRepository;
        private readonly ICodeLanguage codeLanguage;

        public CommandDefinitionContentProvider(IFileSystem fileSystem, ITemplateResolver templateResolver, ITemplateRepository templateRepository, ICodeLanguage codeLanguage)
        {
            this.fileSystem = fileSystem;
            this.templateResolver = templateResolver;
            this.templateRepository = templateRepository;
            this.codeLanguage = codeLanguage;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.Value<CommandDefinition>() != null &&
                   key != EntityKeys.TemplateKey &&
                   (key == EntityKeys.PathKey ||
                    key == EntityKeys.FullNameKey ||
                    HasArgument() ||
                    HasRelationship());

            bool HasArgument()
            {
                return owner.Value<CommandDefinition>().Argument<Argument>(key) != null ||
                       owner.HasTemplate() && owner.Template().Arguments.Any(a => a.name.Equals(key, StringComparison.OrdinalIgnoreCase));
            }

            bool HasRelationship()
            {
                return key != EntityKeys.TemplateKey &&
                       owner.HasTemplate() &&
                       owner.Template()?.Relationship?
                          .Any(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                       == true;
            }
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            templateRelationship[] relationships = owner.Template().Relationship;
            templateRelationship relationship = relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                ??relationships?.FirstOrDefault(r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));
            TemplateDescription relationshipDescription = relationship != null ?templateRepository.Template(relationship.type):null;
            if (relationshipDescription != null)
            {
                return GetRelationship();
            }
            if (key == EntityKeys.PathKey)
            {
                return GetCommandPath();
            }
            if (key == EntityKeys.FullNameKey)
            {
                return GetFullName();
            }

            return GetArgument();

            Entity GetFullName()
            {
                SingleValueArgument nameArgument = owner.Value<CommandDefinition>()
                                                        .Argument<SingleValueArgument>(EntityKeys.NameKey);
                SingleValueArgument namespaceArgument = owner.Value<CommandDefinition>()
                                                             .Argument<SingleValueArgument>(EntityKeys.NamespaceKey);
                string fullName = codeLanguage.CombineNamespace(Value(namespaceArgument, EntityKeys.NamespaceKey), 
                                                                Value(nameArgument, EntityKeys.NameKey));
                return owner.Create(key, fullName, nameArgument, namespaceArgument);
            }

            Entity GetCommandPath()
            {
                SingleValueArgument singleValueArgument = owner.Value<CommandDefinition>()
                                                               .Argument<SingleValueArgument>(EntityKeys.OutputKey);
                string basePath = owner.IsRoot() ? string.Empty : owner.Root.Path;
                string path = fileSystem.GetDirectory(Value(singleValueArgument), basePath, false).FullName;
                return owner.Create(key, path, singleValueArgument);
            }

            string Value(Argument arg, string argumentName = null)
            {
                if (string.IsNullOrEmpty(argumentName))
                {
                    argumentName = key;
                }
                if (arg != null && arg is BoolArgument boolArgument)
                {
                    return boolArgument.Value.ToString();
                }

                SingleValueArgument singleValueArgument = (SingleValueArgument) arg;
                if (singleValueArgument?.Value != null)
                {
                    return ResolveValue(singleValueArgument.Value);
                }

                if (
                    (arg != null && arg.Name != argumentName && TryGetTemplateDefault(arg.Name, out string result))||
                    TryGetTemplateDefault(argumentName, out result))
                {
                    return result;
                }

                throw new ContentProviderException(key, owner);
            }

            string ResolveValue(string value)
            {
                string result = templateResolver.Resolve(value, owner);
                if (TryGetTemplateFormat(out string format) &&
                    !string.IsNullOrEmpty(format))
                {
                    Entity temporary = owner.Create(key, result);
                    result = temporary.Format()[format].Value<string>();
                }
                return result;

                bool TryGetTemplateFormat(out string formattedValue)
                {
                    TemplateDescription description = owner.Template();
                    if (description != null)
                    {
                        templateArgumentDefinition templateArgument = description.Arguments.FirstOrDefault(a => a.name == key);
                        if (templateArgument != null)
                        {
                            formattedValue = templateArgument.format;
                            return true;
                        }
                    }

                    formattedValue = null;
                    return false;
                }
            }

            bool TryGetTemplateDefault(string argumentName, out string value)
            {
                TemplateDescription description = owner.Template();
                if (description != null)
                {
                    templateArgumentDefinition templateArgument = description.Arguments.FirstOrDefault(a => a.name == argumentName);
                    if (templateArgument != null)
                    {
                        value = ResolveValue(templateArgument.@default);
                        return true;
                    }
                }

                value = null;
                return false;
            }

            Entity GetArgument()
            {
                Argument argument = owner.Value<CommandDefinition>().Argument<Argument>(key);
                if (argument == null)
                {
                    if (TryGetTemplateDefault(key, out string value))
                    {
                        return owner.Create(key, value);
                    }
                    throw new ContentProviderException(key, owner);
                }

                switch (argument)
                {
                    case BoolArgument boolArgument:
                        return owner.Create(key, Value(argument), boolArgument);
                    case SingleValueArgument singleValueArgument:
                        return owner.Create(key, Value(argument), singleValueArgument);
                    case MultipleValueArgument multipleValueArgument:
                        IEnumerable<Entity> values = Values(multipleValueArgument);
                        return owner.Create(key, values);
                    default:
                        throw new InvalidOperationException($"Unkown argument type {argument.GetType()}");
                }

                IEnumerable<Entity> Values(MultipleValueArgument multipleValueArgument)
                {
                    if (multipleValueArgument.Values != null)
                    {
                        return multipleValueArgument.Values
                                                    .Select(s => owner.Create(key,
                                                                              ResolveValue(s),
                                                                              multipleValueArgument));
                    }

                    if (TryGetTemplateDefault(key, out string result))
                    {
                        return new[] { owner.Create(key, result, multipleValueArgument) };
                    }

                    throw new ContentProviderException(key, owner);
                }
            }

            Entity GetRelationship()
            {
                if (relationshipDescription.isRoot)
                {
                    return owner.Root;
                }

                if (relationship.multiplicity == multiplicity.One)
                {
                    string name = owner.Value<CommandDefinition>()
                                       .Argument<SingleValueArgument>(relationship.name)
                                       .Value;
                    return relationship.GetRelationship(relationshipDescription, owner, name);
                }

                IEnumerable<string> names = owner.Value<CommandDefinition>()
                                                 .Argument<MultipleValueArgument>(relationship.name)
                                                 .Values;
                return relationship.GetRelationship(relationshipDescription, owner, names.ToArray());
            }
        }
    }
}
