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
using Autofac.Features.Indexed;
using PlcNext.Common.Build;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class GenerateLibraryCommand : SyncCommand<GenerateLibraryCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IIndex<string, IBuilder> builders;

        public GenerateLibraryCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, IIndex<string, IBuilder> builders) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.builders = builders;
        }

        protected override int Execute(GenerateLibraryCommandArgs args, ChangeObservable observable)
        {
            if (!string.IsNullOrEmpty(args.LibraryGuid) && !Guid.TryParse(args.LibraryGuid, out Guid realGuid))
            {
                throw new LibraryIdMalformattedException(args.LibraryGuid);
            }

            Entity project = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            TemplateEntity templateEntity = TemplateEntity.Decorate(project);
            return builders[templateEntity.Template.buildEngine].BuildLibraryForProject(project, observable, args.MetaFilesDirectory, args.LibraryLocation,
                                           args.OutputDirectory, realGuid, args.Target, args.ExternalLibraries, null);
        }
    }
}
