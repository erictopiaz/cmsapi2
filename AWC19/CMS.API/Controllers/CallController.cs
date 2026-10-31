using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AWC2019.Core.Application.Account.Queries;
using CMS.API.BLL;
using CMS.API.BLL.Saler;
using CMS.Core;
using CMS.Core.Application.Account.Queries;
using CMS.Core.Application.电销管理.Queries;
using CMS.Core.Application.电销管理.Transact;
using CMS.Core.Application.账号管理.Queries;
//using AWC2019.Core.Entities;
using CMS.Data;
using ShareEntity.Call;
using ShareEntity.InputModels.Saler;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;
using Common = CMS.Core.Common;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class CallController : BaseCtrl
    {
        private IHttpContextAccessor _accessor;
        private readonly WriteToFile _writeToFile;
        public CallController(IMapper mapper, IMediator mediator, ILogger<CallController> logger, 
            IHttpContextAccessor accessor, WriteToFile writeToFile) 
            : base(mapper, mediator, logger)
        {
            this._accessor = accessor;
            this._writeToFile = writeToFile;
        }


        [HttpPost]
        [Description("备注记录")]
        [Route("Log")]
        [ActionName("备注记录")]
        public async Task<IActionResult> 呼叫记录(备注记录Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("备注记录【旧】")]
        [Route("Log_stg")]
        [ActionName("备注记录【旧】")]
        public async Task<IActionResult> 呼叫记录旧(备注记录Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("备注记录V")]
        [Route("LogV")]
        [ActionName("备注记录V")]
        public async Task<IActionResult> QueryCallLogs([FromBody] QueryCallLogModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

                var result = await CallBLL.QueryCallLogs(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryCallLogs", "QueryCallLogs ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "备注记录加载-失败e",
                    message_code = "call_log_exception"
                });
            }
        }

        /// <summary>
        /// 通话报表
        /// </summary>
        /// <param name="month">0：本月，1：上月  默认为0</param>
        /// <returns></returns>
        [HttpGet]
        [Description("备注报表")]
        [Route("RemarkReport")]
        [ActionName("备注报表")]
        public async Task<IActionResult> RemarkReport(int month=0)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                var adminAccount = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

                var result = await CallBLL.RemarkReport(month);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("RemarkReport_ex", "RemarkReport", 
                    string.Format(@"e.message={0}", e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "备注报表加载失败-失败e",
                    message_code = "remark_report_load_exception"
                });
            }
        }

        [HttpPost]
        [Description("电销报表")]
        [Route("SalerReport")]
        [ActionName("电销报表")]
        public async Task<IActionResult> SalerRecords([FromBody] SalerReportInputModel x)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                x.adminAccount = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                x.AdminIpAddress = client.ipaddress;

                var result = await SalerBLL.SalerRecords(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("SalerRecords", "SalerRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "电销报表记录加载失败-失败e",
                    message_code = "saler_record_load_exception"
                });
            }
        }

        [HttpPost]
        [Description("电销会员【查询】")]
        [Route("SearchAccount")]
        [ActionName("电销会员【查询】")]
        public async Task<IActionResult> 账号搜索(账号搜索电销 x) {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("电销任务")]
        [Route("Jobs")]
        [ActionName("电销任务")]
        public async Task<IActionResult> 电销任务(电销任务Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("加备注")]
        [Route("AddComment")]
        [ActionName("加备注")]
        public async Task<IActionResult> 电销任务(增加备注Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("任务导入")]
        [Route("AssignJobs")]
        [ActionName("任务导入")]
        public async Task<IActionResult> 任务导入(导入客户分配Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("活跃记录")]
        [Route("Activity")]
        [ActionName("活跃记录")]
        public async Task<IActionResult> 活跃记录(活跃记录Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        

    }
}
