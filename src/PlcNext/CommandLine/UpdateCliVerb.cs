#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using PlcNext.Common.Commands;

namespace PlcNext.CommandLine
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    [Verb("cli", HelpText = "Updates the CLI.", Hidden = true)]
    internal class UpdateCliVerb : UpdateVerb
    {
        [Option('v', "version", HelpText = "Specific version to update to.", SetName = "online")]
        public string Version { get; set; }

        [Option('p', "proxy", HelpText = "Http proxy. Overrides the proxy from the settings.", SetName = "online")]
        public string Proxy { get; set; }

        [Option('f', "file", HelpText = "The extractable file to update the version from.", SetName = "file")]
        public string File { get; set; }

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new UpdateCliCommandArgs(Version, Proxy, File)))
                                       .ConfigureAwait(false);
        }
    }
}
