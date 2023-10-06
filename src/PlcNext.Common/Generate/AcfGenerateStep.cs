#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlcNext.Common.Generate
{
    class AcfGenerateStep : IGenerateStep
    {
        private readonly ExecutionContext executionContext;

        public string Identifier => "AcfGenerateStep";

        public AcfGenerateStep(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            if (!project.Settings.IsPersistent)
            {
                try
                {
                    ICodeModel codeModel = dataModel.Root.Value<ICodeModel>();
                    if (codeModel != null)
                    {
                        VirtualEntry acfFile = codeModel.SourceDirectories
                                 .SelectMany(directory => directory.Entries)
                                 .Where(entry => entry.Name.EndsWith(Constants.AcfConfigExtension, StringComparison.OrdinalIgnoreCase))
                                 .FirstOrDefault();
                        if (acfFile != null)
                        {
                            using (Stream xmlStream = (acfFile as VirtualFile).OpenRead())
                            using (XmlReader reader = XmlReader.Create(xmlStream))
                            {
                                while (reader.Read())
                                {
                                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "Component")
                                    {
                                        string componenttype = reader.GetAttribute("type");
                                        if(componenttype.Contains("::", StringComparison.Ordinal))
                                        {
                                            throw new OldAcfConfigException();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (XmlException)
                {
                    executionContext.WriteWarning("From version 2021.6 on the component type attribute inside the .acf.config file" +
                                              " must use the namespace separator '.' instead of '::'!");
                }
            }
        }
    }
}
