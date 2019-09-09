#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using System.Threading.Tasks;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.CommandLine
{
    public class DynamicVerb : VerbBase
    {
        public CommandDefinition Defintion { get; set; }

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            AddDefinitionContent();
            return await commandManager.Execute(AddDeprecatedInformation(new DynamicCommandArgs(Defintion)));

            void AddDefinitionContent()
            {
                foreach (PropertyInfo publicProperty in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    CommandDefinition definition = Defintion;
                    while (definition != null)
                    {
                        if (definition.Arguments.ContainsArgument(publicProperty.Name))
                        {
                            Argument argument = definition.Arguments[publicProperty.Name];
                            argument.SetValue(publicProperty.GetValue(this));
                        }
                        definition = definition.BaseDefinition;
                    }
                }
            }
        }
    }
}
