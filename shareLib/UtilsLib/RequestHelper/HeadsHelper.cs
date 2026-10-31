using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib.RequestHelper
{
    /// <summary>
    /// 请求header 处理
    /// </summary>
    public static class HeadsHelper
    {
        public static string QueryHeaderKey(this IHttpContextAccessor accessor,
            string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return string.Empty;
                return accessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == key).Value.FirstOrDefault();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
