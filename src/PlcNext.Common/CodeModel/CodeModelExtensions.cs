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

        public static string RootNamespace(this ICodeModel model, IEnumerable<TemplateDescription> templates, bool allowNoCodeEntities = true)
        {
            string result = string.Empty;
            IEnumerable<string> namespaces = model.Classes.Keys.Where(m => m.Namespace != null)
                                                  .Where(type => templates.Where(t => t.isRelevantForProjectNamespace)
                                                                          .Select(t => t.name).Any(type.HasAttributeWithoutValue))
                                                  .Select(m => m.Namespace)
                                                  .ToArray();
            if (!namespaces.Any())
            {
                namespaces = model.Classes.Keys.Where(m => m.Namespace != null)
                                  .Where(type => templates.Select(t => t.name).Any(type.HasAttributeWithoutValue))
                                  .Select(m => m.Namespace)
                                  .ToArray();
            }
            if (!namespaces.Any())
            {
                if (allowNoCodeEntities)
                    return string.Empty;
                throw new FormattableException("No code entities found for the specified project.");
            }

            result = string.Join(".", namespaces
                                     .Select(s => s.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries)
                                                   .AsEnumerable())
                                     .Transpose()
                                     .TakeWhile(s => s.All(x => x == s.First())).Select(s => s.First()));
            if (string.IsNullOrEmpty(result))
            {
                throw new MultipleRootNamespacesException(namespaces);
            }
            return result;
        }
    }
}
