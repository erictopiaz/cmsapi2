namespace ShareEntity.SqlCommandText
{
    public class BonusSqlCommandText
    {
        /// <summary>
        /// 更新当前红利优惠状态
        /// </summary>
        public const string UpdateBonusStatusSql = @"update ofBonusPromo set status=@status , reviewperson=@reviewperson,reason=@reason,reviewtime=dateadd(hour,(8),getutcdate()) where status=0 and id=@id";
        /// <summary>
        /// 更新当前红利优惠状态 自定义时间
        /// </summary>
        public const string UpdateBonusStatusSql_with_Datetime = @"update ofBonusPromo set status=@status , reviewperson=@reviewperson,reason=@reason,reviewtime=@reviewtime where status=0 and id=@id";

        public const string LoadBonusModelSql = @"select a.id, a.account,b.affaccount,a.amount,a.reward,a.bet,a.remarks,a.createtime,a.referenceguid,a.bonusPromoType,a.gametype,a.bonusPromoCode,b.misc, b.currencyType,b.merchantAccount from ofBonusPromo a left join ofUser b on a.account=b.account where a.id=@aid";

        #region 福利中心
        /// <summary>
        /// 福利中心 记录查询 by id
        /// </summary>
        public const string LoadSelfBonusModelSql = @"select a.id, a.account,b.affaccount,a.amount,a.reward,a.bet,a.remarks,a.createtime,a.expiredTime,a.referenceguid,a.bonusPromoType,a.gametype,a.bonusPromoCode,b.misc, b.currencyType,b.merchantAccount from ofBonusPromoSelf a left join ofUser b on a.account=b.account where a.id=@aid";
        /// <summary>
        /// 福利心项目选择列表
        /// </summary>
        public const string SelfBonusList = @"select body from ofSettings with(nolock) where code='self_bonus_key_values_dict'";
        /// <summary>
        /// 福利中心 取消
        /// </summary>
        public const string CancelSelfBonusStatusSql = @"update ofBonusPromoSelf set status=@status,reviewperson=@reviewperson,reason=@reason,reviewtime=dateadd(hour,(8),getutcdate()) where status in(0,4) and id=@id";
        /// <summary>
        /// 福利中心 审核 审核后才可以领取
        /// </summary>
        public const string VerifySelfBonusStatusSql = @"update ofBonusPromoSelf set status=@status,reviewperson=@reviewperson,verifyTime=dateadd(hour,(8),getutcdate()) where id=@id and status = 4 ";
        /// <summary>
        /// 福利中心 审核 审核后才可以领取 新版
        /// </summary>
        public const string VerifySelfBonusStatusSqlV = @"update ofBonusPromoSelf set status=0,reviewperson='{0}',verifyTime=dateadd(hour,(8),getutcdate()) where id={1} and status = 4 ";

        #endregion

        /// <summary>
        /// 会员日财务报表
        /// </summary>
        public const string UpdatePlayerDailyReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerDailyReportFinance WHERE financeDay = @dd and account=@account ))
begin
INSERT INTO [dbo].[ofPlayerDailyReportFinance]
           ([account],[affaccount],[financeDay],[bonus],[promo])
     VALUES(@account,@affaccount,@dd,@bonus,@promo)
end
else
begin
update ofPlayerDailyReportFinance set bonus=bonus+@bonus,promo=promo+@promo where financeDay=@dd and account=@account
end";
        /// <summary>
        /// 会员月财务报表
        /// </summary>
        public const string UpdatePlayerMonthReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerMonthReportFinance WHERE financeMonth = @MM and account=@account))
begin
INSERT INTO [dbo].[ofPlayerMonthReportFinance]
           ([account],[affaccount],[financeMonth],[bonus],[promo])
     VALUES(@account,@affaccount,@MM,@bonus,@promo)
end
else
begin
update ofPlayerMonthReportFinance set bonus=bonus+@bonus,promo=promo+@promo where financeMonth=@MM and account=@account
end";
        /// <summary>
        /// 代理日财务报表
        /// </summary>
        public const string UpdateAffDailyReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofAffDailyReportFinance WHERE financeDay = @dd and account=@affaccount))
begin
INSERT INTO [dbo].[ofAffDailyReportFinance]
           ([account],[financeDay],[bonus],[promo])
     VALUES(@affaccount,@dd,@bonus,@promo)
end
else
begin
update ofAffDailyReportFinance set bonus= bonus + @bonus,promo=promo+@promo where financeDay=@dd and account=@affaccount
end";
        /// <summary>
        /// 代理月财务报表
        /// </summary>
        public const string UpdateAffMonthReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofAffMonthReportFinance WHERE financeMonth = @MM and account=@affaccount))
begin
INSERT INTO [dbo].[ofAffMonthReportFinance]
           ([account],[financeMonth],[bonus],[promo])
     VALUES(@affaccount,@MM,@bonus,@promo)
end
else
begin
update ofAffMonthReportFinance set bonus=bonus+@bonus,promo=promo+@promo where financeMonth=@MM and account=@affaccount
end";


        #region  推荐优惠
        /// <summary>
        /// 推荐优惠结算状态，更新
        /// </summary>
        public const string UpdateSpreadBilledStatusSql = @"update promoSpreadMonthBilled set status=@status, amount=@amount , reviewperson=@reviewperson,reason=@reason,update_time=dateadd(hour,(8),getutcdate()) where status=0 and id=@id";
        /// <summary>
        /// 获取结算记录
        /// </summary>
        public const string LoadSpreadBilledModelSql = @"select a.id, a.account, a.amount,a.spreadSettleAmount,a.financegameday,b.misc, b.affaccount
	from promoSpreadMonthBilled a left join ofUser b on a.account=b.account where a.id=@id";


        /// <summary>
        /// 新增优惠或红利
        /// 并设置状态和电销账号
        /// </summary>
        public const string CreatePromoAndUpdateWithSalerSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[status],[createtime],[reviewtime],[autopromotype],[platformType],[saler],[aaffaccount])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@status,@createTime,DATEADD(hour,8,getutcdate()),@autopromotype,@platformType,@saler,@aaffaccount)";


        #endregion
    }
}
