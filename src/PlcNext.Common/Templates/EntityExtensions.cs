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
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Templates
{
    public static class EntityExtensions
    {
        public static TemplateDescription Template(this Entity owner)
        {
            return owner[EntityKeys.TemplateKey].Value<TemplateDescription>();
        }

        public static Entity GetRelationship(this templateRelationship relationship,
                                             TemplateDescription relationshipDescription, Entity owner,
                                             params string[] relationshipNames)
        {

            if (relationshipDescription.isRoot)
            {
                return owner.Root;
            }

            ICodeModel codeModel = owner.Root.Value<ICodeModel>();
            IType[] availableTypes = codeModel?.Types.Keys
                                               .Where(t => t.HasAttributeWithoutValue(relationshipDescription.name))
                                               .ToArray() ?? new IType[0];
            Entity[] availableEntities = owner.EntityHierarchy()
                                              .Where(e => e.Type.Equals(relationshipDescription.name,
                                                                        StringComparison.OrdinalIgnoreCase))
                                              .Where(e => e.HasValue<IType>() || e.HasValue<CommandDefinition>())
                                              .ToArray();
            if (relationship.multiplicity == multiplicity.One)
            {
                if (relationshipNames.Length != 1)
                {
                    throw new RelationshipMultiplicityMismatchException(relationship.name, owner.Name);
                }
                string name = relationshipNames[0];
                return CreateRelationshipEntity(name);
            }
            else
            {
                return owner.Create(relationship.name.Plural(), relationshipNames.Select(CreateRelationshipEntity));
            }

            Entity CreateRelationshipEntity(string name)
            {
                IType exactType = availableTypes.FirstOrDefault(t => t.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));
                IType[] relationshipTypes = exactType != null
                                                ? new[] {exactType}
                                                : availableTypes.Where(t => t.FullName.ToLowerInvariant()
                                                                             .Contains(name.ToLowerInvariant()))
                                                                .ToArray();
                Entity[] relationshipEntities = availableEntities.Where(e => e.Name?.Equals(name,
                                                                                              StringComparison.OrdinalIgnoreCase)
                                                                             == true)
                                                                 .ToArray();
                if (relationshipTypes.Length > 1)
                {
                    throw new AmbiguousRelationshipTypeException(relationship.name, name, relationshipTypes);
                }

                if (relationshipTypes.Length == 0 && !relationshipEntities.Any())
                {
                    throw new RelationshipTypeNotFoundException(relationship.name, name, availableTypes);
                }

                IType relationshipType = relationshipTypes.SingleOrDefault();
                if (relationshipType != null)
                {
                    return availableEntities.FirstOrDefault(e => e.HasValue<IType>() &&
                                                                 e.Value<IType>() == relationshipType)
                           ?? owner.Create(relationship.name, relationshipType.Name, relationshipType);
                }

                if (relationshipEntities.Length > 1)
                {
                    throw new AmbiguousRelationshipTypeException(relationship.name, name, relationshipEntities);
                }

                return relationshipEntities.Single();
            }
        }

        public static bool HasTemplate(this Entity owner)
        {
            return owner.HasContent(EntityKeys.TemplateKey) &&
                   owner[EntityKeys.TemplateKey].HasValue<TemplateDescription>();
        }

        public static Entity Format(this Entity owner)
        {
            return owner[EntityKeys.FormatKey];
        }

        public static bool IsTemplateOnly(this Entity owner)
        {
            return owner.MetaData<bool>(EntityKeys.IsTemplateOnly);
        }

        public static IEnumerable<Entity> Hierarchy(this Entity owner)
        {
            return owner[EntityKeys.HiearchyKey];
        }

        public static IEnumerable<Entity> Related(this Entity owner)
        {
            return owner[EntityKeys.RelatedKey];
        }
    }
}
