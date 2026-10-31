using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.PA
{
    public class order_pa
    {
        [Key]
        public long id { set; get; }
        public string session_token { set; get; }
        public string currency { set; get; }
        public decimal? value { set; get; }
        public string playname { set; get; }
        public string agent_code { set; get; }
        public DateTime bet_time { set; get; }
        public string transaction_id { set; get; }
        public string platform_type { set; get; }
        public string round { set; get; }
        public string gametype { set; get; }
        public string game_code { set; get; }
        public string table_code { set; get; }
        public string transaction_type { set; get; }
        public string transaction_code { set; get; }
        public string device_type { set; get; }
        public string playtype { set; get; }
        public string parents { set; get; }

    }

    // session_token, currency, value, playname,
    // agent_code, bet_time, transaction_id, platform_type, round,
    // gametype, game_code, table_code, transaction_type,
    // transaction_code, device_type, playtype, parents
}
