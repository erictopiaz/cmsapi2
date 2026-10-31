using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class UserSqlCommandText
    {
        /// <summary>
        /// 会员充值和提款报表
        /// </summary>
        public const string UserDepositWithdrawReportSql = @"select a.account,a.t_amount depositamount,iif(a.depositnumber is not null, a.depositnumber,0) depositnumber,
  iif(b.withdrawalnumber is not null, b.withdrawalnumber, 0) withdrawalnumber, iif(b.w_amount is not null, w_amount, 0) withdrawalamount from (
 select aa.account,sum(aa.t_amount) t_amount,count(id) depositnumber from (
	 select id,account,amount t_amount from ofDeposit
	where account='{0}' and status=1
	union all
	select id,account,amount t_amount from ofDeposit2014
	where account='{0}' and status=1) aa
	group by aa.account
) a
left join (
select account,sum(amount) w_amount,count(id) withdrawalnumber from ofWithdrawal
where account='{0}' and status=2
group by account) b on a.account=b.account";

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public const string LoadUserDatailSql = @"Select top 1 u.*,ue.payPassword,iif(ue.riskTypesAready is null, -1, ue.riskTypesAready) riskTypesAready from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 获取用户信息 + 品牌
        /// </summary>
        public const string LoadUserDatailSql_mer = @"Select top 1 u.id uid,u.currencyType,u.merchantAccount,iif(ue.riskTypesAready is null, -1, ue.riskTypesAready) riskTypesAready from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account and currencyType=@currencycode and merchantAccount=@merchantAccount ";

        /// <summary>
        /// 获取用户信息 会员登录弹框
        /// </summary>
        public const string LoadUserDatailForPopSql_old = @"Select * from ofUser a with(nolock),
(select count(id) leftTimes from ofBonusPromoSelf with(nolock)
where account='{2}' and createtime>'{0}' and createtime<='{1}' and expiredTime>='{1}' and status=0) b
where a.account='{2}'";

        public const string LoadUserDatailForPopSql = @"select top 4 reward ,bet from ofBonusPromoSelf with(nolock)
where account='{2}' and createtime>'{0}' and createtime<='{1}' and expiredTime>='{1}' and status=0 order by createtime desc ";

        /// <summary>
        /// 获取用户信息和对应该等级存提金额限制
        /// </summary>
        public const string LoadUserDatailWithDWSql = @"Select u.*,ue.payPassword, ug.misc umisc,
iif(( ue.withdrawMinAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMinAmount))) != 0), ue.withdrawMinAmount,0) withdrawMinAmount,
iif(( ue.withdrawMaxAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMaxAmount))) != 0), ue.withdrawMaxAmount,0) withdrawMaxAmount,
iif(( ue.isLimitedfixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedfixWithdrawal))) != 0), ue.isLimitedfixWithdrawal,0) isLimitedfixWithdrawal,
iif(( ue.isLimitedAliFixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedAliFixWithdrawal))) != 0), ue.isLimitedAliFixWithdrawal,0) isLimitedAliFixWithdrawal,
iif(( ue.isLimitedWXFixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedWXFixWithdrawal))) != 0), ue.isLimitedWXFixWithdrawal,0) isLimitedWXFixWithdrawal from ofUser u with(nolock)
left join Ofusergroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";


        /// <summary>
        /// 获取用户信息和对应该等级存提金额限制
        /// </summary>
        public const string LoadUserDatailWithDWSqlCurrency = @"Select u.*, {0} umisc,
iif(( ue.withdrawMinAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMinAmount))) != 0), ue.withdrawMinAmount,0) withdrawMinAmount,
iif(( ue.withdrawMaxAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMaxAmount))) != 0), ue.withdrawMaxAmount,0) withdrawMaxAmount,
iif(( ue.isLimitedfixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedfixWithdrawal))) != 0), ue.isLimitedfixWithdrawal,0) isLimitedfixWithdrawal from ofUser u with(nolock)
left join Ofusergroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 获取用户信息和额外特别优惠
        /// </summary>
        public const string LoadUserExtentionSql = @"select u.level,u.misc,u.depositnumber,u.currencyType,u.walletType,u.merchantAccount,u.referrerreg,u.depositamount,ue.specialDepositPromo,ue.sp_status,ue.disabledMids,u.createtime,u.status,u.withdrawcards,iif((ue.riskTypesAready IS not NULL and LEN(RTRIM(LTRIM(ue.riskTypesAready))) != 0),ue.riskTypesAready,0) riskTypesAready,iif((ue.isneedcheckrealname IS not NULL and LEN(RTRIM(LTRIM(ue.isneedcheckrealname))) != 0),ue.isneedcheckrealname,0) isneedcheckrealname from ofUser u with(nolock)
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 获取最新的短信验证码
        /// </summary>
        public const string LoadSendCodeSql = @"select top 1 * from Ofaccountvalidate
where account=@account and type=@type and body=@telephone
order by createtime desc ";

        /// <summary>
        /// 用户的自定义配置
        /// </summary>
        public const string UpdateCustomerSettingSql = @"if(not EXISTS (SELECT top 1 * FROM ofUserCustomerSetting WHERE account=@account))
begin
INSERT INTO [dbo].[ofUserCustomerSetting] ([account],[customerSetting])
     VALUES(@account,@customerSetting)
end
else
begin
update ofUserCustomerSetting set customerSetting=@customerSetting where account=@account
end ";
        /// <summary>
        /// 获取用户的自定义配置
        /// </summary>
        public const string LoadCustomerSettingSql = @"Select customerSetting from ofUserCustomerSetting where account=@account ";


        /// <summary>
        /// 检测数据是否已经被占用
        /// </summary>
        public const string CheckDetailSql = @"select * from (
	select count(telephone) tCount from ofUser with(nolock) where telephone=@telephone and account <> @account
)a,
(select telephone,realname,referrerreg,currencyType from ofUser with(nolock) where account=@account) b";

        /// <summary>
        /// 完善资料，更新
        /// </summary>
        public const string AppendDetailSql = @"update ofUser
set realname=@realname,email=@email,qq=@qq,sex=@sex,birthday=@birthday,fb=@fb
where account=@account ";

        /// <summary>
        /// 获取用户部分数据信息 + 单一钱包 gameWallet
        /// 2025-05-31 品牌 
        /// </summary>
        public const string LoadMemberDetailSql = @"select * from(
select u.id uid,u.account,u.merchantAccount,trynumber,realname,sex,birthday,email,telephone,fb,qq,status, u.lang, ug.levels level,ug.misc_inr,u.phoneCode,iif(ugw.wallet is not null,ugw.wallet,0) gameWallet,
amountactive,depositamount,bonuspoints,withdrawcards,CONVERT(VARCHAR(20), createtime, 120) createtime, level level_id,
u.misc,u.currencyType,amountsuspend,ug.nextLevelNumber,ug.nextLevels,ug.vipIndex,ue.liveUserLevel viplevel,ue.membericon,ue.liveNickname nickname,g.prevlogin prevloginTime
from ofUser u with(nolock)
left join ofUserGroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserExtention ue with(nolock) on u.account=ue.account
left join ofUserGameWallet ugw with(nolock) on u.account = ugw.account
left join (select account,createtime prevlogin from ofLog with(nolock)
where account=@account and event_type=11
order by id desc
OFFSET 1 ROW
FETCH NEXT 1 ROW ONLY) g on u.account = g.account
where u.account=@account) a
,(select iif(len(sum(amount))>0,sum(amount),0) monthdeposit from ofDeposit  with(nolock)
where account=@account and requesttime >=@startTime and ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime)<@endTime and status=1) b
,
(select iif(LEN(sum(totalbets))>0,sum(totalbets),0) monthlybets from ofTurnover with(nolock) where account=@account and begindate>=@startTime and begindate<@endTime and status=1)c,
(select count(account) signCount from ofpromomisc with(nolock)
where account=@account and type=0 and DateDiff(MM,createtime,DATEADD(hour,8,getutcdate()))=0 ) d,
(select body logoUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e,
(select body levelUrls from ofSettings with(nolock)
where code='GROUP_LEVELURL') f";

        /// <summary>
        /// 用记中心加载用户报表数据
        /// </summary>
        public const string LoadAccountReportSql = @"select d_amount,w_amount,turnover,bets,bonus from (
select iif((sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount), 0) d_amount 
FROM  ofDeposit with(nolock)
where account=@account and  ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime)<@endTime and status=1
) a,
(select iif((sum(amount) IS not NULL and LEN(RTRIM(LTRIM(sum(amount)))) != 0), sum(amount), 0) w_amount
from ofwithdrawal a
where account=@account and a.paytime >=@startTime and a.paytime <@endTime and status=2)b, 
(select iif((sum(reward) IS not NULL and LEN(RTRIM(LTRIM(sum(reward)))) != 0), sum(reward), 0) turnover,iif((sum(totalbets) IS not NULL and LEN(RTRIM(LTRIM(sum(totalbets)))) != 0), sum(totalbets), 0) bets
from  ofTurnover with(nolock)
where account=@account and [status] = 1 AND begindate >= @startTime AND begindate < @endTime )c, 
(select iif((sum(reward) IS not NULL and LEN(RTRIM(LTRIM(sum(reward)))) != 0), sum(reward), 0) bonus
from ofBonusPromo with(nolock)
where account=@account and reviewtime >=@startTime and reviewtime <@endTime and status=1 )d";


        /// <summary>
        /// 用记中心加载用户报表数据 外站
        /// </summary>
        public const string LoadAccountReportIntlSql = @"select * from (select iif((sum(effectivebets) IS not NULL and LEN(RTRIM(LTRIM(sum(effectivebets)))) != 0), sum(effectivebets), 0) dailyWaggered,
iif((sum(cmBet) IS not NULL and LEN(RTRIM(LTRIM(sum(cmBet)))) != 0), sum(cmBet), 0) dailyTotalbets,
iif((sum(winlost) IS not NULL and LEN(RTRIM(LTRIM(sum(winlost)))) != 0), sum(winlost), 0) dailyWinlost
from  ofTurnover with(nolock)
where account=@account and [status] = 1 AND begindate >= @yestodayStartTime AND begindate < @yestodayEndTime 
) a,(select iif((sum(effectivebets) IS not NULL and LEN(RTRIM(LTRIM(sum(effectivebets)))) != 0), sum(effectivebets), 0) monthlyWaggered,
iif((sum(cmBet) IS not NULL and LEN(RTRIM(LTRIM(sum(cmBet)))) != 0), sum(cmBet), 0) monthlyTotalbets,
iif((sum(winlost) IS not NULL and LEN(RTRIM(LTRIM(sum(winlost)))) != 0), sum(winlost), 0) monthlyWinlost
from  ofTurnover with(nolock)
where account=@account and [status] = 1 AND begindate >= @monthlyStartTime AND begindate < @monthlyEndTime ) b";



        /// <summary>
        /// 生成登录token 用到 （会员账号登录）
        /// 2025-05-31 品牌
        /// </summary>
        public const string LoadUserInfoSql = @"Select u.account,u.id,u.depositamount,u.status,u.createtime,u.password,u.telephone,u.appGameUrlMode,ue.lockdate, ue.liveNickname nickname,u.currencyType,u.agaccount2,u.bbaffaccount,u.misc,u.account gameAccount,affaccount,u.walletType,u.merchantAccount from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";



        /// <summary>
        /// 查询会员数据信息 （目前包括积分和总余额）
        /// </summary>
        public const string QueryUserDataSql = @"select u.id uid,u.account,u.merchantAccount,trynumber,realname,sex,birthday,email,telephone,fb,qq,status, u.lang, ug.levels level,ug.misc_inr,u.phoneCode,
(u.amountactive + iif(ugw.wallet is not null,ugw.wallet,0)) amount,depositamount,bonuspoints,withdrawcards,CONVERT(VARCHAR(20), createtime, 120) createtime, level level_id,
u.misc,u.currencyType,amountsuspend,ug.nextLevelNumber,ug.nextLevels,ug.vipIndex,ue.liveUserLevel viplevel,ue.membericon,ue.liveNickname nickname
from ofUser u with(nolock)
left join ofUserGroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserExtention ue with(nolock) on u.account=ue.account
left join ofUserGameWallet ugw with(nolock) on u.account = ugw.account
where u.account=@account";

        /// <summary>
        /// 生成登录token 用到  （会员账号查询 或 手机号登录 ）
        /// </summary>
        public const string LoadUserInfoAccountOrPhoneSql = @"Select u.account,u.id,u.depositamount,u.status,u.createtime,u.password,u.telephone,ue.lockdate, ue.liveNickname nickname,u.currencyType from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account
union
Select u.account,u.id,u.depositamount,u.status,u.createtime,u.password,u.telephone,ue.lockdate, ue.liveNickname nickname,u.currencyType from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.telephone=@telephone and u.phoneCode=@phoneCode and u.telephone IS not NULL and LEN(RTRIM(LTRIM(u.telephone))) != 0 and status=1 ";

        /// <summary>
        /// 生成登录token 用到  （手机号登录 ）
        /// </summary>
        public const string LoadUserInfoPhoneSql = @"Select u.account,u.id,u.depositamount,u.status,u.createtime,u.password,u.telephone,ue.lockdate, ue.liveNickname nickname,u.currencyType from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.telephone=@telephone and u.phoneCode=@phoneCode and u.telephone IS not NULL and LEN(RTRIM(LTRIM(u.telephone))) != 0 and status=1 ";



        /// <summary>
        /// app 下载页面,登录，无须产生token
        /// </summary>
        public const string LoadUserInfoSql_V = @"Select u.account,u.depositamount,u.status,u.createtime,u.password,u.telephone,ue.lockdate, ue.liveNickname nickname,,u.currencyType from ofUser u with(nolock) 
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 验证登录成功的ip或指纹
        /// </summary>
        public const string CheckUserLoginIPorFGSql = @"select top 1 id from oflog with(nolock)
where account = @account and remark like N'登录成功%' and ip=@ip
union
select top 1 id from oflog with(nolock)
where account = @account and remark like N'登录成功%' and comment=@fingerid 
OPTION (RECOMPILE)";
        /// <summary>
        /// 检测会员之前是否有登录过
        /// </summary>
        public const string CheckUserLoginBeforeSql = @"select top 1 id from oflog with(nolock)
where account = @account and remark like N'登录成功%'";

        /// <summary>
        /// 验证登录成功的ip   ios 只有ip
        /// </summary>
        public const string CheckUserLoginIPorFGSql_ios = @"select top 1 id from oflog with(nolock)
where account = @account and remark like N'登录成功%' and ip=@ip 
OPTION (RECOMPILE)";

        /// <summary>
        /// 电销使用，确认最后一次电销人员
        /// </summary>
        public const string LoadUserDatailSalerSql_old = @"Select top 1 u.*,cl.createperson saler from ofUser u
left join (select * from ofcalllog where createtime>='2021-01-17') cl on u.account = cl.account
where u.account=@account
order by cl.createtime desc ";

        public const string LoadUserDatailSalerSql = @"Select top 1 u.*,cl.createperson saler, ug.misc umisc,iif((ue.riskTypesAready IS not NULL and LEN(RTRIM(LTRIM(ue.riskTypesAready))) != 0),ue.riskTypesAready,0) riskTypesAready,iif((g.wallet IS not NULL and LEN(RTRIM(LTRIM(g.wallet))) != 0),g.wallet,0) wallet from ofUser u with(nolock)
left join (select top 1 account, createperson from ofcalllog with(nolock) where createtime>='2021-01-17' order by id desc) cl on u.account = cl.account
left join Ofusergroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserGameWallet g with(nolock) on u.account=g.account
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 提款ui 参数sql
        /// 2025-02-05 新加 支付宝定额提款 ui 显示控制
        /// 2025-05-31 品牌区分
        /// </summary>
        public const string LoadUserDatailWithdrawSql = @"Select u.*, ug.misc umisc,ug.misc_inr,
iif(( ue.withdrawMinAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMinAmount))) != 0), ue.withdrawMinAmount,0) withdrawMinAmount,
iif(( ue.withdrawMaxAmount IS not NULL and LEN(RTRIM(LTRIM(ue.withdrawMaxAmount))) != 0), ue.withdrawMaxAmount,0) withdrawMaxAmount,
iif(( ue.isLimitedfixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedfixWithdrawal))) != 0), ue.isLimitedfixWithdrawal,0) isLimitedfixWithdrawal, 
iif(( ue.fixWAmountLimitLevel IS not NULL and LEN(RTRIM(LTRIM(ue.fixWAmountLimitLevel))) != 0), ue.fixWAmountLimitLevel,0) fixWAmountLimitLevel,
iif(( ue.isLimitedAliFixWithdrawal IS not NULL and LEN(RTRIM(LTRIM(ue.isLimitedAliFixWithdrawal))) != 0), ue.isLimitedAliFixWithdrawal,0) isLimitedAliFixWithdrawal from ofUser u with(nolock)
left join Ofusergroup ug with(nolock) on u.level=ug.levelnumber
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account=@account ";

        /// <summary>
        /// 不同vip 访问网站地址不同
        /// </summary>
        public const string LoadVipUrl = @"select body from ofSettings with(nolock)
where code='GROUP_LEVELURL' ";

        /// <summary>
        /// 不同品牌推广会员的链接
        /// </summary>
        public const string SpreadUrlByMerchantAccount = @"select body from ofSettings with(nolock)
where code='spread_url_by_merchant_account' ";

        /// <summary>
        /// 获取用户信息和附加信息
        /// 针对客人，额外加码使用
        /// </summary>
        public const string LoadUserExtentionWithSalerSql = @"Select top 1 u.*,cl.createperson saler,ue.betCodeDetail,ue.b_status from ofUser u
left join (select * from ofcalllog where createtime>='2021-01-17') cl on u.account = cl.account
left join ofUserExtention ue on u.account=ue.account
where u.account=@account
order by cl.createtime desc ";


        /// <summary>
        /// 批量--获取用户信息
        /// </summary>
        public const string LoadUserDatailBatSql = @"Select * from ofUser where account in {0} ";

        /// <summary>
        /// 批量--获取用户信息
        /// 确认批量客人，最后一次电销人员
        /// </summary>
        public const string LoadUserDatailBatSalerSql = @"select u.*,uu.createperson saler from ofUser u
left join 
(select a.account,b.createtime,b.createperson from
(select distinct account from ofcalllog) a
cross apply
(select top(1) * from ofcalllog aa where aa.account = a.account  order by createtime desc) b
where createtime>='2021-01-17'
) uu on uu.account=u.account
where u.account in {0} ";

        /// <summary>
        /// 用户增加金额
        /// </summary>
        public const string UpdateBalanceASql = @"Update ofUser set amountactive = amountactive + @amount  where account=@account";

        /// <summary>
        /// 用户增加金额 并累积充值金额（当前微调金额是 充值金额调整过来的）
        /// depositamount,depositnumber 要累加
        /// </summary>
        public const string UpdateBalanceAndDepositamountSql = @"Update ofUser set amountactive = amountactive + @amount, depositamount= depositamount+@amount, depositnumber=depositnumber+1  where account=@account";

        /// <summary>
        /// 添加余额和积分
        /// </summary>
        public const string UpdateBalanceAWithPointsSql = @"Update ofUser set amountactive = amountactive + @amount, bonuspoints = bonuspoints+@bonuspoints  where account=@account";

        /// <summary>
        /// 更新主账号余额和升级
        /// </summary>
        public const string UpdateBalanceWithLevelSql = @"Update ofUser set amountactive = amountactive + @amount, level = @nextLevel  where account=@account";


        /// <summary>
        /// 用户扣除金额
        /// </summary>
        public const string SubBalanceASql = @"Update ofUser set amountactive = amountactive - @amount  where account=@account and amountactive - @amount>=0";
        /// <summary>
        /// 用户增加积分
        /// </summary>
        public const string UpdatePointsASql = @"Update ofUser set bonuspoints = bonuspoints + @bonuspoints  where account=@account and bonuspoints + @bonuspoints >= 0";

        /// <summary>
        /// 用户扣除积分
        /// </summary>
        public const string UpdateSubPointsASql = @"Update ofUser set bonuspoints = bonuspoints + @bonuspoints  where account=@account and bonuspoints + @bonuspoints >= 0 ";

        /// <summary>
        /// 用户扣除积分并增加金额
        /// </summary>
        public const string UpdatesPointsAndAmountSql = @"Update ofUser set bonuspoints = bonuspoints + @bonuspoints, amountactive = amountactive + @amount  where account=@account and bonuspoints + @bonuspoints >= 0 ";

        /// <summary>
        /// 用户减少
        /// </summary>
        public const string UpdateBalanceDSql = @"(Update ofUser set amountactive = amountactive - @amount  where amountactive - @amount >=0 and account=@account)";

        public const string preBalance = @"(select amountactive from ofUser where account='{0}')";
        public const string afterBalanceA = @"(select amountactive + {0} from ofUser where account='{1}')";
        public const string afterBalanceD = @"(select amountactive - {0} from ofUser where account='{1}')";

        /// <summary>
        /// 代理账变
        /// </summary>
        //public const string preAgentBalance = @"(select balance+subTotalAmount from ofAffiliate where account='{0}')";
        //public const string afterAgentBalanceA = @"(select balance+subTotalAmount + {0} from ofAffiliate where account='{1}')";
        //public const string afterAgentBalanceD = @"(select balance+subTotalAmount - {0} from ofAffiliate where account='{1}')";

        public const string preAgentBalance = @"(select balance+subTotalAmount+unSettleAmount from ofAffiliate where account='{0}')";
        public const string afterAgentBalanceA = @"(select balance+subTotalAmount + unSettleAmount + {0} from ofAffiliate where account='{1}')";
        public const string afterAgentBalanceD = @"(select balance+subTotalAmount + unSettleAmount - {0} from ofAffiliate where account='{1}')";




        /// <summary>
        /// 查询指定时间到账总额
        /// </summary>
        public const string LoadDepositWeeklySql = @"select iif(len(sum(amount))>0,sum(amount),0) amoutWeekly from ofDeposit
where requesttime>=@startTime and requesttime<=@endTime and status=1 and account=@account";
    }
}
