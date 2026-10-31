using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.API.BLL.Promo;
using ShareEntity.InputModels.TurnOver;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using UtilsLib;
using UtilsLib.Cache;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/TurnOver/[action]")]

    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class TurnOverController : Controller
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private IMemoryCache _cache;
        public TurnOverController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config,
            IMemoryCache memoryCache)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
            _cache = memoryCache;
        }


        [HttpPost]
        public async Task<IActionResult> TurnOverRecords([FromBody]QueryTurnOverInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;
                var result = await TurnOverBLL.TurnOverRecords(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("TurnOverRecords", "TurnOverRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "返水记录加载-失败e",
                    message_code = "turnover_record_create_exception"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTurnOver([FromBody]CreateTurnOverInputModel form)
        {
            try
            {
                
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                #region 防址连续性操作
                var key = string.Format(@"{0}VerifyTurnOver", form.adminAccount);
                string ant = CacheHelper.GetCache(_cache, key);
                if (!string.IsNullOrEmpty(ant))
                    return Ok(new { code = "false", message = "操作太频繁" + form.adminAccount, message_code = "refresh_deny" });
                if (!CacheHelper.SetCache(_cache, key, form.adminAccount, 12))
                    return Ok(new { code = "false", message = "添加返水,TOKEN设置失败" + form.adminAccount });
                #endregion

                var result = await TurnOverBLL.CreateTurnOver(form, _writeToFile);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateTurnOver", "CreateTurnOver ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "创建返水记录加载-失败e",
                    message_code = "turnover_create_exception"
                });
            }
        }
        /// <summary>
        /// 返水审核
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyTurnOver([FromBody]VerifyTurnOverInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                #region 防址连续性操作
                var key = string.Format(@"{0}VerifyTurnOver", form.adminAccount);
                string ant = CacheHelper.GetCache(_cache, key);
                if (!string.IsNullOrEmpty(ant))
                    return Ok(new { code = "false", message = "操作太频繁" + form.adminAccount, message_code = "refresh_deny" });
                if (!CacheHelper.SetCache(_cache, key, form.adminAccount, 12))
                    return Ok(new { code = "false", message = "添加返水,TOKEN设置失败" + form.adminAccount });

                #endregion

                var result = await TurnOverBLL.VerifyTurnOver(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifyTurnOver", "VerifyTurnOver ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "审核返水记录加载-失败e",
                    message_code = "turnover_verify_exception"
                });
            }
        }
        /// <summary>
        /// 返水取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelTurnOver([FromBody]VerifyTurnOverInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await TurnOverBLL.CancelTurnOver(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelTurnOver", "CancelTurnOver ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "取消返水记录加载-失败e",
                    message_code = "turnover_cancel_exception"
                });
            }
        }
    }
}