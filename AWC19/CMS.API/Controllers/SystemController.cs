using System;
using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AWC2019.Core.Application.Account.Queries;
using CMS.Core.Application.Account.Queries;
//using AWC2019.Core.Entities;
using CMS.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CMS.Core.Application.系统管理.Queries;
using System.Linq;
using CMS.Core;
using CMS.Core.Application.系统管理.Transact;
using CMS.API.BLL.Sys;
using LogLib.Helper;
using UtilsLib;
using ShareEntity.InputModels.OfSetting;
using Common = CMS.Core.Common;
using Newtonsoft.Json.Linq;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]

    public class SystemController : BaseCtrl
    {
        private readonly IActionDescriptorCollectionProvider _provider;
        private readonly WriteToFile _writeToFile;
        public SystemController(
            IMapper mapper,
            IMediator mediator,
            IActionDescriptorCollectionProvider provider1,
            ILogger<AccountController> logger, WriteToFile writeToFile) : base(mapper, mediator, logger)
        {
            _provider = provider1;
            _writeToFile = writeToFile;

            
        }

        [HttpPost]
        [Description("更新APP游戏登录模式")]
        [Route("UpdateGameUrlMode")]
        [ActionName("更新APP游戏登录模式")]
        public async Task<IActionResult> UpdateGameUrlMode([FromBody] AppGameUrlModeSettingModels x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateGameUrlMode(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateGameUrlMode_ex", "UpdateGameUrlMode ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新app老虎机参数-失败e",
                    message_code = "update_game_url_mode_settings_exception"
                });
            }
        }

        [HttpPost]
        [Description("查询APP游戏登录模式")]
        [Route("QueryGameUrlMode")]
        [ActionName("查询APP游戏登录模式")]
        public async Task<IActionResult> QueryGameUrlMode([FromBody] QueryAppGameUrlModeInputModels x)
        {
            var adminAccount = string.Empty;
            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryGameUrlMode(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("游戏登录模式_ex", "游戏登录模式 ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询APP游戏登录模式-失败e",
                    message_code = "query_app_game_url_mode_settings_exception"
                });
            }
        }


        [HttpPost]
        [Description("更新APP老虎机游戏配置")]
        [Route("UpdateAppSlotSetting")]
        [ActionName("更新APP老虎机游戏配置")]
        public async Task<IActionResult> UpdateAppSlotSetting([FromBody] AppSlotGameSettingModels x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAppSlotSetting(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateAppSlotSetting_ex", "UpdateAppSlotSetting ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新app老虎机参数-失败e",
                    message_code = "update_app_slot_settings_exception"
                });
            }
        }

        [HttpGet]
        [Description("查询APP老虎机游戏配置")]
        [Route("QueryAppSlotSetting")]
        [ActionName("查询APP老虎机游戏配置")]
        public async Task<IActionResult> QueryAppSlotSetting()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAppSlotSetting();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryAppSlotSetting_ex", "QueryAppSlotSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询APP老虎机参数-失败e",
                    message_code = "query_app_slot_game_settings_exception"
                });
            }
        }

        [HttpPost]
        [Description("更新APP首页游戏配置")]
        [Route("UpdateAppIndexSetting")]
        [ActionName("更新APP首页游戏配置")]
        public async Task<IActionResult> UpdateAppIndexSetting([FromBody] AppIndexGameSettingModels x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAppIndexSetting(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("updateAppIndexSetting_ex", "updateAppIndexSetting ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新app首页参数-失败e",
                    message_code = "update_app_index_settings_exception"
                });
            }
        }

        [HttpGet]
        [Description("查询APP首面游戏配置")]
        [Route("QueryAppIndexSetting")]
        [ActionName("查询APP首面游戏配置")]
        public async Task<IActionResult> QueryAppIndexSetting()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAppIndexSetting();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryAppIndexSetting_ex", "QueryAppIndexSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询APP参数-失败e",
                    message_code = "query_app_index_game_settings_exception"
                });
            }
        }

        [HttpPost]
        [Description("更新网站参数配置")]
        [Route("UpdateWebSiteSetting")]
        [ActionName("更新网站参数配置")]
        public async Task<IActionResult> UpdateWebSiteSetting([FromBody] WebSiteSettingsModels x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.updateWebSiteSetting(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateWebSiteSetting_ex", "UpdateWebSiteSetting ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新网站参数-失败e",
                    message_code = "update_wb_settings_exception"
                });
            }
        }


        [HttpGet]
        [Description("查询网站参数配置")]
        [Route("QueryWebSiteSetting")]
        [ActionName("查询网站参数配置")]
        public async Task<IActionResult> QueryWebSiteSetting()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryWebSiteSetting();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryWebSiteSetting_ex", "QueryWebSiteSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询网站参数-失败e",
                    message_code = "query_wb_settings_exception"
                });
            }
        }

        [HttpGet]
        [Description("合营参数配置【查询】")]
        [Route("QueryAgentRegisterHostSetting")]
        [ActionName("合营参数配置【查询】")]
        public async Task<IActionResult> QueryAgentRegisterHostSetting()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAgentRegisterHostSetting();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryAgentRegisterHostSetting_ex", "QueryAgentRegisterHostSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询合营参数配置-失败e",
                    message_code = "query_agent_settings_exception"
                });
            }
        }
        
        [HttpPost]
        [Description("合营参数配置【更新】")]
        [Route("updateAgentRegisterHostSetting")]
        [ActionName("合营参数配置【更新】")]
        public async Task<IActionResult> UpdateAgentRegisterHostSetting([FromBody] AgentRegisterHostModel model)
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAgentRegisterHostSetting(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("UpdateAgentRegisterHostSetting_ex", "UpdateAgentRegisterHostSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新-失败e",
                    message_code = "update_agent_spread_exception"
                });
            }
        }

        [HttpPost]
        [Description("接口地址列表【更新】")]
        [Route("updateApiUrlModelSetting")]
        [ActionName("接口地址列表【更新】")]
        public async Task<IActionResult> UpdateApiUrlModelSetting([FromBody] ApiUrlModel model)
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateApiUrlModelSetting(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("UpdatePromoJumpUrlSetting_ex", "UpdatePromoJumpUrlSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新-失败e",
                    message_code = "update_promo_jump_url_exception"
                });
            }
        }

        [HttpPost]
        [Description("优惠跳转地址【更新】")]
        [Route("updatePromoJumpUrlSetting")]
        [ActionName("优惠跳转地址【更新】")]
        public async Task<IActionResult> UpdatePromoJumpUrlSetting([FromBody] PromoJumpUrlModel model)
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdatePromoJumpUrlSetting(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("UpdatePromoJumpUrlSetting_ex", "UpdatePromoJumpUrlSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新-失败e",
                    message_code = "update_promo_jump_url_exception"
                });
            }
        }

        [HttpPost]
        [Description("更新APP参数配置")]
        [Route("UpdateAppSetting")]
        [ActionName("更新APP参数配置")]
        public async Task<IActionResult> UpdateAppSetting([FromBody] AppSettingsModel x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAppSetting(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateAppSetting_ex", "UpdateAppSetting ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新APP参数-失败e",
                    message_code = "update_app_settings_exception"
                });
            }
        }
        [HttpGet]
        [Description("查询APP参数配置")]
        [Route("QueryAppSetting")]
        [ActionName("查询APP参数配置")]
        public async Task<IActionResult> QueryAppSetting()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAppSetting();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryAppSetting_ex", "QueryAppSetting ", 
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询APP参数-失败e",
                    message_code = "query_app_settings_exception"
                });
            }
        }

        #region 更新 Bayo app Version

        [HttpPost]
        [Description("更新APP INTL参数配置")]
        [Route("UpdateAppSettingIntl")]
        [ActionName("更新APP INTL参数配置")]
        public async Task<IActionResult> UpdateAppSettingIntl([FromBody] AppSettingsModel x) {

            try {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAppSettingIntl(x);
                return Ok(result);
            }
            catch (Exception e) {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateAppSettingIntl_ex", "UpdateAppSettingIntl ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object> {
                    code = 0,
                    message = "更新APP intl参数-失败e",
                    message_code = "update_app_settings_exception"
                });
            }
        }
        [HttpGet]
        [Description("查询APP INTL参数配置")]
        [Route("QueryAppSettingIntl")]
        [ActionName("查询APP INTL参数配置")]
        public async Task<IActionResult> QueryAppSettingIntl() {
            var adminAccount = string.Empty;
            try {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAppSettingIntl();
                return Ok(result);
            }
            catch (Exception e) {
                _writeToFile.Done("QueryAppSettingIntl_ex", "QueryAppSettingIntl ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object> {
                    code = 0,
                    message = "查询APP intl参数-失败e",
                    message_code = "query_app_settings_exception"
                });
            }
        }

        #endregion

        #region 更新 Boss app Version

        [HttpPost]
        [Description("更新APP Boss参数配置")]
        [Route("UpdateAppSettingBoss")]
        [ActionName("更新APP Boss参数配置")]
        public async Task<IActionResult> UpdateAppSettingBoss([FromBody] AppSettingsModel x)
        {

            try
            {
                x.adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.UpdateAppSettingBoss(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("UpdateAppSettingBoss_ex", "UpdateAppSettingBoss ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "更新APP Boss参数-失败e",
                    message_code = "update_app_settings_exception"
                });
            }
        }
        [HttpGet]
        [Description("查询APP Boss参数配置")]
        [Route("QueryAppSettingBoss")]
        [ActionName("查询APP Boss参数配置")]
        public async Task<IActionResult> QueryAppSettingBoss()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await OfSettingBLL.QueryAppSettingBoss();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryAppSettingBoss_ex", "QueryAppSettingBoss ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询APP Boss参数-失败e",
                    message_code = "query_app_settings_exception"
                });
            }
        }

        #endregion

        /// <summary>
        /// 查询当前管理员支付的货币类型
        /// </summary>
        /// <param name="v">0：返回货币类型，1：返回 货币+品牌</param>
        /// <returns></returns>
        [HttpGet]
        [Description("查询货币配置")]
        [Route("QueryCurrencys")]
        [ActionName("查询货币配置")]
        [AllowAnonymous]
        public async Task<IActionResult> QueryCurrencys(int v=0) {
            var adminAccount = string.Empty;
            try {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var currencys = CMS.Core.Common.Security.QueryCurrencyTypes(User.Claims) ;
                var result = v == 0 ? await OfSettingBLL.LoadCurrency(currencys) : await OfSettingBLL.LoadCurrencyV(currencys);
                return Ok(result);
            }
            catch (Exception e) {
                _writeToFile.Done("QueryCurrencys_ex", "QueryCurrencys ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object> {
                    code = 0,
                    message = "查询A参数-失败e",
                    message_code = "query_app_settings_exception"
                });
            }
        }




        [HttpGet]
        [Description("查询货币配置")]
        [Route("QuerySettings")]
        [ActionName("查询货币配置")]
        [AllowAnonymous]
        public async Task<IActionResult> QuerySettings()
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var currencys = CMS.Core.Common.Security.QueryCurrencyTypes(User.Claims);

                var x = new GetSystemSettings();
                var result = await _mediator.Send(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QuerySettings_ex", "QuerySettings ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询A参数-失败e",
                    message_code = "query_app_settings_exception"
                });
            }
        }




        [HttpPost]
        [Description("获取所有权限")]
        [Route("Access/CommandList")]
        [ActionName("获取所有权限")]
        public async Task<IActionResult> Access_CommandList(GetApiAccessList x)
        {
            var routes = _provider.ActionDescriptors.Items
                .Where(a => a.AttributeRouteInfo != null)
                .Select(ad => new
                {
                    Description = ad.RouteValues["action"],
                    //Controller = ad.RouteValues["controller"],
                    Command = ad.AttributeRouteInfo.Template
                }).Distinct().ToDictionary(a => a.Command, a => a.Description);

            // Push in OtherRoutes
            foreach (var item in CachedData.OtherRoutes)
            {
                if (!routes.ContainsKey(item.Key))
                    routes.Add(item.Key, item.Value);
            }

            x.ApiList = routes;
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Route("Access")]
        [Description("获取本用户所有权限")]
        [ActionName("获取本用户所有权限")]
        public async Task<IActionResult> Access(GetAccessPrivilege x)
        {
            x.Account = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            //x.Account = User.Identity.Name;
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("Access/Update")]
        [Description("更新权限")]
        [ActionName("更新权限")]
        public async Task<IActionResult> AccessUpdate(UpdateAccessPrivilege x)
        {
            //x.access_privilege = x.access_privilege_string.Split(',');
            //var result = await _mediator.Send(new UpdateAccessPrivilege()
            //{
            //x.Account = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            //    accessprivilegestring = x.accessprivilegestring
            //}) ;
            var result = await _mediator.Send(x);

            return Ok(result);
        }

        [HttpPost]
        [Route("Access/Update/Custom")]
        [Description("批量更新权限")]
        [ActionName("批量更新权限")]
        private async Task<IActionResult> AccessUpdateCustom(UpdateAccessPrivilegeCustom x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Route("Access/Update/All/Add/Custom")]
        [Description("批量更新权限")]
        [ActionName("批量更新权限")]
        private async Task<IActionResult> AccessUpdateAllAddCustom(UpdateAccessPrivilegeAllAddCustom x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("Access/Update/All/Remove/Custom")]
        [Description("批量更新权限")]
        [ActionName("批量更新权限")]
        private async Task<IActionResult> AccessUpdateAllRemoveCustom(UpdateAccessPrivilegeAllAddCustom x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [Route("Access/TogglePrivilege")]
        [Description("更新权限")]
        [ActionName("更新权限")]
        public async Task<IActionResult> TogglePrivilege([FromBody] TogglePrivilege x)
        {
            var routes = _provider.ActionDescriptors.Items
                .Where(a => a.AttributeRouteInfo != null)
                .Select(ad => new
                {
                    Description = ad.RouteValues["action"],
                    //Controller = ad.RouteValues["controller"],
                    Command = ad.AttributeRouteInfo.Template
                }).Distinct().ToDictionary(a => a.Command, b => b.Description);
            //.ToDictionary(x => x.Command, x => x.Description);

            x.ApiList = routes;


            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Route("User/ToggleUserStatus")]
        [Description("账号开关")]
        [ActionName("账号开关")]
        public async Task<IActionResult> ToggleUserStatus(ToggleUserStatus x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Route("Code/GameCode")]
        [Description("游戏代码")]
        [ActionName("获取游戏编码")]
        public async Task<IActionResult> GameCode(GetGameCode x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("User/Group")]
        [Description("获取分组")]
        [ActionName("获取分组编码【比狗】")]
        public async Task<IActionResult> GetManageGroup()
        {
            var result = await _mediator.Send(new GetManageGroup() { });
            return Ok(result);
        }

        [HttpPost]
        [Route("User/Update")]
        [Description("更新后台分组")]
        [ActionName("用户分组【更新】")]
        public async Task<IActionResult> UpdateUser(UpdateUserDetails x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Route("UsersRecord")]
        [Description("后台用户列表")]
        [ActionName("后台用户列表")]
        public async Task<IActionResult> GetUsers(GetUsers x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("Access/UsersMenu")]
        [Description("后台用户列表")]
        [ActionName("后台用户菜单")]
        public async Task<IActionResult> GetUserMenu(GetUserMenu x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("Access/UsersMenu/Update")]
        [Description("更新菜单")]
        [ActionName("更新后台用户权限")]
        public async Task<IActionResult> UpdateUserMenu(UpdateUserMenu x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Route("GameConfig")]
        [Description("游戏设置")]
        [ActionName("游戏设置")]
        public async Task<IActionResult> GameConfig(GetGameConfig x)
        {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        
        [HttpPost]
        [Route("GameConfig/Switch")]
        [Description("游戏设置")]
        [ActionName("开关游戏状态")]
        public async Task<IActionResult> GameConfigSwitch(ToggleGameStatus x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("GameProxyConfig")]
        [Description("游戏【代理】查询")]
        [ActionName("游戏【代理】查询")]
        public async Task<IActionResult> GameProxyConfig(GetGameProxyConfig x) {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("ResetGameProxyConfig")]
        [Description("游戏【代理】配置")]
        [ActionName("游戏【代理】配置")]
        public async Task<IActionResult> ResetGameProxyConfig(ResetGameProxyConfig x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Route("Access/UsersMenuOwn")]
        [Description("后台用户列表")]
        [ActionName("UsersMenuOwn")]
        public async Task<IActionResult> UsersMenuOwn()
        {
            var result = await _mediator.Send(new GetUserMenu()
            {
                Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim(),
                account = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim()
            });

            var filtered = from p in result.Data
                           where p.status == true
                           select p;

            result.Data = filtered;
            return Ok(result);
        }


        [HttpPost]
        [Route("Access/DiffUsersMenuAccess")]
        [Description("获取其他用户权限")]
        [ActionName("DiffUsersMenuAccess")]
        public async Task<IActionResult> DiffUsersMenuAccess(GetDiffUserAccessMenu x)
        {
            // Only Allow Templated Users Prefix it before it execute
            x.Account = string.Format("template_{0}", x.Account);



            var admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var routes = _provider.ActionDescriptors.Items
                .Where(a => a.AttributeRouteInfo != null)
                .Select(ad => new
                {
                    Description = ad.RouteValues["action"],
                    //Controller = ad.RouteValues["controller"],
                    Command = ad.AttributeRouteInfo.Template
                }).Distinct().ToDictionary(a => a.Command, a => a.Description);

            // Push in OtherRoutes
            foreach (var item in CachedData.OtherRoutes)
            {
                if (!routes.ContainsKey(item.Key))
                    routes.Add(item.Key, item.Value);
            }

            var resultA = await _mediator.Send(new GetApiAccessList() {
                account = x.Account, 
                ApiList = routes,
                Admin = x.Account
            }) ;

            var resultB = await _mediator.Send(x);
            resultB.Data.allowedpath = resultA.Data;
            return Ok(resultB);
        }


        [HttpPost]
        [Route("Access/UsersMenu/Toggle")]
        [Description("更新权限")]
        [ActionName("更新权限")]
        public async Task<IActionResult> ToggleMenu([FromBody] ToggleMenu x)
        {
            //var routes = _provider.ActionDescriptors.Items
            //    .Where(a => a.AttributeRouteInfo != null)
            //    .Select(ad => new
            //    {
            //        Description = ad.RouteValues["action"],
            //        //Controller = ad.RouteValues["controller"],
            //        Command = ad.AttributeRouteInfo.Template
            //    }).Distinct().ToDictionary(x => x.Command, x => x.Description);

            //x.ApiList = routes;


            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        //[HttpPost]
        //[Route("SyncAffiliate")]
        //[Description("更新权限")]
        //[ActionName("更新权限")]
        //public async Task<IActionResult> SyncAffiliate()
        //{
        //    //var ids = new long[] ;
        //    //for (int i = 0; i < ids.Length; i++)
        //    //{

        //    //    var result = await _mediator.Send(new SyncAffiliate()
        //    //    {
        //    //        Id = ids[i]
        //    //    });

        //    //}
        //    return Ok(null);
        //}


        [HttpPost]
        [Route("ChangePassword")]
        [Description("更新自己密码")]
        [ActionName("更新自己密码【比钩】")]
        public async Task<IActionResult> ChangePassword(UpdatePassword x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            //if(result.code == ShareEntity.Global.ResponseCode.code.SUCCESS)
            //{
            //    return new UnauthorizedResult();
            //}
            return Ok(result);
        }



        [HttpPost]
        [Description("更新网站参数配置")]
        [Route("UpdateWebSiteNavSetting")]
        [ActionName("更新网站参数配置")]
        public async Task<IActionResult> UpdateWebSiteNavSetting([FromBody] WebSiteNavSettingsModel x)
        {

                var y = new UpdateWebsiteNavSettings()
                {
                    Admin = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim(),
                    inputModel = x,
                    merchantAccount = x.mer,
                    currencyType = x.currency
                };
                var result = await _mediator.Send(y);
                return Ok(result);
        }


        [HttpGet]
        [Description("查询网站导航配置")]
        [Route("QueryWebSiteNavSetting")]
        [ActionName("查询网站导航配置")]
        public async Task<IActionResult> QueryWebSiteNavSetting(string mer, int currency)
        {
            var adminAccount = string.Empty;
            try
            {
                adminAccount = CMS.Core.Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = mer.Equals("Awc", StringComparison.OrdinalIgnoreCase) 
                    ? await OfSettingBLL.QueryWebSiteNavSetting()
                    : await OfSettingBLL.QueryWebSiteNavSettingINR(merchantAccount:mer, currencyType: currency);

                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryWebSiteSetting_ex", "QueryWebSiteSetting ",
                    string.Format(@"e.message={0} , adminAccount={1}", e.Message, adminAccount), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "查询网站参数-失败e",
                    message_code = "query_wb_settings_exception"
                });
            }
        }

    }
}
