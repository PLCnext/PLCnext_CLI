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
using System.Globalization;
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
        public const string EmptyArgumentName = "empty";
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
                ParseExample(description, example, command);

                builder.AddExample(command.ToString(), example.Description ?? string.Empty);
            }

            builder.CreateArgument()
                .SetName(ForcedArgumentName)
                .SetShortName('f')
                .SetArgumentType(ArgumentType.Bool)
                .SetHelp("Overrides existing files when encountered.")
                .Build();
            
            builder.CreateArgument()
                .SetName(EmptyArgumentName)
                .SetShortName('e')
                .SetArgumentType(ArgumentType.Bool)
                .SetHelp("Do not provide code skeleton.")
                .Build();

            return builder.Build();
        }

        private static void ParseExample(TemplateDescription description, templateExample example, StringBuilder command, bool checkExample = true)
        {
            foreach (templateArgumentInstance argumentInstance in example.Arguments ??
                                                                  Enumerable.Empty<templateArgumentInstance>())
            {
                if (checkExample && description.Arguments?.Any(a => a.name == argumentInstance.name) != true)
                {
                    throw new ArgumentNotFoundException(argumentInstance.name, description.name);
                }

                command.Append(argumentInstance.valueSpecified
                                   ? $" --{argumentInstance.name.ToLowerInvariant()} {argumentInstance.value}"
                                   : $" --{argumentInstance.name.ToLowerInvariant()}");
            }

            foreach (templateRelationshipInstance relationshipInstance in example.Relationship ??
                                                                          Enumerable.Empty<templateRelationshipInstance>())
            {
                if (checkExample && description.Relationship?.Any(r => r.name == relationshipInstance.name) != true)
                {
                    throw new RelationshipNotFoundException(relationshipInstance.name, description.name);
                }

                string value = relationshipInstance.value.Contains(' ', StringComparison.Ordinal)
                                   ? $"\"{relationshipInstance.value}\""
                                   : relationshipInstance.value;
                command.Append(CultureInfo.InvariantCulture, $" --{relationshipInstance.name.ToLowerInvariant()} {value}");
            }
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
                                                                                        .EnableUseChildVerbsAsCategory()
                .AddExample($"deploy --path Path/To/Project", "Deploy files for all targets supported by project")
                .AddExample($"deploy --path Path/To/Project --files doc/HelpText.txt|doc", "Deploy doc/HelpText.txt to the doc directory in the deploy directory additionally to the normally deployed files.")
                .AddExample($"deploy --path Path/To/Project --target AXCF2152 RFC4072S", "Deploy library for targets AXCF2152 and RFC4072S"),
                                                                allTemplates.Where(t => !t.isHidden & t.isRoot),
                                                                shortNames),
                                            allTemplates.Where(t => !t.isHidden & t.isRoot),
                                            shortNames, false)
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
                                            new[] { currentRootTemplate }, shortNames, true)
                   .Build();
            }

            CommandDefinitionBuilder AddTemplateArguments(CommandDefinitionBuilder builder, IEnumerable<TemplateDescription> descriptions,
                List<char> shortNames, bool addExamples)
            {
                string[] defaultArguments =
                {
                    EntityKeys.PathKey.ToUpperInvariant(),
                    Constants.OutputArgumentName.ToUpperInvariant(),
                    Constants.FilesArgumentName.ToUpperInvariant(),
                    Constants.TargetArgumentName.ToUpperInvariant(),
                    Constants.BuildTypeArgumentName.ToUpperInvariant()
                };
                Dictionary<templateArgumentDefinition, string> argumentToStringDictionary = new Dictionary<templateArgumentDefinition, string>();
                List<templateArgumentDefinition> setlessArguments = new List<templateArgumentDefinition>();
                foreach (TemplateDescription description in descriptions)
                {
                    IEnumerable<templateArgumentDefinition> arguments = (description.DeployPostStep ?? Enumerable.Empty<templateDeployPostStep>())
                       .SelectMany(d => d.Arguments??Enumerable.Empty<templateArgumentDefinition>());
                    foreach (templateArgumentDefinition argument in arguments)
                    {
                        if (!defaultArguments.Contains(argument.name.ToUpperInvariant()))
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

                    if (!addExamples)
                    {
                        continue;
                    }
                    IEnumerable<templateExample> examples = (description.DeployPostStep ?? Enumerable.Empty<templateDeployPostStep>())
                                                            .SelectMany(d => d.Example?? Enumerable.Empty<templateExample>());
                    foreach (templateExample example in examples)
                    {
                        StringBuilder command = new StringBuilder("deploy");
                        ParseExample(description, example, command, false);

                        builder.AddExample(command.ToString(), example.Description ?? string.Empty);
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
                       .SelectMany(d => d.Arguments??Enumerable.Empty<templateArgumentDefinition>()));

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
                                   $"root folder, defined with the '{EntityKeys.PathKey}' argument. It can contain the wildcard '*' " +
                                   $"at the end of the path. In this case all files from the directory and all sub-directories will " +
                                   $"be deployed into the destination directory without recreating the directory structure. The path/to/destination will be " +
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

        public IEnumerable<CommandDefinition> CreateGenerateCommandDefinitions(ICollection<CommandDefinition> generateCommands, 
                                                                               IEnumerable<TemplateDescription> allTemplates)
        {
            HashSet<string> generators = new HashSet<string>();
            Dictionary<string, List<templateGenerateStep>> generatorsDictionary = new Dictionary<string, List<templateGenerateStep>>();
            foreach (TemplateDescription template in allTemplates)
            {
                if (template.isHidden)
                {
                    continue;
                }

                foreach (string generator in (template.GeneratedFile ?? Enumerable.Empty<templateGeneratedFile>())
                                                      .Select(g => g.generator.ToLowerInvariant()))
                {
                    generators.Add(generator);
                }

                foreach (string generator in generators)
                {
                    if (generatorsDictionary.TryGetValue(generator, out List<templateGenerateStep> list))
                    {
                        if (template.GenerateStep != null && template.GenerateStep.Where(step => step.generator == generator).Any())
                        {
                            if (list == null)
                            {
                                list = new List<templateGenerateStep>(template.GenerateStep.Where(step => step.generator == generator));
                                generatorsDictionary[generator] = list;
                            }
                            else
                                list.AddRange(template.GenerateStep.Where(step => step.generator == generator));
                        }
                    }
                    else
                    {
                        generatorsDictionary.Add(generator, template.GenerateStep != null
                                         ? new List<templateGenerateStep>(template.GenerateStep.Where(step => step.generator == generator))
                                         : null);
                    }
                }
            }

            generateCommands.Clear();
            foreach (string generator in generatorsDictionary.Keys)
            {
                generateCommands.Add(AddArguments(CommandDefinitionBuilder.Create()
                                                                          .SetName(generator)
                                                                          .SetHelp($"Generates all files with the '{generator}' generator.")
                                                                          .AddExample($"generate {generator} --{EntityKeys.PathKey} Path/To/Project",
                                                                                      $"generate all {generator} files in default location."),
                                                  generator,
                                                  generatorsDictionary[generator]));
            }

            generateCommands.Add(AddArguments(CommandDefinitionBuilder.Create()
                                                                      .SetName("all")
                                                                      .SetHelp($"Generates all files.")
                                                                      .AddExample($"generate all --{EntityKeys.PathKey} Path/To/Project",
                                                                                  $"generate all files in default location."),
                                                                      "all",
                                                                      allTemplates.Where(t => !t.isHidden && t.GenerateStep != null)
                                                                                  .SelectMany(t => t.GenerateStep)
                                                                                  .Where(t => t !=null)
                                                                      ));

            return generateCommands;

            CommandDefinition AddArguments(CommandDefinitionBuilder definitionBuilder, 
                                           string generatorName, 
                                           IEnumerable<templateGenerateStep> generateSteps = null)
            {
                definitionBuilder = definitionBuilder.CreateArgument()
                                        .SetName(EntityKeys.PathKey)
                                        .SetShortName('p')
                                        .SetHelp(
                                             "The path to the project settings file or the project root directory. " +
                                             "Default is the current directory.")
                                        .SetArgumentType(ArgumentType.SingleValue)
                                        .Build()
                                        .CreateArgument()
                                        .SetName(EntityKeys.SourceDirectoryKey)
                                        .SetShortName('s')
                                        .SetHelp(
                                             "The path of the source directories separated by ','. Default is the 'src' directory " +
                                             "if such a directory exists. If not, the directory " +
                                             "defined with the '--path' option is used. Relative paths are relative " +
                                             "to the directory defined with the '--path' option. If any path contains a ' ' quotation " +
                                             "marks should be used around all paths, e.g. \"path1,path With Space,path2\".")
                                        .SetArgumentType(ArgumentType.MultipleValue)
                                        .Build()
                                        .CreateArgument()
                                        .SetName(EntityKeys.IncludeDirectoryKey)
                                        .SetShortName('i')
                                        .SetHelp(
                                             "Overrides the includes used to find header files. Usually CMake is used to determine " +
                                             "the include paths. If that is not possible or wanted the value of this argument is " +
                                             "used instead of the CMake determined include paths. Relative paths are relative " +
                                             "to the directory defined with the '--path' option. If any path contains a ' ' quotation " +
                                             "marks should be used around all paths, e.g. \"path1,path With Space,path2\". Additionally " +
                                             "to these paths the include paths determined by the SDK will always be considered and " +
                                             "do not need to be specified additionally.")
                                        .SetArgumentType(ArgumentType.MultipleValue)
                                        .Build()
                                        .CreateArgument()
                                        .SetName(Constants.NoIncludePathDetection)
                                        .SetShortName('n')
                                        .SetHelp("Disables the automatic include path detection using CMake. This option is not necessary if the " +
                                                 $"'-{EntityKeys.IncludeDirectoryKey}' option is used, as that option will automatically disable " +
                                                 "the CMake detection. The system paths defined by the SDK are still used.")
                                        .SetArgumentType(ArgumentType.Bool)
                                        .Build()
                                        .CreateArgument()
                                        .SetName(Constants.OutputArgumentName)
                                        .SetShortName('o')
                                        .SetHelp("The path where the files will be generated in. " +
                                                 "Default is the 'intermediate' directory. Relative paths are relative " +
                                                 "to the directory defined with the '--path' option.")
                                        .SetArgumentType(ArgumentType.SingleValue)
                                        .Build();

                if (generateSteps != null)
                {
                    foreach (templateArgumentDefinition argument in generateSteps.Where(step => step.Arguments != null)
                                                                                .SelectMany(step => step.Arguments))
                    {
                        ArgumentBuilder argumentBuilder = definitionBuilder.CreateArgument()
                                                                .SetName(argument.name);
                        if (argument.shortnameSpecified)
                        {
                            argumentBuilder = argumentBuilder.SetShortName(argument.shortname[0]);
                        }
                        if (argument.helpSpecified)
                        {
                            argumentBuilder = argumentBuilder.SetHelp(argument.help);
                        }

                        ArgumentType argumentType = argument.hasvalueSpecified && argument.hasvalue
                                                    ? argument.multiplicitySpecified && argument.multiplicity == multiplicity.OneOrMore
                                                        ? ArgumentType.MultipleValue
                                                        : ArgumentType.SingleValue
                                                    : ArgumentType.Bool;
                        argumentBuilder = argumentBuilder.SetArgumentType(argumentType);
                        definitionBuilder = argumentBuilder.Build();
                    }

                    foreach (templateExample example in generateSteps.Where(step => step.Example != null)
                                                                     .SelectMany(step => step.Example))
                    {
                        StringBuilder exampleBuilder = new StringBuilder($"generate {generatorName.ToLowerInvariant()}");
                        ParseExample(null, example, exampleBuilder, false);
                        definitionBuilder.AddExample(exampleBuilder.ToString(), example.Description ?? string.Empty);
                    }
                }
                return definitionBuilder.Build();
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
