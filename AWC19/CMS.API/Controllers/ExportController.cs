using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.API.BLL.Export;
using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.agent;
using ShareEntity.InputModels.BetRecordModel;
using ShareEntity.InputModels.Bonus;
using ShareEntity.InputModels.TurnOver;
using ShareEntity.OutModels.agent;
using ShareEntity.OutModels.Bonus;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using UtilsLib;
using ShareEntity.Agent.Finance;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Export/[action]")]

    [ApiController]
    [Authorize(Policy = "PathBasedPolicy")]
    public class ExportController : ControllerBase
    {
        private readonly WriteToFile _writeToFile;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ExportController(WriteToFile writeToFile, IWebHostEnvironment hostingEnvironment)
        {
            this._writeToFile = writeToFile;
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [ActionName("LBPFD")]
        public async Task<IActionResult> LoadBonusPromoFromDB([FromBody]QueryBonusInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.ExportBonusRecords(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportBonusOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("BonusPromo.xlsx", responseModel.data.modelList));
                        //var stream = new MemoryStream();

                        //using (var package = new ExcelPackage(stream))
                        //{
                        //    var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                        //    // simple way
                        //    workSheet.Cells.LoadFromCollection(responseModel.data.modelList, true);


                        //    package.Save();
                        //}
                        //stream.Position = 0;
                        //string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                        //return File(stream, "application/octet-stream", excelName);
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadBonusPromoFromDB", "LoadBonusPromoFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_bonus_promo_data_exception"
                });
            }
        }


        [HttpPost]
        [ActionName("LAWDF")]
        public async Task<IActionResult> LoadAgentWithdrawDataFromDB([FromBody]QueryAgentWithdrawInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentWithdrawDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportAgentWithdrawOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("AgentWithdraw.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentWithdrawDataFromDB", "LoadAgentWithdrawDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_withdraw_data_exception"
                });
            }
        }

        /// <summary>
        /// 代理结算数据导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LABILL")]
        public async Task<IActionResult> LoadAgentBillDataFromDB([FromBody]QueryAgentBilledInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentBillDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportAgentBilledOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("AgentBilled.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentBillDataFromDB", "LoadAgentBillDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_billed_data_exception"
                });
            }
        }

        /// <summary>
        /// 代理日自动结算数据导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LABILLAUTO")]
        public async Task<IActionResult> LoadAgentBillAutoDataFromDB([FromBody] QueryAgentBilledInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentBillAutoDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportAFBillReportOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {
                        return Ok(SaveExcel("AgentAutoBilled.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentBillAutoDataFromDB", "LoadAgentBillAutoDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_auto_billed_data_exception"
                });
            }
        }

        /// <summary>
        /// 代理下线会员月游戏数据-----导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadAgentPlayersGameMonthDataFromDB([FromBody]AgentReportQueryInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentPlayersGameMonthDataFromDB(exportInputModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentPlayersGameMonthDataFromDB", "LoadAgentPlayersGameMonthDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_game_month_data_exception"
                });
            }
        }
        /// <summary>
        /// 代理下线会员日游戏数据-----导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LAPGD")]
        public async Task<IActionResult> LoadAgentPlayersGameDailayDataFromDB([FromBody]AgentReportQueryInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentPlayersGameDailayDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportPlayersDailyReportGameOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("PlayerGameDaily.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentPlayersGameDailayDataFromDB", "LoadAgentPlayersGameDailayDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_game_daily_data_exception"
                });
            }
        }
        /// <summary>
        /// 导出代理时间段内的统计数据
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LAGD")]
        public async Task<IActionResult> LoadAgentGameDailayDataFromDB([FromBody] AgentReportQueryInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentGameDailayDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportPlayersDailyReportGameOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("AgentGameDaily.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentPlayersGameDailayDataFromDB", "LoadAgentPlayersGameDailayDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_game_daily_data_exception"
                });
            }
        }

        /// <summary>
        /// 游戏平台数据导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BetRecord")]
        public async Task<IActionResult> LoadUserBetRecord([FromBody] QueryUserBetLogInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadUserBetRecord(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<CRBetRecordExportModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {
                        var fileName = string.Format(@"betRecord_{0}.xlsx", exportInputModel.gameId);
                        return Ok(SaveExcel(fileName, responseModel.data.modelList));
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadUserBetRecord_ex", "LoadUserBetRecord ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_game_bet_records_exception"
                });
            }
        }

        /// <summary>
        /// 代理下线会员月存款数据 -----导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadAgentPlayersFinanceMonthDataFromDB([FromBody]AgentReportQueryInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentPlayersFinanceMonthDataFromDB(exportInputModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentPlayersFinanceMonthDataFromDB", "LoadAgentPlayersFinanceMonthDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_finance_month_data_exception"
                });
            }
        }

        /// <summary>
        /// 代理下线会员日存款数据 -----导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LAPFD")]
        public async Task<IActionResult> LoadAgentPlayersFinanceDailayDataFromDB([FromBody]AgentReportQueryInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentPlayersFinanceDailayDataFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportPlayersDailyReportFinanceOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {

                        return Ok(SaveExcel("PlayerFinanceDaily.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);

            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentPlayersFinanceDailayDataFromDB", "LoadAgentPlayersFinanceDailayDataFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_finance_daily_data_exception"
                });
            }
        }

        /// <summary>
        /// 代理下线会员日存款数据 -----导出
        /// </summary>
        /// <param name="exportInputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LATURNOVER")]
        public async Task<IActionResult> LoadAgentTurnoverFromDB([FromBody]QueryTurnOverInputModel exportInputModel)
        {
            try
            {
                var result = await ExportBLL.LoadAgentTurnoverFromDB(exportInputModel);
                var jsonData = JsonConvert.SerializeObject(result);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var responseModel = JsonConvert.DeserializeObject<ApiResultModel<PageModel<ExportTurnOverOutModel>>>(jsonData);
                    if (responseModel.code.Equals("true"))
                    {
                        return Ok(SaveExcel("PlayerTurnover.xlsx", responseModel.data.modelList));
                    }
                }
                return Ok(result);

            }
            catch (Exception e)
            {
                var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(exportInputModel);
                _writeToFile.Done("LoadAgentTurnoverFromDB", "LoadAgentTurnoverFromDB ", string.Format(@"requestdata = {0}   e.message={1}", requestData, e.Message), true);
                return Ok(new ApiResultModel<object>
                {
                    code = "false",
                    message = "导出数据异常-失败e",
                    message_code = "agent_export_turnover_data_exception"
                });
            }
        }

        private object SaveExcel<T>(string excelName, List<T> models)
        {
            #region 第一种方案
            var sss = "no";
            string folder = _hostingEnvironment.WebRootPath;
            if (models.Count <= 0)
                return new { code = "false", message = "导出记录为空-失败", message_code = "export_data_failed" };
            try
            {
                var requestHost = Request.Headers.ContainsKey("Origin") ? Request.Headers["Origin"].First() : "";

                if(string.IsNullOrEmpty(requestHost))
                    return new { code = "false", message = "导出记录-失败[获取地址失败]", message_code = "export_data_failed" };

                string downloadUrl = string.Format("{0}/export/{1}", requestHost, excelName);



                _writeToFile.Done("SaveExcel", "SaveExcel_downloadUrl",
                    string.Format(@"downloadUrl = {0} , requestHost = {1}", downloadUrl, requestHost), true);


                //string downloadUrl = string.Format("https://zshjhtai.awht.net/export/{0}", excelName);


                FileInfo file = new FileInfo(Path.Combine(folder+ "/export/", excelName));
               
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(folder + "/export/", excelName));
                    //file = new FileInfo(Path.Combine(folder, excelName));
                    sss = "yes";
                }

                using (var package = new ExcelPackage(file))
                {
                    var sheet = package.Workbook.Worksheets.Add("loai");
                    sheet.Cells.LoadFromCollection(models, true);
                    package.Save();
                }
                return new { code = "true", message = "", message_code = "export_data_success", url = downloadUrl };
            }
            catch (Exception ex)
            {
                return new { code = "false", message = "导出记录-失败e"+ex.Message+ "   sss="+ sss+"   =="+ folder, message_code = "export_data_failed" };
            }
            #endregion
        }

        //private async Task SaveExcel_New<T>(string excelName, List<T> models)
        //{
        //    #region 第一种方案
            
        //    try
        //    {
        //        string folder = _hostingEnvironment.WebRootPath;
        //        FileInfo file = new FileInfo(Path.Combine(folder + "/export/", excelName));

        //        if (file.Exists)
        //        {
        //            file.Delete();
        //            file = new FileInfo(Path.Combine(folder + "/export/", excelName));
        //            //file = new FileInfo(Path.Combine(folder, excelName));
        //        }
        //        using (var package = new ExcelPackage(file))
        //        {
        //            var sheet = package.Workbook.Worksheets.Add("loai");
        //            sheet.Cells.LoadFromCollection(models, true);
        //            package.Save();


        //            var data = package.GetAsByteArray();
        //            //保存和归来的Excel文件作为一个ByteArray
        //            HttpResponse response = HttpContext.Request.HttpContext.Response;
        //            response.Clear();

        //            //输出头文件　　
        //            response.Headers.Add("content-disposition", "attachment;  filename=" + excelName );
        //            response.ContentType = "application/vnd.ms-excel;charset=UTF-8";
        //            await response.WriteAsync(Encoding.UTF8.GetString(data));
        //        }
        //        return ;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ;
        //    }
        //    #endregion
        //}

    }
}