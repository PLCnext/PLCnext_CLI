#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using System;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Tools.MSBuild
{
    internal class MSBuildProjectInformationProvider
    {
        public static string GetCSharpProjectOutputPath(IProcessManager processManager, ILog log, string csharpProjectPath, MSBuildData msbuild)
        {
            StringBuilderUserInterface userInterface = new StringBuilderUserInterface(log, writeInformation: true, writeError: true);
            string arguments = $"{(string.IsNullOrEmpty(msbuild.Parameters) ? string.Empty : msbuild.Parameters + " ")}-target:GetProjectVariables \"{csharpProjectPath}\"";
            using (IProcess process = processManager.StartProcess(msbuild.FullName, arguments, userInterface))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new FormattableException("Project information could not be fetched from csharp project. See log for details.");
                }
            }
            if (!EclrVersionIsSupported(userInterface.Information))
            {
                throw new FormattableException("This version of the PLCnCLI supports only eCLR versions 3.3.0 and 3.4.0");
            }
            return GetProjectOutputPath(userInterface.Information);

            bool EclrVersionIsSupported(string information)
            {
                Match match = Regex.Match(information, @"@begineclrversion(?<eclrversion>.*)@endeclrversion");
                if (match.Success)
                {
                    return match.Groups["eclrversion"].Value.Equals("eCLR3.3", StringComparison.OrdinalIgnoreCase) ||
                        match.Groups["eclrversion"].Value.Equals("eCLR3.4", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }

            string GetProjectOutputPath(string information)
            {
                Match match = Regex.Match(information, @"@beginoutputpath(?<outputpath>.*)@endoutputpath");
                if (match.Success)
                {
                    return match.Groups["outputpath"].Value;
                }
                throw new FormattableException("Project output path could not be fetched from csharp project. See log for details.");
            }
        }
    }
}
