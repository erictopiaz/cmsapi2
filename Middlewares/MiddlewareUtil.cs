using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Middlewares
{
    public class MiddlewareUtil
    {
        public static string GetIP(HttpContext context)
        {
			try
			{
                string ip = "";
                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                    ip = context.Request.Headers["X-Forwarded-For"].ToString();
                else
                    ip = context.Connection.RemoteIpAddress.ToString();

                return ip;
            }
			catch (Exception)
			{
                return "";
			}

        }
    }
}
