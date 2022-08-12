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
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Priority;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Project
{
    internal class TargetParser : PriorityContentProvider, ITargetParser
    {
        private readonly Regex targetVersionParser = new Regex(@"^(?<name>[^,]+)(?:,(?<version>.+))?$", RegexOptions.IgnoreCase);
        private readonly Regex targetVersionLocationParser = new Regex(@"^(?<name>[^,]+),(?<version>[^,]+),(?<location>.+)$", RegexOptions.IgnoreCase);

        private readonly ISdkRepository sdkRepository;
        private readonly ExecutionContext executionContext;

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return (owner.IsRoot() &&
                    key == EntityKeys.TargetsKey) ||
                   (owner.IsRoot() &&
                    key == EntityKeys.ValidatedTargetsKey) ||
                    (owner.IsRoot() &&
                    key == EntityKeys.MinTargetVersionKey) ||
                   (key == EntityKeys.TargetFullNameKey &&
                    owner.HasValue<Target>()) ||
                   (key == EntityKeys.TargetShortFullNameKey &&
                    owner.HasValue<Target>()) ||
                   (key == EntityKeys.TargetEngineerVersionKey &&
                    owner.HasValue<Target>()) ||
                   (key == EntityKeys.TargetVersionKey &&
                    owner.HasValue<Target>()) ||
                   (key == EntityKeys.NameKey &&
                    owner.HasValue<Target>());
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.TargetFullNameKey:
                    return owner.Create(key, owner.Value<Target>().GetFullName());
                case EntityKeys.TargetShortFullNameKey:
                    return owner.Create(key, owner.Value<Target>().GetShortFullName());
                case EntityKeys.TargetEngineerVersionKey:
                    return owner.Create(key, EngineerVersion(owner.Value<Target>()));
                case EntityKeys.TargetVersionKey:
                    Version version = new Version();
                    string versionString = owner.Value<Target>().Version;
                    if (Version.TryParse(versionString, out Version parsedVersion))
                    {
                        version = parsedVersion;
                    }
                    return owner.Create(key, version);
                case EntityKeys.NameKey:
                    return owner.Create(key, owner.Value<Target>().Name);
                case EntityKeys.MinTargetVersionKey:
                    return owner.Create(key, MinTargetVersion());
                default:
                    return GetTargets(key == EntityKeys.ValidatedTargetsKey);
            }

            Entity GetTargets(bool validate)
            {
                IEnumerable<Target> targetsSet = GetTargetsSet(validate);
                return owner.Create(key, targetsSet.Select(t => owner.Create(key.Singular(), t.Name, t)));
            }
            IEnumerable<Target> GetTargetsSet(bool validate)
            {
                CommandEntity commandEntity = CommandEntity.Decorate(owner.Origin);
                if (commandEntity.IsCommandArgumentSpecified(Constants.TargetArgumentName))
                {
                    IEnumerable<string> targets = commandEntity.GetMultiValueArgument(Constants.TargetArgumentName);
                    return GetSpecificTargets(targets, validate, false).Select(t => t.Item1);
                }
                ProjectEntity project = ProjectEntity.Decorate(owner);
                return Targets(project, validate).ValidTargets;
            }

            string EngineerVersion(Target target)
            {
                string possibleVersion = target.LongVersion.Trim().Split(' ')[0];
                if (Version.TryParse(possibleVersion, out Version version))
                {
                    int parts = possibleVersion.Split('.').Length;
                    if (parts == 2)
                    {
                        return $"{version.ToString(2)}.0";
                    }

                    if (parts > 2)
                    {
                        return version.ToString(3);
                    }
                }

                return target.ShortVersion;
            }

            Entity MinTargetVersion()
            {
                IEnumerable<Target> targetsSet = GetTargetsSet(false);
                string minVersion = targetsSet.Min(t => t.Version);
                Target minTarget = targetsSet.Where(t => t.Version == minVersion).FirstOrDefault();
                if (minTarget == null || string.IsNullOrEmpty(minTarget.Version))
                {
                    return owner.Create(key, "99.99.99"); // if no target found, assume newest target
                }
                return owner.Create(key, minTarget.Version);
            }
        }

        public TargetParser(ISdkRepository sdkRepository, ExecutionContext executionContext)
        {
            this.sdkRepository = sdkRepository;
            this.executionContext = executionContext;
        }

        public TargetsResult Targets(ProjectEntity project, bool validateTargets = true)
        {
            if (validateTargets)
                return ParseTargets(project.Settings.Value.Target ?? Array.Empty<string>());
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
        private static string FormatTarget(Target target)
        {
            return FormatTarget(target.Name, target.Version);
        }
        private static string FormatTargetLongVersion(Target target)
        {
            return FormatTarget(target.Name, target.LongVersion);
        }
        private static string FormatTarget(string name, string version)
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
                IEnumerable<Target> versionTarget = possibleTargets.Where(t => t.Version.StartsWith(version,
                                                                                   StringComparison.Ordinal) || 
                                                                               t.LongVersion.StartsWith(version,
                                                                                   StringComparison.Ordinal))
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
            foreach (string targetEntry in project.Settings.Value.Target ?? Array.Empty<string>())
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
            if ((project.Settings.Value.Target ?? Array.Empty<string>())
               .Any(x => x.Equals(formattedTarget, StringComparison.OrdinalIgnoreCase)
                         || x.Equals(FormatTarget(realTarget), StringComparison.OrdinalIgnoreCase)))
            {
                throw new TargetAlreadySupportedException(formattedTarget);
            }

            project.Settings.AddTarget(formattedTarget);

            return realTarget;
        }

        public IEnumerable<(Target, string)> GetSpecificTargets(IEnumerable<string> targets, bool validate = true, bool parseLocation = true)
        {
            HashSet<(Target, string)> result = new HashSet<(Target, string)>();
            foreach (string target in targets)
            {
                string name = target;
                string version = string.Empty;
                string location = null;

                Match match = targetVersionLocationParser.Match(target);
                if (match.Success && parseLocation)
                {
                    name = match.Groups["name"].Value;
                    version = match.Groups["version"].Value;
                    location = match.Groups["location"].Value;
                }
                else if (!match.Success && (match = targetVersionParser.Match(target)).Success)
                {
                    name = match.Groups["name"].Value;
                    version = match.Groups["version"].Success ? match.Groups["version"].Value : string.Empty;
                }
                else
                {
                    throw new TargetFormatMismatchException(target, parseLocation);
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

        public void UpdateTargets(ProjectEntity project, bool downgrade)
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
                                                     .Item1;
                if(newVersion == null)
                {
                    if(downgrade)
                    {
                        newVersion = availableVersions.OrderBy(t => t.Item2)
                                                        .First()
                                                        .Item1;
                    }
                    else
                    {
                        throw new NoHigherTargetAvailableException(projectTarget.GetLongFullName());
                    }
                }

                RemoveTarget(project, projectTarget.Name, projectTarget.LongVersion);
                AddTarget(project, newVersion.Name, newVersion.LongVersion);
                executionContext.WriteInformation($"Updated the target '{projectTarget.Name}' from version {projectTarget.Version} to version {newVersion.Version}");
            }
        }
    }
}