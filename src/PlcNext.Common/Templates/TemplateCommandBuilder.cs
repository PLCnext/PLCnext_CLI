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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Templates
{
    internal class TemplateCommandBuilder : ITemplateCommandBuilder
    {
        public const string ForcedArgumentName = "force";
        private readonly ITemplateResolver resolver;

        public TemplateCommandBuilder(ITemplateResolver resolver)
        {
            this.resolver = resolver;
        }

        public CommandDefinition GenerateTemplateCommandDefinition(Entity templateEntity, TemplateDescription description,
                                                                   CommandDefinition baseCommand,
                                                                   IEnumerable<TemplateDescription> otherTemplates)
        {
            otherTemplates = otherTemplates.ToArray();
            CommandDefinitionBuilder builder = CommandDefinitionBuilder.Create()
                                                                           .SetName(description.name.ToLowerInvariant())
                                                                           .SetHelp(resolver.Resolve(description.Description ?? string.Empty,
                                                                                                     templateEntity))
                                                                           .SetBaseDefintion(baseCommand);
            List<char> shortNames = new List<char>(new []{'f'});
            foreach (templateArgumentDefinition argument in description.Arguments ?? Enumerable.Empty<templateArgumentDefinition>())
            {
                ArgumentBuilder argumentBuilder = builder.CreateArgument()
                                                         .SetName(argument.name)
                                                         .SetArgumentType(GetArgumentType(argument))
                                                         .SetHelp(resolver.Resolve(argument.help ?? string.Empty, templateEntity))
                                                         .SetRestriction(new ArgumentRestriction(argument.ValueRestriction).Verify);
                if (!string.IsNullOrEmpty(argument.shortname) && 
                    !shortNames.Contains(argument.shortname[0]))
                {
                    argumentBuilder.SetShortName(argument.shortname[0]);
                    shortNames.Add(argument.shortname[0]);
                }

                argumentBuilder.Build();
            }

            foreach (templateRelationship relationship in description.Relationship??Enumerable.Empty<templateRelationship>())
            {
                if (otherTemplates.FirstOrDefault(t => t.name.Equals(relationship.@type,
                                                                     StringComparison.OrdinalIgnoreCase))
                                 ?.isRoot == true)
                {
                    continue;
                }
                ArgumentType argumentType = relationship.multiplicity == multiplicity.One
                                                ? ArgumentType.SingleValue
                                                : ArgumentType.MultipleValue;

                ArgumentBuilder argBuilder = builder.CreateArgument()
                                                    .SetName(relationship.name)
                                                    .SetArgumentType(argumentType)
                                                    .SetMandatory()
                                                    .SetHelp(
                                                         $"Specifies the {relationship.name} of the {description.name}. " +
                                                         $"The {relationship.name} must be an " +
                                                         $"existing {relationship.type}.");
                char shortName = relationship.name[0];
                if (!shortNames.Contains(shortName))
                {
                    shortNames.Add(shortName);
                    argBuilder.SetShortName(shortName);
                }

                argBuilder.Build();
            }
            
            foreach (templateExample example in description.Example??Enumerable.Empty<templateExample>())
            {
                StringBuilder command = new StringBuilder($"new {description.name.ToLowerInvariant()}");
                foreach (templateArgumentInstance argumentInstance in example.Arguments??Enumerable.Empty<templateArgumentInstance>())
                {
                    if (description.Arguments?.Any(a => a.name == argumentInstance.name) != true)
                    {
                        throw new ArgumentNotFoundException(argumentInstance.name, description.name);
                    }
                    string value = argumentInstance.value.Contains(' ')
                                       ? $"\"{argumentInstance.value}\""
                                       : argumentInstance.value;
                    command.Append(argumentInstance.valueSpecified
                                       ? $" --{argumentInstance.name.ToLowerInvariant()} {value}"
                                       : $" --{argumentInstance.name.ToLowerInvariant()}");
                }

                foreach (templateRelationshipInstance relationshipInstance in example.Relationship??Enumerable.Empty<templateRelationshipInstance>())
                {
                    if (description.Relationship?.Any(r => r.name == relationshipInstance.name) != true)
                    {
                        throw new RelationshipNotFoundException(relationshipInstance.name, description.name);
                    }
                    string value = relationshipInstance.value.Contains(' ')
                                       ? $"\"{relationshipInstance.value}\""
                                       : relationshipInstance.value;
                    command.Append($" --{relationshipInstance.name.ToLowerInvariant()} {value}");
                }

                builder.AddExample(command.ToString(), example.Description ?? string.Empty);
            }

            builder.CreateArgument()
                   .SetName(ForcedArgumentName)
                   .SetShortName('f')
                   .SetArgumentType(ArgumentType.Bool)
                   .SetHelp("Overrides existing files when encountered.")
                   .Build();

            return builder.Build();

            ArgumentType GetArgumentType(templateArgumentDefinition argument)
            {
                return argument.hasvalue
                           ? (argument.multiplicity == multiplicity.OneOrMore
                                  ? ArgumentType.MultipleValue
                                  : ArgumentType.SingleValue)
                           : ArgumentType.Bool;
            }
        }

        private class ArgumentRestriction
        {
            private readonly valueRestriction valueRestriction;

            public ArgumentRestriction(valueRestriction valueRestriction)
            {
                this.valueRestriction = valueRestriction;
            }

            public (bool success, string message, string newValue) Verify(string value)
            {
                return valueRestriction?.Verify(value) ?? (true, null, value);
            }
        }
    }
}
