#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppField : CppCodeEntity, IField
    {
        public CppField(string name, IDataType dataType, CppComment[] comments, string[] multiplicity,
                        string attributePrefix, IType containingType = null) : base(name, attributePrefix)
        {
            DataType = dataType;
            Comments = comments;
            Multiplicity = multiplicity;
            ContainingType = containingType;
        }

        public CppField(string name, IDataType dataType, IReadOnlyCollection<IComment> comments, IReadOnlyCollection<string> multiplicity,
                        string attributePrefix, IType containingType = null) : base(name, attributePrefix)
        {
            DataType = dataType;
            Comments = comments;
            Multiplicity = multiplicity;
            ContainingType = containingType;
        }

        public IDataType DataType { get; }
        public IReadOnlyCollection<string> Multiplicity { get; }
        public IType ContainingType { get; private set; }

        public static IEnumerable<CppField> Parse(ParseNode declaration, string[] usings, string ns,
                                                  List<ParserMessage> messages, string attributePrefix,
                                                  CppType containingType)
        {
            if (!declaration.IsValidFieldDeclaration())
            {
                return Enumerable.Empty<CppField>();
            }

            ParseNode[] identifiers = declaration.GetFieldIdentifier();
            if (identifiers.FirstOrDefault()?.ToString() == "using")
            {
                //using directive inside class/struct
                return Enumerable.Empty<CppField>();
            }
            ParseNode[] typeNodes = declaration.GetFieldTypeNodes(identifiers);
            if (identifiers.SequenceEqual(typeNodes))
            {
                if (typeNodes.Any())
                {
                    (int line, int column) position = declaration.Position;
                    messages.Add(new ParserMessage("CPP0001", position.line, position.column));
                }

                //Empty group "private:"
                return Enumerable.Empty<CppField>();
            }

            CppDataType dataType = typeNodes.GetFieldDataType(usings, ns);

            IEnumerable<ParseNode> fieldIdentifiers = identifiers.Intersect(typeNodes).Any()
                                                          ? identifiers.SkipWhile(i => !typeNodes.Contains(i))
                                                                       .SkipWhile(typeNodes.Contains)
                                                          : identifiers;

            IEnumerable<(string name, string[] multiplicity)> fields = fieldIdentifiers 
               .Select(i => (i.ToString(), i.GetParent().GetFieldMultiplicity()));
            CppComment[] comments = declaration.GetFieldComments();

            return fields.Select(fd => new CppField(fd.name, dataType, comments, fd.multiplicity, attributePrefix, containingType));
        }

        public void RegisterContainingType(CppType cppType)
        {
            ContainingType = cppType;
        }
    }
}
