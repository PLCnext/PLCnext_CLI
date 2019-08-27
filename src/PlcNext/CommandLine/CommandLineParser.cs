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
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json.Linq;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.UI;

namespace PlcNext.CommandLine
{
    internal class CommandLineParser : ICommandLineParser
    {
        private readonly IUserInterface userInterface;
        private readonly ICommandManager commandManager;
        private readonly IDynamicVerbFactory dynamicVerbFactory;
        private readonly ExecutionContext executionContext;
        private readonly ILifetimeScope lifetimeScope;

        private const int DisplayWidth = 80;

        public CommandLineParser(IUserInterface userInterface,
                                 ICommandManager commandManager, IDynamicVerbFactory dynamicVerbFactory, ExecutionContext executionContext, ILifetimeScope lifetimeScope)
        {
            this.userInterface = userInterface;
            this.commandManager = commandManager;
            this.dynamicVerbFactory = dynamicVerbFactory;
            this.executionContext = executionContext;
            this.lifetimeScope = lifetimeScope;
        }

        public Task<int> Parse(params string[] args)
        {
            ParserResult<object> result = new Parser(settings =>
            {
                settings.HelpWriter = null;
                settings.EnableDashDash = true;
            }).ParseVerbs(
                args, dynamicVerbFactory, typeof(BuildVerb), typeof(GenerateVerb), typeof(GetVerb),
                typeof(SetVerb), typeof(UpdateVerb), typeof(InstallVerb),
                typeof(ScanSdksVerb), typeof(StartServerVerb), typeof(StartClientVerb),
                typeof(ShowLogVerb), typeof(MigrateCliVerb));
            return result
                  .MapResult((object verb) => Execute(verb),
                             (errors) => OnError(result));
        }

        public string GetParseResult(params string[] args)
        {
            ParserResult<object> result = new Parser(settings =>
            {
                settings.HelpWriter = null;
                settings.EnableDashDash = true;
            }).ParseVerbs(
                args, dynamicVerbFactory, typeof(BuildVerb), typeof(GenerateVerb), typeof(GetVerb),
                typeof(SetVerb), typeof(UpdateVerb), typeof(InstallVerb),
                typeof(ScanSdksVerb), typeof(StartServerVerb), typeof(StartClientVerb),
                typeof(ShowLogVerb), typeof(MigrateCliVerb));
            return result
               .MapResult((object verb) => ConvertToString(verb),
                          (errors) => ConvertError(result));
        }

        private string ConvertError(ParserResult<object> result)
        {
            return new JObject(new JProperty("error",GetHelpText())).ToString();

            string GetHelpText()
            {
                IEnumerable<Error> errors = ((NotParsed<object>)result).Errors.ToArray();
                if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
                {
                    return new HelpText(CreateHeadingInfo())
                    {
                        MaximumDisplayWidth = DisplayWidth
                    }.AddPreOptionsLine(Environment.NewLine).ToString();
                }

                ParserTypeInfo typeInfo = result.GetTypeInfo();
                if (typeInfo == null)
                {
                    return "Could not construct help because command was not parsed as expected.";
                }

                bool helpRequested = errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError) ||
                                     errors.Any(e => e.Tag == ErrorType.HelpRequestedError);

                string help = ConstructHelp(result, typeInfo);

                return help;
            }
        }

        private string ConvertToString(object verb)
        {
            string command = GetCommand();
            Dictionary<string, string> options = GetOptions();
            IEnumerable<string> values = GetValues();

            return new JObject(new JProperty("command", command),
                               new JProperty("arguments",
                                             new JObject(options.Select(kv => new JProperty(kv.Key, kv.Value))
                                                                .Concat(values.Select((value, i) => new JProperty(i.ToString("D"), 
                                                                                                                  value)))
                                                                .Cast<object>()
                                                                .ToArray())))
               .ToString();

            string GetCommand()
            {
                List<string> parts = new List<string>();
                Type current = verb.GetType();
                VerbAttribute attribute;
                while ((attribute = current?.GetCustomAttribute<VerbAttribute>()) != null)
                {
                    parts.Add(attribute.Name);
                    current = current.BaseType;
                }

                parts.Reverse();
                return string.Join(" ", parts);
            }

            IEnumerable<string> GetValues()
            {
                return verb.GetType().GetProperties()
                           .Where(p => p.GetCustomAttribute<ValueAttribute>()?.Hidden == false)
                           .OrderBy(p => p.GetCustomAttribute<ValueAttribute>().Index)
                           .Select(p => ConvertValue(p.GetValue(verb),p.GetCustomAttribute<ValueAttribute>().Default));
            }

            Dictionary<string, string> GetOptions()
            {
                return verb.GetType().GetProperties()
                           .Where(p => p.GetCustomAttribute<OptionAttribute>()?.Hidden == false)
                           .ToDictionary(p => p.GetCustomAttribute<OptionAttribute>().LongName,
                                         p => ConvertValue(p.GetValue(verb),p.GetCustomAttribute<OptionAttribute>().Default));
            }

            string ConvertValue(object value, object fallback)
            {
                value = value ?? fallback;
                if (value is IEnumerable enumerable && !(value is string))
                {
                    return string.Join(",", enumerable.Cast<object>().ToArray());
                }

                return value?.ToString() ?? string.Empty;
            }
        }

        private Task<int> OnError(ParserResult<object> result)
        {
            IEnumerable<Error> errors = ((NotParsed<object>) result).Errors.ToArray();
            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                userInterface.WriteInformation(new HelpText(CreateHeadingInfo())
                {
                    MaximumDisplayWidth = DisplayWidth
                }.AddPreOptionsLine(Environment.NewLine));
                return Task.FromResult(0);
            }

            ParserTypeInfo typeInfo = result.GetTypeInfo();
            if (typeInfo == null)
            {
                userInterface.WriteError(
                    "Could not construct help because command was not parsed as expected.");
                return Task.FromResult(1);
            }

            bool helpRequested = errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError) || 
                                 errors.Any(e => e.Tag == ErrorType.HelpRequestedError);

            string help = ConstructHelp(result, typeInfo);

            userInterface.WriteInformation(help);

            return Task.FromResult(helpRequested ? 0 : 1);
        }

        private string ConstructHelp(ParserResult<object> result, ParserTypeInfo typeInfo)
        {
            IEnumerable<Error> errors = ((NotParsed<object>)result).Errors.ToArray();

            CopyrightInfo CreateCopyrightInfo()
            {
                try
                {
                    return CopyrightInfo.Default;
                }
                catch (Exception e)
                {
                    executionContext.WriteError($"Exception while creating the copyright:{Environment.NewLine}{e}", false);
                }

                return CopyrightInfo.Empty;
            }

            bool helpVerbRequested = errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError);

            HelpText helpText = new HelpText
            {
                Heading = CreateHeadingInfo(),
                Copyright = CreateCopyrightInfo(),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = DisplayWidth
            };

            helpText = HelpText.DefaultParsingErrorsHandler(result, helpText);

            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            string commandName = "plcnext";
            if (assembly != null)
            {
                AssemblyLicenseAttribute licenseAttribute = assembly.GetCustomAttribute<AssemblyLicenseAttribute>();
                licenseAttribute?.AddToHelpText(helpText);

                commandName = assembly.GetName().Name.ToLowerInvariant();
            }

            PropertyInfo property = typeInfo.Current.GetProperties(BindingFlags.Public | BindingFlags.Static)
                                            .FirstOrDefault(p => p.GetCustomAttribute<UsageAttribute>() != null &&
                                                                 p.PropertyType ==
                                                                 typeof(IEnumerable<UsageExample>));
            IEnumerable<UsageExample> examples =
                ((IEnumerable<UsageExample>) property?.GetValue(null) ?? Enumerable.Empty<UsageExample>())
               .ToArray();

            if (examples.Any())
            {
                helpText.AddPostOptionsLine("Examples:");
                helpText.AddPostOptionsLine(string.Empty);
            }

            foreach (UsageExample usageExample in examples)
            {
                helpText.AddPostOptionsLine(usageExample.HelpText.EndsWith(":")
                                                ? usageExample.HelpText
                                                : $"{usageExample.HelpText}:");
                helpText.AddPostOptionsLine($@"{commandName} {usageExample.Command}");
                helpText.AddPostOptionsLine(string.Empty);
            }

            bool removeVersion = false;
            if ((helpVerbRequested && typeInfo.Choices.Any())
                || errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError)
                || errors.Any(e => e.Tag == ErrorType.BadVerbSelectedError))
            {
                helpText.AddDashesToOption = false;
                helpText.AddVerbs(typeInfo.Choices.ToArray());
                //direct base type == VerbBase means first level - show version option
                removeVersion = typeInfo.Choices.Any() &&
                                typeInfo.Choices.First().BaseType != typeof(VerbBase);
            }
            else
            {
                helpText.AddOptions(result);
                removeVersion = true;
            }

            List<string> helpLines = new List<string>(helpText.ToString()
                                                              .Split(new[] {Environment.NewLine},
                                                                     StringSplitOptions.None));
            if (removeVersion)
            {
                string versionLine = helpLines.FirstOrDefault(l => l.Trim().StartsWith("version") ||
                                                                   l.Trim().StartsWith("--version"));
                if (!string.IsNullOrEmpty(versionLine))
                {
                    int index = helpLines.IndexOf(versionLine);
                    do
                    {
                        helpLines.RemoveAt(index - 1);
                    } while (!string.IsNullOrEmpty(helpLines[index - 1]));
                }
            }

            string help = string.Join(Environment.NewLine, helpLines);
            return help;
        }

        private HeadingInfo CreateHeadingInfo()
        {
            try
            {
                Assembly executingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                string name = executingAssembly.GetName().Name;
                string version = executingAssembly.GetName().Version.ToString();
                AssemblyTitleAttribute title = executingAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
                if (title != null)
                {
                    name = title.Title;
                }

                AssemblyInformationalVersionAttribute informationalVersion = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (informationalVersion != null)
                {
                    version = $"{informationalVersion.InformationalVersion} ({version})";
                }

                return new HeadingInfo(name, version);
            }
            catch (Exception e)
            {
                executionContext.WriteError($"Exception while creating the heading:{Environment.NewLine}{e}", false);
            }

            return HeadingInfo.Empty;
        }

        private async Task<int> Execute(object verb)
        {
            VerbBase verbBase = (VerbBase) verb;
            verbBase.CommandManager = commandManager;
            verbBase.UserInterface = userInterface;
            verbBase.LifetimeScope = lifetimeScope;
            if (verbBase is DynamicVerb dynamicVerb)
            {
                dynamicVerb.Defintion = dynamicVerbFactory.GetCommandDefintionForVerb(dynamicVerb.GetType());
            }
            return await verbBase.Execute();
        }
    }
}
