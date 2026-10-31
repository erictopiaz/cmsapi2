using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.JDB {
    public class JDB_BetOrderLog_wd {
        public int action { set; get; }
        /// <summary>
        /// 当前系统时间
        /// </summary>
        public long ts { set; get; }
        public DateTime bet_time { set; get; }
        /// <summary>
        /// 交易序号
        /// </summary>
        [Key]
        public string transferId { set; get; }
        /// <summary>
        /// 玩家账号(30)
        /// </summary>
        public string uid { set; get; }

        /// <summary>
        /// 游戏序号
        /// </summary>
        public long gameSeqNo { set; get; }
        /// <summary>
        /// 游戏序号
        /// </summary>
        public string historyId { set; get; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public int gType { set; get; }
        /// <summary>
        /// 机台类型
        /// </summary>
        public int mType { set; get; }
        /// <summary>
        /// 报表日期（dd-MM-yyyy）
        /// </summary>
        public string reportDate { set; get; }
        /// <summary>
        /// 游戏日期 （dd-MM-yyyy HH:mm:ss）
        /// </summary>
        public string gameDate { set; get; }
        /// <summary>
        /// 货币别
        /// </summary>
        public string currency { set; get; }
        /// <summary>
        /// 押注金额
        /// </summary>
        public decimal bet { set; get; }
        /// <summary>
        /// 游戏赢分
        /// </summary>
        public decimal win { set; get; }
        /// <summary>
        /// 总输赢
        /// </summary>
        public decimal netWin { set; get; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime lastModifyTime { set; get; }
        /// <summary>
        /// 序号 预设无此参数，如需要请洽业务人员 (暂没处理)
        /// </summary>
        public string sessionNo { set; get; }

        /// <summary>
        /// 自定义未结算状态， 默认全是结算成功状态， 2：取消
        /// </summary>
        public int status { set; get; } = 1;

        /// <summary>
        /// 投注面值
        /// </summary>
        public decimal denom { set; get; } = -1;
        /// <summary>
        /// 系统结算标记 0：否 1：是
        /// </summary>
        public int systemTakeWin { set; get; } = -1;
        /// <summary>
        /// 游戏区域
        /// -1：大厅（成就游戏）0：小压码区 1：中压码区 2：大压码区
        /// </summary>
        public int roomType { set; get; } = -2;
        public decimal mb { set; get; } = -1;

    }
}
