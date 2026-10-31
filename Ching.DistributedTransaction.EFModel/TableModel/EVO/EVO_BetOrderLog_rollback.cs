using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.EVO {
    public class EVO_BetOrderLog_rollback{
        public string sid { get; set; }
        public string userId { get; set; }
        public string currency { get; set; }
        public string gameId { get; set; }
        public string gameType { get; set; }
        public string gameDetailTableId { set; get; }
        public string gameDetailTableVid { set; get; }
        [Key]
        public string transactionId { get; set; }
        public string transactionRefId { get; set; }
        public decimal transactionAmount { get; set; }
        public string transactionUuid { get; set; }
        /// <summary>
        /// 注单状态：0：下注成功，1：已结算，2：注单取消
        /// </summary>
        public int status { get; set; } = 0;
        public DateTime create_time { get; set; }
    }
}
