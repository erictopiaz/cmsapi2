using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    /// <summary>
    /// 输出存款UI参数配置 sql
    /// </summary>
    public class PaySettingSql
    {
        /// <summary>
        /// pc和h5 获取所有开启的支付通道数据到缓存中
        /// </summary>
        public const string QueryActivedSql_old = @"select tp.id,tp.minamount,tp.maxamount,tp.misc,tp.method,tp.displayname name,tp.sort,tp.status,tp.appstatus,tp.levels,tp.fastpay,tp.paycategory,tp.banklist,kv.dkey,tp.queryMerchantWhere,kv.sortid sortid,kv.dvalue,kv.icon,kv.recommend,kv.navilogo,kv.title,kv.selectTitle from Ofthirdpay tp with(nolock)
left join ofKeyValues kv with(nolock) on tp.paycategory=kv.dkey
where tp.status=1 and kv.dtype=0 {0} order by tp.sort asc";
        /// <summary>
        /// 低级商户存款通道
        /// </summary>
        public const string QueryActivedSql = @"select tp.id,tp.minamount,tp.maxamount,tp.misc,tp.method,tp.displayname name,tp.sort,tp.status,tp.appstatus,tp.levels,tp.fastpay,tp.paycategory,tp.banklist,kv.dkey,tp.queryMerchantWhere,kv.sortid sortid,kv.dvalue,kv.icon,kv.recommend,kv.navilogo,kv.title,kv.selectTitle,tp.merchantids,tp.merchantLevel,tp.currencyType,kv.dvalues,tp.displaynames names,tp.t_walletType,tp.t_merchantAccounts from Ofthirdpay tp with(nolock)
left join ofKeyValues kv with(nolock) on tp.paycategory=kv.dkey
where tp.status=1 and kv.dtype=0 {0} order by tp.sort asc";


        /// <summary>
        /// 查询指定通道使用次数
        /// </summary>
        public const string QueryUsedMerchantTimesByIDsSql = @"select account,payplatform,count(id) cnt from ofDeposit with(nolock)
where account='{0}' and status=1 and payplatform in ({1})
group by account,payplatform ";
        /// <summary>
        /// 查询会员当日所以渠道各使用的次数
        /// </summary>
        public const string QueryUsedMerchantTimesSql = @"select account,payplatform,count(id) cnt from ofDeposit with(nolock)
where account='{0}' and status>=0 and DateDiff(dd,requesttime,DATEADD(hour,8,getutcdate())) =0
group by account,payplatform";

        /// <summary>
        /// app 获取所有开启的支付通道数据到缓存中
        /// </summary>
        public const string QueryAppActivedSql_old = @"select tp.id,tp.minamount,tp.maxamount,tp.misc,tp.method,tp.displayname name,tp.sort,tp.status,tp.appstatus,tp.levels,tp.fastpay,tp.paycategory,tp.banklist,tp.queryMerchantWhere,kv.dkey,kv.sortid sortid,kv.dvalue,kv.icon,kv.recommend,kv.navilogo,kv.title,kv.selectTitle from Ofthirdpay tp with(nolock)
left join ofKeyValues kv with(nolock) on tp.paycategory=kv.dkey
where tp.appstatus=1 and kv.dtype=0 {0} order by tp.sort asc";
        /// <summary>
        /// 品牌 2025-05-31
        /// </summary>
        public const string QueryAppActivedSql = @"select tp.id,tp.minamount,tp.maxamount,tp.misc,tp.method,tp.displayname name,tp.sort,tp.status,tp.appstatus,tp.levels,tp.fastpay,tp.paycategory,tp.banklist,tp.queryMerchantWhere,kv.dkey,kv.sortid sortid,kv.dvalue,kv.icon,kv.recommend,kv.navilogo,kv.title,kv.selectTitle,tp.merchantids,tp.merchantLevel,tp.currencyType,kv.dvalues,tp.displaynames names,tp.t_walletType,tp.t_merchantAccounts from Ofthirdpay tp with(nolock)
left join ofKeyValues kv with(nolock) on tp.paycategory=kv.dkey
where tp.appstatus=1 and kv.dtype=0 {0} order by tp.sort asc";


        /// <summary>
        /// 获取提款菜单到缓存中 offline=0上线的提款分类，1:包括测试的分类
        /// 2023-08-29 加货币区分
        /// 2025-05-31 品牌区分
        /// </summary>
        public const string QueryWithdrawMenusSql = @"select dkey id,dvalue, icon, navilogo,recommend,title,dvalues,sortid,kv_merchantAccounts as t_merchantAccounts from ofKeyValues with(nolock)
where dtype=1 and kv_currencycode={0} {1} 
order by sortid asc";

        /// <summary>
        /// 获取银行列表到缓存中
        /// 2023-08-15 新加货币区分
        /// </summary>
        public const string QueryOfBankBaseSqlV = @"select bid id,name,icon,url, category,withdrawType,status from ofBankBase with(nolock) where status >=0 and currencycode={0} ";


        /// <summary>
        /// 获取提款转换汇率
        /// </summary>
        public const string QueryOTCRateSql = @"SELECT  t1.usdtxrate rate, t1.withdrawType
	FROM  ofRMBXRate AS t1 RIGHT OUTER JOIN
                          (SELECT  withdrawType, MAX(create_at) AS C
                            FROM       ofRMBXRate
                            GROUP BY withdrawType) AS t2 ON t1.withdrawType = t2.withdrawType AND t1.create_at = t2.C ";
        /// <summary>
        /// 获取提款转换汇率 
        /// </summary>
        public const string QueryOTCRateVSql = @"select body from ofSettings with(nolock) where code='PAYMENT_CRYPTO' and status=1";

        /// <summary>
        /// 获取地区信息
        /// </summary>
        public const string QueryCountrySql = @"select co.phonecode,co.country,co.countryLang,co.countryLogo,cu.currencyname,cu.currencycode,cu.currencyLang,co.countrycode from ofCountry co with(nolock)
left join ofCurrency cu with(nolock) on co.cid = cu.areaId
where co.online=1 and co.merchantAccount='{0}' and co.countrycode='{1}' ";

        public const string QueryCountryByMerchantSql = @"select co.phonecode,co.country,co.countryLang,co.countryLogo,cu.currencyname,cu.currencycode,cu.currencyLang,co.countrycode from ofCountry co with(nolock)
left join ofCurrency cu with(nolock) on co.cid = cu.areaId
where co.online=1 and co.merchantAccount='{0}'";

        /// <summary>
        /// 极速提款时间区间优惠,每个时间段有一个优惠金额 
        /// </summary>
        public const string QueryFixWithdrawDescSql = @"select body from ofSettings with(nolock) where code='fix_withdraw_desc' and status=1";
        public const string QueryFixWithdrawDescVSql = @"select * from (
select account,misc from ofUser with(nolock) where account=@account) a ,(select body from ofSettings with(nolock) where code='fix_withdraw_desc' and status=1) b";

        /// <summary>
        /// 存款时，获取更新定额  和对应的优惠比例
        /// </summary>
        public const string QueryFixDepositDescSql = @"select body from ofSettings with(nolock) where code='fix_amount_and_promo' and status=1";

        /// <summary>
        /// 存款时，获取更新定额  和对应的优惠比例， 并有相关级别限制会员能使用多少的金额 
        /// 默认为level=1 , 只能看到10000以下的金额
        /// </summary>
        public const string QueryFixDepositLimitLevelDescSql = @"select a.fixAmountList,b.fixAmountLevels,c.fixAmountLimitLevel from (
select body fixAmountList from ofSettings with(nolock) where code='fix_amount_and_promo' and status=1) a
,(select body fixAmountLevels from ofsettings with(nolock)
where code='fix_amount_level') b,
(select iif((ue.fixAmountLimitLevel IS not NULL and LEN(RTRIM(LTRIM(ue.fixAmountLimitLevel))) != 0),ue.fixAmountLimitLevel,1) fixAmountLimitLevel from ofUser u with(nolock)
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account='{0}') c";

        /// <summary>
        /// 存款时，获取更新支付宝定额  和对应的优惠比例， 并有相关级别限制会员能使用多少的金额 
        /// 默认为level=1 , 只能看到10000以下的金额
        /// </summary>
        public const string QueryAlipayFixDepositLimitLevelDescSql = @"select a.fixAmountList,b.fixAmountLevels,c.fixAmountLimitLevel from (
select body fixAmountList from ofSettings with(nolock) where code='alipay_fix_amount_and_promo' and status=1) a
,(select body fixAmountLevels from ofsettings with(nolock)
where code='fix_amount_level') b,
(select iif((ue.fixAmountLimitLevel IS not NULL and LEN(RTRIM(LTRIM(ue.fixAmountLimitLevel))) != 0),ue.fixAmountLimitLevel,1) fixAmountLimitLevel from ofUser u with(nolock)
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account='{0}') c";

        /// <summary>
        /// 存款时，获取更新微信定额  和对应的优惠比例， 并有相关级别限制会员能使用多少的金额 
        /// 默认为level=1 , 只能看到10000以下的金额
        /// </summary>
        public const string QueryWXFixDepositLimitLevelDescSql = @"select a.fixAmountList,b.fixAmountLevels,c.fixAmountLimitLevel from (
select body fixAmountList from ofSettings with(nolock) where code='wx_fix_amount_and_promo' and status=1) a
,(select body fixAmountLevels from ofsettings with(nolock)
where code='fix_amount_level') b,
(select iif((ue.fixAmountLimitLevel IS not NULL and LEN(RTRIM(LTRIM(ue.fixAmountLimitLevel))) != 0),ue.fixAmountLimitLevel,1) fixAmountLimitLevel from ofUser u with(nolock)
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account='{0}') c";


        /// <summary>
        /// 存款时，获取更新定额  和对应的优惠比例， 并有相关级别限制会员能使用多少的金额 
        /// 默认为level=1 , 只能看到10000以下的金额
        /// 新版
        /// </summary>
        public const string QueryFixDepositLimitLevelDescSql_V = @"select a.*,b.fixAmountLevels,c.fixAmountLimitLevel,d_level,umisc from (
select w.account,w.realname,w.cardno,w.cardbank,w.cardaddress,w.id withdrawid, w.checkperson,w.amount,w.requesttime,u.level w_level,iif((u.misc IS not NULL and LEN(RTRIM(LTRIM(u.misc))) != 0),u.misc,'') misc from ofWithdrawal w with(nolock)
left join ofUser u with(nolock) on w.account = u.account
where w.wid=3 and w.currencycode={0} and w.status=1) a
,(select body fixAmountLevels from ofsettings with(nolock)
where code='fix_amount_level') b,
(select iif((ue.fixAmountLimitLevel IS not NULL and LEN(RTRIM(LTRIM(ue.fixAmountLimitLevel))) != 0),ue.fixAmountLimitLevel,1) fixAmountLimitLevel,u.level d_level, iif((u.misc IS not NULL and LEN(RTRIM(LTRIM(u.misc))) != 0),u.misc,'') umisc from ofUser u with(nolock)
left join ofUserExtention ue with(nolock) on u.account=ue.account
where u.account='{1}') c";

        public const string QueryThirdFixAmountListSql = @"select b.id,b.amounts,b.startTime,b.endTime,b.bankCardNo,b.limitMaxAmount,b.bankOwner,b.bankAddress,bc.bankName,b.availableAmounts,o.operatorId from operatorMapBanks o with(nolock)
left join banks b with(nolock) on b.bankCardNo=o.bankCardNo
left join bankCode bc with(nolock) on bc.bankId = b.bankId
where o.operatorId='awc9006' and b.startTime <= '{0}' and b.endTime > '{0}' and b.b_status=1";

        /// <summary>
        /// 接收定单 新版
        /// 2021-04-28 limitMaxAmount 新增记录产生定单时，当前定单对应收款卡最高可收款的金额
        /// </summary>
        public const string InsertOrderVSql = @"INSERT INTO [dbo].[ordersCreated]
           ([operatorId],[orderno],[awcOrderNo],[bankType],[bankId],[bankCardNo],[amount],[ip],[name],[sign],[remark],[vAmount],[vRate],[whoRate],[limitMaxAmount],[currencyType],[chainId])
     VALUES
           (@operatorId,@orderno,@awcOrderNo,@bankType,@bankId,@bankCardNo,@amount,@ipaddress,@name,@sign,@remark,@vAmount,@vRate,@whoRate,@limitMaxAmount,@currencyType,@chainId)";

        /// <summary>
        /// 更改商户可用金额
        /// </summary>
        public const string UpdateThirdFixAvailableAmountsSql = @"update banks set availableAmounts=@availableAmounts where id=@id";

        /// <summary>
        /// 获取通道手续费计算汇率
        /// </summary>
        public const string QueryMerchantFeeRateSql = @"select misc from ofThirdpay with(nolock) where id=@id and status=1";

        /// <summary>
        /// 确认客人在指定天数内是否有存款记录
        /// </summary>
        public const string QuerySuerAlreadyDepositSql = @"select top 1 id from ofDeposit with(nolock)
where account=@account and DateDiff(dd,requesttime,DATEADD(hour,8,getutcdate()))<=@days and status=1";


        #region 客人周存流水优惠申请
        /// <summary>
        /// 查询会员周存款
        /// </summary>
        public const string QueryTotalWeekAmountSql = @"select account,sum(amount) amount from ofDeposit a with(nolock)
 where requesttime >='{0}' and account='{2}' and ISNULL(orderendtime,reviewtime) >='{0}' and ISNULL(orderendtime,reviewtime)<'{1}' and status=1
 group by account ";

        /// <summary>
        /// 查询会员周投注
        /// </summary>
        public const string QueryTotalWeekStakeSql = @"select account,sum(totalbets) amount from ofTurnover a with(nolock)
 where account='{2}' and begindate >='{0}' and begindate<'{1}' and status=1
 group by account ";

        /// <summary>
        /// 查询客人是否有申请vip 周存权限
        /// </summary>
        public const string QueryAccessWeekDepositStakePromoSql = @"select top 1 * from ofVipWeekBonusAccounts with(nolock)
where account='{0}'";
        #endregion

        /// <summary>
        /// 过滤可使用的商户通道 存款次数使用
        /// 非usdt 商户 统计不包括usdt存款次数 最近180天
        /// </summary>
        public const string depositTimesNotIncludeUsdtSql = @"select count(d.id) cnt from ofDeposit d with(nolock)
left join ofThirdpay t with(nolock) on d.payplatform = t.id
where requesttime>='{0}' and account='{1}' and d.status=1 and t.paycategory not in (7)";

    }
}
