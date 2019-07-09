#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Tools.DynamicCommands
{
    public class ArgumentCollection : IEnumerable<Argument>
    {
        private readonly IEnumerable<Argument> innerCollection;

        public ArgumentCollection(IEnumerable<Argument> innerCollection)
        {
            this.innerCollection = innerCollection;
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) innerCollection).GetEnumerator();
        }

        public Argument this[string key]
        {
            get { return innerCollection.FirstOrDefault(a => a.Name == key); }
        }

        public bool ContainsArgument(string key)
        {
            return this[key] != null;
        }

        public override string ToString()
        {
            return $"{nameof(innerCollection)}: {string.Join(",", innerCollection)}";
        }
    }
}
