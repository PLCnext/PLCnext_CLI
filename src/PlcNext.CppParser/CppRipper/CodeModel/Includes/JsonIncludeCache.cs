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
using System.Text;
using Newtonsoft.Json;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.CppParser.CppRipper.CodeModel.Includes
{
    internal class JsonIncludeCache : IIncludeCache
    {
        private IDictionary<string, IncludeCacheEntry> cache;
        private IDictionary<string, IncludeCacheEntry> Cache => cache ?? throw new InvalidOperationException("Include cache not initialized.");

        private VirtualFile cacheFile;
        private VirtualFile CacheFile => cacheFile ?? throw new InvalidOperationException("Include cache not initialized.");

        private readonly ILog log;

        public JsonIncludeCache(ILog log)
        {
            this.log = log;
        }

        public void LoadCache(VirtualFile cacheFile)
        {
            try
            {
                Stopwatch loadWatch = new Stopwatch();
                loadWatch.Start();
                using (Stream fileStream = cacheFile.OpenRead())
                using (TextReader reader = new StreamReader(fileStream))
                using (JsonReader jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = CreateSerializer();
                    IncludeCacheEntry[] result = serializer.Deserialize<IncludeCacheEntry[]>(jsonReader);
                    cache = result?.ToDictionary(r => r.File, r => r)?? new Dictionary<string, IncludeCacheEntry>();
                    this.cacheFile = cacheFile;
                }
                loadWatch.Stop();
                log.LogInformation($"Successfully loaded the include cache in {loadWatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception e)
            {
                log.LogError($"Error while parsing include cache file, using empty cache.{Environment.NewLine}{e}");
                cache = new Dictionary<string, IncludeCacheEntry>();
                this.cacheFile = cacheFile;
            }
        }

        private static JsonSerializer CreateSerializer()
        {
            return new JsonSerializer {Formatting = Formatting.Indented};
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

        public IIncludeCacheTransaction StartTransaction()
        {
            return new IncludeCacheTransaction(this);
        }

        public IEnumerable<IncludeCacheEntry> Entries => Cache.Values;

        private void SaveCache()
        {
            try
            {
                using (Stream fileStream = CacheFile.OpenWrite())
                using (TextWriter writer = new StreamWriter(fileStream))
                using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                {
                    fileStream.SetLength(0);
                    JsonSerializer serializer = CreateSerializer();
                    serializer.Serialize(jsonWriter, Cache.Values.ToArray());
                }
                log.LogInformation($"Successfully saved the include cache.");
            }
            catch (Exception e)
            {
                log.LogError($"Error while saving include cache file, cache will not be saved.{Environment.NewLine}{e}");
            }
        }

        private class IncludeCacheTransaction : IIncludeCacheTransaction
        {
            private readonly JsonIncludeCache cache;

            public IncludeCacheTransaction(JsonIncludeCache cache)
            {
                this.cache = cache;
            }

            public void Dispose()
            {
                cache.SaveCache();
            }

            public void DeleteEntry(IncludeCacheEntry cacheEntry)
            {
                cache.log.LogInformation($"Removed the following entry from the include cache:{Environment.NewLine}" +
                                         $"{JsonConvert.SerializeObject(cacheEntry, Formatting.Indented)}");
                cache.Cache.Remove(cacheEntry.File);
            }

            public void AddEntry(IncludeCacheEntry cacheEntry)
            {
                cache.log.LogInformation($"Added the following entry from the include cache:{Environment.NewLine}" +
                                         $"{JsonConvert.SerializeObject(cacheEntry, Formatting.Indented)}");
                cache.Cache.Add(cacheEntry.File, cacheEntry);
            }
        }
    }
}
