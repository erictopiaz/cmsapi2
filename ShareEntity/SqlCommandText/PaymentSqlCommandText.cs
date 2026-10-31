using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class PaymentSqlCommandText
    {
        /// <summary>
        /// 创建账变记录
        /// </summary>
        public const string CreatePaymentSql = @"INSERT INTO [dbo].[ofPaymentRecord]
           ([currency],[account],[transactiontype],[summary],[referenceguid],[preBalance],[bonus],[afterBalance],[status],[summaryType])
     VALUES (@currency,@account,@transactiontype,@summary,@referenceguid,{0},@bonus,{1},@status,@summaryType)";
        /// <summary>
        /// 创建账变记录
        /// 账变金额为sql
        /// </summary>
        public const string CreatePaymentWithRewardSql = @"INSERT INTO [dbo].[ofPaymentRecord]
           ([currency],[account],[transactiontype],[summary],[referenceguid],[preBalance],[bonus],[afterBalance],[status],[summaryType])
     VALUES (@currency,@account,@transactiontype,@summary,@referenceguid,{0},{1},{2},@status,@summaryType)";

        /// <summary>
        /// 当天和昨天充值统计daytype（0:当天，1：昨日）
        /// </summary>
        public const string DepositReportSql = @"
select * from (
SELECT account, sum(amount) amount,daytype=0 FROM  ofDeposit with(nolock)
	where account='{0}' and status=1 and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=0 
    GROUP BY account) a
	union all 
	(
SELECT account, sum(amount) amount,daytype=1 FROM  ofDeposit with(nolock)
	where account='{0}' and status=1 and DateDiff(dd,ISNULL(orderendtime,reviewtime),DATEADD(hour,8,getutcdate()))=1
    GROUP BY account) ";

    }

}
