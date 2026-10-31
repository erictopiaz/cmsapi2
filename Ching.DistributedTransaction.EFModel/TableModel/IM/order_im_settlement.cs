using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.IM
{

    //, , , , , , round_id, game_no, ,
    //, , ref_transaction_ids, , bet_type, , ,
    //, actual_bet_amount, actual_win_amount, detail,
    //settlement_date, , , "", sg_type, sg_count, sg_sequence

    public class order_im_settlement
    {
        [Key]
        public long id { set; get; }
        public string product_wallet { set; get; }
        public string player_id { set; get; }
        public string provider { set; get; }
        public string game_id { set; get; }
        public string game_name { set; get; }
        public string bet_id { set; get; }
        public string transaction_id { set; get; }
        public int action_id { set; get; }
        public string type { set; get; }
        public string currency { set; get; }
        public decimal amount { set; get; }
        public int? wallet_type { set; get; }
        public int product { set; get; }
        public string event_id { set; get; }
        public long timestamp { set; get; }
        /// <summary>
        /// 上级代理
        /// </summary>
        public string parents { set; get; } = "";
    }




}
