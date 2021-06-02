#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Produces(typeof(ClassFound))]
    [Produces(typeof(StructureFound))]
    [Produces(typeof(EnumFound))]
    [Intercepts(typeof(TypeDeclarationFound))]
    internal class TypeSpecifier : InterceptorAgent
    {
        public TypeSpecifier(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            TypeDeclarationFound declarationFound = messageData.Get<TypeDeclarationFound>();
            switch (declarationFound.Declaration[1].RuleName)
            {
                case "struct_decl":
                    StructureFound.Decorate(declarationFound);
                    break;
                case "class_decl":
                    ClassFound.Decorate(declarationFound);
                    break;
                case "enum_decl":
                    EnumFound.Decorate(declarationFound);
                    break;
                default:
                    //do nothing
                    break;
            }

            return InterceptionAction.Continue;
        }
    }
}
