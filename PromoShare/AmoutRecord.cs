using Ching.DistributedTransaction.EFModel;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoShare
{
   
    public class AmoutRecord
    {
        /// <summary>
        /// postgre 地址，可以外部传入
        /// </summary>
        private static readonly string postgreSql = "";
        
        /// <summary>
        /// 余额节点确认
        /// </summary>
        /// <param name="account">会员账号</param>
        /// <param name="merchantAccount">品牌</param>
        /// <param name="currencyType">货币</param>
        /// <param name="referenceguid">关链guid</param>
        /// <param name="r_type">0：充值，1：红利优惠，2：反水, 3:微调, 4: 提款后（暂无使用）</param>
        /// <param name="currentDate">操作确认时间 yyyy-MM-dd HH:mm:ss</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public static async Task<Tuple<string,object>> ConfirmBalanceAsync(string account, 
            string merchantAccount, 
            int currencyType,
            string referenceguid,
            int r_type,
            string currentDate,
            string remark)
        {
            #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
            if (!merchantAccount.Equals("Bayo", StringComparison.OrdinalIgnoreCase))
            {
                var gameWallet = 0.0m;
                using (var context = new AmountRecordDbContext(DapperStringBase.ConnectionGamePostgreString))
                {
                    var user = context.UserGameWalletPostgres.Where(x=>x.account == account.ToLower()).FirstOrDefault();
                    if (user != null)
                    {
                        //找到
                        gameWallet = user.wallet;
                    }
                    else
                    {
                        //不存在会员
                        //return new Tuple<string, object>("", null);
                    }
                }

                var walletSql = string.Format(@"select amountactive from ofUser where account='{0}'", account);
                var totalWallet = 0m;
                var walletModel = await DapperBase.QueryAsync<dynamic>(walletSql, null,
                               commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                if (walletModel != null && walletModel.Count > 0)
                {
                    totalWallet = walletModel.First().amountactive + gameWallet;
                    if (totalWallet <= 10)
                    {
                        var WithdrawalVerifyTimelineParams = new
                        {
                            account = account,
                            ref_guid = referenceguid,
                            currentAmount = totalWallet,
                            r_type = r_type, //0：充值，1：红利优惠，2：反水
                            r_time = currentDate,
                            remark = remark,// "后台红利优惠到账",
                            currencyType = currencyType
                        };
                        var insertWithdrawalVerifyTimelineSql = string.Format(@"INSERT INTO [dbo].[ofWithdrawalVerifyTimeline]
           ([account],[ref_guid],[currentAmount],[r_type],[r_time],[remark],[currencyType])
     VALUES
           (@account, @ref_guid, @currentAmount,@r_type, @r_time, @remark, @currencyType)");

                        return new Tuple<string, object>(insertWithdrawalVerifyTimelineSql, WithdrawalVerifyTimelineParams);
                    }
                }
                return new Tuple<string, object>("", null);
            }
            else
            {
                return await ConfirmBalanceAsync_Bayo(account, merchantAccount,
                    currencyType, referenceguid, r_type, currentDate, remark);
            }
            #endregion
           
        }



        /// <summary>
        /// 余额节点确认
        /// </summary>
        /// <param name="account">会员账号</param>
        /// <param name="merchantAccount">品牌</param>
        /// <param name="currencyType">货币</param>
        /// <param name="referenceguid">关链guid</param>
        /// <param name="r_type">0：充值，1：红利优惠，2：反水</param>
        /// <param name="currentDate">操作确认时间 "yyyy-MM-dd HH:mm:ss"</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public static async Task<Tuple<string, object>> ConfirmBalanceAsync_Bayo(string account,
            string merchantAccount,
            int currencyType,
            string referenceguid,
            int r_type,
            string currentDate,
            string remark)
        {

            #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28
            var walletSql = string.Format(@"select amountactive,uc.wallet  from ofUser u
left join ofUserGameWallet uc on u.account = uc.account
where u.account='{0}'", account);
            var totalWallet = 0m;
            var walletModel = await DapperBase.QueryAsync<dynamic>(walletSql, null,
                           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            if (walletModel != null && walletModel.Count > 0)
            {
                totalWallet = walletModel.First().amountactive + walletModel.First().wallet;
                if (totalWallet <= 10)
                {
                    var WithdrawalVerifyTimelineParams = new
                    {
                        account = account,
                        ref_guid = referenceguid,
                        currentAmount = totalWallet,
                        r_type = 1, //0：充值，1：红利优惠，2：反水
                        r_time = currentDate,
                        remark = remark, //"后台红利优惠到账",
                        currencyType = currencyType
                    };

                    var insertWithdrawalVerifyTimelineSql = string.Format(@"INSERT INTO [dbo].[ofWithdrawalVerifyTimeline]
           ([account],[ref_guid],[currentAmount],[r_type],[r_time],[remark],[currencyType])
     VALUES
           (@account, @ref_guid, @currentAmount,@r_type, @r_time, @remark, @currencyType)");


                    return new Tuple<string, object>(insertWithdrawalVerifyTimelineSql, WithdrawalVerifyTimelineParams);
                }
            }
            
            return new Tuple<string, object>("", null);
            #endregion
        }
    }
}
