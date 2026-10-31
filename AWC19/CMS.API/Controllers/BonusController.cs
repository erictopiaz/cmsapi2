using Ching.DistributedTransaction.EFModel;
using CMS.API.BLL.Promo;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ShareEntity.InputModels.Bonus;
using ShareEntity.InputModels.Promo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.Cache;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Bonus/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]

    public class BonusController : Controller
    {
        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;
        private IMemoryCache _cache;
        private readonly AmountRecordDbContext _amountRecordDbContext;
        public BonusController(WriteToFile writeToFile,IHttpContextAccessor accessor,
            AmountRecordDbContext amountRecordDbContext,
            IConfiguration config, 
            IMemoryCache memoryCache)
        {
            this._writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
            _cache = memoryCache;
            _amountRecordDbContext = amountRecordDbContext;
        }
        /// <summary>
        /// 红利记录查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BonusRecords([FromBody]QueryBonusInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.remarks = string.IsNullOrEmpty(form.remarks) ? "" : form.remarks.Replace("'", "''");
                var result = await BonusBLL.BonusRecordsAsync(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {
                
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("BonusRecords", "BonusRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录加载-失败e",
                    message_code = "bonus_record_load_exception"
                });
            }
        }

        /// <summary>
        /// 获取红利
        /// 根据红利ID
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoadBonusById(long tid)
        {
            try
            {
                var result = await BonusBLL.LoadBonusById(tid);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("LoadBonusById", "LoadBonusById ", string.Format(@"requestdata = {0}   e.message={1}", tid, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录加载-失败e",
                    message_code = "bonus_record_load_exception"
                });
            }
        }
        /// <summary>
        /// 创建优惠红利
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateBonus([FromBody]CreatePromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;
                form.remarks = string.IsNullOrEmpty(form.remarks) ? "" : form.remarks.Replace("'", "''");
               
                var result = await BonusBLL.CreateBonusAsync(form, client.token, _config["jwt:loginKey"]);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateBonusPromo", "CreateBonusPromo ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利创建失败-失败e",
                    message_code = e.Message
                }); ;
            }
        }
        /// <summary>
        /// 批量创建优惠红利
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateBonusBat([FromBody]CreatePromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.CreateBonusBatAsync(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateBonusPromo", "CreateBonusPromo ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利创建失败-失败e",
                    message_code = e.Message
                }); ;
            }
        }
        /// <summary>
        /// 修改红利金额 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBonus([FromBody]UpdatePromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.UpdateBonusAsync(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("UpdateBonus", "UpdateBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利编辑失败-失败e",
                    message_code = "promo_bonus_modify_exception"
                });
            }
        }
        /// <summary>
        /// 红利审核通过
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyBonus([FromBody]VerifyBonusInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.VerifyBonusAsync(form, postgreSql:"");
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifyBonus", "VerifyBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录审核-失败e",
                    message_code = "bonus_record_veify_exception"
                });
            }
        }
        /// <summary>
        /// 红利审核取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelBonus([FromBody]VerifyBonusInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.CancelBonusAsync(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelBonus", "CancelBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录取消-失败e",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }


        #region 福利中心
        /// <summary>
        /// 福利中心 红利审核取消
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelSelfBonus([FromBody] VerifyBonusInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.CancelSelfBonusAsync(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CancelSelfBonus_ex", "CancelSelfBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "取消操作，处理异常",
                    message_code = "bonus_record_cancel_exception"
                });
            }
        }

        /// <summary>
        /// 福利中心 记录查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SelfBonusRecords([FromBody] QueryBonusInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClientWithOutMac(_accessor);
                var result = await BonusBLL.SelfBonusRecordsAsync(form, client.token, _config["jwt:loginKey"]);
                return Ok(result);
            }
            catch (Exception e)
            {

                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("BonusRecords", "BonusRecords ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "福利中心记录加载-失败e",
                    message_code = "bonus_record_load_exception"
                });
            }
        }


        /// <summary>
        /// 福利中心 修改红利金额 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSelfBonus([FromBody] UpdateSelfPromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.UpdateSelfBonusAsync(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("UpdateSelfBonus_ex", "UpdateSelfBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利编辑失败-失败e",
                    message_code = "promo_bonus_modify_exception"
                });
            }
        }

        /// <summary>
        /// 福利中心 获取红利
        /// 根据红利ID
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoadSelfBonusById(long tid)
        {
            try
            {
                var result = await BonusBLL.LoadSelfBonusById(tid);
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("LoadSelfBonusById_ex", "LoadSelfBonusById ", string.Format(@"requestdata = {0}   e.message={1}", tid, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录加载-失败e",
                    message_code = "bonus_record_load_exception"
                });
            }
        }

        /// <summary>
        /// 福利中心，项目选择列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SelfBonusList()
        {
            try
            {
                var result = await BonusBLL.SelfBonusList();
                return Ok(result);
            }
            catch (Exception e)
            {
                _writeToFile.Done("SelfBonusList_ex", "SelfBonusList ", string.Format(@" e.message={0}", e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "项目选择列表加载-失败e",
                    message_code = "bonus_record_load_exception"
                });
            }
        }

        /// <summary>
        /// 福利中心 批量创建优惠红利
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateSelfBonusBat([FromBody] CreateSelfPromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.CreateSelfBonusBatAsync(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateBonusPromo", "CreateBonusPromo ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利创建失败-失败e",
                    message_code = e.Message
                }); ;
            }
        }

        /// <summary>
        /// 福利中心 创建优惠红利
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateSelfBonus([FromBody] CreateSelfPromoInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                var result = await BonusBLL.CreateSelfBonusAsync(form);

                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("CreateSelfBonus_ex", "CreateSelfBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "优惠红利创建失败-失败e",
                    message_code = e.Message
                }); ;
            }
        }


        /// <summary>
        /// 福利中心 红利审核通过 审核后才可以领取
        /// 批量/单个审核 公共api
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifySelfBonus([FromBody] VerifyBonusBatInputModel form)
        {
            try
            {
                var client = HttpHelper.GetClient(_accessor);
                form.adminAccount = JwtHelper.GetName(client.token, _config["jwt:loginKey"]);
                form.IpAddress = client.ipaddress;

                #region 防址连续性操作
                var key = string.Format(@"{0}VerifySelfBonus", form.adminAccount);
                string ant = CacheHelper.GetCache(_cache, key);
                if (!string.IsNullOrEmpty(ant))
                    return Ok(new { code = "false", message = "操作太频繁" + form.adminAccount, message_code = "refresh_deny" });
                if (!CacheHelper.SetCache(_cache, key, form.adminAccount, 12))
                    return Ok(new { code = "false", message = "操作设置失败" + form.adminAccount });

                #endregion

                var result = await BonusBLL.VerifySelfBonusBatAsync(form);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(form);
                _writeToFile.Done("VerifySelfBonus_ex", "VerifySelfBonus ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    message = "红利优惠记录审核-失败e",
                    message_code = "bonus_record_veify_exception"
                });
            }
        }

        #endregion

    }
}
