using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MiniERP.InventoryService.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static void SetRecord<T>(this IDistributedCache cache,
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
            cache.SetString(key, json, options);
        }

        /// <exception cref = "DistributedCacheException" /> 
        public static T? GetRecord<T>(this IDistributedCache cache, string key)
        {
            string? json = cache.GetString(key);

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
