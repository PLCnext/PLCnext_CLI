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
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Tools.DynamicCommands
{
    public interface IDynamicCommandProvider
    {
        IEnumerable<CommandDefinition> GetCommands(IEnumerable<string> context);
        bool CanExecute(CommandDefinition definition);
        Task<int> Execute(CommandDefinition definition, ChangeObservable observable);
    }
}
