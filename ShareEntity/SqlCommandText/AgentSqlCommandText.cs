using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    /// <summary>
    /// 代理sql语句
    /// </summary>
    public class AgentSqlCommandText
    {
        /// <summary>
        /// 获取上级代理基本信息
        /// 根据当前代理注册的HOST
        /// </summary>
        public const string QueryAgentModelSql = @"select * from ofAffiliate where website like CONCAT('%',(select registerUrl from ofAffiliate where account=@agentAccount))";

        /// <summary>
        /// 代理注册
        /// </summary>
        public const string RegisterAgentSql = @"if not exists( Select account from ofAffiliate Where account=@account)
		begin
			INSERT INTO[dbo].[ofAffiliate]
        ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[ipaddress],[macaddress],[createperson],[registerUrl])
    VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@host);
		end";

        /// <summary>
        /// 代理注册
        /// </summary>
    //    public const string RegisterAgentSql = @"INSERT INTO[dbo].[ofAffiliate]
    //    ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[adcode],[ipaddress],[macaddress],[createperson],[registerUrl])
    //VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@registerUrl)";
        
        /// <summary>
        /// 查询代理密码
        /// </summary>
        public const string QueryAgentPasswordSql = @"select password from [dbo].[ofAffiliate] where account=@account";
        
     
        /// <summary>
        /// 获取代理信息根据账号
        /// ofAffiliate（所有列）
        /// ofAffiliateTree(除id 所有列)
        /// </summary>
        public const string QueryAgentSql_V = @"select a.*,atree.[agentId],atree.[agentAccount],atree.[parentId],atree.[agentLevel],atree.[allowCreateAgent],atree.[agentIds],atree.[platformFeeP],atree.[billP],atree.[isLeaf],atree.[parentAccount],atree.[agentAccounts],atree.[isMultiAccount] from ofAffiliate a
left join ofAffiliateTree atree on a.account=atree.agentAccount
where account = @account";


        public const string QueryAgentByAccountSql = @"select * from ofAffiliate a
where account = {0} ";

        /// <summary>
        /// 获取代理信息 根据id
        /// </summary>
        public const string QueryAgentSqlById = @"SELECT a.[currency],a.[account] ,a.[bbaccount],a.[password],a.[realname] ,a.[sex],a.[birthday],a.[country],a.[email],a.[telephone],a.[qq] ,a.[website] ,a.[adlink],a.[riskTypeAready],a.[riskRemark]
      ,a.[adcode],a.[cardno] ,a.[cardname],a.[cardaddress],a.[ew_uuid] ,a.[pt_uuid] ,a.[sun_uuid] ,a.[remarks],a.[status],a.[verify],a.[createtime] ,a.[createperson],a.[subTotalAmount],a.[unSettleAmount],a.[playerBonus],a.[salaryBonus],a.[festivalBonus]
      ,a.[alloc],a.[verifyperson],a.[verifytime],a.[balance],a.agentMarket,a.billPromoMonth,a.[unavailable],a.[ipaddress],a.[macaddress],a.[lastipaddress],a.[lastmacaddress],a.[adlinkhit],a.[adcodelock],a.[commission] ,a.[upperlevel],a.[defaultAgent],a.[registerUrl],a.[parentAdcode],a.[disabledMids],a.[transferAllowed],a.[depositAllowed],atree.*
  FROM [dbo].[ofAffiliate] a
  left join ofAffiliateTree atree on a.account=atree.agentAccount
where atree.agentId = @id";
        /// <summary>
        /// 合营风迭级别sql
        /// </summary>
        public const string QueryAgentRiskConfigSql = @"select body from ofSettings with(nolock)
where code='aff_risk_config_setting'";
        /// <summary>
        /// 根据代理ID，获取注册代理信息
        /// </summary>
        public const string QueryViewAgentSqlById = @"select * from ofAffiliate a
where a.id = @id and a.status=0 and a.verify=0 ";



        /// <summary>
        /// 管理员修改代理密码
        /// </summary>
        public const string UpdateAgentPasswordSql = @"update [ofAffiliate] set password=@password where account=@account ";

        /// <summary>
        /// 管理员修改代理风控等级
        /// </summary>
        public const string UpdateAgentRiskSql = @"update [ofAffiliate] set riskTypeAready=@riskTypeAready,riskRemark=@riskRemark where account=@account ";

        /// <summary>
        /// 代理提交修改密码
        /// </summary>
        public const string UpdatePasswordByAgentSql = @"update [ofAffiliate] set password=@password where account=@account and password=@oldPwd";

        /// <summary>
        /// 更新代理登录IP 和MAC
        /// </summary>
        public const string UpdateAgentLoginSql = @"update [ofAffiliate] set ipaddress=@ipaddress, macaddress=@macaddress, ,lastLoginDate=@lastLoginDate where account=@account";
        /// <summary>
        /// 代理提款申请
        /// </summary>
        public const string InsertAgentWithdrawSql = @"INSERT INTO [dbo].[ofAffWithdraw]
           ([account],[realname],[amount],[cardno],[cardaddress],[cardbank],[requestip],[requestaddress])
     VALUES(@account,@realname,@amount,@cardno,@cardaddress,@cardbank,@requestip,@requestaddress)";
        /// <summary>
        /// 提款时更新代理余额
        /// 并冻结提款的金额
        /// </summary>
        public const string UpdateAgentBalaceSql = @"update ofAffiliate set balance = balance-@amount  where balance-@amount >= 0 and account=@account";

        /// <summary>
        /// 提款取消时还款到主账号
        /// </summary>
        public const string ReturnAgentBalanceSql = @"update ofAffiliate set balance = balance+(select amount from ofAffWithdraw where id=@id and status <> 3 )  where account =(select account from ofAffWithdraw where id=@id and status <> 3)";

        /// <summary>
        /// 结算定单审核时更新代理钱包
        /// </summary>
        public const string UpdateAgentBillSql = @"update ofAffiliate 
set balance = balance + (select amount from ofAffBillReport where id=@id and status=0),
subTotalAmount = subTotalAmount + (select subTotalAmount from ofAffBillReport where id=@id and status=0),
unSettleAmount = unSettleAmount + (select unSettleAmount from ofAffBillReport where id=@id and status=0)  
where account=(select account from ofAffBillReport where id=@id and status=0) ";
        public const string UpdateAgentBillStatusSql = @"update ofAffBillReport set status=1 ,verifycustomer=@adminAccount,verifytime=@verifyTime where status=0 and id=@id";
        /// <summary>
        /// 人工代理结算
        /// </summary>
        public const string CreateAgentBillSql = @"INSERT INTO [dbo].[ofAffBillReport]
           ([account],[amount],[subTotalAmount],[unSettleAmount],[billedTime],[remark],[createcustomer],[grossamount],[proportion],[platformfee],[turnover],[promotion],[bonus],[withdrawfee],[depositfee],[ipaddress],[afpaymentlivetype],[settleType])
     VALUES (@account,@amount,@subTotalAmount,@unSettleAmount,@billedTime,@remark,@createcustomer,@grossamount,@proportion,@platformfee,@turnover,@promotion,@bonus,@withdrawfee,@depositfee,@ipaddress,@afpaymentlivetype,@settleType)";
        #region
        //        /// <summary>
        //        /// 未审核前编辑 
        //        /// </summary>
        //        public const string ModifyAgentBillSql_old = @"UPDATE [dbo].[ofAffBillReport]
        //            set [account]=@account,[amount]=@amount,[billedTime]=@billedTime,[remark]=@remark,[createcustomer]=@createcustomer,[grossamount]=@grossamount,[proportion]=@proportion,[subTotalAmount]=@subTotalAmount,[unSettleAmount]=@unSettleAmount,
        //[platformfee]=@platformfee,[turnover]=@turnover,[promotion]=@promotion,[bonus]=@bonus,[withdrawfee]=@withdrawfee,[depositfee]=@depositfee,[ipaddress]=@ipaddress,[afpaymentlivetype]=@afpaymentlivetype,[settleType]=@settleType,[totalSettleAmount]=@totalSettleAmount
        //where status=0 and id=@tid ";

        /// <summary>
        /// 未审核前编辑  2026-06-17 添加编辑unSettleAmountAfter  
        /// </summary>
        public const string ModifyAgentBillSql_V = @"UPDATE [dbo].[ofAffBillReport]
            set [account]=@account,[amount]=@amount,[billedTime]=@billedTime,[remark]=@remark,[createcustomer]=@createcustomer,[grossamount]=@grossamount,[proportion]=@proportion,[subTotalAmount]=@subTotalAmount,[unSettleAmount]=@unSettleAmount,[unSettleAmountAfter]=@unSettleAmountAfter,
[platformfee]=@platformfee,[turnover]=@turnover,[promotion]=@promotion,[bonus]=@bonus,[withdrawfee]=@withdrawfee,[depositfee]=@depositfee,[ipaddress]=@ipaddress,[afpaymentlivetype]=@afpaymentlivetype,[settleType]=@settleType,[totalSettleAmount]=@totalSettleAmount
where status=0 and id=@tid ";

        #endregion

        /// <summary>
        /// 加载当前结算记录
        /// 根据传的ID
        /// </summary>
        public const string LoadAgentBillSql = @"select b.*,a.isMultiAccount,a.parentAccount from ofAffBillReport b
left join ofAffiliateTree a on b.account= a.agentAccount
where b.id = @tid ";

        public const string LoadAgentBillWithStatusSql = @"select * from ofAffBillReport where id = @tid and status=0 ";
        /// <summary>
        /// 获取之前的结算记录
        /// </summary>
        public const string LoadAgentBillRecordsPreSql = @"select top 1 * from ofAffBillReport where account='{0}' and settleType=0 and billedTime < {1} order by billedTime desc";

        /// <summary>
        /// 确认当前代理是否可以结算
        ///  新加  pp, iaes  20260711  (jdb也参与)
        /// jdb 不计算 活跃人数
        /// </summary>
        public const string IsAlloweSettleSql = @"select affaccount
from 
(
select fg.account,fg.affaccount,SUM(depositAmount) totalDeposit,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+jdb_bet+pp_bet+iaes_bet) totalBet
from ofPlayerDailyReportFG fg with(nolock)
where financeGameDay>={0} and financeGameDay<{1}
group by fg.account ,fg.affaccount
) a
where a.totalDeposit > 1000 and a.totalBet > 1000 and affaccount='{2}'
group by affaccount
having count(affaccount) >= 5 ";




        /// <summary>
        /// 获取达到到结算条件的代理账号
        /// 新加  pp, iaes  20260711
        /// 新增： obg_bet+imsp_bet+obcv_bet+sw_bet+impk_bet
        /// 20221201 fb jdb
        /// 20221201 jdb 不能与活跃人数据统计  
        /// 20240301 开始区分货币 加db
        /// 20241118 加 rsg
        /// 20250205 加 bti
        /// 20260205 加 dbkeno
        /// 区分：品牌+货币
        /// </summary>
        public const string allowedSettleSqlV1_V_RealTime = @"select affaccount,count(affaccount) cnt
from 
(
select fg.account,fg.affaccount,SUM(depositAmount) totalDeposit,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet
		+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+jdb_bet+pp_bet+iaes_bet) totalBet
from ofPlayerDailyReportFG_RealTime fg with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = fg.affaccount 
where financeGameDay>=@starttime and financeGameDay<@endtime and oa.currencyType = 156 and oa.a_merchantAccount='{0}'
group by fg.account ,fg.affaccount
) a
where a.totalDeposit>=1000 and a.totalBet >= 1000 
group by affaccount ";


        /// <summary>
        /// 代理开启或停用
        /// </summary>
        public const string AgentDisableOrEnableSql = @"update ofAffiliate set status=@status where account=@account";
        /// <summary>
        /// 更新代理的结算比例
        /// </summary>
        public const string UpdateAgentBillP = @"update [ofAffiliate] set billP=@billP,allowCreateAgent=@allowCreateAgent where account=@account";
        
        /// 更新代理的结算比例 新版
        /// </summary>
        //public const string UpdateAgentAdlinkV_old = @"update [ofAffiliate] set adlink=@links, adcode=@adcode,parentAdcode=@parentAdcode, depositAllowed=@depositAllowed, transferAllowed=@transferAllowed,agentMarket=@agentMarket,billPromoMonth=@billPromoMonth,billPromoP=@billPromoP where account=@account";
        
        /// 更新代理的结算比例 新版
        /// </summary>
        public const string UpdateAgentAdlinkV_New = @"update [ofAffiliate] set adlink=@links, adcode=@adcode,parentAdcode=@parentAdcode, depositAllowed=@depositAllowed, transferAllowed=@transferAllowed,agentMarket=@agentMarket,billPromoMonth=@billPromoMonth,billPromoP=@billPromoP,remarks=CONCAT(remarks,',',@remarks) where account=@account";

        /// <summary>
        /// 更新代理的结算比例
        /// </summary>
        public const string UpdateAgentTreeBillP = @"update [ofAffiliateTree] set billP=@billP,allowCreateAgent=@allowCreateAgent,isMultiAccount=@isMultiAccount where agentAccount=@account";
        /// <summary>
        /// 更新代理的基本信息
        /// </summary>
        public const string UpdateAgentBaseInfo = @"update [ofAffiliate] {0} where account='{1}'";
        public const string UpdateAgentTree = @"update [ofAffiliateTree] {0} where agentAccount='{1}'";
        /// <summary>
        /// 代理示结算金额清0,如果未结算金额为0
        /// </summary>
        public const string RemoveUnSettleAmountSql = @"update [ofAffiliate] set unSettleAmount=0 where account='{0}' and unSettleAmount < 0 ";


        /// <summary>
        /// 取消代理结算
        /// </summary>
        public const string CancelAgentBillSql = @"update ofAffBillReport set status=2,verifycustomer=@adminAccount,verifytime=@verifyTime,cancelRemarks=@cancelRemarks where id=@id";
        /// <summary>
        /// 更新提款账单状态
        /// 0：等审核
        /// 1：等支付
        /// 2：支付成功
        /// 3：支付取消
        /// </summary>
        public const string VerifyAgentWithdrawSql = @"update ofAffWithdraw set status={0}, reviewperson='{1}',reviewtime='{2}'  where id={3} and status = {4}";
        public const string PayAgentWithdrawSql = @"update ofAffWithdraw set status={0}, payperson='{1}',paytime='{2}' where id={3} and status = {4}";
        public const string CancelAgentWithdrawSql = @"update ofAffWithdraw set status=3, payperson=@payperson,paytime=@paytime, cancelreason=@cancelreason where id=@id";
        /// <summary>
        /// 获取代理统计数据
        /// </summary>
        public const string QueryAgentDataSql = @"select 
(select count(id) from ofUser where createtime >=@startTime and createtime<@endTime {0}) as registerNumber,
(select sum(amount) from ofDeposit 
where requesttime >=@startTime and requesttime <@endTime and ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime) <@endTime  and status=1 {0}) as depositTotal,
(select sum(amount) from ofWithdrawal where requesttime>=@startTime and requesttime<@endTime and paytime>=@startTime and paytime<@endTime and status=2 {0} ) as withdrawTotal,
(select count(pr.id) from ofPaymentRecord pr left join ofUser u on pr.account=u.account 
where pr.createtime>=@startTime and pr.createtime<@endTime and preBalance=0 and summaryType=1 {0}) as firstDepositNumber,
(select count(id) from ofDeposit 
where requesttime >=@startTime and requesttime <@endTime and ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime) <@endTime  and status=1 {0}) as depositUserTotal";

        public const string QueryAgentDataReportSql = @"select *,oat.agentId as aid from (
select account,sum(registerNumber) registerNumber ,sum(firstDepositNumber) firstDepositNumber,sum(depositTotal) depositTotal,sum(withdrawTotal) withdrawTotal,sum(visterTimes) visterTimes,sum(depositUserTotal) depositUserTotal 
from ofAffDataReport  with(nolock)
where reportDay >= @st and reportDay<=@et
group by account ) a left join ofAffiliateTree oat on a.account=oat.agentAccount
where account=@account ";


        /// <summary>
        /// 根据代理账号获取代理账号上级层级
        /// </summary>
        public const string LoadAgentAccounts = @"select agentAccounts from ofAffiliateTree where agentAccount=@agentAccount ";
        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// 财务报表
        /// </summary>
        public const string FinanceReportSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex 
order by TT.winlost desc ";


        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20210101  kg3
        /// 20211122  obg, imsp, sw, cr obcv
        /// 增加fb 平台  20221201  jdb 
        /// 增加DB 平台，2024-04-15
        /// 增加RSG 平台，2024-11-16
        /// 加bti 2025-02-05
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportSqlV1 = @"SELECT TT.*, TT.account as prefix_account, oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,
SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost
from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_winlost) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {1}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
WHERE TT.Row between @startIndex and @endIndex 
order by TT.winlost desc ";

        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20210101  kg3
        /// 20211122  obg, imsp, sw, cr obcv
        /// 增加fb 平台  20221201  jdb 
        /// 增加DB 平台，2024-04-15
        /// 增加RSG 平台，2024-11-16
        /// 加bti 2025-02-05
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportSqlV1_RealTime = @"SELECT TT.*, TT.account as prefix_account, oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,
SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost
from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {1}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
WHERE TT.Row between @startIndex and @endIndex 
order by TT.winlost desc ";


        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// 财务报表
        /// </summary>
        public const string FinanceReportUnionSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {0}

union

select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {1}

) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex 
order by TT.winlost desc ";



        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20210101 kg3
        /// 20211122 obg,imsp,sw, cr
        /// 增加fb 平台  20221201 jdb
        /// 增加 Db 平台  2024-04-15
        /// 增加 rsg 平台  2024-11-16
        /// 加bti 2025-02005
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportUnionSqlV1 = @"SELECT TT.*, TT.account as prefix_account, oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,
SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,sum(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost
from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {2}

union all

select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {1} {2}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc ";

        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20210101 kg3
        /// 20211122 obg,imsp,sw, cr
        /// 增加fb 平台  20221201 jdb
        /// 增加 Db 平台  2024-04-15
        /// 增加 rsg 平台  2024-11-16
        /// 加bti 2025-02005
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportUnionSqlV1_RealTime = @"SELECT TT.*, TT.account as prefix_account, oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(withdrawFee) totalWithdrawFee,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,
SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,sum(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost
from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {2}

union all

select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets,
ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {1} {2}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc ";


        /// <summary>
        /// 财务报表统计总计录
        /// </summary>
        public const string FinanceReportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT ";
        /// <summary>
        /// 合并后版本
        /// 新加： pp iaes  20260711
        /// 增加fb 平台  20221201  只是查询记录条数可以不加
        /// 增加rsg 平台  20241116
        /// 加bti 202502
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportCountSqlV1 = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {1}
) dd
group by dd.account
 ) T
) 
TT ";

        /// <summary>
        /// 合并后版本
        /// 新加： pp iaes  20260711
        /// 增加fb 平台  20221201  只是查询记录条数可以不加
        /// 增加rsg 平台  20241116
        /// 加bti 202502
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportCountSqlV1_RealTime = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_bet+iaes_bet) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {1}
) dd
group by dd.account
 ) T
) 
TT ";


        public const string FinanceReportCountUnionSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {0}

union

select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from 
( 
			 select a.account, a.financeDay d from ofAffDailyReportFinance a with(nolock) union
			 select b.account,b.gameReportDay d from ofAffDailyReportGame b with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance a with(nolock) on allk.account= a.account and allk.d = a.financeDay 
			 left join ofAffDailyReportGame b with(nolock) on allk.account = b.account and allk.d= b.gameReportDay
where financeDay >= @startTime and financeDay<=@endTime {1}

) dd
group by dd.account
 ) T
) 
TT ";
        /// <summary>
        /// 新加： pp iaes  20260711
        /// 加rsg 2024-11-16
        /// 加bti 2025-02-05
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportCountUnionSqlV1 = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {2}

union all

select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {1} {2}

) dd
group by dd.account
 ) T
) 
TT ";

        /// <summary>
        /// 新加： pp iaes  20260711
        /// 加rsg 2024-11-16
        /// 加bti 2025-02-05
        /// 加dbkeno  20260109
        /// </summary>
        public const string FinanceReportCountUnionSqlV1_RealTime = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(rebate) rebate,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {0} {2}

union all

select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.rebate,a.turnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
from ofAffDailyReportFG_RealTime a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime {1} {2}

) dd
group by dd.account
 ) T
) 
TT ";




        /// <summary>
        /// 20200711新加tf  aw  cq 
        /// 20210101 kg3   fb,jdb
        /// 20240602 db
        /// 20241118 rsg
        /// 20250205 bti  dbkeno
        /// </summary>
        public const string PlayersFinanceReportSql_New = @"SELECT TT.*,u.id,u.affaccount,CONVERT(VARCHAR(20), u.createtime, 120) createtime,u.lastLogin,ug.levels,ug.levelnumber,oat.parentAccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) pm_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost, sum(obcv_winlost) obcv_winlost, sum(fb_winlost) fb_winlost, sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,
sum(obg_turnover) pm_turnover,sum(imsp_turnover) imsp_turnover,sum(sw_turnover) sw_turnover,sum(cr_turnover) cr_turnover, sum(obcv_turnover) obcv_turnover, sum(fb_turnover) fb_turnover, sum(jdb_turnover) jdb_turnover,sum(db_turnover) db_turnover,sum(rsg_turnover) rsg_turnover,sum(bti_turnover) bti_turnover,sum(dbkeno_turnover) dbkeno_turnover,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) pm_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet, sum(obcv_bet) obcv_bet, sum(fb_bet) fb_bet, sum(jdb_bet) jdb_bet, sum(db_bet) db_bet, sum(rsg_bet) rsg_bet, sum(bti_bet) bti_bet, sum(dbkeno_bet) dbkeno_bet
from (
	select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,a.turnover,
	(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost) as winlost,
	ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,kg3_turnover,obg_turnover,imsp_turnover,sw_turnover,cr_turnover,obcv_turnover,fb_turnover,jdb_turnover,db_turnover,rsg_turnover,bti_turnover,dbkeno_turnover,
	(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet) as bets,
	ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,
	ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost
	from ofPlayerDailyReportFG a with(nolock)
	where financeGameDay >=@startTime and financeGameDay<=@endTime and a.affaccount=@affaccount {0}
	) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex  
order by TT.winlost desc 
OPTION (RECOMPILE) ";


        /// <summary>
        /// 20211122 对 PlayersFinanceReportSql_New 统计
        /// "会员报表" 总计   fb ,jdb
        /// 20240602 db
        /// 20241118 rsg
        /// 20250205 bti  dbkeno
        /// </summary>
        public const string PlayersFinanceReportTotalSql_New = @"select  SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,
SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,
SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) pm_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost) as winlost,
	SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet) as bets
	from ofPlayerDailyReportFG a with(nolock)
	where financeGameDay >=@startTime and financeGameDay<=@endTime and a.affaccount=@affaccount {0}
OPTION (RECOMPILE) ";



        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20210101 kg3
        /// 20211122 obg, imsp , sw , cr   obcv
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20240904 [evo],[b9w],[turbo],[spribe],[jl]
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// 20260109 ching 加 dbkeno
        /// </summary>
        public const string PlayersFinanceReportSql_NewV1 = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.createtime,u.id uid,u.anomaly,u.walletType,u.merchantAccount,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
		(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
		(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc 
OPTION (RECOMPILE) ";


        /// <summary>
        /// 返回统计数据（底部数据）
        /// 新加： pp iaes  20260711
        /// 20210101 kg3
        /// 20211122 obg, imsp , sw , cr   obcv
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20240904 [evo],[b9w],[turbo],[spribe],[jl]
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// 20260109 ching 加 dbkeno
        /// </summary>
        public const string PlayersFinanceReportSql_NewV1_report = @"SELECT sum(TT.depositAmount) depositAmount, SUM(TT.depositTimes) depositTimes , SUM(TT.withdrawAmount) withdrawAmount , SUM(TT.withdrawTimes) withdrawTimes , SUM(TT.totalWithdrawFee) totalWithdrawFee , 
SUM(TT.fee) fee , SUM(TT.totalDepositFee) totalDepositFee ,SUM(TT.totalRealBankFee) totalRealBankFee, sum(TT.promo) promo , SUM(TT.bonus) bonus , sum(TT.turnover) turnover , SUM(TT.bets) bets ,SUM(TT.winlost) winlost FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.createtime,u.id uid,u.anomaly,u.walletType,u.merchantAccount,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_winlost+iaes_winlost) turnover,
		(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
		(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex
OPTION (RECOMPILE) ";


        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl
        /// </summary>
        public const string PlayersFinanceReportSql_NewV1_INR = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.id uid,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(pg_turnover+jdb_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover) turnover,
		(pg_winlost+jdb_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost) as winlost,
		(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc 
OPTION (RECOMPILE) ";

        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl
        /// </summary>
        public const string PlayersFinanceReportSql_NewV1_Boss_cny = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.id uid,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(fb_turnover+imsp_turnover+ag_turnover+imes_turnover+tf_turnover+lb_turnover+xj_turnover+db_turnover+ky_turnover+pg_turnover+dbkeno_turnover+jdb_turnover) turnover,
		(fb_winlost+imsp_winlost+ag_winlost+imes_winlost+tf_winlost+lb_winlost+xj_winlost+db_winlost+ky_winlost+pg_winlost+dbkeno_winlost+jdb_winlost) as winlost,
		(fb_bet+imsp_bet+ag_bet+imes_bet+tf_bet+lb_bet+xj_bet+db_bet+ky_bet+pg_bet+dbkeno_bet+jdb_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc 
OPTION (RECOMPILE)";

        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl 时实游戏数据统计
        /// </summary>
        public const string PlayersFinanceReportSql_NewV1_Boss_cny_RealTime = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.id uid,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(fb_turnover+imsp_turnover+ag_turnover+imes_turnover+tf_turnover+lb_turnover+xj_turnover+db_turnover+ky_turnover+pg_turnover+dbkeno_turnover+jdb_turnover) turnover,
		(fb_winlost+imsp_winlost+ag_winlost+imes_winlost+tf_winlost+lb_winlost+xj_winlost+db_winlost+ky_winlost+pg_winlost+dbkeno_winlost+jdb_winlost) as winlost,
		(fb_bet+imsp_bet+ag_bet+imes_bet+tf_bet+lb_bet+xj_bet+db_bet+ky_bet+pg_bet+dbkeno_bet+jdb_bet) as bets
		from ofPlayerDailyReportFG_RealTime a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex
order by TT.winlost desc 
OPTION (RECOMPILE)";

        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl 时实游戏数据统计(合计)
        /// </summary>
        public const string PlayersFinanceTotalReportSql_NewV1_Boss_cny_RealTime = @"select SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(fb_turnover+imsp_turnover+ag_turnover+imes_turnover+tf_turnover+lb_turnover+xj_turnover+db_turnover+ky_turnover+pg_turnover+dbkeno_turnover+jdb_turnover) turnover,
		(fb_winlost+imsp_winlost+ag_winlost+imes_winlost+tf_winlost+lb_winlost+xj_winlost+db_winlost+ky_winlost+pg_winlost+dbkeno_winlost+jdb_winlost) as winlost,
		(fb_bet+imsp_bet+ag_bet+imes_bet+tf_bet+lb_bet+xj_bet+db_bet+ky_bet+pg_bet+dbkeno_bet+jdb_bet) as bets
		from ofPlayerDailyReportFG_RealTime a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	OPTION (RECOMPILE)";

        public const string PlayersFinanceTotalReportSql_NewV1_INR = @"select SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(pg_turnover+jdb_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover) turnover,
		(pg_winlost+jdb_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost) as winlost,
		(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	OPTION (RECOMPILE) ";




        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl
        /// </summary>
        public const string PlayersFinanceReportCountSql_newV1_INR = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(pg_turnover+jdb_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover) turnover,
		(pg_winlost+jdb_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost) as winlost,
		(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
OPTION (RECOMPILE) ";


        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl
        /// </summary>
        public const string PlayersFinanceReportCountSql_newV1_Boss_cny = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(fb_turnover+imsp_turnover+ag_turnover+imes_turnover+tf_turnover+lb_turnover+xj_turnover+db_turnover+ky_turnover+pg_turnover+dbkeno_turnover+jdb_turnover) turnover,
		(fb_winlost+imsp_winlost+ag_winlost+imes_winlost+tf_winlost+lb_winlost+xj_winlost+db_winlost+ky_winlost+pg_winlost+dbkeno_winlost+jdb_winlost) as winlost,
		(fb_bet+imsp_bet+ag_bet+imes_bet+tf_bet+lb_bet+xj_bet+db_bet+ky_bet+pg_bet+dbkeno_bet+jdb_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
OPTION (RECOMPILE) ";

        /// <summary>
        /// inr:   pg,jdb, evo,9w, turbo, spribe,jl  实时游戏数据统计
        /// </summary>
        public const string PlayersFinanceReportCountSql_newV1_Boss_cny_RealTime = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(fb_turnover+imsp_turnover+ag_turnover+imes_turnover+tf_turnover+lb_turnover+xj_turnover+db_turnover+ky_turnover+pg_turnover+dbkeno_turnover+jdb_turnover) turnover,
		(fb_winlost+imsp_winlost+ag_winlost+imes_winlost+tf_winlost+lb_winlost+xj_winlost+db_winlost+ky_winlost+pg_winlost+dbkeno_winlost+jdb_winlost) as winlost,
		(fb_bet+imsp_bet+ag_bet+imes_bet+tf_bet+lb_bet+xj_bet+db_bet+ky_bet+pg_bet+dbkeno_bet+jdb_bet) as bets
		from ofPlayerDailyReportFG_RealTime a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
OPTION (RECOMPILE) ";






        /// <summary>
        /// 会员财务报表--导出
        /// </summary>
        public const string PlayersFinanceReportExportSql_old = @"SELECT TT.*,u.affaccount,oat.parentAccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus+promo) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet) as bets
from ofPlayerDailyReportFinance a with(nolock)
left join ofPlayerDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
where financeDay >=@startTime and financeDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex  {1}
order by TT.winlost desc 
OPTION (RECOMPILE)";

        /// <summary>
        /// 20221201 fb
        /// </summary>
        public const string PlayersFinanceReportExportSql = @"SELECT TT.*,u.affaccount,oat.parentAccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus+promo) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+aw_winlost+cq_winlost+tf_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+aw_bet+cq_bet+tf_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet) as bets
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >=@startTime and financeGameDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex  {1}
order by TT.winlost desc 
OPTION (RECOMPILE) ";

        /// <summary>
        /// 会员财务报表统计总计录
        /// </summary>
        public const string PlayersFinanceReportCountSql_old = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from ofPlayerDailyReportFinance a with(nolock)
left join ofPlayerDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
where financeDay >=@startTime and financeDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT  
OPTION (RECOMPILE)";

        /// <summary>
        /// 20241118 rsg
        ///  20250205 bti dbkeno
        /// </summary>
        public const string PlayersFinanceReportCountSql_new = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,a.turnover,
	(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost) as winlost,
	ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,kg3_turnover,obg_turnover,imsp_turnover,sw_turnover,cr_turnover,obcv_turnover,fb_turnover,jdb_turnover,db_turnover,rsg_turnover,bti_turnover,dbkeno_turnover,
	(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet) as bets,
	ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,
	ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost
	from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >=@startTime and financeGameDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT 
OPTION (RECOMPILE) ";



        #region
        /// <summary>
        /// 新加： pp iaes  20260711
        /// 20240904 [evo],[b9w],[turbo],[spribe],[jl]
        /// 20241116 rsg
        ///  20250205 ching 加 bti
        ///  20260109 ching 加 dbkeno
        /// </summary>
        public const string PlayersFinanceReportCountSql_newV1 = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, T.*,u.affaccount,u.anomaly,ue.riskRemark,ue.riskTypesAready,oat.parentAccount,ug.levels,u.level from (
	select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(depositFee) totalDepositFee,SUM(realBankFee) totalRealBankFee,SUM(promo) promo,SUM(bonus+promo+rebate) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost 
	from (
		select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.depositFee,a.withdrawFee,a.realBankFee,a.bonus,a.promo,a.rebate,
		(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) turnover,
		(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as winlost,
		(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as bets
		from ofPlayerDailyReportFG a with(nolock)
		where financeGameDay >=@startTime and financeGameDay<=@endTime  {0}
	) dd
	group by dd.account
 ) T
 left join ofUser u with(nolock) on u.account= T.account
left join ofUserExtention ue with(nolock) on ue.account= T.account
left join ofUserGroup ug with(nolock) on ug.levelnumber = u.level
left join ofAffiliateTree oat with(nolock) on u.affaccount=oat.agentAccount
WHERE 1=1 {1}
) 
TT
OPTION (RECOMPILE) ";

        #endregion



        /// <summary>
        /// 会员财务报表统计总计录--导出
        /// </summary>
        public const string PlayersFinanceReportExportCountSql_old = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from ofPlayerDailyReportFinance a with(nolock)
left join ofPlayerDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
where financeDay >=@startTime and financeDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT  
left join ofUser u on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
{1}
OPTION (RECOMPILE)";

        public const string PlayersFinanceReportExportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes,SUM(promo) promo,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeGameDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >=@startTime and financeGameDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT  
left join ofUser u on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
{1}
OPTION (RECOMPILE)";

        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// 代理 游戏报表
        /// </summary>
        public const string GameReportSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) totalBet
from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex 
order by TT.totalWinlost desc 
OPTION (RECOMPILE) ";

        /// <summary>
        /// 20260711 ching 加  pp iaes
        /// 20210101 kg3
        /// 20211122 obg, imsp, sw, cr
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string GameReportSqlV1 = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {0} {1}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex and totalBet>0
order by TT.totalWinlost desc 
OPTION (RECOMPILE) ";


        /// <summary>
        /// 20260711 ching 加  pp iaes
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string GameReportCountSqlV1 = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {0} {1}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
 WHERE totalBet>0
OPTION (RECOMPILE) ";

//        public const string GameReportCountSqlV1_old = @" SELECT count(account) cnt FROM 
//( 
//SELECT ROW_NUMBER() OVER 
//(order by T.totalWinlost desc)AS Row, * from (
//select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
//SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,
//SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) totalWinlost,
//SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) totalBet
//from (
//select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
//ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet
//from ofAffDailyReportFG a with(nolock)
//where financeGameDay >= @startTime and financeGameDay<=@endTime  {0}
//) dd
//group by dd.account
// ) T
//) 
//TT
//left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
// WHERE totalBet>0
//OPTION (RECOMPILE)";

        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// </summary>
        public const string GameReportUnionSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) totalBet
from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  {0}

union

select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  {1}

) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex 
order by TT.totalWinlost desc ";

        /// <summary>
        /// 20260711 ching 加  pp iaes
        /// 20210101 kg3
        /// 20211122 obg, imsp, sw, cr
        /// 20221201 fb  jdb
        /// 20240415 db
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string GameReportUnionSqlV1 = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {0} {2}

union all

select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {1} {2}

) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex and TT.totalBet > 0
order by TT.totalWinlost desc
OPTION (RECOMPILE) ";


        /// <summary>
        /// 游戏报表 统计
        /// </summary>
        public const string GameReportCountSql = @" select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) winlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime {0}
) dd
group by dd.account
 ) T
 ) TT 
OPTION (RECOMPILE)";

        

        public const string GameReportCountUnionSql = @" select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) winlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime {0}

union
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime {1}

) dd
group by dd.account
 ) T
 ) TT ";
        /// <summary>
        /// 20260711 ching 加  pp iaes
        /// 20241116 rsg
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string GameReportCountUnionSqlV1 = @" SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bit_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {0} {2}

union all

select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate oa with(nolock) on oa.account = a.account
where financeGameDay >= @startTime and financeGameDay<=@endTime  {1} {2}

) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
where TT.totalBet > 0
OPTION (RECOMPILE)";


        /// <summary>
        /// 代理 游戏报表
        /// </summary>
        public const string PlayersGameReportSql_old = @"SELECT TT.*,u.affaccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+rg_bet+dscp_bet+dses_bet) totalBet
from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
 WHERE TT.Row between @startIndex and @endIndex
order by TT.totalWinlost desc  
OPTION (RECOMPILE) ";
        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// </summary>
        public const string PlayersGameReportSql_new = @"SELECT TT.*,u.affaccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) totalBet
from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
 WHERE TT.Row between @startIndex and @endIndex
order by TT.totalWinlost desc ";



        /// <summary>
        /// 20260711 ching 加 pp + iaes
        /// 20210101  kg3
        /// 20211122 obg imsp sw cr  obcv
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20241116  rsg
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string PlayersGameReportSql_newV1 = @"SELECT TT.*,u.affaccount,u.anomaly,ue.riskRemark,ue.riskTypesAready FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
left join ofUserExtention ue with(nolock) on ue.account= TT.account
WHERE TT.Row between @startIndex and @endIndex and TT.totalBet>0
order by TT.totalWinlost desc 
OPTION (RECOMPILE) ";

        

        public const string PlayersGameExportReportSql = @"SELECT TT.*,u.affaccount,oat.parentAccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(cq_winlost) cq_winlost,SUM(aw_winlost) aw_winlost,SUM(tf_winlost) tf_winlost,SUM(kg3_winlost) kg3_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(cq_bet) cq_bet,SUM(aw_bet) aw_bet,SUM(tf_bet) tf_bet,SUM(kg3_bet) kg3_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(cq_turnover) cq_turnover,SUM(aw_turnover) aw_turnover,SUM(tf_turnover) tf_turnover,SUM(kg3_turnover) kg3_turnover,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+cq_winlost+aw_winlost+tf_winlost+kg3_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+cq_bet+aw_bet+tf_bet+kg3_bet) totalBet,
SUM(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+cq_turnover+aw_turnover+tf_turnover+kg3_turnover) totalTurnover
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,cq_winlost,aw_winlost,tf_winlost,kg3_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,cq_bet,aw_bet,tf_bet,kg3_bet,
ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,cq_turnover,aw_turnover,tf_turnover,kg3_turnover
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex {1}
order by TT.totalWinlost desc  
OPTION (RECOMPILE) ";
        /// <summary>
        /// 代理导出各游戏平台的数据
        /// </summary>
        public const string AgentGameExportReportSql_old = @"SELECT TT.*,TT.account,oat.parentAccount affaccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account, SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,
sum(totalTurnover) totalTurnover,sum(totalWinlost) totalWinlost,sum(totalBet) totalBet
from (
select a.account,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,
ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,kg3_turnover,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover) totalTurnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost) as totalWinlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet) as totalBet
from ofAffDailyReportFG a with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on TT.account=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex and TT.totalBet>0 {1}
order by TT.totalWinlost desc  
OPTION (RECOMPILE) ";

        /// <summary>
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20241116 rsg
        /// 20250205 bti 
        /// 20260205 dbkeno
        /// 20260805 iaes+ pp
        /// </summary>
        public const string AgentGameExportReportSqlV = @"SELECT TT.*,TT.account,oat.parentAccount affaccount FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account, SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(iaes_winlost) iaes_winlost,sum(pp_winlost) pp_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(iaes_bet) iaes_bet,sum(pp_bet) pp_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,
sum(obg_turnover) obg_turnover,sum(imsp_turnover) imsp_turnover,sum(sw_turnover) sw_turnover,sum(cr_turnover) cr_turnover,sum(obcv_turnover) obcv_turnover,sum(fb_turnover) fb_turnover,sum(jdb_turnover) jdb_turnover,sum(db_turnover) db_turnover,sum(rsg_turnover) rsg_turnover,sum(bti_turnover) bti_turnover,sum(dbkeno_turnover) dbkeno_turnover,sum(iaes_turnover) iaes_turnover,sum(pp_turnover) pp_turnover,
sum(totalTurnover) totalTurnover,sum(totalWinlost) totalWinlost,sum(totalBet) totalBet
from (
select a.account,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,iaes_winlost,pp_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,iaes_bet,pp_bet,
ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,kg3_turnover,obg_turnover,imsp_turnover,sw_turnover,cr_turnover,obcv_turnover,fb_turnover,jdb_turnover,db_turnover,rsg_turnover,bti_turnover,dbkeno_turnover,iaes_turnover,pp_turnover,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+iaes_turnover+pp_turnover) totalTurnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+iaes_winlost+pp_winlost) as totalWinlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+iaes_bet+pp_bet) as totalBet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate u with(nolock) on a.account=u.account
where financeGameDay >= @startTime and financeGameDay<=@endTime and u.currencyType=@currencyType {0} 
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on TT.account=oat.agentAccount
WHERE TT.Row between @startIndex and @endIndex {1} 
order by TT.totalWinlost desc  
OPTION (RECOMPILE) ";



        public const string PlayersGameExportReportCountSql = @" select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) winlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) bet from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime {0}
) dd
group by dd.account
 ) T
 ) TT 
left join ofUser u on u.account= TT.account
left join ofAffiliateTree oat on u.affaccount=oat.agentAccount
{1}
OPTION (RECOMPILE) ";
        /// <summary>
        /// 代理数据导出数据总条数
        /// </summary>
        public const string AgentGameExportReportCountSql_old = @"select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account, SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,
sum(totalTurnover) totalTurnover,sum(totalWinlost) totalWinlost
from (
select a.account,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,
ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover) totalTurnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost) as totalWinlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) as totalBet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate u with(nolock) on a.account=u.account
where financeGameDay >= @startTime and financeGameDay<=@endTime and u.currencyType=@currencyType {0}
) dd
group by dd.account
 ) T
 ) TT 
left join ofAffiliateTree oat on TT.account=oat.agentAccount
{1}
OPTION (RECOMPILE)";

        /// <summary>
        /// 之前版本 AgentGameExportReportCountSql_old
        /// 20241116 rsg  
        /// 20250205 bti  
        /// 20260205 dbkeno  iaes+ pp
        /// 20260805 iaes+ pp
        /// </summary>
        public const string AgentGameExportReportCountSql = @"select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select dd.account, SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(iaes_winlost) iaes_winlost,sum(pp_winlost) pp_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(iaes_bet) iaes_bet,sum(pp_bet) pp_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,
sum(obg_turnover) obg_turnover,sum(imsp_turnover) imsp_turnover,sum(sw_turnover) sw_turnover,sum(cr_turnover) cr_turnover,sum(obcv_turnover) obcv_turnover,sum(fb_turnover) fb_turnover,sum(jdb_turnover) jdb_turnover,sum(db_turnover) db_turnover,sum(rsg_turnover) rsg_turnover,sum(bti_turnover) bti_turnover,sum(dbkeno_turnover) dbkeno_turnover,sum(iaes_turnover) iaes_turnover,sum(pp_turnover) pp_turnover,
sum(totalTurnover) totalTurnover,sum(totalWinlost) totalWinlost,sum(totalBet) totalBet
from (
select a.account,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,iaes_winlost,pp_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,iaes_bet,pp_bet,
ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,ttg_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover,dscp_turnover,dses_turnover,bg_turnover,eb_turnover,td_turnover,tf_turnover,aw_turnover,cq_turnover,kg3_turnover,obg_turnover,imsp_turnover,sw_turnover,cr_turnover,obcv_turnover,fb_turnover,jdb_turnover,db_turnover,rsg_turnover,bti_turnover,dbkeno_turnover,iaes_turnover,pp_turnover,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+iaes_turnover+pp_turnover) totalTurnover,
(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+iaes_winlost+pp_winlost) as totalWinlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+iaes_bet+pp_bet) as totalBet
from ofAffDailyReportFG a with(nolock)
left join ofAffiliate u with(nolock) on a.account=u.account
where financeGameDay >= @startTime and financeGameDay<=@endTime and u.currencyType=@currencyType {0}
) dd
group by dd.account
 ) T
 ) TT 
left join ofAffiliateTree oat on TT.account=oat.agentAccount
{1}
OPTION (RECOMPILE)";

        /// <summary>
        /// 会员 游戏报表 统计
        /// </summary>
        public const string PlayersGameReportCountSql_old = @" select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) winlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
 ) TT OPTION (RECOMPILE) ";

        public const string PlayersGameReportCountSql_new = @" select count(*) cnt from (SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) winlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
 ) TT ";


        /// <summary>
        /// 20260711 ching 加 pp + iaes
        /// 20241116 加 rsg 
        /// 20250205 ching 加 bti
        /// ching 加 dbkeno
        /// </summary>
        public const string PlayersGameReportCountSql_newV1 = @" SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) totalWinlost,
SUM(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) totalBet
from (
select  a.account,a.financeGameDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,ttg_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,dscp_winlost,dses_winlost,bg_winlost,eb_winlost,td_winlost,tf_winlost,aw_winlost,cq_winlost,kg3_winlost,obg_winlost,imsp_winlost,sw_winlost,cr_winlost,obcv_winlost,fb_winlost,jdb_winlost,db_winlost,rsg_winlost,bti_winlost,dbkeno_winlost,pp_winlost,iaes_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,ttg_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet,dscp_bet,dses_bet,bg_bet,eb_bet,td_bet,tf_bet,aw_bet,cq_bet,kg3_bet,obg_bet,imsp_bet,sw_bet,cr_bet,obcv_bet,fb_bet,jdb_bet,db_bet,rsg_bet,bti_bet,dbkeno_bet,pp_bet,iaes_bet
from ofPlayerDailyReportFG a with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime  and a.affaccount=@affaccount {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofUser u with(nolock) on u.account= TT.account
WHERE TT.totalBet>0
OPTION (RECOMPILE) ";

       

        /// <summary>
        /// 从日报中统计客人
        /// 时间段内各游戏平台的的总投和总输赢
        /// </summary>
        public const string PlayerBetWinlostByGameSql_old = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet
from ofPlayerDailyReportGame with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime and account=@account 
OPTION (RECOMPILE)";
        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// </summary>
        public const string PlayerBetWinlostByGameSql_new = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet
from ofPlayerDailyReportGame with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime and account=@account ";

        /// <summary>
        /// 20260711 加 pp + iaes
        /// 20200711 ching 新加 tf aw cq
        /// 20210101 kg3 obg, imsp , sw,  cr
        /// 20121201 fb jdb
        /// 20240415 db
        /// 20241116 加 rsg
        /// 20250205 加 bti
        /// </summary>
        public const string PlayerBetWinlostByGameSql_newV1 = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,
SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
SUM(obg_winlost) obg_winlost,SUM(imsp_winlost) imsp_winlost,SUM(sw_winlost) sw_winlost,SUM(cr_winlost) cr_winlost,SUM(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,
SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,
SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,SUM(obg_bet) obg_bet,SUM(imsp_bet) imsp_bet,SUM(sw_bet) sw_bet,SUM(cr_bet) cr_bet,SUM(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet
from ofPlayerDailyReportFG with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime and account=@account";

        /// <summary>
        /// 代理的各平台游戏数据
        /// 20260711 加 pp + iaes
        /// 20210101 kg3
        /// 20211122 obg, imsp , sw,  cr
        /// 20221201 fb jdb
        /// 20240415 db
        /// 20241116 加rsg
        /// 20250205 加 bti
        /// 加 dbkeno
        /// </summary>
        public const string AgentBetWinlostByGameSql_newV1 = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,
sum(obg_winlost) obg_winlost,sum(imsp_winlost) imsp_winlost,sum(sw_winlost) sw_winlost,sum(cr_winlost) cr_winlost,sum(obcv_winlost) obcv_winlost,sum(fb_winlost) fb_winlost,sum(jdb_winlost) jdb_winlost,sum(db_winlost) db_winlost,sum(rsg_winlost) rsg_winlost,sum(bti_winlost) bti_winlost,sum(dbkeno_winlost) dbkeno_winlost,sum(pp_winlost) pp_winlost,sum(iaes_winlost) iaes_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,
sum(obg_bet) obg_bet,sum(imsp_bet) imsp_bet,sum(sw_bet) sw_bet,sum(cr_bet) cr_bet,sum(obcv_bet) obcv_bet,sum(fb_bet) fb_bet,sum(jdb_bet) jdb_bet,sum(db_bet) db_bet,sum(rsg_bet) rsg_bet,sum(bti_bet) bti_bet,sum(dbkeno_bet) dbkeno_bet,sum(pp_bet) pp_bet,sum(iaes_bet) iaes_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,
sum(obg_turnover) obg_turnover,sum(imsp_turnover) imsp_turnover,sum(sw_turnover) sw_turnover,sum(cr_turnover) cr_turnover,sum(obcv_turnover) obcv_turnover,sum(fb_turnover) fb_turnover,sum(jdb_turnover) jdb_turnover,sum(db_turnover) db_turnover,sum(rsg_turnover) rsg_turnover,sum(bti_turnover) bti_turnover,sum(dbkeno_turnover) dbkeno_turnover,sum(pp_turnover) pp_turnover,sum(iaes_turnover) iaes_turnover,
sum(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover+fb_turnover+jdb_turnover+db_turnover+rsg_turnover+bti_turnover+dbkeno_turnover+pp_turnover+iaes_turnover) totalTurnover,
sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost+dbkeno_winlost+pp_winlost+iaes_winlost) as totalWinlost,
sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet+dbkeno_bet+pp_bet+iaes_bet) as totalBet
from ofAffDailyReportFG with(nolock)
where financeGameDay >= @startTime and financeGameDay<=@endTime and account=@account ";

        /// <summary>
        /// 代理的各平台游戏数据
        /// 20210101 新增kg3 obg, imsp , sw,  cr
        /// </summary>
        public const string AgentBetWinlostByGameMonthSql_newV1 = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(ttg_winlost) ttg_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,SUM(dscp_winlost) dscp_winlost,SUM(dses_winlost) dses_winlost,SUM(bg_winlost) bg_winlost,SUM(eb_winlost) eb_winlost,SUM(td_winlost) td_winlost,SUM(tf_winlost) tf_winlost,SUM(aw_winlost) aw_winlost,SUM(cq_winlost) cq_winlost,SUM(kg3_winlost) kg3_winlost,SUM(obg_winlost) obg_winlost,SUM(imsp_winlost) imsp_winlost,SUM(sw_winlost) sw_winlost,SUM(cr_winlost) cr_winlost,SUM(obcv_winlost) obcv_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(ttg_bet) ttg_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet,SUM(dscp_bet) dscp_bet,SUM(dses_bet) dses_bet,SUM(bg_bet) bg_bet,SUM(eb_bet) eb_bet,SUM(td_bet) td_bet,SUM(tf_bet) tf_bet,SUM(aw_bet) aw_bet,SUM(cq_bet) cq_bet,SUM(kg3_bet) kg3_bet,SUM(obg_bet) obg_bet,SUM(imsp_bet) imsp_bet,SUM(sw_bet) sw_bet,SUM(cr_bet) cr_bet,SUM(obcv_bet) obcv_bet,
SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(ttg_turnover) ttg_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover,SUM(dscp_turnover) dscp_turnover,SUM(dses_turnover) dses_turnover,SUM(bg_turnover) bg_turnover,SUM(eb_turnover) eb_turnover,SUM(td_turnover) td_turnover,SUM(tf_turnover) tf_turnover,SUM(aw_turnover) aw_turnover,SUM(cq_turnover) cq_turnover,SUM(kg3_turnover) kg3_turnover,SUM(obg_turnover) obg_turnover,SUM(imsp_turnover) imsp_turnover,SUM(sw_turnover) sw_turnover,SUM(cr_turnover) cr_turnover,SUM(obcv_turnover) obcv_turnover,
sum(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+ttg_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover+dscp_turnover+dses_turnover+bg_turnover+eb_turnover+td_turnover+tf_turnover+aw_turnover+cq_turnover+kg3_turnover+obg_turnover+imsp_turnover+sw_turnover+cr_turnover+obcv_turnover) totalTurnover,
sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost) as totalWinlost,
sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet) as totalBet
from ofAffDailyReportFG with(nolock)
where {0} and account=@account ";





        /// <summary>
        /// 代理 推广数据报表
        /// </summary>
        public const string AgentSpreadReportSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.registerNumber desc)AS Row, * from (
select dd.account,SUM(registerNumber) registerNumber,SUM(firstDepositNumber) firstDepositNumber,SUM(depositTotal) depositTotal,SUM(withdrawTotal) withdrawTotal,SUM(visterTimes) visterTimes,SUM(depositUserTotal) depositUserTotal
from (
select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {0}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
WHERE TT.Row between @startIndex and @endIndex 
order by registerNumber desc ";

        public const string AgentSpreadReportUnionSql = @"SELECT TT.*,oat.isLeaf,oat.agentId as tid FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.registerNumber desc)AS Row, * from (
select dd.account,SUM(registerNumber) registerNumber,SUM(firstDepositNumber) firstDepositNumber,SUM(depositTotal) depositTotal,SUM(withdrawTotal) withdrawTotal,SUM(visterTimes) visterTimes,SUM(depositUserTotal) depositUserTotal
from (
select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {0}
union
select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {1}
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat with(nolock) on oat.agentAccount= TT.account
WHERE TT.Row between @startIndex and @endIndex 
order by registerNumber desc ";


        /// <summary>
        /// 代理 推广数据报表 记录数
        /// </summary>
        public const string AgentSpreadReportCountSql = @" SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositTotal desc)AS Row, * from (
select dd.account,SUM(registerNumber) registerNumber,SUM(firstDepositNumber) firstDepositNumber,SUM(depositTotal) depositTotal,SUM(withdrawTotal) withdrawTotal,SUM(visterTimes) visterTimes,SUM(depositUserTotal) depositUserTotal
from (
select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {0}

) dd
group by dd.account
 ) T
) 
TT ";

        public const string AgentSpreadReportCountUnionSql = @" SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositTotal desc)AS Row, * from (
select dd.account,SUM(registerNumber) registerNumber,SUM(firstDepositNumber) firstDepositNumber,SUM(depositTotal) depositTotal,SUM(withdrawTotal) withdrawTotal,SUM(visterTimes) visterTimes,SUM(depositUserTotal) depositUserTotal
from (
select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {0}

union

select  a.account,a.reportDay,registerNumber,firstDepositNumber,depositTotal,withdrawTotal,visterTimes,depositUserTotal
from ofAffDataReport a with(nolock)
where reportDay >= @startTime and reportDay<=@endTime {1}

) dd
group by dd.account
 ) T
) 
TT ";


        /// <summary>
        /// 代理的首存人员信息,记录条数
        /// </summary>

        public const string AgentFirstDepositPlayerCountSql = @"select count(*) cnt  from (
select id, account,referenceguid from ofPaymentRecord pr
where pr.createtime>=@startTime and pr.createtime< @endTime and preBalance=0 and summaryType=1) oa
left join ofDeposit od on oa.account =od.account and oa.referenceguid = od.referenceguid
left join ofUser u on oa.account = u.account
where u.affaccount IS not NULL and LEN(RTRIM(LTRIM(u.affaccount))) != 0 {0}";



        /// <summary>
        /// 代理的首存人员信息
        /// </summary>
        public const string AgentFirstDepositPlayerSql = @"select oa.account,u.affaccount,u.createtime,od.amount  from (
select id, account,referenceguid from ofPaymentRecord pr
where pr.createtime>=@startTime and pr.createtime< @endTime and preBalance=0 and summaryType=1) oa
left join ofDeposit od on oa.account =od.account and oa.referenceguid = od.referenceguid
left join ofUser u on oa.account = u.account
where u.affaccount IS not NULL and LEN(RTRIM(LTRIM(u.affaccount))) != 0 {0}";








    }
}
