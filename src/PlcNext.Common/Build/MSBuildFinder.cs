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
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.Tools.FileSystem;
using System.IO;

namespace PlcNext.Common.Build
{
    internal class MSBuildFinder
    {
        internal static string FindMsBuild(IEnvironmentService environmentService, IFileSystem fileSystem,
                               ISettingsProvider settingsProvider, IBinariesLocator binariesLocator, Entity entity)
        {
            string msbuild = Constants.MSBuildName;
            if (environmentService.Platform == OSPlatform.Windows)
            {
                msbuild += ".exe";
            }
            string msbuildLocation = GetMSBuildOptionValue();
            if (string.IsNullOrEmpty(msbuildLocation))
            {
                msbuildLocation = GetMSBuildSetting();
            }
            if (string.IsNullOrEmpty(msbuildLocation))
            {
                msbuildLocation = GetMSBuildFromPath();
            }
            if (string.IsNullOrEmpty(msbuildLocation))
            {
                throw new FormattableException("MSBuild was not found. Please use the option --msbuild or set the settings key 'MSBuildPath' or add it to your PATH environment.");
            }

            return msbuildLocation;

            string GetMSBuildOptionValue()
            {
                CommandEntity command = CommandEntity.Decorate(entity.Origin);
                string location = command.GetSingleValueArgument("msbuild");
                return FindMsBuildExecutable(location);
            }

            string GetMSBuildSetting()
            {
                string location = settingsProvider.Settings.MSBuildPath;
                return FindMsBuildExecutable(location);
            }

            string GetMSBuildFromPath()
            {
                string location = binariesLocator.GetExecutableCommand(msbuild);
                return location;
            }

            string FindMsBuildExecutable(string location)
            {
                if (!string.IsNullOrEmpty(location))
                {
                    if (fileSystem.FileExists(location))
                    {
                        return location;
                    }
                    if (fileSystem.FileExists(msbuild, location))
                    {
                        return Path.Combine(location, msbuild);
                    }
                }
                return location;
            }

        }
    }
}
