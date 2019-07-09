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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Templates
{
    internal interface ITemplateCommandBuilder
    {
        CommandDefinition GenerateTemplateCommandDefinition(Entity templateEntity, TemplateDescription template,
                                                            CommandDefinition baseCommand,
                                                            IEnumerable<TemplateDescription> otherTemplates);
    }
}
