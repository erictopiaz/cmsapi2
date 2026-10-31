using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CMS.API.BLL.Account;
using CMS.API.BLL.Vip;
using ShareEntity.InputModels.Vip;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Account/[action]")]
    [ApiController]

    [Authorize(Policy = "PathBasedPolicy")]
    public class VipController : ControllerBase
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private IMemoryCache _cache;
        public VipController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config,
            IMemoryCache memoryCache)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
            _cache = memoryCache;
        }

        #region vip 升级处理
        /// <summary>
        /// 会员升降级列表数据显示
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelMonthRecords([FromBody] QueryVipLevelInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelMonthRecords(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelMonthRecords", "VipLevelMonthRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip上有数据加载-失败e",
                    message_code = "vip_last_month_record_create_exception"
                });
            }
        }


        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelUpTo([FromBody] VipLevelUpDownInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelUpTo(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelUpTo", "VipLevelUpTo ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip升级-失败e",
                    message_code = "vip_level_up_exception"
                });
            }
        }

        /// <summary>
        /// 取消升降级
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelCancel([FromBody] VipLevelUpDownInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelCancel(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelCancel", "VipLevelCancel ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip升级取消-失败e",
                    message_code = "vip_last_month_cancel_exception"
                });
            }
        }

        #endregion

        #region vip 降级处理
        /// <summary>
        /// 会员降级列表数据显示
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelDownMonthRecords([FromBody] QueryVipLevelInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelDownMonthRecords(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelDownMonthRecords_ex", "VipLevelDownMonthRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip数据加载-失败e",
                    message_code = "vip_last_month_record_create_exception"
                });
            }
        }

        /// <summary>
        /// 降级
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelDownTo([FromBody] VipLevelUpDownInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelDownTo(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelDownTo_ex", "VipLevelDownTo ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip降级-失败e",
                    message_code = "vip_level_down_exception"
                });
            }
        }


        /// <summary>
        /// 取消升降级
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VipLevelDownCancel([FromBody] VipLevelUpDownInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipLevelDownCancel(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipLevelDownCancel_ex", "VipLevelCancel ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip降级取消-失败e",
                    message_code = "vip_last_month_cancel_exception"
                });
            }
        }

        #endregion

        #region 每天各vip存款次数
        /// <summary>
        /// 每天各vip存款次数
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        //[Description("每天各vip存款次数")]
        public async Task<IActionResult> VipDepositTimesDailyRecords([FromBody] QueryVipDepositTimesDailyInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await VipLevelUp.VipDepositTimesDailyRecords(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VipDepositTimesDailyRecords_ex", "VipDepositTimesDailyRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "vip数据加载-失败e",
                    message_code = "vip_last_month_record_create_exception"
                });
            }
        }
        #endregion


        #region 分配管理员联系方式给到vip 

        /// <summary>
        /// VIP专员导入
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportVIPContacts([FromBody] ImportVIPContactModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await AccountBLL.ImportVIPContacts(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("ImportVIPContacts_ex", "ImportVIPContacts ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "VIP专员导入,处理异常",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> QueryVipMapping([FromBody] VipMappingInputModel x)
        {

            try
            {
                var loginAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                //x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await AccountBLL.QueryVipMapping(x, loginAccount);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("QueryVipMapping_ex", "QueryVipMapping ", 
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询,处理异常",
                    message_code = "query_vip_mapping_exception"
                });
            }
        }

        /// <summary>
        /// 查询所有已分配置联系VPI
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QueryAllVIPContacts([FromBody] VipMappingInputModel x)
        {
            await Task.Delay(1);
            return Ok(new ApiResponseModel<object>
            {
                code = 0,
                message = "",
                message_code = "QueryAllVIPContacts"
            });
        }

        /// <summary>
        /// 查询当前管理员的联系方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoadContactsData()
        {

            try
            {
                var adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await AccountBLL.LoadContactsData(adminAccount);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryVipMapping_ex", "QueryVipMapping ",
                    string.Format(@"e.message={0}", e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询,处理异常",
                    message_code = "query_vip_mapping_exception"
                });
            }
        }

        /// <summary>
        /// 确认vip会员已经添加我们联系方式
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConformedVIP([FromBody] ConformActionModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await AccountBLL.ConformedVIP(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("ConformedVIP_ex", "ConformedVIP ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "确认vip会员已经添加我们联系方式,处理异常",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }

        /// <summary>
        /// 取消分配置到VIP联系方式
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelContactVIP([FromBody] ConformActionModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await AccountBLL.CancelContactVIP(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelContactVIP_ex", "CancelContactVIP ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "取消分配置到VIP联系方式,处理异常",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }
        #endregion
    }
}
