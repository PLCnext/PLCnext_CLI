#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Generate
{
    internal class FileGenerationHelper
    {
        private readonly ITemplateResolver resolver;
        private readonly ITemplateRepository repository;
        private readonly IFileSystem fileSystem;

        public FileGenerationHelper(ITemplateResolver resolver, ITemplateRepository repository, IFileSystem fileSystem)
        {
            this.resolver = resolver;
            this.repository = repository;
            this.fileSystem = fileSystem;
        }

        internal async Task<(string content, Encoding encoding)> GetResolvedTemplateContent(Entity dataModel, templateFile file, TemplateDescription template)
        {
            string content;
            Encoding encoding;
            VirtualFile virtualFile = fileSystem.GetFirstExistingFile(file.template, repository.GetTemplateBases(template));
            if (virtualFile?.Exists != true)
            {
                return default;
            }
            using (Stream fileStream = virtualFile.OpenRead())
            using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                encoding = reader.CurrentEncoding;
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            if (encoding.BodyName == Encoding.UTF8.BodyName)
            {
                encoding = new UTF8Encoding(false);
            }

            content = await resolver.ResolveAsync(content, dataModel).ConfigureAwait(false);
            return (content, encoding);
        }
    }
}
