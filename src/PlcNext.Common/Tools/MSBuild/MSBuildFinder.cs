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
using System.Runtime.InteropServices;
using PlcNext.Common.Tools.FileSystem;
using System.IO;
using System;

namespace PlcNext.Common.Tools.MSBuild
{
    internal class MSBuildFinder : IMSBuildFinder
    {
        private readonly IEnvironmentService environmentService;
        private readonly IFileSystem fileSystem;
        private readonly ISettingsProvider settingsProvider;
        private readonly IBinariesLocator binariesLocator;


        public MSBuildFinder(IEnvironmentService environmentService, IFileSystem fileSystem,
                               ISettingsProvider settingsProvider, IBinariesLocator binariesLocator)
        {
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;
            this.settingsProvider = settingsProvider;
            this.binariesLocator = binariesLocator;
        }
        public MSBuildData FindMSBuild(Entity rootEntity)
        {
            string msbuild = Constants.MSBuildName;
            string dotnet = Constants.DotNetName;
            if (environmentService.Platform == OSPlatform.Windows)
            {
                msbuild += ".exe";
                dotnet += ".exe";
            }
            MSBuildData msbuildLocation = GetMSBuildOptionValue();
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

            MSBuildData GetMSBuildOptionValue()
            {
                CommandEntity command = CommandEntity.Decorate(rootEntity.Origin);
                string location = command.GetSingleValueArgument("msbuild");
                return FindMsBuildExecutable(location);
            }

            MSBuildData GetMSBuildSetting()
            {
                string location = settingsProvider.Settings.MSBuildPath;
                return FindMsBuildExecutable(location);
            }

            MSBuildData GetMSBuildFromPath()
            {
                string location = binariesLocator.GetExecutableCommand(msbuild);
                return new MSBuildData(location);
            }

            MSBuildData GetDotNetFromPath()
            {
                string location = binariesLocator.GetExecutableCommand(dotnet);
                return new MSBuildData(location) { Parameters = "msbuild" };
            }

            MSBuildData FindMsBuildExecutable(string location)
            {
                if (!string.IsNullOrEmpty(location))
                {
                    if (fileSystem.FileExists(location))
                    {
                        if (fileSystem.GetFile(location).Name.StartsWith("dotnet", StringComparison.OrdinalIgnoreCase))
                        {
                            return new MSBuildData(location) { Parameters = "msbuild" };
                        }
                        return new MSBuildData(location);
                    }
                    if (fileSystem.FileExists(msbuild, location))
                    {
                        return new MSBuildData(Path.Combine(location, msbuild));
                    }
                    if (fileSystem.FileExists(dotnet, location))
                    {
                        return new MSBuildData(Path.Combine(location, dotnet)) { Parameters = "msbuild" };
                    }
                }
                return new MSBuildData(location);
            }
        }
    }
}