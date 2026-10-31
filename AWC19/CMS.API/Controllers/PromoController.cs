using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.API.BLL.Euro;
using CMS.API.BLL.Promo;
using ShareEntity.InputModels.Euro;
using ShareEntity.InputModels.Promo;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    /// <summary>
    /// 优惠管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Promo/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]

    public class PromoController : ControllerBase
    {

        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private readonly IMediator _mediator;


        #region Arvin 给重调整初始化
        public PromoController(IMediator mediator, WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
            _mediator = mediator;
        }
        #endregion
        /// <summary>
        /// 加载当前的自助优惠的分类菜单
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AutoPromoTypes([FromBody] QueryAutoPromoTypesInputModel form)
        {
            try
            {
                //var client = HttpHelper.GetClient(_accessor);
                //form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                //form.AdminIpAddress = client.ipaddress;

                var result = await PromoBLL.AutoPromoTypes(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("AutoPromoTypes", "AutoPromoTypes ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "加载自助优惠的分类菜单-失败e",
                    message_code = "load_auto_promo_menus_exception"
                });
            }
        }

        [HttpPost]
        [Route("PromoCodeRecords")]
        [ActionName("优惠记录")]
        public async Task<IActionResult> PromoCodeRecords([FromBody] QueryPromoCodeInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await PromoCodeBLL.PromoCodeRecords(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("PromoCodeRecords", "PromoCodeRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠代码记录加载失败-失败e",
                    message_code = "promo_code_load_exception"
                });
            }
        }

        



        /// <summary>
        /// 优惠积分-分页查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PromoMiscRecords([FromBody] QueryPromoMiscInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;

                var result = await PromoMiscBLL.PromoMiscRecords(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("PromoMiscRecords", "PromoMiscRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠积分记录加载失败-失败e",
                    message_code = "promo_misc_load_exception"
                });
            }
        }

        /// <summary>
        /// 创建优惠积分
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePromoMisc([FromBody] CreatePromoMiscInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await PromoMiscBLL.CreatePromoMisc(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreatePromoMisc", "CreatePromoMisc ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "创建优惠积分失败-失败e",
                    message_code = "promo_misc_create_exception"
                });
            }
        }
        /// <summary>
        /// 优惠积分审核
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyPromoMisc([FromBody] VerifyPromoMiscInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await PromoMiscBLL.VerifyPromoMisc(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifyPromoMisc", "VerifyPromoMisc ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠积分审核失败-失败e",
                    message_code = "promo_misc_verify_exception"
                });
            }
        }
        /// <summary>
        /// 优惠积分取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelPromoMisc([FromBody] VerifyPromoMiscInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await PromoMiscBLL.CancelPromoMisc(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelPromoMisc", "CancelPromoMisc ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠积分取消失败-失败e",
                    message_code = "promo_misc_cancel_exception"
                });
            }
        }
        /// <summary>
        /// 推荐管理 分页查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SpreadRecords([FromBody] QuerySpreadInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;
                var result = await SpreadBLL.SpreadRecords(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("SpreadRecords", "SpreadRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "加载推记录失败-失败e",
                    message_code = "promo_spread_load_exception"
                });
            }
        }

        #region 推荐会员管理
        /// <summary>
        /// 推荐结算 分页查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SpreadBilledRecords([FromBody] QuerySpreadBilledInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;

                var result = await SpreadBLL.SpreadBilledRecords(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("SpreadBilledRecords", "SpreadBilledRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "加载推记录失败-失败e",
                    message_code = "promo_spread_load_exception"
                });
            }
        }

        /// <summary>
        /// 推荐结算 分页查询 INTL
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SpreadBilledRecordsINTL([FromBody] QuerySpreadBilledIntlInputModel form) {
            try {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;

                var result = await SpreadBLL.SpreadBilledRecordsINTL_Weekly(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e) {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("SpreadBilledRecordsINTL_ex", "SpreadBilledRecordsINTL ", 
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object> {
                    code = "false",
                    message = "加载推记录失败-失败e",
                    message_code = "promo_spread_load_exception"
                });
            }
        }

        /// <summary>
        /// 推荐结算 取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelSpreadBilled([FromBody] CacelSpreadBilledInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await SpreadBLL.CancelSpreadBilled(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelSpreadBilled", "CancelSpreadBilled ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "推荐结算取消-失败e",
                    message_code = "promo_spread_cancel_exception"
                });
            }
        }
        /// <summary>
        /// 推荐结算 审核
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifySpreadBilled([FromBody] CacelSpreadBilledInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.AdminIpAddress = client.ipaddress;

                var result = await SpreadBLL.VerifySpreadBilled(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifySpreadBilled", "VerifySpreadBilled ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "推荐结算审核-失败e",
                    message_code = "promo_spread_verify_exception"
                });
            }
        }

        #endregion


        /// <summary>
        /// 优惠内容的配置  创建
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePromoContent([FromBody] CreatePromoContentInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;
                form.createcustomer = form.adminAccount;

                var result = await PromoBLL.CreatePromoContent(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreatePromoContent", "CreatePromoContent ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "创建优惠活动失败-失败e",
                    message_code = "promo_create_content_exception"
                });
            }
        }
        /// <summary>
        /// 优惠内容单调出来供修改
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoadPromoContentById(long tid)
        {
            try
            {
                var result = await PromoBLL.LoadPromoContentById(tid);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("LoadPromoContentById", "LoadPromoContentById ", string.Format(@"promo_id = {0}   e.message={1}", tid, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "获取优惠活动失败-失败e",
                    message_code = "promo_content_load_exception"
                });
            }
        }
        /// <summary>
        /// 优惠内容  分页查询
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PromoContentRecords([FromBody] QueryPromoContentInputModel inputModel)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                inputModel.AdminIpAddress = client.ipaddress;
                var result = await PromoBLL.PromoContentRecords(inputModel, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(inputModel);
                _writeToFile.Done("PromoContentRecords", "PromoContentRecords ", string.Format(@"requestData = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "获取优惠活动失败-失败e",
                    message_code = "promo_content_load_exception"
                });
            }
        }


        #region 欧洲杯竞猜 

        /// <summary>
        /// 欧洲杯竞猜，数据
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ECupBetRecords([FromBody] QueryEuroCupBilledInputModel inputModel)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                inputModel.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                inputModel.AdminIpAddress = client.ipaddress;

                var result = await EuroBLL.ECupBetRecords(inputModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(inputModel);
                _writeToFile.Done("ECupBetRecords", "ECupBetRecords ", string.Format(@"requestData = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "获取竞猜队伍-失败e",
                    message_code = "promo_content_load_exception"
                });
            }
        }

        /// <summary>
        /// 欧洲杯竞猜审核
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyECupBilled([FromBody] EuroUpdateInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.ipAddress = client.ipaddress;

                var result = await EuroBLL.VerifyECupBilled(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifyBonus", "VerifyBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录审核-失败e",
                    message_code = "bonus_record_veify_exception"
                });
            }
        }

        /// <summary>
        /// 欧洲杯竞猜审核取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelECupBilled([FromBody] EuroUpdateInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.ipAddress = client.ipaddress;

                var result = await EuroBLL.CancelECupBilled(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelECupBilled_ex", "CancelECupBilled ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "欧洲杯竞猜审核取消-失败e",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }
        #endregion







        [HttpPost]
        [Description("红包雨讯息")]
        //[Route("HongBaoYuSettings")]
        //[ActionName("红包雨设置调整")]
        public async Task<IActionResult> HongBaoYuSettings()
        {
            var result = await _mediator.Send(new Core.Application.优惠管理.Queries.红包雨讯息());
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("红包雨讯息")]
        //[Route("HongBaoYuSettings")]
        //[ActionName("红包雨设置调整")]
        public async Task<IActionResult> HongBaoYuUpdate([FromBody] Core.Application.网站管理.Queries.红包雨设置Update x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("周周挑战")]
        //[Route("HongBaoYuSettings")]
        //[ActionName("红包雨设置调整")]
        public async Task<IActionResult> BonusPromoAuto([FromBody] Core.Application.优惠管理.Queries.BonusPromoAuto x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("周周挑战详细")]
        //[Route("HongBaoYuSettings")]
        //[ActionName("红包雨设置调整")]
        public async Task<IActionResult> BonusPromoAutoZhouzhouDetails([FromBody] Core.Application.优惠管理.Queries.周周挑战详细 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }




    }
}
