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
using System.Xml.Serialization;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Templates.Format;
using PlcNext.Common.Templates.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;

namespace PlcNext.Common.Templates
{
    internal class SettingsBasedTemplateLoader : ITemplateLoader
    {
        private readonly IFileSystem fileSystem;
        private readonly ISettingsProvider settingsProvider;
        private readonly IEnvironmentService environmentService;

        public SettingsBasedTemplateLoader(IFileSystem fileSystem, ISettingsProvider settingsProvider, IEnvironmentService environmentService)
        {
            this.fileSystem = fileSystem;
            this.settingsProvider = settingsProvider;
            this.environmentService = environmentService;
        }

        public IReadOnlyCollection<TemplateLoaderResult> LoadTemplates()
        {
            List<TemplateLoaderResult> result = new List<TemplateLoaderResult>();
            XmlSerializer templatesSerializer = new XmlSerializer(typeof(Templates.Templates));
            XmlSerializer templateDescriptionSerializer = new XmlSerializer(typeof(TemplateDescription));
            XmlSerializer fieldTemplateSerializer = new XmlSerializer(typeof(FieldTemplates));
            XmlSerializer formatTemplateSerializer = new XmlSerializer(typeof(FormatTemplates));

            foreach (string templateLocation in settingsProvider.Settings.TemplateLocations)
            {
                VirtualFile baseFile = fileSystem.GetFile(templateLocation, environmentService.AssemblyDirectory);
                Templates.Templates templates;
                using (Stream fileStream = baseFile.OpenRead())
                {
                    templates = (Templates.Templates)templatesSerializer.Deserialize(fileStream);
                }

                foreach (include include in templates.Include)
                {
                    string path = include.Value.Replace('\\', Path.DirectorySeparatorChar)
                                         .Replace('/', Path.DirectorySeparatorChar);
                    path = Path.Combine(baseFile.Parent.FullName, path);
                    using (Stream includeStream = fileSystem.GetFile(path).OpenRead())
                    {
                        switch (include.type)
                        {
                            case includeType.Template:
                                result.Add(new TemplateLoaderResult(
                                               templateDescriptionSerializer.Deserialize(includeStream),
                                               Path.GetDirectoryName(path)));
                                break;
                            case includeType.Fields:
                                result.Add(new TemplateLoaderResult(
                                               fieldTemplateSerializer.Deserialize(includeStream),
                                               Path.GetDirectoryName(path)));
                                break;
                            case includeType.Format:
                                result.Add(new TemplateLoaderResult(
                                               formatTemplateSerializer.Deserialize(includeStream),
                                               Path.GetDirectoryName(path)));
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                }
            }

            MergeTemplates(result.Select(r => r.Template)
                                 .OfType<TemplateDescription>()
                                 .ToArray());

            return result;

            void MergeTemplates(TemplateDescription[] descriptions)
            {
                List<TemplateDescription> merged = new List<TemplateDescription>();
                foreach (TemplateDescription templateDescription in descriptions)
                {
                    MergeTemplate(templateDescription);
                }

                void MergeTemplate(TemplateDescription templateDescription)
                {
                    TemplateDescription baseDescription = GetBaseDescription();
                    if (baseDescription != null)
                    {
                        if (!merged.Contains(baseDescription))
                        {
                            MergeTemplate(baseDescription);
                        }
                        templateDescription.Merge(baseDescription);
                    }
                    merged.Add(templateDescription);

                    TemplateDescription GetBaseDescription()
                    {
                        return descriptions.FirstOrDefault(d => d.name.Equals(templateDescription.basedOn,
                                                                              StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }
    }
}
