using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.Spribe {
   
    public class Spribe_BetOrderLog_withdraw {

        public string user_id { set; get; }
        public string currency { set; get; }
        public decimal amount { set; get; }
        public string provider { set; get; }
        [Key]
        public string provider_tx_id { set; get; }
        public string game { set; get; }
        public string action { set; get; }
        public string action_id { set; get; }
        public string session_token { set; get; }
        public string platform { set; get; }
        public string withdraw_provider_tx_id { set; get; }
        public DateTime create_time { set; get; }
    }
}
