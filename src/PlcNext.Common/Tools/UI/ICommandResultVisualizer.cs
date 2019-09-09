#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Commands;

namespace PlcNext.Common.Tools.UI
{
    public interface ICommandResultVisualizer
    {
        void Visualize(object result, CommandArgs args);
    }
}