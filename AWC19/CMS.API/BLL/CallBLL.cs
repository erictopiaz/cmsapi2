using CMS.API.Models.Call;
using ShareEntity.Call;
using ShareEntity.InputModels.agent;
using ShareEntity.SqlCommandText;
using Dapper;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL
{
    /// <summary>
    /// 电销处理
    /// </summary>
    public class CallBLL
    {
        /// <summary>
        ///  电销  备注报表
        /// </summary>
        /// <param name="month">0:本月， 1：上月</param>
        /// <returns></returns>
        public static async Task<object> RemarkReport(int month)
        {
            if (month == 1 && DateTime.UtcNow.AddHours(8) < Convert.ToDateTime("2021-05-01 00:00:00"))
                return ApiResultModel<object>.GetResult("false", "暂不支持查询上月，5月1号后开放", "query_time_limit");
            if (month != 1 && month != 0)
                return ApiResultModel<object>.GetResult("false", "查询时间有误!", "query_time_code_error");

            //指定电销只能看到自己带来的客人 CallLogSqlCommandText
            var sql = string.Format(CallLogSqlCommandText.QueryCallRemarkReportSql, month);

            var result = await DapperBase.QueryAsync<CallReportV>(sql, null,
                     commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            var code = result != null && result.Count > 0;
            #region 统计电话接通率
            if (result.First().normalTotalCalls > 0)
                result.First().np = string.Format(@"{0}%", ((result.First().normalPickCalls * 1.00 / result.First().normalTotalCalls * 1.00) *100).ToString("F2"));
            
            if (result.First().vipTotalCalls > 0)
                result.First().vp = string.Format(@"{0}%", ((result.First().vipPickCalls*1.00 / result.First().vipTotalCalls*1.00) *100).ToString("F2"));

            if (result.First().totalCalls > 0)
                result.First().tp = string.Format(@"{0}%", (((result.First().vipPickCalls + result.First().normalPickCalls) * 1.00 / result.First().totalCalls * 1.00) *100).ToString("F2"));
            ////////
            if (result.First().vipHJTotalCalls > 0)
                result.First().hjp = string.Format(@"{0}%", ((result.First().vipHJPickCalls * 1.00 / result.First().vipHJTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipPTTotalCalls > 0)
                result.First().ptp = string.Format(@"{0}%", ((result.First().vipPTPickCalls * 1.00 / result.First().vipPTTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipZSTotalCalls > 0)
                result.First().zsp = string.Format(@"{0}%", ((result.First().vipZSPickCalls * 1.00 / result.First().vipZSTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipTYTotalCalls > 0)
                result.First().typ = string.Format(@"{0}%", ((result.First().vipTYPickCalls * 1.00 / result.First().vipTYTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipZZTotalCalls > 0)
                result.First().zzp = string.Format(@"{0}%", ((result.First().vipZZPickCalls * 1.00 / result.First().vipZZTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipJXTotalCalls > 0)
                result.First().jxp = string.Format(@"{0}%", ((result.First().vipJXPickCalls * 1.00 / result.First().vipJXTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipBYINTotalCalls > 0)
                result.First().byinp = string.Format(@"{0}%", ((result.First().vipBYINPickCalls * 1.00 / result.First().vipBYINTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipBYTotalCalls > 0)
                result.First().byp = string.Format(@"{0}%", ((result.First().vipBYPickCalls * 1.00 / result.First().vipBYTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipWXTotalCalls > 0)
                result.First().wxp = string.Format(@"{0}%", ((result.First().vipWXPickCalls * 1.00 / result.First().vipWXTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipHCTotalCalls > 0)
                result.First().hcp = string.Format(@"{0}%", ((result.First().vipHCPickCalls * 1.00 / result.First().vipHCTotalCalls * 1.00) * 100).ToString("F2"));

            if (result.First().vipBJTotalCalls > 0)
                result.First().bjp = string.Format(@"{0}%", ((result.First().vipBJPickCalls * 1.00 / result.First().vipBJTotalCalls * 1.00) * 100).ToString("F2"));
            #endregion

            #region 统计VIP接通率
            if (result.First().normalTotalCallsV > 0)
                result.First().npv = string.Format(@"{0}%", ((result.First().normalPickCallsV * 1.00 / result.First().normalTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipTotalCallsV > 0)
                result.First().vpv = string.Format(@"{0}%", ((result.First().vipPickCallsV * 1.00 / result.First().vipTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().totalCallsV > 0)
                result.First().tpv = string.Format(@"{0}%", (((result.First().vipPickCallsV + result.First().normalPickCallsV) * 1.00 / result.First().totalCallsV * 1.00) * 100).ToString("F2"));
            ////////
            if (result.First().vipHJTotalCallsV > 0)
                result.First().hjpv = string.Format(@"{0}%", ((result.First().vipHJPickCallsV * 1.00 / result.First().vipHJTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipPTTotalCallsV > 0)
                result.First().ptpv = string.Format(@"{0}%", ((result.First().vipPTPickCallsV * 1.00 / result.First().vipPTTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipZSTotalCallsV > 0)
                result.First().zspv = string.Format(@"{0}%", ((result.First().vipZSPickCallsV * 1.00 / result.First().vipZSTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipTYTotalCallsV > 0)
                result.First().typv = string.Format(@"{0}%", ((result.First().vipTYPickCallsV * 1.00 / result.First().vipTYTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipZZTotalCallsV > 0)
                result.First().zzpv = string.Format(@"{0}%", ((result.First().vipZZPickCallsV * 1.00 / result.First().vipZZTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipJXTotalCallsV > 0)
                result.First().jxpv = string.Format(@"{0}%", ((result.First().vipJXPickCallsV * 1.00 / result.First().vipJXTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipBYINTotalCallsV > 0)
                result.First().byinpv = string.Format(@"{0}%", ((result.First().vipBYINPickCallsV * 1.00 / result.First().vipBYINTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipBYTotalCallsV > 0)
                result.First().bypv = string.Format(@"{0}%", ((result.First().vipBYPickCallsV * 1.00 / result.First().vipBYTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipWXTotalCallsV > 0)
                result.First().wxpv = string.Format(@"{0}%", ((result.First().vipWXPickCallsV * 1.00 / result.First().vipWXTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipHCTotalCallsV > 0)
                result.First().hcpv = string.Format(@"{0}%", ((result.First().vipHCPickCallsV * 1.00 / result.First().vipHCTotalCallsV * 1.00) * 100).ToString("F2"));

            if (result.First().vipBJTotalCallsV > 0)
                result.First().bjpv = string.Format(@"{0}%", ((result.First().vipBJPickCallsV * 1.00 / result.First().vipBJTotalCallsV * 1.00) * 100).ToString("F2"));
            #endregion

            return ApiResultModel<object>.GetResult(
                code ? "true" : "false",
                code ? "获取成功" : "获取失败",
                code ? "query_call_remark_success" : "query_call_remar_failed",
                result.First()
                );
            
        }


        /// <summary>
        /// 电销记录查询
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> QueryCallLogs(QueryCallLogModel inputModel)
        {
            var whereSql = string.Empty;
            var whereLevelSql = string.Empty;
            if(!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" and account='{0}'", inputModel.account);
            }

            if (!string.IsNullOrEmpty(inputModel.createperson))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ?  string.Format(@" and createperson='{0}'", inputModel.createperson): string.Format(@" {0} and createperson='{1}'", whereSql, inputModel.createperson);
            }

            if(!string.IsNullOrEmpty(inputModel.starttime) &&
                !string.IsNullOrEmpty(inputModel.endtime))
            {

            }
            else
            {
                var currentDate = DateTime.UtcNow.AddHours(8);
                inputModel.starttime = currentDate.AddMonths(-1).ToString("yyyy-MM-01 00:00:00");
                inputModel.endtime = currentDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if(inputModel.contactStatus != -2 )
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" and contactStatus={0}", inputModel.contactStatus) : 
                    string.Format(@" {0} and contactStatus={1}", whereSql, inputModel.contactStatus);
            
            if (inputModel.level != -1)
                whereLevelSql = string.Format(@" and T.level={0} ", inputModel.level);

            var queryCallLogCountSql = string.Format(CallLogSqlCommandText.QueryCallLogCountSql, whereSql, whereLevelSql);


            var param = new DynamicParameters();
            param.Add("@starttime", inputModel.starttime, dbType: DbType.DateTime);
            param.Add("@endtime", inputModel.endtime, dbType: DbType.DateTime);


            var count = await DapperBase.QueryAsync<RecordsCount>(queryCallLogCountSql, param,
                   commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (count != null && count.Count > 0 && count.First().cnt > 0)//有记录，查询出来
            {
                var startIndex = (inputModel.pageIndex - 1) * inputModel.pageSize + 1;
                var endIndex = inputModel.pageIndex * inputModel.pageSize;
                var totalPages = count.First().cnt % inputModel.pageSize > 0 ? count.First().cnt / inputModel.pageSize + 1 : count.First().cnt / inputModel.pageSize;

                param.Add("@startIndex", startIndex, dbType: DbType.Int32);
                param.Add("@endIndex", endIndex, dbType: DbType.Int32);

                var queryCallLogsSql = string.Format(CallLogSqlCommandText.QueryCallLogsSql, whereSql, whereLevelSql);

                var resultGame = await DapperBase.QueryAsync<CallLogOutModel>(queryCallLogsSql,
                    param,commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                var code = (resultGame != null && resultGame.Count > 0);
                return new AgentApiResultModel<PageModel<CallLogOutModel>, object>()
                {
                    code = code ? "true" : "false",
                    message = code ? "获取下级数据统计-成功" : "获取下级数据统计-失败",
                    message_code = code ? "agent_report_data_success" : "agent_report_data_failed",
                    levels = "",
                    data = new PageModel<CallLogOutModel>
                    {
                        TotalRecord = count.First().cnt,
                        TotalPages = totalPages,
                        PageIndex = inputModel.pageIndex,
                        modelList = resultGame.ToList()
                    },
                    total = null
                };
            }
            else
            {

                return new AgentApiResultModel<object, object>()
                {
                    code = "true",
                    message = "没有查找到相关记录",
                    message_code = "agent_report_data_no_records",
                    levels = "",
                    data = new PageModel<object>
                    {
                        TotalRecord = 0,
                        TotalPages = 0,
                        PageIndex = 0,
                        modelList = new List<object>() { }
                    },
                    total = null
                };
            }
        }
    }

   
}
