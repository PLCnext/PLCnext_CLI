#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Commands
{
    internal class GetProjectInformationCommand : SyncCommand<GetProjectInformationCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IProjectPropertiesProvider propertiesProvider;

        public GetProjectInformationCommand(ITransactionFactory transactionFactory,
                                          IExceptionHandler exceptionHandler,
                                          ExecutionContext executionContext, 
                                          ICommandResultVisualizer commandResultVisualizer, 
                                          IEntityFactory entityFactory, IProjectPropertiesProvider propertiesProvider)
            : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.entityFactory = entityFactory;
            this.propertiesProvider = propertiesProvider;
        }

        protected override CommandResult ExecuteDetailed(GetProjectInformationCommandArgs args, ChangeObservable observable)
        {
            Entity root = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;

            propertiesProvider.Initialize(root);

            return new CommandResult(0, 
                new ProjectInformationCommandResult(propertiesProvider.ProjectName, 
                                                    propertiesProvider.ProjectNamespace,
                                                    propertiesProvider.ProjectType,
                                                    propertiesProvider.ProjectTargets.Select(t => new TargetResult(t.Target.Name, 
                                                                                                                   t.Target.Version,
                                                                                                                   t.Target.LongVersion,
                                                                                                                   t.Target.ShortVersion,
                                                                                                                   t.IsAvailable)),
                                                    propertiesProvider.ProjectCodeEntities.Select(kvp => new EntityResult(kvp.Key.Name,
                                                                                                                          kvp.Key.Namespace,
                                                                                                                          kvp.Key.Type,
                                                                                                                          kvp.Value.Select(en => en.Name))),
                                                    propertiesProvider.IncludePaths.Select(p => new CommandResults.IncludePath(p.PathValue, 
                                                                                                                               (p.Exists!= null ? (bool)p.Exists : false), 
                                                                                                                               p.Targets
                                                                                                                                .Select(t => new TargetResult(t.Name, 
                                                                                                                                                              t.Version, 
                                                                                                                                                              t.LongVersion, 
                                                                                                                                                              t.ShortVersion)
                                                                                                                                        )
                                                                                                                                )
                                                                                           ),
                                                    propertiesProvider.ExternalLibraries.Select(p => new CommandResults.Path(p)),
                                                    propertiesProvider.GenerateNamespaces
                                                    ),
                propertiesProvider.Exceptions);
        }
    }
}
