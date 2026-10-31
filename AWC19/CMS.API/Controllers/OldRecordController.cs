using System;
using System.Collections.Generic;
using System.Linq;
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
using CMS.Core.Application.优惠管理.Queries;
using CMS.Core.Application.游戏管理.Queries;
using UtilsLib.JWT;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class OldRecordController : BaseCtrl
    {
        private readonly IActionDescriptorCollectionProvider _provider;

        public OldRecordController(
            IMapper mapper,
            IMediator mediator,
            IActionDescriptorCollectionProvider provider1,
            ILogger<AccountController> logger) : base(mapper, mediator, logger)
        {
            _provider = provider1;
        }



        [HttpPost]
        [Route("BonusPromoRecord")]
        [Description("红利优惠列表")]
        [ActionName("BonusRecord")]
        public async Task<IActionResult> BonusPromoRecord(红利优惠列表 x)
        {
            var result = await _mediator.Send(x);
            return Ok(result);
        }



        [HttpPost]
        [Route("TransferRecord")]
        [Description("转账列表")]
        [ActionName("TransferRecord")]
        public async Task<IActionResult> TransferRecord(游戏转账搜索 x)
        {
            x.currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(User).ToList();
            var result = await _mediator.Send(x);
            return Ok(result);
        }


    }
}
