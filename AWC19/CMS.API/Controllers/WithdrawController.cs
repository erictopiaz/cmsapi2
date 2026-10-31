using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CMS.Core;
using UtilsLib.JWT;
using LogLib.Helper;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]

    public class WithdrawController : BaseCtrl
    {
        private readonly WriteToFile _writeToFile;
        public WithdrawController(IMapper mapper,
            IMediator mediator,
            ILogger<AccountController> logger,
            WriteToFile writeToFile) : base(mapper, mediator, logger)
        {
            _writeToFile = writeToFile;
        }



        [HttpPost]
        [Description("普通等级列表, 只是会列出大于代付限制以上的请求")]
        [Route("RegularUserRecord")]
        [ActionName("提款-普通会员表格")]
        public async Task<IActionResult> RegularUserRecord(Core.Application.提款管理.Queries.普通会员Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("新会员列表, 只是会列出大于代付限制以上的请求")]
        [Route("NewUserRecord")]
        [ActionName("提款-新会员列表")]
        public async Task<IActionResult> NewUserRecord(Core.Application.提款管理.Queries.新会员Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }
        
        [HttpPost]
        [Description("新会员列表, 只是会列出大于代付限制以上的请求")]
        [Route("NewUserDirectPayRequest")]
        [ActionName("新会员代付列表")]
        public async Task<IActionResult> NewUserDirectPayRequest(Core.Application.提款管理.Queries.新会员DirectPayQuery x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

         

        [HttpPost]
        [Description("VIP(1)等级列表, 只是会列出大于代付限制以上的请求")]
        [Route("VIP1Record")]
        [ActionName("提款-VIP1组列表")]
        public async Task<IActionResult> VIP1Record(Core.Application.提款管理.Queries.VIP1Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("手动转卡")]
        [Route("ManualPayRecord")]
        [ActionName("提款-手动转卡")]
        public async Task<IActionResult> VIP1Record(Core.Application.提款管理.Queries.ManualPayQuery x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [Description("推给卡付组")]
        [Route("TransferToManualPay")]
        [ActionName("推给卡付组")]
        public async Task<IActionResult> TransferToManualPay(Core.Application.提款管理.Queries.TransferToManualPay x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("推给风控组")]
        [Route("TransferToRiskManagement")]
        [ActionName("推给风控组")]
        public async Task<IActionResult> TransferToRiskMgmt(Core.Application.提款管理.Queries.TransferToRiskManagement x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("VIP(2)等级列表, 只是会列出大于代付限制以上的请求")]
        [Route("VIP2Record")]
        public async Task<IActionResult> VIP2Record(Core.Application.提款管理.Queries.VIP2Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("VIP(2)代付 等级列表")]
        [Route("VIP2DirectPayRecord")]
        public async Task<IActionResult> VIP2DirectPayQuery(Core.Application.提款管理.Queries.VIP2DirectPayQuery x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("VIP(3)代付 等级列表")]
        [Route("VIP3DirectPayRecord")]
        public async Task<IActionResult> VIP3DirectPayQuery(Core.Application.提款管理.Queries.VIP3DirectPayQuery x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("VIP(3)等级列表, 只是会列出大于代付限制以上的请求")]
        [Route("VIP3Record")]
        public async Task<IActionResult> VIP3Record(Core.Application.提款管理.Queries.VIP3Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("USDT代付 等级列表")]
        [Route("USDTDirectPayRecord")]
        public async Task<IActionResult> USDTDirectPayQuery(Core.Application.提款管理.Queries.USDTDirectPayQuery x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("支付宝/微信代付 等级列表")]
        [Route("QRManualPay")]
        [ActionName("支付宝/微信代付查询")]
        public async Task<IActionResult> QRManualPayQuery(Core.Application.提款管理.Queries.USDTDirectPayQuery x) {
            x.paycategory = (x.paycategory == 19 || x.paycategory == 20) ? x.paycategory : -2;//-2决定 查询 支付宝和微信代付 一起返回 
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("VIP(4)等级列表, 只是会列出大于代付限制以上的请求")]
        [Route("VIP4Record")]
        public async Task<IActionResult> VIP4Record(Core.Application.提款管理.Queries.VIP4Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        //[HttpPost]
        //[Description("代付提款所有的在代付金额范围里")]
        //[ActionName("代付提款申请(所有)")]
        //[Route("DirectPayRequest")]
        //public async Task<IActionResult> DirectPayRequest(Core.Application.提款管理.Queries.代付列表所有Query x)
        //{
        //    x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
        //    var result = await _mediator.Send(x);
        //    return Ok(result);
        //}


        [HttpPost]
        [Description("代付提款普通玩家 - 的在代付金额范围里")]
        [ActionName("代付提款申请(所有)")]
        [Route("AllUserDirectPayRequest")]
        public async Task<IActionResult> AllUserDirectPayRequest(Core.Application.提款管理.Queries.代付列表所有Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("代付提款普通玩家 - 的在代付金额范围里")]
        [ActionName("代付提款申请(普通玩家)")]
        [Route("RegularUserDirectPayRequest")]
        public async Task<IActionResult> RegularUserDirectPayRequests(Core.Application.提款管理.Queries.代付申请普通Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [Description("代付提款VIP玩家 - 的在代付金额范围里")]
        [ActionName("代付提款申请(所有)")]
        [Route("VipUserDirectPayRequest")]
        public async Task<IActionResult> VipUserDirectPayRequest(Core.Application.提款管理.Queries.代付申请VIPQuery x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("VIP提款列表")]
        [Description("VIP提款玩家")]
        [Route("VipUserWithdrawRequest")]
        public async Task<IActionResult> VipUserWithdrawRequest(Core.Application.提款管理.Queries.VIP提款列表Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("大额VIP列表")]
        [Description("大额VIP列表")]
        [Route("VipBigWithdrawRequest")]
        public async Task<IActionResult> VipBigWithdrawRequest(Core.Application.提款管理.Queries.大额VIP列表 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("大额VIP3列表")]
        [Description("大额VIP3列表")]
        [Route("Vip3BigWithdrawRequest")]
        public async Task<IActionResult> Vip3BigWithdrawRequest(Core.Application.提款管理.Queries.大额VIP3列表 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("大额普通(列表)")]
        [Description("大额普通列表")]
        [Route("RegularBigWithdrawRequest")]
        public async Task<IActionResult> RegularBigWithdrawRequest(Core.Application.提款管理.Queries.大额普通列表 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [ActionName("未到账状态(更新)")]
        [Description("未到账状态(更新)")]
        [Route("PaymentNotReceivedUpdate")]
        public async Task<IActionResult> PaymentNotReceivedUpdate(Core.Application.提款管理.Queries.PaymentNotReceivedUpdate x)
        {
            //x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        //[HttpPost]
        //[Description("VIP(1)等级列表, 只是会列出大于代付限制以上的请求")]
        //[Route("VIP1Record")]
        //[ActionName("提款-VIP1组列表")]
        //public async Task<IActionResult> VIP1Record(Core.Application.提款管理.Queries.VIP1Query x)
        //{
        //                x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
        //    var result = await _mediator.Send(x);
        //    return Ok(result);
        //}



        [HttpPost]
        [Description("锁住一个提款请求，才可以进行【手动支付，代付】")]
        [Route("LockWithdrawRequest")]
        [ActionName("提款-锁提款")]
        public async Task<IActionResult> LockWithdrawRequest(Core.Application.提款管理.Queries.LockWithdrawRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }




        [HttpPost]
        [ActionName("手动支付")]
        [Description("手动支付")]
        [Route("ManualPay")]
        public async Task<IActionResult> ManualPay(Core.Application.提款管理.Queries.ManualPayRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("进行代付【*】")]
        [Description("进行代付,按照后台启用的第三方来进行代付")]
        [Route("DirectPay")]
        public async Task<IActionResult> DirectPay(Core.Application.提款管理.Queries.DirectPayRequestV2 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            x.writeToFile = _writeToFile;
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("代理代付【*】")]
        [Description("代理进行代付")]
        [Route("DirectPayAffiliate")]
        public async Task<IActionResult> DirectPayAffiliate(Core.Application.提款管理.Queries.DirectPayAffiliateV2 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        //[HttpPost]
        //[ActionName("代付")]
        //[Description("进行代付,按照后台启用的第三方来进行代付")]
        //[Route("DirectPayTest")]
        //public async Task<IActionResult> DirectPayTest(Core.Application.提款管理.Queries.DirectPayRequestTest x)
        //{
        //    x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
        //    var result = await _mediator.Send(x);
        //    return Ok(result);
        //}



        [HttpPost]
        [ActionName("代付")]
        [Description("进行代付,按照后台启用的第三方来进行代付")]
        [Route("DiscloseAmount")]
        public async Task<IActionResult> DiscloseAmount(Core.Application.提款管理.Queries.提款金额查询 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        
        [HttpPost]
        [ActionName("显示卡号【点选】")]
        [Description("点选可以显示卡号")]
        [Route("DiscloseCardno")]
        public async Task<IActionResult> DiscloseCardno(Core.Application.提款管理.Queries.提款卡号查询 x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [ActionName("取消提款")]
        [Description("取消提款去请求")]
        [Route("CancelRequest")]
        public async Task<IActionResult> CancelRequest(Core.Application.提款管理.Queries.CancelPayRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("提款详细")]
        [Description("提款详细")]
        [Route("Details")]
        public async Task<IActionResult> CancelRequest(Core.Application.提款管理.Queries.提款详细Query x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("提款流水详细")]
        [Description("提款流水详细")]
        [Route("VerifyBetData")]
        [AllowAnonymous]
        public async Task<IActionResult> VerfiyWithdrawBetRequest(Core.Application.提款管理.Queries.提款流水详细Query x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [ActionName("代付详细")]
        [Description("代付详细")]
        [Route("PaymentLogRecord")]
        public async Task<IActionResult> PaymentLog(Core.Application.提款管理.Queries.代付列表所有Query/*代付详细列表*/ x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }
        [HttpPost]
        [ActionName("支付记录")]
        [Description("支付记录")]
        [Route("PaymentRecord")]
        public async Task<IActionResult> PaymentRecord(Core.Application.提款管理.Queries.支付记录Query/*代付详细列表*/ x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("审核提款请求")]
        [Description("取消提款去请求")]
        [Route("ApproveRequest")]
        public async Task<IActionResult> ApproveRequest(Core.Application.提款管理.Queries.ApproveRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("切割提款金额")]
        [Description("切割提款金额")]
        [Route("SplitWithdrawOrder")]
        public async Task<IActionResult> SplitWithdrawOrder(Core.Application.提款管理.Queries.SplitWithdrawOrder x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("回撤提款请求")]
        [Description("回撤提款请求")]
        [Route("ReturnOrder")]
        public async Task<IActionResult> ReturnOrder(Core.Application.提款管理.Queries.ReturnFunds x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("转极速定额")]
        [Description("转极速定额")]
        [Route("TransferToFix")]
        public async Task<IActionResult> TransferToFix(Core.Application.提款管理.Queries.TransferToFix x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("显示提款列表")]
        [Route("AllRecord")]
        [ActionName("提款-所有提款请求")]
        public async Task<IActionResult> AllRecord(Core.Application.提款管理.Queries.AllRecord x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("查询待审核单数")]
        [Route("QueryWithdrawOrders")]
        [ActionName("查询待审核单数")]
        public async Task<IActionResult> QueryWithdrawOrders(Core.Application.提款管理.Queries.WithdrawOrders x) {
            //x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("显示提款列表【代理】")]
        [Route("AllAgentPlayerRecord")]
        [ActionName("提款-所有提款请求【代理】")]
        public async Task<IActionResult> AllAgentPlayerRecord(Core.Application.提款管理.Queries.AllAgentPlayerRecord x) {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }

        [HttpPost]
        [Description("显示风控列表")]
        [Route("RiskMgmtRecord")]
        [ActionName("提款-风控列表")]
        public async Task<IActionResult> RiskMgmtRecord(Core.Application.提款管理.Queries.RiskMgmtRecord x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }
        
        
        [HttpPost]
        [Description("互转未处理单子")]
        [Route("CrossTransactPendingWithdraw")]
        [ActionName("提款-互转未处理单子")]
        public async Task<IActionResult> CrossTransactPendingWithdraw(Core.Application.提款管理.Queries.CrossTransactPendingQuery x)
        {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }




        [HttpPost]
        [Description("互转微信未处理单子")]
        [Route("CrossTransactWechatPendingWithdraw")]
        [ActionName("提款-互转微信未处理单子")]
        public async Task<IActionResult> CrossTransactWechatPendingQuery(Core.Application.提款管理.Queries.CrossTransactWechatPendingQuery x)
        {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


        [HttpPost]
        [Description("互转支付宝未处理单子")]
        [Route("CrossTransactAlipayPendingWithdraw")]
        [ActionName("提款-互转支付宝未处理单子")]
        public async Task<IActionResult> CrossTransactAlipayPendingWithdraw(Core.Application.提款管理.Queries.CrossTransactAlipayPendingQuery x)
        {
            //x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name).ToLower().Trim();
            var result = await _mediator.Send(x);
            return Ok(result);
        }
    }
}
