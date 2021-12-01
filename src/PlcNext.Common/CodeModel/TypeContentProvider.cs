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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.CodeModel
{
    internal class TypeContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateResolver resolver;

        public TypeContentProvider(ITemplateRepository templateRepository, ITemplateResolver resolver)
        {
            this.templateRepository = templateRepository;
            this.resolver = resolver;
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            if (!owner.HasValue<IType>() ||
                key == EntityKeys.TemplateKey)
            {
                return false;
            }

            IType type = owner.Value<IType>();

            metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                  .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                       StringComparison
                                                                          .OrdinalIgnoreCase));

            Templates.Type.metaDataTemplate typeMetaDataTemplate = templateRepository.TypeTemplates
               .FirstOrDefault(t => t.name.Equals(key, StringComparison.OrdinalIgnoreCase));

            templateRelationship[] relationships = owner.HasTemplate() ? owner.Template().Relationship : null;
            templateRelationship relationship =
                relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                ?? relationships?.FirstOrDefault(
                    r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));

            return key is EntityKeys.PathKey or EntityKeys.FieldsKey or EntityKeys.BaseTypeKey or EntityKeys.FileKey ||
                   type.HasPropertyValueEntity(key) ||
                   metaDataTemplate != null ||
                   typeMetaDataTemplate != null ||
                   relationship != null &&
                   type.Attributes.Any(a => a.Name.Equals(relationship.name,
                                                          StringComparison.OrdinalIgnoreCase));
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            IType type = owner.Value<IType>();
            switch (key)
            {
                case EntityKeys.PathKey:
                {
                    return owner.Create(key, type.GetFile(owner).Parent.FullName);
                }
                case EntityKeys.FieldsKey:
                {
                    return owner.Create(
                        key,
                        GetAllInheritedFields(owner.Root.Value<ICodeModel>(), type)
                           .Select(f => owner.Create(key.Singular(), f.Name, f)));
                }
                case EntityKeys.FileKey:
                {
                    VirtualFile file = owner.Root.Value<ICodeModel>()
                                            .Types[type];
                    return owner.Create(key, file.Name, file);
                }
                case EntityKeys.BaseTypeKey:
                {
                    IDataType baseType = type.BaseTypes.FirstOrDefault();
                    return owner.Create(key, baseType);
                }
                default:
                {
                    if (TryResolveTypeTemplate(owner, key, type, out Entity result))
                    {
                        return result;
                    }

                    if (TryResolveProperty(out result))
                    {
                        return result;
                    }

                    if (TryResolveFields(owner, key, type, out result))
                    {
                        return result;
                    }

                    if (TryResolveRelationship(owner, key, type, out result))
                    {
                        return result;
                    }

                    throw new ContentProviderException(key, owner);
                }
            }

            bool TryResolveProperty(out Entity result)
            {
                result = owner.PropertyValueEntity(key, type);
                return result != null;
            }
        }

        private bool TryResolveFields(Entity owner, string key, IType type, out Entity result)
        {
            metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                  .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                       StringComparison
                                                                          .OrdinalIgnoreCase));
            if (metaDataTemplate != null)
            {
                IEnumerable<IField> fields = type.Fields
                                                 .Where(f => f.Attributes
                                                              .Any(a => a.Name
                                                                         .Equals(metaDataTemplate.name,
                                                                              StringComparison.OrdinalIgnoreCase)));
                result = owner.Create(key, fields.Select(f => owner.Create(metaDataTemplate.name, f.Name, f)));
                return true;
            }

            result = null;
            return false;
        }

        private bool TryResolveRelationship(Entity owner, string key, IType type, out Entity result)
        {
            templateRelationship[] relationships = owner.Template().Relationship;
            templateRelationship relationship =
                relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                ?? relationships?.FirstOrDefault(
                    r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));
            TemplateDescription relationshipDescription =
                relationship != null ? templateRepository.Template(relationship.type) : null;
            if (relationshipDescription != null)
            {
                IEnumerable<string> names = type.Attributes
                                                .Where(a => a.Name.Equals(relationship.name,
                                                                          StringComparison.OrdinalIgnoreCase))
                                                .SelectMany(a => a.Values);
                result = relationship.GetRelationship(relationshipDescription, owner, names.ToArray());
                return true;
            }

            result = null;
            return false;
        }

        private bool TryResolveTypeTemplate(Entity owner, string key, IType type, out Entity result)
        {
            IEnumerable<(Templates.Type.metaDataTemplate, string, string)> templates = templateRepository.TypeTemplates
               .Select(t => (t, t.name, t.context));
            Templates.Type.metaDataTemplate typeMetaDataTemplate = ChooseMetaDataTemplate(templates, key, owner);
            if (typeMetaDataTemplate != null)
            {
                result = GetTypeTemplateEntity(typeMetaDataTemplate);
                return true;
            }

            result = null;
            return false;

            Entity GetTypeTemplateEntity(Templates.Type.metaDataTemplate metaDataTemplate)
            {
                if (!metaDataTemplate.hasvalue)
                {
                    bool hasAttribute = type.HasAttributeWithoutValue(metaDataTemplate.name);
                    return owner.Create(key, new Func<string>(hasAttribute.ToString), hasAttribute);
                }

                (string[] values, CodePosition position) = GetTypeTemplateValue();
                IEnumerable<string> formattedValues = VerifyValues().ToArray();

                return metaDataTemplate.multiplicity == Templates.Type.multiplicity.OneOrMore
                           ? owner.Create(key, formattedValues.Select(v => owner.Create(key.Singular(), v)))
                           : owner.Create(key, formattedValues.First());

                IEnumerable<string> VerifyValues()
                {
                    return values.Select(VerifyValue);

                    string VerifyValue(string arg)
                    {
                        (bool success, string message, string newValue) =
                            metaDataTemplate.ValueRestriction.Verify(arg);
                        if (!success)
                        {
                            owner.AddCodeException(
                                new FieldAttributeRestrictionException(
                                    metaDataTemplate.name, arg, message, position, type.GetFile(owner)));
                        }

                        return newValue;
                    }
                }

                (string[], CodePosition) GetTypeTemplateValue()
                {
                    IAttribute attribute = type.Attributes.LastOrDefault(a => a.Name.Equals(metaDataTemplate.name,
                                                                             StringComparison.OrdinalIgnoreCase));
                    string value = attribute?.Values.FirstOrDefault() ??
                                   resolver.Resolve(metaDataTemplate.defaultvalue, owner);
                    return (metaDataTemplate.multiplicity == Templates.Type.multiplicity.OneOrMore
                                ? value.Split(new[] { metaDataTemplate.split },
                                              StringSplitOptions.RemoveEmptyEntries)
                                : new[] { value },
                            attribute?.Position ?? new CodePosition(0, 0));
                }
            }
        }

        private IEnumerable<IField> GetAllInheritedFields(ICodeModel model, IType type)
        {
            IEnumerable<IType> allTypes = GetAllTypes(type);
            return allTypes.SelectMany(t => t.Fields);

            IEnumerable<IType> GetAllTypes(IType child)
            {
                yield return child;
                foreach (IDataType baseDataType in child.BaseTypes.Where(
                    t => t.Visibility.Equals("public", StringComparison.OrdinalIgnoreCase)))
                {
                    IType realType = baseDataType.PotentialFullNames
                                                 .Select(model.Type)
                                                 .FirstOrDefault(t => t != null);
                    if (realType == null)
                    {
                        continue;
                    }

                    foreach (IType baseType in GetAllTypes(realType))
                    {
                        yield return baseType;
                    }
                }
            }
        }

        private T ChooseMetaDataTemplate<T>(IEnumerable<(T template, string name, string context)> templates,
                                            string key, Entity owner)
        {
            return templates.OrderByDescending(t => ContextComplexity(t.context ?? string.Empty))
                            .FirstOrDefault(Matches)
                            .template;

            bool Matches((T template, string name, string context) template)
            {
                if (!key.Equals(template.name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(template.context))
                {
                    return true;
                }

                string[] contextPath = template
                                      .context.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Reverse().ToArray();
                Entity current = owner;
                foreach (string part in contextPath)
                {
                    current = current?.Owner;
                    if (current?[$"is{part}"].Value<bool>() != true)
                    {
                        return false;
                    }
                }

                return true;
            }

            int ContextComplexity(string context)
            {
                return context.Count(c => c == '/' || c == '\\');
            }
        }
    }
}