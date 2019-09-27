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
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public IEnvironmentPathNames PathNames { get; } = new EnvironmentPathNames();
    }

    public class EnvironmentPathNames : IEnvironmentPathNames
    {
        public string this[string key] => Enum.TryParse(key, true, out Environment.SpecialFolder folder)
                                              ? Environment.GetFolderPath(folder)
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
