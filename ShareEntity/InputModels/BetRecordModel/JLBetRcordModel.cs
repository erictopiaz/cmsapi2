using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.BetRecordModel {
    
    public class JLBetRcordModel : BetRcordBaseModel {

        /// <summary>
        /// 是否被取消
        /// </summary>
        private int _isC;
        public int isC {
            set {
                _isC = value;
                if (value == 1)
                    statusDesc = "Cancel";

            }
            get { return _isC; }
        }
        public long sessionId { set; get; }
        public int gid { set; get; }
    }

}
