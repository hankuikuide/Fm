using System;
using System.Web;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Han.Infrastructure
{
    public class CacheHelper
    {
        public static Dictionary<string, CacheInfo> cacheDic = new Dictionary<string, CacheInfo>();
        private static FileSystemWatcher watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "cacheConfig.json");

        /// <summary>
        /// 初始化缓存配置
        /// </summary>
        public static void InitCacheConfig()
        {
            watcher.Renamed += new RenamedEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;

            RefreshCacheConfig();
        }

        /// <summary>
        /// 配置文件监控事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void watcher_Changed(object sender, RenamedEventArgs e)
        {
            RefreshCacheConfig();
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        public static void RefreshCacheConfig()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cacheConfig.json");
            var content = File.ReadAllText(filePath);

            cacheDic = JsonConvert.DeserializeObject<Dictionary<string, CacheInfo>>(content);

            var refreshDic = cacheDic.Where(m =>
            {
                return m.Value.IsRefresh == true;
            });

            //刷新已经更新的缓存
            foreach (var item in refreshDic)
            {
                Remove(item.Key);
                cacheDic[item.Key].IsRefresh = false;
            }

            //更新配置文件
            watcher.Renamed -= new RenamedEventHandler(watcher_Changed);
            File.WriteAllText(filePath, ConvertJsonString(JsonConvert.SerializeObject(cacheDic)));
            watcher.Renamed += new RenamedEventHandler(watcher_Changed);
        }

        /// <summary>
        /// 格式化json字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ConvertJsonString(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 获取数据 如果缓存有则从缓存中获取 没有则走业务方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="addItemFactory"></param>
        /// <returns></returns>
        public static T GetOrAdd<T>(string key, Func<T> addItemFactory)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            var existingCacheItem = objCache.Get(key);

            if (existingCacheItem != null)
            {
                return (T)existingCacheItem;
            }

            try
            {
                CacheInfo cacheInfo = cacheDic[key];
                object rel = addItemFactory();
                Set(key, rel, DateTime.Now.AddMinutes(cacheInfo.Time));
                return (T)(rel);
            }
            catch
            {
                objCache.Remove(key);
                throw;
            }
        }

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="CacheKey">键</param>
        public static object Get(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache.Get(CacheKey);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void Set(string CacheKey, object objObject, DateTime outTime)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, outTime, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void Remove(string CacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(CacheKey);
        }
    }

    public class CacheInfo
    {
        public long Time { get; set; }
        public bool IsRefresh { get; set; }
    }
}