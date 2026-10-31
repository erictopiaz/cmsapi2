using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CMS.API.Filter
{

    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        //private readonly ILoggerManager _logger;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            //_logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return  context.Response.WriteAsync(
                        Newtonsoft.Json.JsonConvert.SerializeObject(new ShareEntity.OutputModels.ResponseModel()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "error handler response"
                        })); ;
        }
    }


}
