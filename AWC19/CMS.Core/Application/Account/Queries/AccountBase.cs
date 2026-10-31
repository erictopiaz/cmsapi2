using Dapper;
using DBLibs.WXDapper;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.Account.Queries {
    public class AccountBase {
        /// <summary>
        /// 获取会员账号根据id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string QueryAccountById(long id) {
            var queryAccountSql = string.Format(@"select account from ofUser with(nolock) where id={0}", id);
            var accountObj = DapperBase.ExecuteScalarValue(queryAccountSql, null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString);
            if (accountObj != null)
                return accountObj.ToString();
            else
                return "";
        }


        /// <summary>
        /// 查询会员充值数据统计 , 返回充、提总金额+充、提总次数
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public static IList<ofUserx> QueryUserWDReport(List<string> accounts)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var accountJoinString = "";
                foreach (var name in accounts)
                {
                    accountJoinString = string.IsNullOrEmpty(accountJoinString)
                        ? string.Format(@"'{0}'", name)
                        : string.Format(@"{0},'{1}'", accountJoinString, name);
                }


                var queryWDReportSql = string.Format(@"select a.account,a.t_amount depositamount,iif(a.depositnumber is not null, a.depositnumber,0) depositnumber,
  iif(b.withdrawalnumber is not null, b.withdrawalnumber, 0) withdrawalnumber, iif(b.w_amount is not null, w_amount, 0) withdrawalamount from (
 select aa.account,sum(aa.t_amount) t_amount,count(id) depositnumber from (
	 select id,account,amount t_amount from ofDeposit
	where account in ({0}) and status=1
	union all
	select id,account,amount t_amount from ofDeposit2014
	where account in ({0}) and status=1) aa
	group by aa.account
) a
left join (
select account,sum(amount) w_amount,count(id) withdrawalnumber from ofWithdrawal
where account in ({0}) and status=2
group by account) b on a.account=b.account", accountJoinString);
                var ofUsers = connection.Query<ofUserx>(queryWDReportSql);
                return (ofUsers != null && ofUsers.Count() > 0) ? ofUsers.ToList() : new List<ofUserx>();

            }
        }

    }
}
