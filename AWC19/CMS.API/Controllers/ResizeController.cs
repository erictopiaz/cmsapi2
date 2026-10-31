using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.API.BLL.Promo;
using ShareEntity.InputModels.Resize;
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
    /// 账号微调操作
    /// </summary>
    [Produces("application/json")]
    [Route("api/Resize/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]

    [ApiController]
    public class ResizeController : Controller
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        public ResizeController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
        }
        /// <summary>
        /// 微调记录查询返回
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResizeRecords([FromBody]QueryResizeInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.AdminIpAddress = client.ipaddress;

                var result = await ResizeBLL.ResizeRecords(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("ResizeRecords", "ResizeRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "微调记录加载-失败e",
                    message_code = "reszie_record_create_exception"
                });
            }
        }
        #region  会员微调
        /// <summary>
        /// 微调创建
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateResize([FromBody]CreateResizeInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.createperson = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await ResizeBLL.CreateResize(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateResize", "CreateResize ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "微调创建-失败e",
                    message_code = "reszie_create_exception"
                });
            }
        }
        /// <summary>
        /// 微调审核
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyResize([FromBody]VerifyResizeInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await ResizeBLL.VerifyResize(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifyResize", "VerifyResize ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "微调-失败e",
                    message_code = "reszie_verify_exception"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CancelResize([FromBody]VerifyResizeInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await ResizeBLL.CancelResize(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelResize", "CancelResize ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "微调-失败e",
                    message_code = "reszie_cancel_exception"
                });
            }
        }

        #endregion

       
    }
}