using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.Fb
{

    /// <summary>
    /// fb 通知基本数据结构
    /// merchantUserId,merchantId,currencyId
    /// </summary>
    public class FBRequestBaseModel: MerchantModel
    {
        /// <summary>
        /// 渠道用户ID
        /// </summary>
        public string merchantUserId { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public string merchantId { get; set; }
        /// <summary>
        /// 币种ID
        /// </summary>
        public int currencyId { get; set; }

    }
    /// <summary>
    /// 运营商数据
    /// </summary>
    public class MerchantModel
    {
        /// <summary>
        /// 会员上级代理
        /// </summary>
        public string affaccount { set; get; } = "";
    }


    /// <summary>
    /// fb 注单推送除 提前结算的注单
    /// </summary>
    public class FBOrderModel : FBRequestBaseModel
    {
        /// <summary>
        /// 下单时的businessId
        /// </summary>
        public string id { get; set; }
        public int? rejectReason { get; set; }
        public string rejectReasonStr { get; set; }
        public string userId { get; set; }
        public int? currency { set; get; }
        public decimal? exchangeRate { get; set; }
        public int? seriesType { get; set; }
        public string betType { get; set; }
        public int? allUp { get; set; }
        public int? allUpAlive { get; set; }
        public decimal? stakeAmount { get; set; }
        public decimal? liabilityStake { get; set; }
        public decimal? settleAmount { get; set; }
        public int? orderStatus { get; set; }
        public int? payStatus { get; set; }
        public int? oddsChange { get; set; }
        public string device { set; get; }
        public string ip { get; set; }
        public long? settleTime { get; set; }
        public long? createTime { get; set; }
        public long? modifyTime { get; set; }
        public long? cancelTime { get; set; }
        public string thirdRemark { get; set; }
        public string relatedId { get; set; }
        public decimal? maxWinAmount { get; set; }
        public decimal? loseAmount { get; set; }
        public int? rollBackCount { get; set; }
        public int? itemCount { get; set; }
        public int? seriesValue { get; set; }
        public int? betNum { get; set; }
        public decimal? cashOutTotalStake { get; set; }
        public decimal? liabilityCashoutStake { get; set; }
        public decimal? cashOutPayoutStake { get; set; }
        public string reserveId { get; set; }
        public int? cashOutCount { get; set; }
        public decimal? unitStake { get; set; }
        public int? reserveVersion { get; set; }

        public List<FBBetListModel> betList { get; set; }

        public decimal? maxStake { get; set; }
        public decimal? validSettleStakeAmount { get; set; }
        public decimal? validSettleAmount { get; set; }
        public decimal? cashOutCancelStake { set; get; }
        public int? walletType { get; set; }
        public int? version { get; set; }
    }

    public class FBOrderTableModel : FBRequestBaseModel
    {
        /// <summary>
        /// 下单时的businessId
        /// </summary>
        public string id { get; set; }
        public int? rejectReason { get; set; }
        public string rejectReasonStr { get; set; }
        public string userId { get; set; }
        public int? currency { set; get; }
        public decimal? exchangeRate { get; set; }
        public int? seriesType { get; set; }
        public string betType { get; set; }
        public int? allUp { get; set; }
        public int? allUpAlive { get; set; }
        public decimal? stakeAmount { get; set; }
        public decimal? liabilityStake { get; set; }
        public decimal? settleAmount { get; set; }
        public int? orderStatus { get; set; }
        public int? payStatus { get; set; }
        public int? oddsChange { get; set; }
        public string device { set; get; }
        public string ip { get; set; }
        public long? settleTime { get; set; }
        public long? createTime { get; set; }
        public long? modifyTime { get; set; }
        public long? cancelTime { get; set; }
        public string thirdRemark { get; set; }
        public string relatedId { get; set; }
        public decimal? maxWinAmount { get; set; }
        public decimal? loseAmount { get; set; }
        public int? rollBackCount { get; set; }
        public int? itemCount { get; set; }
        public int? seriesValue { get; set; }
        public int? betNum { get; set; }
        public decimal? cashOutTotalStake { get; set; }
        public decimal? liabilityCashoutStake { get; set; }
        public decimal? cashOutPayoutStake { get; set; }
        public string reserveId { get; set; }
        public int? cashOutCount { get; set; }
        public decimal? unitStake { get; set; }
        public int? reserveVersion { get; set; }

        public string betList_json { get; set; } = "";

        public decimal? maxStake { get; set; }
        public decimal? validSettleStakeAmount { get; set; }
        public decimal? validSettleAmount { get; set; }
        public decimal? cashOutCancelStake { set; get; }
        public int? walletType { get; set; }
        public int? version { get; set; }
    }


    public class FBBetListModel
    {
        public string id { get; set; }
        public string orderId { get; set; }
        public int? sportId { get; set; }
        public string matchId { get; set; }
        public string matchName { get; set; }
        public int? period { get; set; }
        public string marketId { get; set; }
        public int? marketType { get; set; }
        public int? optionType { get; set; }
        public string optionName { get; set; }
        public string marketName { get; set; }
        public string tournamentId { get; set; }
        public string tournamentName { get; set; }
        public string odds { get; set; }
        public int? oddsFormat { get; set; }
        public string betOdds { get; set; }
        public int? settleStatus { get; set; }
        public int? settleResult { get; set; }
        public bool? isInplay { get; set; }
        public string remark { get; set; }
        public decimal? p1 { get; set; }
        public decimal? p2 { get; set; }
        public decimal? p3 { get; set; }
        public string extendedParameter { get; set; }
        public string extraInfo { get; set; }
        public string pendingTime { get; set; }
        public string betScore { get; set; }
        public int? cancelReason { get; set; }
        public string cancelReasonName { get; set; }
        public int? matchType { get; set; }
        public string matchTime { get; set; }
        public int? virtualMatchDay { get; set; }
        public int? virtualLegOrder { get; set; }
        public int? virtualWeekDay { get; set; }
        public int? virtualBlockId { get; set; }
        public int? leaguePhase { get; set; }
    }

}
