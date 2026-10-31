using CMS.Core.Application.存款管理.Transact;
using DBLibs;
using DBLibs.WXDapper;
using Org.BouncyCastle.Asn1.Ocsp;
using PromoShare;
using ShareEntity.InputModels.Resize;
using ShareEntity.Models;
using ShareEntity.OutModels.Resize;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.Merchant;

namespace CMS.API.BLL.Promo
{
    public class ResizeBLL
    {
        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> ResizeRecords(QueryResizeInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofminus a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";
            inputModel.adminAccount = JwtHelper.GetName(token, loginKey);
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
            if (!currencys.Contains(inputModel.currencyType)) {
                return new AgentApiResultModel<object, object>() {
                    code = "false",
                    message = "请选择货币查询",
                    message_code = "error_currency_search",
                    levels = "",
                };
            }
            var accounts = string.IsNullOrEmpty(inputModel.account) ? "" : MerchantCodes.GetAccounts(inputModel.account);
            var affaccounts = string.IsNullOrEmpty(inputModel.affaccount) ? "" : MerchantCodes.GetAccounts(inputModel.affaccount);


            if (string.IsNullOrEmpty(inputModel.merchantAccount))
                inputModel.merchantAccount = "-1";
            else
            {
                inputModel.merchantAccount = inputModel.merchantAccount.Replace("'", "''");
            }

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account in {0} ", accounts);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount in {0} ", affaccounts) 
                    : string.Format(@" {0} and b.affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.gameType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.gametype = {0} ", inputModel.gameType) : string.Format(@" {0} and a.gametype = {1} ", whereSql, inputModel.gameType);

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.createtime >= '{0}' and a.createtime <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.createtime >= '{1}' and a.createtime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            if (!string.IsNullOrEmpty(inputModel.remarks))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" remarks like N'%{0}%' ", inputModel.remarks) : string.Format(@" {0} and remarks like N'%{1}%' ", whereSql, inputModel.remarks);
            }
            if (inputModel.decreasetype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" decreasetype = {0} ", inputModel.decreasetype) : string.Format(@" {0} and decreasetype = {1} ", whereSql, inputModel.decreasetype);


            if (inputModel.platformType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" platformType = {0} ", inputModel.platformType) : string.Format(@" {0} and platformType = {1} ", whereSql, inputModel.platformType);

            if (inputModel.currencyType != -1) {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and b.currencyType = {1} ", whereSql, inputModel.currencyType);
            }

            whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.accountType = 0 ") : string.Format(@" {0} and a.accountType = 0 ", whereSql);
            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and b.merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and b.walletType = {1} ", whereSql, inputModel.walletType);
            }

            var leftjoinString = " left join ofUser b with(nolock) on a.account = b.account  left join ofUserExtention ue with(nolock) on a.account = ue.account ";
            var fields = " a.id as tid,b.id uid,a.account,a.amount,a.remarks,a.createtime,a.reviewtime,a.createperson,a.reviewperson,a.cancelreason,a.status,a.decreasetype,b.affaccount,b.anomaly,b.walletType,b.merchantAccount,ue.riskRemark,ue.riskTypesAready ";

            return await DapperPaging<QueryResizeOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }

        
        /// <summary>
        /// 创建微调
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreateResize(CreateResizeInputModel inputModel)
        {
            if((inputModel.decreasetype == 0 || inputModel.decreasetype == 1 || inputModel.decreasetype == 2) 
                && inputModel.amount > 0 
                && !string.IsNullOrEmpty(inputModel.account) 
                && !string.IsNullOrEmpty(inputModel.remarks))
            {

                var defModel = MerchantCodes.GetDefaultModel(inputModel.merchantAccount, inputModel.currencycode);
                if (defModel == null)
                    return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");

                var userModels = await DapperBase.QueryAsync<ofUserx>(UserSqlCommandText.LoadUserDatailSql_mer,
                    new
                    {
                        account = inputModel.account,
                        currencycode = inputModel.currencycode,
                        merchantAccount = inputModel.merchantAccount
                    }, commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);

                if(userModels != null && userModels.Count == 1)
                {
                    var um = userModels.First();
                    if (um.currencyType != inputModel.currencycode
                        || !um.merchantAccount.Equals(inputModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                    {
                        return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                    }
                    inputModel.r_f_uid = um.uid;
                    var sqlArray = new List<string>();
                    var param = new List<object> { };
                    inputModel.referenceguid = string.Format(@"{0}", Common.GuidToLongString());
                    sqlArray.Add(PromoSqlCommandText.CreateResizeSql_V);
                    param.Add(inputModel);
                    if (inputModel.decreasetype == 2)
                    {
                        sqlArray.Add(PromoSqlCommandText.SuppendUserAmountSql);
                        param.Add(new { account = inputModel.account, amount = inputModel.amount });
                    }
                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                            param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                    return new ApiResultModel<object>
                    {
                        code = result ? "true" : "false",
                        message = result ? "微调-成功" : "微调创建-失败",
                        message_code = result ? "promo_resize_create_success" : "promo_resize_create_failed"
                    };
                }
                else
                {
                    return new ApiResultModel<object>
                    {
                        code = "false",
                        message = "会员账号检测失败",
                        message_code = "promo_resize_create_failed"
                    };
                }


                   
            }
            else
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message ="优惠微调创建-失败[请确认提交信息是否正确]",
                    message_code = "promo_resize_create_failed"
                };
            }
        }

       


        //public static async Task<object> CreateResize_new(CreateResizeInputModel inputModel)
        //{
        //    if ((inputModel.decreasetype == 0 || inputModel.decreasetype == 1 || inputModel.decreasetype == 2)
        //        && inputModel.amount > 0 
        //        && !string.IsNullOrEmpty(inputModel.account) 
        //        && !string.IsNullOrEmpty(inputModel.remarks))
        //    {
        //        var sqlArray = new List<string>();
        //        var param = new List<object> { };
        //        inputModel.referenceguid = string.Format(@"resize{0}", Common.GuidToLongString());
        //        sqlArray.Add(PromoSqlCommandText.CreateResizeSql);
        //        param.Add(inputModel);
        //        if (inputModel.decreasetype == 2)
        //        {
        //            sqlArray.Add(PromoSqlCommandText.SuppendUserAmountSql);
        //            param.Add(new { account = inputModel.account, amount = inputModel.amount });
        //        }

        //        #region
        //        if(inputModel.appendType == 0)//存款补单，金额返还(提交金额和到账金额不一致)
        //        {
        //            //设置存款订单为取消状态
        //            //var cancelDepositOrderSql = string.Format(@" update ofDeposit set status=2 where account='{0}' and merchantno='{1}' and status = 0",
        //            //    inputModel.account, inputModel.orderNo);

        //            var cancelDepositOrderSql = string.Format(@" update ofDeposit set status=2,reviewperson='{2}',reviewtime='{3}' where account='{0}' and merchantno='{1}' and status = 0",
        //                inputModel.account, inputModel.orderNo, inputModel.adminAccount,DateTime.UtcNow.AddHours(8));
        //            sqlArray.Add(cancelDepositOrderSql);
        //            param.Add(null);
        //            //新增到账订单
        //            var insertDepositOrderSql = string.Format(@"");
        //        }
        //        else if(inputModel.appendType == 1)//出款金额返还
        //        {
        //            //出款失败，返还金额给客人，直接更新当前定单为失败状态，并返回出款金额到账号
        //        }
        //        #endregion


        //        var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
        //                param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

        //        return new ApiResultModel<object>
        //        {
        //            code = result ? "true" : "false",
        //            message = result ? "微调-成功" : "微调创建-失败",
        //            message_code = result ? "promo_resize_create_success" : "promo_resize_create_failed"
        //        };
        //    }
        //    else
        //    {
        //        return new ApiResultModel<object>
        //        {
        //            code = "false",
        //            message = "优惠微调创建-失败[请确认提交信息是否正确]",
        //            message_code = "promo_resize_create_failed"
        //        };
        //    }
        //}
       
        /// <summary>
        /// 会员微调审核
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifyResize(VerifyResizeInputModel inputModel)
        {
            
            var models = await DapperBase.QueryAsync<ofMinuEx>(PromoSqlCommandText.LoadResizeByIdSql,
                        new { tid=inputModel.tid}, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (models == null || models.Count <= 0)
                return ApiResultModel<object>.GetResult("false", "微调审核-失败", "promo_resize_verify_failed");

            if (models != null && models.Count > 0)
            {
                var sqlArray = new List<string>();
                var param = new List<object>();
                var model = models.First();
                var defModel = MerchantCodes.GetDefaultModel(model.merchantAccount, model.currencyType);
                var reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                if (model.decreasetype == 1 
                    || model.decreasetype == 0)//新增或扣除
                {
                    var preBalance = string.Format(UserSqlCommandText.preBalance, model.account);
                    var afterBalance = model.decreasetype == 0 ? string.Format(UserSqlCommandText.afterBalanceD, model.amount, model.account) 
                        : string.Format(UserSqlCommandText.afterBalanceA, model.amount, model.account);
                    var CreatePaymentSql = string.Format(PaymentSqlCommandText.CreatePaymentSql, preBalance, afterBalance);

                    #region 2023-12-02 add
                    var refguid = model.referenceguid.StartsWith("resize");//充值金额调整过来的, 要给会员累加充值金额和累积存款次数
                    var updateSql = (refguid && model.decreasetype == 1) 
                        ? UserSqlCommandText.UpdateBalanceAndDepositamountSql : UserSqlCommandText.UpdateBalanceASql;
                    #endregion
                    var updateBalaceSql = model.decreasetype == 1 ? updateSql : UserSqlCommandText.SubBalanceASql;
                    
                    sqlArray.Add(PromoSqlCommandText.UpdateResizeByIdSql_with_datetime);
                    sqlArray.Add(CreatePaymentSql);
                    sqlArray.Add(updateBalaceSql);
                    param.Add(new { status = 1, tid = inputModel.tid, reviewtime= reviewtime, reviewperson = inputModel.adminAccount, cancelreason = "" });
                    param.Add(new
                    {
                        currency = defModel.currency,// model.currency,
                        account = model.account,
                        transactiontype = model.decreasetype,
                        summary = model.remarks,
                        referenceguid = model.referenceguid,
                        bonus = model.amount,
                        status = 1,
                        summaryType = 6//微调
                    });
                    param.Add(new { amount = model.amount, account = model.account });

                }
                else //model.First().decreasetype == 2 //冻结
                {
                    sqlArray.Add(PromoSqlCommandText.UpdateResizeByIdSql);
                    param.Add(new { status = 1, tid = inputModel.tid, reviewperson = inputModel.adminAccount, cancelreason = "" });

                    sqlArray.Add(PromoSqlCommandText.SubSuppendAmountSql);
                    param.Add(new { account = model.account, amount = model.amount });

                }
                
                #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
                if (!defModel.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
                {
                    var tpSqls = await AmoutRecord.ConfirmBalanceAsync(model.account, defModel.merchantAccount, defModel.currencyType,
                        referenceguid: model.referenceguid,
                        r_type: 3, currentDate: reviewtime, remark: "后台余额微调");
                    if (!string.IsNullOrEmpty(tpSqls.Item1))
                    {
                        sqlArray.Add(tpSqls.Item1);
                        param.Add(tpSqls.Item2);
                    }
                }

                #endregion



                var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                    param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                return new ApiResultModel<object>
                {
                    code = result ? "true" : "false",
                    message = result ? "微调审核-成功" : "微调审核-失败",
                    message_code = result ? "promo_resize_verify_success" : "promo_resize_verify_failed"
                };
            }
            else
            {
                return new ApiResultModel<object>
                {
                    code = "false",
                    message = "优惠微调创建-失败[请确认提交信息是否正确]",
                    message_code = "promo_resize_create_failed"
                };
            }
        }
        
        /// <summary>
        /// 会员微调取消
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CancelResize(VerifyResizeInputModel inputModel)
        {
            var sqlArray = new List<string>();
            var param = new List<object> { };

            var model = await DapperBase.QueryAsync<ofMinu>(PromoSqlCommandText.LoadResizeByIdSql,
                        new { tid = inputModel.tid }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (model == null || model.Count <=0 )
                return ApiResultModel<object>.GetResult("false", "微调取消-失败", "promo_resize_cancel_failed");
            if(model.First().decreasetype == 2)//冻结后取消会返还金额给客人
            {
                sqlArray.Add(PromoSqlCommandText.AddUserAmountSql);
                param.Add(new { account = model.First().account, amount = model.First().amount });
            }

            sqlArray.Add(PromoSqlCommandText.UpdateResizeByIdSql);
            param.Add(new { tid = inputModel.tid, status = 2, reviewperson = inputModel.adminAccount, cancelreason = inputModel.cancelreason });

            var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                        param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return new ApiResultModel<object>
            {
                code = result ? "true" : "false",
                message = result ? "微调取消-成功" : "微调取消-失败",
                message_code = result ? "promo_resize_cancel_success" : "promo_resize_cancel_failed"
            };
        }

       
    }

}
