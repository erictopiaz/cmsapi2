using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class UserServiceSqlText
    {
        /// <summary>
        /// 短信验证码查询
        /// </summary>
        public const string QueryVerifyCodeBySql = @"SELECT top 1 id, validate, CONVERT(VARCHAR(20), createtime, 120) createtime, status FROM ofAccountValidate WITH(NOLOCK) WHERE account = @account AND validate = @msgCode and type = @type ORDER BY createtime DESC";
        /// <summary>
        /// 代理，注册会员 
        /// </summary>
        public const string RegisterSql = @"INSERT INTO ofuser(currency,account,password,passkey,nickname,realname,birthday,country,status,level,telephone,email,referrer,referrerreg,ipaddress,macaddress,marcode,regdevice,affaccount,createBy,currencyType)
values(@currency,@account,@password,@passkey,@nickname,@realname,@birthday,@country,@status,@level,@telephone,@email,@referrer,@referrerreg,@ipaddress,@macaddress,@markcode,@regdevice,@affaccount,@createBy,@currencyType)";


        /// <summary>
        /// 获取代理信息根据账号
        /// </summary>
        public const string QueryAgentSql = @"select *,atree.billP as billPS from ofAffiliate a
left join ofAffiliateTree atree on a.account=atree.agentAccount
where account = @account";

		/// <summary>
		/// 代理，注册会员    品牌添加
		/// </summary>
		public const string RegisterSql_V = @"INSERT INTO ofuser(currency,account,password,passkey,nickname,realname,birthday,country,status,level,telephone,email,referrer,referrerreg,ipaddress,macaddress,marcode,regdevice,affaccount,createBy,currencyType,merchantAccount,walletType)
        values(@currency,@account,@password,@passkey,@nickname,@realname,@birthday,@country,@status,@level,@telephone,@email,@referrer,@referrerreg,@ipaddress,@macaddress,@markcode,@regdevice,@affaccount,@createBy,@currencyType,@merchantAccount,@walletType)";



		/// <summary>
		/// 返回最近的绑卡验证码
		/// </summary>
		public const string QueryAgentVerifyCodeSql = @"SELECT top 1 id, validate, CONVERT(VARCHAR(20), createtime, 120) createtime, status FROM ofAccountValidate WITH(NOLOCK) WHERE account = @account AND type = 3 and accountType=1 ORDER BY createtime DESC";
        /// <summary>
        /// 获取提款转换汇率 
        /// </summary>
        public const string QueryOTCRateVSql = @"select body from ofSettings with(nolock) where code='PAYMENT_CRYPTO' and status=1";
        /// <summary>
        /// 多货币兑换usdt 汇率
        /// </summary>
       // public const string QueryOTCSRateVSql = @"select body from ofSettings with(nolock) where code='PAYMENT_CRYPTO_USD' and status=1";

        /// <summary>
        /// 获取银行列表到缓存中
        /// </summary>
        public const string QueryOfBankBaseSqlV = @"select bid id,name,icon,url, category,withdrawType from ofBankBase where status=1 and currencycode={0} ";



        /// <summary>
        /// 代理提款申请
        /// 新加bankType 0:银行卡，1：虚拟卡
        /// </summary>
        public const string InsertAgentWithdrawSql = @"INSERT INTO [dbo].[ofAffWithdraw]
           ([account],[realname],[amount],[cardno],[cardaddress],[cardbank],[requestip],[requestaddress],[bankType],[rate])
     VALUES(@account,@realname,@amount,@cardno,@cardaddress,@cardbank,@requestip,@requestaddress,@bankType,@rate)";


        /// <summary>
        /// 查询代理本月结算佣金（数据上传后有会最新结算金额）inr
        /// </summary>
        public const string QueryAgentMonthlyCommissionSql = @"select report.*,oat.agentLevel,iif((oat.parentAccount is null or len(oat.parentAccount)<=0),'',oat.parentAccount) parentAccount,oat.billP,oat.isMultiAccount,oa.subTotalAmount,oa.unSettleAmount,oa.balance from (
select g.account,sum(g.depositFee+g.withdrawFee) totalFee,sum(g.bonus+g.promo+g.rebate) totalBonus,sum(g.depositFee) totalDeposit_fee,sum(g.withdrawFee) totalWithdraw_fee,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,
sum(pg_turnover+jdb_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover) totalTurnover,
sum(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) totalBet,
sum(pg_winlost+jdb_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost) totalWinlost,
sum(g.pg_bet) pg_bet,sum(g.jdb_bet) jdb_bet,sum(g.evo_bet) evo_bet,sum(g.b9w_bet) b9w_bet,sum(g.turbo_bet) turbo_bet,sum(g.spribe_bet) spribe_bet,sum(g.jl_bet) gg_bet, 
			 sum(g.pg_winlost) pg_winlost,sum(g.jdb_winlost) jdb_winlost,sum(g.evo_winlost) evo_winlost,sum(g.turbo_winlost) turbo_winlost,sum(g.b9w_winlost) b9w_winlost,sum(g.spribe_winlost) spribe_winlost,sum(g.jl_winlost) jl_winlost
			 from ofAffDailyReportFG g with (nolock)
			 where g.account=@affaccount and g.financeGameDay>=@startTime and g.financeGameDay<@endTime
			 group by account
			 ) report
			 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = report.account
			 left join ofAffiliate oa with(nolock) on oa.account = report.account
where oa.currencyType = 356";

        /// <summary>
        /// 查询代理本月结算佣金（数据上传后有会最新结算金额） cny
        /// </summary>
        public const string QueryAgentMonthlyCommissionSql_cny = @"select report.*,oat.agentLevel,iif((oat.parentAccount is null or len(oat.parentAccount)<=0),'',oat.parentAccount) parentAccount,oat.billP,oat.isMultiAccount,oa.subTotalAmount,oa.unSettleAmount,oa.balance from (
select g.account,sum(g.depositFee+g.withdrawFee) totalFee,sum(g.bonus+g.promo+g.rebate) totalBonus,sum(g.depositFee) totalDeposit_fee,sum(g.withdrawFee) totalWithdraw_fee,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,
sum(imsp_turnover+imes_turnover+fb_turnover+tf_turnover+ag_turnover+lb_turnover+ky_turnover+xj_turnover) totalTurnover,
sum(imsp_bet+imes_bet+fb_bet+tf_bet+ag_bet+lb_bet+ky_bet+xj_bet) totalBet,
sum(imsp_winlost+imes_winlost+fb_winlost+tf_winlost+ag_winlost+lb_winlost+ky_winlost+xj_winlost) totalWinlost,
sum(g.imsp_bet) imsp_bet,sum(g.imes_bet) imes_bet,sum(g.fb_bet) fb_bet,sum(g.tf_bet) tf_bet,sum(g.ag_bet) ag_bet, sum(g.lb_bet) lb_bet,sum(g.ky_bet) ky_bet,sum(g.xj_bet) xj_bet,  
			 sum(g.imsp_winlost) imsp_winlost,sum(g.imes_winlost) imes_winlost,sum(g.fb_winlost) fb_winlost,sum(g.tf_winlost) tf_winlost,sum(g.ag_winlost) ag_winlost,sum(g.lb_winlost) lb_winlost,sum(g.ky_winlost) ky_winlost,sum(g.xj_winlost) xj_winlost
			 from ofAffDailyReportFG g with (nolock)
			 where g.account=@affaccount and g.financeGameDay>=@startTime and g.financeGameDay<@endTime
			 group by account
			 ) report
			 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = report.account
			 left join ofAffiliate oa with(nolock) on oa.account = report.account
where oa.currencyType = 156";

        /// <summary>
        /// 合营活跃人数
        /// </summary>
        public const string allowedSettleSqlV1 = @" select affaccount,count(affaccount) cnt
from 
(
select fg.account,fg.affaccount,SUM(depositAmount) totalDeposit,
		sum(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) totalBet
from ofPlayerDailyReportFG fg with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = fg.affaccount 
where fg.affaccount=@affaccount and financeGameDay>=@startTime and financeGameDay<@endTime and oa.currencyType = 356 
group by fg.account ,fg.affaccount
) a
where a.totalDeposit>=500 and a.totalBet >= 1000 
group by affaccount  ";

        /// <summary>
        /// 合营活跃人数
        /// </summary>
        public const string allowedSettleSqlV1_cny = @" select affaccount,count(affaccount) cnt
from 
(
select fg.account,fg.affaccount,SUM(depositAmount) totalDeposit,
		sum(imsp_bet+imes_bet+fb_bet+tf_bet+ag_bet+lb_bet+ky_bet+xj_bet) totalBet
from ofPlayerDailyReportFG fg with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = fg.affaccount 
where fg.affaccount=@affaccount and financeGameDay>=@startTime and financeGameDay<@endTime and oa.currencyType = 156 
group by fg.account ,fg.affaccount
) a
where a.totalDeposit>=500 and a.totalBet >= 1000 
group by affaccount  ";

        /// <summary>
        /// 指定时间内，会员充值金额大多少
        /// 返回  会员号和充值总金额
        /// </summary>
        public const string depositAmountRangeSql = @"select account, sum(amount) totalAmount from ofdeposit d with(nolock)
where requesttime>='{0}' and requesttime<'{1}' re and status=1 and account in ({2})
group by account
having sum(amount) >=@totalAmount";

        /// <summary>
        /// 20241118 加  rsg
        /// </summary>
        public const string marketReportSql = @"select * from 
	(
		select count(account) activityUserCount from
		(
		select allk.account,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet) as bets,
		sum(turnover) as turnovers,sum(allk.depositAmount) depositAmount
		from ofPlayerDailyReportFG allk with(nolock)
		where allk.financeGameDay>=@startTime and allk.financeGameDay<=@endTime 
		 and
		allk.affaccount in ({0})
		group by allk.account
		) c
		where c.depositAmount >= 500 and c.turnovers > 0 and c.bets >= 1000
	) d
	,(select count(id) userCount from ofUser with(nolock) where affaccount in ({0}) and status=1) e
	,(select count(id) totalRegisterNumber from ofUser with(nolock) where cast(format(createtime,'yyyyMMdd') as int)>=@startTime and cast(format(createtime,'yyyyMMdd') as int)<=@endTime and  affaccount in ({0}) and status=1) g
	,(select count(id) totalFirstDepositNumber from userFirstDepositDetail with(nolock) where depositDay>=@startTime and depositDay<=@endTime and  affaccount in ({0}) ) h
	,(
		select iif(totalTurnover is not null, totalTurnover,0) totalTurnover, 
		 iif(totalDepositAmount is not null, totalDepositAmount,0) totalDepositAmount, 
		 iif(totalWithdrawAmount is not null, totalWithdrawAmount,0) totalWithdrawAmount,
		 iif(totalDepositFee is not null, totalDepositFee,0) totalDepositFee,
		 iif(totalWithdrawFee is not null, totalWithdrawFee,0) totalWithdrawFee,
		  iif(fee is not null, fee,0) fee,
		  iif(totalRealBankFee is not null, totalRealBankFee,0) totalRealBankFee,
		  iif(totalBonus is not null, totalBonus,0) totalBonus,
		  iif(totalPromo is not null, totalPromo,0) totalPromo,
		  iif(totalWinlost is not null, totalWinlost,0) totalWinlost,
		  iif(totalBet is not null, totalBet,0) totalBet from (
		 select sum(turnover) totalTurnover,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,SUM(depositFee) totalDepositFee,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,sum(realBankFee) totalRealBankFee,sum(bonus+promo+rebate) totalBonus,sum(promo) totalPromo,
		  sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost) totalWinlost,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet) totalBet
		 from 
		 (
			 select g.account, g.financeGameDay,g.depositAmount,g.withdrawAmount,g.depositFee,g.withdrawFee,g.realBankFee,g.bonus,g.turnover,g.promo,g.rebate,
			 g.ea_bet,g.kg_bet,g.sb_bet,g.bbin_bet,g.kg2_bet,g.pt_bet,g.ttg_bet,g.gg_bet,g.ag_bet,g.vr_bet,g.xj_bet,g.imes_bet,g.ob_bet,g.ysb_bet,g.pg_bet,g.lb_bet,g.ky_bet,g.rg_bet,g.dscp_bet,g.dses_bet,g.bg_bet,g.eb_bet,g.td_bet,g.tf_bet,g.aw_bet,g.cq_bet,g.kg3_bet,g.obg_bet,g.imsp_bet,g.sw_bet,g.cr_bet,g.obcv_bet,g.fb_bet,g.jdb_bet,g.db_bet,g.rsg_bet,
			 g.ea_winlost,g.kg_winlost,g.sb_winlost,g.bbin_winlost,g.kg2_winlost,g.pt_winlost,g.ttg_winlost,g.gg_winlost,g.ag_winlost,g.vr_winlost,g.xj_winlost,g.imes_winlost,g.ob_winlost,g.ysb_winlost,g.pg_winlost,g.lb_winlost,g.ky_winlost,g.rg_winlost,g.dscp_winlost,g.dses_winlost,g.bg_winlost,g.eb_winlost,g.td_winlost,g.tf_winlost,g.aw_winlost,g.cq_winlost,g.kg3_winlost,g.obg_winlost,g.imsp_winlost,g.sw_winlost,g.cr_winlost,g.obcv_winlost,g.fb_winlost,g.jdb_winlost,g.db_winlost,g.rsg_winlost
			 from ofAffDailyReportFG g with(nolock)
		 ) aaa
		 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = aaa.account 
		 where aaa.financeGameDay>=@startTime and aaa.financeGameDay<=@endTime and oat.agentAccount in ({0}) ) dda
	 ) f
	 ,(
		select iif(sum(amount) is not null, sum(amount), 0) billedDailyAmount, iif(sum(profit) is not null, sum(profit), 0) billedProfit from afBillReport br with(nolock)
		left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
		where billDay>=@startTime and billDay<=@endTime and oat.agentAccount in ({0})
	 ) ff
	 ,(
	 select count(id) depositCount from ofPlayerDailyReportFG allk with(nolock) 
where allk.financeGameDay>=@startTime and allk.financeGameDay<=@endTime 
		 and allk.depositAmount>0 and
		allk.affaccount in ({0})
	 ) dd
	 OPTION (RECOMPILE)";

    }
}
