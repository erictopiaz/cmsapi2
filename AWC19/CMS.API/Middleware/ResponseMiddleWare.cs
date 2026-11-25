using CMS.Core;
using Dapper;
using Dapper.Contrib.Extensions;
using LogLib.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Middlewares;
using Middlewares.logMiddleware;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Org.BouncyCastle.Asn1.Ocsp;
using ShareEntity.Models;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UtilsLib.JWT;

namespace CMS.API.Middleware
{
    public class ResponseMiddleWare
    {
        private readonly RequestDelegate _next;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        public ResponseMiddleWare(RequestDelegate next,
            IConfiguration config,
            IHttpContextAccessor accessor)
        {
            _next = next;
            _accessor = accessor;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var method = context.Request.Method;
            var host = context.Request.Host.Value;
            var headers = context.Request.Headers;
            var account = string.Empty;

            StringValues token = string.Empty;
            _accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
            var adminAccount = JwtHelper.GetName(token, _config["jwt:loginKey"]);
            context.Request.Headers.Add("adminAccount", adminAccount);

            if (!"/api/Login".Equals(path, StringComparison.OrdinalIgnoreCase)
                && await checkAdminStatus(adminAccount))
            {
                //
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                var result = JsonConvert.SerializeObject(new { code = 1403, message = "账号状态有误或被踢出，请重新登录", message_code = "cli_access_token_need" });
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            else
            {
                //open
                await _next(context);
            }
            
        }

        private async Task<bool> checkAdminStatus(string adminAccount)
        {
            await Task.Delay(1);
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var result = connection.QueryFirstOrDefault<ofManage>("" +
                    "SELECT * " +
                    "FROM ofManage with(nolock) " +
                    "WHERE account=@account ",
                    new
                    {
                        account = adminAccount
                    });

                //string access = result == null ? null : result.access;

                if (result != null)
                {
                    if (result.status != 1 
                        || result.online != 1)
                    {
                        return true;
                    }
                    //var row = connection.Get<ofManage>(request.Account);
                    //row.currentloginip = request.ipaddress;
                    //row.currentlogintime = Common.CurrentTime;
                    //row.online = 1;
                    //row.loginnumber = row.loginnumber + 1;
                    //row.currentloginaddress = request.ipaddress;
                    //connection.Update<ofManage>(row);
                }
            }
            return false;
        }

    }
    public static class ResponseMiddleWareExtensions
    {
        public static IApplicationBuilder UseResponse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseMiddleWare>();
        }
    }
}
