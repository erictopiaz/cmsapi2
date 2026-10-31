using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.Spribe {
    
    public class Spribe_BetOrderLog_rollback {

        public string user_id { set; get; }
        public decimal amount { set; get; }
        public string provider { set; get; }
        /// <summary>
        /// 这个是对应 withdraw 中的 provider_tx_id
        /// </summary>
        public string rollback_provider_tx_id { set; get; }
        [Key]
        public string provider_tx_id { set; get; }
        public string game { set; get; }
        public string session_token { set; get; }
        public string action { set; get; }
        public string action_id { set; get; }
        public DateTime create_time { set; get; }
    }
}
