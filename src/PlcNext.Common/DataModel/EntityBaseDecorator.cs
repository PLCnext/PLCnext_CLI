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
using System.Text;

namespace PlcNext.Common.DataModel
{
    public class EntityBaseDecorator : IEntityBase, IDecoratableEntity
    {
        private readonly IEntityBase entityBase;

        public EntityBaseDecorator(IEntityBase entityBase)
        {
            if (entityBase is IDecoratableEntity decoratable)
            {
                while (decoratable.DecoratedByEntity != null)
                {
                    decoratable = decoratable.DecoratedByEntity;
                }

                entityBase = decoratable as IEntityBase ?? entityBase;
                decoratable = (IDecoratableEntity) entityBase;
                DecoratedEntity = decoratable;
                DecoratedEntity.DecoratedByEntity = this;
            }
            this.entityBase = entityBase;
        }

        protected static T Decorate<T>(IEntityBase baseEntity)
            where T : class, IDecoratableEntity, IEntityBase
        {
            if (baseEntity is IDecoratableEntity decoratableEntity)
            {
                return CheckBase(decoratableEntity) ?? CheckDecorators(decoratableEntity);
            }

            return null;

            T CheckBase(IDecoratableEntity decoratable)
            {
                while (decoratable != null)
                {
                    if (decoratable is T result)
                    {
                        return result;
                    }
                    decoratable = decoratable.DecoratedEntity;
                }

                return null;
            }

            T CheckDecorators(IDecoratableEntity decoratable)
            {
                while (decoratable != null)
                {
                    if (decoratable is T result)
                    {
                        return result;
                    }
                    decoratable = decoratable.DecoratedByEntity;
                }

                return null;
            }
        }

        public virtual IEnumerator<Entity> GetEnumerator()
        {
            return entityBase.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) entityBase).GetEnumerator();
        }

        public virtual int Count => entityBase.Count;

        public virtual Entity Empty => entityBase.Empty;

        public virtual string Name => entityBase.Name;

        public virtual string Path => entityBase.Path;

        public virtual string Type => entityBase.Type;

        public virtual Entity Owner => entityBase.Owner;

        public virtual IEnumerable<Entity> CachedContent => entityBase.CachedContent;
        public Entity Root => entityBase.Root;

        public bool HasName => entityBase.HasName;

        public bool HasPath => entityBase.HasPath;
        public Entity Origin => entityBase.Origin;

        public virtual Entity this[string key] => entityBase[key];

        public virtual Entity this[int index] => entityBase[index];

        public virtual T MetaData<T>(string key = "")
        {
            return entityBase.MetaData<T>(key);
        }

        public virtual void SetMetaData<T>(T value, string key = "")
        {
            entityBase.SetMetaData(value, key);
        }

        public virtual bool HasContent(string key)
        {
            return entityBase.HasContent(key);
        }

        public virtual T Value<T>()
        {
            return entityBase.Value<T>();
        }

        public virtual IEnumerable<T> Values<T>()
        {
            return entityBase.Values<T>();
        }

        public virtual IEnumerable<object> Values()
        {
            return entityBase.Values();
        }

        public virtual void AddValue<T>(Func<T> createDataSource)
        {
            entityBase.AddValue(createDataSource);
        }

        public virtual void AddEntity(Entity entity, string key = null)
        {
            entityBase.AddEntity(entity, key);
        }

        public virtual void AddValue<T>(T dataSource)
        {
            entityBase.AddValue(dataSource);
        }

        public virtual IDisposable AddTemporaryDataSource<T>(T dataSource)
        {
            return entityBase.AddTemporaryDataSource(dataSource);
        }

        public virtual IDisposable SkipCaching(string key)
        {
            return entityBase.SkipCaching(key);
        }

        public virtual void SetContext(Entity contextEntity)
        {
            entityBase.SetContext(contextEntity);
        }

        public virtual Entity Create(string type, params object[] entityValues)
        {
            return entityBase.Create(type, entityValues);
        }

        public virtual Entity Create(string type, IEnumerable<Entity> entityCollection, params object[] entityValues)
        {
            return entityBase.Create(type, entityCollection, entityValues);
        }

        public virtual string ToShortString()
        {
            return entityBase.ToShortString();
        }

        public virtual bool HasValue<T>()
        {
            return entityBase.HasValue<T>();
        }

        public virtual void SetValue<T>(Func<T> createDataSource)
        {
            entityBase.SetValue(createDataSource);
        }

        public virtual void SetValue<T>(T dataSource)
        {
            entityBase.SetValue(dataSource);
        }

        public virtual void RemoveValue<T>()
        {
            entityBase.RemoveValue<T>();
        }

        public IDecoratableEntity DecoratedEntity { get; }
        public IDecoratableEntity DecoratedByEntity { get; set; }
    }
}
