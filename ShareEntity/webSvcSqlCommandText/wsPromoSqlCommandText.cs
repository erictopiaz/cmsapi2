using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.webSvcSqlCommandText
{
    public class wsPromoSqlCommandText
    {
        /// <summary>
        /// 当天0点到客人提交请求时，客人的已经到账的存款金额
        /// </summary>
        public const string DepositAmountCurrentDate = @"select sum(amount) amount
FROM  ofDeposit 
where requesttime>@before and account=@account and  ISNULL(orderendtime,reviewtime) >=@startTime and ISNULL(orderendtime,reviewtime)<@endTime and status=1 ";

        /// <summary>
        /// 抽奖获取到的实物
        /// </summary>
        public const string GoodsToFreeAmount = @"INSERT INTO [dbo].[Goods]
           ([account],[goodsId],[goodsName],[goodsPrice],[refid],[isGoods])
     VALUES
           (@account,@goodsId,@goodsName,@goodsPrice,@refid,@isGoods) ";
        /// <summary>
        /// 新增优惠或红利
        /// </summary>
        public const string InsertPromoSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[referrerreg],[referenceguid],[ipaddress],[macaddress],[status],[reviewtime],[reviewperson],[bonusPromoCode])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@referrerreg,@referenceguid,@ipaddress,@macaddress,@status,@reviewtime,@reviewperson,@bonusPromoCode)";
        /// <summary>
        /// 当天中奖-获取到的实物
        /// </summary>
        public const string GoodsListSql = @"
select gd.account,gd.goodsId,gd.goodsName,gd.goodsPrice,gd.refid,gd.isGoods from ofBonusPromo bp,Goods gd
where  bp.createtime >='2019-12-01' and bp.referenceguid = gd.refid and bp.status=1 and DateDiff(dd,bp.reviewtime,DATEADD(hour,8,getutcdate()))=0 and bp.bonusPromoType=0 and bp.bonusPromoCode =1
";
        /// <summary>
        /// 活动期间-获取到所有的实物
        /// </summary>
        public const string GoodsListPromoDateRanageSql = @"
select gd.account,gd.goodsId,gd.goodsName,gd.goodsPrice,gd.refid,gd.isGoods from ofBonusPromo bp,Goods gd
where  bp.createtime >=@startTime and bp.createtime <=@endTime and bp.referenceguid = gd.refid and bp.status=1 and gd.account=@account and gd.isGoods=1 and gd.allowedTimes=@allowedTimes and bp.bonusPromoType=0 and bp.bonusPromoCode=1
";
        /// <summary>
        /// 查询当前客人存提情况
        /// 方便抽取红包
        /// </summary>
        public const string RedWalletAmountCheckSql = @"select damount,wamount,(damount-wamount) dwamount from(
select sum(amount) damount from ofDeposit a
 where requesttime >'2018-12-30' and ISNULL(orderendtime,reviewtime) >='2019-01-01' and ISNULL(orderendtime,reviewtime)<=@currentTime and status=1 and account =@account
 ) a,
 (
select sum(amount) wamount from ofwithdrawal a
 where requesttime >'2018-12-30' and a.paytime >='2019-01-01' and a.paytime <=@currentTime and status=2 and account =@account
 ) b";
        /// <summary>
        /// 当天中奖-获取到的实物
        /// </summary>
        public const string CheckRedWalletTimesSql = @"
select * from ofBonusPromo bp
where  bp.createtime >='2019-12-01' and bp.status=1 and DateDiff(dd,bp.reviewtime,DATEADD(hour,8,getutcdate()))=0 and bp.bonusPromoType=0 and bp.bonusPromoCode =2 and bp.account =@account
";
        /// <summary>
        /// 理新当前客人余额
        /// </summary>
        public const string UpdateUserAmountSql = @"update ofUser set amountactive = amountactive + @amount where account=@account ";
    }
}
