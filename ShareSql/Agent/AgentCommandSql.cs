using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareSql.Agent
{
    public class AgentCommandSql
    {
        /// <summary>
        /// 代理账变sql
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="summary">描述备注子代理</param>
        /// <param name="reward">账变金额</param>
        /// <param name="transactiontype">新增或扣除0：扣除，1：增加</param>
        /// <param name="summaryType">1000:月结，1001：人头奖励，1002：保底，1003：节日红包， 
        /// 1004：提款，1005：未结算金额清零, 1006:代理划款给会员， 1007:代理发放优惠给会员 </param>
        /// <param name="referenceguid">guid</param>
        /// <param name="currency">默认为cny</param>
        /// <returns></returns>
        public static Tuple<bool, string, object> AmountChange(string account,
            string summary, string reward, int transactiontype, int summaryType, string referenceguid, string currency= "CNY")
        {
            try
            {
                //账变
                var preBalance = string.Format(UserSqlCommandText.preAgentBalance, account);
                var afterBalance = string.Format(UserSqlCommandText.afterAgentBalanceA, reward, account);
                var CreatePaymentSql = string.Format(PaymentSqlCommandText.CreatePaymentWithRewardSql, preBalance, reward, afterBalance);

                return new Tuple<bool, string, object>(true, CreatePaymentSql, new
                {
                    currency = currency,
                    account = account,
                    transactiontype = transactiontype,
                    summary = summary,// ? "风控返水审核" : "返水审核",
                    referenceguid = referenceguid, //Common.GuidToLongString(),
                    //bonus = reward,
                    status = 1,
                    summaryType = summaryType//代理账变
                });
            }
            catch (Exception)
            {
                //账变
                return new Tuple<bool, string, object>(false, "账变更新失败!", null);
            }


        }


    }
}
