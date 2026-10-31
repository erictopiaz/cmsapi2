using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.webSvcSqlCommandText
{
    /// <summary>
    /// 网站服务代理相关sql
    /// </summary>
    public class wsAgentSqlCommandText
    {
        /// <summary>
        /// 获取当前代理账号，如果存代理账号不存，则会返回默认的代理账号
        /// </summary>
        public const string queryAgentAccountByRegisterUrl = @"if exists( select top 1 account from ofAffiliate where status=1 and verify = 1 and adlink like CONCAT('%/',@domain,'%')) begin
select top 1 account from ofAffiliate where status=1 and verify = 1 and adlink like CONCAT('%/',@domain,'%') 
end
else if exists( select top 1 account from ofAffiliate where status=1 and verify = 1 and (adcode=@adcode and adcode IS not NULL and LEN(RTRIM(LTRIM(adcode))) != 0 ))
begin
select top 1 account from ofAffiliate where status=1 and verify = 1 and adcode=@adcode
end
else
select top 1 account from ofAffiliate where status=1 and verify = 1 and defaultAgent=1";

        /// <summary>
        /// 查询合营推广链接池
        /// </summary>
        public const string queryAgentSpreadLinkPoolSql = @"select body from ofsettings with(nolock)
where code='aff_spread_link_pool'";


        /// <summary>
        /// 获取代理信息根据账号
        /// </summary>
        public const string QueryAgentSql = @"select a.*,atree.billP as billPS from ofAffiliate a
left join ofAffiliateTree atree on a.account=atree.agentAccount
where account = @account";

        /// <summary>
        /// 更新合营推广链接地址
        /// </summary>
        public const string UpdateAffAdlinkSql = @"update ofAffiliate set adlink=@adlink where account=@account";

        /// <summary>
        /// 查询市场人员数据
        /// </summary>
        public const string QueryMarketSql = @"select account,password from ofManage WHERE account=@account";

        /// <summary>
        /// 查询代理通过指定会员账号
        /// </summary>
        public const string QueryAgentWithPlayerSql = @"select a.password,a.balance,u.amountactive,a.transferAllowed,a.depositAllowed,u.realname,u.referrer,u.referrerreg from ofAffiliate a with(nolock)
left join ofAffiliateTree atree with(nolock) on a.account=atree.agentAccount
left join ofUser u with(nolock) on a.account = u.affaccount
where a.account = @affaccount and u.account=@account ";

        /// <summary>
        /// 合营账号去查询会员（会员，登录代理时会检测是否存在代理，不存在会创建代理）
        /// </summary>
        public const string QueryUserWithAgentAccountSql = @"select u.realname,u.referrer,u.referrerreg, password from ofUser u with(nolock)
where  u.account=@account ";

        public const string QueryAgentAccountSql = @"select balance from ofAffiliate u with(nolock)
where  u.account=@account ";

        /// <summary>
        /// 检测当前账号是否在代理或会员账号列表内
        /// </summary>
        public const string CheckAccountSql = @"select account from ofAffiliate 
where account = @account 
union all
select account from ofUser 
where account = @account ";

        /// <summary>
        /// 代理创建会员
        /// </summary>
        public const string createPlayerSql = @"INSERT INTO [dbo].[ofUser]
           ([currency],[account],[password],[passkey],[nickname],[sex],[telephone],[country],[status],[referrerreg]
           ,[ipaddress],[macaddress],[affaccount],[bbaffaccount],[freezereason],[acceptmail],[marcode],[createBy],[realname],[birthday],
[email],[referrer],[accounttype],[level],)
     VALUES
           ('{0}','{1}','{2}','{3}','{4}',0
           ,'{5}','CN',1,'{6}','{7}',N'{8}','{9}','{10}','{11}',1,'{12}',1,'','','','',1,10001)";

        /// <summary>
        /// 获取代理卡相关信息(指定卡类型)
        /// </summary>
        public const string QueryAgentCardsByBankTypSqlV = @"select bc.*,bb.category from ofBankCards bc
left join ofBankBase bb on bc.bankCode = bb.bid
where bc.account='{0}' and bc.accountType=1 and bc.cardStatus=1 and bc.bankType={1} and bb.currencycode={2}";

        /// <summary>
        /// 查询卡号是否被使用了
        /// </summary>
        public const string CheckCardUsedSql = @"select account from ofBankCards bc with(nolock)
where cardno='{0}' and bankType=1 and accountType=1";

        /// <summary>
        /// 获取代理所有卡相关信息
        /// </summary>
        public const string QueryAgentCardsAllSql = @"select bc.*,bb.category from ofBankCards bc
left join ofBankBase bb on bc.bankCode = bb.bid
where bc.account='{0}' and bc.accountType=1 and bc.cardStatus=1";

        /// <summary>
        /// cms 获取所有人代理卡号列表
        /// </summary>
        public const string QueryAgentCardsListSql = @"select bc.*,af.realname,af.verifyperson from ofBankCards bc
left join ofAffiliate af on bc.account=af.account
where bc.account='{0}' and accountType=1 and cardStatus=1 ";

        /// <summary>
        /// 获取人民币兑换汇率
        /// </summary>
        public const string QueryRMBXRateSql = @"select body from ofSettings where id=10035 and status=1";


        /// <summary>
        /// 代理提款时获取收款卡信息
        /// </summary>
        public const string QueryAgentWithdrawCardsSql = @"select bc.*,bb.category from ofBankCards bc
left join ofBankBase bb on bc.bankCode = bb.bid
where bc.account='{0}' and bc.accountType=1 and bc.cardStatus=1 ";

        /// <summary>
        /// 获取合法管理员
        /// </summary>
        public const string loadAdminDataInfoSql = @"SELECT * FROM ofManage WHERE account=@account AND password=@password and status=1";

        /// <summary>
        /// 更新代理卡相关信息
        /// </summary>
        public const string UpdateAgentCardsSql = @"UPDATE [dbo].[ofBankCards]
   SET [bankName] = @bankName,[bankAddress] = @bankAddress,[owner] = @owner,[bankCode]=@bankCode,[bankType]=@bankType,[remarks]=@remarks,[ownerPY]=@ownerPY,[swift]=@swift
 WHERE account=@account and cardno=@cardno and cardStatus=1 ";

        /// <summary>
        /// 关闭代理卡相关信息
        /// </summary>
        public const string UpdateAgentCardsStatusSql = @"UPDATE [dbo].[ofBankCards]
   SET [cardStatus] = 0 WHERE account=@account and cardno=@cardno ";

        /// <summary>
        /// 代理绑定新卡
        /// </summary>
        public const string InsertAgentCardsSql = @"INSERT INTO [dbo].[ofBankCards]
           ([account],[cardno],[bankName],[bankAddress],[owner]
           ,[who_bind],[cardStatus],[bankCode],[accountType],[requestip],[icon],[bankType],[remarks],[ownerPY],[swift])
     VALUES
           (@account,@cardno,@bankName,@bankAddress
           ,@owner,@who_bind,@cardStatus,@bankCode,@accountType,@requestip,@icon,@bankType,@remarks,@ownerPY,@swift)";

        public const string QueryAgentSql_New = @"select a.*,atree.*,atree.billP as billPS,(a.balance+a.subTotalAmount+a.playerBonus+a.salaryBonus+a.festivalBonus) activeAmount, (a.balance+a.subTotalAmount+a.unSettleAmount+a.playerBonus+a.salaryBonus+a.festivalBonus) totalAmount ,iif((brm.amount IS not NULL and LEN(RTRIM(LTRIM(brm.amount))) != 0), brm.amount,0) curBilledAmount from ofAffiliate a
left join ofAffiliateTree atree on a.account=atree.agentAccount
left join (select amount+subTotalAmount+unSettleAmount amount,account from ofAffBillReportCurrentMonth where billedTime={1} ) brm on a.account=brm.account
where a.account = '{0}' ";

        /// <summary>
        /// 代理登录并返回上月结算数据
        /// </summary>
        public const string QueryAgentSql_New20240805 = @"select a.*,atree.*,atree.billP as billPS,(a.balance+a.subTotalAmount+a.playerBonus+a.salaryBonus+a.festivalBonus) activeAmount, 
(a.balance+a.subTotalAmount+a.unSettleAmount+a.playerBonus+a.salaryBonus+a.festivalBonus) totalAmount ,
iif((brm.amount IS not NULL and LEN(RTRIM(LTRIM(brm.amount))) != 0), brm.amount,0) curBilledAmount from ofAffiliate a
left join ofAffiliateTree atree on a.account=atree.agentAccount
left join (select amount,account from ofAffBillReport where billedTime={1} and account='{0}' ) brm on a.account=brm.account
where a.account = '{0}'";

        /// <summary>
        /// 获取代理信息根据推荐代码
        /// </summary>
        public const string QueryAgentByParentAdcodeSql = @"select * from ofAffiliate a where adcode = @adcode";
        /// <summary>
        /// 给定代理的层次结构   
        /// </summary>
        public const string QueryAgentLevelSql = @"select * from  ofAffiliateTree
where  agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount=@account),'%') and agentLevel > (select agentLevel from ofAffiliateTree where agentAccount=@account) 
union
select * from  ofAffiliateTree
where  agentAccount=@account";

  //      /// <summary>
  //      /// 代理注册
  //      /// </summary>
  //      public const string RegisterAgentSql = @"if not exists( Select account from ofAffiliate Where account=@account)
		//begin
		//	INSERT INTO[dbo].[ofAffiliate]
  //      ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[ipaddress],[macaddress],[createperson],[registerUrl],[status])
  //  VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@registerUrl,0);
		//end";

        /// <summary>
        /// 代理注册  
        /// 2025-05-31 钱包类型+品牌
        /// </summary>
        public const string RegisterAgentSqlV = @"if not exists( Select account from ofAffiliate Where account=@account)
		begin
			INSERT INTO[dbo].[ofAffiliate]
        ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[ipaddress],[macaddress],[createperson],[registerUrl],[status],[a_walletType],[a_merchantAccount])
    VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@registerUrl,0,@a_walletType,@a_merchantAccount);
		end";

        /// <summary>
        /// 注册会员
        /// 2025-05-13 walletType 区分 转账钱包或是单一钱包  merchantAccount
        /// </summary>
        public const string RegisterSql_V = @"INSERT INTO ofuser(currency,account,password,passkey,nickname,realname,birthday,country,status,level,telephone,email,referrer,referrerreg,ipaddress,macaddress,marcode,freezereason,regdevice,phoneCode,currencyType,lang,accounttype,walletType,merchantAccount,affaccount)
values(@currency,@account,@password,@passkey,@nickname,@realname,@birthday,@country,@status,@level,@telephone,@email,@referrer,@referrerreg,@ipaddress,@macaddress,@markcode,@freezereason,@regdevice,@phoneCode,@currencyType,@lang,@accounttype,@walletType,@merchantAccount,@affaccount)";

        /// <summary>
        /// 生成游戏钱包
        /// </summary>
        public const string InsertOrUpdateSql = @"if(not EXISTS (SELECT top 1 id FROM ofUserGameWallet WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserGameWallet] ([account],[wallet])
     VALUES(@account,@wallet)
end";


        /// <summary>
        /// 代理注册并记录设备id
        /// </summary>
        public const string RegisterAgentDeviceSql_bak = @"if not exists( Select account from ofAffiliate Where account=@account)
		begin
			INSERT INTO[dbo].[ofAffiliate]
        ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[ipaddress],[macaddress],[createperson],[registerUrl],[status],[a_device],[advantages])
    VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@registerUrl,0,@device,@content);
		end";

        /// <summary>
        /// 代理注册并记录设备id  v
        /// </summary>
        public const string RegisterAgentDeviceSql_V = @"if not exists( Select account from ofAffiliate Where account=@account)
		begin
			INSERT INTO[dbo].[ofAffiliate]
        ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[ipaddress],[macaddress],[createperson],[registerUrl],[status],[a_device],[advantages],[a_walletType],[a_merchantAccount])
    VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @adcode, @ipaddress, @macaddress, @createperson,@registerUrl,0,@device,@content,@a_walletType,@a_merchantAccount);
		end";


        /// <summary>
        /// 代理注册并记录设备id  并加了货币类型
        /// 2025-05-31 品牌+钱包类型
        /// </summary>
        public const string RegisterAgentCurrencySqlV = @"if not exists( Select account from ofAffiliate Where account=@account)
		begin
			INSERT INTO[dbo].[ofAffiliate]
        ([currency],[account],[password],[realname],[sex],[birthday],[email],[telephone],[qq],[website],[parentAdcode],[adcode],[adlink],[ipaddress],[macaddress],[createperson],[registerUrl],[status],[a_device],[advantages],[currencyType],[lang],[a_walletType],[a_merchantAccount])
    VALUES(@currency, @account, @password, @realname, @sex, @birthday, @email, @telephone, @qq, @website, @parentAdcode, @adcode,@adlink, @ipaddress, @macaddress, @createperson,@registerUrl,0,@device,@content,@currencyType,@lang,@a_walletType,@a_merchantAccount);
		end";

        /// <summary>
        /// 代理提交修改密码
        /// </summary>
        public const string UpdatePasswordByAgentSql = @"update [ofAffiliate] set password=@password where account=@account and password=@oldPwd";
        public const string UpdatePasswordByMarketSql = @"update [ofManage] set password=@password where account=@account";

        /// <summary>
        /// 查询代理密码
        /// </summary>
        public const string QueryAgentPasswordSql = @"select password from [dbo].[ofAffiliate] where account=@account and status=1 and verify=1 ";

        /// <summary>
        /// 代理提款申请
        /// 新加bankType 0:银行卡，1：虚拟卡
        /// </summary>
        public const string InsertAgentWithdrawSql = @"INSERT INTO [dbo].[ofAffWithdraw]
           ([account],[realname],[amount],[cardno],[cardaddress],[cardbank],[requestip],[requestaddress],[bankType])
     VALUES(@account,@realname,@amount,@cardno,@cardaddress,@cardbank,@requestip,@requestaddress,@bankType)";

        /// <summary>
        /// 提款时更新代理余额
        /// 并冻结提款的金额
        /// </summary>
        //public const string UpdateAgentBalaceSql = @"update ofAffiliate set balance = balance-@amount  where balance-@amount >= 0 and account=@account";
        //public const string UpdateAgentBalaceSql = @"update ofAffiliate set balance = balance+subTotalAmount+playerBonus+salaryBonus+festivalBonus-@amount, subTotalAmount = 0  where balance+subTotalAmount+playerBonus+salaryBonus+festivalBonus-@amount >= 0 and account=@account";
        public const string UpdateAgentBalaceSql = @"update ofAffiliate set balance = balance+subTotalAmount-@amount, subTotalAmount = 0  where balance+subTotalAmount-@amount >= 0 and account=@account";

        /// <summary>
        /// 更新代理可用余额
        /// </summary>
        public const string UpdateAgentAmountSql = @"update ofAffiliate set balance = balance-@amount where balance-@amount >= 0 and account=@account";

        /// <summary>
        /// 财务报表
        /// </summary>
        public const string FinanceReportSql = @"SELECT TT.*,oat.isLeaf FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(withdrawAmount) withdrawAmount,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.withdrawAmount,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+tt_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets,
from ofAffDailyReportFinance a with(nolock)
left join ofAffDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
where financeDay >= @startTime and financeDay<=@endTime and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex ";

        /// <summary>
        /// 财务报表总计数据
        /// </summary>
        public const string FinanceTotalReportSql = @"select SUM(depositAmount) depositAmount,SUM(withdrawAmount) withdrawAmount,SUM(bonus) bonus,SUM(turnovers) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.withdrawAmount,a.bonus,a.promo,a.turnover,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+tt_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+tt_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover) as turnovers
from ofAffDailyReportFinance a with(nolock)
left join ofAffDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
left join ofAffiliateTree oat on oat.agentAccount = a.account
where financeDay >= @startTime and financeDay<=@endTime and oat.agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount=@agentAccount), '%') 
) cc ";

        /// <summary>
        /// 财务报表统计总计录
        /// </summary>
        public const string FinanceReportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(withdrawAmount) withdrawAmount,SUM(bonus) bonus,SUM(turnover) turnover,SUM(bets) bets,SUM(winlost) winlost from (
select  a.account,a.financeDay,a.depositAmount,a.withdrawAmount,a.bonus,a.promo,(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+tt_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets,
(ea_turnover+kg_turnover+sb_turnover+bbin_turnover+kg2_turnover+pt_turnover+tt_turnover+gg_turnover+ag_turnover+vr_turnover+xj_turnover+imes_turnover+ob_turnover+ysb_turnover+pg_turnover+lb_turnover+ky_turnover+rg_turnover) as turnover
from ofAffDailyReportFinance a with(nolock)
left join ofAffDailyReportGame b with(nolock) on a.financeDay=b.gameReportDay and a.account=b.account
where financeDay >= @startTime and financeDay<=@endTime and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT ";

        /// <summary>
        /// 存取报表
        /// </summary>
        public const string DepositWithdrawReportSql = @"SELECT TT.*,oat.isLeaf FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover
from ofAffDailyReportFinance a with(nolock)
where financeDay >= @startTime and financeDay<=@endTime and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex";

        /// <summary>
        /// 存取报表 总计
        /// </summary>
        public const string DepositWithdrawTotalReportSql = @" select SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover
from ofAffDailyReportFinance a with(nolock)
left join ofAffiliateTree oat on oat.agentAccount = a.account
where financeDay >= @startTime and financeDay<=@endTime and oat.agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount=@agentAccount), '%') 
) dd ";

        /// <summary>
        /// 存取报表统计总记录
        /// </summary>
        public const string DepositWithdrawReportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.depositAmount desc)AS Row, * from (
select dd.account,SUM(depositAmount) depositAmount,SUM(depositTimes) depositTimes,SUM(withdrawAmount) withdrawAmount,SUM(withdrawTimes) withdrawTimes from (
select  a.account,a.financeDay,a.depositAmount,a.depositTimes,a.withdrawAmount,a.withdrawTimes,a.bonus,a.promo,a.turnover
from ofAffDailyReportFinance a with(nolock)
where financeDay >= @startTime and financeDay<=@endTime and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT ";

        /// <summary>
        /// 游戏报表
        /// </summary>
        public const string GameReportSql = @"SELECT TT.*,oat.isLeaf FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(tt_winlost) tt_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(tt_bet) tt_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,tt_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,tt_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex ";

        /// <summary>
        /// 游戏报表 统计
        /// </summary>
        public const string GameTotalReportSql = @"select SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(tt_winlost) tt_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(tt_bet) tt_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,tt_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,tt_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofAffDailyReportGame a with(nolock)
left join ofAffiliateTree oat on oat.agentAccount = a.account
where gameReportDay >= @startTime and gameReportDay<=@endTime  and oat.agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount=@agentAccount), '%') 
) dd ";

        /// <summary>
        /// 游戏报表返回记录数
        /// </summary>
        public const string GameReportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(tt_winlost) tt_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(tt_bet) tt_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,tt_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,tt_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofAffDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT ";

        /// <summary>
        /// 游戏报表
        /// </summary>
        public const string GameTurnoverSql = @"SELECT TT.*,oat.isLeaf FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_turnover desc)AS Row, * from (
select dd.account,SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(tt_turnover) tt_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover from (
select  a.account,a.financeDay,ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,tt_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover
from ofAffDailyReportFinance a with(nolock)
where financeDay >= @startTime and financeDay<=@endTime  and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT
left join ofAffiliateTree oat on oat.agentAccount= TT.account
 WHERE TT.Row between @startIndex and @endIndex ";

        /// <summary>
        /// 游戏报表 统计
        /// </summary>
        public const string GameTotalTurnoverSql = @"select SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(tt_turnover) tt_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover from (
select  a.account,a.financeDay,ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,tt_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover
from ofAffDailyReportFinance a with(nolock)
left join ofAffiliateTree oat with(nolock) on oat.agentAccount = a.account
where financeDay >= @startTime and financeDay<=@endTime  and oat.agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount=@agentAccount), '%') 
) dd ";

        /// <summary>
        /// 游戏返水 记录统计总数
        /// </summary>
        public const string GameTurnoverCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_turnover desc)AS Row, * from (
select dd.account,SUM(ea_turnover) ea_turnover,SUM(kg_turnover) kg_turnover,SUM(sb_turnover) sb_turnover,SUM(bbin_turnover) bbin_turnover,SUM(kg2_turnover) kg2_turnover,SUM(pt_turnover) pt_turnover,SUM(tt_turnover) tt_turnover,SUM(gg_turnover) gg_turnover,SUM(ag_turnover) ag_turnover,SUM(vr_turnover) vr_turnover,SUM(xj_turnover) xj_turnover,SUM(imes_turnover) imes_turnover,SUM(ob_turnover) ob_turnover,SUM(ysb_turnover) ysb_turnover,SUM(pg_turnover) pg_turnover,SUM(lb_turnover) lb_turnover,SUM(ky_turnover) ky_turnover,SUM(rg_turnover) rg_turnover from (
select  a.account,a.financeDay,ea_turnover,kg_turnover,sb_turnover,bbin_turnover,kg2_turnover,pt_turnover,tt_turnover,gg_turnover,ag_turnover,vr_turnover,xj_turnover,imes_turnover,ob_turnover,ysb_turnover,pg_turnover,lb_turnover,ky_turnover,rg_turnover
from ofAffDailyReportFinance a with(nolock)
where financeDay >= @startTime and financeDay<=@endTime  and a.account in (select agentAccount from ofAffiliateTree oa where  oa.parentId = (select agentId from ofAffiliateTree where agentAccount=@agentAccount) )
) dd
group by dd.account
 ) T
) 
TT ";


        /// <summary>
        /// 会员游戏报表
        /// </summary>
        public const string PlayerGameReportSql = @"SELECT * FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(tt_winlost) tt_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(tt_bet) tt_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,tt_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,tt_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.affaccount=@agentAccount {0}
) dd
group by dd.account
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex ";
        /// <summary>
        /// 会员游戏报表记录数
        /// </summary>
        public const string PlayerGameReportCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.xj_winlost desc)AS Row, * from (
select dd.account,SUM(ea_winlost) ea_winlost,SUM(kg_winlost) kg_winlost,SUM(sb_winlost) sb_winlost,SUM(bbin_winlost) bbin_winlost,SUM(kg2_winlost) kg2_winlost,SUM(pt_winlost) pt_winlost,SUM(tt_winlost) tt_winlost,SUM(gg_winlost) gg_winlost,SUM(ag_winlost) ag_winlost,SUM(vr_winlost) vr_winlost,SUM(xj_winlost) xj_winlost,SUM(imes_winlost) imes_winlost,SUM(ob_winlost) ob_winlost,SUM(ysb_winlost) ysb_winlost,SUM(pg_winlost) pg_winlost,SUM(lb_winlost) lb_winlost,SUM(ky_winlost) ky_winlost,SUM(rg_winlost) rg_winlost,
SUM(ea_bet) ea_bet,SUM(kg_bet) kg_bet,SUM(sb_bet) sb_bet,SUM(bbin_bet) bbin_bet,SUM(kg2_bet) kg2_bet,SUM(pt_bet) pt_bet,SUM(tt_bet) tt_bet,SUM(gg_bet) gg_bet,SUM(ag_bet) ag_bet,SUM(vr_bet) vr_bet,SUM(xj_bet) xj_bet,SUM(imes_bet) imes_bet,SUM(ob_bet) ob_bet,SUM(ysb_bet) ysb_bet,SUM(pg_bet) pg_bet,SUM(lb_bet) lb_bet,SUM(ky_bet) ky_bet,SUM(rg_bet) rg_bet from (
select  a.account,a.gameReportDay,ea_winlost,kg_winlost,sb_winlost,bbin_winlost,kg2_winlost,pt_winlost,tt_winlost,gg_winlost,ag_winlost,vr_winlost,xj_winlost,imes_winlost,ob_winlost,ysb_winlost,pg_winlost,lb_winlost,ky_winlost,rg_winlost,
ea_bet,kg_bet,sb_bet,bbin_bet,kg2_bet,pt_bet,tt_bet,gg_bet,ag_bet,vr_bet,xj_bet,imes_bet,ob_bet,ysb_bet,pg_bet,lb_bet,ky_bet,rg_bet
from ofPlayerDailyReportGame a with(nolock)
where gameReportDay >= @startTime and gameReportDay<=@endTime  and a.affaccount=@agentAccount {0}
) dd
group by dd.account
 ) T
) 
TT ";
        /// <summary>
        /// 根据代理账号获取代理账号上级层级
        /// </summary>
        public const string LoadAgentAccounts = @"select agentAccounts from ofAffiliateTree where agentAccount=@agentAccount ";
        /// <summary>
        /// 更新代理登录IP 和MAC
        /// </summary>
        public const string UpdateAgentLoginSql = @"update [ofAffiliate] set ipaddress=@ipaddress, macaddress=@macaddress,lastLoginDate=@lastLoginDate where account=@account ";
        
        /// <summary>
        /// 记录代理登录信息
        /// </summary>
        public const string InsertAccountLoginMsgSql = @"INSERT INTO [dbo].[ofAccountLoginMsgs]
           ([account],[accountType],[fg],[ipaddress])
     VALUES
           (@account,@accountType,@fg,@ipaddress)";

        /// <summary>
        /// 查询代理本月结算数据 延迟45s bayo
        /// </summary>
        public const string QueryAgentMonthlyReportDataSql = @"select sum(depositAmount) depositAmount, sum(withdrawAmount) withdrawAmount, sum(promo+bonus) rewards, 
 sum(pg_bet+jdb_bet+evo_bet+b9w_bet+turbo_bet+spribe_bet+jl_bet) wagered,
 sum(pg_winlost+jdb_winlost+evo_winlost+b9w_winlost+turbo_winlost+spribe_winlost+jl_winlost) winlost,
 sum(pg_turnover+jdb_turnover+evo_turnover+b9w_turnover+turbo_turnover+spribe_turnover+jl_turnover) rebate
 from ofAffDailyReportFG with(nolock)
 where account=@affaccount and financeGameDay>=@startTime and financeGameDay<@endTime ";

        /// <summary>
        /// 查询代理本月结算数据 延迟45s boss
        /// </summary>
        public const string QueryAgentMonthlyReportDataSql_boss = @" select sum(depositAmount) depositAmount, sum(withdrawAmount) withdrawAmount, sum(bonus+promo+rebate) rewards, sum(depositFee+withdrawFee) fee,
sum(fb_winlost) fb_winlost,sum(imsp_winlost) imsp_winlost,sum(tf_winlost) tf_winlost,sum(imes_winlost) imes_winlost,
 sum(fb_bet+imsp_bet+tf_bet+imes_bet) wagered,
 sum(fb_winlost+imsp_winlost+tf_winlost+imes_winlost) winlost,
 sum(fb_turnover+imsp_turnover+tf_turnover+imes_turnover) rebate
 from ofAffDailyReportFG with(nolock)
 where account=@affaccount and financeGameDay>=@startTime and financeGameDay<@endTime";


        /// <summary>
        /// 获取代理统计数据
        /// 1.时间段内---本月新增用户
        /// 2.总用户
        /// 3.时间段内---总存款
        /// 4.时间段内---总提款
        /// 5.时间段内---首存客人
        /// 6.时间段内---活跃人数
        /// 7.时间段内---总输赢
        /// </summary>
        public const string QueryAgentDataSql = @"select 
(select count(id) from ofUser where createtime >=@startTime and createtime<@endTime and affaccount=@affaccount) as registerNumber,
(select count(id) from ofUser where affaccount=@affaccount and status = 1 ) as totalUserNumber,
(select sum(amount) from ofDeposit 
where requesttime >=@startTime and requesttime <@endTime and ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime) <@endTime  and status=1 and affaccount=@affaccount) as depositTotal,
(select sum(amount) from ofWithdrawal where requesttime>=@startTime and requesttime<@endTime and paytime>=@startTime and paytime<@endTime and status=2 and affaccount=@affaccount ) as withdrawTotal,
(select count(pr.id) from ofPaymentRecord pr left join ofUser u on pr.account=u.account 
where pr.createtime>=@startTime and pr.createtime<@endTime and preBalance=0 and summaryType=1 and u.affaccount=@affaccount) as firstDepositNumber,
(select count(account) from ofDeposit 
where requesttime >=@startTime and requesttime <@endTime and ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime) <@endTime  and status=1 and affaccount=@affaccount group by account ) as depositUserTotal,
(select sum(a.winlost) from ofTurnover a
left join ofUser u on a.account = u.account where a.status=1 and a.begindate>=@startTime and a.begindate<@endTime and u.affaccount=@affaccount ) as totalWinlost";


        /// <summary>
        /// app  首页数据 统计 
        /// </summary>

        public const string QueryAgentAppReportDataSql = @" select * from (
select count(*) activityUserCount from (
select a.account,
sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+tt_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet) as bets,
sum(turnover) as turnovers,sum(a.depositAmount) depositAmount
from ofPlayerDailyReportFinance a with(nolock)
left join ofPlayerDailyReportGame b with(nolock) on a.account = b.account and a.financeDay = b.gameReportDay
where a.financeDay>=@startTime and a.financeDay<=@endTime and a.affaccount='{0}'
group by a.account) c
where c.depositAmount >= 500 and c.turnovers > 0 and c.bets >= 1000
) d
,(select count(id) userCount from ofUser where affaccount='{0}' and status=1) e
,(select sum(turnover) totalTurnover,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,sum(bonus) totalBonus,sum(promo) totalPromo,
 sum(isnull(registerNumber,0)) totalRegisterNumber,sum(isnull(firstDepositNumber,0)) totalFirstDepositNumber,
  sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost) totalWinlost,
sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet) totalBet

 from (
 select allk.account, allk.d,f.depositAmount,f.withdrawAmount,f.bonus,f.turnover,f.promo,r.registerNumber,r.firstDepositNumber,
 g.ea_bet,g.kg_bet,g.sb_bet,g.bbin_bet,g.kg2_bet,g.pt_bet,g.ttg_bet,g.gg_bet,g.ag_bet,g.vr_bet,g.xj_bet,g.imes_bet,g.ob_bet,g.ysb_bet,g.pg_bet,g.lb_bet,g.ky_bet,g.rg_bet,g.dscp_bet,g.dses_bet,
 g.ea_winlost,g.kg_winlost,g.sb_winlost,g.bbin_winlost,g.kg2_winlost,g.pt_winlost,g.ttg_winlost,g.gg_winlost,g.ag_winlost,g.vr_winlost,g.xj_winlost,g.imes_winlost,g.ob_winlost,g.ysb_winlost,g.pg_winlost,g.lb_winlost,g.ky_winlost,g.rg_winlost,g.dscp_winlost,g.dses_winlost
 from ( select ff.account, ff.financeDay d from ofAffDailyReportFinance ff union
 select g.account,g.gameReportDay d from ofAffDailyReportGame g union
 select r.account,r.reportDay d from ofAffDataReport r
 ) allk 
 left join ofAffDailyReportFinance f on allk.account= f.account and allk.d = f.financeDay 
 left join ofAffDailyReportGame g on allk.account = g.account and allk.d= g.gameReportDay
 left join ofAffDataReport r on allk.account = r.account and allk.d = r.reportDay
 ) aaa
 left join ofAffiliateTree oat on oat.agentAccount = aaa.account 
 where aaa.d>=@startTime and aaa.d<=@endTime and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree where agentAccount='{0}' ) as varchar)+'%' ) f ";

        /// <summary>
        /// 当前代理下级所有代理自身数据 - 分组统计
        /// 可以删除
        /// </summary>
        public const string QueryAllAgentChildsAppSql_old = @" SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
select aaa.account, sum(turnover) totalTurnover,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,sum(bonus) totalBonus,sum(promo) totalPromo,
 sum(isnull(registerNumber,0)) totalRegisterNumber,sum(isnull(firstDepositNumber,0)) totalFirstDepositNumber,
  sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost) totalWinlost,
sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet) totalBet

 from (
 select allk.account, allk.d,f.depositAmount,f.withdrawAmount,f.bonus,f.turnover,f.promo,r.registerNumber,r.firstDepositNumber,
 g.ea_bet,g.kg_bet,g.sb_bet,g.bbin_bet,g.kg2_bet,g.pt_bet,g.ttg_bet,g.gg_bet,g.ag_bet,g.vr_bet,g.xj_bet,g.imes_bet,g.ob_bet,g.ysb_bet,g.pg_bet,g.lb_bet,g.ky_bet,g.rg_bet,g.dscp_bet,g.dses_bet,
 g.ea_winlost,g.kg_winlost,g.sb_winlost,g.bbin_winlost,g.kg2_winlost,g.pt_winlost,g.ttg_winlost,g.gg_winlost,g.ag_winlost,g.vr_winlost,g.xj_winlost,g.imes_winlost,g.ob_winlost,g.ysb_winlost,g.pg_winlost,g.lb_winlost,g.ky_winlost,g.rg_winlost,g.dscp_winlost,g.dses_winlost
 from ( select ff.account, ff.financeDay d from ofAffDailyReportFinance ff with(nolock) union
 select g.account,g.gameReportDay d from ofAffDailyReportGame g with(nolock) union
 select r.account,r.reportDay d from ofAffDataReport r with(nolock)
 ) allk 
 left join ofAffDailyReportFinance f with(nolock) on allk.account= f.account and allk.d = f.financeDay 
 left join ofAffDailyReportGame g with(nolock) on allk.account = g.account and allk.d= g.gameReportDay
 left join ofAffDataReport r with(nolock) on allk.account = r.account and allk.d = r.reportDay
 ) aaa
 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = aaa.account 
 where aaa.d>=@startTime and aaa.d<=@endTime and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock)  where agentAccount='{0}' ) as varchar)+'%'
 group by aaa.account
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex 
OPTION (RECOMPILE)";
        /// <summary>
        /// 当前代理下级所有代理自身数据 - 分组统计
        /// 优化后效果
        /// </summary>
        public const string QueryAllAgentChildsAppSql_bak = @" SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from 
(
	select account,totalTurnover,totalDepositAmount,totalWithdrawAmount,totalDepositFee,totalWithdrawFee,fee,totalBonus,totalPromo,totalWinlost,totalBet,totalRegisterNumber,totalFirstDepositNumber from
	(
		select aaa.account, sum(turnover) totalTurnover,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,SUM(depositFee) totalDepositFee,SUM(withdrawFee) totalWithdrawFee,SUM(depositFee+withdrawFee) fee,SUM(realBankFee) totalRealBankFee,sum(bonus+promo+rebate) totalBonus,sum(promo) totalPromo,
		  sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost) totalWinlost,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet) totalBet

		 from 
		 (
			 select allk.account, allk.d,f.depositAmount,f.withdrawAmount,f.depositFee,f.withdrawFee,f.realBankFee,f.bonus,f.turnover,f.promo,f.rebate,
			 g.ea_bet,g.kg_bet,g.sb_bet,g.bbin_bet,g.kg2_bet,g.pt_bet,g.ttg_bet,g.gg_bet,g.ag_bet,g.vr_bet,g.xj_bet,g.imes_bet,g.ob_bet,g.ysb_bet,g.pg_bet,g.lb_bet,g.ky_bet,g.rg_bet,g.dscp_bet,g.dses_bet,g.bg_bet,g.eb_bet,g.td_bet,
			 g.ea_winlost,g.kg_winlost,g.sb_winlost,g.bbin_winlost,g.kg2_winlost,g.pt_winlost,g.ttg_winlost,g.gg_winlost,g.ag_winlost,g.vr_winlost,g.xj_winlost,g.imes_winlost,g.ob_winlost,g.ysb_winlost,g.pg_winlost,g.lb_winlost,g.ky_winlost,g.rg_winlost,g.dscp_winlost,g.dses_winlost,g.bg_winlost,g.eb_winlost,g.td_winlost
			 from 
			 ( 
				 select ff.account, ff.financeDay d from ofAffDailyReportFinance ff with(nolock) union
				 select g.account,g.gameReportDay d from ofAffDailyReportGame g with(nolock)
			 ) allk 
			 left join ofAffDailyReportFinance f with(nolock) on allk.account= f.account and allk.d = f.financeDay 
			 left join ofAffDailyReportGame g with(nolock) on allk.account = g.account and allk.d= g.gameReportDay
		 ) aaa
		 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = aaa.account 
		 where aaa.d>=@startTime and aaa.d<=@endTime and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock)  where agentAccount='{0}' ) as varchar)+'%'
		 group by aaa.account
	 ) aaa
left join (select count(id) totalRegisterNumber,affaccount g_affaccount from ofUser with(nolock) where cast(format(createtime,'yyyyMMdd') as int)>=@startTime and cast(format(createtime,'yyyyMMdd') as int)<=@endTime and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) and status=1
	 group by affaccount) g on aaa.account=g.g_affaccount
	left join (select count(id) totalFirstDepositNumber,affaccount h_affaccount from userFirstDepositDetail with(nolock) where depositDay>=@startTime and depositDay<=@endTime and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) 
	group by affaccount) h on aaa.account=h.h_affaccount
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
OPTION (RECOMPILE) ";
        /// <summary>
        /// 20200711 ching 新加 tf aw cq
        /// 20241118 加rsg
        /// 20250205 加 bti
        /// </summary>
        public const string QueryAllAgentChildsAppSql = @" SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from 
(
	select aaa.*
		 from 
		 (
			 select allk.account,d.totalFirstDepositNumber,g.totalRegisterNumber,totalTurnover,totalDepositAmount,totalWithdrawAmount,totalDepositFee,totalWithdrawFee,fee,totalRealBankFee,totalBonus,totalPromo,totalWinlost,totalBet
			 from 
			 ( 
				 select ff.account from (SELECT * FROM u_emp(@startTime,@endTime,@account)) ff union
				 select gg.g_affaccount account from (SELECT * FROM u_register(@startTime,@endTime,@account)) gg  union
				 select dd.h_affaccount account from (SELECT * FROM u_depositNumber(@startTime,@endTime,@account)) dd 
			 ) allk 
			 left join (SELECT * FROM u_emp(@startTime,@endTime,@account)) f  on allk.account= f.account 
			 left join (SELECT * FROM u_register(@startTime,@endTime,@account)) g on allk.account = g.g_affaccount
			 left join (SELECT * FROM u_depositNumber(@startTime,@endTime,@account)) d on allk.account = d.h_affaccount
		 ) aaa
		 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = aaa.account 
		 where oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock)  where agentAccount=@account ) as varchar)+'%'
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex 
OPTION (RECOMPILE) ";

        #region 当前代理和下级代理日结算总额
        public const string QueryAgentBilledTotalSql = @"select sum(profit) billedProfit
from afBillReport br with(nolock)
left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
where billDay>={0} and billDay<={1} and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'  ";

        #endregion


        #region 当前代理和下级代理在指定时间内日结算总额
        public const string QueryAgentBilledDailyCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.profit desc)AS Row, * from (
	select profit,account,billDay
	from afBillReport with(nolock) 
	where billDay>=@startTime and billDay<=@endTime
	and account in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' )
	--group by account
 ) T
) 
TT
OPTION (RECOMPILE) ";

        public const string QueryAgentBilledDailySql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.profit desc)AS Row, * from 
(
	select account,profit,grossamount,turnover, bonus,platformfee, withdrawfee,billDay
	from afBillReport with(nolock) 
	where billDay>=@startTime and billDay<=@endTime
	and account in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' )
	--group by account
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
OPTION (RECOMPILE) ";

        #endregion
        #region 代理月结
        /// <summary>
        /// 代理月地总和
        /// </summary>
        public const string QueryAgentLastMontBilledSql = @" select sum(amount) amount,sum(subTotalAmount) subTotalAmount,billedTime
from {3} br with(nolock)
left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
where billedTime>={0} and billedTime<={1} and status=1 and settleType=0
and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%' 
group by billedTime
order by billedTime desc  ";
        /// <summary>
        /// 代理和子代（结算总和）
        /// </summary>
        public const string QueryAgentLastMontBilledSql_New = @" select sum(amount) amount,sum(subTotalAmount) subTotalAmount,billedTime,sum(profit) profit
from {3} br with(nolock)
left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
where billedTime>={0} and billedTime<={1} and status={5} and {4}
and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%' 
group by billedTime
order by billedTime desc  ";
        /// <summary>
        /// 代理自身结算总和
        /// </summary>
        public const string QueryAgentLastMontBilledSelfSql_New = @" select sum(amount) amount,sum(subTotalAmount) subTotalAmount,billedTime,sum(profit) profit
from {3} br with(nolock)
where billedTime>={0} and billedTime<={1} and status={5} and {4}
and br.account='{2}'
group by billedTime
order by billedTime desc  ";

        /// <summary>
        /// 历史结算数据
        /// </summary>
        public const string QueryAgentLastMonthBilledHistorySql = @"select sum(amount) amount,billedTime
from ofAffBillReport br with(nolock)
left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
where billedTime>={0} and billedTime<={1} 
and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%' 
group by billedTime
order by billedTime desc ";

        #region 代理账户  入账/出账
        /// <summary>
        /// 入账记录  sql
        /// </summary>
        public const string QueryAgentAmountInListCountSql = @" SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from (
	select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) b, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) >0
	and {2}
	union
	select account,createtime,bonus b, summary remark, settleType=6
	from ofPaymentRecord
	where summaryType = 1004 and account='{3}' and bonus>0
 ) T
) 
TT ";


        public const string QueryAgentAmountInListSql = @" SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from 
(
	select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) updateAmount, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) >0
	and {2}
	union
	select account,createtime,bonus updateAmount, summary remark, settleType=6
	from ofPaymentRecord
	where summaryType = 1004 and account='{3}' and bonus>0
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by createtime desc
OPTION (RECOMPILE) ";

        /// <summary>
        /// 出账记录  sql
        /// </summary>
        public const string QueryAgentAmountOutListCountSql = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from (
	select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) b, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) <0
	and {2}
	union
	select account,createtime,bonus b, summary remark, settleType=6
	from ofPaymentRecord
	where summaryType = 1004 and account='{3}' and bonus<0
 ) T
) 
TT";
        public const string QueryAgentAmountOutListSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from 
(
	  select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) b, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) < 0
	and {2}
	union
	select account,createtime,bonus b, summary remark, settleType=6
	from ofPaymentRecord
	where summaryType = 1004 and account='{3}' and bonus < 0
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by createtime desc
OPTION (RECOMPILE) ";
        /// <summary>
        /// 全部的账变
        /// </summary>
        public const string QueryAgentAmountAllListCountSql = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from (
	select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) updateAmount, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) <> 0
	and {2}
union
select account,createtime,bonus updateAmount, summary remark, settleType=6
from ofPaymentRecord
where summaryType = 1004 and account='{3}' and bonus<>0
 ) T
) 
TT";
        /// <summary>
        /// 全部的账变
        /// </summary>
        public const string QueryAgentAmountAllListSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from 
(
	  select account,createtime,(balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) updateAmount, remark, settleType
	from ofAffBillReport br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = 1 and settleType>=0 and settleType<=3 and (balanceAfter+subTotalAmountAfter-balancePre+subTotalAmountPre) <> 0
	and {2}
union
select account,createtime,bonus updateAmount, summary remark, settleType=6
from ofPaymentRecord
where summaryType = 1004 and account='{3}' and bonus<>0
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by createtime desc
OPTION (RECOMPILE) ";

        #endregion

        /// <summary>
        /// 代理和子代理
        /// </summary>
        public const string QueryAgentLastMontBilledListCountSql_New = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.amount desc)AS Row, * from (
	select account,amount,billedTime,proportion,grossamount,turnover,bonus,platformfee,withdrawfee,profit
	from {3} br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = {5} and {4}
	and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'
 ) T
) 
TT ";
        /// <summary>
        /// 代理自身
        /// </summary>
        public const string QueryAgentLastMontBilledListSelfCountSql_New = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.amount desc)AS Row, * from (
	select account,amount,billedTime,proportion,grossamount,turnover,bonus,platformfee,withdrawfee,profit
	from {3} br with(nolock)
	where billedTime>={0} and billedTime<={1} and status = {5} and {4}
	and br.account='{2}'
 ) T
) 
TT ";

        public const string QueryAgentLastMontBilledListGroupCountSql_New = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.billedTime desc)AS Row, * from (
	select account,billedTime
	from {3} br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = {5} and {4}
	and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'
group by br.account,billedTime
 ) T
) 
TT ";
        /// <summary>
        /// 代理自身
        /// </summary>
        public const string QueryAgentLastMontBilledListGroupSelfCountSql_New = @"SELECT count(account) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.billedTime desc)AS Row, * from (
	select account,billedTime
	from {3} br with(nolock)
	where billedTime>={0} and billedTime<={1} and status = {5} and {4}
	and br.account='{2}'
group by br.account,billedTime
 ) T
) 
TT ";

        public const string QueryAgentLastMontBilledListGroupSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.amount desc)AS Row, * from 
(
	select account,sum(amount+subTotalAmount) amount,billedTime,sum(grossamount) grossamount,sum(turnover) turnover,sum(bonus) bonus,
	sum(platformfee) platformfee,sum(withdrawfee) withdrawfee, sum(profit) profit
	from {3} br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = {4} and settleType=0
	and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'
group by br.account,billedTime
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by billedTime desc
OPTION (RECOMPILE) ";

        /// <summary>
        /// 代理自身
        /// </summary>
        public const string QueryAgentLastMontBilledListGroupSelfSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.amount desc)AS Row, * from 
(
	select account,sum(amount+subTotalAmount) amount,billedTime,sum(grossamount) grossamount,sum(turnover) turnover,sum(bonus) bonus,
	sum(platformfee) platformfee,sum(withdrawfee) withdrawfee, sum(profit) profit
	from {3} br with(nolock)
	where billedTime>={0} and billedTime<={1} and status = {4} and settleType=0
	and br.account='{2}'
group by br.account,billedTime
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by billedTime desc
OPTION (RECOMPILE) ";

        /// <summary>
        /// 代理和子代理
        /// </summary>
        public const string QueryAgentLastMontBilledListSql_20211129 = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.billedTime desc)AS Row, * from 
(
	select account,amount,subTotalAmount,betBonus,billedTime,br.createtime,br.verifytime,proportion,grossamount,turnover,bonus,platformfee,withdrawfee,profit,remark,unSettleAmountAfter,bet,unSettleAmount,amount+subTotalAmount+unSettleAmount totalSettle,deposit_fee,withdraw_fee,depositamount,withdrawamount
	from {3} br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = {4} and settleType=0
	and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by billedTime desc
OPTION (RECOMPILE) ";
        /// <summary>
        /// 代理自身
        /// </summary>
        public const string QueryAgentLastMontBilledListSelfSql_20211129 = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.billedTime desc)AS Row, * from 
(
	select account,amount,subTotalAmount,betBonus,billedTime,br.createtime,br.verifytime,proportion,grossamount,turnover,bonus,platformfee,withdrawfee,profit,remark,unSettleAmountAfter,bet,unSettleAmount,amount+subTotalAmount+unSettleAmount totalSettle,deposit_fee,withdraw_fee,depositamount,withdrawamount
	from {3} br with(nolock)
	where billedTime>={0} and billedTime<={1} and status = {4} and settleType=0
	and br.account = '{2}'
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by billedTime desc
OPTION (RECOMPILE) ";


        public const string QueryAgentLastMontBilledListSql_New = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.amount desc)AS Row, * from 
(
	select account,amount+subTotalAmount amount,billedTime,proportion,grossamount,turnover,bonus,platformfee,withdrawfee,profit,remark,unSettleAmountAfter
	from {3} br with(nolock)
	left join ofAffiliateTree oat with(nolock) on oat.agentAccount = br.account 
	where billedTime>={0} and billedTime<={1} and status = {5} and {4}
	and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{2}' ) as varchar)+'%'
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
order by billedTime desc
OPTION (RECOMPILE) ";

        /// <summary>
        /// 当前代理的流水和活跃会员数
        /// </summary>
        public const string allowedSettleSqlV1 = @"select affaccount,count(affaccount) cnt
from 
(
select fg.account,fg.affaccount,SUM(depositAmount) totalDeposit,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet) totalBet
from ofPlayerDailyReportFG fg with(nolock)
where financeGameDay>={0} and financeGameDay<={1}
group by fg.account ,fg.affaccount
) a
where a.totalDeposit>=500 and a.totalBet >= 1000 and affaccount='{2}'
group by affaccount";

        #endregion


        #region 当前代理和下级代理在指定时间内注册会员的量
        public const string QueryAgentRegisterPlayerCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalRegisterNumber desc)AS Row, * from (
	select count(id) totalRegisterNumber,affaccount account 
	from ofUser with(nolock) where cast(format(createtime,'yyyyMMdd') as int)>=@startTime and cast(format(createtime,'yyyyMMdd') as int)<=@endTime
	and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) and status=1
	group by affaccount
 ) T
) 
TT
OPTION (RECOMPILE) ";
        public const string QueryAgentRegisterPlayerSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalRegisterNumber desc)AS Row, * from 
(
	select count(id) totalRegisterNumber,affaccount account 
	from ofUser with(nolock) where cast(format(createtime,'yyyyMMdd') as int)>=@startTime and cast(format(createtime,'yyyyMMdd') as int)<=@endTime
	and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) and status=1
	group by affaccount
 ) T
) 
TT
 WHERE TT.Row between @startIndex and @endIndex
OPTION (RECOMPILE)";
        #endregion

        /// <summary>
        /// 最新优化查询
        /// 20241118 加 rsg
        /// 20250205 加 bti
        /// </summary>

        public const string QueryAllAgentChildsAppCountSql = @" SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.totalWinlost desc)AS Row, * from (
	select * from
	(
		select aaa.account, sum(turnover) totalTurnover,sum(depositAmount) totalDepositAmount,sum(withdrawAmount) totalWithdrawAmount,sum(bonus) totalBonus,sum(promo) totalPromo,
		  sum(ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost+dscp_winlost+dses_winlost+bg_winlost+eb_winlost+td_winlost+tf_winlost+aw_winlost+cq_winlost+kg3_winlost+obg_winlost+imsp_winlost+sw_winlost+cr_winlost+obcv_winlost+fb_winlost+jdb_winlost+db_winlost+rsg_winlost+bti_winlost) totalWinlost,
		sum(ea_bet+kg_bet+sb_bet+bbin_bet+kg2_bet+pt_bet+ttg_bet+gg_bet+ag_bet+vr_bet+xj_bet+imes_bet+ob_bet+ysb_bet+pg_bet+lb_bet+ky_bet+rg_bet+dscp_bet+dses_bet+bg_bet+eb_bet+td_bet+tf_bet+aw_bet+cq_bet+kg3_bet+obg_bet+imsp_bet+sw_bet+cr_bet+obcv_bet+fb_bet+jdb_bet+db_bet+rsg_bet+bti_bet) totalBet

		 from 
		 (
			 select g.account, g.financeGameDay,g.depositAmount,g.withdrawAmount,g.bonus,g.turnover,g.promo,
			 g.ea_bet,g.kg_bet,g.sb_bet,g.bbin_bet,g.kg2_bet,g.pt_bet,g.ttg_bet,g.gg_bet,g.ag_bet,g.vr_bet,g.xj_bet,g.imes_bet,g.ob_bet,g.ysb_bet,g.pg_bet,g.lb_bet,g.ky_bet,g.rg_bet,g.dscp_bet,g.dses_bet,g.bg_bet,g.eb_bet,g.td_bet,g.tf_bet,g.aw_bet,g.cq_bet,g.kg3_bet,g.obg_bet,g.imsp_bet,g.sw_bet,g.cr_bet,g.obcv_bet,g.fb_bet,g.jdb_bet,g.db_bet,g.rsg_bet,g.bti_bet,
			 g.ea_winlost,g.kg_winlost,g.sb_winlost,g.bbin_winlost,g.kg2_winlost,g.pt_winlost,g.ttg_winlost,g.gg_winlost,g.ag_winlost,g.vr_winlost,g.xj_winlost,g.imes_winlost,g.ob_winlost,g.ysb_winlost,g.pg_winlost,g.lb_winlost,
			 g.ky_winlost,g.rg_winlost,g.dscp_winlost,g.dses_winlost,g.bg_winlost,g.eb_winlost,g.td_winlost,g.tf_winlost,g.aw_winlost,g.cq_winlost,g.kg3_winlost,g.obg_winlost,g.imsp_winlost,g.sw_winlost,g.cr_winlost,g.obcv_winlost,g.fb_winlost,g.jdb_winlost,g.db_winlost,g.rsg_winlost,g.bti_winlost
			 from ofAffDailyReportFG g
		 ) aaa 
		 left join ofAffiliateTree oat with(nolock) on oat.agentAccount = aaa.account 
		 where aaa.financeGameDay>=@startTime and aaa.financeGameDay<=@endTime and oat.agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock)  where agentAccount='{0}' ) as varchar)+'%'
		 group by aaa.account
	 ) aaa
	 ,(select count(id) totalRegisterNumber from ofUser with(nolock) where cast(format(createtime,'yyyyMMdd') as int)>=@startTime and cast(format(createtime,'yyyyMMdd') as int)<=@endTime and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) and status=1) g
	,(select count(id) totalFirstDepositNumber from userFirstDepositDetail with(nolock) where depositDay>=@startTime and depositDay<=@endTime and  affaccount in (select agentAccount from ofAffiliateTree with(nolock) where agentIds like '%'+cast((select agentId from ofAffiliateTree with(nolock) where agentAccount='{0}' ) as varchar)+'%' ) ) h

 ) T
) 
TT
OPTION (RECOMPILE) ";

        public const string QueryOBBetRecordCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.winlost desc)AS Row, * from 
(
	select trans_id settleId , bet_team selecttion, bet_type betType ,stake,winlost_amount winlost,odds,vendor_member_id awc_account,winlost_datetime settleTime,transaction_time createTime, home_id, away_id,b.teamName as awc_hn, c.teamName awc_an, ticket_status awc_orderStatus,parlayData,cashOutData,singleParlayData from (
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id, away_id,parlayData,cashOutData,singleParlayData, ticket_status from awc_sp_ob_BetDetails with(nolock)
	union
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id=0, away_id=0,parlayData='',cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetNumberDetails with(nolock)
	union
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id, away_id,parlayData,cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetVirtualSportDetails with(nolock)
	union
	select trans_id,bet_team='*',bet_type,stake,winlost_amount,odds=0,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id=0, away_id=0,parlayData='',cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetCasinoDetails with(nolock)
	) a
	left join awc_sp_ob_TeamName b on a.home_id= b.tid
	left join awc_sp_ob_TeamName c on a.away_id= c.tid
    {0}
 ) T
) 
TT
OPTION (RECOMPILE) ";


        public const string QueryOBBetRecordSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createTime desc)AS Row, * from 
(
	select trans_id settleId , bet_team selecttion, bet_type betType ,stake,ABS(winlost_amount) StakeAccurate, winlost_amount winlost,odds,vendor_member_id awc_account, vendor_member_id memberCode,winlost_datetime settleTime,transaction_time createTime, home_id, away_id,b.teamName as awc_hn, c.teamName awc_an, ticket_status awc_orderStatus,parlayData,cashOutData,singleParlayData from (
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id, away_id,parlayData,cashOutData,singleParlayData, ticket_status from awc_sp_ob_BetDetails with(nolock)
	union
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id=0, away_id=0,parlayData='',cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetNumberDetails with(nolock)
	union
	select trans_id,bet_team,bet_type,stake,winlost_amount,odds,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id, away_id,parlayData,cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetVirtualSportDetails with(nolock)
	union
	select trans_id,bet_team='*',bet_type,stake,winlost_amount,odds=0,vendor_member_id,winlost_datetime,transaction_time,wagerStatus, home_id=0, away_id=0,parlayData='',cashOutData='',singleParlayData='', ticket_status from awc_sp_ob_BetCasinoDetails with(nolock)
	) a
	left join awc_sp_ob_TeamName b on a.home_id= b.tid
	left join awc_sp_ob_TeamName c on a.away_id= c.tid
    {0}
 ) T
) 
TT
where TT.Row between @startIndex and @endIndex 
order by createTime desc
OPTION (RECOMPILE) ";


        /// <summary>
        /// 获取今天所以代理下线会员的首存情况
        /// 账号，存款时间，存款金额，存款状态，所属代理
        /// </summary>
        public const string QueryAllAgentChildsFirstDepositSql = @"
select fdp.account,fdp.amount,fdp.paytime,fdp.affaccount, u.status from (
select fdd.account,ISNULL(orderendtime,reviewtime) paytime, fdd.status, fdd.amount,iif(( fdd.affaccount IS not NULL and LEN(RTRIM(LTRIM(fdd.affaccount))) != 0), fdd.affaccount,'allwincity') affaccount
from (
select account from (
	SELECT MIN(ISNULL(orderendtime,reviewtime)) AS t ,account
    FROM  ofDeposit  with(nolock)
	where requesttime>=@startTime and  ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime)<@endTime and status=1 
    GROUP BY account) a
	EXCEPT select account from ofDeposit  with(nolock) where requesttime<@startTime and status=1
	) fd
	 cross apply (select top 1 * from ofDeposit  with(nolock) where requesttime>=@startTime and  ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime)<@endTime and status=1 and account=fd.account
	 order by ISNULL(orderendtime,reviewtime) asc ) fdd
	 )fdp left join ofUser u on u.account = fdp.account
";

    }
}
