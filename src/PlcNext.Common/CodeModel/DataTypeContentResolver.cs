#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;

namespace PlcNext.Common.CodeModel
{
    internal class DataTypeContentResolver : PriorityContentProvider
    {
        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<IDataType>() &&
                   key is EntityKeys.NameKey or EntityKeys.FullNameKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            IDataType dataType = owner.Value<IDataType>();
            
            switch (key)
            {
                case EntityKeys.NameKey:
                    return owner.Create(key, dataType.Name);
                case EntityKeys.FullNameKey:
                    string fullName = GetDataTypeFullName();
                    return owner.Create(key, fullName);
                default:
                    throw new ContentProviderException(key, owner);
            }

            string GetDataTypeFullName()
            {
                ICodeModel rootCodeModel = owner.Root.Value<ICodeModel>();
                return dataType.PotentialFullNames.FirstOrDefault(fn => rootCodeModel.Type(fn) != null)
                       ?? dataType.Name;
            }
        }
    }
}