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
        
        public VirtualDirectory ConfigDirectory => FileEntity.Decorate(this[EntityKeys.InternalConfigPathKey]).Directory;

 #region SigningProperties
        public bool SignRequested => this[EntityKeys.InternalSigningKey].Value<bool>();

        public string PKCS12Container => this[EntityKeys.InternalPKCS12Key].Value<string>();

        public string PrivateKeyPath => this[EntityKeys.InternalPrivateKeyKey].Value<string>();

        public string PublicKeyPath => this[EntityKeys.InternalPublicKeyKey].Value<string>();

        public IEnumerable<string> CertificatePaths => this[EntityKeys.InternalCertificatesKey].Value<IEnumerable<string>>();

        public bool TimestampFlag => this[EntityKeys.InternalTimestampKey].Value<bool>();

        public string TimestampConfigPath => this[EntityKeys.InternalTimestampConfigKey].Value<string>();

        public string SigningPassword => this[EntityKeys.InternalPasswordKey].Value<string>();
#endregion
    }
}
