using AutoMapper;
using CMS.Core.Application.Admin.Queries;
using CMS.Core.Application.存款管理.Transact;
using CMS.Core.Application.系统管理.Queries;
using ShareEntity.OutputModels;
//using CMS.Filter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Route("api/Misc")]
    [Description("登入后台验证")]
    public class MiscController : BaseCtrl
    {
        private IHttpContextAccessor _accessor;
        public MiscController(IMapper mapper, IMediator mediator, ILogger<MiscController> logger, IHttpContextAccessor accessor) : base(mapper, mediator, logger)
        {
            _accessor = accessor;
        }


        [HttpPost]
        [Description("对外内充值")]
        [Route("ExternalThirdpayLink")]
        [AllowAnonymous]
        [ActionName("对外内充值")]
        public async Task<IActionResult> ThirdpayLink(GetThirdpayLinkRequest x)
        {
            x.Admin = "External";
            var result = await _mediator.Send(x);
            return await Task.FromResult(Ok(result));
        }


    }
}
