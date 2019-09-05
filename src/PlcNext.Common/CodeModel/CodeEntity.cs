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

        public IEnumerable<string> Attributes => HasContent(EntityKeys.AttributesKey)
                                                     ? this[EntityKeys.AttributesKey].Select(e => e.Value<string>())
                                                     : Enumerable.Empty<string>();

        public IEnumerable<CodeEntity> Fields => HasContent(EntityKeys.FieldsKey)
                                                     ? this[EntityKeys.FieldsKey].Select(Decorate)
                                                     : Enumerable.Empty<CodeEntity>();

        public CodeEntity ResolvedType => HasContent(EntityKeys.ResolvedTypeKey) && this[EntityKeys.ResolvedTypeKey] != null
                                          ? Decorate(this[EntityKeys.ResolvedTypeKey])
                                          : null;

        public IField AsField => Value<IField>();
        public IType AsType => Value<IType>();
        public IEnum AsEnum => Value<IEnum>();
    }
}
