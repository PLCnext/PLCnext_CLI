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

namespace PlcNext.Common.Tools.FileSystem
{
    public class FileEntity : EntityBaseDecorator
    {
        private FileEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static FileEntity Decorate(IEntityBase entityBase)
        {
            return Decorate<FileEntity>(entityBase) ?? new FileEntity(entityBase);
        }

        public VirtualDirectory Directory => HasValue<VirtualDirectory>()
                                                 ? Value<VirtualDirectory>()
                                                 : this[EntityKeys.InternalDirectoryKey].Value<VirtualDirectory>();

        public VirtualFile File => HasValue<VirtualFile>()
                                       ? Value<VirtualFile>()
                                       : null;
    }
}
