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
using PlcNext.Common.Tools;
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

        public CommandDefinition GenerateNewCommandDefinition(Entity templateEntity, TemplateDescription description,
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
                GenerateArgumentFromDefinition(templateEntity, builder, argument, shortNames);
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
        }

        private void GenerateArgumentFromDefinition(Entity templateEntity, CommandDefinitionBuilder builder,
                                                    templateArgumentDefinition templateArgumentDefinition, List<char> shortNames,
                                                    string setName = null)
        {
            ArgumentBuilder argumentBuilder = builder.CreateArgument()
                                                     .SetName(templateArgumentDefinition.name)
                                                     .SetArgumentType(GetArgumentType(templateArgumentDefinition))
                                                     .SetHelp(resolver.Resolve(
                                                                  templateArgumentDefinition.help ?? string.Empty,
                                                                  templateEntity))
                                                     .SetRestriction(
                                                          new ArgumentRestriction(
                                                              templateArgumentDefinition.ValueRestriction).Verify);
            if (templateArgumentDefinition.separatorSpecified &&
                !string.IsNullOrEmpty(templateArgumentDefinition.separator))
            {
                argumentBuilder.SetSeparator(templateArgumentDefinition.separator[0]);
            }
            if (!string.IsNullOrEmpty(templateArgumentDefinition.shortname) &&
                !shortNames.Contains(templateArgumentDefinition.shortname[0]))
            {
                argumentBuilder.SetShortName(templateArgumentDefinition.shortname[0]);
                shortNames.Add(templateArgumentDefinition.shortname[0]);
            }

            if (!string.IsNullOrEmpty(setName))
            {
                argumentBuilder.SetSetName(setName);
            }

            argumentBuilder.Build();

            ArgumentType GetArgumentType(templateArgumentDefinition argument)
            {
                return argument.hasvalue
                           ? (argument.multiplicity == multiplicity.OneOrMore
                                  ? ArgumentType.MultipleValue
                                  : ArgumentType.SingleValue)
                           : ArgumentType.Bool;
            }
        }

        public CommandDefinition GenerateDeployCommandDefinition(Entity templateEntity, TemplateDescription currentRootTemplate,
                                                                 CommandDefinition baseCommand, ICollection<TemplateDescription> allTemplates)
        {
            if (baseCommand == null)
            {
                return GenerateBaseCommand();
            }

            return GenerateChildCommand();

            CommandDefinition GenerateBaseCommand()
            {
                List<char> shortNames = new List<char>();
                return AddTemplateArguments(AddDefaultArguments(CommandDefinitionBuilder.Create()
                                                                                        .SetName("deploy")
                                                                                        .SetHelp(
                                                                                             "Deploys files for production.")
                                                                                        .EnableUseChildVerbsAsCategory(),
                                                                allTemplates.Where(t => !t.isHidden & t.isRoot),
                                                                shortNames),
                                            allTemplates.Where(t => !t.isHidden & t.isRoot),
                                            shortNames)
                   .Build();
            }

            CommandDefinition GenerateChildCommand()
            {
                List<char> shortNames = new List<char>();
                return AddTemplateArguments(AddDefaultArguments(CommandDefinitionBuilder.Create()
                                                                                        .SetBaseDefintion(baseCommand)
                                                                                        .SetName(currentRootTemplate
                                                                                                    .name)
                                                                                        .SetHelp(
                                                                                             $"For {currentRootTemplate.name.Plural()}:"),
                                                                new[] { currentRootTemplate },
                                                                shortNames),
                                            new[] { currentRootTemplate }, shortNames)
                   .Build();
            }

            CommandDefinitionBuilder AddTemplateArguments(CommandDefinitionBuilder builder, IEnumerable<TemplateDescription> descriptions,
                List<char> shortNames)
            {

                string[] defaultArguments =
                {
                    EntityKeys.PathKey.ToLowerInvariant(),
                    Constants.OutputArgumentName.ToLowerInvariant(),
                    Constants.FilesArgumentName.ToLowerInvariant(),
                    Constants.TargetArgumentName.ToLowerInvariant(),
                };
                Dictionary<templateArgumentDefinition, string> argumentToStringDictionary = new Dictionary<templateArgumentDefinition, string>();
                List<templateArgumentDefinition> setlessArguments = new List<templateArgumentDefinition>();
                foreach (TemplateDescription description in descriptions)
                {
                    IEnumerable<templateArgumentDefinition> arguments = (description.DeployPostStep ?? Enumerable.Empty<templateDeployPostStep>())
                       .SelectMany(d => d.Arguments);
                    foreach (templateArgumentDefinition argument in arguments)
                    {
                        if (!defaultArguments.Contains(argument.name.ToLowerInvariant()))
                        {
                            templateArgumentDefinition existingDefinition = argumentToStringDictionary.Keys
                                                                                                      .FirstOrDefault(d => d.name
                                                                                                                            .Equals(argument.name, 
                                                                                                                                    StringComparison.OrdinalIgnoreCase));
                            if (existingDefinition != null)
                            {
                                if (argumentToStringDictionary[existingDefinition] != description.name)
                                {
                                    argumentToStringDictionary.Remove(existingDefinition);
                                    setlessArguments.Add(existingDefinition);
                                }
                            }
                            else
                            {
                                argumentToStringDictionary.Add(argument, description.name);
                            }
                        }
                    }
                }

                foreach (templateArgumentDefinition argument in setlessArguments)
                {
                    GenerateArgumentFromDefinition(templateEntity, builder, argument, shortNames, null);
                }

                foreach (templateArgumentDefinition argument in argumentToStringDictionary.Keys)
                {
                    GenerateArgumentFromDefinition(templateEntity, builder, argument, shortNames, argumentToStringDictionary[argument]);
                }
                return builder;
            }

            CommandDefinitionBuilder AddDefaultArguments(CommandDefinitionBuilder builder, IEnumerable<TemplateDescription> descriptions, List<char> shortNames)
            {
                IEnumerable<templateArgumentDefinition> arguments = descriptions.SelectMany(x => (x.DeployPostStep ?? Enumerable.Empty<templateDeployPostStep>())
                       .SelectMany(d => d.Arguments));

                if (!ArgumentAvailableFromDescription(EntityKeys.PathKey))
                {
                    builder = builder.CreateArgument()
                                              .SetName(EntityKeys.PathKey)
                                              .SetShortName('p')
                                              .SetHelp(
                                                   "The path to the project settings file or the project root directory. " +
                                                   "Default is the current directory.")
                                              .SetArgumentType(ArgumentType.SingleValue)
                                              .Build();
                    shortNames.Add('p');
                }


                if (!ArgumentAvailableFromDescription(Constants.OutputArgumentName))
                {
                    builder = builder.CreateArgument()
                                                      .SetName(Constants.OutputArgumentName)
                                                      .SetShortName('o')
                                                      .SetHelp("The path where the files will be deployed in. " +
                                                               "Default is the 'bin/target/Release' directory. Relative paths are relative " +
                                                               "to the directory defined with the '--path' option.")
                                                      .SetArgumentType(ArgumentType.SingleValue)
                                                      .Build();
                    shortNames.Add('o');
                }

                if (!ArgumentAvailableFromDescription(Constants.FilesArgumentName))
                {
                    builder = builder.CreateArgument()
                              .SetName(Constants.FilesArgumentName)
                              .SetShortName('f')
                              .SetHelp(
                                   "Additional files to be deployed. Files are separated by ' '. Files need to be defined in the following format: " +
                                   "path/to/source|path/to/destination(|target). The path/to/source file will be relative to the " +
                                   $"root folder, defined with the '{EntityKeys.PathKey}' argument. The path/to/destination will be " +
                                   $"relative to the output/targetname/Release directory, where output is defined with the '{Constants.OutputArgumentName}' argument. " +
                                   "Optionally each file can have a target definition. Without the target definition the file is deployed to every target. " +
                                   "Targets need to be defined in the following format: targetname(,targetversion). The version is optional and " +
                                   "is only needed if there are multiple versions of a target in the project. The target must match one " +
                                   $"of the targets of the project or one of the targets defined with the '{Constants.TargetArgumentName}' " +
                                   "option, if it is defined.")
                              .SetArgumentType(ArgumentType.MultipleValue)
                              .SetSeparator(' ')
                              .Build();
                    shortNames.Add('f');
                }

                if (!ArgumentAvailableFromDescription(Constants.TargetArgumentName))
                {
                    builder = builder.CreateArgument()
                              .SetName(Constants.TargetArgumentName)
                              .SetShortName('t')
                              .SetHelp(
                                   "List of targets for which files are deployed. Targets are separated by ' '. " +
                                   "Targets need to be defined in the following format: targetname(,targetversion). The version is optional and " +
                                   "is only needed if there are multiple versions of a target in the registered SDKs. " +
                                   "If used, this option overrides the targets defined in the project. Please consider, that the actual binaries " +
                                   "are deployed in the build step. Therefore if they are not built for a specific target, neither are they built here.")
                              .SetArgumentType(ArgumentType.MultipleValue)
                              .SetSeparator(' ')
                              .Build();
                    shortNames.Add('t');
                }

                if (!ArgumentAvailableFromDescription(Constants.BuildTypeArgumentName))
                {
                    builder = builder.CreateArgument()
                              .SetName(Constants.BuildTypeArgumentName)
                              .SetShortName('b')
                              .SetHelp(
                                   "Build type for which the deploy should be executed. Default is 'Release'")
                              .SetArgumentType(ArgumentType.SingleValue)
                              .Build();
                    shortNames.Add('b');
                }

                builder.AddExample("deploy --path MyAcfProject", "Deploy the acf.config for 'MyAcfProject'");
                builder.AddExample("deploy --path MyPlmProject", "Deploy a .pcwlx library for 'MyPlmProject'");
                builder.AddExample($"deploy --path Path/To/Project", "Deploy library for all targets supported by project");
                builder.AddExample($"deploy --path Path/To/Project --target AXCF2152 RFC4072S", "Deploy library for targets AXCF2152 and RFC4072S");
                builder.AddExample($"deploy --path Path/To/Project --target AXCF2152,2019.0,path/to/Project.so",
                                    "Deploy library for target with compilation file in special location");
                builder.AddExample($"deploy --path Path/To/Project " +
                    $"--externalLibraries AXCF2152,2019.0,path/to/libforaxc.so,path/to/otherlib.so RFC4072S,path/to/libfornfc.so",
                    "Deploy library with external libraries");
                return builder;

                bool ArgumentAvailableFromDescription(string argumentName)
                {
                    templateArgumentDefinition argument = arguments.Where(a => a.name.Equals(argumentName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (argument == null)
                        return false;
                    GenerateArgumentFromDefinition(templateEntity, builder, argument, shortNames);
                    return true;
                }
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
