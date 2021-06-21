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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Agents.Net;
using Autofac;
using PlcNext.CommandLine;
using PlcNext.Common;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser;
using PlcNext.Migration;
using Serilog;
using Serilog.Formatting.Compact;

namespace PlcNext
{
    internal class Program
    {
        static int Main(string[] args)
        {
            Task<int> mainTask = MainAsync(args);
            mainTask.ConfigureAwait(false).GetAwaiter().GetResult();
            //Console.ReadKey();
            return mainTask.Result;
        }

        private static async Task<int> MainAsync(string[] args)
        {
            if (args.Any() && 
                args[0].Equals(CommandLineConstants.MigrateCliVerb, StringComparison.OrdinalIgnoreCase) &&
                !args.Any(a => a.TrimEnd().EndsWith("help", StringComparison.OrdinalIgnoreCase)))
            {
                return Migrate() ? 0 : 1;
            }
#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            try
            {
                bool noSdkExploration = args.Any(a => a.Contains("--no-sdk-exploration", StringComparison.Ordinal));
                ILog log = CreateLog();
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterInstance(log);
                builder.RegisterModule(new DiModule(noSdkExploration));

                using (IContainer container = builder.Build())
                {
                    try
                    {
                        ICommandLineParser commandLineParser = container.Resolve<ICommandLineParser>();
                        
                        ConfigureSerilog(log, container.Resolve<ISettingsProvider>());
                        
                        IMessageBoard messageBoard = container.Resolve<IMessageBoard>();
                        Agent[] agents = container.Resolve<IEnumerable<Agent>>().ToArray();
                        messageBoard.Register(agents);
                        messageBoard.Start();
                        
#if DEBUG
                        Console.WriteLine($@"Startup timer {stopwatch.Elapsed}");
                        Console.WriteLine($@"Arguments: {args.Aggregate(string.Empty, (s, s1) => s + "_" + s1)}");
#endif
                        int result =  await commandLineParser.Parse(args).ConfigureAwait(false);
                        return result;
                    }
                    catch (Exception e)
                    {
                        IExceptionHandler exceptionHandler = container.Resolve<IExceptionHandler>();
                        if (!exceptionHandler.HandleException(e))
                        {
                            throw;
                        }

                        return -1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Unexpected exception during execution{Environment.NewLine}{e}");
                Trace.TraceError(e.ToString());
                return -1;
            }

            ILog CreateLog()
            {
                string path = LogHelper.GetLogCatalogLocation();
                ILog result = LogCatalog.CreateNewLog(path, string.Join(" ", args));
                result.AddInitialLog(args);
                return result;
            }

            void ConfigureSerilog(ILog log, ISettingsProvider settingsProvider)
            {
                if (args.All(a => a.Trim() != "--verbose") &&
                    !settingsProvider.Settings.AlwaysWriteExtendedLog)
                {
                    return;
                }
                string logFile = Path.GetTempFileName();
                log.LogVerbose($"Logging agent framework in {logFile}");
                Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.Async(l => l.File(new CompactJsonFormatter(), logFile))
                            .CreateLogger();
            }

            bool Migrate()
            {
                ILog log = LogHelper.GetMigrationLog();
                try
                {
                    log.AddInitialLog(args);
                    //Not implemented feature: Old version has caches and settings in same location as current version.
                    //How to identify the version? Probably create a .version file.
                    return MigrationChain.Start(m =>
                                          {
                                              Console.WriteLine(m);
                                              log.LogInformation(m);
                                          })
                                         .AddPotentialLocation(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                            "plcncli.Common"),new Version(19,0))
                                         .AddMigrationFile("settings.xml")
                                         .AddMigrationFile("sdk-properties.xml")
                                         .SetMigrationDestination(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                               (Assembly.GetEntryAssembly()??Assembly.GetExecutingAssembly()).GetName().Name))
                                         .AddConversionStep<ConversionFrom190>()
                                         .Execute();
                }
                finally
                {
                    (log as IDisposable)?.Dispose();
                }
            }
        }
    }
}
