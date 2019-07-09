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
using Autofac;
using PlcNext.Common;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser;

namespace PlcNext
{
    internal class Program
    {
        static int Main(string[] args)
        {
            Task<int> mainTask = MainAsync(args);
            mainTask.Wait();
            //Console.ReadKey();
            return mainTask.Result;
        }

        private static async Task<int> MainAsync(string[] args)
        {
#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            try
            {
                bool noSdkExploration = args.Any(a => a.Contains("--no-sdk-exploration"));
                ILog log = CreateLog();
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterInstance(log);
                builder.RegisterModule(new DiModule(noSdkExploration));

                using (IContainer container = builder.Build())
                {
                    try
                    {
                        ICommandLineParser commandLineParser = container.Resolve<ICommandLineParser>();
#if DEBUG
                        Console.WriteLine($@"Startup timer {stopwatch.Elapsed}");
                        Console.WriteLine($@"Arguments: {args.Aggregate(string.Empty, (s, s1) => s + "_" + s1)}");
#endif
                        int result =  await commandLineParser.Parse(args);
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
        }
    }
}
