//using CMS.API.Models;
using CMS.API.Models;
using CMS.API.Models.Vip;
using Dapper;
using DBLibs;
using DBLibs.WXDapper;
using LogLib.Models;
using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.Vip;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.SqlInject;
using static Dapper.SqlBuilder;

namespace CMS.API.BLL.Account
{
    public class AccountBLL
    {
        /// <summary>
        /// 获取会员账号根据id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string QueryAccountById(long id)
        {
            var queryAccountSql = string.Format(@"select account from ofUser with(nolock) where id={0}", id);
            var accountObj = DapperBase.ExecuteScalarValue(queryAccountSql, null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
            if(accountObj != null)
                return accountObj.ToString();
            else
                return "";
        }

        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> QueryUserLogRecords(QueryUserLogInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofAccountLogs a ";
            string orderBy = " a.create_at desc";
            string keyName = " a.logno ";
            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            //if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            //{
            //    whereSql = string.Format(@" a.account in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount);
            //}

            var currentDate = DateTime.UtcNow.AddHours(8);

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) 
                    ? string.Format(@" a.account='{0}' ", inputModel.account) 
                    : string.Format(@" {0} and a.account='{1}' ", whereSql, inputModel.account);

            if(inputModel.accountType == -1)
                inputModel.accountType = 1; //默认合营

            if (inputModel.accountType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) 
                    ? string.Format(@" a.accountType = {0} ", inputModel.accountType) 
                    : string.Format(@" {0} and a.accountType = '{1}' ", whereSql, inputModel.accountType);


            if(string.IsNullOrEmpty(inputModel.startTime) 
                || string.IsNullOrEmpty(inputModel.endTime))
            {
                inputModel.startTime = currentDate.ToString("yyyy-MM-dd 00:00:00");
                inputModel.endTime = currentDate.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            }

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) 
                    ? string.Format(@" a.create_at >= '{0}' and a.create_at <= '{1}'", inputModel.startTime, inputModel.endTime)
                    : string.Format(@" {0} and a.create_at >= '{1}' and a.create_at <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }
           

            //if (!string.IsNullOrEmpty(inputModel.remarks))
            //{
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" remarks like N'%{0}%' ", inputModel.remarks) : string.Format(@" {0} and remarks like N'%{1}%' ", whereSql, inputModel.remarks);
            //}

            var leftjoinString = string.Format(@"");
            var fields = " a.* ";

            return await DapperPaging<LogResponseModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionLogs);
        }

        /// <summary>
        /// 获取红利或优惠数据模型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, VipImportContentModel>, string> QueryVIPAccounts(string content)
        {
            try
            {
                if(string.IsNullOrEmpty(content))
                    return new Tuple<bool, Dictionary<string, VipImportContentModel>, string>(false, null, "请输入会员账号?" );
                var arrays = content.Split("#");
                //var models = new List<string>();
                var models = new Dictionary<string,VipImportContentModel>();

                foreach (var item in arrays)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    var rowContent = item.Replace("\t", "").Replace("\n", "");
                    var account = rowContent.Split("::")[0];
                    var message = rowContent.Split("::")[1];
                    message = string.IsNullOrEmpty(message) ? "" : message.Replace("'", "''");
                    if (!ExpressSqlInject.CharAndDigitalAndUnderScore(account, 4, 20))
                        return new Tuple<bool, Dictionary<string, VipImportContentModel>, string>(false, null, "数据校验有误:"+ rowContent);
                    else
                    {

                        if (models.ContainsKey(account.ToLower()))
                            return new Tuple<bool, Dictionary<string, VipImportContentModel>, string>(false, null, string.Format(@"{0}重复", rowContent));
                        else
                            models.Add(account, new VipImportContentModel { account = account, message = message });
                    }
                }
                return new Tuple<bool, Dictionary<string, VipImportContentModel>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, VipImportContentModel>, string>(false, null, "导入数据异常");
            }

        }
        /// <summary>
        /// VIP专员导入
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> ImportVIPContacts(ImportVIPContactModel inputModel)
        {
            var models = QueryVIPAccounts(inputModel.content);

            if (models.Item1)//输入数据格式正确
            {
                //获取指定qq和wx 数据
                var whereSql = string.Empty;
                if (inputModel.qid > 0)
                    whereSql = string.Format(@"{0}", inputModel.qid);
                if (inputModel.wid > 0)
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@"{0}", inputModel.wid) : string.Format(@"{0},{1}", whereSql, inputModel.wid);
                
                if (inputModel.lid > 0)
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@"{0}", inputModel.lid) : string.Format(@"{0},{1}", whereSql, inputModel.lid);
                if (inputModel.fid > 0)
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@"{0}", inputModel.fid) : string.Format(@"{0},{1}", whereSql, inputModel.fid);
                if (inputModel.tid > 0)
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@"{0}", inputModel.tid) : string.Format(@"{0},{1}", whereSql, inputModel.tid);


                if (string.IsNullOrEmpty(whereSql))
                    return ApiResultModel<object>.GetResult("false", "请选择联系方式", "");

                var querySql = string.Format(@"select * from ofManageContact with(nolock) where id in ({0}) and status=1 ", whereSql);

                var qModels = await DapperBase.QueryAsync<dynamic>(querySql,
                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                if(qModels != null && qModels.Count > 0)
                {
                    var qq = string.Empty;
                    var qqBase64 = string.Empty;
                    var wx = string.Empty;
                    var wxBase64 = string.Empty;
                    var line = string.Empty;
                    var lineBase64 = string.Empty;
                    var fb = string.Empty;
                    var fbBase64 = string.Empty;
                    var telegram = string.Empty;
                    var telegramBase64 = string.Empty;
                    foreach (var m in qModels)
                    {
                        if (m.contact_type == 0)
                        {
                            qq = m.contact_no;
                            qqBase64 = m.base64Src;
                        }
                        else if (m.contact_type == 1)
                        {
                            wx = m.contact_no;
                            wxBase64 = m.base64Src;
                        }
                        else if (m.contact_type == 2)
                        {
                            line = m.contact_no;
                            lineBase64 = m.base64Src;
                        }
                        else if(m.contact_type == 3) {
                            fb = m.contact_no;
                            fbBase64 = m.base64Src;
                        }
                        else if (m.contact_type == 4){
                            telegram = m.contact_no;
                            telegramBase64 = m.base64Src;
                        }

                    }

                    var contact_type = (!string.IsNullOrEmpty(qq) && !string.IsNullOrEmpty(wx) && !string.IsNullOrEmpty(line)) ? 15
                        :(!string.IsNullOrEmpty(wx) && !string.IsNullOrEmpty(line)) ? 13
                        : (!string.IsNullOrEmpty(qq) && !string.IsNullOrEmpty(wx)) ? 12
                        : (!string.IsNullOrEmpty(qq) && !string.IsNullOrEmpty(line) ) ? 11 
                        :!string.IsNullOrEmpty(qq) ? 0 : !string.IsNullOrEmpty(wx) ? 1 : !string.IsNullOrEmpty(line) ? 2 : 100;

                    var insertModels = new List<object>();
                    foreach (var m in models.Item2)
                    {
                        var account = m.Key.ToLower();
                        insertModels.Add(new
                        {
                            account = account,
                            manage_account = inputModel.adminAccount,
                            qq = string.IsNullOrEmpty(qq) ? "" : qq,
                            qqQRBase64 = qqBase64,
                            wx = string.IsNullOrEmpty(wx) ? "" : wx,
                            wxQRBase64 = wxBase64,
                            line = string.IsNullOrEmpty(line) ? "" : line,
                            lineQRBase64 = lineBase64,
                            contact_type = contact_type,
                            message = m.Value.message,
                            contacts = Newtonsoft.Json.JsonConvert.SerializeObject(new { fb = fb, fb_qr = fbBase64, telegram = telegram, telegram_qr = telegramBase64 })

                        });
                    }

                    var insertSql = string.Format(@"INSERT INTO [dbo].[ofUserMapManage]
           ([account],[manage_account],[qq],[qqQRBase64],[wx],[wxQRBase64],[line],[lineQRBase64],[contact_type],[message],[contacts])
     VALUES
           (@account, @manage_account, @qq, @qqQRBase64, @wx, @wxQRBase64,@line, @lineQRBase64, @contact_type,@message,@contacts)");

                    var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(insertSql,
                    insertModels, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);

                    return new ApiResultModel<object>()
                    {
                        code = insertResult ? "true" : "false",
                        message = insertResult ? "分配成功" : "分配失败",
                        message_code = "calc_bonus_promo_finished"
                    };

                }
                else
                {
                    return ApiResultModel<object>.GetResult("false", "联系方式检测失败", "");
                }

                
                
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = models.Item3,
                    message_code = "",

                };

            }

        }



        /// <summary>
        /// 分配管理员联系方式到VIP
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="loginAccount">当前登录的管理员</param>
        /// <returns></returns>
        public static async Task<object> QueryVipMapping(VipMappingInputModel inputModel, string loginAccount)
        {
            string whereSql = string.Empty;
            var viewName = " ofUserMapManage a ";
            string orderBy = " a.create_at desc";
            string keyName = " a.id ";

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account='{0}' ", inputModel.account) : string.Format(@" {0} and a.account='{1}' ", whereSql, inputModel.account);

            if (!string.IsNullOrEmpty(inputModel.qq))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.qq='{0}' ", inputModel.qq) : string.Format(@" {0} and a.qq='{1}' ", whereSql, inputModel.qq);
            
            if (!string.IsNullOrEmpty(inputModel.wx))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.wx='{0}' ", inputModel.wx) : string.Format(@" {0} and a.wx='{1}' ", whereSql, inputModel.wx);
            
            if (!string.IsNullOrEmpty(inputModel.wx))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.line='{0}' ", inputModel.line) : string.Format(@" {0} and a.line='{1}' ", whereSql, inputModel.line);

            if (await LoadAllowedPathByAccount(loginAccount, "api/Account/QueryAllVIPContacts"))//查询所有已分配置联系VPI
            {
                //查询所有专员分配的vip 联系方式
                if (!string.IsNullOrEmpty(inputModel.adminAccount))
                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.manage_account='{0}' ", inputModel.adminAccount) : string.Format(@" {0} and a.manage_account='{1}' ", whereSql, inputModel.adminAccount);

            }
            else
            {

                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.manage_account='{0}' ", loginAccount) : string.Format(@" {0} and a.manage_account='{1}' ", whereSql, loginAccount);

            }

            //if (!string.IsNullOrEmpty(inputModel.startTime)
            //    && !string.IsNullOrEmpty(inputModel.endTime))
            //{
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.create_at >= '{0}' and a.create_at <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.createtime >= '{1}' and a.createtime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            //}


            var leftjoinString = string.Format(@"");
            var fields = " a.* ";

            return await DapperPaging<VipMappingOutputModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionString);
        }

        private static async Task<bool> LoadAllowedPathByAccount(string account, string requestPath)
        {
            try
            {
                var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
                var result = await DapperBase.QueryAsync<RightModel>(accessSql, new { account = account }, commandType: CommandType.Text,
                            commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                if (result != null && result.Count == 1 && result.First().access.IndexOf(requestPath, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 确认vip会员已经添加我们联系方式
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> ConformedVIP(ConformActionModel inputModel)
        {
            var currentDateTime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            var cancelSql = string.Format(@"update ofUserMapManage set conformed_by='{0}',conformed_at='{1}',c_status=1 where id={2} and r_status=1",
                inputModel.adminAccount, currentDateTime, inputModel.id);
            var result = await DapperBase.ExecuteTransctionAsync(cancelSql,
                  null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>()
            {
                code = result ? "true" : "false",
                message = result ? "确认成功" : "确认失败",
                message_code = result ? "cancel_bonus_promo_success" : "cancel_bonus_promo_failed"
            };
        }

        /// <summary>
        /// 取消分配置到VIP联系方式
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelContactVIP(ConformActionModel inputModel)
        {
            var currentDateTime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            var cancelSql = string.Format(@"update ofUserMapManage set r_status=0,cancel_at='{0}',conformed_by='{1}' where id={2} and r_status = 1",
                currentDateTime, inputModel.adminAccount, inputModel.id);
            var result = await DapperBase.ExecuteTransctionAsync(cancelSql,
                  null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>()
            {
                code = result ? "true" : "false",
                message = result ? "取消分配成功" : "取消分配失败",
                message_code = result ? "cancel_bonus_promo_success" : "cancel_bonus_promo_failed"
            };
        }


        /// <summary>
        /// 查询当前管理员的联系方式
        /// </summary>
        /// <param name="adminAccount"></param>
        /// <returns></returns>
        public static async Task<object> LoadContactsData(string adminAccount)
        {
            var cancelSql = string.Format(@"select contact_no,contact_type,id from ofManageContact where account='{0}' and status=1", adminAccount);
            var result = await DapperBase.QueryAsync<dynamic>(cancelSql,
                  null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            var code = result != null && result.Count > 0;
            return new ApiResultModel<object>()
            {
                code = code ? "true" : "false",
                message = code ? "查询联系方式成功" : "查询联系方式失败",
                message_code = code ? "cancel_bonus_promo_success" : "cancel_bonus_promo_failed",
                data = result
            };
        }
    }
}
