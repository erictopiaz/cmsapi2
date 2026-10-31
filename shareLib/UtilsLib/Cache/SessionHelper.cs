using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLib.Cache
{
    public class SessionHelper
    {
        private readonly IHttpContextAccessor accessor;
        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            this.accessor = httpContextAccessor;
        }
        /// <summary>
        /// 设置session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SSet<T>(string key, T value)
        {
            try
            {
                accessor.HttpContext.Session.Set<T>(key, value);
                if (accessor.HttpContext.Session.Get<T>(key) != null)
                    return true;
                else
                    return false;

            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public bool SSet(string key, string value)
        {
            try
            {
                accessor.HttpContext.Session.SetString(key, value);
                if (accessor.HttpContext.Session.GetString(key) != null)
                    return true;
                else
                    return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GGet<T>(string key)
        {
            try
            {
                return accessor.HttpContext.Session.Get<T>(key);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        public string GGet(string key)
        {
            try
            {
                return accessor.HttpContext.Session.GetString(key);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
