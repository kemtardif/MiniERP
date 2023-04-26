using StackExchange.Redis;
using System.Text.Json;

namespace MiniERP.InventoryService.Caching
{
    public static class DatabaseExtension
    {
        public static void SetRecord<T>(this IDatabase cache,
                                        string key,
                                        T data,
                                        TimeSpan? expiry = null)
        {
            string json = JsonSerializer.Serialize(data);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(json))
            {
                return;
            }
            cache.StringSet(key, json, expiry);
        }

        public static T? GetRecord<T>(this IDatabase cache, string key)
        {
            string? json = cache.StringGet(key);

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
