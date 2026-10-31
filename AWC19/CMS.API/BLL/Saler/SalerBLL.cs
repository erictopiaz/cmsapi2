using ShareEntity.InputModels.Saler;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL.Saler
{
    /// <summary>
    /// 电销操作
    /// </summary>
    public class SalerBLL
    {
        /// <summary>
        /// 电销会员报表
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> SalerRecords(SalerReportInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofSalerDailyReportFG a ";
            string orderBy = " a.financeGameDay desc";
            string keyName = " a.account ";

            //指定电销只能看到自己带来的客人
            if (await CheckRight.LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Call/AllowQuerySalerSelf"))
            {
                inputModel.saler = inputModel.adminAccount;
            }


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account = '{0}' ", inputModel.account);
            }
            var currentDay = DateTime.UtcNow.AddHours(8);

            var startTime = Convert.ToInt32(currentDay.AddMonths(-1).ToString("yyyyMM01"));
            var endTime = Convert.ToInt32(currentDay.ToString("yyyyMMdd"));

            if (inputModel.startTime != -1
               && inputModel.endTime != -1)
            {
                startTime = inputModel.startTime <= startTime ? startTime : inputModel.startTime;
                endTime = inputModel.endTime;
            }

            if (!string.IsNullOrEmpty(inputModel.saler))
                whereSql = string.IsNullOrEmpty(whereSql) 
                    ? string.Format(@" a.affaccount = '{0}' ", inputModel.saler) 
                    : string.Format(@" {0} and a.affaccount = '{1}' ", whereSql, inputModel.saler);


            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.financeGameDay >= {0} and a.financeGameDay <= {1} ", startTime, endTime)
                     : string.Format(@" {0} and a.financeGameDay >= {1} and a.financeGameDay <= {2}", whereSql, startTime, endTime);


            var leftjoinString = "";
            var fields = " account, affaccount saler, financeGameDay,depositAmount,withdrawAmount,promo,bonus ";

            var pageModel = await DapperPaging<SalerReportOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);

            ApiResultModel<PageModel<SalerReportOutModel>> apiResultModel = pageModel as ApiResultModel<PageModel<SalerReportOutModel>>;

            if (inputModel.startTime != -1
               && inputModel.endTime != -1) //输入统计时间
            {

                //获取统计数据： 时间段内的总投注和总输赢
                var tsql = string.Format(@"select sum(depositAmount) depositAmount,sum(withdrawAmount) withdrawAmount,sum(promo) promo,sum(bonus) bonus 
from ofSalerDailyReportFG a
where {0} ", whereSql);

                var result = await DapperBase.QueryAsync<SalerReport>(tsql, null,
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                var code = (result != null && result.Count > 0);
                if (code)
                {
                    apiResultModel.data.totalData = result.First();
                }
            }
            return apiResultModel;
        }

    }

}
