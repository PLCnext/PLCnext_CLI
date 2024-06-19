#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            return LogCatalogPathDefault.CleanPath().ResolvePathName(new EnvironmentPathNames());
        }

        public static ILog GetMigrationLog()
        {
            string migrationFile = Path.Combine(MigrationLogPathDefault.CleanPath(),
                                                $"migration-{DateTime.Now:dd-MM-yyyy_hh-mm-ss-fff}.txt")
                                       .ResolvePathName(new EnvironmentPathNames());
            return FileLog.Create(migrationFile);
        }

        public static void AddInitialLog(this ILog log, string[] args)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

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

            //replace password with * in logged arguments
            int index = -1;
            string[] argsWithoutPw = new string[args.Length];
            Array.Copy(args, argsWithoutPw, args.Length);
            
            if (args.Contains("--password"))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--password" 
                        && args.Length > i +1 
                        && !args[i+1].StartsWith('-'))
                    {
                        index = i+1; 
                        break;
                    }
                }
                if (index > -1)
                {
                    argsWithoutPw[index] = "*";
                }
            }

            log.LogInformation($"Arguments: {string.Join(" ", argsWithoutPw)}");
            log.LogInformation($"Execution context: {Directory.GetCurrentDirectory()}");
            log.LogInformation($"Process id: {Environment.ProcessId}");
        }
    }
}