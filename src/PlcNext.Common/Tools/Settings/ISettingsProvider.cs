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

namespace PlcNext.Common.Tools.Settings
{
    public interface ISettingsProvider
    {
        Settings Settings { get; }

        string GetSetting(string key);

        IEditableSettings StartSettingTransaction();

        bool HasSetting(string key);

        string GetSettingValue(string setting);

        IEnumerable<string> GetSettingKeys();
        
        string GetSettingDescription(string key);
    }
}
