using LogLib.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Middlewares.logMiddleware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WriteToFile _writeToFile;
        /// <summary>
        /// 回应数据code 是bool或int
        /// </summary>
        private readonly object _code; // 接收的参数
        private RequestResponseLog _logInfo;
        private IHttpContextAccessor _accessor;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="writeToFile"></param>
        /// <param name="accessor"></param>
        /// <param name="code">回应数据code 是bool或int</param>
        public CustomExceptionMiddleware(RequestDelegate next,
            WriteToFile writeToFile,
            IHttpContextAccessor accessor,
            object code)
        {
            _next = next;
            _writeToFile = writeToFile;
            _code = code;
            _accessor = accessor;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await _next(context);
            }
            catch (Exception e)
            {
                await ExceptionHandlerAsync(context, e, _code);
            }
        }

        private async Task ExceptionHandlerAsync(HttpContext context, Exception exception, object code)
        {
            try
            {
                _logInfo = new RequestResponseLog();
                HttpRequest request = context.Request;
                _logInfo.Url = request.Path.ToString();
                _logInfo.Headers = request.Headers.ToDictionary(k => k.Key, v => string.Join(";", v.Value.ToList()));
                _logInfo.requestIP = MiddlewareUtil.GetIP(_accessor.HttpContext);
                _logInfo.Method = request.Method;
                _logInfo.ExcuteStartTime = DateTime.UtcNow.AddHours(8);

                //获取request.Body内容
                if (request.Method.ToLower().Equals("post"))
                {
                    request.EnableBuffering(); //启用倒带功能，就可以让 Request.Body 可以再次读取
                    var stream = request.Body;
                    byte[] buffer = new byte[request.ContentLength.Value];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    _logInfo.RequestBody = Encoding.UTF8.GetString(buffer);
                    request.Body.Position = 0;
                }
                else if (request.Method.ToLower().Equals("get"))
                {
                    _logInfo.RequestBody = request.QueryString.Value;
                }
                _logInfo.StackTrace = exception.StackTrace;
                _writeToFile.Done($"RequestResponseLog{_logInfo.Url.Replace("/", "_")}_ex",
                    $"RequestResponseLog {_logInfo.Url}",
                    "VisitLog=" + _logInfo.ToString(), true);

                var rCode = (code.ToString().Equals("0") || code.ToString().Equals("false")) ? 0 : 1;
                var result = JsonConvert.SerializeObject(new
                {
                    code = code,
                    rCode = rCode,
                    message = "An error occurred.",
                    message_code = "request_ex_"
                });
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);

            }
            catch (Exception)
            {
                var rCode = (code.ToString().Equals("0") || code.ToString().Equals("false")) ? 0 : 1;
                var result = JsonConvert.SerializeObject(new
                {
                    code = code,
                    rCode = rCode,
                    message = "An error occurred..",
                    message_code = "request_ex_"
                });
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            

        }
    }

    public static class CustomExceptionMiddlewareExtensions
    {
        /// <summary>
        /// 自定义全局异常
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="code">false或0</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomException(this IApplicationBuilder builder, object code)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>(code);
        }
    }
}
