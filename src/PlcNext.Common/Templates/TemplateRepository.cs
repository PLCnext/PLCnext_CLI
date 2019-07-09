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
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Templates.Format;

namespace PlcNext.Common.Templates
{
    internal class TemplateRepository : ITemplateRepository
    {
        private readonly ITemplateLoader templateLoader;

        private Dictionary<TemplateDescription, string> templates;
        private FieldTemplates[] fieldTemplates;
        private FormatTemplates[] formatTemplates;

        public TemplateRepository(ITemplateLoader templateLoader)
        {
            this.templateLoader = templateLoader;
        }

        public IEnumerable<TemplateDescription> Templates
        {
            get
            {
                EnsureTemplates();
                return templates.Keys;
            }
        }

        public IEnumerable<fieldTemplate> FieldTemplates
        {
            get
            {
                EnsureTemplates();
                return fieldTemplates.SelectMany(t => t.FieldTemplate);
            }
        }

        public IEnumerable<formatTemplate> FormatTemplates
        {
            get
            {
                EnsureTemplates();
                return formatTemplates.SelectMany(t => t.FormatTemplate);
            }
        }

        public TemplateDescription Template(string name)
        {
            return Templates.FirstOrDefault(t => t.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public string GetTemplateBase(TemplateDescription template)
        {
            return templates[template];
        }

        private void EnsureTemplates()
        {
            if (templates != null)
            {
                return;
            }
            
            IReadOnlyCollection<TemplateLoaderResult> result = templateLoader.LoadTemplates();
            templates = result.Where(r => r.Template is TemplateDescription)
                              .ToDictionary(r => (TemplateDescription)r.Template,r => r.TemplateLocation);
            fieldTemplates = result.Select(r => r.Template)
                                   .OfType<FieldTemplates>()
                                   .ToArray();
            formatTemplates = result.Select(r => r.Template)
                                    .OfType<FormatTemplates>()
                                    .ToArray();
        }
    }
}
