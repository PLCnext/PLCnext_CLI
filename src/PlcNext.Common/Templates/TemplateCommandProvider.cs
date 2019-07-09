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
using System.Threading.Tasks;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Templates
{
    internal class TemplateCommandProvider : IDynamicCommandProvider
    {
        private readonly ITemplateCommandBuilder templateCommandBuilder;
        private readonly ITemplateRepository repository;
        private readonly IEntityFactory entityFactory;
        private readonly ITemplateFileGenerator templateFileGenerator;
        private readonly IUserInterface userInterface;

        private CommandDefinition newCommand;
        private readonly List<CommandDefinition> generateCommands = new List<CommandDefinition>();

        public TemplateCommandProvider(ITemplateCommandBuilder templateCommandBuilder, ITemplateRepository repository, IEntityFactory entityFactory, ITemplateFileGenerator templateFileGenerator, IUserInterface userInterface)
        {
            this.templateCommandBuilder = templateCommandBuilder;
            this.repository = repository;
            this.entityFactory = entityFactory;
            this.templateFileGenerator = templateFileGenerator;
            this.userInterface = userInterface;
        }

        public IEnumerable<CommandDefinition> GetCommands(IEnumerable<string> context)
        {
            context = context.ToArray();
            if (!context.Any())
            {
                newCommand = CommandDefinitionBuilder.Create()
                                                     .SetName("new")
                                                     .SetHelp("Create new files / projects.")
                                                     .Build();
                return new[] {newCommand};
            }

            if (context.SequenceEqual(new[] {"new"}))
            {
                List<CommandDefinition> commandDefinitions = new List<CommandDefinition>();
                foreach (TemplateDescription template in repository.Templates)
                {
                    if (template.isHidden || commandDefinitions.Any(d => d.Name == template.name))
                    {
                        continue;
                    }
                    Entity templateEnitity = entityFactory.Create(template.name);
                    templateEnitity.SetMetaData(true, EntityKeys.IsTemplateOnly);
                    commandDefinitions.Add(templateCommandBuilder.GenerateTemplateCommandDefinition(templateEnitity, template, newCommand, repository.Templates));
                }
                return commandDefinitions;
            }

            if (context.SequenceEqual(new[] { "generate" }))
            {
                return CreateGenerateCommands();
            }

            return Enumerable.Empty<CommandDefinition>();

            IEnumerable<CommandDefinition> CreateGenerateCommands()
            {
                HashSet<string> generators = new HashSet<string>();
                foreach (TemplateDescription template in repository.Templates)
                {
                    if (template.isHidden)
                    {
                        continue;
                    }

                    foreach (string generator in template.GeneratedFile.Select(g => g.generator.ToLowerInvariant()))
                    {
                        generators.Add(generator);
                    }
                }

                generateCommands.Clear();
                foreach (string generator in generators)
                {
                    generateCommands.Add(AddArguments(CommandDefinitionBuilder.Create()
                                                                              .SetName(generator)
                                                                              .SetHelp($"Generates all files with the '{generator}' generator.")
                                                                              .AddExample($"generate {generator} --{EntityKeys.PathKey} Path/To/Project",
                                                                                          $"generate all {generator} files in default location."))
                    );
                }

                generateCommands.Add(AddArguments(CommandDefinitionBuilder.Create()
                                                                          .SetName("all")
                                                                          .SetHelp($"Generates all files.")
                                                                          .AddExample($"generate all --{EntityKeys.PathKey} Path/To/Project",
                                                                                      $"generate all files in default location."))
                                     );

                return generateCommands;

                CommandDefinition AddArguments(CommandDefinitionBuilder definitionBuilder)
                {
                    return definitionBuilder.CreateArgument()
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
                                            .SetName(Constants.OutputArgumentName)
                                            .SetShortName('o')
                                            .SetHelp("The path where the files will be generated in. " +
                                                     "Default is the 'intermediate' directory. Relative paths are relative " +
                                                     "to the directory defined with the '--path' option.")
                                            .SetArgumentType(ArgumentType.SingleValue)
                                            .Build()
                                            .Build();
                }
            }
        }

        public bool CanExecute(CommandDefinition definition)
        {
            return definition.BaseDefinition == newCommand ||
                   generateCommands.Contains(definition);
        }

        public async Task<int> Execute(CommandDefinition definition, ChangeObservable observable)
        {
            if (definition.BaseDefinition == newCommand)
            {
                Entity dataModel = entityFactory.Create(definition.Name, definition);
                IEnumerable<VirtualFile> files = await templateFileGenerator.InitalizeTemplate(dataModel, observable);

                userInterface.WriteInformation($"Successfully created template '{dataModel.Template().name}' in {GetCommonPath(files,dataModel.Path)}.");
            }

            if (generateCommands.Contains(definition))
            {
                Entity dataModel = entityFactory.Create(definition.Name, definition);
                SingleValueArgument singleValueArgument = definition.Argument<SingleValueArgument>(Constants.OutputArgumentName);
                await templateFileGenerator.GenerateFiles(dataModel.Root, definition.Name, singleValueArgument.Value, singleValueArgument.IsDefined, observable);

                userInterface.WriteInformation(definition.Name == "all"
                                                   ? $"Successfully generated all files for {dataModel.Root.Path}."
                                                   : $"Successfully generated all files with the '{definition.Name}' " +
                                                     $"generator for {dataModel.Root.Path}.");
            }

            return 0;

            string GetCommonPath(IEnumerable<VirtualFile> generatedFiles, string fallback)
            {
                IEnumerable<IEnumerable<VirtualDirectory>> paths = generatedFiles.Select(GetPath);
                VirtualDirectory commonDirectory = paths.Transpose()
                                                        .TakeWhile(d => d.Distinct().Count() == 1)
                                                        .FirstOrDefault()
                                                       ?.First();
                return commonDirectory?.FullName ?? fallback;

                IEnumerable<VirtualDirectory> GetPath(VirtualFile file)
                {
                    VirtualDirectory current = file.Parent;
                    while (current != null)
                    {
                        yield return current;
                        current = current.Parent;
                    }
                }
            }
        }
    }
}
