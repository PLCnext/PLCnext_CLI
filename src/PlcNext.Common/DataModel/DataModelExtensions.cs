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
using System.Text;
using PlcNext.Common.Tools;

namespace PlcNext.Common.DataModel
{
    internal static class DataModelExtensions
    {
        public static IEnumerable<Entity> EntityHierarchy(this Entity start)
        {
            List<Entity> result = new List<Entity>();
            Stack<Entity> unvisited = new Stack<Entity>();
            unvisited.Push(start);
            while (unvisited.Any())
            {
                Entity current = unvisited.Pop();
                result.Add(current);
                if (current.Owner != null && !result.Contains(current.Owner))
                {
                    unvisited.Push(current.Owner);
                }

                foreach (Entity entity in current.CachedContent.Where(e => !result.Contains(e)))
                {
                    unvisited.Push(entity);
                }
            }

            return result;
        }

        public static Func<string> ToFunc(this IEnumerable<Entity> collection)
        {
            if (collection.Any(c => c.HasValue<string>()))
            {
                return () => string.Join(",", collection.Select(e => e.Value<string>() ?? string.Empty));
            }

            return () => string.Empty;
        }

        public static bool HasPropertyValueEntity(this object valueProvider, string key)
        {
            PropertyInfo property = valueProvider.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() ==
                                                                                                key);
            return property != null;
        }

        public static Entity PropertyValueEntity(this Entity owner, string key, object valueProvider, string entityType = null)
        {
            if (string.IsNullOrEmpty(entityType))
            {
                entityType = key;
            }
            PropertyInfo property = valueProvider.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() ==
                                                                                           key);
            if (property != null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                    property.PropertyType != typeof(string))
                {
                    IEnumerable array = (IEnumerable)property.GetValue(valueProvider);
                    return owner.Create(entityType, array.OfType<object>()
                                                         .Select(v => owner.Create(entityType.Singular(),
                                                                                   v.ToString(),
                                                                                   v)));
                }

                return owner.Create(entityType, property.GetValue(valueProvider).ToString());
            }

            return null;
        }

        public static T PropertyValue<T>(this object valueProvider, string key)
        {
            PropertyInfo property = valueProvider.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() ==
                                                                                                key);
            if (property != null && typeof(T).IsAssignableFrom(property.PropertyType))
            {
                return (T) property.GetValue(valueProvider);
            }

            return default(T);
        }

        public static bool HasPropertyValue(this object valueProvider, string key, Type propertyType)
        {
            PropertyInfo property = valueProvider.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() ==
                                                                                                key);
            return property != null && propertyType.IsAssignableFrom(property.PropertyType);
        }

        public static bool IsRoot(this IEntityBase owner)
        {
            return owner.MetaData<bool>(EntityKeys.IsRoot);
        }
    }
}
