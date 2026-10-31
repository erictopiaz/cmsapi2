using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.PA
{
    public class order_pa_tie
    {
        [Key]
        public long id { set; get; }
        public string session_token { set; get; }
        public string currency { set; get; }
        public decimal? net_amount { set; get; }
        public decimal? valid_bet_amount { set; get; }
        public string playname { set; get; }
        public string agent_code { set; get; }
        public DateTime settletime { set; get; }
        public string bill_no { set; get; }
        public string transaction_id { set; get; }
        public string gametype { set; get; }
        public string game_code { set; get; }
        public string transaction_type { set; get; }
        public string transaction_code { set; get; }
        public string ticket_status { set; get; }
        public string game_result { set; get; }
        public bool? finish { set; get; }
        public string parents { set; get; }
    }
    //session_token, currency, net_amount, valid_bet_amount,
    //playname, agent_code, settletime, bill_no,
    //transaction_id, gametype, game_code, transaction_type,
    //transaction_code, ticket_status, game_result, finish, parents
}
