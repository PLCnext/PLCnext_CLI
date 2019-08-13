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

namespace PlcNext.Common.Templates
{
    internal class CollectiveTemplateIdentifierRepository : ITemplateIdentifierRepository
    {
        private readonly IEnumerable<ITemplateIdentifier> identifiers;

        public CollectiveTemplateIdentifierRepository(IEnumerable<ITemplateIdentifier> identifiers)
        {
            this.identifiers = identifiers;
        }

        public IEnumerable<Entity> FindAllEntities(string entityName, Entity owner, string identifierName)
        {
            ITemplateIdentifier identifier = GetIdentifier();

            return identifier.FindAllEntities(entityName, owner);

            ITemplateIdentifier GetIdentifier()
            {
                ITemplateIdentifier result = identifiers.FirstOrDefault(i => i.IdentifierKey.Equals(identifierName, 
                                                                                                    StringComparison.OrdinalIgnoreCase));
                if (result == null)
                {
                    throw new IdentifierNotFoundException(identifierName, entityName);
                }

                return result;
            }
        }
    }
}
