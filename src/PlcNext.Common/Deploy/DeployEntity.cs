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
using System.Text;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Deploy
{
    public class DeployEntity : EntityBaseDecorator
    {
        private DeployEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static DeployEntity Decorate(IEntityBase commandEntity)
        {
            return Decorate<DeployEntity>(commandEntity) ?? new DeployEntity(commandEntity);
        }

        public VirtualDirectory DeployDirectory => FileEntity.Decorate(this[EntityKeys.InternalDeployPathKey]).Directory;
    }
}
