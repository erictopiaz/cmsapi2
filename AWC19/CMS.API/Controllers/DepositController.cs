using AutoMapper;
using CMS.Core;
using CMS.Core.Application.存款管理.Queries;
using CMS.Core.Application.存款管理.Transact;
using CMS.Core.Application.提款管理.Queries;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UtilsLib.JWT;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]

    public class DepositController : BaseCtrl
    {
        public DepositController(IMapper mapper, IMediator mediator, ILogger<AccountController> logger,
            WriteToFile writeToFile)
    : base(mapper, mediator, logger) {
        }



        //[HttpPost]
        //[Description("对外内充值")]
        //[Route("ExternalThirdpayLink")]
        //[AllowAnonymous]
        //[ActionName("对外内充值")]
        //public async Task<IActionResult> ThirdpayLink(GetThirdpayLinkRequest x)
        //{
        //    x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
        //    var result = await _mediator.Send(x);
        //    return await Task.FromResult(Ok(result));
        //}



        [HttpPost]
        [Description("互转列表")]
        [Route("CrossTransact")]
        [ActionName("互转列表")]
        public async Task<IActionResult> CrossTransact(CrossTransactList x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("互转支付宝")]
        [Route("CrossTransactAlipay")]
        [ActionName("互转支付宝")]
        public async Task<IActionResult> CrossTransactAlipay(CrossTransactListAlipay x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("互转微信")]
        [Route("CrossTransactWechat")]
        [ActionName("互转微信")]
        public async Task<IActionResult> CrossTransactWehchat(CrossTransactListWechat x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("获取第三方链接")]
        [Route("ThirdpayLink")]
        [ActionName("获取第三方链接")]
        public async Task<IActionResult> ThirdpayLink(GetThirdpayLinkRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [HttpGet]
        [Description("银行存款截图")]
        [Route("BankDepositScrShot")]
        [ActionName("银行存款截图")]
        public async Task<IActionResult> BankDepositScrShot()
        {
            var stream = new StreamReader(Request.BodyReader.AsStream());
            string x = stream.ReadToEnd();
            WebClient ex = new WebClient();
            var result = ex.DownloadString(string.Concat("https://awcpt-storage.azurewebsites.net/lmessage/", x, ".txt"));
            //result = result.Replace("data:image/png;base64,", "");
            return await Task.FromResult(Ok(result));
        }


        [HttpPost] 
        [Description("网银存款列表")]
        [Route("NetList")]
        [ActionName("网银-网银存款列表")]
        public async Task<IActionResult> 网银存款列表(网银列表 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("内充列表")]
        [Route("DepositInternal")]
        [ActionName("内充列表")]
        public async Task<IActionResult> 内充列表(内充列表 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("内充取消")]
        [Route("Internal/Cancel")]
        [ActionName("内充取消")]
        public async Task<IActionResult> 内充取消(InternalCancel x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }
        
        [HttpPost]
        [Description("内充补单")]
        [Route("Internal/Verify")]
        [ActionName("内充补单")]
        public async Task<IActionResult> 内充补单(InternalVerify x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("第三方存款列表")]
        [Route("ThirdpayList")]
        [ActionName("第三方行-第三方存款列表")]
        public async Task<IActionResult> 第三方存款列表(第三方Query x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("第三方存款列表【代理】")]
        [Route("ThirdpayAgentList")]
        [ActionName("第三方行-第三方存款列表【代理】")]
        public async Task<IActionResult> 第三方代理存款列表(第三方代理Query x) {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("第三方超一个月")]
        [Route("ThirdpayListNoLimit")]
        [ActionName("第三方超一个月")]
        public async Task<IActionResult> 第三方超一个月(第三方超一个月 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("第三方平台列表")]
        [Route("ThirdpayPlatforms")]
        [ActionName("第三方平台列表")]
        public async Task<IActionResult> 第三方平台列表(第三方平台 x)
        {
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("审核网银请求")]
        [Route("VerifyRequest")]
        [ActionName("审核网银请求")]
        public async Task<IActionResult> VerifyRequest(Core.Application.存款管理.Transact.VerifyDepositRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }




        [HttpPost]
        [Description("互转审核")]
        [Route("CrossTransactVerify")]
        [ActionName("审核网银请求")]
        public async Task<IActionResult> CrossTransactCancel(Core.Application.存款管理.Transact.CrossTransactVerify x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


        [HttpPost]
        [Description("互转取消")]
        [Route("CrossTransactCancel")]
        [ActionName("互转取消")]
        public async Task<IActionResult> CrossTransactCancel(Core.Application.存款管理.Transact.CrossTransactCancel x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }




        [HttpPost]
        [Description("补单请求")]
        [Route("VerifyFailedRequest")]
        [ActionName("补单请求")]
        public async Task<IActionResult> VerifyFailedRequest(Core.Application.存款管理.Transact.VerifyFailedDepositRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("补单请求改金额 ")]
        [Route("VerifyWithAlterAmountFailedRequest")]
        [ActionName("补单请求改金额")]
        public async Task<IActionResult> VerifyFailedRequestWithAlterAmount(Core.Application.存款管理.Transact.VerifyFailedDepositRequestWithAlterAmount x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        [Description("微调定单金额 ")]
        [Route("ResizeAmount")]
        [ActionName("微调定单金额")]
        public async Task<IActionResult> ResizeAmount(Core.Application.存款管理.Transact.ResizeAmount x) {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }



        [HttpPost]
        [Description("取消网银请求")]
        [Route("CancelRequest")]
        [ActionName("取消网银请求")]
        public async Task<IActionResult> CancelRequest(Core.Application.存款管理.Transact.CancelDepositRequest x)
        {
            x.Admin = Common.Security.DESDeCode_Name(User.Identity.Name);
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }

    }
}
