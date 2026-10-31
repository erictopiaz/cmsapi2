using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.Http.Controllers;

namespace CMS.API.Filter
{
    public class ValidateModelAttribute : Attribute, IActionFilter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public bool AllowMultiple { get; set; }

        //public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        //{

        //    // If ModelState validation has FAILED
        //    if (!actionContext.ModelState.IsValid)
        //    {
        //        // Picking out the first member of the Model
        //        var error = string.Concat(
        //            actionContext.ModelState.First().Key,
        //            actionContext.ModelState.First().Value.Errors[0].ErrorMessage);

        //        error = actionContext.ModelState.First().Value.Errors[0].ErrorMessage;

        //        // Logging failed ModelState
        //        //lognet.ErrorFormat("MODELSTATEINVALID " +
        //        //    "\n     REQ: {0}, " +
        //        //    "\n     ERR: {1}, " +
        //        //    "\n     ARG: {2}",
        //        //    actionContext.Request.RequestUri,
        //        //    Newtonsoft.Json.JsonConvert.SerializeObject(actionContext.ModelState),
        //        //    Newtonsoft.Json.JsonConvert.SerializeObject(actionContext.ActionArguments));

        //        // Default Return
        //        return Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.OK,
        //           new Dictionary<string, string>()
        //            {
        //                {"status",  "0"},
        //                {"message", error },
        //            }
        //        ));
        //    }

        //    else
        //    {

        //        try
        //        {
        //            // Actual Excution of the Controller
        //            var result = continuation();
        //            result.Wait();
        //            return result;
        //        }
        //        catch (Exception ex)
        //        {
        //            // Loggin Exception
        //            //lognet.FatalFormat("CONTROLLEREXCEPTION " +
        //            //    "\n     REQ: {0}, " +
        //            //    "\n     EXP: {1}, " +
        //            //    "\n     ARG: {2}",
        //            //    actionContext.Request.RequestUri,
        //            //    ex.Message,
        //            //    Newtonsoft.Json.JsonConvert.SerializeObject(actionContext.ActionArguments));


        //            // Generic Controller Exception Reponse
        //            return Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.OK,
        //               new Dictionary<string, string>()
        //                {
        //                    {"status",  "0"},
        //                    {"message", "运行出错, 请联系在线客服" },
        //                }
        //            ));
        //        }
        //    }
        //}

        //public ValidateModelAttribute()
        //{
        //}
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (!context.ModelState.IsValid)
            {
                // 有错误
                //var x = context.ModelState.First().Value.Errors;
                //string errors = string.Join("|", x);
                var errors = context.ModelState.Keys
                    .SelectMany(a => context.ModelState[a].Errors.ToDictionary(k => a, v => v.ErrorMessage));
                string key = context.ModelState.First().Key;


                _logger.Error(context.HttpContext.Request.Path);

                var _y = context.ModelState;
                var response = new ShareEntity.OutputModels.ResponseModel<object>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = $"数据验证有误:{errors.First().Value}" 
                    //Data = _y.ToList()
                };

                context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(response);
                return;
            }


        }
    }
}
