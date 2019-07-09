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
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Templates.Format;

namespace PlcNext.Common.Templates
{
    public interface ITemplateRepository
    {
        IEnumerable<TemplateDescription> Templates { get; }
        IEnumerable<fieldTemplate> FieldTemplates { get; }
        IEnumerable<formatTemplate> FormatTemplates { get; }
        TemplateDescription Template(string name);
        string GetTemplateBase(TemplateDescription template);
    }
}
