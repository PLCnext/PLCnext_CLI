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

namespace PlcNext.Common.Commands
{
    public class GetSettingsCommandArgs : CommandArgs
    {
        public GetSettingsCommandArgs(string key, bool all)
        {
            Key = key;
            All = all;
        }

        public string Key { get; }
        public bool All { get; }
    }
}
