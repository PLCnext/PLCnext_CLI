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
using System.Reflection;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.Templates
{
    internal class TemplateContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;

        public TemplateContentProvider(ITemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            if (key == EntityKeys.TemplateKey ||
                key == EntityKeys.HiearchyKey ||
                key == EntityKeys.RelatedKey)
            {
                TemplateDescription template = templateRepository.Template(owner.Type);
                return template != null || ParentTemplateEntity(owner) != null;
            }

            TemplateDescription ownerTemplate = owner.Value<TemplateDescription>();
            if (ownerTemplate != null)
            {
                return true;
            }

            IEnumerable<ForeachItemContainer> foreachItems = owner.Values<ForeachItemContainer>();
            if (foreachItems.Any(i => i.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return owner.Any(e => e.Value<templateFile>() != null) ||
                   IsCheck(owner, key) ||
                   key.StartsWith("is", StringComparison.OrdinalIgnoreCase) && fallback ||
                   (owner.Value<templateFile>() != null &&
                    owner.Value<templateFile>().GetType().GetProperty(key) != null);
        }

        bool IsCheck(Entity entity, string key)
        {
            if (!key.StartsWith("is", StringComparison.OrdinalIgnoreCase) ||
                key.Length == 2)
            {
                return false;
            }

            return entity.Type.Equals(key.Substring(2), StringComparison.OrdinalIgnoreCase);
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            if (key.StartsWith("is", StringComparison.OrdinalIgnoreCase))
            {
                return owner.Create(key, IsCheck(owner, key));
            }
            switch (key)
            {
                case EntityKeys.TemplateKey:
                    TemplateDescription template = templateRepository.Template(owner.Type);
                    return template != null
                               ? owner.Create(key, owner.Type, template)
                               : ParentTemplateEntity(owner);
                case EntityKeys.RelatedKey:
                    return GetRelatedEntities(owner);
                case EntityKeys.HiearchyKey:
                    return GetHierarchyEntities(owner);
                default:
                    //continue execution
                    break;
            }

            IEnumerable<ForeachItemContainer> foreachItems = owner.Values<ForeachItemContainer>();
            ForeachItemContainer foreachItem = foreachItems.FirstOrDefault(i => i.Key.Equals(key,
                                                                                             StringComparison.OrdinalIgnoreCase));
            if (foreachItem != null)
            {
                return foreachItem.Current;
            }

            if (owner.Any(o => o.Value<templateFile>()?.key?.Equals(key, StringComparison.OrdinalIgnoreCase) == true))
            {
                return owner.First(o => o.Value<templateFile>()?.key?.Equals(key, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (owner.Value<templateFile>() != null)
            {
                Entity result = owner.Create(key, new Func<string>(() => owner.Value<templateFile>().GetType()
                                                                              .GetProperty(key)?
                                                                              .GetValue(owner.Value<templateFile>())
                                                                             ?.ToString() ?? string.Empty));
                result.SetContext(FindContext(owner));
                return result;
            }

            TemplateDescription ownerTemplate = owner.Value<TemplateDescription>();
            switch (key)
            {
                case EntityKeys.TemplateFilesKey:
                    Entity[] files = ownerTemplate.File.Concat(ownerTemplate.GeneratedFile??Enumerable.Empty<templateGeneratedFile>())
                                                  .Select(tf => owner.Create("TemplateFile", tf.template, tf))
                                                  .ToArray();
                    return owner.Create("TemplateFiles", (IEnumerable<Entity>) files);
                default:
                    PropertyInfo info = ownerTemplate.GetType().GetProperty(key);
                    if (info != null)
                    {
                        object value = info.GetValue(ownerTemplate);
                        return owner.Create(key, value?.ToString() ?? string.Empty, value);
                    }
                    throw new ContentProviderException(key, owner);
            }

            Entity FindContext(Entity current)
            {
                while (current != null && current.Value<TemplateDescription>() == null)
                {
                    current = current.Owner;
                }

                return current?.Owner;
            }

            Entity GetRelatedEntities(Entity entity)
            {
                TemplateDescription template = entity.Template();
                IType entityType = entity.Value<IType>();
                (templateRelationship relationship, TemplateDescription template)[] relatedTemplates = GetRelatedTemplates();
                List<(string tn, IType t)> related = new List<(string, IType)>();
                if (entity.Root.HasValue<ICodeModel>())
                {
                    ICodeModel codeModel = entity.Root.Value<ICodeModel>();
                    foreach (IType type in codeModel.Types.Keys)
                    {
                        (templateRelationship rel, TemplateDescription des) = relatedTemplates.FirstOrDefault(t => type.HasAttributeWithoutValue(t.template.name));
                        bool isRelevant = false;
                        if (rel != null)
                        {
                            if (template.TemplateNames(templateRepository)
                                        .Any(n => n.Equals(rel.type, StringComparison.OrdinalIgnoreCase)))
                            {
                                isRelevant = template.isRoot||
                                             type.Attributes.Any(a => a.Name.Equals(rel.name, StringComparison.OrdinalIgnoreCase) &&
                                                                      a.Values.Any(v => IsRelatedType(entityType, v)));
                            }
                            else
                            {
                                isRelevant = des.isRoot||
                                             entityType?.Attributes.Any(a => a.Name.Equals(rel.name, StringComparison.OrdinalIgnoreCase) &&
                                                                             a.Values.Any(v => IsRelatedType(type, v))) == true;
                            }
                        }
                        if (isRelevant)
                        {
                            related.Add((des.name, type));
                        }
                    }

                    bool IsRelatedType(IType relatedType, string name)
                    {
                        if (relatedType?.FullName.Equals(name, StringComparison.Ordinal) == true)
                        {
                            return true;
                        }

                        return codeModel.Type(name) == null &&
                               relatedType?.FullName.Contains(name) == true;
                    }
                }

                return owner.Create(key, 
                                    related.Select(r => owner.EntityHierarchy()
                                                             .FirstOrDefault(e => e.HasValue<IType>() &&
                                                                                  e.Value<IType>() == r.t) ??
                                                        owner.Create(r.tn, r.t.FullName, r.t)));

                (templateRelationship, TemplateDescription)[] GetRelatedTemplates()
                {
                    IEnumerable<(templateRelationship, TemplateDescription)> ownRelationships = template.IncludingBaseTemplates(templateRepository)
                                                                                                        .SelectMany(t => t.Relationship??Enumerable.Empty<templateRelationship>())
                                                                                                        .Select(r => (r, templateRepository.Template(r.type)))
                                                                                                        .Where(t => t.Item2 != null);
                    return templateRepository.Templates
                                             .Select(GetRelation)
                                             .Where(t => t.Item1 != null)
                                             .Concat(ownRelationships)
                                             .ToArray();

                    (templateRelationship, TemplateDescription) GetRelation(TemplateDescription arg)
                    {
                        return (arg.Relationship?.FirstOrDefault(r => template.TemplateNames(templateRepository)
                                                                              .Any(n => n.Equals(r.type,StringComparison.OrdinalIgnoreCase))), 
                                                                 arg);
                    }
                }
            }

            Entity GetHierarchyEntities(Entity entity)
            {
                HashSet<Entity> result = new HashSet<Entity>();
                Add(entity);

                void Add(Entity current)
                {
                    bool newEntity = result.Add(current);
                    if (newEntity)
                    {
                        foreach (Entity related in current.Related())
                        {
                            Add(related);
                        }
                    }
                }

                CheckWildEntities();

                return owner.Create(key, result);

                void CheckWildEntities()
                {
                    Dictionary<string, string[]> relationships = GetAllRelationships();
                    (IType, string)[] wildEnities = FindWildEntities().ToArray();
                    if (wildEnities.Length == 1)
                    {
                        throw new WildEntityException(wildEnities[0].Item1, wildEnities[0].Item2.ToLowerInvariant(),
                                                      relationships[wildEnities[0].Item2]);
                    }

                    if (wildEnities.Length > 1)
                    {
                        throw new AggregateException(wildEnities.Select(t => new WildEntityException(t.Item1, t.Item2, relationships[t.Item2])));
                    }

                    IEnumerable<(IType, string) > FindWildEntities()
                    {
                        if (entity.Root.HasValue<ICodeModel>())
                        {
                            IEnumerable<IType> typesToCheck = entity.Root.Value<ICodeModel>().Types.Keys
                                                                    .Except(result.Select(e => e.Value<IType>()));
                            return typesToCheck.Select(t => (t, IsWildType(t)))
                                               .Where(t => t.Item2.Item1)
                                               .Select(t => (t.Item1, t.Item2.Item2));
                        }

                        return Enumerable.Empty<(IType, string)>();

                        (bool, string) IsWildType(IType arg)
                        {
                            return arg.Attributes.Select(a => (!a.Values.Any() &&
                                                               !a.NamedValues.Any() &&
                                                               relationships.ContainsKey(a.Name.ToUpperInvariant()) &&
                                                               relationships[a.Name.ToUpperInvariant()].Any(),
                                                               a.Name.ToUpperInvariant()))
                                      .FirstOrDefault(r => r.Item1);
                        }
                    }

                    Dictionary<string, string[]> GetAllRelationships()
                    {
                        return templateRepository.Templates.ToDictionary(t => t.name.ToUpperInvariant(), description => (description.Relationship ?? Enumerable.Empty<templateRelationship>())
                                                                                          .Select(r => templateRepository.Template(r.type))
                                                                                          .Where(t => t != null)
                                                                                          .Select(t => t.isRoot ? string.Empty : t.name.ToLowerInvariant())
                                                                                          .Where(n => !string.IsNullOrEmpty(n))
                                                                                          .ToArray());
                    }
                }
            }
        }

        private static Entity ParentTemplateEntity(Entity current)
        {
            while (current != null &&
                   current.Type != EntityKeys.TemplateKey)
            {
                current = current.Owner;
            }

            return current;
        }
    }
}
