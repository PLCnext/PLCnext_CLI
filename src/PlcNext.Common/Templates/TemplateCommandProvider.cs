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
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using PlcNext.Common.DataModel;
using PlcNext.Common.Deploy;
using PlcNext.Common.Generate;
using PlcNext.Common.Project;
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
        private readonly IIndex<string, ITemplateFileGenerator> templateFileGenerators;
        private readonly IUserInterface userInterface;
        private readonly IIndex<string, IDeployService> deployServices;
        private readonly IEnumerable<IDeployStep> deploySteps;
        private readonly ExecutionContext executionContext;
        private readonly IEnumerable<IGenerateStep> generateSteps;

        private CommandDefinition newCommand;
        private CommandDefinition deployCommand;
        private readonly List<CommandDefinition> generateCommands = new List<CommandDefinition>();

        public TemplateCommandProvider(ITemplateCommandBuilder templateCommandBuilder, ITemplateRepository repository,
            IEntityFactory entityFactory, IIndex<string, ITemplateFileGenerator> templateFileGenerators, IUserInterface userInterface,
            IIndex<string, IDeployService> deployServices, IEnumerable<IDeployStep> deploySteps, ExecutionContext executionContext,
            IEnumerable<IGenerateStep> generateSteps)
        {
            this.templateCommandBuilder = templateCommandBuilder;
            this.repository = repository;
            this.entityFactory = entityFactory;
            this.templateFileGenerators = templateFileGenerators;
            this.userInterface = userInterface;
            this.deployServices = deployServices;
            this.deploySteps = deploySteps;
            this.executionContext = executionContext;
            this.generateSteps = generateSteps;
        }

        public IEnumerable<CommandDefinition> GetCommands(IEnumerable<string> context)
        {
            context = context.ToArray();
            if (!context.Any())
            {
                newCommand = CommandDefinitionBuilder.Create()
                                                     .SetName("new")
                                                     .SetHelp("Creates new files / projects.")
                                                     .Build();
                deployCommand = templateCommandBuilder.GenerateDeployCommandDefinition(null, null, null,repository.Templates.ToArray());
                return new[] {newCommand, deployCommand};
            }

            if (context.SequenceEqual(new[] {"new"}))
            {
                List<CommandDefinition> commandDefinitions = new List<CommandDefinition>();
                foreach (TemplateDescription template in repository.Templates.OrderBy(tmpl => tmpl.name))
                {
                    if (template.isHidden || commandDefinitions.Any(d => d.Name == template.name))
                    {
                        continue;
                    }
                    Entity templateEntity = entityFactory.Create(template.name);
                    templateEntity.SetMetaData(true, EntityKeys.IsTemplateOnly);
                    commandDefinitions.Add(templateCommandBuilder.GenerateNewCommandDefinition(templateEntity, template, newCommand, repository.Templates));
                }
                return commandDefinitions;
            }

            if (context.SequenceEqual(new[] {"deploy"}))
            {
                List<CommandDefinition> commandDefinitions = new List<CommandDefinition>();
                foreach (TemplateDescription template in repository.Templates.OrderBy(tmpl => tmpl.name))
                {
                    if (template.isHidden || 
                        commandDefinitions.Any(d => d.Name == template.name) ||
                        !template.isRoot)
                    {
                        continue;
                    }
                    Entity templateEntity = entityFactory.Create(template.name);
                    templateEntity.SetMetaData(true, EntityKeys.IsTemplateOnly);
                    commandDefinitions.Add(templateCommandBuilder.GenerateDeployCommandDefinition(templateEntity,
                                                                                                  template,
                                                                                                  deployCommand,
                                                                                                  repository.Templates.ToArray()));
                }
                return commandDefinitions;
            }

            if (context.SequenceEqual(new[] { "generate" }))
            {
                return templateCommandBuilder.CreateGenerateCommandDefinitions(generateCommands, repository.Templates);
            }

            return Enumerable.Empty<CommandDefinition>();

            }

        public bool CanExecute(CommandDefinition definition)
        {
            return definition.BaseDefinition == newCommand ||
                   generateCommands.Contains(definition) ||
                   definition == deployCommand;
        }

        public async Task<int> Execute(CommandDefinition definition, ChangeObservable observable)
        {
            if (definition.BaseDefinition == newCommand)
            {
                Entity dataModel = entityFactory.Create(definition.Name, definition);
                TemplateEntity templateEntity = TemplateEntity.Decorate(dataModel);
                IEnumerable<VirtualFile> files = await templateFileGenerators[templateEntity.Template.generateEngine].InitalizeTemplate(dataModel, observable).ConfigureAwait(false);

                userInterface.WriteInformation($"Successfully created template '{dataModel.Template().name}' in {GetCommonPath(files,dataModel.Path)}.");
            }

            if (generateCommands.Contains(definition))
            {
                Entity dataModel = entityFactory.Create(definition.Name, definition);
                Entity root = dataModel.Root;
                ProjectEntity project = ProjectEntity.Decorate(root);
                if (project.Version.Major > project.ToolProjectVersion.Major)
                {
                    throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
                }

                userInterface.WriteInformation(definition.Name == "all"
                                                   ? $"Generating all files for {dataModel.Root.Path}."
                                                   : $"Generating all files with the '{definition.Name}' " +
                                                     $"generator for {dataModel.Root.Path}.");

                SingleValueArgument singleValueArgument = definition.Argument<SingleValueArgument>(Constants.OutputArgumentName);
                TemplateEntity templateEntity = TemplateEntity.Decorate(dataModel.Root);
                await templateFileGenerators[templateEntity.Template.generateEngine].GenerateFiles(dataModel.Root, definition.Name, singleValueArgument.Value, singleValueArgument.IsDefined, observable)
                                           .ConfigureAwait(false);
                TemplateDescription template = TemplateEntity.Decorate(root).Template;
                foreach (templateGenerateStep generateStep in template.GenerateStep?.Where(s => !String.IsNullOrEmpty(s.identifier)) ?? Enumerable.Empty<templateGenerateStep>())
                {
                    if (definition.Name == "all" || generateStep.generator == definition.Name)
                    {
                        IGenerateStep step = generateSteps.FirstOrDefault(s => s.Identifier == generateStep.identifier);
                        if (step != null)
                        {
                            step.Execute(dataModel, observable);
                        }
                        else
                        {
                            executionContext.WriteWarning(
                                $"Generate step '{generateStep.identifier}' could not be executed because there is no implementation. " +
                                $"Available implementations are:{Environment.NewLine}" +
                                $"{string.Join(Environment.NewLine, generateSteps.Select(d => d.Identifier))}");
                        }
                    }
                }

                userInterface.WriteInformation(definition.Name == "all"
                                                   ? $"Successfully generated all files for {dataModel.Root.Path}."
                                                   : $"Successfully generated all files with the '{definition.Name}' " +
                                                     $"generator for {dataModel.Root.Path}.");
            }

            if (definition == deployCommand)
            {
                Entity dataModel = entityFactory.Create(definition.Name, definition);
                ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
                if (project.Version.Major > project.ToolProjectVersion.Major)
                {
                    throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
                }
                
                TemplateEntity templateEntity = TemplateEntity.Decorate(dataModel.Root);
                deployServices[templateEntity.Template.deployEngine].DeployFiles(dataModel);

                Entity root = dataModel.Root;
                TemplateDescription template = TemplateEntity.Decorate(root).Template;
                foreach (templateDeployPostStep deployPostStep in template.DeployPostStep??Enumerable.Empty<templateDeployPostStep>())
                {
                    IDeployStep step = deploySteps.FirstOrDefault(s => s.Identifier == deployPostStep.identifier);
                    if (step != null)
                    {
                        step.Execute(dataModel, observable);
                    }
                    else
                    {
                        executionContext.WriteWarning(
                            $"Deploy post step '{deployPostStep.identifier}' could not be executed because there is no implementation. " +
                            $"Available implementations are:{Environment.NewLine}" +
                            $"{string.Join(Environment.NewLine, deploySteps.Select(d => d.Identifier))}");
                    }
                }

                userInterface.WriteInformation($"Successfully deployed all files for {dataModel.Root.Path}.");
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
