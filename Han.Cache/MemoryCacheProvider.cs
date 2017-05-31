
namespace Han.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;

    /// <summary>
    /// A cache provider based on <see cref="MemoryCache"/>.
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider<object>
    {
        private const string _tagKey = "global::tag::{0}";
        
        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="cacheKey">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        public bool Add(string cacheKey, object value, CachePolicy cachePolicy)
        {

            var item = new CacheItem(cacheKey, value);
            var policy = CreatePolicy(cacheKey, cachePolicy);

            var existing = MemoryCache.Default.AddOrGetExisting(item, policy);
            return existing.Value == null;
        }

        /// <summary>
        /// Gets the cache value for the specified key
        /// </summary>
        /// <param name="cacheKey">A unique identifier for the cache entry.</param>
        /// <returns>
        /// The cache value for the specified key, if the entry exists; otherwise, <see langword="null"/>.
        /// </returns>
        public object Get(string cacheKey)
        {

            return MemoryCache.Default.Get(cacheKey);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="cacheKey">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a value to insert into cache.</param>
        /// <param name="cachePolicy">A <see cref="CachePolicy"/> that contains eviction details for the cache entry.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the cache,
        /// or the new value for the key as returned by <paramref name="valueFactory"/> if the key was not in the cache.
        /// </returns>
        public object GetOrAdd(string cacheKey, Func<object> valueFactory, CachePolicy cachePolicy)
        {

            if (MemoryCache.Default.Contains(cacheKey))
            {
                Debug.WriteLine("Cache Hit : " + cacheKey);
                return MemoryCache.Default.Get(cacheKey);
            }

            Debug.WriteLine("Cache Miss: " + cacheKey);
            // get value and add to cache
            object value = valueFactory();
            if (this.Add(cacheKey, value, cachePolicy))
                return value;

            // add failed
            return null;
        }

        /// <summary>
        /// Removes a cache entry from the cache.
        /// </summary>
        /// <param name="cacheKey">A unique identifier for the cache entry.</param>
        /// <returns>
        /// If the entry is found in the cache, the removed cache entry; otherwise, <see langword="null"/>.
        /// </returns>
        public object Remove(string cacheKey)
        {

            return MemoryCache.Default.Remove(cacheKey);
        }

       

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="cacheKey">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">A <see cref="CachePolicy"/> that contains eviction details for the cache entry.</param>
        /// <returns></returns>
        public void Set(string cacheKey, object value, CachePolicy cachePolicy)
        {

            var item = new CacheItem(cacheKey, value);
            var policy = CreatePolicy(cacheKey, cachePolicy);

            MemoryCache.Default.Set(item, policy);
        }



        internal static CacheItemPolicy CreatePolicy(string key, CachePolicy cachePolicy)
        {
            var policy = new CacheItemPolicy();

            switch (cachePolicy.Mode)
            {
                case CacheExpirationMode.Sliding:
                    policy.SlidingExpiration = cachePolicy.SlidingExpiration;
                    break;
                case CacheExpirationMode.Absolute:
                    policy.AbsoluteExpiration = cachePolicy.AbsoluteExpiration;
                    break;
                case CacheExpirationMode.Duration:
                    policy.AbsoluteExpiration = DateTimeOffset.Now.Add(cachePolicy.Duration);
                    break;
                default:
                    policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
                    break;
            }

            var changeMonitor = CreateChangeMonitor(key);
            if (changeMonitor != null)
                policy.ChangeMonitors.Add(changeMonitor);

            return policy;
        }

        internal static CacheEntryChangeMonitor CreateChangeMonitor(string key)
        {
            var cache = MemoryCache.Default;
            return cache.CreateCacheEntryChangeMonitor(new List<string>(){key});
        }
    }
}