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
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PlcNext.Common.Tools.Settings
{
    internal class SettingsParser
    {
        private readonly Dictionary<string, SettingsProperty> properties = new Dictionary<string, SettingsProperty>();

        public SettingsParser(Settings settings)
        {
            ParseSettings(settings);
        }

        private void ParseSettings(Settings settings)
        {
            properties.Add(nameof(Settings.AttributePrefix),
                           new SingleSettingsProperty(nameof(Settings.AttributePrefix), settings.AttributePrefix, SettingDescriptons.AttributePrefix));
            properties.Add(nameof(Settings.SdkPaths),
                           new CollectionSettingsProperty(nameof(Settings.SdkPaths), settings.SdkPaths.ToList(), SettingDescriptons.SdkPaths));
            properties.Add(nameof(Settings.LogFilePath),
                           new SingleSettingsProperty(nameof(Settings.LogFilePath), settings.LogFilePath, SettingDescriptons.LogFilePath));
            properties.Add(nameof(Settings.TemplateLocations),
                           new CollectionSettingsProperty(nameof(Settings.TemplateLocations), settings.TemplateLocations.ToList(), SettingDescriptons.TemplateLocations));
            properties.Add(nameof(Settings.UseSystemCommands),
                           new BoolSettingsProperty(nameof(Settings.UseSystemCommands), settings.UseSystemCommands, SettingDescriptons.UseSystemCommands));
            properties.Add(nameof(Settings.AlwaysWriteExtendedLog),
                           new BoolSettingsProperty(nameof(Settings.AlwaysWriteExtendedLog), settings.AlwaysWriteExtendedLog, SettingDescriptons.AlwaysWriteExtendedLog));
            properties.Add(nameof(Settings.MSBuildPath),
                           new SingleSettingsProperty(nameof(Settings.MSBuildPath), settings.MSBuildPath, SettingDescriptons.MSBuildPath));
        }

        public Settings GetSettings()
        {
            return new Settings(((SingleSettingsProperty) properties[nameof(Settings.AttributePrefix)]).Value,
                                ((CollectionSettingsProperty) properties[nameof(Settings.SdkPaths)]).Values.ToArray(),
                                ((SingleSettingsProperty)properties[nameof(Settings.LogFilePath)]).Value,
                                ((CollectionSettingsProperty)properties[nameof(Settings.TemplateLocations)]).Values.ToArray(),
                                ((BoolSettingsProperty)properties[nameof(Settings.UseSystemCommands)]).Value,
                                ((BoolSettingsProperty)properties[nameof(Settings.AlwaysWriteExtendedLog)]).Value,
                                ((SingleSettingsProperty)properties[nameof(Settings.MSBuildPath)]).Value);
        }

        public string GetSetting(string key)
        {
            SettingsProperty property = GetProperty(key);
            if (property is SingleSettingsProperty single)
            {
                return single.Value;
            }

            CollectionSettingsProperty collection = (CollectionSettingsProperty)property;
            return string.Join(";", collection.Values);
        }
       
        public string GetSettingDescription(string key)
        {
            SettingsProperty property = GetProperty(key);
            return property?.Description;
        }

        private SettingsProperty GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new SettingKeyIsEmptyException(string.Join(", ", properties.Keys));
            }
            if (!properties.ContainsKey(key))
            {
                string closestMatch = key.GetClosestMatch(properties.Keys);
                if (LevenshteinDistance.Compute(closestMatch.ToUpperInvariant(), key.ToUpperInvariant()) < 4)
                {
                    throw new SettingAlmostFoundException(key, closestMatch);
                }
                else
                {
                    throw new SettingNotFoundException(key, string.Join(", ", properties.Keys));
                }
            }

            SettingsProperty property = properties[key];
            return property;
        }

        public void SetSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new SettingValueIsEmptyException();
            }
            SettingsProperty property = GetProperty(key);
            if (property is SingleSettingsProperty single)
            {
                single.Value = value;
            }
            else
            {
                CollectionSettingsProperty collection = (CollectionSettingsProperty) property;
                collection.Values.Clear();
                collection.Values.AddRange(value.Split(new[] {Constants.SettingSplitChar}, StringSplitOptions.RemoveEmptyEntries)
                                                .Distinct());
            }
        }

        public void AddSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new SettingValueIsEmptyException();
            }
            SettingsProperty property = GetProperty(key);
            if (property is CollectionSettingsProperty collection)
            {
                collection.Values.AddRange(value.Split(new[] { Constants.SettingSplitChar }, StringSplitOptions.RemoveEmptyEntries)
                                                .Distinct()
                                                .Where(i => !collection.Values.Contains(i))
                                                .ToArray());
            }
            else
            {
                throw new SettingIsNotCollectionException(key);
            }
        }

        public void RemoveSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new SettingValueIsEmptyException();
            }
            SettingsProperty property = GetProperty(key);
            if (property is CollectionSettingsProperty collection)
            {
                List<SettingValueNotFoundException> valueNotFoundExceptions = new List<SettingValueNotFoundException>();
                foreach (string v in value.Split(new[] { Constants.SettingSplitChar }, StringSplitOptions.RemoveEmptyEntries))
                {
                    bool deleted = collection.Values.Remove(v);
                    if (!deleted)
                    {
                        string closestMatch = v.GetClosestMatch(collection.Values);
                        if (LevenshteinDistance.Compute(closestMatch.ToUpperInvariant(), v.ToUpperInvariant()) < 4)
                        {
                            throw new SettingValueAlmostFoundException(v, closestMatch);
                        }
                        else
                        {
                            throw new SettingValueNotFoundException(v, string.Join(", ", collection.Values));
                        }
                    }
                }

                if (valueNotFoundExceptions.Count == 1)
                {
                    throw valueNotFoundExceptions[0];
                }

                if (valueNotFoundExceptions.Any())
                {
                    throw new AggregateException(valueNotFoundExceptions);
                }
            }
            else
            {
                throw new SettingIsNotCollectionException(key);
            }
        }

        public void ClearSetting(string key)
        {
            SettingsProperty property = GetProperty(key);
            if (property is SingleSettingsProperty single)
            {
                single.Value = string.Empty;
            }
            else
            {
                CollectionSettingsProperty collection = (CollectionSettingsProperty)property;
                collection.Values.Clear();
            }
        }

        private class SettingsProperty
        {
            protected SettingsProperty(string key, string description)
            {
                Key = key;
                Description = description;
            }

            protected string Key { get; }
            public string Description { get; }
        }

        private class SingleSettingsProperty : SettingsProperty
        {
            public SingleSettingsProperty(string key, string value, string description) : base(key, description)
            {
                Value = value;
            }

            public string Value { get; set; }

            public override string ToString()
            {
                return new JObject { { Key, Value } }.ToString();
            }
        }

        private class BoolSettingsProperty : SingleSettingsProperty
        {
            public BoolSettingsProperty(string key, bool value, string description) : base(key, value.ToString(CultureInfo.InvariantCulture), description)
            {
                Value = value;
            }

            public new bool Value
            {
                get
                {
                    if (bool.TryParse(base.Value, out bool result))
                    {
                        return result;
                    }
                    throw new BoolSettingsValueFormatException(Key, base.Value);
                }
                private set => base.Value = value.ToString(CultureInfo.InvariantCulture);
            }

            public override string ToString()
            {
                return new JObject { { Key, Value } }.ToString();
            }
        }

        private class CollectionSettingsProperty : SettingsProperty
        {
            public CollectionSettingsProperty(string key, List<string> values, string description) : base(key, description)
            {
                Values = values;
            }

            public List<string> Values { get; }

            public override string ToString()
            {
                return new JObject { { Key, new JArray(Values.Cast<object>().ToArray()) } }.ToString();
            }
        }

        public bool HasSetting(string key)
        {
            return properties.ContainsKey(key);
        }

        public IEnumerable<string> GetSettingsKeys()
        {
            return properties.Keys;
        }
    }
}
