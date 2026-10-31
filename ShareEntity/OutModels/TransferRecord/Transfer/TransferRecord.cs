using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Transfer
{
   
    /// <summary>
    /// 转账记录
    /// </summary>
    public class TransferRecord : RecordBase
    {
        private int _gameid;
        /// <summary>
        /// 转账游戏id
        /// </summary>
        public int gameid
        {
            set
            {
                amount = amount * (value > 3000 ? 1 : -1);
                _gameid = value;
            }
            get { return _gameid; }
        }

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
        /// <summary>
        /// 转账方向
        /// </summary>
        public string gamename2 { set; get; }

        /// <summary>
        /// 左转前
        /// </summary>
        public decimal leftamountbefore { set; get; } = 0;
        /// <summary>
        /// 左转后
        /// </summary>
        public decimal leftamountafter { set; get; } = 0;
        /// <summary>
        /// 右转前
        /// </summary>
        public decimal rightamountbefore { set; get; } = 0;
        /// <summary>
        /// 右转后
        /// </summary>
        public decimal rightamountafter { set; get; } = 0;
    }



}
