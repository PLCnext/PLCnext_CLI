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

namespace PlcNext.Common.DataModel
{
    internal class EntityFactory : IEntityFactory
    {
        private readonly IEntityContentProvider contentProvider;

        public EntityFactory(IEntityContentProvider contentProvider)
        {
            this.contentProvider = contentProvider;
        }

        public Entity Create(string type, params object[] entityValues)
        {
            return new Entity(type, contentProvider, entityValues);
        }

        public Entity Create(string type, IEnumerable<Entity> entityCollection, params object[] entityValues)
        {
            return new Entity(type, contentProvider, entityValues, entityCollection);
        }
    }
}
