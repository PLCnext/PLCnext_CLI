#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Resources;
using System.Runtime.CompilerServices;

[assembly: NeutralResourcesLanguage("en")]
#if !PUBLISHBUILD
[assembly: InternalsVisibleTo("Test.PlcNext")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}