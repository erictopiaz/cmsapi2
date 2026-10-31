using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.JL {

    public class order_jl_poker {
        /// <summary>
        /// 同一注单重送请求时会与前次不同
        /// </summary>
        public string reqid { set; get; }
        /// <summary>
        ///  营运商 api access token
        /// </summary>
        public string token { set; get; }
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
        /// 注单结账时间戳
        /// </summary>
        public long wagerstime { set; get; }
        /// <summary>
        /// 押注金额
        /// </summary>
        public decimal betamount { set; get; }
        public decimal winloseamount { set; get; }
        public bool isfreeround { set; get; } = false;
        /// <summary>
        /// 玩家账号唯一值(依营运商需求带入; 离线开奖一定会带)
        /// </summary>
        public string userid { set; get; } = "";
        /// <summary>
        /// 1. 鱼机游戏大单号(依营运商需求带入) 或 2. 离线开奖的触发局局号
        /// </summary>
        public long transactionid { set; get; } = -1;
        /// <summary>
        /// 玩家装置信息(依营运商需求带入; 营运商须在 Login §2.1.1 /LoginWithoutRedirect §2.1.2 带入)
        /// </summary>
        public string platform { set; get; } = "";
        /// <summary>
        /// 注单类型(依营运商需求带入; 请参考附录 C)
        /// </summary>
        public int statementtype { set; get; } = -1;
        /// <summary>
        /// 游戏类型(依营运商需求带入; 请参考附录 D)
        /// </summary>
        public int gamecategory { set; get; } = -1;
        /// <summary>
        /// 仅用于免费游戏(free spin), 见免费游戏
        /// </summary>
        public string freespindata { set; get; } = "";

        /// <summary>
        ///  注单类型:1=下注 2=结算
        /// </summary>
        public int? type { set; get; } = 0;



        /// <summary>
        /// 请参考离线模式
        /// </summary>
        public bool? offline { set; get; } = false;  //////////////////LSessionBet

        /// <summary>
        /// 牌局唯一值(所有下注及结算共享同一值, 等同 §3.1.3 WagersId)
        /// </summary>
        public long? sessionid { set; get; }              //////////////////LSessionBet
       
        /// <summary>
        /// 有效投注金额, 结算才会带入(请参考 Turnover (有效投注额) 一节)
        /// </summary>
        public decimal? turnover { set; get; }       //////////////////LSessionBet
        /// <summary>
        /// 预扣金额(仅用于有预先扣款机制的游戏，请参考Preserve (预扣保证金))
        /// </summary>
        public decimal? preserve { set; get; }        //////////////////LSessionBet

        /// <summary>
        /// 牌局全部下注总和(依营运商需求带入; 只在结算时
        /// </summary>
        public decimal? sessiontotalbet { set; get; }        //////////////////LSessionBet
    }
}
