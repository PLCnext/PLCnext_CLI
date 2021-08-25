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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Deploy.SolutionMapping;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Deploy
{
    public class SolutionMappingManager
    {
        private readonly IEnvironmentService environmentService;
        private readonly IFileSystem fileSystem;

        private static Regex VersionRangePattern = new Regex(@"^(?<lowerinorex>\[|\()(?<lowerbound>\d+(.\d+)?(.\d+)?(.\d+)?),(?<upperbound>(\d+(.\d+)?(.\d+)?(.\d+)?)?)(?<upperinorex>\]|\))$", RegexOptions.Compiled);

        public SolutionMappingManager(IEnvironmentService environmentService,
                                      IFileSystem fileSystem)
        {
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;
        }

        public string GetSolutionVersionFromEngineerVersion(string engineerVersion)
        {
            SolutionMappings mappings = LoadSolutionMappings();

            IEnumerable<SolutionMapping.SolutionMapping> matchingMappings = mappings?.Mapping.Where(m => IsMatch(m.EngineerVersionRange, engineerVersion));

            if (matchingMappings.Count() > 1)
                throw new FormattableException("The SolutionMappings file contains invalid mappings: There is more than one mapping for version " + engineerVersion);

            if (matchingMappings.Count() < 1)
                throw new FormattableException($"The selected Engineer version {engineerVersion} is not in the range of known versions.");

            return matchingMappings.FirstOrDefault()?.SolutionVersion;
            
            bool IsMatch(string versionRange, string wantedVersion)
            {
                if(Version.TryParse(wantedVersion, out Version wanted))
                {
                    if (wanted < new Version("2020.0"))
                    {
                        throw new FormattableException($"Selected Engineer version  is {wanted}, but versions below 2020.0 are not supported.");
                    }

                    Match patternMatch = VersionRangePattern.Match(versionRange);
                    if (!patternMatch.Success)
                    {
                        return false;
                    }

                    string lowerinorex = patternMatch.Groups["lowerinorex"].Value;
                    string upperinorex = patternMatch.Groups["upperinorex"].Value;
                    if (!Version.TryParse(patternMatch.Groups["lowerbound"].Value, out Version lowerbound))
                    {
                        return false;
                    }
                    if (!Version.TryParse(patternMatch.Groups["upperbound"].Value, out Version upperbound))
                    {
                        if (!string.IsNullOrEmpty(patternMatch.Groups["upperbound"].Value))
                        {
                            return false;
                        }
                        upperbound = null;
                    }

                    if (lowerinorex == "[")
                    {
                        if (wanted < lowerbound)
                            return false;
                    }else
                    {
                        if (wanted <= lowerbound)
                            return false;
                    }
                    if (upperbound == null)
                    {
                        return true;
                    }
                    if (upperinorex == "]")
                    {
                        if (wanted > upperbound)
                            return false;
                    }
                    else
                    {
                        if (wanted >= upperbound)
                            return false;
                    }
                    return true;                    
                }
                throw new FormattableException($"The value {wantedVersion} does not have the version format major.minor[.build[.revision]].");
            }
        }

        private SolutionMappings LoadSolutionMappings()
        {

            string solutionMappingsFile = Path.Combine(environmentService.AssemblyDirectory, Constants.SolutionMappingsFileName);
            if (!fileSystem.FileExists(solutionMappingsFile))
            {
                throw new SolutionMappingsMissingException();
            }

            using (Stream solutionMappingsFileStream = fileSystem.GetFile(solutionMappingsFile).OpenRead())
            using (XmlReader reader = XmlReader.Create(solutionMappingsFileStream))
            {
                XmlSerializer serializer = new(typeof(SolutionMappings));

                return (SolutionMappings) serializer.Deserialize(reader);
            }
        }
    }
}
