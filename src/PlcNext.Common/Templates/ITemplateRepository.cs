﻿#region Copyright
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
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Templates.Format;

namespace PlcNext.Common.Templates
{
    public interface ITemplateRepository
    {
        IEnumerable<TemplateDescription> Templates { get; }
        IEnumerable<metaDataTemplate> FieldTemplates { get; }
        IEnumerable<formatTemplate> FormatTemplates { get; }
        IEnumerable<Type.metaDataTemplate> TypeTemplates { get; }
        TemplateDescription Template(string name);
        string GetTemplateBase(TemplateDescription templateDescription);
        IEnumerable<string> GetTemplateBases(TemplateDescription templateDescription);
    }
}
