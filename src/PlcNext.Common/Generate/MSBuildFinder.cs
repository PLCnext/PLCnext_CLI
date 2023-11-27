#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools;
using System.Runtime.InteropServices;
using PlcNext.Common.Tools.FileSystem;
using System.IO;
using System;

namespace PlcNext.Common.Generate
{
    internal class MSBuildFinder
    {
        internal static MSBuild FindMsBuild(IEnvironmentService environmentService, IFileSystem fileSystem,
                               ISettingsProvider settingsProvider, IBinariesLocator binariesLocator, Entity entity)
        {
            string msbuild = Constants.MSBuildName;
            string dotnet = Constants.DotNetName;
            if (environmentService.Platform == OSPlatform.Windows)
            {
                msbuild += ".exe";
                dotnet += ".exe";
            }
            MSBuild msbuildLocation = GetMSBuildOptionValue();
            if (string.IsNullOrEmpty(msbuildLocation.FullName))
            {
                msbuildLocation = GetMSBuildSetting();
            }
            if (string.IsNullOrEmpty(msbuildLocation.FullName))
            {
                msbuildLocation = GetMSBuildFromPath();
            }
            if (string.IsNullOrEmpty(msbuildLocation.FullName))
            {
                msbuildLocation = GetDotNetFromPath();
            }
            if (string.IsNullOrEmpty(msbuildLocation.FullName))
            {
                throw new FormattableException("MSBuild was not found. Please use the option --msbuild or set the settings key 'MSBuildPath' or add it to your PATH environment.");
            }

            return msbuildLocation;

            MSBuild GetMSBuildOptionValue()
            {
                CommandEntity command = CommandEntity.Decorate(entity.Origin);
                string location = command.GetSingleValueArgument("msbuild");
                return FindMsBuildExecutable(location);
            }

            MSBuild GetMSBuildSetting()
            {
                string location = settingsProvider.Settings.MSBuildPath;
                return FindMsBuildExecutable(location);
            }

            MSBuild GetMSBuildFromPath()
            {
                string location = binariesLocator.GetExecutableCommand(msbuild);
                return new MSBuild(location);
            }

            MSBuild GetDotNetFromPath()
            {
                string location = binariesLocator.GetExecutableCommand(dotnet);
                return new MSBuild(location) { Parameters = "msbuild" };
            }

            MSBuild FindMsBuildExecutable(string location)
            {
                if (!string.IsNullOrEmpty(location))
                {
                    if (fileSystem.FileExists(location))
                    {
                        if(fileSystem.GetFile(location).Name.StartsWith("dotnet", StringComparison.OrdinalIgnoreCase)) 
                        {
                            return new MSBuild(location) { Parameters = "msbuild"};
                        }
                        return new MSBuild(location);
                    }
                    if (fileSystem.FileExists(msbuild, location))
                    {
                        return new MSBuild(Path.Combine(location, msbuild));
                    }
                    if (fileSystem.FileExists(dotnet, location))
                    {
                        return new MSBuild(Path.Combine(location, dotnet)) { Parameters = "msbuild" };
                    }
                }
                return new MSBuild(location);
            }
        }
    }
    internal class MSBuild
    {
        public MSBuild(string path)
        {
            FullName = path;
        }

        public string FullName { get; set; }
        public string Parameters { get; set; } = string.Empty;
    }
}