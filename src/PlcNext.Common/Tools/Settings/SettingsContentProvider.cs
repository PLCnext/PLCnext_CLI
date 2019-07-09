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
using System.Linq;
using System.Reflection;
using System.Text;
using PlcNext.Common.DataModel;

namespace PlcNext.Common.Tools.Settings
{
    internal class SettingsContentProvider : IEntityContentProvider
    {
        private readonly ISettingsProvider settingsProvider;

        public SettingsContentProvider(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.SettingsKey || owner.Value<Settings>() != null;
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            if (key == EntityKeys.SettingsKey)
            {
                return owner.Create(key, settingsProvider.Settings);
            }

            Settings settings = owner.Value<Settings>();
            Entity result = owner.PropertyValueEntity(key, settings);
            if (result != null)
            {
                return result;
            }
            throw new ContentProviderException(key, owner);
        }
    }
}
