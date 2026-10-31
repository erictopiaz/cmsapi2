using DBLibs.WXDapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.BLL.Promo
{
    /// <summary>
    /// 优惠公共操作
    /// </summary>
    public class PromoBaseBLL
    {
        /// <summary>
        /// 检查是否有申请过的优惠 单个优惠代码确认
        /// </summary>
        /// <param name="account">当前账号</param>
        /// <param name="autopromotype">自助活动类型</param>
        /// <param name="startTime">申请到的活动时间</param>
        /// <param name="platformType">来自哪种类型的平台</param>
        /// <param name="promoCode">来自哪个已配置好的优惠活动</param>
        /// <param name="gametype">来自哪个游戏平台</param>
        /// <returns></returns>
        public static async Task<bool> CheckPromo(string account, int autopromotype, string startTime,
            int platformType = -1, long promoCode = -1, int gametype = -1)
        {
            var whereSql = string.Empty;
            if (platformType != -1)
                whereSql = string.Format(@" and platformType = {0}", platformType);
            if (promoCode != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" and bonusPromoCode = {0}", promoCode) : string.Format(@" {0} and bonusPromoCode = {1}", whereSql, promoCode);
            if (gametype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" and gametype = {0}", gametype) : string.Format(@" {0} and gametype = {1}", whereSql, gametype);

            var sql = string.Format(@"select id from ofBonusPromo
where account='{0}' and autopromotype = {1} and reviewtime>='{2}' and status<2 {3} ",
account, autopromotype, startTime, whereSql);

            return await DapperBase.ExecuteScalarAsync(sql, null,
                commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
        }

        /// <summary>
        /// 检查是否有申请过的优惠 多个优惠代码确认
        /// </summary>
        /// <param name="account">当前账号</param>
        /// <param name="autopromotype">自助活动类型</param>
        /// <param name="startTime">申请时间</param>
        /// <param name="platformType">来自哪种类型的平台</param>
        /// <param name="promoCodes">来自哪个已配置好的优惠活动</param>
        /// <param name="gametype">来自哪个游戏平台</param>
        /// <returns></returns>
        public static async Task<bool> CheckPromoByMultiPromoCode(string account, int autopromotype, string startTime,
            int platformType = -1, List<long> promoCodes = null, int gametype = -1)
        {
            var whereSql = string.Empty;
            if (platformType != -1)
                whereSql = string.Format(@" and platformType = {0}", platformType);

            if (promoCodes != null && promoCodes.Count > 0)
            {
                var promoCodeStr = "";
                foreach(var promoCode in promoCodes)
                {
                    promoCodeStr = string.IsNullOrEmpty(promoCodeStr) ? string.Format(@"{0}", promoCode) : string.Format(@"{0},{1}", promoCodeStr, promoCode);
                }
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" and bonusPromoCode in ({0})", promoCodeStr) : string.Format(@" {0} and bonusPromoCode in ({1})", whereSql, promoCodeStr);

            }
            if (gametype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" and gametype = {0}", gametype) : string.Format(@" {0} and gametype = {1}", whereSql, gametype);

            var sql = string.Format(@"select id from ofBonusPromo
where account='{0}' and autopromotype = {1} and createtime >='{2}' and status<2 {3} ",
account, autopromotype, startTime, whereSql);

            return await DapperBase.ExecuteScalarAsync(sql, null,
                commandTimeout: 1500, ConnectionString: DapperStringBase.ConnectionString);
        }


    }
}
