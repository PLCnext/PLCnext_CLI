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
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Generate;

public class NoGenerateEngine : ITemplateFileGenerator
{
    private readonly ITemplateFileGenerator templateFileGenerator;

    public NoGenerateEngine([KeyFilter("DefaultGenerateEngine")] ITemplateFileGenerator templateFileGenerator)
    {
        this.templateFileGenerator = templateFileGenerator;
    }

    public Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable)
    {
        return templateFileGenerator.InitalizeTemplate(dataModel, observable);
    }

    public Task GenerateFiles(Entity rootEntity, string generator, string output, bool outputDefined, ChangeObservable observable)
    {
        TemplateEntity template = TemplateEntity.Decorate(rootEntity);
        throw new FormattableException($"Generate is not available for the project type '{template.Template.name}'.");
    }
}