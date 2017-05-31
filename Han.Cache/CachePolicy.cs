
namespace Han.Cache
{
    using System;
    using System.Runtime.Caching;

    /// <summary>
    /// Represents a set of eviction and expiration details for a specific cache entry.
    /// </summary>
    public class CachePolicy
    {
        private static readonly Lazy<CachePolicy> _current = new Lazy<CachePolicy>(() => new CachePolicy());

        /// <summary>
        /// Gets the default <see cref="CachePolicy"/>.
        /// </summary>
        public static CachePolicy Default
        {
            get { return _current.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachePolicy"/> class.
        /// </summary>
        public CachePolicy()
        {
            this.Mode = CacheExpirationMode.None;
            this._absoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
            this._slidingExpiration = ObjectCache.NoSlidingExpiration;
            this._duration = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets the cache expiration mode.
        /// </summary>
        /// <value>The cache expiration mode.</value>
        public CacheExpirationMode Mode { get; set; }

        private DateTimeOffset _absoluteExpiration;

        /// <summary>
        /// Gets or sets a value that indicates a cache entry should be evicted after a specified duration.
        /// </summary>
        public DateTimeOffset AbsoluteExpiration
        {
            get { return this._absoluteExpiration; }
            set
            {
                this._absoluteExpiration = value;
                this.Mode = CacheExpirationMode.Absolute;
            }
        }

        private TimeSpan _slidingExpiration;

        /// <summary>
        /// Gets or sets a value that indicates a cache entry should be evicted if it has not been accessed in a given span of time. 
        /// </summary>
        public TimeSpan SlidingExpiration
        {
            get { return this._slidingExpiration; }
            set
            {
                this._slidingExpiration = value;
                this.Mode = CacheExpirationMode.Sliding;
            }
        }

        private TimeSpan _duration;

        /// <summary>
        /// Gets or sets a value that indicates a cache entry should be evicted after a given span of time. 
        /// </summary>
        public TimeSpan Duration
        {
            get { return this._duration; }
            set
            {
                this._duration = value;
                this.Mode = CacheExpirationMode.Duration;
            }
        }

        /// <summary>
        /// Creates a <see cref="CachePolicy"/> with the absolute expiration.
        /// </summary>
        /// <param name="expirationSpan">The <see cref="TimeSpan"/> used to calculate absolute expiration from now.</param>
        /// <returns>An instance of <see cref="CachePolicy"/>.</returns>
        public static CachePolicy WithDurationExpiration(TimeSpan expirationSpan)
        {
            var policy = new CachePolicy
                {
                    Duration = expirationSpan
                };

            return policy;
        }

        /// <summary>
        /// Creates a <see cref="CachePolicy"/> with the absolute expiration.
        /// </summary>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <returns>An instance of <see cref="CachePolicy"/>.</returns>
        public static CachePolicy WithAbsoluteExpiration(DateTimeOffset absoluteExpiration)
        {
            var policy = new CachePolicy
                {
                    AbsoluteExpiration = absoluteExpiration
                };

            return policy;
        }

        /// <summary>
        /// Creates a <see cref="CachePolicy"/> with the sliding expiration.
        /// </summary>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        /// <returns>An instance of <see cref="CachePolicy"/>.</returns>
        public static CachePolicy WithSlidingExpiration(TimeSpan slidingExpiration)
        {
            var policy = new CachePolicy
                {
                    SlidingExpiration = slidingExpiration
                };

            return policy;
        }

    }
}