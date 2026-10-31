using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.IM
{

    //id, , , , , , , , round_id, 
    //game_no, , , 
    //action_id, 
    //, , , bet_on, bet_type, platform, tray, product, "", bet_date

    public class order_im
    {
        [Key]
        public long id {  get; set; }
        public string product_wallet { set; get; }
        public string session_token { set; get; }
        public string player_id { set; get; }
        public string provider_player_id { set; get; }
        public string provider { set; get; }
        public string game_id { set; get; }
        public string game_name { set; get; }
        public string platform { set; get; }
        public string bet_id { set; get; }
        public string transaction_id { set; get; }
        public string type { set; get; }
        public int? action_id { set; get; }
        public string currency { set; get; }
        public decimal? amount { set; get; }
        public int product { set; get; }

       // [Column(TypeName = "timestamp")]
        public long timestamp { set; get; }
        /// <summary>
        /// 上级代理
        /// </summary>
        public string parents { set; get; } = "";
    }

}
