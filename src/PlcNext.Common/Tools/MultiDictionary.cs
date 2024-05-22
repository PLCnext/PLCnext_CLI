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

namespace PlcNext.Common.Tools
{
    internal class MultiDictionary<TKey, TValue>
    {
        private Dictionary<TKey, List<TValue>> data = new Dictionary<TKey, List<TValue>>();

        public IEnumerable<TKey> Keys => data.Keys;

        public void Add(TKey key, TValue value)
        {
            if (data.TryGetValue(key, out List<TValue> list))
            {
                if (!list.Contains(value)) // do not allow duplicate values
                    list.Add(value);
            }
            else
                data.Add(key, new List<TValue>() { value });
        }

        public List<TValue> Get(TKey key)
        {
            if (data.TryGetValue(key, out List<TValue> list))
                return list;
            else
                return new List<TValue>();
        }

        public bool Remove(TKey key, TValue value)
        {
            bool result = false;
            if (data.TryGetValue(key, out List<TValue> list))
            {
                result = list.Remove(value);
                if (!list.Any())
                {
                    data.Remove(key);
                }
            }
            return result;
        }
    }
}
