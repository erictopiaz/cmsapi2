using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.Fb
{

    public class fb_orders
    {
        [Key]
        public long selfid { set; get; }
        /// <summary>
        /// 提前结算id
        /// </summary>
        public string id { set; get; }
        /// <summary>
        /// 定单id
        /// </summary>
        public string order_id { set; get; }

        /// <summary>
        /// 渠道用户ID
        /// </summary>
        public string merchant_user_id { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public string merchant_id { get; set; }
        
        public string user_id { get; set; }
        public int? wallet_type { get; set; }
        public int? currency { set; get; }
        public decimal? exchange_rate { get; set; }
        public int? series_type { get; set; }
        public string bet_type { get; set; }
        public int? all_up { get; set; }
        public int? all_up_alive { get; set; }
        public decimal? stake_amount { get; set; }
        public decimal? liability_stake { get; set; }
        public decimal? settle_amount { get; set; }
        public decimal? max_win_amount { get; set; }
        public decimal? lose_amount { get; set; }
        public decimal? valid_settle_stake_amount { get; set; }
        public decimal? valid_settle_amount { get; set; }

        public decimal? cashout_cancel_stake { set; get; }
        public int? order_status { get; set; }
        public int? pay_status { get; set; }
        public int? odds_change { get; set; }
        public string device { set; get; }
        public string ip { get; set; }
        public long? settle_time { get; set; }
        public long? create_time { get; set; }
        public long? modify_time { get; set; }
        public long? cancel_time { get; set; }


        public string third_remark { get; set; }
        public string related_id { get; set; }
        public int? rollback_count { get; set; }
        public int? item_count { get; set; }
        public int? series_value { get; set; }
        public int? bet_num { get; set; }
        public decimal? unit_stake { get; set; }
        public string reserve_id { get; set; }
        public int? reserve_version { get; set; }
        public int? reject_reason { get; set; }
        public string reject_reason_str { get; set; }
        public decimal? cashout_total_stake { get; set; }
        public decimal? liability_cashout_stake { get; set; }
        public decimal? cashout_payout_stake { get; set; }
        public int? cashout_count { get; set; }
        public int? version { get; set; }

        [Column(TypeName = "jsonb")]
        public string bet_list { get; set; }

        public bool? accept_odds_change { set; get; }
        public decimal? cashout_stake { set; get; }
        public long? cashout_time { set; get; }
        public int? cancel_reason_code { set; get; }
        public decimal? cancel_cashout_amount_to { set; get; }
        public decimal? unit_cashout_payout_stake { set; get; }
        public string remark { set; get; }
        public long? last_modify_time { set; get; }
        /// <summary>
        /// 上级代理
        /// </summary>
        public string parents { set; get; } = "";

    }


}
