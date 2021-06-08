#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using Autofac;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser
{
    public class AgentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.CppFileAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.SourceDirectoryAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.CppFileFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.FileOpener>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ExceptionCancelExecuter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.StreamParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.TypeDeclarationFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.TypeSpecifier>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.NameParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.UsingsParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.IncludesParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.CommentParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.DeclarationFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.BaseTypeParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.DeclarationFilter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.IdentifierParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.FieldMultiplicityParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.FieldNameParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.TypeFieldAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.ClassConstructor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.StructureConstructor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.FileTypesAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.CodeModelConstructor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.FieldSetConstructor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ParserErrorHandler>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.FieldSetCommentParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.EnumConstructor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.SymbolParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.CppFileResultCollector>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheFileOpener>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheFileCreator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.ParserMessageAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.TypeFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheFileChecker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheCheckAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheFileSaver>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheCheckUpdater>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheCheckStarter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheEntryValidator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheFileLocator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CodeModelCacheUpdater>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheCheckInterceptor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeDefinitionsProcessor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeFileLocator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeFileParsingDecorator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeFileLocationChecker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.NoTypeFileResultBuilder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ServiceBridge>().As<Agent>().As<IParser>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheEntryAdder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CodeModelIncludesUpdater>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CacheVersionChecker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.ExistingCacheEntryChecker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<CppRipper.Agents.DefineStatementParser>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.CodeModelCreationStarter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.CodeModelIncludesRegister>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeCacheFileResultCollector>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeFileResultEvaluater>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.AddedEntryParsingErrorCollector>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<IncludeManager.Agents.IncludeDefineStatementsRegister>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
