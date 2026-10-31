using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace UtilsLib.Filter
{
    //静态区
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = false;
            //path 
            //当wget 

            //权限验证省略
            if (token)
            {
                


                // 取得由 API 返回的状态代码
                //result.code = (int)HttpStatusCode.Unauthorized;
                // 取得由 API 返回的资料
                //result.data = null;
                //result.msg = "invalid ticket value";

                //HttpResponseMessage response = new HttpResponseMessage
                //{
                //    Content = new StringContent(JsonConvert.SerializeObject(result),
                //       Encoding.GetEncoding("UTF-8"), "application/json")
                //};
                //结果转为自定义消息格式
                //HttpResponseMessage httpResponseMessage = response;

                // 重新封装回传格式
                //context. = httpResponseMessage;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                //执行成功 取得由 API 返回的资料
                ObjectResult result = filterContext.Result as ObjectResult;
                if (result != null)
                {
                    // 重新封装回传格式
                    ApiResultModel<object> apiResultModel = new ApiResultModel<object>();
                    //robj.Success(result.Value);
                    ObjectResult objectResult = new ObjectResult(apiResultModel);
                    filterContext.Result = objectResult;
                }
            }
            else
            {

            }
            base.OnActionExecuted(filterContext);
        }

        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    base.OnResultExecuting(context);
        //}

        //public override void OnResultExecuted(ResultExecutedContext context)
        //{
        //    base.OnResultExecuted(context);
        //}
    }
}
