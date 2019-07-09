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
    public interface IEditableSettings : IDisposable
    {
        void SetSetting(string key, string value);

        void AddSetting(string key, string value);

        void RemoveSetting(string key, string value);

        void ClearSetting(string key);
    }
}
