using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Turnover
{
    
    /// <summary>
    /// 返水记录
    /// </summary>
    public class TurnoverRecord : RecordBase
    {
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal totalbets { set; get; }
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winlost { set; get; }
        /// <summary>
        /// 游戏平台
        /// </summary>
        public string gamename { set; get; }

        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 0 ? "等待处理" : value == 1 ? "处理完成" : "失败";
                _status = value;
            }
            get { return _status; }
        }
    }

}
