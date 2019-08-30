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
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Deploy
{
    internal class EngineeringLibraryBuilderDeployStep : IDeployStep
    {
        private readonly IBuilder builder;

        public EngineeringLibraryBuilderDeployStep(IBuilder builder)
        {
            this.builder = builder;
        }

        public string Identifier => "LibraryBuilderDeployStep";

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            CommandEntity command = CommandEntity.Decorate(dataModel);

            string metaFilesDirectory = command.GetSingleValueArgument(Constants.MetaPathArgumentName);

            string libraryLocation = command.GetSingleValueArgument(Constants.LibraryLocationArgumentName);

            string outputDirectory = command.GetSingleValueArgument(Constants.OutputArgumentName);

            if (command.IsCommandArgumentSpecified(Constants.IdArgumentName) 
                && !Guid.TryParse(command.GetSingleValueArgument(Constants.IdArgumentName), out Guid realGuid))
            {
                throw new LibraryIdMalformattedException(command.GetSingleValueArgument(Constants.IdArgumentName));
            }

            IEnumerable<string> targets = command.GetMultiValueArgument(Constants.TargetArgumentName);

            IEnumerable<string> externalLibraries = command.GetMultiValueArgument(Constants.ExternalLibrariesArgumentName);

            builder.BuildLibraryForProject(dataModel.Root, observable, metaFilesDirectory, libraryLocation, outputDirectory, realGuid, targets, externalLibraries);            
        }
    }
}
