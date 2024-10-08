﻿using Microsoft.Extensions.Caching.Memory;
using srtool.core;

namespace srtool
{
    public static class SuperCache
    {
        private static MemoryCache _cache;
        static SuperCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = null
            });
        }

        public static bool Exist(string key)
        {
            return _cache.TryGetValue(key, out CacheItem res);
        }
        public static async Task<T> Get<T>(Func<Task<T>> task, string key, TimeSpan span)
        {
            return await Get(task, new CacheSetting(key, span));
        }

        public static async Task<T> Get<T>(Func<Task<T>> task, CacheSetting settings)
        {
            if (!_cache.TryGetValue(settings.key, out CacheItem cacheEntry))// Look for cache key.
            {
                cacheEntry = new CacheItem();
                // Key not in cache, so get data.
                cacheEntry.Value = await task();
                _cache.Set(settings.key, cacheEntry, GetOption(settings));
            }

            return (T)cacheEntry.Value;
        }

        public static T Get<T>(Func<T> task, string key, TimeSpan span)
        {
            return Get(task, new CacheSetting(key, span));
        }
        public static T Get<T>(Func<T> task, CacheSetting settings)
        {
            if (!_cache.TryGetValue(settings.key, out CacheItem cacheEntry))// Look for cache key.
            {
                cacheEntry = new CacheItem();
                // Key not in cache, so get data.
                cacheEntry.Value = task();
                _cache.Set(settings.key, cacheEntry, GetOption(settings));
            }
            return (T)cacheEntry.Value;
        }
        public static T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out CacheItem cacheEntry))// Look for cache key.
                return (T)cacheEntry.Value;

            return default;
        }
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }
        public static void Set<T>(T value, CacheSetting settings)
        {
            _cache.Set(settings.key, new CacheItem() { Value = value }, GetOption(settings));
        }
        private static MemoryCacheEntryOptions GetOption(CacheSetting settings)
        {
            return new MemoryCacheEntryOptions()
             .SetSize(1)//Size amount
             .SetPriority(CacheItemPriority.High)
            // .SetSlidingExpiration(settings.timeLife)
             .SetAbsoluteExpiration(settings.absoloteLifeTime == null ? settings.timeLife : settings.absoloteLifeTime.Value);
        }
    }
    public class CacheItem
    {
        public object Value { get; set; }
    }
    public class CacheSetting
    {
        public CacheSetting()
        {

        }
        public CacheSetting(string key, TimeSpan timeLife)
        {
            this.key = key;
            this.timeLife = timeLife;
            this.absoloteLifeTime = timeLife;
        }
        public string key { get; set; }
        public TimeSpan timeLife { get; set; }
        public TimeSpan? absoloteLifeTime { get; set; }
        public bool clone { get; set; }

    }
}