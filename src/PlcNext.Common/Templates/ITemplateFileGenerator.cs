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
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Templates
{
    public interface ITemplateFileGenerator
    {
        Task<IEnumerable<VirtualFile>> InitalizeTemplate(Entity dataModel, ChangeObservable observable);
        Task GenerateFiles(Entity rootEntity, string generator, string output, bool outputDefined,
                           ChangeObservable observable);
    }
}
