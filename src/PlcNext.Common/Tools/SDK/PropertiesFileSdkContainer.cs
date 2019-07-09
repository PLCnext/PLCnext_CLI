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
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Tools.SDK
{
    internal class PropertiesFileSdkContainer : ISdkContainer
    {
        private const string SdkPropertiesPath = "{ApplicationData}/{ApplicationName}/sdk-properties.xml";
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;
        private readonly ExecutionContext executionContext;
        private VirtualFile sdkPropertiesFile;
        private Properties sdkProperties;
        private readonly HashSet<string> exploredPaths = new HashSet<string>();

        public PropertiesFileSdkContainer(IFileSystem fileSystem, IEnvironmentService environmentService, ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.executionContext = executionContext;
        }

        private VirtualFile SdkPropertiesFile
        {
            get
            {
                if(sdkPropertiesFile == null)
                {
                    string path = ResolvePathNames(SdkPropertiesPath.CleanPath());
                    Uri pathUri = new Uri(path, UriKind.RelativeOrAbsolute);
                    if (!pathUri.IsAbsoluteUri)
                    {
                        path = Path.Combine(environmentService.AssemblyDirectory, path);
                    }
                    sdkPropertiesFile = fileSystem.GetFile(path);

                    string ResolvePathNames(string unresolved)
                    {
                        string resolved = unresolved;
                        Regex resolvePattern = new Regex(@"{(?<resolvable>\w+)}");
                        Match resolveMatch = resolvePattern.Match(unresolved);
                        while (resolveMatch.Success)
                        {
                            string key = resolveMatch.Groups["resolvable"].Value;
                            if (environmentService.PathNames.ContainsKey(key))
                            {
                                resolved = resolved.Replace(resolveMatch.Value, environmentService.PathNames[key]);
                            }

                            resolveMatch = resolvePattern.Match(resolved);
                        }

                        return resolved;
                    }
                }
                return sdkPropertiesFile;
            }
        }

        private Properties SdkProperties
        {
            get
            {
                EnsurePropertiesLoaded();
                return sdkProperties;
            }
        }

        private void EnsurePropertiesLoaded()
        {
            if (sdkProperties == null)
            {
                sdkProperties = LoadProperties();
            }
            
            Properties LoadProperties()
            {
                if (!fileSystem.FileExists(SdkPropertiesFile.FullName))
                {
                    executionContext.WriteVerbose($"No sdk properties found in {SdkPropertiesFile.FullName}. Emtpy properties will be used.");
                    return new Properties();
                }

                using (Stream fileStream = SdkPropertiesFile.OpenRead())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Properties));
                    Properties result = (Properties) serializer.Deserialize(fileStream);
                    return NeedsConversion(result) ? new Properties() : result;
                }
                
                bool NeedsConversion(Properties result)
                {
                    //Beta -> release
                    if (result.SDK != null && result.SDK.Length > 0 &&
                        result.SDK[0].CompilerSpec == null)
                    {
                        return true;
                    }

                    //Up-to-date
                    return false;
                }
            }
        }

        private void SaveProperties()
        {
            if (!SdkPropertiesFile.CheckAccess())
            {
                executionContext.WriteWarning("The properties of the configured SDKs cannot be saved and " +
                                           "will be explored every time the CLI is executed. " +
                                           "Please call the command 'scan sdks' with administator rights to " +
                                           "save the SDKs properties.");
            }

            using (Stream fileStream = SdkPropertiesFile.OpenWrite())
            {
                fileStream.SetLength(0);
                XmlSerializer serializer = new XmlSerializer(typeof(Properties));
                serializer.Serialize(fileStream, SdkProperties);
                executionContext.WriteVerbose("Saved SDK properties file.");
            }
        }

        public bool Contains(string path)
        {
            EnsurePropertiesLoaded();
            return exploredPaths.Contains(path);
        }

        public void Remove(string sdkRootPath)
        {
            sdkRootPath = sdkRootPath.CleanPath();

            if (SdkProperties.SDK != null)
            {
                SdkProperties.SDK = SdkProperties.SDK.Where(s => s.path != sdkRootPath)
                                                 .ToArray();
                SaveProperties();
            }
        }

        public void Add(string sdkRootPath, SdkSchema sdkSchema)
        {
            SdkProperties.SDK = SdkProperties.SDK != null
                                    ? SdkProperties.SDK.Concat(new[] {sdkSchema}).ToArray()
                                    : new[] {sdkSchema};
            SaveProperties();
            exploredPaths.Add(sdkRootPath);
        }

        public Sdk Get(string sdkRootPath)
        {
            sdkRootPath = sdkRootPath.CleanPath();

            return ParseSdk();

            Sdk ParseSdk()
            {
                SdkSchema definition = SdkProperties.SDK?.FirstOrDefault(s => s.path == sdkRootPath);
                if (definition == null)
                {
                    throw new InvalidOperationException($"This should not happen. Expected properties for SDK {sdkRootPath}, but found none.");
                }

                IEnumerable<Target> targets = definition.Target?.Select(t => new Target(t.name, t.version))
                                              ?? Enumerable.Empty<Target>();
                if (!fileSystem.DirectoryExists(definition.path))
                {
                    throw new SdkPathNotExistingException(definition.path);
                }
                return new Sdk(targets, definition.IncludePath ?? Enumerable.Empty<string>(),
                               fileSystem.GetDirectory(definition.path),
                               string.IsNullOrEmpty(definition.makepath)
                                   ? null
                                   : fileSystem.GetFile(definition.makepath),
                               new Compiler(definition.CompilerSpec.compilerPath,
                                            definition.CompilerSpec.compilerSysroot,
                                            definition.CompilerSpec.compilerFlags,
                                            definition.CompilerSpec.IncludePath,
                                            definition.CompilerSpec.CompilerMacro
                                                      .Select(m => new CompilerMakro(m.name, m.value))));
            }
        }
    }
}