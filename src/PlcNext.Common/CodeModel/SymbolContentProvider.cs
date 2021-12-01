#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.DataModel;

namespace PlcNext.Common.CodeModel
{
    internal class SymbolContentProvider : PriorityContentProvider
    {
        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<ISymbol>() &&
                   owner.Value<ISymbol>().HasPropertyValueEntity(key);
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            ISymbol symbol = owner.Value<ISymbol>();
            Entity result = owner.PropertyValueEntity(key, symbol);
            if (result != null)
            {
                return result;
            }

            throw new ContentProviderException(key, owner);
        }
    }
}