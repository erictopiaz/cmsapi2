using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.API.BLL.Game;
using ShareEntity.InputModels.GameConfig;
using ShareEntity.InputModels.GameReport;
using ShareEntity.InputModels.Transfer;
using DBLibs.WXDapper;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UtilsLib;
using UtilsLib.Cache;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    /// <summary>
    /// 游戏管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Game/[action]")]
    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class GameController : ControllerBase
    {
        private WriteToFile _writeToFile;
        private DapperHelper _helper;///
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        public GameController(WriteToFile writeToFile, IHttpContextAccessor accessor,
            DapperHelper dapperHelper, IConfiguration config)
        {
            _writeToFile = writeToFile;
            _helper = dapperHelper;
            _accessor = accessor;
            _config = config;
        }

        #region 转账列表操作

        [HttpPost]
        public async Task<IActionResult> LoadTransferRecords([FromBody] QueryTransferInputModel model)
        {
            try
            {
                var result = await TransferBLL.LoadTransferRecords(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadTransferRecords", "LoadTransferRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "加载转账记录-失败e",
                    message_code = "load_transfer_records_exception"
                });
            }
        }
        /// <summary>
        /// 获取当前转账记录的详情
        /// </summary>
        /// <param name="tranferId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ViewTransferById(long tranferId)
        {
            try
            {
                var result = await TransferBLL.ViewTransferById(tranferId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("ViewTransferById", "ViewTransferById ", string.Format(@"tranferId = {0}   e.message={1}", tranferId, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "查看转账记录-失败e",
                    message_code = "view_transfer_record_exception"
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CancleTransferAsync(CancleTransferInputModel inputModel)
        {
            try
            {
                var result = await TransferBLL.CancleTransferAsync(inputModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(inputModel);
                _writeToFile.Done("CancleTransferAsync", "CancleTransferAsync ", string.Format(@"requestData = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "查看转账记录-失败e",
                    message_code = "cancle_transfer_exception"
                });
            }
        }

        #endregion

        #region 游戏参数配置列表
        /// <summary>
        /// 获取游戏分类列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadGameConfig([FromBody] QueryGameConfigInputModel model)
        {
            try
            {
                var result = await GameBLL.LoadGameConfig(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadGameConfig", "LoadGameConfig ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "加载转账记录-失败e",
                    message_code = "load_game_config_exception"
                });
            }
        }
        /// <summary>
        /// 获取游戏参数列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadGameConfigParams([FromBody] QueryGameConfigInputModel model)
        {
            try
            {
                var result = await GameBLL.LoadGameConfigParams(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadGameConfigParams", "LoadGameConfigParams ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "加载转账记录-失败e",
                    message_code = "load_game_config_params_exception"
                });
            }
        }
        /// <summary>
        /// 更新参数列表的状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGameConfigStatus([FromBody] UpdateGameConfigInputModel model)
        {
            try
            {
                var result = await GameBLL.UpdateGameConfigStatus(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadGameConfigParams", "LoadGameConfigParams ", string.Format(@"requestData = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "加载转账记录-失败e",
                    message_code = "load_game_config_params_exception"
                });
            }
        }
        #endregion

        /// <summary>
        /// 游戏盈利报表
        /// 风控使用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadGameReport([FromBody] QueryGameReportInputModel model)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                var adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                var result = await GameBLL.LoadGameReport(model, adminAccount);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadGameReport", "LoadGameReport ", string.Format(@"requestData = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "加载转账记录-失败e",
                    message_code = "load_game_report_exception"
                });
            }
        }


        #region 获取游戏电子游戏列表
        /// <summary>
        /// 获取游戏电子游戏列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadGameModels([FromBody] QueryGameModelsInputModel model)
        {
            try
            {
                var result = await GameBLL.LoadGameModels(model);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = JsonConvert.SerializeObject(model);
                _writeToFile.Done("LoadGameModels_ex", "LoadGameModels ",
                    string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "获取游戏列表-失败e",
                    message_code = "load_game_list_exception"
                });
            }
        }

        #endregion
    }

}