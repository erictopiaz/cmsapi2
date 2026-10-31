using CMS.Core;
using LogLib.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Middlewares;
using Middlewares.logMiddleware;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.IO;
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

            StringValues token = string.Empty;
            _accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
            var adminAccount = JwtHelper.GetName(token, _config["jwt:loginKey"]);
            context.Request.Headers.Add("adminAccount", adminAccount);
            await _next(context);
            
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
