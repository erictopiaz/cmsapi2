using ShareEntity.InputModels.Transfer;
using ShareEntity.OutModels.Transfer;
using ShareEntity.SqlCommandText;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL.Game
{
    /// <summary>
    /// 转账表的操作
    /// </summary>
    public class TransferBLL
    {
        /// <summary>
        /// 查询转账记录
        /// </summary>
        /// <returns></returns>
        public static async Task<object> LoadTransferRecords(QueryTransferInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofTransferNew ";
            string orderBy = " id ";
            string keyName = " id ";

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" account = '{0}' ", inputModel.account);
            }
            if (inputModel.transferCode != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" gameid = {0} ", inputModel.transferCode) : string.Format(@" {0} and gameid = {1} ", whereSql, inputModel.transferCode);
            if (inputModel.transferStatus != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" status = {0} ", inputModel.transferStatus) : string.Format(@" {0} and status = {1} ", whereSql, inputModel.transferStatus);

            if ( !string.IsNullOrEmpty(inputModel.startTime) && !string.IsNullOrEmpty(inputModel.endTime))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" verifytime >= '{0}' and verifytime < '{1}' ", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and verifytime >= '{1}' and verifytime < '{2}' ", whereSql, inputModel.startTime, inputModel.endTime);

            if (!string.IsNullOrEmpty(inputModel.ipaddress))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" ipaddress = '{0}' ", inputModel.ipaddress) : string.Format(@" {0} and ipaddress = '{1}' ", whereSql, inputModel.ipaddress);

            if (!string.IsNullOrEmpty(inputModel.agentAccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" affaccount = '{0}' ", inputModel.agentAccount) : string.Format(@" {0} and affaccount = '{1}' ", whereSql, inputModel.agentAccount);

            var leftjoinString = "";
            var fields = " * ";
            return await DapperPaging<QueryTransferOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
        /// <summary>
        /// 撤回转账
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancleTransferAsync(CancleTransferInputModel inputModel)
        {
            var sqlArray = new List<string> { TransferSqlCommandText.returnTransferAmount, TransferSqlCommandText.UpdateTransferStatus };
            var param = new List<object> {
                new { transferId = inputModel.transferId },
                new { ipaddress = inputModel.IpAddress, adminAccount = inputModel.adminAccount,transferId = inputModel.transferId } };
            var result = await DapperBase.ExecuteTransctionAsync(sqlArray, param, 
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>
            {
                code = result.ToString(),
                message = result ? "转账补单-成功" : "转账补单-失败",
                message_code = result ? "cancle_transfer_success" : "cancle_transfer_failed"
            };
        }

        /// <summary>
        /// 查看当前转账记录详情
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public static async Task<object> ViewTransferById(long transferId)
        {
            var transferList = await DapperBase.QueryAsync<ViewTransferOutModel>(TransferSqlCommandText.viewTransferSql, new { id = transferId },
                commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionStringReadOnly);
            var result = (transferList != null && transferList.Count == 1);
            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "查看转账记录-成功" : "查看转账记录-失败",
                message_code = result ? "view_transfer_record_success" : "view_transfer_record_failed"
            };
        }


    }
}
