using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.JL {
    public class JL_OrderLog_Cancel {

        /// <summary>
        /// 请求标识符。同一取消重送请求时会与前次不同
        /// </summary>
        public string reqId { set; get; }
        /// <summary>
        /// 货币名称
        /// </summary>
        public string currency { set; get; }
        /// <summary>
        ///  游戏代码
        /// </summary>
        public int game { set; get; }
        /// <summary>
        /// 注单唯一识别值(等同 §3.1.3 WagersId)
        /// </summary>
        [Key]
        public long round { set; get; }
        /// <summary>
        /// 请参考离线模式
        /// </summary>
        public bool offline { set; get; } = false;

        /// <summary>
        /// 押注金额
        /// </summary>
        public decimal betAmount { set; get; }
        public decimal winloseAmount { set; get; }
        /// <summary>
        /// 玩家账号唯一值(依营运商需求带入; 离线开奖一定会带)
        /// </summary>
        public string userId { set; get; } = "";

        /// <summary>
        /// 牌局唯一识别值(等同 §4.2.7 sessionId)
        /// </summary>
        public long sessionId { set; get; } = -1;
        public int type { set; get; }
        /// <summary>
        ///  预扣金额(等同 §4.2.7 preserve)
        /// </summary>
        public decimal preserve { set; get; } = 0;


        
    }
}
