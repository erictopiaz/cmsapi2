using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.Fb
{
    /// <summary>
    /// fb 提前结算
    /// </summary>
    public class FBCashoutModel : FBRequestBaseModel
    {
        /// <summary>
        /// 提前结算的id
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 下单时的 businessId
        /// </summary>
        public string orderId { set; get; }
        public string userId { set; get; }
        public int? walletType { set; get; }
        public int? currency { set; get; }
        public decimal? exchangeRate { set; get; }
        public long? cashoutTime { set; get; }
        public long? betTime { set; get; }
        public long? settleTime { set; get; }
        public long? createTime { set; get; }
        public long? cancelTime { set; get; }
        public decimal? cashOutStake { set; get; }
        public int? orderStatus { set; get; }
        public decimal? cashOutPayoutStake { set; get; }
        public bool? acceptOddsChange { set; get; }
        public int? seriesType { set; get; }
        public string betType { set; get; }
        public decimal? orderStakeAmount { set; get; }
        public string ip { set; get; }
        public string remark { set; get; }
        public int? cancelReasonCode { set; get; }
        public decimal? cancelCashOutAmountTo { set; get; }
        public decimal? unitCashOutPayoutStake { set; get; }
        public string device { set; get; }
        public int? version { set; get; }
        public long? lastModifyTime { set; get; }
    }
}
