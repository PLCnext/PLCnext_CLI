#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Generate
{
    interface IGenerateStep
    {
        string Identifier { get; }

        void Execute(Entity dataModel, ChangeObservable observable);
    }
}
