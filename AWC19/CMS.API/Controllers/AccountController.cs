using Amazon.Runtime.Internal.Util;
using AutoMapper;
using AWC2019.Core.Application.Account.Queries;
using Ching.DistributedTransaction.EFModel;
using CMS.API.BLL.Account;
using CMS.Core;
using CMS.Core.Application.Account.Queries;
using CMS.Core.Application.账号管理.Queries;
using CMS.Core.Application.账号管理.Transact;
using CMS.Data;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using ShareEntity.InputModels.Account;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.Cache;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;
using Common = CMS.Core.Common;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class AccountController : BaseCtrl
    {

        //private IHostingEnvironment _env;
        private WriteToDB _writeToDB;
        private IHttpContextAccessor _accessor;
        private readonly WriteToFile _writeToFile;
        private IMemoryCache _cache;
        private IConfiguration _config;
        public AccountController(IMapper mapper, IMemoryCache memoryCache, 
            IMediator mediator, ILogger<AccountController> logger, 
            //IHostingEnvironment env,
            WriteToDB writeToDB, IConfiguration config,
            IHttpContextAccessor httpContextAccessor, WriteToFile writeToFile)
        : base(mapper, mediator, logger) {
            //_env = env;
            this._writeToDB = writeToDB;
            this._accessor = httpContextAccessor;
            this._writeToFile = writeToFile;
            this._cache = memoryCache;
            this._config = config;
        }


        [HttpPost]
        [Description("账号搜索")]
        [Route("SearchAccount")]
        [ActionName("会员列表")]
        public async Task<IActionResult> 账号搜索(账号搜索 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("会员基本信息【查询】")]
        [Route("SearchUserBaseInfo")]
        [ActionName("会员基本信息【查询】")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<SearchUserBaseInfoReturn>), 200)]
        public async Task<IActionResult> SearchUserBaseInfo(SearchUserBaseInfo x)
        {
            x.Admin =  Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("会员本月数据【查询】")]
        [Route("SearchUserMonthlyData")]
        [ActionName("会员本月数据【查询】")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<SearchUserMonthlyDataReturn>), 200)]
        public async Task<IActionResult> SearchUserMonthlyData(SearchUserMonthlyData x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("会员绑卡数据【查询】")]
        [Route("SearchUserBindCards")]
        [ActionName("会员绑卡数据【查询】")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<List<SearchUserBindCardsReturn>>), 200)]
        public async Task<IActionResult> SearchUserBindCards(SearchUserBindCards x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("会员登录分布【查询】")]
        [Route("SearchUserLoginChart")]
        [ActionName("会员登录分布【查询】")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<List<SearchUserLoginChartReturn>>), 200)]
        public async Task<IActionResult> SearchUserLoginChart(SearchUserLoginChart x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("会员事件时间点【查询】")]
        [Route("SearchUserEventTime")]
        [ActionName("会员事件时间点【查询】")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<List<SearchUserEventTimeReturn>>), 200)]
        public async Task<IActionResult> SearchUserEventTime(SearchUserEventTime x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("关联搜索")]
        [Route("RelateSearch")]
        [ActionName("关联搜索")]
        public async Task<IActionResult> RelateSearch(关联搜索 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("首存账号列表")]
        [Route("InitialDepositAccounts")]
        [ActionName("首存用户(列表)")]
        public async Task<IActionResult> 首存账号列表(首存账号 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("存款账号列表")]
        [Route("DepositorAccounts")]
        [ActionName("存款用户(列表)")]
        public async Task<IActionResult> 存款账号列表(存款账号 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("邮件发送列表")]
        [Route("SendMail")]
        [ActionName("邮件发送列表")]
        public async Task<IActionResult> 邮件发送列表(邮件发送列表 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("邮件发送-新增")]
        [Route("CreateSendMail")]
        [ActionName("邮件发送-新增")]
        public async Task<IActionResult> 新增邮件(邮件发送_新增 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("邮件发送-导入")]
        [Route("ImportSendMail")]
        [ActionName("邮件发送-导入")]
        public async Task<IActionResult> 导入邮件(邮件发送_导入 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("邮件发送-发送")]
        [Route("SetMailToSend")]
        [ActionName("邮件发送-发送")]
        public async Task<IActionResult> 发送邮件(邮件发送_改状态 x)
        {
            x.status = 2;
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("邮件发送-取消")]
        [Route("CancelSendMail")]
        [ActionName("邮件发送-取消")]
        public async Task<IActionResult> 取消邮件(邮件发送_改状态 x)
        {
            x.status = 3;
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("账户存款列表")]
        [Route("Depositors")]
        [ActionName("账户存款列表")]
        public async Task<IActionResult> Depositors(账户存款列表 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [AllowAnonymous]
        [Description("TEST")]
        [Route("TEST")]
        [ActionName("TEST")]
        public async Task<IActionResult> TEST(TestFluent x)
        {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();

            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("账号详细")]
        [Route("AccountDetails")]
        [ActionName("会员资料")]
        public async Task<IActionResult> AccountDetails(账号详细Query x)
        {

            var nameJson = Common.Security.DESDeCode_NameJson(User.Identity.Name);
            if (nameJson.ContainsKey("Account")) {
                x.Admin = nameJson["Account"];
                x.allowedToManageAgent = Convert.ToInt32(nameJson["allowedToManageAgent"]);
            }
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("账号优惠禁止")]
        [Route("AccountPromoDisable")]
        [ActionName("账号优惠禁止【更新】")]
        public async Task<IActionResult> AccountDetails(账号优惠禁止 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("账号详细更新")]
        [Route("AccountDetailsUpdate")]
        [ActionName("会员资料【更新】")]
        public async Task<IActionResult> AccountDetails(账号详细Update x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("账号登入解锁")]
        [Route("AccountLoginUnlock")]
        [ActionName("账号登入解锁【更新】")]
        public async Task<IActionResult> AccountLoginUnlock(AccountLoginUnlock x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("账号状态更新")]
        [Route("AccountStatusUpdate")]
        [ActionName("会员状态【更新】")]
        public async Task<IActionResult> AccountStatusUpdate(账号详细Update x)
        {
            var result = await _mediator.Send(new 账号详细Update()
            {
                id = x.id,
                status = x.status,
                Account = x.Account
            });
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("交易记录")]
        [Route("AccountTransactionDetails")]
        [ActionName("交易记录")]
        public async Task<IActionResult> 账号交易记录(交易记录 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("交易记录")]
        [Route("AccountTransactionDetails/V2")]
        [ActionName("交易记录")]
        public async Task<IActionResult> 账号交易记录(交易记录V2 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("等级List")]
        [Route("LevelList")]
        [ActionName("等级表格【*比购*】")]
        public async Task<IActionResult> 等级List(等级List x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("账号等级")]
        [Route("AccountLevel")]
        [ActionName("账户等级(列表)")]
        public async Task<IActionResult> AccountDetails(等级列表 x) {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("等级编辑")]
        [Route("LevelEdit")]
        [ActionName("会员等级【更新】")]
        public async Task<IActionResult> 等级编辑(Core.Application.账号管理.Transact.等级Edit x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新玩家电话")]
        [Route("UpdateAccountTelephone")]
        [ActionName("会员电话【更新】")]
        public async Task<IActionResult> UpdateAccountTelephone(Core.Application.账号管理.Transact.电话更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);

            var client = HttpHelper.GetClient(_accessor);
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.Account = AccountBLL.QueryAccountById(Convert.ToInt64(x.id));
            
            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"更改会员{0}手机号", x.Account),
                logresult = 1,
                account = x.Admin,
                logtype = 15,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.Account,
                logoDetail = JsonConvert.SerializeObject(new {
                    account = x.Account,
                    phone = x.telephone,
                    result = JsonConvert.SerializeObject(result)
                })
            });
            result.prePhone = "";
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新OPENVPN状态")]
        [Route("UpdateOpenvpn")]
        [ActionName("更新OPENVPN状态")]
        public async Task<IActionResult> UpdateOpenvpn(Core.Application.账号管理.Transact.VPN开关 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("更新存款条件")]
        [Route("UpdateDepositRules")]
        [ActionName("会员存款条件【更新】")]
        public async Task<IActionResult> UpdateDepositRules(Core.Application.账号管理.Transact.更新存款条件 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新标记")]
        [Route("UpdateTags")]
        [ActionName("会员标记【更新】")]
        public async Task<IActionResult> UpdateUserTags(Core.Application.账号管理.Transact.更新标记 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("更新实名存款验证")]
        [Route("UpdateIsNeedCheckRealNameRules")]
        [ActionName("实名存款验证【更新】")]
        public async Task<IActionResult> UpdateIsNeedCheckRealNameRules(Core.Application.账号管理.Transact.实名存款更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("限制定额提款")]
        [Route("UpdateIsLimitFixWithdrawalRules")]
        [ActionName("限制定额提款【更新】")]
        public async Task<IActionResult> UpdateIsLimitFixWithdrawalRules(Core.Application.账号管理.Transact.定额提款限制更新 x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("更新是否参与推荐结算")]
        [Route("UpdateSpreadBilledRules")]
        [ActionName("参与推荐结算【更新】")]
        public async Task<IActionResult> UpdateSpreadBilledRules(Core.Application.账号管理.Transact.参与推荐结算 x)
        {
            var client = HttpHelper.GetClient(_accessor);
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            x.account = AccountBLL.QueryAccountById(x.id);
            var remark = x.isLimitSpreadBilled == 0 ? "不参与" : "参与";
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"更改会员{0}推荐荐好友结算状态为{1}", x.account, remark),
                logresult = 1,
                account = x.Admin,
                logtype = 19,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("IP/指纹黑名单【加锁】")]
        [Route("lockIPs")]
        [ActionName("IP/指纹黑名单【加锁】")]
        public async Task<IActionResult> LockIPs(CMS.Core.Application.系统管理.Transact.黑白名单更新 x) {
            var client = HttpHelper.GetClient(_accessor);
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.type = 0;
            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"会员黑名单加锁"),
                logresult = 1,
                account = x.Admin,
                logtype = 22,
                accounttype = 1,
                actionType = 1,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("IP/指纹黑名单【解锁】")]
        [Route("unLockIPs")]
        [ActionName("IP/指纹黑名单【解锁】")]
        public async Task<IActionResult> UnLockIPs(CMS.Core.Application.系统管理.Transact.黑白名单更新 x) {
            var client = HttpHelper.GetClient(_accessor);
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.type = 1;
            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"会员黑名单解锁"),
                logresult = 1,
                account = x.Admin,
                logtype = 22,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新玩家邮箱")]
        [Route("UpdateAccountEmail")]
        [ActionName("会员邮箱【更新】")]
        public async Task<IActionResult> UpdateAccountEmail(Core.Application.账号管理.Transact.邮箱更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新玩家生日")]
        [Route("UpdateAccountBirthday")]
        [ActionName("会员生日【更新】")]
        public async Task<IActionResult> UpdateAccountBirthday(Core.Application.账号管理.Transact.生日更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Description("更新玩家密码【】")]
        //[Route("UpdateAccountPassword")]
        //[ActionName("会员密码【更新】")]
        //public async Task<IActionResult> UpdateAccountPassword(Core.Application.账号管理.Transact.密码更新 x)
        //{
        //    x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
        //    var result = await _mediator.Send(x);
        //    return await Task.FromResult(Ok(result));
        //}

        [HttpPost]
        [Description("更新玩家密码【新】")]
        [Route("NewUpdateAccountPassword")]
        [ActionName("会员密码更新【新】")]
        public async Task<IActionResult> NewUpdateAccountPassword(Core.Application.账号管理.Transact.密码更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("更新玩家名称")]
        [Route("UpdateAccountRealname")]
        [ActionName("会员姓名【更改】")]
        public async Task<IActionResult> UpdateAccountRealname(Core.Application.账号管理.Transact.姓名更新 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        [HttpPost]
        [Description("更新玩家加码比例")]
        [Route("UpdateBetCodeXRate")]
        [ActionName("会员加码比例【更改】")]
        public async Task<IActionResult> UpdateBetCodeXRate(Core.Application.账号管理.Transact.更新会员加码比例 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);

            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"更改会员{0}的加码比例", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 13,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("关闭会员指定商户")]
        [Route("DisableMids")]
        [ActionName("关闭会员指定商户")]
        public async Task<IActionResult> DisableMids(CMS.Core.Application.账号管理.Transact.关闭会员商户 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);
            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"关闭会员{0}商户,商户ids:{1}", x.account, string.IsNullOrEmpty(x.disableMids) ?"": x.disableMids.Replace("'", "''")),
                logresult = 1,
                account = x.Admin,
                logtype = 18,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新会员充值实名列表")]
        [Route("EableDepositName")]
        [ActionName("更新会员充值实名列表")]
        public async Task<IActionResult> EableDepositName(CMS.Core.Application.账号管理.Transact.更新会员充值实名 x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);
            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"更新会员充值实名{0}", string.IsNullOrEmpty(x.realNames) ? "" : x.realNames.Replace("'", "''")),
                logresult = 1,
                account = x.Admin,
                logtype = 25,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("开启会员选择商户")]
        [Route("EnableMids")]
        [ActionName("开启会员选择商户")]
        public async Task<IActionResult> EnableMids(CMS.Core.Application.账号管理.Transact.会员开启商户 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);

            var result = await _mediator.Send(x);
            var mids = (x.openMids != null && x.openMids.Count > 0) ? JsonConvert.SerializeObject(x.openMids) : JsonConvert.SerializeObject(new List<int>());
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"关闭会员{0}商户,商户ids:{1}", x.account, mids),
                logresult = 1,
                account = x.Admin,
                logtype = 18,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("配置存款优惠")]
        [Route("UpdateDepositPromo")]
        [ActionName("配置存款优惠【更改】")]
        public async Task<IActionResult> UpdateDepositPromo(CMS.Core.Application.优惠管理.Transact.配置存款优惠 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);

            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"配置会员{0}存款优惠", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 13,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("风控状态修改")]
        [Route("UpdateRiskUser")]
        [ActionName("风控状态修改")]
        public async Task<IActionResult> UpdateRiskUser(CMS.Core.Application.账号管理.Transact.会员风控状态修改 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);


            #region 防止重复使用账号-风控状态修改
            var key = string.Format(@"{0}UpdateRiskUser", x.account);
            string alreadyKey = CacheHelper.GetCache(_cache, key);
            if (!string.IsNullOrEmpty(alreadyKey)) {
                return Ok(new { code = 0, message = $"重复操作会员账号({x.account})，稍后再试...", message_code = "cli_1050" });
            }
            if (!CacheHelper.SetCacheWithCheckKey(_cache, key, x.account, 4))
                return Ok(new { code = 0, message = "准备操作,失败!" + x.account, message_code = "cli_1054" });

            #endregion


            var result = await _mediator.Send(x);

            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"风控状态修改{0}", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 13,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account,
                logoDetail = JsonConvert.SerializeObject(x)
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("VIP状态修改")]
        [Route("UpdateVIPStatus")]
        [ActionName("VIP状态修改")]
        public async Task<IActionResult> UpdateVIPStatus(CMS.Core.Application.账号管理.Transact.会员VIP状态修改 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);


            #region 防止重复使用账号-风控状态修改
            var key = string.Format(@"{0}UpdateVIPStatus", x.account);
            string alreadyKey = CacheHelper.GetCache(_cache, key);
            if (!string.IsNullOrEmpty(alreadyKey))
            {
                return Ok(new { code = 0, message = $"重复操作会员账号({x.account})，稍后再试...", message_code = "cli_1050" });
            }
            if (!CacheHelper.SetCacheWithCheckKey(_cache, key, x.account, 4))
                return Ok(new { code = 0, message = "准备操作,失败!" + x.account, message_code = "cli_1054" });

            #endregion


            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"VIP状态修改{0}", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 13,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account,
                logoDetail = JsonConvert.SerializeObject(x)
            });
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("定额限制修改")]
        [Route("UpdateFixAmountLimit")]
        [ActionName("定额限制修改")]
        public async Task<IActionResult> UpdateFixAmountLimit(CMS.Core.Application.账号管理.Transact.定额限制修改 x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);
            var result = await _mediator.Send(x);

            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"定额限制修改{0}", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 0,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account,
                logoDetail = JsonConvert.SerializeObject(x)
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("银行卡单笔提款区间限制")]
        [Route("UpdateWithdrawAmountRange")]
        [ActionName("银行卡单笔提款区间限制")]
        public async Task<IActionResult> UpdateWithdrawAmountRange(CMS.Core.Application.账号管理.Transact.银行卡单笔提款区间限制 x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = string.IsNullOrEmpty(x.account) ? AccountBLL.QueryAccountById(x.id):x.account;
            var result = await _mediator.Send(x);

            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"银行卡单笔提款区间限制{0}", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 0,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account,
                logoDetail = JsonConvert.SerializeObject(x)
            });
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("定额提款限制修改")]
        [Route("UpdateFixWAmountLimit")]
        [ActionName("定额提款限制修改")]
        public async Task<IActionResult> UpdateFixWAmountLimit(CMS.Core.Application.账号管理.Transact.定额提款限制修改 x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);
            x.account = AccountBLL.QueryAccountById(x.id);
            var result = await _mediator.Send(x);

            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"定额提款限制修改{0}", x.account),
                logresult = 1,
                account = x.Admin,
                logtype = 0,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.account,
                logoDetail = JsonConvert.SerializeObject(x)
            });
            return await Task.FromResult(Ok(result));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("QueryBetCodeXRate")]
        public async Task<IActionResult> QueryBetCodeXRate(Core.Application.账号管理.Transact.查询会员加码比例 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            //var client = HttpHelper.GetClient(_accessor);

            var result = await _mediator.Send(x);
            
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("更新玩家代理")]
        [Route("ChangeUpAffaccount")]
        [ActionName("所属代理【更改】")]
        public async Task<IActionResult> ChangeUpAffaccount(Core.Application.账号管理.Transact.更新上级代理 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var client = HttpHelper.GetClient(_accessor);

            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel()
            {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"更改会员{0}的上级代理为{1}", x.Account, x.affaccount),
                logresult=1,
                account=x.Admin,
                logtype = 7,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = x.Account
            }) ;
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("查询账号操作日志")]
        [Route("QueryUserLog")]
        [ActionName("账号操作日志")]
        public async Task<IActionResult> QueryUserLog([FromBody] QueryUserLogInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
                var result = await AccountBLL.QueryUserLogRecords(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("QueryUserLog", "QueryUserLog ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code="false",
                    message = "日志记录加载-失败e",
                    message_code = "log_record_create_exception"
                });
            }
        }




        [HttpPost]
        [Description("删除提款卡")]
        [Route("DeleteWithdrawCard")]
        [ActionName("删除提款卡【更新】")]
        public async Task<IActionResult> DeleteWithdrawCard(Core.Application.账号管理.Transact.绑定卡删除 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("查询提款卡地址")]
        [Route("QueryWithdrawCardAddress")]
        [ActionName("查询提款卡地址【更新】")]
        public async Task<IActionResult> QueryWithdrawCardAddress(Core.Application.账号管理.Transact.卡查询 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("查询电话地址")]
        [Route("QueryTelephoneAddress")]
        [ActionName("查询电话地址【更新】")]
        public async Task<IActionResult> QueryTelephoneAddress(Core.Application.账号管理.Transact.电话查询 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("获取游戏金额")]
        [Route("GetGameBalance")]
        [ActionName("会员游戏金额【获取】")]
        public async Task<IActionResult> GetGameBalance(Core.Application.账号管理.Queries.获取游戏金额 x)
        {
            x.callSec = _config["game:callSec"];
            x.apiUrl = _config["game:apiUrl"];
            x.jwtEncoderKey = _config["jwt:jwtKey"];
            x.desEncodeKey = _config["jwt:pKeySec"];
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.writeToFile = _writeToFile;
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpGet]
        [Description("获取游戏金额")]
        [Route("GetGameBalanceNoAuth")]
        [AllowAnonymous]
        [ActionName("会员游戏金额【获取】")]
        public async Task<IActionResult> GetGameBalanceNoAuth(string account, string gamecode)
        {

            Core.Application.账号管理.Queries.获取游戏金额 x = new 获取游戏金额();
            x.Account = account;
            x.GameCode = gamecode;
            x.id = "";

            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("游戏金额重置")]
        [Route("ResetGameBalance")]
        [ActionName("游戏金额重置【更新】")]
        public async Task<IActionResult> ResetBalance(Core.Application.账号管理.Transact.游戏金额重置 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.writeToFile = _writeToFile;
            x.callSec = _config["game:callSec"];
            x.apiUrl = _config["game:apiUrl"];
            x.jwtEncoderKey = _config["jwt:jwtKey"];
            x.desEncodeKey = _config["jwt:pKeySec"];


            var client = HttpHelper.GetClient(_accessor);
            var result = await _mediator.Send(x);
            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = Common.Security.GuidToLong(),
                logmessage = string.Format(@"{0}", result.message),
                logresult = 1,
                account = x.Admin,
                logtype = 7,
                accounttype = 1,
                actionType = 2,
                ipaddress = client.ipaddress,
                relationAccount = result.account
            });

            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("更新游戏状态")]
        [Route("UpdateGameStatus")]
        [ActionName("更新游戏状态【更新】")]
        public async Task<IActionResult> GetGameBalance(Core.Application.账号管理.Transact.更新游戏状态 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("更改提款卡名称")]
        [Route("UpdateWithdrawCardAccountName")]
        [ActionName("更改提款卡名称【更新】")]
        public async Task<IActionResult> UpdateWithdrawCardAccountName(Core.Application.账号管理.Transact.更新卡名称 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


    }
}
