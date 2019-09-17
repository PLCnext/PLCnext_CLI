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
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Build
{
    internal class CmakeBuildInformationService : IBuildInformationService
    {
        private readonly CmakeExecuter cmakeExecuter;
        private readonly ISdkRepository sdkRepository;
        private readonly ICMakeConversation cMakeConversation;
        private readonly IFileSystem fileSystem;

        public CmakeBuildInformationService(CmakeExecuter cmakeExecuter, ISdkRepository sdkRepository,
                                            ICMakeConversation cMakeConversation, IFileSystem fileSystem)
        {
            this.cmakeExecuter = cmakeExecuter;
            this.sdkRepository = sdkRepository;
            this.cMakeConversation = cMakeConversation;
            this.fileSystem = fileSystem;
        }

        public BuildSystemProperties RetrieveBuildSystemProperties(Entity rootEntity, Target projectTarget,
                                                                   ChangeObservable observable)
        {
            (bool success, VirtualDirectory cmakeFolder) = cmakeExecuter.EnsureConfigured(
                new BuildInformation(rootEntity,
                                     null,
                                     false,
                                     false,
                                     string.Empty,
                                     null)
                {
                    Target = projectTarget,
                    Sdk = sdkRepository.GetSdk(projectTarget)
                }, showWarningsToUser: true, observable, showMessagesToUser: false);
            JArray codeModel = cMakeConversation.GetCodeModelFromServer(fileSystem.GetTemporaryDirectory(),
                                                                        FileEntity.Decorate(rootEntity).Directory,
                                                                        cmakeFolder);
            IEnumerable<string> includePaths = GetIncludePathsFromCodeModel();
            return new BuildSystemProperties(includePaths);

            IEnumerable<string> GetIncludePathsFromCodeModel()
            {
                List<string> result = new List<string>();
                JObject cmakeTarget = codeModel.GetProjectTarget(rootEntity.Name, false);
                if (cmakeTarget != null && cmakeTarget["fileGroups"] is JArray fileGroups)
                {
                    foreach (JObject fileGroup in fileGroups.OfType<JObject>())
                    {
                        if (fileGroup["includePath"] is JArray paths)
                        {
                            result.AddRange(paths.Select(path => path["path"]?.Value<string>())
                                                 .Where(include => !string.IsNullOrEmpty(include)));
                        }
                    }
                }

                return result;
            }
        }
    }
}
