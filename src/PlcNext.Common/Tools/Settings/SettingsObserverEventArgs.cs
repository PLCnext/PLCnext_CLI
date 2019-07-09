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

namespace PlcNext.Common.Tools.Settings
{
    public class SettingsObserverEventArgs : EventArgs
    {
        public SettingsObserverEventArgs(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; set; }
        public string[] SplitValue
        {
            get => Value?.Split(new[] { Constants.SettingSplitChar }, StringSplitOptions.RemoveEmptyEntries)??new string[0];
            set => Value = string.Join(new string(Constants.SettingSplitChar, 1), value);
        }
    }
}
