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
    public interface ISettingsObserver
    {
        event EventHandler<SettingsObserverEventArgs> SettingSetting;
        event EventHandler<SettingsObserverEventArgs> SettingSet;
        event EventHandler<SettingsObserverEventArgs> SettingAdding;
        event EventHandler<SettingsObserverEventArgs> SettingAdded;
        event EventHandler<SettingsObserverEventArgs> SettingRemoving;
        event EventHandler<SettingsObserverEventArgs> SettingRemoved;
        event EventHandler<SettingsObserverEventArgs> SettingCleared;
        event EventHandler<EventArgs> SettingsLoaded;
    }
}
