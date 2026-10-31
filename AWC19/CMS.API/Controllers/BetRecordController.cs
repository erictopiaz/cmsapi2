using AutoMapper;
using AWC2019.Core.Application.Account.Queries;
using CMS.API.BLL.Game;
using CMS.Core;
//using AWC2019.Core.Entities;
using CMS.Data;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.BetRecordModel;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/bet/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]
    public class BetRecordController : Controller
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private WriteToDB _writeToDB;
        public BetRecordController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config, WriteToDB writeToDB)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
            _writeToDB = writeToDB;
        }



        [HttpPost]
        public async Task<IActionResult> QueryUserBetLog([FromBody] QueryUserBetLogInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.AdminIpAddress = client.ipaddress;
                form.adminAccount = Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                form.betId = string.IsNullOrEmpty(form.betId) ? "" : form.betId.Replace("'", "''");
                //var result = await GameBLL.QueryUserBetRecords<List<BetWithPrefixModel>, BetTotal>(form, _config);
                var result = await GameBLL.QueryUserBetRecordsV(form, _config);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryUserBetLog_ex", "QueryUserBetLog ",
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<PageModel<object>>()
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_page_failed",
                    data = new PageModel<object>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<object>()
                    }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> QueryBetAmountResize([FromBody] QueryUserBetLogInputModel form) {
            try {
                var client = HttpHelper.GetClient(_accessor);
                form.AdminIpAddress = client.ipaddress;
                form.adminAccount = Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

                var result = await GameBLL.QueryBetAmountResize(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e) {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryUserBetLog_ex", "QueryUserBetLog ",
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<PageModel<object>>() {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_page_failed",
                    data = new PageModel<object> {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<object>()
                    }
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> QueryOrdersByPlat([FromBody] QueryOrdersByPlatInputModel form) {
            try {
                var client = HttpHelper.GetClient(_accessor);
                form.AdminIpAddress = client.ipaddress;
                form.adminAccount = Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

                var result = await GameOrdersBLL.QueryOrdersByPlat(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e) {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryOrdersByPlat_ex", "QueryOrdersByPlat ",
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<PageModel<object>>() {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_page_failed",
                    data = new PageModel<object> {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<object>()
                    }
                });
            }
        }

        /// <summary>
        /// boss 游戏平台日报表  （每天对应平台一条记录）
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> QueryDailyGameReportByPlatAccunt([FromBody] QueryGameReportByPlatAccuntInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.AdminIpAddress = client.ipaddress;
                form.adminAccount = Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

                var result = await GameOrdersBLL.QueryDailyGameReportByPlatAccunt(form, _writeToFile, mapGameId:true);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryDailyGameReportByPlatAccunt_ex", "QueryDailyGameReportByPlatAccunt",
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<PageModel<object>>()
                {
                    code = 0,
                    message = "游戏加载数据_失败E",
                    message_code = "query_page_failed",
                    data = new PageModel<object>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<object>()
                    }
                });
            }
        }
    }
}
