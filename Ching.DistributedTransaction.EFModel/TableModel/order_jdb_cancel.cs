using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel {
    /// <summary>
    /// jdb 注单取消通知数据
    /// </summary>
    public class order_jdb_cancel {
        public int action { set; get; }
        /// <summary>
        /// 当前系统时间
        /// </summary>
        public long ts { set; get; }
        /// <summary>
        /// 交易序号
        /// </summary>
        [Key]
        public long transferId { set; get; }
        /// <summary>
        /// 玩家账号(30)
        /// </summary>
        public string uid { set; get; }
        /// <summary>
        /// 货币代码
        /// </summary>
        public string currency { set; get; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public int gType { set; get; }
        /// <summary>
        /// 机台类型
        /// </summary>
        public int mType { set; get; }
        public string systemSessionId { set; get; }
    }
}
