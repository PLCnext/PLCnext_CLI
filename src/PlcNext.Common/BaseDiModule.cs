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
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Features.AttributeFilters;
using PlcNext.Common.Build;
using PlcNext.Common.CodeModel;
using PlcNext.Common.CodeModel.Cpp;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Deploy;
using PlcNext.Common.Generate;
using PlcNext.Common.Installation;
using PlcNext.Common.Installation.SDK;
using PlcNext.Common.MetaData;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Priority;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.Security;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using ExecutionContext = PlcNext.Common.Tools.ExecutionContext;

namespace PlcNext.Common
{
    public class BaseDiModule : Module
    {
        private readonly bool noSdkExploration;
        private readonly bool activateAutoComponents;

        public BaseDiModule(bool noSdkExploration, bool activateAutoComponents = true)
        {
            this.noSdkExploration = noSdkExploration;
            this.activateAutoComponents = activateAutoComponents;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => PageStreamFactory.CreateDefault()).As<StreamFactory>()
                   .InstancePerLifetimeScope();
            builder.Register(_ => CancellationToken.None).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ExecutionContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CommandManager>().As<ICommandManager>().InstancePerLifetimeScope();
            builder.RegisterType<OutputFormatterPool>().As<IOutputFormatterPool>().InstancePerLifetimeScope();
            builder.RegisterType<PriorityHell>().As<IPriorityMaster>().InstancePerLifetimeScope();
            builder.RegisterType<BuildCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<GetCompilerSpecificationsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<GetSdksCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<GetSettingsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<GetTargetsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<GetProjectInformationCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<InstallSdkCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<ScanSdksCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<CheckProjectCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<SetSettingsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<SetTargetsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateTargetsCommand>().As<ICommand>().InstancePerLifetimeScope();
            builder.RegisterType<TargetParser>().As<ITargetParser>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<AutomaticRollbackTransactionFactory>().As<ITransactionFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StaticDatatypeConversion>().As<IDatatypeConversion>().InstancePerLifetimeScope();
            builder.RegisterType<SettingsBasedSdkRepository>().As<ISdkRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CmakeExecuter>().As<IBuildExecuter>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CmakeBuildInformationService>().As<IBuildInformationService>().InstancePerLifetimeScope();
            builder.RegisterType<Builder>().Keyed<IBuilder>("DefaultBuildEngine").InstancePerLifetimeScope();
            builder.RegisterType<EngineeringLibraryBuilderExecuter>().As<ILibraryBuilderExecuter>().InstancePerLifetimeScope();
            builder.RegisterType<FileBasedSettingsProvider>()
                   .As<ISettingsProvider>()
                   .As<ISettingsObserver>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<JsonCommandResultVisualizer>().As<ICommandResultVisualizer>().InstancePerLifetimeScope();
            builder.RegisterType<RsaSecurityValidator>().As<ISecurityValidator>().InstancePerLifetimeScope();
            builder.RegisterType<SharpZipFileUnpackService>().As<IFileUnpackService>().InstancePerLifetimeScope();
            builder.RegisterType<DirectoryPackService>().As<IDirectoryPackService>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessBasedSettingsMigrationInstallationStep>().As<IInstallationStep>().InstancePerLifetimeScope();
            builder.RegisterType<FileBasedBinariesLocator>().As<IBinariesLocator>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateCommandProvider>().As<IDynamicCommandProvider>().InstancePerLifetimeScope();
            builder.RegisterType<SettingsBasedTemplateLoader>().As<ITemplateLoader>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateCommandBuilder>().As<ITemplateCommandBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateRepository>().As<ITemplateRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateResolver>().As<ITemplateResolver>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateFileGenerator>().Keyed<ITemplateFileGenerator>("DefaultGenerateEngine").InstancePerLifetimeScope();
            builder.RegisterType<CollectiveTemplateIdentifierRepository>().As<ITemplateIdentifierRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SdkInstaller>().As<ISdkInstaller>().InstancePerLifetimeScope();
            builder.RegisterType<PropertiesFileSdkContainer>().As<ISdkContainer>().InstancePerLifetimeScope();
            builder.RegisterType<EntityFactory>().As<IEntityFactory>().InstancePerLifetimeScope();
            builder.Register(c => c.ResolveNamed<IEnumerable<IEntityContentProvider>>("Implementation")).As<IEnumerable<IEntityContentProvider>>().InstancePerLifetimeScope();
            builder.RegisterType<CMakeBuildContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<CodeModelContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<TypeContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<FieldContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<DataTypeContentResolver>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<SymbolContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<PortContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<CodeConstantsContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<CommandDefinitionContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<ConstantContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<FormatTemplateContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<RootEntityContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<SettingsContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<DeployCommandContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<TemplateContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<CppContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<GuidContentProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<ProjectSettingsProvider>()
                   .Named<IEntityContentProvider>("Implementation")
                   .As<ITemplateIdentifier>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<CollectiveEntityContentProvider>().As<IEntityContentProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectConfigurationProvider>().Named<IEntityContentProvider>("Implementation").InstancePerLifetimeScope();
            builder.RegisterType<DeployService>().Keyed<IDeployService>("DefaultDeployEngine").InstancePerLifetimeScope();
            builder.RegisterType<EngineeringLibraryBuilderDeployStep>().As<IDeployStep>().InstancePerLifetimeScope();
            builder.RegisterType<AcfEngineeringLibraryBuilderDeployStep>().As<IDeployStep>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectPropertiesProvider>().As<IProjectPropertiesProvider>().InstancePerLifetimeScope();
            builder.RegisterType<CopyDependenciesDeployStep>().As<IDeployStep>().InstancePerLifetimeScope();
            builder.RegisterType<AppDeployEngine>().Keyed<IDeployService>("AppDeployEngine").InstancePerLifetimeScope();
            builder.RegisterType<NoBuildEngine>().Keyed<IBuilder>("NoBuildEngine")
                   .WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<NoGenerateEngine>().Keyed<ITemplateFileGenerator>("NoGenerateEngine")
                   .WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<SharedNativeGenerateEngine>().Keyed<ITemplateFileGenerator>(nameof(SharedNativeGenerateEngine))
                   .WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<SharedNativeDeployEngine>().Keyed<IDeployService>(nameof(SharedNativeDeployEngine))
                   .WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<SharedNativeLibraryBuilderDeployStep>().As<IDeployStep>().InstancePerLifetimeScope();
            builder.RegisterType<AcfGenerateStep>().As<IGenerateStep>().InstancePerLifetimeScope();
            if (activateAutoComponents)
            {
                AddAutoActivatedComponents(builder, noSdkExploration);
            }
        }

        public static void AddAutoActivatedComponents(ContainerBuilder builder, bool noSdkExploration = false)
        {
            builder.RegisterType<SettingPathCleaner>().AsSelf().AutoActivate().InstancePerLifetimeScope();
            if (!noSdkExploration)
            {
                builder.RegisterType<SdkSettingObserver>().AsSelf().AutoActivate().InstancePerLifetimeScope();
            }

            builder.RegisterType<VerboseChangeObserver>().AsSelf().AutoActivate().InstancePerLifetimeScope()
                   .OnlyIf(registry => registry.IsRegistered(new TypedService(typeof(ExecutionContext))));
        }
    }
}
