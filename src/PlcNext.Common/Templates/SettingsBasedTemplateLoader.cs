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
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Templates.Format;
using PlcNext.Common.Templates.Templates;
using PlcNext.Common.Templates.Type;
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
        private readonly ExecutionContext executionContext;

        public SettingsBasedTemplateLoader(IFileSystem fileSystem, ISettingsProvider settingsProvider, IEnvironmentService environmentService,
                                            ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.settingsProvider = settingsProvider;
            this.environmentService = environmentService;
            this.executionContext = executionContext;
        }

        public IReadOnlyCollection<TemplateLoaderResult> LoadTemplates()
        {
            List<TemplateLoaderResult> result = new List<TemplateLoaderResult>();
            XmlSerializer templatesSerializer = new XmlSerializer(typeof(Templates.Templates));
            XmlSerializer templateDescriptionSerializer = new XmlSerializer(typeof(TemplateDescription));
            XmlSerializer fieldTemplateSerializer = new XmlSerializer(typeof(FieldTemplates));
            XmlSerializer typeTemplateSerializer = new XmlSerializer(typeof(TypeTemplates));
            XmlSerializer formatTemplateSerializer = new XmlSerializer(typeof(FormatTemplates));

            foreach (string templateLocation in settingsProvider.Settings.TemplateLocations)
            {
                try
                {
                    if (!fileSystem.FileExists(templateLocation, environmentService.AssemblyDirectory))
                    {
                        executionContext.WriteWarning($"The template location {templateLocation} does not point to an existing file." +
                                                      " Fix problems with template or remove the location from the settings.");
                        continue;
                    }

                    VirtualFile baseFile = fileSystem.GetFile(templateLocation, environmentService.AssemblyDirectory);
                    Templates.Templates templates;
                    using (Stream fileStream = baseFile.OpenRead())
                    using (XmlReader reader = XmlReader.Create(fileStream))
                    {
                        templates = (Templates.Templates)templatesSerializer.Deserialize(reader);
                    }

                    foreach (include include in templates.Include)
                    {
                        string path = include.Value.Replace('\\', Path.DirectorySeparatorChar)
                            .Replace('/', Path.DirectorySeparatorChar);
                        path = Path.Combine(baseFile.Parent.FullName, path);
                        using (Stream includeStream = fileSystem.GetFile(path).OpenRead())
                        using (XmlReader reader = XmlReader.Create(includeStream))
                        {
                            switch (include.type)
                            {
                                case includeType.Template:
                                    result.Add(new TemplateLoaderResult(
                                        templateDescriptionSerializer.Deserialize(reader),
                                        Path.GetDirectoryName(path)));
                                    break;
                                case includeType.Fields:
                                    result.Add(new TemplateLoaderResult(
                                        fieldTemplateSerializer.Deserialize(reader),
                                        Path.GetDirectoryName(path)));
                                    break;
                                case includeType.Types:
                                    result.Add(new TemplateLoaderResult(
                                        typeTemplateSerializer.Deserialize(reader),
                                        Path.GetDirectoryName(path)));
                                    break;
                                case includeType.Format:
                                    result.Add(new TemplateLoaderResult(
                                        formatTemplateSerializer.Deserialize(reader),
                                        Path.GetDirectoryName(path)));
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    executionContext.WriteWarning($"The template {templateLocation} could not be loaded. See log for details.");
                    executionContext.WriteWarning($"Fix problems with template or remove template {templateLocation} from the settings", false);
                    executionContext.WriteError(e.ToString(), false);
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
