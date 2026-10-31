using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    public class GameSqlCommandText
    {
        #region 读取游戏参数到缓存中

        /// <summary>
        /// 获取游戏参数列表 （旧）
        /// </summary>
        public const string QueryGameParamsSql = @"select gamename,params as param, gameid from ofGameClass with(nolock) where class=1 and online=1 and gameid >0 and agentAccount=@agentAccount";
        /// <summary>
        /// 获取游戏参数列表(线上或线下都能获取到) （旧）
        /// </summary>
        public const string QueryAllGameParamsSql = @"select gamename,params as param,gameid from ofGameClass with(nolock) where class=1 and online in(0,1) and gameid >0 and agentAccount=@agentAccount";


        /// <summary>
        /// 获取游戏参数列表 
        /// 2025-05-31 品牌
        /// </summary>
        public const string QueryMerchantGameParamsSql = @"select gamename,params as param, gameid, g_merchantAccounts,currencyTypes from ofGameClassMerchant with(nolock) where class=1 and online=1 and gameid >0 and agentAccount=@agentAccount";
        /// <summary>
        /// 获取游戏参数列表(线上或线下都能获取到)
        /// 2025-05-31 品牌
        /// </summary>
        public const string QueryAllMerchantGameParamsSql = @"select gamename,params as param,gameid, g_merchantAccounts,currencyTypes from ofGameClassMerchant with(nolock) where class=1 and online in(0,1) and gameid >0 and agentAccount=@agentAccount";

        #endregion


        /// <summary>
        /// 游戏存点记录
        /// </summary>
        public const string InsertTransferDepositSql = @"insert into ofTransferNew(currency, account, amount, gameid, gamename, promotioncode, requesttime, leftamountbefore, leftamountafter, rightamountbefore, rightamountafter, status, affaccount, elementid, referrerno, ipaddress, macaddress, referrer, levels, createcustomer, process, hasview, currenturl, apidata)
values('{0}','{1}',{2},{3}, N'{4}',{5},'{6}',(select amountactive from ofUser where account='{1}'),(select amountactive-{2} from ofUser where account='{1}'),{7},{8},{9},'{10}','{11}','{12}','{13}', N'{14}','{15}',{16},'{17}','{18}','{19}','{20}','{21}')";
        /// <summary>
        /// 游戏取点记录
        /// </summary>
        public const string InsertTransferWithdrawSql = @"insert into ofTransferNew(currency, account, amount, gameid, gamename, promotioncode, requesttime, leftamountbefore, leftamountafter, rightamountbefore, rightamountafter, status, affaccount, elementid, referrerno, ipaddress, macaddress, referrer, levels, createcustomer, process, hasview, currenturl, apidata)
values('{0}','{1}',{2},{3}, N'{4}',{5},'{6}',{7},{8},(select amountactive from ofUser where account='{1}'),(select amountactive-{2} from ofUser where account='{1}'),{9},'{10}','{11}','{12}','{13}', N'{14}','{15}',{16},'{17}','{18}','{19}','{20}','{21}')";
        /// <summary>
        /// 游戏存点前扣除主账号余额
        /// </summary>
        public const string UpdateBalanceSql = @"update ofUser set amountactive=amountactive-{0} where account='{1}' and amountactive-{0}>=0";

        /// <summary>
        /// 获取前当游戏id和名字列表放到cache
        /// </summary>
        public const string GetActiveGameidAndName = @"Select gameid,gamename from ofGameClass where gamestatus=1 and class=0 ";

        /// <summary>
        /// 获取客人的账号密码
        /// Select PassKey from ofUser where account= @account
        /// </summary>
        public const string GetPasskey = @"Select PassKey from ofUser where account= @account";
        /// <summary>
        /// 获取ag账号和密码
        /// </summary>
        public const string LoadAgAccountSql = @"select agaccount from ofUser where account = @account";
        /// <summary>
        /// 获取游戏账号和密码
        /// </summary>
        public const string LoadGameAccountSql = @"select * from ofUser where account = @account";
        /// <summary>
        /// 更新游戏的参数列表的状态
        /// </summary>

        public const string UpdateGameConfigSql = @"update ofGameClass set gamestatus=@gamestatus where id=@id";

        /// <summary>
        /// 查询客人的转账平台
        /// </summary>
        public const string QueryTransfePlatSql = @"select gc.plat from ofTransfer t
left join ofGameClass gc on t.type= gc.gameid
where account='{0}' and gc.useable=1 and gc.plat is not null
group by gc.plat ";
    }
}
