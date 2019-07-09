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
using System.Text;

namespace PlcNext.Common.Templates
{
    internal interface ITemplateLoader
    {
        IReadOnlyCollection<TemplateLoaderResult> LoadTemplates();
    }

    internal class TemplateLoaderResult
    {
        public TemplateLoaderResult(object template, string templateLocation)
        {
            Template = template;
            TemplateLocation = templateLocation;
        }

        public object Template { get; }
        public string TemplateLocation { get; }
    }
}
