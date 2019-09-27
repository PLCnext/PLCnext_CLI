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
using System.Linq.Expressions;
using System.Text;
using PlcNext.Common.Tools;

namespace PlcNext.Common.DataModel
{
    public class Entity : IEntityBase, IDecoratableEntity
    {
        public Entity Empty => new Entity("Empty", contentProvider, Enumerable.Empty<object>());

        private readonly IEntityContentProvider contentProvider;
        private Entity context;
        private readonly List<object> values;
        private readonly IEnumerable<Entity> collection;
        private readonly Dictionary<Type, List<Func<object>>> lazyValues;
        private readonly Dictionary<string, Entity> cache = new Dictionary<string, Entity>();
        private readonly Dictionary<MetaDataKey, object> metaData = new Dictionary<MetaDataKey, object>();
        private readonly List<string> noCacheKeys = new List<string>();

        public Entity(string type, IEntityContentProvider contentProvider,
                      IEnumerable<object> values, IEnumerable<Entity> collection = null, 
                      Entity owner = null)
        {
            (object dataSource, bool isLazy, Func<object> lazyFunc, Type dsType)[] sorted = values
                                                   .Where(o => o != null)
                                                   .Select(ds => (ds, TryConvertToFunc(ds, out Func<object> creation, out Type dsType),
                                                                  creation, dsType))
                                                   .ToArray();
            this.collection = collection ?? new[] {this};
            this.contentProvider = contentProvider;
            this.values = sorted.Where(s => !s.isLazy)
                                     .Select(s => s.dataSource)
                                     .ToList();
            lazyValues = sorted.Where(s => s.isLazy)
                                   .GroupBy(s => s.dsType)
                                   .ToDictionary(g => g.Key, g => g.Select(s => s.lazyFunc).ToList());
            Type = type;
            Owner = owner;
        }

        #region Extension Properties

        public string Name => this[EntityKeys.NameKey].Value<string>();

        public string Path => this[EntityKeys.PathKey].Value<string>();

        public Entity Root => this[EntityKeys.RootKey];

        public Entity Origin
        {
            get
            {
                Entity current = this;
                while (current.Owner != null)
                {
                    current = current.Owner;
                }

                return current;
            }
        }

        public Entity Base => this;

        public bool HasName => HasContent(EntityKeys.NameKey);

        public bool HasPath => HasContent(EntityKeys.PathKey);

        #endregion

        #region Properties
        public string Type { get; }

        public Entity Owner { get; }

        public Entity this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                key = key.ToLowerInvariant();
                if (!cache.ContainsKey(key))
                {
                    if (context != null && !contentProvider.CanResolve(this, key))
                    {
                        return context[key];
                    }

                    if (noCacheKeys.Contains(key))
                    {
                        return contentProvider.Resolve(this, key);
                    }

                    Entity result = contentProvider.Resolve(this, key);
                    cache[key] = result;
                }
                return cache[key];
            }
        }

        public Entity this[int index] => collection.ElementAt(index);

        public IEnumerable<Entity> CachedContent => cache.Values;

        #endregion

        #region MetaData

        public T MetaData<T>(string key = "")
        {
            MetaDataKey metaDataKey = new MetaDataKey(typeof(T), key);
            if (metaData.ContainsKey(metaDataKey))
            {
                return (T) metaData[metaDataKey];
            }

            return default(T);
        }

        public void SetMetaData<T>(T value, string key = "")
        {
            MetaDataKey metaDataKey = new MetaDataKey(typeof(T), key);
            metaData[metaDataKey] = value;
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<Entity> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => collection.Count();

        #endregion

        #region Methods

        public bool HasContent(string key)
        {
            return cache.ContainsKey(key) ||
                   contentProvider.CanResolve(this, key) || 
                   context?.HasContent(key) == true;
        }

        public T Value<T>()
        {
            ResolveLazyValue<T>(false);
            T result = values.OfType<T>().FirstOrDefault();
            if (typeof(T) == typeof(string) && result == null)
            {
                result = (T)(object)string.Empty;
            }
            return result;
        }

        private void ResolveLazyValue<T>(bool resolveAll)
        {
            if (!resolveAll && values.OfType<T>().Any())
            {
                return;
            }
            foreach (Type key in lazyValues.Keys.ToArray())
            {
                if (typeof(T).IsAssignableFrom(key))
                {
                    foreach (Func<object> resolver in lazyValues[typeof(T)].ToArray())
                    {
                        values.Add(resolver());
                        lazyValues[typeof(T)].Remove(resolver);
                        if (!resolveAll)
                        {
                            return;
                        }
                    }

                    lazyValues.Remove(typeof(T));
                }
            }
        }

        public IEnumerable<T> Values<T>()
        {
            ResolveLazyValue<T>(true);
            return values.OfType<T>();
        }

        public IEnumerable<object> Values()
        {
            ResolveLazyValue<object>(true);
            return values;
        }

        public void AddValue<T>(Func<T> createDataSource)
        {
            if (!lazyValues.ContainsKey(typeof(T)))
            {
                lazyValues.Add(typeof(T), new List<Func<object>>());
            }
            lazyValues[typeof(T)].Add(() => createDataSource());
        }

        public void AddValue<T>(T dataSource)
        {
            values.Add(dataSource);
        }

        public void SetValue<T>(Func<T> createDataSource)
        {
            RemoveValue<T>();
            AddValue(createDataSource);
        }

        public void SetValue<T>(T dataSource)
        {
            RemoveValue<T>();
            AddValue(dataSource);
        }

        public void RemoveValue<T>()
        {
            values.RemoveAll(o => o is T);
            lazyValues.Remove(typeof(T));
        }

        public IDisposable AddTemporaryDataSource<T>(T dataSource)
        {
            values.Add(dataSource);
            return new DisposeAction(() => values.Remove(dataSource));
        }

        public void AddEntity(Entity entity, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToByteString();
            }
            cache[key] = entity;
        }

        public IDisposable SkipCaching(string key)
        {
            noCacheKeys.Add(key);
            return new DisposeAction(() => noCacheKeys.Remove(key));
        }

        public void SetContext(Entity contextEntity)
        {
            context = contextEntity;
        }

        public Entity Create(string type, params object[] entityValues)
        {
            return new Entity(type, contentProvider, entityValues, null, this);
        }

        public Entity Create(string type, IEnumerable<Entity> entityCollection, params object[] entityValues)
        {
            entityCollection = entityCollection.ToArray();
            if (!entityValues.Any(ds => ds is string || ds is Func<string>))
            {
                entityValues = entityValues.Concat(new[] {entityCollection.ToFunc()}).ToArray();
            }
            return new Entity(type, contentProvider, entityValues, entityCollection, this);
        }

        #endregion

        #region ToString

        public string ToShortString()
        {
            string result = Type;
            if (HasValue<string>())
            {
                result += $":{Value<string>()}";
            }

            return result;
        }

        public override string ToString()
        {
            List<string> path = new List<string>();
            Entity current = Owner;
            while (current != null)
            {
                path.Add(current.ToShortString());
                current = current.Owner;
            }

            string result = $"{nameof(Type)}: {Type}";
            if (HasValue<string>())
            {
                result += $"; {nameof(Value)}: {Value<string>()}";
            }
            if (path.Any())
            {
                path.Reverse();
                result += $"; Path: {string.Join("/", path)}";
            }
            return result;
        }

        #endregion

        #region Helper

        private bool TryConvertToFunc(object obj, out Func<object> creationFunction, out Type functionReturnType)
        {
            creationFunction = null;
            functionReturnType = null;
            Type type = obj.GetType();
            Type generic = null;
            if (type.IsGenericTypeDefinition) generic = type;
            else if (type.IsGenericType) generic = type.GetGenericTypeDefinition();
            if (generic != null &&
                generic == typeof(Func<>) && 
                !type.GenericTypeArguments[0].IsValueType)
            {
                creationFunction = (Func<object>) obj;
                functionReturnType = type.GenericTypeArguments[0];
                return true;
            }

            return false;
        }

        #endregion

        #region Decoratable entity
        
        public IDecoratableEntity DecoratedEntity => null;
        public IDecoratableEntity DecoratedByEntity { get; set; }

        #endregion

        private class MetaDataKey : IEquatable<MetaDataKey>
        {
            public MetaDataKey(Type type, string key)
            {
                Type = type;
                Key = key;
            }

            public bool Equals(MetaDataKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Type == other.Type && string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((MetaDataKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                }
            }

            public static bool operator ==(MetaDataKey left, MetaDataKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(MetaDataKey left, MetaDataKey right)
            {
                return !Equals(left, right);
            }

            private Type Type { get; }
            private string Key { get; }
        }

        public bool HasValue<T>()
        {
            return lazyValues.Keys.Any(key => typeof(T).IsAssignableFrom(key)) ||
                   values.OfType<T>().Any();
        }
    }

}
