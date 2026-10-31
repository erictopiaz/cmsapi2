using AutoMapper;
using AWC2019.Core.Application.Account.Queries;
using CMS.API.BLL.Currency;
using CMS.API.BLL.Goods;
using CMS.API.BLL.Merchant;
using CMS.API.BLL.Message;
using CMS.API.BLL.Promo;
using CMS.Core;
using CMS.Core.Application.Account.Queries;
using CMS.Core.Application.系统管理.Queries;
using CMS.Core.Application.网站管理.Queries;
using CMS.Core.Application.网站管理.Transact;
using CMS.Data;
using ShareEntity.InputModels.Goods;
using ShareEntity.InputModels.Message;
using ShareEntity.InputModels.website;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;
using UtilsLib.RequestHelper;
using Common = CMS.Core.Common;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class WebsiteController : BaseCtrl
    {
        private readonly IActionDescriptorCollectionProvider _provider;
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;

        public WebsiteController(
            WriteToFile writeToFile,
            IMapper mapper,
            IMediator mediator,
            IActionDescriptorCollectionProvider provider1,
            ILogger<AccountController> logger,
            IHttpContextAccessor accessor, 
            IConfiguration config) : base(mapper, mediator, logger)
        {
            _provider = provider1;
            this._writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
        }

        [HttpPost]
        [Description("编辑公告内容")]
        [Route("NoticeEdit")]
        [ActionName("公告编辑")]
        public async Task<IActionResult> NoticeEdit(WebNoticeEdit x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

            var client = HttpHelper.GetClientWithOutMac(_accessor);
            
            var currecnyAccessCheck = CurrencyBLL.QueryAccess(token: client.token,
                secret: _config["jwt:loginKey"],
                currencycode: x.currencycode);

            if (currecnyAccessCheck.code == ShareEntity.Global.ResponseCode.code.FAIL)
                return await Task.FromResult(Ok(currecnyAccessCheck));

            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("获取域名7天登录数据")]
        [Route("QueryDomainAuthData")]
        [ActionName("获取域名7天登录数据")]
        public async Task<IActionResult> 获取域名7天登录数据(DomainAuthModel x) {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("增加新公告")]
        [Route("NoticeAdd")]
        [ActionName("公告增加")]
        public async Task<IActionResult> 公告增加(WebNoticeAdd x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

            var client = HttpHelper.GetClientWithOutMac(_accessor);
            var currecnyAccessCheck = CurrencyBLL.QueryAccess(token: client.token,
                secret: _config["jwt:loginKey"],
                currencycode: x.currencycode);
            if(currecnyAccessCheck.code == ShareEntity.Global.ResponseCode.code.FAIL)
                return await Task.FromResult(Ok(currecnyAccessCheck));

            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("获取公告列表")]
        [Route("NoticeRecord")]
        [ActionName("公告列表")]
        public async Task<IActionResult> 公告列表(WebNoticeRecord x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("关闭开启列表")]
        [Route("NoticeSwitch")]
        [ActionName("公告开关")]
        public async Task<IActionResult> NoticeSwitch([FromBody] WebNoticeSwitch x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("有管APP参数的设置值")]
        [Route("AppConfig")]
        [ActionName("APP参数列表")]
        public async Task<IActionResult> AppConfig()
        {
            var result = await _mediator.Send(new AppConfig());
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("APP优惠图片列表")]
        [Route("AppBanner")]
        [ActionName("APP优惠图片列表")]
        public async Task<IActionResult> AppBanner()
        {
            var result = await _mediator.Send(new AppBanner());
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("APP优惠图片【更新】")]
        [Route("AppBannerUpdate")]
        [ActionName("APP优惠图片【更新】")]
        public async Task<IActionResult> AppBannerUpdate(AppBannerUpdate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("网站链接设置")]
        [Route("WebLinks")]
        [ActionName("网站链接设置")]
        public async Task<IActionResult> WebLinks()
        {
            var result = await _mediator.Send(new WebLinks());
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("网站链接设置【更新】")]
        [Route("WebLinksUpdate")]
        [ActionName("网站链接设置【更新】")]
        public async Task<IActionResult> WebLinksUpdate(WebLinksUpdate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("APP设置更新")]
        [Route("AppConfigUpdate")]
        [ActionName("APP参数更新")]
        public async Task<IActionResult> AppConfigUpdate(AppConfigUpdate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }






        [HttpPost]
        [Description("银行卡管理")]
        [Route("BankCardRecord")]
        [ActionName("收款银行卡列表")]
        public async Task<IActionResult> 收款银行卡列表(NetBankRecord x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("Dashboard")]
        [Route("Dashboard")]
        [ActionName("Dashboard")]
        public async Task<IActionResult> 收款银行卡列表(GetDashboardDetails x)
        {
            var client = HttpHelper.GetClientWithOutMac(_accessor);
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();//ching add  for mk admin load data
            x.loginKey = _config["jwt:loginKey"];
            x.loginToken = client.token;

            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("增加银行卡")]
        [Route("BankCardAdd")]
        [ActionName("增加银行卡")]
        public async Task<IActionResult> 增加银行卡(NetBankAdd x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("银行卡禁用")]
        [Route("BankCardDisable")]
        [ActionName("银行卡禁用")]
        public async Task<IActionResult> BankCardDisable(NetBankDisable x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("第三方商户禁用")]
        [Route("ThirdpayDisable")]
        [ActionName("第三方商户禁用")]
        public async Task<IActionResult> ThirdpayDisable(ThirdpayDisable x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("银行卡启用")]
        [Route("ThirdpaySwitch")]
        [ActionName("商户启用")]
        public async Task<IActionResult> ThirdpaySwitch(ThirdpaySwitch x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("商户外层启用")]
        [Route("ThirdpayNewUserSwitch")]
        [ActionName("商户外层启用")]
        public async Task<IActionResult> ThirdpayNewUserSwitch(ThirdpayNewUserSwitch x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("商户组开关")]
        [Route("ThirdpayGroupSwitch")]
        [ActionName("商户组开关")]
        public async Task<IActionResult> ThirdpayGroupSwitch(ThirdpayGroupSwitch x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }





        [HttpPost]
        [Description("银行卡启用")]
        [Route("BankCardSwitch")]
        [ActionName("商户启用")]
        public async Task<IActionResult> BankCardSwitch(NetBankSwitch x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("银行卡编辑")]
        [Route("BankCardUpdate")]
        [ActionName("银行卡编辑")]
        public async Task<IActionResult> BankCardUpdate(NetBankEdit x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("第三方通道列表")]
        [Route("ThirdpayPlatformList")]
        [ActionName("第三方通道")]
        public async Task<IActionResult> 第三方通道(ThirdpayRecord x)
        {
            var client = HttpHelper.GetClientWithOutMac(_accessor);
            x.token = client.token;
            x.loginkey = _config["jwt:loginKey"];

            x.adminAccount = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("第三方详细")]
        [Route("ThirdpayPlatformDetail")]
        [ActionName("第三方详细")]
        public async Task<IActionResult> 第三方详细(ThirdpayDetail x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("第三方讯息编辑")]
        [Route("ThirdpayEdit")]
        [ActionName("第三方讯息编辑")]
        public async Task<IActionResult> 第三方讯息编辑(ThirdpayEdit x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("商户通道存款限制设置")]
        [Route("ThirdpayEdit_DepositLimitOnly")]
        [ActionName("商户通道存款限制设置")]
        public async Task<IActionResult> 商户通道存款限制设置(ThirdpayEdit_DepositLimitOnly x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("附加存款记录查询支付通道")]
        [Route("savedQueryWhere")]
        [ActionName("附加存款记录查询支付通道")]
        public async Task<IActionResult> SaveQueryMerchantWhere([FromBody] QueryMerchantWhereModel x)
        {

            try
            {
                x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await MerchantBLL.SaveQueryMerchantWhere(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("SaveQueryMerchantWhere", "SaveQueryMerchantWhere ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResponseModel<object>
                {
                    code = 0,
                    message = "设置查询商户通道使用条件-失败e",
                    message_code = "save_merchant_where_exception"
                });
            }
        }



        [HttpPost]
        [Description("会员站内讯息列表")]
        [Route("MessageRecord")]
        [ActionName("站内讯息列表")]
        public async Task<IActionResult> 站内讯息(MessageRecord x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("增加会员讯息")]
        [Route("MessageAdd")]
        [ActionName("增加站内讯息")]
        public async Task<IActionResult> 增加站内讯息(MessageAdd x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();


            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("回复会员留言")]
        [Route("LMessageReply")]
        [ActionName("回复会员留言")]
        public async Task<IActionResult> LMessageReply([FromBody]LMessageReplyInputModel x)
        {
            
            try
            {
                x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await MesageBLL.LMessageReply(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("LMessageReply", "LMessageReply ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code="false",
                    message = "留言-失败e",
                    message_code = "LMessage_reply_create_exception"
                });
            }
        }

        [HttpPost]
        [Description("留言信息列表")]
        [Route("LMessageRecord")]
        [ActionName("留言信息列表")]
        public async Task<IActionResult> LMessageRecords([FromBody] LMessageInputModel x)
        {
            try
            {
                //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await MesageBLL.LMessageRecords(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("LMessageRecords", "LMessageRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code="false",
                    message = "留言信息加载-失败e",
                    message_code = "LMessage_record_create_exception"
                });
            }
        }
        //CancelReply
        [HttpGet]
        [Description("留言信息取消")]
        [Route("LMessageCancel")]
        [ActionName("留言信息取消")]
        public async Task<IActionResult> LMessageCancel(long tid)
        {
            try
            {
                var Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await MesageBLL.LMessageCancel(tid, Admin);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("LMessageCancel", "LMessageCancel ", 
                    string.Format(@"id = {0}   e.message={1}", tid, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "留言信息取消-失败e",
                    message_code = "LMessage_cancel_exception"
                });
            }
        }


        [HttpGet]
        [Description("留言信息查看")]
        [Route("LMessageDetail")]
        [ActionName("留言信息查看")]
        public async Task<IActionResult> LMessageById(long id)
        {
            try
            {
                var result = await MesageBLL.LMessageById(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("LMessageById", "LMessageById ", string.Format(@"id = {0}   e.message={1}", id, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "留言信息查看-失败e",
                    message_code = "LMessage_query_exception"
                });
            }
        }


        //QueryExchangeGoods

        [HttpPost]
        [Description("积分礼品记录")]
        [Route("QueryExchangeGoods")]
        [ActionName("积分礼品记录")]
        public async Task<IActionResult> QueryExchangeGoods([FromBody] QueryGoodsInputModel x)
        {
            try
            {
                //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await PointsGoodsBLL.QueryExchangeGoods(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("QueryExchangeGoods", "QueryExchangeGoods ", 
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "积分礼品加载-失败e",
                    message_code = "query_goods_records_exception"
                });
            }
        }

        [HttpPost]
        [Description("积分礼品新增")]
        [Route("CreatePointsGoods")]
        [ActionName("积分礼品新增")]
        public async Task<IActionResult> CreatePointsGoods([FromBody] CreateGoodsInputModel x)
        {

            try
            {
                x.create_by = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await PointsGoodsBLL.CreatePointsGoods(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("CreatePointsGoods", "CreatePointsGoods ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "积分礼品新增-失败e",
                    message_code = "point_goods_create_exception"
                });
            }
        }
        //OffOrOn

        [HttpPost]
        [Description("积分礼品启用/关闭")]
        [Route("OffOrOn")]
        [ActionName("积分礼品启用/关闭")]
        public async Task<IActionResult> OffOrOn([FromBody] OffOrOnInputModel x)
        {

            try
            {
                //x.create_by = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await PointsGoodsBLL.OffOrOn(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("OffOrOn", "OffOrOn ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "积分礼品启用/关闭-失败e",
                    message_code = "point_goods_off_on_exception"
                });
            }
        }
        
        [HttpGet]
        [Description("积分礼品详情")]
        [Route("QueryGoodsById")]
        [ActionName("积分礼品详情")]
        public async Task<IActionResult> QueryGoodsById(long id)
        {
            try
            {
                var result = await PointsGoodsBLL.QueryGoodsById(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("QueryGoodsById", "QueryGoodsById ", string.Format(@"id = {0}   e.message={1}", id, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "积分礼品详情-失败e",
                    message_code = "query_goods_detail_exception"
                });
            }
        }

        
        [HttpPost]
        [Description("积分礼品修改")]
        [Route("ModifyGoods")]
        [ActionName("积分礼品修改")]
        public async Task<IActionResult> ModifyGoods([FromBody] UpdateGoodsInputModel x)
        {

            try
            {
                x.update_by = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await PointsGoodsBLL.ModifyGoods(x);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                _writeToFile.Done("ModifyGoods", "ModifyGoods ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "积分礼品修改-失败e",
                    message_code = "modify_goods_exception"
                });
            }
        }



        [HttpPost]
        [Description("验证码列表")]
        [Route("ValidateCodeRecord")]
        [ActionName("ValidateCodeRecord")]
        public async Task<IActionResult> ValidateCodeRecord(ValidateCodeRecord x)
        {
            x.admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("游戏管理列表")]
        [Route("GameControl")]
        [ActionName("游戏管理列表")]
        public async Task<IActionResult> GameControl(GameControl x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("代付管理")]
        [Route("DirectPayGatewayList")]
        [ActionName("代付管理")]
        public async Task<IActionResult> DirectpayList(DirectPaymentGatewayList x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("USDT代付管理")]
        [Route("DirectPaymentGatewayUsdtList")]
        [ActionName("USDT代付管理")]
        public async Task<IActionResult> DirectPayGatewayUsdtList(DirectPaymentGatewayUsdtList x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("代付管理开关")]
        [Route("DirectPayGatewaySwitch")]
        [ActionName("代付管理开关")]
        public async Task<IActionResult> DirectPayGatewaySwitch(PaymentGatewaySwitchV2 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("代付限额")]
        [Route("DirectPayMaxAmountList")]
        [ActionName("代付限额")]
        public async Task<IActionResult> DirectPayMaxAmount(DirectPayMaxAmountList x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("代付平台表")]
        [Route("DirectPaySettingsList")]
        [ActionName("代付平台表")]
        public async Task<IActionResult> DirectPaySettings()
        {
            var result = await _mediator.Send(new DirectPaySettingList());
            return await Task.FromResult(Ok(result));
        }

        #region INR代付页面 配置

        [HttpPost]
        [Description("代付平台表INR")]
        [Route("DirectPaySettingsListINR")]
        [ActionName("代付平台表INR")]
        public async Task<IActionResult> DirectPaySettingsINR() {
            var result = await _mediator.Send(new DirectPaySettingListINR());
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("代付临时开关INR")]
        [Route("SaveDirectPayType")]
        [ActionName("代付临时开关INR")]
        public async Task<IActionResult> SaveDirectPayType(DirectPayTypeSwitch x) {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("代付管理开关INR")]
        [Route("DirectPayGatewaySwitchINR")]
        [ActionName("代付管理开关INR")]
        public async Task<IActionResult> DirectPayGatewaySwitchINR(PaymentGatewaySwitchV2INR x) {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("代付平台更新INR")]
        [Route("DirectPaySettingsUpdateINR")]
        [ActionName("代付平台更新INR")]
        public async Task<IActionResult> DirectPaySettingsUpdateINR(DirectPaySettingsUpdateV2 x) {
            x.currencyCode = "356";
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("汇率设置INR【更新】")]
        [Route("SetExchangeRateINR")]
        [ActionName("汇率设置INR【更新】")]
        public async Task<IActionResult> SetExchangeRateINR(SetExchangeRate x) {
            x.currencyCode = "356";
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        #endregion

        [HttpPost]
        [Description("汇率设置")]
        [Route("ExchangeRate")]
        [ActionName("汇率设置")]
        public async Task<IActionResult> ExchangeRate(ExchangeRate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("汇率设置【更新】")]
        [Route("SetExchangeRate")]
        [ActionName("汇率设置【更新】")]
        public async Task<IActionResult> SetExchangeRate(SetExchangeRate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("代付平台更新")]
        [Route("DirectPaySettingsUpdate")]
        [ActionName("代付平台更新")]
        public async Task<IActionResult> DirectPaySettingsUpdate(DirectPaySettingsUpdateV2 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("设置代付限额")]
        [Route("SetDirectPayMaxAmount")]
        [ActionName("设置代付限额")]
        public async Task<IActionResult> SetDirectPayMaxAmount(SetDirectPayMaxAmountV2 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("网站参数")]
        [Route("WebVariables")]
        [ActionName("设置网站参数")]
        public async Task<IActionResult> V(WebVariables x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        
        [HttpPost]
        [Description("网站参数更新")]
        [Route("WebVariablesUpdate")]
        [ActionName("设置网站参数")]
        public async Task<IActionResult> VUpdate(WebVariablesUpdate x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }




        [HttpPost]
        [Description("导入禁止提款")]
        [Route("IWRA")]
        [ActionName("导入禁止提款")]
        public async Task<IActionResult> ImportWithdrawRestrictedAccountt(ImportWithdrawRestrictedAcct x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }




    }
}
