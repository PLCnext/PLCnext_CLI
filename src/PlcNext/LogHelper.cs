#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext
{
    public static class LogHelper
    {
        private const string LogCatalogPathDefault = "{ApplicationData}/{ApplicationName}/logs/cli/catalog.json";
        private const string MigrationLogPathDefault = "{ApplicationData}/{ApplicationName}/logs";
        
        public static string GetLogCatalogLocation()
        {
            return ResolvePathNames(LogCatalogPathDefault.CleanPath());
        }

        private static string ResolvePathNames(string unresolved)
        {
            IEnvironmentPathNames pathNames = new EnvironmentPathNames();
            string resolved = unresolved;
            Regex resolvePattern = new Regex(@"{(?<resolvable>\w+)}");
            Match resolveMatch = resolvePattern.Match(unresolved);
            while (resolveMatch.Success)
            {
                string key = resolveMatch.Groups["resolvable"].Value;
                if (pathNames.ContainsKey(key))
                {
                    resolved = resolved.Replace(resolveMatch.Value, pathNames[key]);
                }

                resolveMatch = resolvePattern.Match(resolved);
            }

            return resolved;
        }

        public static ILog GetMigrationLog()
        {
            string migrationFile = ResolvePathNames(Path.Combine(MigrationLogPathDefault.CleanPath(),
                                                                 $"migration-{DateTime.Now:dd-MM-yyyy_hh-mm-ss-fff}.txt"));
            return FileLog.Create(migrationFile);
        }

        public static void AddInitialLog(this ILog log, string[] args)
        {
            try
            {
                Assembly executingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                string name = executingAssembly.GetName().Name;
                string version = executingAssembly.GetName().Version.ToString();
                AssemblyTitleAttribute title = executingAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
                if (title != null)
                {
                    name = title.Title;
                }

                AssemblyInformationalVersionAttribute informationalVersion =
                    executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (informationalVersion != null)
                {
                    version = $"{informationalVersion.InformationalVersion} ({version})";
                }
                log.LogInformation($"{name}: {version}");
            }
            catch (Exception e)
            {
                log.LogError($"Could not create application infos:{Environment.NewLine}{e}");
            }
            log.LogInformation($"Arguments: {string.Join(" ", args)}");
            log.LogInformation($"Execution context: {Directory.GetCurrentDirectory()}");
            log.LogInformation($"Process id: {Process.GetCurrentProcess().Id}");
        }
    }
}