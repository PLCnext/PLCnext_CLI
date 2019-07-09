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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    public class TemplateEntity : EntityBaseDecorator
    {
        private TemplateEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static TemplateEntity Decorate(IEntityBase templateEntity)
        {
            return Decorate<TemplateEntity>(templateEntity) ?? new TemplateEntity(templateEntity);
        }

        public TemplateDescription Template => HasValue<TemplateDescription>()
                                                   ? Value<TemplateDescription>()
                                                   : this[EntityKeys.TemplateKey].Value<TemplateDescription>();

        public bool HasTemplate => HasValue<TemplateDescription>() ||
                                   HasContent(EntityKeys.TemplateKey) &&
                                   this[EntityKeys.TemplateKey].HasValue<TemplateDescription>();

        public IEnumerable<Entity> RelatedEntites => HasContent(EntityKeys.RelatedKey)
                                                         ? this[EntityKeys.RelatedKey]
                                                         : Enumerable.Empty<Entity>();

        public IEnumerable<Entity> EntityHierarchy => HasContent(EntityKeys.HiearchyKey)
                                                          ? this[EntityKeys.HiearchyKey]
                                                          : Enumerable.Empty<Entity>();

        public IEntityBase FormatOrigin
        {
            get
            {
                if (Type != EntityKeys.FormatKey)
                {
                    return this;
                }

                Entity result = Owner;
                while (result.Owner != null && result.Owner.Type == EntityKeys.FormatKey)
                {
                    result = result.Owner.Owner;
                }

                return result;
            }
        }
    }
}
