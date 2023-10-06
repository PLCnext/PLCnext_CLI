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
using System.Text.RegularExpressions;
using System.Threading;

namespace PlcNext.Common.Tools
{
    public static class Extensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string ToPropertyName(this string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return name.Replace("-", "", StringComparison.Ordinal)
                       .Replace("_", "", StringComparison.Ordinal)
                       .ToLowerInvariant();
        }

        public static string ResolvePathName(this string path, IEnvironmentPathNames pathNames)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (pathNames == null)
            {
                throw new ArgumentNullException(nameof(pathNames));
            }

            string resolved = path;
            Regex resolvePattern = new Regex(@"{(?<resolvable>\w+)}");
            Match resolveMatch = resolvePattern.Match(path);
            while (resolveMatch.Success)
            {
                string key = resolveMatch.Groups["resolvable"].Value;
                if (pathNames.ContainsKey(key))
                {
                    resolved = resolved.Replace(resolveMatch.Value, pathNames[key], StringComparison.Ordinal);
                }

                resolveMatch = resolvePattern.Match(resolved);
            }

            return resolved;
        }

        public static T ExecutesWithTimeout<T>(this Func<T> function, int timeout)
        {
            T result = default(T);
            Thread actionThread = new Thread(() => result = function());
            actionThread.Start();
            bool finished = actionThread.Join(timeout);
            if (!finished)
            {
                throw new OperationCanceledException();
            }

            return result;
        }
        
        public static void ExecutesWithTimeout(this Action action, int timeout)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Thread actionThread = new Thread(action.Invoke);
            actionThread.Start();
            bool finished = actionThread.Join(timeout);
            if (!finished)
            {
                throw new OperationCanceledException();
            }
        }
        
        public static string ToByteString(this Guid id)
        {
            byte[] bytes = id.ToByteArray();
            return $"{bytes[0]:X2}{bytes[1]:X2}{bytes[2]:X2}{bytes[3]:X2}-{bytes[4]:X2}{bytes[5]:X2}-" +
                   $"{bytes[6]:X2}{bytes[7]:X2}-{bytes[8]:X2}{bytes[9]:X2}-" +
                   $"{bytes[10]:X2}{bytes[11]:X2}{bytes[12]:X2}{bytes[13]:X2}{bytes[14]:X2}{bytes[15]:X2}";
        }
        
        public static void ThrowIfNotEmpty(this IEnumerable<Exception> exceptions)
        {
            Exception[] frozenExceptions = exceptions.ToArray();
            if (frozenExceptions.Length == 1)
            {
                throw frozenExceptions[0];
            }

            if (frozenExceptions.Length > 1)
            {
                throw new AggregateException(frozenExceptions);
            }
        }

        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> original)
        {
            IEnumerator<T>[] enumerators = original.Select(e => e.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(e => e.MoveNext()))
                {
                    yield return enumerators.Select(e => e.Current).ToArray();
                }
            }
            finally
            {
                Array.ForEach(enumerators, e => e.Dispose());
            }
        }
    }
}
