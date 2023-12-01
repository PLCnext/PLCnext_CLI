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
using PlcNext.Common.Templates.Type;
using PlcNext.Common.Tools;
using metaDataTemplate = PlcNext.Common.Templates.Field.metaDataTemplate;

namespace PlcNext.Common.Templates
{
    internal class TemplateRepository : ITemplateRepository
    {
        private readonly ITemplateLoader templateLoader;
        private readonly ExecutionContext executionContext;

        private Dictionary<TemplateDescription, string> templates;
        private FieldTemplates[] fieldTemplates;
        private FormatTemplates[] formatTemplates;
        private TypeTemplates[] typeTemplates;

        public TemplateRepository(ITemplateLoader templateLoader, ExecutionContext executionContext)
        {
            this.templateLoader = templateLoader;
            this.executionContext = executionContext;
        }

        public IEnumerable<TemplateDescription> Templates
        {
            get
            {
                EnsureTemplates();
                return templates.Keys;
            }
        }

        public IEnumerable<metaDataTemplate> FieldTemplates
        {
            get
            {
                EnsureTemplates();
                return fieldTemplates.SelectMany(t => t.FieldTemplate);
            }
        }

        public IEnumerable<Type.metaDataTemplate> TypeTemplates
        {
            get
            {
                EnsureTemplates();
                return typeTemplates.SelectMany(t => t.TypeTemplate);
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

        public string GetTemplateBase(TemplateDescription templateDescription)
        {
            return templates[templateDescription];
        }

        public IEnumerable<string> GetTemplateBases(TemplateDescription templateDescription)
        {
            yield return templates[templateDescription];
            while (!string.IsNullOrEmpty(templateDescription?.basedOn))
            {
                TemplateDescription basedOnTemplate = templates.Keys.FirstOrDefault(tmpl => tmpl.Description.Equals(templateDescription.basedOn, StringComparison.Ordinal));
                templateDescription = basedOnTemplate;
                if (basedOnTemplate != default)
                {
                    yield return templates[templateDescription];
                }
            }
        }

        private void EnsureTemplates()
        {
            if (templates != null)
            {
                return;
            }
            
            IReadOnlyCollection<TemplateLoaderResult> result = templateLoader.LoadTemplates();
            try
            {
                templates = result.Where(r => r.Template is TemplateDescription)
                    .ToDictionary(r => (TemplateDescription) r.Template, r => r.TemplateLocation);
                fieldTemplates = result.Select(r => r.Template)
                    .OfType<FieldTemplates>()
                    .ToArray();
                typeTemplates = result.Select(r => r.Template)
                    .OfType<TypeTemplates>()
                    .ToArray();
                formatTemplates = result.Select(r => r.Template)
                    .OfType<FormatTemplates>()
                    .ToArray();
            }
            catch (ArgumentException e)
            {
                executionContext.WriteError(e.ToString(), false);
                throw new FormattableException("Two templates with the same name exist. See log for more information.");
            }
        }
    }
}
