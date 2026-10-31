using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core
{
    public static class MemCache
    {
        public static string REDIS_CONNSTRING = "";

        public static bool PushQueue(string jsondata)
        {
            return PushQueue(jsondata, "taskq");
            //if (string.IsNullOrEmpty(REDIS_CONNSTRING)) return false;
            //try
            //{
            //    var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            //    {
            //        string cacheConnection = REDIS_CONNSTRING;
            //        return ConnectionMultiplexer.Connect(cacheConnection);
            //    });
            //    IDatabase cache = lazyConnection.Value.GetDatabase();
            //    var result = cache.ListLeftPush("taskq", jsondata);
            //    if(result > 0)
            //        return true;
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }


        public static bool PushQueue(string jsondata, string queuename)
        {
            if (string.IsNullOrEmpty(REDIS_CONNSTRING)) return false;
            try
            {
                var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    string cacheConnection = REDIS_CONNSTRING;
                    return ConnectionMultiplexer.Connect(cacheConnection);
                });
                IDatabase cache = lazyConnection.Value.GetDatabase();
                var result = cache.ListLeftPush(queuename, jsondata);
                if (result > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //public static MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions()
        //{
        //    AbsoluteExpiration = DateTime.Now.AddMinutes(30),
        //    Priority = CacheItemPriority.Normal
        //};

        //public static MemoryCache memCache = new MemoryCache(cacheExpirationOptions);
    }
}
