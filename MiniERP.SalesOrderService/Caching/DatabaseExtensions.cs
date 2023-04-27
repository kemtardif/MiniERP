using StackExchange.Redis;
using System.Text.Json;

namespace MiniERP.SalesOrderService.Caching
{
    public static class DatabaseExtensions
    {
        public static T? GetRecord<T>(this IDatabase database, 
                                        string key)
        {   
            //tring value = database.HashGet($"invsrv:{key}", "data");

            string value = database.HashGet(key, "data");

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
