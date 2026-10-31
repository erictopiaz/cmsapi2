using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    /// <summary>
    /// 优惠操作sql
    /// </summary>
    public class PromoSqlCommandText
    {

        /// <summary>
        /// 新增优惠或红利
        /// 并设置状态和电销账号
        /// 20210329 增加设备号，20220104加isAutoPromo 
        /// 20241206 带入审核时间 （之前版本 CreatePromoAndUpdateWithSalerWithReviewTimeSql_old_old）  
        /// 20250822 带会会员id
        /// </summary>
        public const string CreatePromoAndUpdateWithSalerWithReviewTimeSql_V = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[status],[createtime],[reviewtime],[autopromotype],[platformType],[saler],[aaffaccount],[device_id],[isAutoPromo],[p_f_uid])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@status,@createTime,@reviewtime,@autopromotype,@platformType,@saler,@aaffaccount,@device_id,@isAutoPromo,@p_f_uid)";


        /// <summary>
        /// 新增优惠或红利
        /// </summary>
        public const string CreatePromoSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress)";

        public const string CreatePromoNewSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[autopromotype],[platformType],[accountType])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@autopromotype,@platformType,@accountType)";
        /// <summary>
        /// 新增优惠或红利
        /// 电销人员
        /// </summary>
        public const string CreatePromoWithSalerSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[autopromotype],[platformType],[accountType],[saler],[aaffaccount])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@autopromotype,@platformType,@accountType,@saler,@aaffaccount)";

        /// <summary>
        /// 新增优惠或红利  加多会员id
        /// 电销人员
        /// </summary>
        public const string CreatePromoWithSalerSql_V = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[autopromotype],[platformType],[accountType],[saler],[aaffaccount],[p_f_uid])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@autopromotype,@platformType,@accountType,@saler,@aaffaccount,@p_f_uid)";


        /// <summary>
        /// 福利中心 新增记录
        /// </summary>
        public const string CreateSelfPromoWithSalerSql_bak = @"INSERT INTO [dbo].[ofBonusPromoSelf]
           ([currency],[account],[amount],[reward],[createtime],[expiredTime],[status],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[autopromotype],[platformType],[accountType],[saler],[aaffaccount])
     VALUES (@currency,@account,@amount,@reward,@createtime,@expiredTime,@status,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@autopromotype,@platformType,@accountType,@saler,@aaffaccount)";
        /// <summary>
        /// 福利中心 新增记录 v
        /// </summary>
        public const string CreateSelfPromoWithSalerSql_V = @"INSERT INTO [dbo].[ofBonusPromoSelf]
           ([currency],[account],[amount],[reward],[createtime],[expiredTime],[status],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[autopromotype],[platformType],[accountType],[saler],[aaffaccount],[ps_f_uid])
     VALUES (@currency,@account,@amount,@reward,@createtime,@expiredTime,@status,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@autopromotype,@platformType,@accountType,@saler,@aaffaccount,@ps_f_uid)";

        /// <summary>
        /// 福利中心(查询记录是否存在)
        /// </summary>
        public const string QuerySelfPromoSql_V = @"select top 1 id from ofBonusPromoSelf with(nolock)
where referenceguid='{0}' and bonusPromoCode=4";

        /// <summary>
        /// 新增优惠或红利
        /// 并设置状态
        /// </summary>
        public const string CreatePromoAndUpdateSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[status],[createtime],[reviewtime])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@status,@createTime,DATEADD(hour,8,getutcdate()))";
        /// <summary>
        /// 新增优惠或红利
        /// 并设置状态和电销人员
        /// </summary>
        public const string CreatePromoAndUpdateWithSalerSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[gamePercentType],[bonusPromoType],[bonusPromoCode],[referrerreg],[referenceguid],[ipaddress],[macaddress],[status],[createtime],[reviewtime],[saler],[aaffaccount])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@gamePercentType,@bonusPromoType,@bonusPromoCode,@referrerreg,@referenceguid,@ipaddress,@macaddress,@status,@createTime,DATEADD(hour,8,getutcdate()),@saler,@aaffaccount)";

        /// <summary>
        /// 更新优惠或红利
        /// </summary>
        public const string UpdatePromoSql = @"UPDATE [dbo].[ofBonusPromo] 
           set [account]=@account,[amount]=@amount,[reward]=@reward,[remarks]=@remarks,[level]=@level,[bet]=@bet,[createperson]=@createperson,[gametype]=@gametype,[gamePercentType]=@gamePercentType,
[bonusPromoType]=@bonusPromoType,[referrerreg]=@referrerreg,[ipaddress]=@ipaddress,[macaddress]=@macaddress where [id] = @tid and status=0";

        public const string UpdatePromoNewSql = @"UPDATE [dbo].[ofBonusPromo] 
           set [account]=@account,[amount]=@amount,[reward]=@reward,[remarks]=@remarks,[level]=@level,[bet]=@bet,[createperson]=@createperson,[gametype]=@gametype,[gamePercentType]=@gamePercentType,[autopromotype]=@autopromotype,[platformType]=@platformType,
[bonusPromoType]=@bonusPromoType,[referrerreg]=@referrerreg,[ipaddress]=@ipaddress,[macaddress]=@macaddress where [id] = @tid and status=0";

        public const string UpdateSelfPromoNewSql = @"UPDATE [dbo].[ofBonusPromoSelf] 
           set [account]=@account,[amount]=@amount,[reward]=@reward,[createtime]=@createtime,[expiredTime]=@expiredTime,[remarks]=@remarks,[level]=@level,[bet]=@bet,[createperson]=@createperson,[gametype]=@gametype,[gamePercentType]=@gamePercentType,[autopromotype]=@autopromotype,[platformType]=@platformType,
[bonusPromoType]=@bonusPromoType,[referrerreg]=@referrerreg,[ipaddress]=@ipaddress,[macaddress]=@macaddress where [id] = @tid and status=4";


        /// <summary>
        /// 创建优惠活动的内容
        /// 方便前台页面去读取更新的内容
        /// </summary>
        public const string CreatePromoContentSql_bak = @"INSERT INTO [dbo].[ofPromotionContent]
           ([title],[titleSec],[promoDetail],[promoMember],[promoGames],[bannerStatus],[picUrl],[starttime],[endtime],[status],[body],[createcustomer],[howApplication],[picBanner],[picPromo],[promotype],[autopromotype],[promoCatalog],[psort],[bsort],[cstatus],[device],[startTimeDesc],[promobgPic],[isDisplayPromoContent],[gametype],[bannerPromoPath],[promoPos],[bannerPromoType],[currencyType])
     VALUES(@title,@titleSec,@promoDetail,@promoMember,@promoGames,@bannerStatus,@picUrl,@starttime,@endtime,@status,@body,@createcustomer,@howApplication,@picBanner,@picPromo,@promotype,@autopromotype,@promoCatalog,@psort,@bsort,@cstatus,@device,@startTimeDesc,@promobgPic,@isDisplayPromoContent,@gametype,@bannerPromoPath,@promoPos,@bannerPromoType,@currencyType)";

        /// <summary>
        /// 创建优惠活动的内容
        /// 方便前台页面去读取更新的内容
        /// 品牌 2025-05-31
        /// </summary>
        public const string CreatePromoContentSql_V = @"INSERT INTO [dbo].[ofPromotionContent]
           ([title],[titleSec],[promoDetail],[promoMember],[promoGames],[bannerStatus],[picUrl],[starttime],[endtime],[status],[body],[createcustomer],[howApplication],[picBanner],[picPromo],[promotype],[autopromotype],[promoCatalog],[psort],[bsort],[cstatus],[device],[startTimeDesc],[promobgPic],[isDisplayPromoContent],[gametype],[bannerPromoPath],[promoPos],[bannerPromoType],[currencyType],[p_walletType],[p_merchantAccounts])
     VALUES(@title,@titleSec,@promoDetail,@promoMember,@promoGames,@bannerStatus,@picUrl,@starttime,@endtime,@status,@body,@createcustomer,@howApplication,@picBanner,@picPromo,@promotype,@autopromotype,@promoCatalog,@psort,@bsort,@cstatus,@device,@startTimeDesc,@promobgPic,@isDisplayPromoContent,@gametype,@bannerPromoPath,@promoPos,@bannerPromoType,@currencyType,@p_walletType,@p_merchantAccounts)";

        /// <summary>
        /// 更新优惠活动的内容
        /// </summary>
        public const string UpdatePromoContentSql_bak = @"UPDATE [dbo].[ofPromotionContent]
   SET [title] = @title,[titleSec] = @titleSec,[promoDetail] = @promoDetail,[promoMember] = @promoMember,[promoGames] = @promoGames,[bannerStatus] = @bannerStatus,[picUrl] = @picUrl,[starttime] = @starttime,[endtime] = @endtime,[status] = @status,[body] = @body,[createcustomer] = @createcustomer,
[howApplication]=@howApplication,[picBanner]=@picBanner,[picPromo]=@picPromo,[promotype]=@promotype,[autopromotype]=@autopromotype,[promoCatalog]=@promoCatalog,[psort]=@psort,[bsort]=@bsort,[cstatus]=@cstatus,[device]=@device,[startTimeDesc]=@startTimeDesc,[promobgPic]=@promobgPic,[isDisplayPromoContent]=@isDisplayPromoContent,
[gametype]=@gametype,[bannerPromoPath]=@bannerPromoPath,[promoPos]=@promoPos,[bannerPromoType]=@bannerPromoType,[vips]=@vips,[currencyType]=@currencyType
 WHERE id=@id";

        /// <summary>
        /// 更新优惠活动的内容
        /// 品牌 2025-05-31
        /// </summary>
        public const string UpdatePromoContentSql_V = @"UPDATE [dbo].[ofPromotionContent]
   SET [title] = @title,[titleSec] = @titleSec,[promoDetail] = @promoDetail,[promoMember] = @promoMember,[promoGames] = @promoGames,[bannerStatus] = @bannerStatus,[picUrl] = @picUrl,[starttime] = @starttime,[endtime] = @endtime,[status] = @status,[body] = @body,[createcustomer] = @createcustomer,
[howApplication]=@howApplication,[picBanner]=@picBanner,[picPromo]=@picPromo,[promotype]=@promotype,[autopromotype]=@autopromotype,[promoCatalog]=@promoCatalog,[psort]=@psort,[bsort]=@bsort,[cstatus]=@cstatus,[device]=@device,[startTimeDesc]=@startTimeDesc,[promobgPic]=@promobgPic,[isDisplayPromoContent]=@isDisplayPromoContent,
[gametype]=@gametype,[bannerPromoPath]=@bannerPromoPath,[promoPos]=@promoPos,[bannerPromoType]=@bannerPromoType,[vips]=@vips,[currencyType]=@currencyType,[p_walletType]=@p_walletType,[p_merchantAccounts]=@p_merchantAccounts
 WHERE id=@id";

        /// <summary>
        /// 读取指定优惠内容
        /// </summary>
        public const string LoadPromoContentByIdSql = @"select * from ofPromotionContent where id= @tid";

        /// <summary>
        /// 创建微调
        /// </summary>
        public const string CreateResizeSql_bak = @"INSERT INTO [dbo].[ofMinus]
           ([currency],[account],[amount],[createperson],[remarks],[referenceguid],[decreasetype],[referrer],[referrerreg],[appendType],[orderNo],[accountType],[amountType])
     VALUES (@currency,@account,@amount,@createperson,@remarks,@referenceguid,@decreasetype,@referrer,(select referrerreg from ofUser where account=@account),@appendType,@orderNo,@accountType,@amountType)";

        /// <summary>
        /// 创建微调
        /// </summary>
        public const string CreateResizeSql_V = @"INSERT INTO [dbo].[ofMinus]
           ([currency],[account],[amount],[createperson],[remarks],[referenceguid],[decreasetype],[referrer],[referrerreg],[appendType],[orderNo],[accountType],[amountType],[r_f_uid])
     VALUES (@currency,@account,@amount,@createperson,@remarks,@referenceguid,@decreasetype,@referrer,(select referrerreg from ofUser where account=@account),@appendType,@orderNo,@accountType,@amountType,@r_f_uid)";



        /// <summary>
        /// 创建代理微调
        /// 20211130 添加 bonusType 0:代理奖励，默认为-1
        /// </summary>
        public const string CreateAgentResizeSql = @"INSERT INTO [dbo].[ofMinus]
           ([currency],[account],[amount],[createperson],[remarks],[referenceguid],[decreasetype],[appendType],[orderNo],[accountType],[amountType],[bonusType])
     VALUES (@currency,@account,@amount,@createperson,@remarks,@referenceguid,@decreasetype,@appendType,@orderNo,@accountType,@amountType,@bonusType)";

        /// <summary>
        /// 冻结客人的余额
        /// </summary>
        public const string SuppendUserAmountSql = @"update ofUser set amountactive=amountactive-@amount,amountsuspend=amountsuspend+@amount
where account=@account and amountactive-@amount >=0 ";
        /// <summary>
        /// 冻结代理的可用余额
        /// </summary>
        public const string SuppendAgentAmountSql = @"update ofAffiliate set balance=balance-@amount
where account=@account and balance-@amount >=0 ";
        /// <summary>
        /// 冻结代理的累积余额
        /// </summary>
        public const string SuppendAgentUnSettleAmountSql = @"update ofAffiliate set unSettleAmount=unSettleAmount-@amount
where account=@account ";
        /// <summary>
        /// 分配金额给客人的余额
        /// </summary>
        public const string AddUserAmountSql = @"update ofUser set amountactive=amountactive+@amount,amountsuspend=amountsuspend-@amount
where account=@account and amountsuspend-@amount >=0 ";

       

        /// <summary>
        /// 审核通过后，去掉冻结的金额
        /// </summary>
        public const string SubSuppendAmountSql = @"update ofUser set amountsuspend=amountsuspend-@amount
where account=@account and amountsuspend-@amount >=0 ";
        /// <summary>
        /// 获取当前微调信息 +品牌
        /// </summary>
        public const string LoadResizeByIdSql = @"select m.*,u.merchantAccount,u.currencyType from ofMinus m with(nolock) left join ofUser u with(nolock) on m.account=u.account where m.id= @tid and m.status=0 ";
        /// <summary>
        /// 审核通或取消
        /// </summary>
        public const string UpdateResizeByIdSql = @"Update ofMinus set status=@status,reviewtime=dateadd(hour,(8),getutcdate()),reviewperson=@reviewperson, cancelreason=@cancelreason where status = 0 and id= @tid";
        /// <summary>
        /// 审核通或取消 自定义时间
        /// </summary>
        public const string UpdateResizeByIdSql_with_datetime = @"Update ofMinus set status=@status,reviewtime=@reviewtime,reviewperson=@reviewperson, cancelreason=@cancelreason where status = 0 and id= @tid";


        /// <summary>
        /// 创建积分
        /// </summary>
        public const string CreatePromoMiscSql_old = @"INSERT INTO [dbo].[ofPromoMisc]
           ([account],[type],[content],[createtime],[points],[status],[createperson],[rowstatus],[left_points])
     VALUES(@account,@type,@content,@createtime,@points,@status,@createperson,@rowstatus,{0})";

        /// <summary>
        /// 创建积分 + uid
        /// </summary>
        public const string CreatePromoMiscSql_V = @"INSERT INTO [dbo].[ofPromoMisc]
           ([account],[type],[content],[createtime],[points],[status],[createperson],[rowstatus],[left_points],[pm_f_uid])
     VALUES(@account,@type,@content,@createtime,@points,@status,@createperson,@rowstatus,@left_points,@pm_f_uid)";

        /// <summary>
        /// 创建积分,使用定单号
        /// </summary>
        public const string CreatePromoMiscWithOrderNoSql = @"INSERT INTO [dbo].[ofPromoMisc]
           ([account],[type],[content],[createtime],[points],[status],[createperson],[rowstatus],[left_points],[orderno])
     VALUES(@account,@type,@content,@createtime,@points,@status,@createperson,@rowstatus,{0},@orderno)";
        /// <summary>
        /// 检测是否申请过积分
        /// </summary>
        public const string CheckPromoMiscSql = @"select top 1 id from ofPromoMisc
where account='{0}' and type={1} and content like '%{2}%' ";
        /// <summary>
        /// 检测本月
        /// 是否有申请手机验证金额
        /// </summary>
        public const string CheckPhoneVerifyRewardSql = @"select id from ofBonusPromo 
where account='{0}' and status=1 and remarks like '{1}%' ";

        /// <summary>
        /// 加载积分信息
        /// </summary>
        public const string LoadPromoMiscByIdSql = @"select * from ofPromoMisc where id= @tid";
        public const string UpdatePromoMiscByIdSql = @"Update ofPromoMisc set rowstatus=@rowstatus,reviewperson=@reviewperson ,left_points = (select bonuspoints + {0} from ofUser where account=@account) where rowstatus = 0 and id= @tid";

        /// <summary>
        /// 获取当前发放返水比例的所有客人
        /// </summary>
        public const string LoadUserTurnOverPercentByAccountSql = @"select *, a.misc pmisc from ofUser a left join ofUserGroup b on a.level=b.levelnumber where a.account in {0}";
        /// <summary>
        ///  获取当前发放返水比例的所有客人
        ///  并确认当前账号是否分配置给电销
        /// </summary>
        public const string LoadUserTurnOverPercentByAccountWithSalerSql_old = @"select *, a.misc pmisc,uu.createperson saler from ofUser a 
left join ofUserGroup b on a.level=b.levelnumber 
left join 
(
	select aa.account,b.createtime,b.createperson from
	(select distinct account from ofcalllog) aa
	cross apply
	(select top(1) * from ofcalllog aaa where aaa.account = aa.account  order by createtime desc) b
    where createtime>='2021-01-17'
) uu on uu.account=a.account
where a.account in {0} ";

        public const string LoadUserTurnOverPercentByAccountWithSalerSql = @"select *, a.id userid, a.misc pmisc,uu.createperson saler,ue.betCodeDetail,iif(ue.b_status is not null,ue.b_status,-1) b_status,b.misc_inr from ofUser a with(nolock)
left join ofUserGroup b with(nolock) on a.level=b.levelnumber 
left join ofUserExtention ue with(nolock) on a.account=ue.account
left join 
(
	select aa.account,b.createtime,b.createperson from
	(select distinct account from ofcalllog) aa
	cross apply
	(select top(1) * from ofcalllog aaa where aaa.account = aa.account  order by createtime desc) b
    where createtime>='2021-01-17'
) uu on uu.account=a.account
where a.account in {0} ";


        /// <summary>
        /// 获取指定平台游戏的反水比例
        /// </summary>
        public const string LoadUserTurnOverPercentByGameSql = @"select *, a.misc pmisc from ofUser a left join ofUserGroup b on a.level=b.levelnumber left join ofUserLevelMapTurnover c on a.level = c.userLevel where a.account in {0}";
        /// <summary>
        /// 获取指定平台游戏的反水比例
        /// 并确认当前客人是否分配给电销
        /// </summary>
        public const string LoadUserTurnOverPercentByGameWithSalerSql_old = @"select *, a.misc pmisc,uu.createperson saler from ofUser a 
left join ofUserGroup b on a.level=b.levelnumber 
left join ofUserLevelMapTurnover c on a.level = c.userLevel 
left join 
(
	select aa.account,bb.createtime,bb.createperson from
	(select distinct account from ofcalllog) aa
	cross apply
	(select top(1) * from ofcalllog aaa where aaa.account = aa.account  order by createtime desc) bb
where createtime>='2021-01-17'
) uu on uu.account=a.account
where a.account in {0} ";

        public const string LoadUserTurnOverPercentByGameWithSalerSql = @"select *,a.id userid, a.misc pmisc,uu.createperson saler,ue.betCodeDetail,iif(ue.b_status is not null,ue.b_status,-1) b_status,b.misc_inr from ofUser a 
left join ofUserGroup b on a.level=b.levelnumber 
left join ofUserLevelMapTurnover c on a.level = c.userLevel 
left join ofUserExtention ue with(nolock) on a.account=ue.account
left join 
(
	select aa.account,bb.createtime,bb.createperson from
	(select distinct account from ofcalllog) aa
	cross apply
	(select top(1) * from ofcalllog aaa where aaa.account = aa.account  order by createtime desc) bb
where createtime>='2021-01-17'
) uu on uu.account=a.account
where a.account in {0} ";



        public const string LoadBBinUserTurnOverPercentByAccountSql = @"select * from ofUser a with(nolock) left join ofUserGroup b with(nolock) on a.level=b.levelnumber where a.bbinaccount in {0}";
        public const string LoadBBinUserTurnOverPercentByAccountVSql = @"select *,a.id userid from ofUser a with(nolock) left join ofUserGroup b with(nolock) on a.level=b.levelnumber where a.id in {0}";
        public const string LoadPtUserTurnOverPercentByAccountSql = @"select * from ofUser a left join ofUserGroup b on a.level=b.levelnumber where a.account in {0}";

        /// <summary>
        /// 返水列表查询返水MODEL 计算统计数据
        /// 2025-08-22  添加品牌
        /// </summary>
        public const string LoadUserTurnOverModelByIdSql = @"select b.account,a.currencyType currencycode,a.merchantAccount,a.affaccount,c.gamename2 as gamename,b.gameType,b.reward,b.effectivebets,b.winlost,b.begindate,b.referenceguid,b.bonusPoints,b.id as tid,b.platformGame,a.misc pmisc 
from ofTurnOver b 
left join ofUser a on b.account = a.account 
left join ofGameClass c on b.gameType = c.gameid
where b.status=0 and b.id in {0}";





       /// <summary>
       /// vip 月升级数据集
       /// </summary>
        public const string LoadVipMonthModelByIdSql = @"select b.id, b.level,b.nextLevel,b.is_up,b.account,a.referrerreg
from lastMonthVipData b 
left join ofUser a on b.account = a.account 
where b.status=0 and b.level=a.level and b.id in {0}";

        /// <summary>
        /// vip 月降级级数据集
        /// </summary>
        public const string LoadVipDownMonthModelByIdSql = @"select b.id, b.level,b.nextLevel,b.is_up,b.account,a.referrerreg
from lastMonthVipDownData b 
left join ofUser a on b.account = a.account 
where b.status=0 and b.level=a.level and b.id in {0}";


        /// <summary>
        /// vip 月升级数据  取消
        /// </summary>
        public const string UpdateVipUpLevelStatusCancel = @"update lastMonthVipData set status=@status, remarks=@cancelperson,verify_who=@cancelreason,verify_at=@canceltime where status=0 and id in {0}";
        /// <summary>
        /// vip 月降级数据  取消
        /// </summary>
        public const string UpdateVipDownLevelStatusCancel = @"update lastMonthVipDownData set status=@status, remarks=@cancelperson,verify_who=@cancelreason,verify_at=@canceltime where status=0 and id in {0}";


        /// <summary>
        /// 更新vip 月升级数据状态
        /// </summary>
        public const string UpdateVipMonthStatus = @"update lastMonthVipData set status=@status, verify_who=@reviewperson,verify_at=@reviewtime where status=0 and id in {0}";
        /// <summary>
        /// 更新vip 月降级数据状态
        /// </summary>
        public const string UpdateVipDownMonthStatus = @"update lastMonthVipDownData set status=@status, verify_who=@reviewperson,verify_at=@reviewtime where status=0 and id in {0}";


        /// <summary>
        /// 更新vip 月升级优惠记录
        /// </summary>
        public const string CreateVipMonthPromoSql = @"INSERT INTO [dbo].[ofBonusPromo]
           ([currency],[account],[amount],[reward],[remarks],[level],[bet],[createperson],[gametype],[bonusPromoType],[status],[referrerreg],[referenceguid],[ipaddress],[reviewtime])
     VALUES (@currency,@account,@amount,@reward,@remarks,@level,@bet,@createperson,@gametype,@bonusPromoType,@status,@referrerreg,@referenceguid,@ipaddress,@reviewtime)";


        /// <summary>
        /// 获取当前已经发放,客人返水金额
        /// 已经审核和未审核过的客人
        /// </summary>
        public const string LoadTotalRewardsGroupByAccountSql = @"select account,sum(reward) as rewards from ofTurnOver WITH (NOLOCK) where (status=1 or status=0) and gametype=@gameType and DateDiff(dd,requesttime,DATEADD(hour,8,getutcdate()))=0 group by account";
        
        public const string LoadTotalRewardsGroupByAccountSql_new = @"select account,sum(reward) as rewards from ofTurnOver WITH (NOLOCK) 
where status=1 and gametype=@gameType and platformGame=@platformGame  and DateDiff(dd,requesttime,DATEADD(hour,8,getutcdate()))=0 
group by account
union
select account,sum(reward) as rewards from ofTurnOver WITH (NOLOCK) 
where status=0 and gametype=@gameType and platformGame=@platformGame and DateDiff(dd,requesttime,DATEADD(hour,8,getutcdate()))=0
group by account ";
        /// <summary>
        /// 新增返水时，创建记录
        /// </summary>
        public const string OfTurnoverInsertSql = @"INSERT INTO [dbo].[ofTurnover]
           ([currency],[account],[totalbets],[effectivebets],[reward],[winlost] ,[leftbets] ,[weekmaxbets] ,[begindate]
           ,[enddate],[requesttime],[gametype],[guid],[referrer],[referrerreg],[level],[referenceguid],[calc],[platformGame],[bonusPoints],[pCalc])
     VALUES
           (@currency,@account,@totalbets,@effectivebets,@reward,@winlost,@leftbets ,@weekmaxbets,@begindate
           ,@enddate ,@requesttime,@gametype,@guid,@referrer,@referrerreg,@level,@referenceguid,@calc,@platformGame,@bonusPoints,@pCalc)";
        /// <summary>
        /// 新增返水时，创建记录
        /// 并确认是否分配置到指定电销人员
        /// </summary>
        public const string OfTurnoverInsertWithSalerSql_bak = @"INSERT INTO [dbo].[ofTurnover]
           ([currency],[account],[totalbets],[effectivebets],[reward],[winlost] ,[leftbets] ,[weekmaxbets] ,[begindate]
           ,[enddate],[requesttime],[gametype],[guid],[referrer],[referrerreg],[level],[referenceguid],[calc],[platformGame],[bonusPoints],[pCalc],[saler],[aaffaccount],[eCalc],[cmBet])
     VALUES
           (@currency,@account,@totalbets,@effectivebets,@reward,@winlost,@leftbets ,@weekmaxbets,@begindate
           ,@enddate ,@requesttime,@gametype,@guid,@referrer,@referrerreg,@level,@referenceguid,@calc,@platformGame,@bonusPoints,@pCalc,@saler,@aaffaccount,@eCalc,@cmBet)";

        /// <summary>
        /// 新增返水时，创建记录 记录会员id
        /// 并确认是否分配置到指定电销人员
        /// </summary>
        public const string OfTurnoverInsertWithSalerSql_V = @"INSERT INTO [dbo].[ofTurnover]
           ([currency],[account],[totalbets],[effectivebets],[reward],[winlost] ,[leftbets] ,[weekmaxbets] ,[begindate]
           ,[enddate],[requesttime],[gametype],[guid],[referrer],[referrerreg],[level],[referenceguid],[calc],[platformGame],[bonusPoints],[pCalc],[saler],[aaffaccount],[eCalc],[cmBet],[t_f_uid])
     VALUES
           (@currency,@account,@totalbets,@effectivebets,@reward,@winlost,@leftbets ,@weekmaxbets,@begindate
           ,@enddate ,@requesttime,@gametype,@guid,@referrer,@referrerreg,@level,@referenceguid,@calc,@platformGame,@bonusPoints,@pCalc,@saler,@aaffaccount,@eCalc,@cmBet,@t_f_uid)";



        #region 代理 报表更新
        /// <summary>
        /// 代理日财务报表
        /// </summary>
        public const string UpdateAffDailyReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofAffDailyReportFinance WHERE financeDay = @dd and account=@affaccount))
begin
INSERT INTO [dbo].[ofAffDailyReportFinance]
           ([account],[financeDay],{0},turnover)
     VALUES(@affaccount,@dd,@turnover,@turnover)
end
else
begin
update ofAffDailyReportFinance set {0}={0}+@turnover,turnover=turnover+@turnover where financeDay=@dd and account=@affaccount
end";
        /// <summary>
        /// 代理月财务报表
        /// </summary>
        public const string UpdateAffMonthReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofAffMonthReportFinance WHERE financeMonth = @MM and account=@affaccount))
begin
INSERT INTO [dbo].[ofAffMonthReportFinance]
           ([account],[financeMonth],{0},turnover)
     VALUES(@affaccount,@MM,@turnover,@turnover)
end
else
begin
update ofAffMonthReportFinance set {0}={0}+@turnover,turnover=turnover+@turnover where financeMonth=@MM and account=@affaccount
end";
        /// <summary>
        /// 更新游戏日报表
        /// </summary>
        public const string UpdateAffDailyReportGame = @"if(not EXISTS (SELECT top 1 * FROM ofAffDailyReportGame WHERE gameReportDay = @dd and account=@account ))
begin
INSERT INTO [dbo].[ofAffDailyReportGame]
           ([account],[gameReportDay],{0},{1})
     VALUES(@account,@dd,{2},{3})
end
else
begin
update ofAffDailyReportGame set {4} where gameReportDay=@dd and account=@account
end";
        /// <summary>
        /// 更新游戏月报表
        /// </summary>
        public const string UpdateAffMonthReportGame = @"if(not EXISTS (SELECT top 1 * FROM ofAffMonthReportGame WHERE gameReportMonth=@MM and account=@account))
begin
INSERT INTO [dbo].[ofAffMonthReportGame]
           ([account],[gameReportMonth],{0},{1})
     VALUES(@account,@MM,{2},{3})
end
else
begin
update ofAffMonthReportGame set {4} where gameReportMonth=@MM and account=@account
end";
        #endregion

        #region 会员 报表更新
        /// <summary>
        /// 代理日财务报表
        /// </summary>
        public const string UpdatePlayerDailyReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerDailyReportFinance WHERE financeDay = @dd and account=@account ))
begin
INSERT INTO [dbo].[ofPlayerDailyReportFinance]
           ([account],[affaccount],[financeDay],{0},turnover)
     VALUES(@account,@affaccount,@dd,@turnover,@turnover)
end
else
begin
update ofPlayerDailyReportFinance set {0}={0}+@turnover,turnover=turnover+@turnover where financeDay=@dd and account=@account
end";
        /// <summary>
        /// 代理月财务报表
        /// </summary>
        public const string UpdatePlayerMonthReportFinance = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerMonthReportFinance WHERE financeMonth = @MM and account=@account))
begin
INSERT INTO [dbo].[ofPlayerMonthReportFinance]
           ([account],[affaccount],[financeMonth],{0},turnover)
     VALUES(@account,@affaccount,@MM,@turnover,@turnover)
end
else
begin
update ofPlayerMonthReportFinance set {0}={0}+@turnover,turnover=turnover+@turnover where financeMonth=@MM and account=@account
end";
        /// <summary>
        /// 更新游戏日报表
        /// </summary>
        public const string UpdatePlayerDailyReportGame = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerDailyReportGame WHERE gameReportDay=@dd and account=@account))
begin
INSERT INTO [dbo].[ofPlayerDailyReportGame]
           ([account],[affaccount],[gameReportDay],{0},{1})
     VALUES(@account,@affaccount,@dd,{2},{3})
end
else
begin
update ofPlayerDailyReportGame set {4} where gameReportDay=@dd and account=@account
end";
        /// <summary>
        /// 更新游戏月报表
        /// </summary>
        public const string UpdatePlayerMonthReportGame = @"if(not EXISTS (SELECT top 1 * FROM ofPlayerMonthReportGame WHERE gameReportMonth=@MM and account=@account ))
begin
INSERT INTO [dbo].[ofPlayerMonthReportGame]
           ([account],[affaccount],[gameReportMonth],{0},{1})
     VALUES(@account,@affaccount,@MM,{2},{3})
end
else
begin
update ofPlayerMonthReportGame set {4} where gameReportMonth=@MM and account=@account
end";
        #endregion
        /// <summary>
        /// 设置返水定单状态
        /// </summary>
        public const string UpdateOfTurnOverStatus = @"update ofTurnOver set status=@status, reviewperson=@reviewperson,reviewtime=@reviewtime where status=0 and id in {0}";

        public const string UpdateOfTurnOverStatusCancel = @"update ofTurnOver set status=@status, cancelperson=@cancelperson,cancelreason=@cancelreason,canceltime=@canceltime where status=0 and id in {0}";


        /// <summary>
        /// 获取当前所有优惠信息
        /// 大图，小图，优惠内容
        /// 主要用来显示首页banner,优惠页的优惠小图和优惠内容
        /// </summary>
        public const string GetPromoDetail_old = @"select * from ofPromotionContent where status=1 and endtime>=@endtime order by psort desc";
        
        /// <summary>
        /// 查询上线的优惠
        /// </summary>
        public const string GetPromoDetailWithCurrency = @"select * from (
select * from ofPromotionContent with(nolock) where status=1 and endtime>=@endtime and offline=0 and currencyType = @currencyType
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";

        /// <summary>
        /// 查询上线和测试的优惠
        /// </summary>
        public const string GetPromoDetailWithCurrency_offline = @"select * from (
select * from ofPromotionContent with(nolock) where status=1 and endtime>=@endtime and offline >= 0 and currencyType = @currencyType
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";


        public const string GetPromoDetailWithoutCurrency = @"select * from (
select * from ofPromotionContent with(nolock) where status=1 and endtime>=@endtime and offline=0
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";

        public const string GetPromoDetailByIdWithoutCurrency = @"select * from (
select * from ofPromotionContent with(nolock) where endtime>=@endtime and offline=0 and id=@promoId
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";

        public const string GetPromoDetailByIdWithoutCurrency_offline = @"select * from (
select * from ofPromotionContent with(nolock) where endtime>=@endtime and offline>=0 and id=@promoId
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";


        /// <summary>
        /// 查询上线和测试的优惠
        /// </summary>
        public const string GetPromoDetailWithoutCurrency_offline = @"select * from (
select * from ofPromotionContent with(nolock) where status=1 and endtime>=@endtime and offline >=0
)a,
(select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL') e
order by a.psort desc ";



        #region 存款的优惠列表


        public const string GetDepositPromoList = @"select id s_value, titleSec s_text,offline from ofPromotionContent with(nolock)
        where isDepositListPromo=1 and status=1 and endtime>=@endtime and vips like '%{0}%' and offline={1}
        order by psort desc";

        /// <summary>
        /// 查询已经参与的周存优惠，如果超过次数，优惠列表不会显示
        /// </summary>
        public const string GetAlreadyDepositPromoList = @"select bonusPromoCode from ofBonusPromo
where account=@account and bonusPromoCode = 10042 and createtime>=@starttime and status=1
union
select bonusPromoCode from ofBonusPromo
where account=@account and bonusPromoCode = 10093 and createtime>=@starttime and status=1
union
select bonusPromoCode from ofBonusPromo
where account=@account and bonusPromoCode = 10094 and createtime>=@starttime and status=1 ";

        #endregion

        /// <summary>
        /// 会员的存款的优惠列表
        /// </summary>
        public const string GetSpecialDepositPromoList = @"select id s_value, titleSec s_text from ofPromotionContent
where isDepositListPromo=1 and status=1 and endtime>=@endtime 
order by psort desc";

        /// <summary>
        /// 体育线下优惠代码
        /// </summary>
        public const string GetOfflineBonusCodeList = @"select bonusPromoCode from ofBonusPromo
where account=@account and bonusPromoCode = 10104 and DateDiff(mm,createtime,DATEADD(hour,8,getutcdate()))=0 and status=1 ";

        /// <summary>
        /// 获取已经分配的线下优惠
        /// </summary>
        public const string GetAccessOfflinePromos = @"select pc.id from ofPromotionContent pc
left join ofBonusPromoOfflineAccess pca on pc.id = pca.bonusPromoCode
where pca.account=@account and pc.offline=1 and pca.status = 1 and pc.status=1";




        #region 存款的优惠列表 优惠券


        /// <summary>
        /// 优惠券 当天使用
        /// </summary>
        public const string GetCouponsDepositPromoList = @"select * from ofBonusCoupons with(nolock)
where account=@account and DateDiff(day,create_at,DATEADD(hour,8,getutcdate()))=0 and c_status=1 and ctype=0 ";

        /// <summary>
        /// 优惠券 时间段内使用 Boss 
        /// </summary>
        public const string GetCouponsDepositPromoList_Boss = @"select * from ofBonusCoupons with(nolock)
where account=@account and startTime<='{0}' and endTime>='{0}' and c_status=1 and ctype=0 ";

        #endregion

    }
}
