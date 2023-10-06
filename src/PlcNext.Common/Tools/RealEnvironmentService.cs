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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PlcNext.Common.Tools
{
    internal class RealEnvironmentService : IEnvironmentService
    {
        public Version AssemblyVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version;
            }
        }

        public string Architecture => RuntimeInformation.ProcessArchitecture.ToString();

        public string PlatformName
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "linux";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "osx";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "windows";
                }

                return "unbound";
            }
        }

        public OSPlatform Platform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return OSPlatform.Linux;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return OSPlatform.OSX;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return OSPlatform.Windows;
                }

                return new OSPlatform();
            }
        }

        public string AssemblyDirectory
        {
            get
            {
#if !DEBUG
                //Workaround for https://github.com/dotnet/core-setup/issues/7491
                ProcessModule mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                if (mainModule != null)
                {
                    return Path.GetDirectoryName(mainModule.FileName);
                }
#endif

#pragma warning disable IL3000 //This code should never be reached in release build
                string location = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(location);
#pragma warning restore IL3000
            }
        }

        public IEnvironmentPathNames PathNames { get; } = new EnvironmentPathNames();
    }

    public class EnvironmentPathNames : IEnvironmentPathNames
    {
        public string this[string key] => Enum.TryParse(key, true, out Environment.SpecialFolder folder)
                                              ? Environment.GetFolderPath(folder, Environment.SpecialFolderOption.DoNotVerify)
                                              : Assembly.GetEntryAssembly().GetName().Name;

        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return Enum.TryParse(key, true, out Environment.SpecialFolder _) ||
                   key.Equals("ApplicationName", StringComparison.OrdinalIgnoreCase);
        }
    }
}
