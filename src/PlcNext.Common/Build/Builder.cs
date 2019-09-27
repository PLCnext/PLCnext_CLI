#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Project;
using PlcNext.Common.Project.Persistence;
using Entity = PlcNext.Common.DataModel.Entity;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Build
{
    internal class Builder : IBuilder
    {
        private readonly IUserInterface userInterface;
        private readonly ISdkRepository sdkRepository;
        private readonly IBuildExecuter buildExecuter;
        private readonly ILibraryBuilderExecuter libraryBuilderExecuter;
        private readonly ITargetParser targetParser;
        private readonly IFileSystem fileSystem;

        public Builder(IUserInterface userInterface, ISdkRepository sdkRepository,
                       IBuildExecuter buildExecuter, ILibraryBuilderExecuter libraryBuilderExecuter, ITargetParser targetParser, IFileSystem fileSystem)
        {
            this.userInterface = userInterface;
            this.sdkRepository = sdkRepository;
            this.buildExecuter = buildExecuter;
            this.libraryBuilderExecuter = libraryBuilderExecuter;
            this.targetParser = targetParser;
            this.fileSystem = fileSystem;
        }

        public void Build(BuildInformation buildInfo, ChangeObservable observable, IEnumerable<string> targets)
        {
            if (!targets.Any())
            {
                TargetsResult projectTargets = targetParser.Targets(buildInfo.RootProjectEntity);
                projectTargets.Errors.ThrowIfNotEmpty();
                if (!projectTargets.ValidTargets.Any())
                {
                    throw new NoAssignedTargetsException(buildInfo.RootEntity.Name);
                }

                BuildProjectForTargets(buildInfo, observable, projectTargets.ValidTargets);
            }
            else
            {
                //build for selected target(s)
                BuildProjectForTargets(buildInfo, observable, targetParser.GetSpecificTargets(targets).Select(t => t.Item1));
            }
        }

        private void BuildProjectForTargets(BuildInformation buildInfo, ChangeObservable observable, IEnumerable<Target> targets)
        {
            targets = targets.ToArray();
            MultiDictionary<SdkInformation, Target> sdks = new MultiDictionary<SdkInformation, Target>();
            List<FormattableException> exceptions = new List<FormattableException>();
            foreach (Target target in targets)
            {
                try
                {
                    sdks.Add(sdkRepository.GetSdk(target), target);
                }
                catch (FormattableException ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            userInterface.WriteInformation($"Requested build for targets {String.Join(", ", targets.Select(x => x.GetFullName()).ToArray())}");
            foreach (SdkInformation sdk in sdks.Keys)
            {
                foreach (Target target in sdks.Get(sdk))
                {
                    userInterface.WriteInformation($"Starting build for target {target.GetFullName()}");
                    buildInfo.SdkInformation = sdk;
                    buildInfo.Target = target;
                    buildExecuter.ExecuteBuild(buildInfo, observable);
                    userInterface.WriteInformation($"Successfully built the project {buildInfo.RootEntity.Name} for target {target.GetFullName()}.");
                }
            }
            userInterface.WriteInformation($"Finished build for all targets");
        }

        public int BuildLibraryForProject(Entity project, ChangeObservable observable,
                                          string metaFilesDirectory, string libraryLocation, string outputDirectory,
                                          Guid libraryGuid, IEnumerable<string> targets,
                                          IEnumerable<string> externalLibraries, string buildType)
        {
            userInterface.WriteInformation("Starting library generation...");

            ProjectEntity projectEntity = ProjectEntity.Decorate(project);
            IEnumerable<(Target, string)> targetsSet;
            if (!targets.Any())
            {
                TargetsResult ts = targetParser.Targets(projectEntity, false);
                if (!ts.ValidTargets.Any())
                {
                    throw new FormattableException
                        ("Please use --target to specify for which targets the library shall be generated.");
                }
                else
                {
                    HashSet<(Target, string)> targetsHashSet = new HashSet<(Target, string)>();
                    foreach (Target target in ts.ValidTargets)
                    {
                        targetsHashSet.Add((target, null));
                    }
                    targetsSet = targetsHashSet;
                }
            }
            else
            {
                targetsSet = targetParser.GetSpecificTargets(targets, false);
            }

            Dictionary<Target, IEnumerable<VirtualFile>> externalLibs =
                ExternalLibrariesParser.ParseExternalLibraries(externalLibraries, targetParser,
                                                                fileSystem, targetsSet.Select(t => t.Item1));
             
            int result = libraryBuilderExecuter.Execute(projectEntity, metaFilesDirectory, libraryLocation,
                outputDirectory, observable, userInterface, libraryGuid, targetsSet, externalLibs, buildType);
            if (result == 0)
            {
                userInterface.WriteInformation("Successfully generated library!");
            }
            return result;
        }
    }
}
