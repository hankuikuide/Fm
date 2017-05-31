
namespace Han.Cache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;

    using Han.EnsureThat;

    /// <summary>
    /// LRUcache 不支持对单个缓存对象的缓存策略
    /// 非线程安全
    /// </summary>
    public class LruMemoryCache<T> : ICacheProvider<T>
    {
       private class CachedResult
        {
            /// <summary>
            /// 被使用次数，get时增加1
            /// </summary>
            public int Usage;
            public DateTime Timestamp;
            public T Value;
        }

        private readonly int cacheMaxSize;
        private readonly TimeSpan maxDuration;
        private readonly ConcurrentDictionary<string, CachedResult> cache =
            new ConcurrentDictionary<string, CachedResult>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDuration">过期时间</param>
        /// <param name="maxSize">最大缓存数</param>
        public LruMemoryCache(TimeSpan maxDuration, int maxSize)
        {
            this.maxDuration = maxDuration;
            this.cacheMaxSize = maxSize;
        }
        /// <summary>
        /// 默认最大缓存50000，无过期时间
        /// </summary>
        public LruMemoryCache()
            : this(TimeSpan.MaxValue, 50000)
        {
            
        }

        public bool Add(string cacheKey, T val, CachePolicy cachePolicy=null)
        {
            //LRUcache 不支持对单个缓存对象的缓存策略
            Ensure.That(cachePolicy).IsNull();
            CachedResult value;
            if (cache.TryGetValue(cacheKey, out value) && (DateTime.UtcNow - value.Timestamp) <= this.maxDuration)
            {
                return false;
            }
            var cachedResult = new CachedResult
            {
                Usage = value == null ? 1 : value.Usage + 1,
                Value = val,
                Timestamp = DateTime.UtcNow
            };

            bool resutlt = cache.TryAdd(cacheKey, cachedResult);
            RemoveExpire(cacheKey);
            return resutlt;
           
        }
        ///// <summary>
        ///// 获取缓存值，如果缓存值为null，也认为有缓存
        ///// </summary>
        ///// <param name="cacheKey"></param>
        ///// <param name="val"></param>
        ///// <returns></returns>
        //private bool TryGet(string cacheKey,out T val)
        //{
        //    CachedResult value;
        //    if (cache.TryGetValue(cacheKey, out value) && (DateTime.UtcNow - value.Timestamp) <= this.maxDuration)
        //    {
                
        //        Interlocked.Increment(ref value.Usage);
        //        val = value.Value;
        //        return true;
        //    }
        //    else
        //    {
        //        val = default(T);
        //        return false;
        //    }
        //}

        public T Get(string cacheKey)
        {
            CachedResult value;
            if (cache.TryGetValue(cacheKey, out value) && (DateTime.UtcNow - value.Timestamp) <= this.maxDuration)
            {
                Interlocked.Increment(ref value.Usage);
                return value.Value;
            }
            else
            {
                return default(T);
            }
        }

        public T GetOrAdd(string cacheKey, Func<T> valueFactory, CachePolicy cachePolicy=null)
        {
            Ensure.That(cachePolicy).IsNull();
            CachedResult value;
            if (cache.TryGetValue(cacheKey, out value) && (DateTime.UtcNow - value.Timestamp) <= this.maxDuration)
            {
                Interlocked.Increment(ref value.Usage);
                return value.Value;
            }
            else
            {
                T newObj = valueFactory();
                this.Add(cacheKey, newObj, cachePolicy);
                return newObj;
            }
            //if (v == null)
            //{
            //    T newObj = valueFactory();
            //    this.Add(cacheKey, newObj, cachePolicy);
            //    return newObj;
            //}
            //else
            //{
            //    return v;
            //}
        }
       
        public T Remove(string cacheKey)
        {
            CachedResult obj;
            bool r= cache.TryRemove(cacheKey, out obj);
            if(r)
                Interlocked.Decrement(ref obj.Usage);
            return obj.Value;
        }

      
        //Set does and insert or update, as necessary
        public void Set(string cacheKey, T val, CachePolicy cachePolicy=null)
        {
            Ensure.That(cachePolicy).IsNull();
            CachedResult value;
            if (cache.TryGetValue(cacheKey, out value) && (DateTime.UtcNow - value.Timestamp) <= this.maxDuration)
            {
                //cache.TryUpdate()
            }
            var cachedResult = new CachedResult
            {
                Usage = value == null ? 1 : value.Usage + 1,
                Value = val,
                Timestamp = DateTime.UtcNow
                
            };
            cache.AddOrUpdate(cacheKey, cachedResult, (_, __) => cachedResult);
            RemoveExpire(cacheKey);
            
        }
      
        private void RemoveExpire(string key)
        {
            if (cache.Count > this.cacheMaxSize)
            {
                foreach (var source in cache
                    .OrderByDescending(x => x.Value.Usage)
                    .ThenBy(x => x.Value.Timestamp)
                    .Skip(this.cacheMaxSize))
                {
                    if (source.Key == key)
                        continue; // we don't want to remove the one we just added
                    CachedResult ignored;
                    cache.TryRemove(source.Key, out ignored);
                }
            }
        }
    }
}
