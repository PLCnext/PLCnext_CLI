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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using PlcNext.Common.CommandLine;

// Include this file in your project.

namespace PlcNext.CommandLine
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ChildVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public ChildVerbsAttribute(params Type[] types)
        {
            Types = types;
        }
    }

    /// <summary>
    /// Extension methods to allow multi level verb parsing.
    /// </summary>
    internal static class ParserVerbExtensions
    {
        public static ParserResult<object> ParseVerbs(this Parser parser, IEnumerable<string> args, IDynamicVerbFactory dynamicVerbFactory, params Type[] types)
        {
            string[] argsArray = ModifyArgs();
            return ParseVerbs(parser, argsArray, dynamicVerbFactory, types.Concat(dynamicVerbFactory.GetDynamicVerbs(Array.Empty<string>())).ToArray(), new List<string>());

            string[] ModifyArgs()
            {
                string[] result = args.ToArray();
                if (result.Length > 1)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i] == "--version")
                        {
                            result[i] = "-v";
                        }
                    }
                }

                return result;
            }
        }

        private static ParserResult<object> ParseVerbs(Parser parser, string[] argsArray, IDynamicVerbFactory dynamicVerbFactory, Type[] types, List<string> path)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            
            if (argsArray.Length == 0 || argsArray[0].StartsWith("-", StringComparison.Ordinal))
            {
                return parser.ParseArguments(argsArray, types);
            }
            
            var verb = argsArray[0];
            foreach (var type in types)
            {
                var verbAttribute = type.GetCustomAttribute<VerbAttribute>();
                if (verbAttribute == null || verbAttribute.Name != verb)
                {
                    continue;
                }
                
                path.Add(verb);
                var subVerbsAttribute = type.GetCustomAttribute<ChildVerbsAttribute>();
                Type[] subTypes = subVerbsAttribute?.Types?? Array.Empty<Type>();
                subTypes = subTypes.Concat(dynamicVerbFactory.GetDynamicVerbs(path)).ToArray();

                if (subTypes.Any() && type.GetCustomAttribute<UseChildVerbsAsCategoryAttribute>() == null)
                {
                    return ParseVerbs(parser, argsArray.Skip(1).ToArray(), dynamicVerbFactory, subTypes, path);
                }

                break;
            }

            return parser.ParseArguments(argsArray, types);
        }
    }

    public class ParserTypeInfo
    {
        internal ParserTypeInfo(Type current, IEnumerable<Type> choices)
        {
            Current = current;
            Choices = choices;
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        public Type Current { get; }

        public IEnumerable<Type> Choices { get; }
    }

    public static class TypeInfoExtensions
    {
        public static ParserTypeInfo GetTypeInfo<T>(this ParserResult<T> result)
        {
            ParserTypeInfo info = null;
            object infoObject = typeof(ParserResult<T>).GetProperty("TypeInfo", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(result);
            if (infoObject != null)
            {
                Type current = infoObject.GetType().GetProperty("Current", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(infoObject) as Type;
                IEnumerable<Type> choices = infoObject.GetType().GetProperty("Choices", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(infoObject) as IEnumerable<Type>;

                if (current != null && choices != null)
                {
                    info = new ParserTypeInfo(current, choices);
                }
            }
            return info;
        }
    }

    internal static class AttributeExtensions
    {
        public static void AddToHelpText(this MultilineTextAttribute textAttribute, HelpText helpText)
        {
            string[] strArray = new[] { textAttribute.Line1, textAttribute.Line2, textAttribute.Line3, textAttribute.Line4, textAttribute.Line5 };
            strArray.Take(GetLastLineWithText(strArray)).Aggregate(helpText, (current, line) => helpText.AddPreOptionsLine(line));
        }

        private static int GetLastLineWithText(string[] value)
        {
            var index = Array.FindLastIndex(value, str => !string.IsNullOrEmpty(str));

            // remember FindLastIndex returns zero-based index
            return index + 1;
        }

        public static TotalAccessType ToTotalAccessType(this Type type)
        {
            return new TotalAccessType(type);
        }
    }
}
