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
using System.Globalization;
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    internal class RelationshipTypeNotFoundException : FormattableException
    {
        public RelationshipTypeNotFoundException(string relationshipType, string relationshipName, IType[] availableTypes) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.RelationshipTypeNotFound, relationshipType, relationshipName, $"{Environment.NewLine}{string.Join(Environment.NewLine, availableTypes.Select(t => $"{Constants.Tab}- {t.FullName}"))}"))
        {
            
        }
    }
}
