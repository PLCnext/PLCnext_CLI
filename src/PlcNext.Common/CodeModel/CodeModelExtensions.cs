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
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.CodeModel
{
    internal static class CodeModelExtensions
    {
        public static void CompleteCodeExceptions(this Exception exception, VirtualDirectory rootDirectory)
        {
            switch (exception)
            {
                case AggregateException aggregateException:
                    foreach (CodeSpecificException codeSpecificException in aggregateException.InnerExceptions.OfType<CodeSpecificException>())
                    {
                        codeSpecificException.ProjectRoot = rootDirectory;
                    }

                    break;
                case CodeSpecificException codeSpecificException:
                    codeSpecificException.ProjectRoot = rootDirectory;
                    break;
            }
        }

        public static void AddCodeException(this Entity entity, CodeSpecificException exception)
        {
            List<CodeSpecificException> exceptions = entity.Root.Value<List<CodeSpecificException>>();
            if (exceptions == null)
            {
                exceptions = new List<CodeSpecificException>();
                entity.Root.AddValue(exceptions);
            }
            exceptions.Add(exception);
        }

        public static Exception GetCodeExceptions(this Entity entity)
        {
            List<CodeSpecificException> exceptions = entity.Root.Value<List<CodeSpecificException>>();
            if (exceptions == null)
            {
                return null;
            }

            if (exceptions.Count == 1)
            {
                return exceptions[0];
            }
            return new AggregateException(exceptions);
        }

        public static bool HasAttributeWithoutValue(this IType type, string name)
        {
            return type.Attributes.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                            !a.Values.Any() &&
                                            !a.NamedValues.Any());
        }

        public static bool IsHidden(this CodeEntity codeEntity)
        {
            return codeEntity.Attributes.Any(a => a.Equals("hidden", StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasAttributeWithoutValue(this IField field, string name)
        {
            return field.Attributes.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                            !a.Values.Any() &&
                                            !a.NamedValues.Any());
        }

        public static string RootNamespaceForOldTarget(this ICodeModel model, string[] relevantTypes,
                                                       string[] additionalRelevantTypes)
        {
            if (!RootNamespaceWithoutException(model, relevantTypes.Concat(additionalRelevantTypes).ToArray(), false, out IEnumerable<string> namespaces, out string result))
            {
                return string.Empty;
            }
            string futureTargetResult = model.RootNamespace(relevantTypes);

            if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(futureTargetResult))
            {
                throw new MultipleRootNamespacesForTargetException(namespaces);
            }

            if(string.IsNullOrEmpty(result))
            {
                throw new MultipleRootNamespacesException(namespaces);
            }
            return result;
        }

        public static string RootNamespace(this ICodeModel model, string[] relevantTypes)
        {
            if (!RootNamespaceWithoutException(model, relevantTypes, true, out IEnumerable<string> namespaces, out string result))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(result))
            {
                throw new MultipleRootNamespacesException(namespaces);
            }
            return result;
        }

        private static bool RootNamespaceWithoutException(ICodeModel model, string[] relevantTypes, bool filterRelevantTypesWithSourceDirectory, out IEnumerable<string> namespaces,
                                                          out string result)
        {
            string[] sourceDirectories = model.SourceDirectories.Select(d => d.FullName).ToArray();
            namespaces = model.Types.Where(c => !filterRelevantTypesWithSourceDirectory ||
                                                sourceDirectories.Any(c.Value.FullName.StartsWith))
                              .Where(c => relevantTypes.Contains(c.Key.FullName))
                              .Select(p => p.Key)
                              .Where(m => m.Namespace != null)
                              .Select(m => m.Namespace)
                              .ToArray();
            if (!namespaces.Any())
            {
                namespaces = model.Types.Where(c => sourceDirectories.Any(c.Value.FullName.StartsWith))
                                  .Select(p => p.Key)
                                  .Where(m => m.Namespace != null)
                                  .Select(m => m.Namespace)
                                  .ToArray();
            }

            if (!namespaces.Any())
            {
                result = string.Empty;
                return false;
            }

            result = string.Join(".", namespaces
                                     .Select(s => s.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries)
                                                   .AsEnumerable())
                                     .Transpose()
                                     .TakeWhile(s => s.All(x => x == s.First())).Select(s => s.First()));
            return true;
        }
    }
}
