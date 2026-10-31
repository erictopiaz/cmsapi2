using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLib.Cache
{
    public class CacheHelper
    {
        /// <summary>
        /// 目前针对游戏回调认证
        /// </summary>
        /// <param name="key">当前游戏的token</param>
        /// <param name="value">账号</param>
        public static bool SetCache(IMemoryCache _cache, string key, string value, double sec = 100)
        {
            if (_cache is null)
                return false;
            try
            {
                _cache.Set(key, value, TimeSpan.FromSeconds(sec));
                if (_cache.Get(key) == null)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 设置key value 并检测当前KEY 是否存在
        /// 不存在而且成功设置，返回true
        /// 存在或设置失败，返回false
        /// </summary>
        /// <param name="_cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static bool SetCacheWithCheckKey(IMemoryCache _cache, string key, string value, double sec = 100)
        {
            if (_cache is null)
                return false;
            try
            {
                lock (_cache)
                {
                    if (_cache.Get(key) != null)//已经在
                        return false;
                    _cache.Set(key, value, TimeSpan.FromSeconds(sec));
                }
                if (_cache.Get(key) == null) 
                    return false;
                else
                    return true; //新设置成功
            }
            catch (Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 目前针对游戏回调认证
        /// </summary>
        /// <param name="key">当前游戏的token</param>
        /// <returns></returns>
        public static string GetCache(IMemoryCache _cache, string key)
        {
            if (_cache is null)
                return string.Empty;
            try
            {
                if (_cache.Get(key) == null)
                    return string.Empty;
                else
                    return _cache.Get(key).ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        /// <summary>
        /// 移除当前缓存内容
        /// </summary>
        /// <param name="key">当前游戏的token</param>
        /// <returns></returns>
        public static bool RemoveCache(IMemoryCache _cache, object key)
        {
            if (_cache is null)
                return true;
            try
            {
                if (_cache.Get(key) == null)
                    return true;
                else
                {
                    _cache.Remove(key);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }





        /// <summary>
        /// 操作失败次数记录
        /// </summary>
        /// <param name="_cache"></param>
        /// <param name="account">账号或ip或其它</param>
        /// <param name="actionName">操作action</param>
        /// <param name="limitTimes">限制失败的次数</param>
        /// <param name="limitSec">限制多少秒后才可以操作,如2小时为 60 * 60 * 2</param>
        /// <param name="limitTimesSec">指定多少秒几次失败后，就会锁limitSec 这么长时间</param>
        /// <returns></returns>
        public static async Task SetLoginFailedCountAsync(IMemoryCache _cache, 
            string account,
            string actionName, 
            int limitTimes,
            int limitSec,
            int limitTimesSec)
        {
            try
            {
                await Task.Delay(1);
                var key = string.Format(@"{0}_{1}", account, actionName);
                string ts = GetCache(_cache, key);
                var times = 0;
                if (!string.IsNullOrEmpty(ts) 
                    && Convert.ToInt32(ts) >= limitTimes)
                {
                    var allowedKey = string.Format(@"{0}_f_{1}", account, actionName);
                    SetCache(_cache, allowedKey, "many times failed", limitSec); //超过限制次数，设置锁定时间
                }
                else
                {
                    times = string.IsNullOrEmpty(ts) ? 0 : Convert.ToInt32(ts);
                }
                if (!SetCache(_cache, key, (times + 1).ToString(), limitTimesSec))
                {
                    //限制次数+1 保存成功
                }

            }
            catch (Exception)
            {
            }
        }


    }
}
