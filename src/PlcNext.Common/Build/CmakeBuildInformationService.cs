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
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Build
{
    internal class CmakeBuildInformationService : IBuildInformationService
    {
        private readonly CmakeExecuter cmakeExecuter;
        private readonly ISdkRepository sdkRepository;

        public CmakeBuildInformationService(CmakeExecuter cmakeExecuter, ISdkRepository sdkRepository)
        {
            this.cmakeExecuter = cmakeExecuter;
            this.sdkRepository = sdkRepository;
        }

        public BuildSystemProperties RetrieveBuildSystemProperties(Entity rootEntity, Target projectTarget,
                                                                   ChangeObservable observable)
        {
            BuildInformation buildInformation = new BuildInformation(rootEntity,
                                                                     null,
                                                                     false,
                                                                     false,
                                                                     string.Empty,
                                                                     null)
            {
                Target = projectTarget,
                SdkInformation = sdkRepository.GetSdk(projectTarget)
            };
            (bool success, VirtualDirectory cmakeFolder) = cmakeExecuter.EnsureConfigured(
                buildInformation, showWarningsToUser: true, observable, showMessagesToUser: false);
            JObject codeModel = buildInformation.BuildEntity.BuildSystem.Value<JObject>();
            IEnumerable<string> includePaths = GetIncludePathsFromCodeModel();
            return new BuildSystemProperties(includePaths);

            IEnumerable<string> GetIncludePathsFromCodeModel()
            {
                JObject cmakeTarget = codeModel;
                var paths = 
                    cmakeTarget["compileGroups"]?
                        .SelectMany(grp => grp["includes"]?.Select(inc => inc["path"]?.Value<string>()));
                if (paths != null)
                {
                    return paths.ToArray();
                }
                return Enumerable.Empty<string>();
            }
        }
    }
}
