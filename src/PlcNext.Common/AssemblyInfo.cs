#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyTrademark("PHOENIX CONTACT GmbH & Co. KG")]

#if !PUBLISHBUILD
[assembly: InternalsVisibleTo("Test.PlcNext")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}