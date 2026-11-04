using Amazon.Runtime.Internal.Util;
using AutoMapper;
using CMS.API.BLL;
using CMS.Core.Application.Admin.Queries;
using CMS.Core.Application.系统管理.Queries;
using ShareEntity.OutputModels;
using GoogleAuth;
using LogLib.Helper;
//using CMS.Filter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
using UtilsLib.Cache;
using UtilsLib.RequestHelper;
using Newtonsoft.Json;

namespace CMS.API.Controllers
{
    [Route("api/Login")]
    [Description("登入后台验证")]
    public class LoginController : BaseCtrl
    {
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private IMemoryCache _cache;
        private WriteToDB _writeToDB;
        private WriteToFile _writeToFile;
        public LoginController(IMapper mapper, 
            IMediator mediator, 
            ILogger<LoginController> logger,
            WriteToFile writeToFile,
            IHttpContextAccessor accessor, 
            IConfiguration config, IMemoryCache memoryCache, WriteToDB writeToDB) : base(mapper, mediator, logger)
        {
            _accessor = accessor;
            _config = config;
            _cache = memoryCache;
            _writeToFile = writeToFile;
            _writeToDB = writeToDB;
        }


        /// <summary>
        /// 后台登入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("登入后台验证")]
        [HttpPost]
        [AllowAnonymous]
        //[ActionName("登入")]

        public async Task<IActionResult> Post([FromBody] LoginRequestQuery request)
        {
            request.ipaddress = Core.Common.Network.GetIP(_accessor.HttpContext);

            string headers = Newtonsoft.Json.JsonConvert.SerializeObject(_accessor.HttpContext.Request.Headers);

            if (_accessor.HttpContext.Request.Headers.ContainsKey("fp"))
                request.fp = _accessor.HttpContext.Request.Headers["fp"];

            _logger.LogInformation("Login - Account:{account}, Password:{password}, IP:{ipaddress}, Headers:{headers}",
                request.Account, request.Password, request.ipaddress, headers);

            _writeToDB.Done(new LogLib.Models.LogModel() {
                logno = CMS.Core.Common.Security.GuidToLong(),
                logmessage = string.Format(@"管理员登录"),
                logresult = 1,
                account = request.Account.Replace("'", "''"),
                logtype = 23,
                accounttype = 1,
                actionType = 1,
                ipaddress = request.ipaddress.Replace("'","''").ToString(),
                relationAccount = request.Account.Replace("'", "''")
            });

            var keyA = string.Format(@"{0}_f_{1}", request.Account, "pms2Login");
            var keyIP = string.Format(@"{0}_f_{1}", request.ipaddress, "pms2Login");

            var value = CacheHelper.GetCache(_cache, keyA);
            var valueIP = CacheHelper.GetCache(_cache, keyIP);

            if (!string.IsNullOrEmpty(value)
                || !string.IsNullOrEmpty(valueIP))
            {
                request.Password = "";
                _writeToFile.Done("LoginRequestQuery_limit_error_times", "LoginRequestQuery 3 times error ",
                   string.Format(@"requestdata = {0} 15 min 3 times limit",
                   JsonConvert.SerializeObject(request)), true);

                return new ForbidResult();

            }

            var valid = await GA.VerifyGoogleAuthCode(request.vCode, _config["jwt:googleLoginKey"]);
            if (!valid)
            {
                //设置失败次数
                await CacheHelper.SetLoginFailedCountAsync(_cache, request.Account, "pms2Login", 3, 60 * 15, 60);
                await CacheHelper.SetLoginFailedCountAsync(_cache, request.ipaddress, "pms2Login", 3, 60 * 15, 60);

                return new UnauthorizedResult();
            }

            var result = await _mediator.Send(request);

            // FAILED CREDENTIALS
            if (result.Status != 1)
                return new UnauthorizedResult();

            var client = HttpHelper.GetClient(_accessor);
            var AdminIpAddress = client.ipaddress;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { request.Account, AdminIpAddress, result.allowedToManageAgent });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("asldkjfvaoe84runvaofjna");
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Common.DESEnCode(json,"8pi*aWc#")),
                    new Claim(ClaimTypes.UserData, result.access ?? ""),
                    new Claim(CustomerClaimTypes.CurrencyTypes, result.currencyTypes)
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            }); ;
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(string.Concat("bearer ", tokenString));
        }


        /// <returns></returns>
        [Description("登入侦测")]
        [HttpPost]
        [Route("CheckStatus")]
        public async Task<IActionResult> CheckStatus()
        {
            return Ok();
        }



        /// <returns></returns>
        [Description("登入的用户")]
        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new ResponseModel()
            {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = User.Identity.Name
            });
        }

        /// <returns></returns>
        [Description("登入的用户")]
        [HttpGet]
        [AllowAnonymous]
        [Route("GetUserPrivilege")]
        public async Task<IActionResult> GetUserPrivilege(GetAccessPrivilege x)
        {

            var result = await _mediator.Send(x);
            return Ok(result);
        }

        //[HttpGet]
        //public IActionResult LoadUser()
        //{
        //    return Ok("hello");
        //}


    }
}
