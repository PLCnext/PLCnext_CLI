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
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Build
{
    public class BuildEntity : EntityBaseDecorator
    {
        public BuildEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static BuildEntity Decorate(IEntityBase commandEntity)
        {
            return Decorate<BuildEntity>(commandEntity) ?? new BuildEntity(commandEntity);
        }

        public string BuildType => this[EntityKeys.BuildTypeKey].Value<string>();
        public bool HasBuildSystem => HasContent(EntityKeys.InternalBuildSystemKey);
        public BuildEntity BuildSystem => Decorate(this[EntityKeys.InternalBuildSystemKey]);
        public VirtualDirectory BuildSystemDirectory => this[EntityKeys.InternalBuildSystemDirectoryKey].Value<VirtualDirectory>();
        public IEnumerable<string> ExternalLibraries => this[EntityKeys.InternalExternalLibrariesKey].Select(e => e.Value<string>());
        public IEnumerable<string> InstallationPaths => this[EntityKeys.InternalInstallationPathsKey].Select(e => e.Value<string>());
        public IEnumerable<string> ExcludedFiles => this[EntityKeys.ExcludeFilesKey].Select(e => e.Value<string>());

    }
}
