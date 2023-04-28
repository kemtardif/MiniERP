using StackExchange.Redis;
using System.Text.Json;

namespace MiniERP.PurchaseOrderService.Extensions
{
    public static class DatabaseExtensions
    {
        public static T? GetRecord<T>(this IDatabase database,
                                        string key)
        {
            string value = database.HashGet(key, "data");

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
