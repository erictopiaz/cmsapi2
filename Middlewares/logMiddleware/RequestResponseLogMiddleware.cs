using LogLib.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;

namespace Middlewares.logMiddleware
{
   
    public class RequestResponseLogMiddleware
    {
        private readonly RequestDelegate _next;
        private RequestResponseLog _logInfo;
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        /// <summary>
        /// 是否要打印出，回应数据body ， true:是，false:否
        /// </summary>
        private readonly bool _needLogBody; // 是否要打印出，回应数据body
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="writeToFile"></param>
        /// <param name="accessor"></param>
        /// <param name="needLogBody">是否要打印出，回应数据body ， true:是，false:否</param>
        public RequestResponseLogMiddleware(RequestDelegate next,
            WriteToFile writeToFile,
            IHttpContextAccessor accessor,
            bool needLogBody)
        {
            _next = next;
            _writeToFile = writeToFile;
            _needLogBody = needLogBody;
            _accessor = accessor;
        }

        public async Task Invoke(HttpContext context)
        {
            //h.Key.StartsWith("Content")
            _logInfo = new RequestResponseLog();
            HttpRequest request = context.Request;
            _logInfo.Url = request.Path.ToString();
            _logInfo.Headers = request.Headers.Where(x => !x.Key.Equals("Authorization")).ToDictionary(k => k.Key, v => string.Join(";", v.Value.ToList()));
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

            #region 打印回应数据
            if (_needLogBody)
            {
                string responseContent;
                var originalBodyStream = context.Response.Body;
                using (var fakeResponseBody = new MemoryStream())
                {
                    context.Response.Body = fakeResponseBody;
                    await _next(context);
                    fakeResponseBody.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(fakeResponseBody))
                    {
                        responseContent = await reader.ReadToEndAsync();
                        fakeResponseBody.Seek(0, SeekOrigin.Begin);
                        _logInfo.ResponseBody = responseContent;
                        _logInfo.ExcuteEndTime = DateTime.UtcNow.AddHours(8);
                        _writeToFile.Done($"RequestResponseLog{_logInfo.Url.Replace("/", "_")}", $"RequestResponseLog {_logInfo.Url}",
                                                 "VisitLog=" + _logInfo.ToString(), true);

                        var buffer = Encoding.UTF8.GetBytes(responseContent);
                        using (var ms2 = new MemoryStream())
                        {
                            ms2.Write(buffer, 0, buffer.Length);
                            ms2.Seek(0, SeekOrigin.Begin);
                            await ms2.CopyToAsync(originalBodyStream);
                        }
                    }
                }
            }
            else
            {
                await _next(context);
                _logInfo.ExcuteEndTime = DateTime.UtcNow.AddHours(8);
                _writeToFile.Done($"RequestLog{_logInfo.Url.Replace("/", "_")}", 
                    $"RequestLog {_logInfo.Url}",
                    "VisitLog=" + _logInfo.ToString(), true);
            }
            #endregion
        }


    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        /// <summary>
        /// 请求和回应数据日志
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="needLogBody">是否要打印出，回应数据body ， true:是，false:否</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder,bool needLogBody)
        {
            return builder.UseMiddleware<RequestResponseLogMiddleware>(needLogBody);
        }
    }
}
