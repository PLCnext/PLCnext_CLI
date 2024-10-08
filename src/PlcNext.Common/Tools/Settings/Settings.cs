﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlcNext.Common.Tools.Settings
{
    public class Settings
    {
        public const string LogFilePathDefault = "{ApplicationData}/{ApplicationName}/log.txt";
        public const string AttributePrefixDefault = "#";
        public static readonly string[] TemplateLocationsDefault = new[] { "./Templates/Templates.xml" };
        public static readonly string[] SdkPathsDefault = System.Array.Empty<string>();
        public const bool UseSystemCommandsDefault = false;
        public const bool AlwaysWriteExtendedLogDefault = false;
        public const string MSBuildPathDefault = "";

        public Settings(string attributePrefix, string[] sdkPaths, string logFilePath, string[] templateLocations,
                        bool useSystemCommands, bool alwaysWriteExtendedLog, string msBuildPath)
        {
            AttributePrefix = attributePrefix;
            SdkPaths = sdkPaths;
            LogFilePath = logFilePath;
            TemplateLocations = templateLocations;
            UseSystemCommands = useSystemCommands;
            AlwaysWriteExtendedLog = alwaysWriteExtendedLog;
            MSBuildPath = msBuildPath;
        }

        public Settings()
        {
            AttributePrefix = AttributePrefixDefault;
            SdkPaths = SdkPathsDefault;
            LogFilePath = LogFilePathDefault;
            TemplateLocations = TemplateLocationsDefault;
            UseSystemCommands = UseSystemCommandsDefault;
            AlwaysWriteExtendedLog = AlwaysWriteExtendedLogDefault;
            MSBuildPath = MSBuildPathDefault;
        }

        public IReadOnlyCollection<string> SdkPaths { get; }

        public IReadOnlyCollection<string> TemplateLocations { get; }

        public string AttributePrefix { get; }

        public string LogFilePath { get; }

        public bool UseSystemCommands { get; }
        
        public bool AlwaysWriteExtendedLog { get; }

        public string MSBuildPath { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented,
                                               new JsonSerializerSettings
                                               {
                                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                               });
        }
    }
}
