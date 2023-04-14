using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MiniERP.SalesOrderService.Caching
{
    public static class DistributedCacheExtension
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string key,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = unusedExpireTime,
            };

            string json = JsonSerializer.Serialize(data);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(json))
            {
                return;
            }

            await cache.SetStringAsync(key, json, options);
        }

        /// <exception cref = "DistributedCacheException" /> 
        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string key)
        {
            string? json = await cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
