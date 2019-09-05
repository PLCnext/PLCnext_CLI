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

namespace PlcNext.Common.DataModel
{
    public interface IEntityBase : IReadOnlyCollection<Entity>
    {
        Entity Empty { get; }
        string Name { get; }
        string Path { get; }
        string Type { get; }
        Entity Owner { get; }
        IEnumerable<Entity> CachedContent { get; }
        Entity Root { get; }
        bool HasName { get; }
        bool HasPath { get; }
        Entity Origin { get; }
        Entity Base { get; }
        Entity this[string key] { get; }
        Entity this[int index] { get; }
        T MetaData<T>(string key = "");
        void SetMetaData<T>(T value, string key = "");
        bool HasContent(string key);
        T Value<T>();
        IEnumerable<T> Values<T>();
        IEnumerable<object> Values();
        void AddValue<T>(Func<T> createDataSource);
        void AddEntity(Entity entity, string key = null);
        void AddValue<T>(T dataSource);
        IDisposable AddTemporaryDataSource<T>(T dataSource);
        IDisposable SkipCaching(string key);
        void SetContext(Entity contextEntity);
        Entity Create(string type, params object[] entityValues);
        Entity Create(string type, IEnumerable<Entity> entityCollection, params object[] entityValues);
        string ToShortString();
        string ToString();
        bool HasValue<T>();
        void SetValue<T>(Func<T> createDataSource);
        void SetValue<T>(T dataSource);
        void RemoveValue<T>();
    }
}