#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Project
{
    internal class TargetParser : ITargetParser
    {
        private readonly Regex targetVersionParser = new Regex(@"^(?<name>[^,]+),(?<version>.+)$", RegexOptions.IgnoreCase);
        private readonly Regex targetVersionLocationParser = new Regex(@"^(?<name>[^,]+),(?<version>[^,]+),(?<location>.+)$", RegexOptions.IgnoreCase);

        private readonly ISdkRepository sdkRepository;
        private readonly ExecutionContext executionContext;

        public TargetParser(ISdkRepository sdkRepository, ExecutionContext executionContext)
        {
            this.sdkRepository = sdkRepository;
            this.executionContext = executionContext;
        }

        public TargetsResult Targets(ProjectEntity project, bool validateTargets = true)
        {
            if (validateTargets)
                return ParseTargets(project.Settings.Value.Target ?? new string[0]);
            else
                return new TargetsResult(GetProjectTargets(project), Enumerable.Empty<Exception>());

            TargetsResult ParseTargets(params string[] targets)
            {
                Target[] searchableTargets = sdkRepository.GetAllTargets().ToArray();
                HashSet<Target> result = new HashSet<Target>();
                List<Exception> errors = new List<Exception>();
                foreach (string target in targets)
                {
                    try
                    {
                        result.Add(ParseTarget(target, null, searchableTargets));
                    }
                    catch (FormattableException e)
                    {
                        errors.Add(e);
                    }
                }

                return new TargetsResult(result, errors);
            }
        }

        public string[] FormatTargets(IEnumerable<Target> targets, bool shortVersion)
        {
            if (shortVersion)
                return targets.Select(FormatTarget).ToArray();

            return targets.Select(FormatTargetLongVersion).ToArray();
        }
        private string FormatTarget(Target target)
        {
            return FormatTarget(target.Name, target.Version);
        }
        private string FormatTargetLongVersion(Target target)
        {
            return FormatTarget(target.Name, target.LongVersion);
        }
        private string FormatTarget(string name, string version)
        {
            return $"{name},{version}";
        }

        public Target ParseTarget(string target, string version, IEnumerable<Target> searchableTargets)
        {
            if (string.IsNullOrEmpty(version))
            {
                (target, version) = ParseTargetString(target);
            }

            IEnumerable<Target> possibleTargets =
                searchableTargets.Where(t => t.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
                                 .ToArray();
            if (!possibleTargets.Any())
            {
                Target closestTarget = searchableTargets.OrderBy(t => LevenshteinDistance.Compute(t.Name, target))
                                                        .FirstOrDefault();
                if (closestTarget != null)
                {
                    throw new TargetNameNotFoundException(target, closestTarget.Name);
                }
                throw new TargetNameNotFoundException(target);
            }
            if (!string.IsNullOrEmpty(version))
            {
                IEnumerable<Target> versionTarget = possibleTargets.Where(t => t.Version.StartsWith(version) || t.LongVersion.StartsWith(version))
                                                                   .ToArray();
                if (!versionTarget.Any())
                {
                    throw new TargetVersionNotFoundException(target, version, possibleTargets.Select(t => t.Version).ToArray());
                }
                possibleTargets = versionTarget;
            }

            if (possibleTargets.Count() > 1)
            {
                Target duplicateTarget = possibleTargets.GroupBy(t => t.GetFullName()).Where(g => g.Count() > 1).FirstOrDefault()?.FirstOrDefault();

                if (duplicateTarget != null)
                {
                    throw new TargetInstalledTwiceException(duplicateTarget.GetFullName());
                }

                string targetname = string.IsNullOrEmpty(version) ? target : $"{target} {version}";
                throw new TargetNameAmbiguousException(targetname, possibleTargets.Select(t => $"{t.Name} {t.Version}"));
            }
            return possibleTargets.FirstOrDefault();
        }

        (string, string) ParseTargetString(string target)
        {
            string version = string.Empty;
            Match match = targetVersionParser.Match(target);
            if (match.Success)
            {
                target = match.Groups["name"].Value;
                version = match.Groups["version"].Value;
            }
            else if (string.IsNullOrEmpty(target))
            {
                throw new FormattableException("Target name cannot be empty.");
            }

            return (target, version);
        }

        public Target GetSpecificTarget(string target, string version)
        {
            return ParseTarget(target, version, sdkRepository.GetAllTargets() );
        }

        public Target RemoveTarget(ProjectEntity project, string target, string version)
        {
            IEnumerable<Target> searchableTargets = GetProjectTargets(project);

            Target matchingTarget = ParseTarget(target, version, searchableTargets);
            if (matchingTarget == null)
                throw new TargetNameNotFoundException(target);

            string formattedTarget = FormatTargetLongVersion(matchingTarget);

            if (project.Settings.RemoveTarget(formattedTarget))
                return matchingTarget;
            return null;
        }

        private IEnumerable<Target> GetProjectTargets(ProjectEntity project)
        {
            List<Target> projectTargets = new List<Target>();
            List<string> removableTargets = new List<string>();
            foreach (string targetEntry in project.Settings.Value.Target ?? new string[0])
            {
                Match match = targetVersionParser.Match(targetEntry);
                if (match.Success)
                {
                    projectTargets.Add(new Target(match.Groups["name"].Value,
                                                     match.Groups["version"].Value));
                }
                else
                {
                    removableTargets.Add(targetEntry);
                    executionContext.WriteInformation($"Project file contained invalid Target entry: {targetEntry}");
                }
            }

            project.Settings.RemoveTargets(removableTargets);
            if (removableTargets.Any())
            {
                executionContext.WriteInformation("All invalid targets were removed from the project file.");
            }

            return projectTargets;
        }

        public Target AddTarget(ProjectEntity project, string target, string version)
        {
            Target realTarget = GetSpecificTarget(target, version);
            string formattedTarget = FormatTargetLongVersion(realTarget);
            if ((project.Settings.Value.Target ?? new string[0])
               .Any(x => x.Equals(formattedTarget, StringComparison.OrdinalIgnoreCase)
                         || x.Equals(FormatTarget(realTarget), StringComparison.OrdinalIgnoreCase)))
            {
                throw new TargetAlreadySupportedException(formattedTarget);
            }

            project.Settings.AddTarget(formattedTarget);

            return realTarget;
        }

        public IEnumerable<(Target, string)> GetSpecificTargets(IEnumerable<string> targets, bool validate = true)
        {
            HashSet<(Target, string)> result = new HashSet<(Target, string)>();
            foreach (string target in targets)
            {
                string name = target;
                string version = string.Empty;
                string location = null;

                Match match = targetVersionLocationParser.Match(target);
                if (match.Success)
                {
                    name = match.Groups["name"].Value;
                    version = match.Groups["version"].Value;
                    location = match.Groups["location"].Value;
                }
                else if ((match = targetVersionParser.Match(target)).Success)
                {
                    name = match.Groups["name"].Value;
                    version = match.Groups["version"].Value;
                }

                if (string.IsNullOrEmpty(target))
                {
                    throw new FormattableException("Target name cannot be empty.");
                }

                Target t = validate
                               ? ParseTarget(name, version, sdkRepository.GetAllTargets())
                               : TryParseTarget(name, version, sdkRepository.GetAllTargets());
                result.Add((t, location));
            }
            return result;
        }

        private Target TryParseTarget(string target, string version, IEnumerable<Target> searchableTargets)
        {
            try
            {
                return ParseTarget(target, version, searchableTargets);
            }
            catch (FormattableException e)
            {
                executionContext.WriteInformation($"For the target {target},{version} no corresponding SDK was found. " +
                                                          "Please make sure you provided the complete version string. " +
                                                          $"The reason no SDK was found is:{Environment.NewLine}" +
                                                          $"{e.Message}");
                return new Target(target, version);
            }
        }

        public void UpdateTargets(ProjectEntity project)
        {
            foreach (Target projectTarget in GetProjectTargets(project).ToArray())
            {
                if (sdkRepository.GetAllTargets()
                                 .Any(t => t.Name == projectTarget.Name &&
                                           t.LongVersion == projectTarget.LongVersion))
                {
                    continue;
                }

                (Target, Version)[] availableVersions = sdkRepository.GetAllTargets()
                                                                     .Where(t => t.Name == projectTarget.Name)
                                                                     .Select(t => (t, Version.Parse(t.Version)))
                                                                     .ToArray();
                if (!availableVersions.Any())
                {
                    throw new TargetNameNotFoundException(projectTarget.Name);
                }

                Version version = Version.Parse(projectTarget.Version);
                Target newVersion = availableVersions.Where(t => t.Item2 > version)
                                                     .OrderBy(t => t.Item2)
                                                     .FirstOrDefault()
                                                     .Item1
                                    ?? availableVersions.OrderBy(t => t.Item2)
                                                        .First()
                                                        .Item1;
                RemoveTarget(project, projectTarget.Name, projectTarget.LongVersion);
                AddTarget(project, newVersion.Name, newVersion.LongVersion);
                executionContext.WriteInformation($"Updated the target '{projectTarget.Name}' from version {projectTarget.Version} to version {newVersion.Version}");
            }
        }
    }
}