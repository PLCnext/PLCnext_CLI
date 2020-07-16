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

namespace PlcNext.Common.Project
{
    public class TargetEntity : EntityBaseDecorator
    {
        public TargetEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static TargetEntity Decorate(IEntityBase entityBase)
        {
            return Decorate<TargetEntity>(entityBase) ?? new TargetEntity(entityBase);
        }

        public bool HasFullName => HasContent(EntityKeys.TargetFullNameKey);

        public string FullName => this[EntityKeys.TargetFullNameKey].Value<string>();
        public string ShortFullName => this[EntityKeys.TargetShortFullNameKey].Value<string>();
        public string EngineerVersion => this[EntityKeys.TargetEngineerVersionKey].Value<string>();
        public Version Version => this[EntityKeys.TargetVersionKey].Value<Version>();
    }
}
