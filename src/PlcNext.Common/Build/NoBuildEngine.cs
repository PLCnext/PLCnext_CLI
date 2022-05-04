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
using Autofac.Features.AttributeFilters;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Build;

internal class NoBuildEngine : IBuilder
{
    private readonly IBuilder baseBuilder;

    public NoBuildEngine([KeyFilter("DefaultBuildEngine")] IBuilder baseBuilder)
    {
        this.baseBuilder = baseBuilder;
    }

    public void Build(BuildInformation buildInfo, ChangeObservable observable, IEnumerable<string> targets)
    {
        TemplateEntity template = TemplateEntity.Decorate(buildInfo.RootProjectEntity);
        throw new FormattableException($"Build is not available for the project type '{template.Template.name}'.");
    }

    public int BuildLibraryForProject(Entity project, ChangeObservable observable, string metaFilesDirectory,
                                      string libraryLocation, string outputDirectory, Guid libraryGuid, IEnumerable<string> targets,
                                      IEnumerable<string> externalLibraries, string buildType)
    {
        return baseBuilder.BuildLibraryForProject(project, observable, metaFilesDirectory, libraryLocation, outputDirectory, libraryGuid, targets, externalLibraries, buildType);
    }
}