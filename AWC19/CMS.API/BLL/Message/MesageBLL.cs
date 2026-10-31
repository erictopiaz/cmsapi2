using ShareEntity.InputModels.Message;
using ShareEntity.Models;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.FTP;

namespace CMS.API.BLL.Message
{
    /// <summary>
    /// 网站消息处理
    /// </summary>
    public class MesageBLL
    {
        public static async Task<object> LMessageRecords(LMessageInputModel inputModel)
        {
            var viewName = " ofLMessage a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";

            // 创建DAPPER TEMPLATE 
            var whereSql = string.Empty;
            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account='{0}' ", inputModel.account);
            }

            if (inputModel.replyStatus != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.replyStatus={0} ", inputModel.replyStatus) : string.Format(@" {0} and a.replyStatus={1} ", whereSql, inputModel.replyStatus);


            if (inputModel.messageType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.messageType={0} ", inputModel.messageType) : string.Format(@" {0} and a.messageType={1} ", whereSql, inputModel.messageType);

            if (inputModel.accountType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.accountType={0} ", inputModel.accountType) : string.Format(@" {0} and a.accountType={1} ", whereSql, inputModel.accountType);

            var leftjoinString = string.Format(@" left join ofUser u with(nolock) on a.account = u.account left join ofUserExtention ue with(nolock) on a.account = ue.account  ");
            var fields = " a.*, u.anomaly, ue.riskRemark,ue.riskTypesAready ";

            return await DapperPaging<LMessageOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionLogs);
        }

        /// <summary>
        /// 回复留言
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LMessageReply(LMessageReplyInputModel inputModel)
        {
            var updateSql = string.Format(@"update ofLMessage set replyMessage=@reply,replyPerson=@Admin,replyStatus=1 where id=@id and status=0 ");
            

            var querySql = string.Format(@"select * from ofLMessage where id=@id");

            var msgModel = await DapperBase.QueryModelAsync<ofLMessage>(querySql, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            if (msgModel == null)
                return ApiResultModel<object>.GetResult("false", "回复留言-失败", "reply_message_failed");

            if (msgModel != null && msgModel.replyStatus == 1)
                return ApiResultModel<object>.GetResult("false", "回复留言-失败[重复回复]", "reply_message_failed");


            var msgParams = new ofMessage()
            {
                account = msgModel.account,
                createperson = inputModel.Admin,
                content = string.IsNullOrEmpty(inputModel.reply) ? "" : inputModel.reply.Replace("'", "''"),
                title = "留言回复:"+msgModel.title,
                createtime = Core.Common.CurrentTime,
                location = "0,1",
                readtime = null,
                status = 0,
                type = 6,
                visible = 1,
                accountType = 0
            };
            inputModel.reply = Common.DESEnCode(inputModel.reply, "Msg*6Wc#");

            var insertMessageSal = string.Format(@"INSERT INTO [dbo].[ofMessage]
           ([account],[type],[title],[content],[location],[createperson],[accountType])
     VALUES
           (@account,@type,@title,@content,@location,@createperson,@accountType)");

            var insertMessage = string.Empty;
            var replyMessage = string.Empty;

            var insertResult = await DapperBase.ExecuteTransctionAsync(insertMessageSal, param: msgParams, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            if (insertResult)
                insertMessage = "站内信息更新-成功!";
            else
                insertMessage = "站内信息更新-失败!";

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            if (updateResult)
                replyMessage = "回复留言-成功!";
            else
                replyMessage = "回复留言-失败!";
            var msg = string.Format(@"{0},{1}", replyMessage, insertMessage);

            return ApiResultModel<object>.GetResult( "true", msg, "reply_message_success");
        }
        /// <summary>
        /// 查询留言信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<object> LMessageById(long id)
        {
            var querySql = string.Format(@"select * from ofLMessage with(nolock) where id = @id");
            var models = await DapperBase.QueryAsync<LMessageOutModel>(querySql, new { id = id }, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            var code = models != null && models.Count == 1;
            var picBase64 = new List<string>();
            if (code && models.First().f_id != null && !string.IsNullOrEmpty(models.First().f_id))
            {
                var m = models.First();
                var fileNames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(m.f_id);
                if(m.createtime >= Convert.ToDateTime("2023-06-18 00:00:00")) {
                    //新方式查询图片
                    foreach (var f in fileNames) {
                        var fileName = string.Format(@"{0}.png", f);
                        //var picData = string.Format(@"http://20.187.93.246:9000/awc/screenshots/{0}", fileName);
                        var picData = string.Format(@"/custimg/awc/screenshots/{0}", fileName);
                        if (!string.IsNullOrEmpty(picData))
                            picBase64.Add(picData);
                    }
                }
                else {
                    //老的方式查看图片
                    foreach (var f in fileNames) {
                        var fileName = string.Format(@"{0}.txt", f);
                        var picData = FTPHelper.FTPDownload(fileName);
                        if (!string.IsNullOrEmpty(picData))
                            picBase64.Add(picData);
                    }
                }
                
            }
            models.First().picBase64 = picBase64.Count > 0 ? picBase64 : new List<string>();
            return ApiResultModel<object>.GetResult(
               code ? "true" : "false",
               code ? "查询留言-成功" : "查询留言-失败",
               code ? "query_message_success" : "query_message_failed",
               code ? models.First() : null);
        }
        /// <summary>
        /// 取消留言
        /// </summary>
        /// <param name="id"></param>
        /// <param name="admin"></param>
        /// <returns></returns>
        public static async Task<object> LMessageCancel(long id, string admin)
        {
            var updateSql = string.Format(@"update ofLMessage set replyStatus=2,replyPerson=@Admin where id = @id and replyStatus=0 ");

            var result = await DapperBase.ExecuteTransctionAsync(updateSql, new { id = id, Admin=admin }, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
           
            return ApiResultModel<object>.GetResult(
               result ? "true" : "false",
               result ? "留言取消-成功" : "留言取消-失败",
               result ? "cancel_message_success" : "cancel_message_failed");
        }



    }




}
