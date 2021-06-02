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
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlcNext.Common.Tools.UI;
using Serilog;

namespace PlcNext.CppParser.IncludeManager
{
    internal class JsonIncludeCache : IIncludeCache
    {
        public static readonly Version CurrentVersion = new Version(1,1);
        private IDictionary<string, IncludeCacheEntry> cache;
        private IDictionary<string, IncludeCacheEntry> Cache => cache ?? throw new InvalidOperationException("Include cache not initialized.");
        private readonly object cacheLock = new object();

        private JsonIncludeCache()
        {
        }

        public static JsonIncludeCache Empty => new JsonIncludeCache {cache = new Dictionary<string, IncludeCacheEntry>()};

        public static IIncludeCache LoadCache(Stream readStream)
        {
            JsonIncludeCache cache = new JsonIncludeCache();
            cache.LoadFrom(readStream);
            return cache;
        }

        private void LoadFrom(Stream fileStream)
        {
            try
            {
                Stopwatch loadWatch = new Stopwatch();
                loadWatch.Start();
                using (TextReader reader = new StreamReader(fileStream))
                using (JsonReader jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = CreateSerializer();
                    IncludeCacheEntry[] result;
                    if (reader.Peek() == '[')
                    {
                        //old version
                        result = serializer.Deserialize<IncludeCacheEntry[]>(jsonReader);
                    }
                    else
                    {
                        IncludeCache deserializedCache = serializer.Deserialize<IncludeCache>(jsonReader);
                        result = deserializedCache.CacheEntries;
                        Version = deserializedCache.Version;
                    }
                    cache = result?.ToDictionary(r => r.File, r => r)?? new Dictionary<string, IncludeCacheEntry>();
                }
                loadWatch.Stop();
                Log.Information($"Successfully loaded the include cache in {loadWatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception e)
            {
                Log.Error($"Error while parsing include cache file, using empty cache.{Environment.NewLine}{e}", e);
                cache = new Dictionary<string, IncludeCacheEntry>();
            }
        }

        private static JsonSerializer CreateSerializer()
        {
            return new JsonSerializer
            {
                Formatting = Formatting.Indented,
                Converters = { new VersionConverter()}
            };
        }

        public bool TryGetCacheEntry(string pathToInclude, out IncludeCacheEntry cacheEntry)
        {
            return Cache.TryGetValue(pathToInclude, out cacheEntry);
        }

        public bool TryGetCacheEntryWithTypeName(string typeFullName, IReadOnlyCollection<string> includeDirectories,
                                                 out IncludeCacheEntry cacheEntry)
        {
            cacheEntry = Cache.Values.FirstOrDefault(v => v.Types.Contains(typeFullName) &&
                                                          includeDirectories.Contains(v.BaseDirectory));
            return cacheEntry != null;
        }

        public IEnumerable<IncludeCacheEntry> Entries => Cache.Values;
        public Version Version { get; private set; } = new Version(1, 0);

        public void SaveCacheTo(Stream saveStream)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(saveStream))
                using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                {
                    saveStream.SetLength(0);
                    JsonSerializer serializer = CreateSerializer();
                    serializer.Serialize(jsonWriter, new IncludeCache
                    {
                        CacheEntries = Cache.Values.ToArray(),
                        Version = CurrentVersion
                    });
                }
                Log.Information($"Successfully saved the include cache.");
            }
            catch (Exception e)
            {
                Log.Error($"Error while saving include cache file, cache will not be saved.{Environment.NewLine}{e}", e);
            }
        }

        public void DeleteEntries(IEnumerable<IncludeCacheEntry> deletableEntries)
        {
            lock (cacheLock)
            {
                foreach (IncludeCacheEntry deletableEntry in deletableEntries)
                {
                    Cache.Remove(deletableEntry.File);
                }
            }
        }

        public void AddEntries(IEnumerable<IncludeCacheEntry> addedEntries)
        {
            lock (cacheLock)
            {
                foreach (IncludeCacheEntry addedEntry in addedEntries)
                {
                    Cache.Add(addedEntry.File, addedEntry);
                }
            }
        }
    }
}
