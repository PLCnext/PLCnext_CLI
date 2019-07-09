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
    public class SetSettingsCommandArgs : CommandArgs
    {
        public SetSettingsCommandArgs(string key, string value, bool add, bool remove, bool clear)
        {
            Key = key;
            Value = value;
            Add = add;
            Remove = remove;
            Clear = clear;
        }

        public string Key { get; }
        
        public string Value { get; }
        
        public bool Add { get; }
        
        public bool Remove { get; }
        
        public bool Clear { get; }
    }
}
