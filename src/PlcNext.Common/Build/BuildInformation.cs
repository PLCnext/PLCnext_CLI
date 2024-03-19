#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using System.Text;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using Entity = PlcNext.Common.DataModel.Entity;
using Target = PlcNext.Common.Tools.SDK.Target;

namespace PlcNext.Common.Build
{
    internal class BuildInformation
    {
        public Entity RootEntity { get; }
        public FileEntity RootFileEntity => FileEntity.Decorate(RootEntity);
        public ProjectEntity RootProjectEntity => ProjectEntity.Decorate(RootEntity);
        public BuildEntity BuildEntity => BuildEntity.Decorate(RootProjectEntity.ValidatedTargets.FirstOrDefault(e => e.Value<Target>() == Target)??
                                                               RootProjectEntity.Targets.FirstOrDefault(e => e.Value<Target>() == Target));
        public SdkInformation SdkInformation { get; set; }
        public Target Target { get; set; }
        public bool Configure { get; }
        public bool NoConfigure { get; }
        public string BuildProperties { get; set; }

        public bool BuildPropertiesSetByFile { get; set; }
        public string Output { get; }

        public BuildInformation(Entity rootEntity, bool configure, bool noConfigure,
                                string buildProperties, string output)
        {
            RootEntity = rootEntity;
            Configure = configure;
            NoConfigure = noConfigure;
            BuildProperties = buildProperties;
            Output = output;
        }
    }
}
