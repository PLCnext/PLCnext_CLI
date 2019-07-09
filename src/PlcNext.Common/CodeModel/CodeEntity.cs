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

namespace PlcNext.Common.CodeModel
{
    public class CodeEntity : EntityBaseDecorator
    {
        private CodeEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static CodeEntity Decorate(IEntityBase codeEntity)
        {
            return Decorate<CodeEntity>(codeEntity) ?? new CodeEntity(codeEntity);
        }

        public string Namespace => this[EntityKeys.NamespaceKey].Value<string>();
    }
}
