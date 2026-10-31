using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBLibs.WXDapper;
using LogLib.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UtilsLib.Cache;

namespace backendService.Controllers
{
    /// <summary>
    /// 游戏管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Game/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private WriteToFile _writeToFile;
        private DapperHelper _helper;///
        public GameController(WriteToFile writeToFile,
            DapperHelper dapperHelper)
        {
            _writeToFile = writeToFile;
            _helper = dapperHelper;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns
        [HttpGet]
        public async Task<IActionResult> TransferList(int s)
        {
            await Task.Delay(1);
            if (ModelState.IsValid)
            {
                var model = new { account = "", pwd = "" };

                if (RedisHelper.SetStringKey("accounttest", "caocao", TimeSpan.FromSeconds(50)))
                {
                    var account = RedisHelper.GetStringKey("accounttest");
                    //account = RedisHelper.GetStringKey<string>(token);
                }

                var result = await _helper.QueryAsync<User>("select top 10 * from ouaccount", null, commandTimeout: 15000);



                return Ok(result);
            }
            else
            {
                var errorList = ModelState.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                );
                //日志
                _writeToFile.Done("SendSecurityCodeByPhone", "ModelState.IsValid ", errorList.First().Key + ":" + errorList.First().Value[0].ToString(), true);
            }
            return Ok(new { a = "", b = "" });
        }
    }
    public class  User
    {
        public string account { set; get; }
    }
}