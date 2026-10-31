using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TransferRecord.Bonus
{
    /// <summary>
    /// 优惠红利记录
    /// </summary>
    public class BonusPromoRecord : RecordBase
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { set; get; }

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

        public int betTimes { set; get; }
    }

}
