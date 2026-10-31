using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.JDB {
    public class JDB_BetOrderLog_rollback {
        public int action { set; get; }
        /// <summary>
        /// 当前系统时间
        /// </summary>
        public long ts { set; get; }
        /// <summary>
        /// 取消时间
        /// </summary>
        public DateTime cancel_time { set; get; }
        /// <summary>
        /// 交易序号
        /// </summary>
        [Key]
        public string transferId { set; get; }
        /// <summary>
        /// 玩家账号(30)
        /// </summary>
        public string uid { set; get; }
    }
}
