using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.API.BLL.Agent;
using ShareEntity.InputModels;
using ShareEntity.InputModels.agent;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    /// <summary>
    /// 代理管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Agent/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]

    public class AgentController : ControllerBase
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        public AgentController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentCreate([FromBody] RegisterAffiliateModel model)
        {
            try
            {
                var result = await AgentBLL.CreateAccountAsync(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                _writeToFile.Done("AgentCreate", "AgentCreate ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "代理注册-失败e",
                    message_code = "agent_register_exception"
                });
            }
        }
        /// <summary>
        /// 代理审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentVerify([FromBody] VerifyAgentInputModel accountModel)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                accountModel.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                accountModel.IpAddress = client.ipaddress;

                var result = await AgentBLL.AgentVerify(accountModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(accountModel);
                _writeToFile.Done("AgentVerify", "AgentVerify ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "代理审核-失败e",
                    message_code = "agent_Verify_exception"
                });
            }
        }
        /// <summary>
        /// 编辑代理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentEdit()
        {
            await Task.Delay(1);

            return Ok(new { a = "", b = "" });
        }
        /// <summary>
        /// 代理开启和停用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentDisableOrEnable(AgentDisableOrEnableInputModel inputModel)
        {
            try
            {
                var result = await AgentBLL.AgentDisableOrEnable(inputModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(inputModel);
                _writeToFile.Done("AgentDisableOrEnable", "AgentDisableOrEnable ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "代理状态更新-失败e",
                    message_code = "agent_disable_enable_exception"
                });
            }
        }
        /// <summary>
        /// 代理显示列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }
        /// <summary>
        /// 代理日存款列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentDepositDailyList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }
        /// <summary>
        /// 代理月存款列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentDepositMonthlyList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }
        /// <summary>
        /// 代理日游戏列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentGameDailyList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }
        /// <summary>
        /// 代理月游戏列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentGameMonthlyList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }

        /// <summary>
        /// 代理日负赢利列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentLoseDailylyList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }

        /// <summary>
        /// 代理提款列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentWithdrawList()
        {
            await Task.Delay(1);
            return Ok(new { a = "", b = "" });
        }
    }
}