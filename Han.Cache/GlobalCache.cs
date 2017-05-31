
namespace Han.Cache
{
    /// <summary>
    /// 全局缓存，GlobalCache
    /// </summary>
    public class GlobalCache
    {
        /// <summary>
        /// cache object
        /// </summary>
        public static readonly ICacheProvider<object> Cache = new LruMemoryCache<object>();
    }
}