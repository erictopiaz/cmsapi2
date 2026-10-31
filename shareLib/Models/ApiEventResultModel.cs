using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    /// <summary>
    /// api操作和操作结果
    /// </summary>
    public class ApiEventResultModel {
        public string actionName { set; get; }
        public string eventName { set; get; }
        public string amount { set; get; }

        public string orderNo { set; get; }
        public string account { set; get; }
        public string cuid { set; get; }
        public string host { set; get; } = "";
        public string ipaddress { set; get; } = "";
        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int deviceId { set; get; }

        /// <summary>
        /// 前端传过来的指纹
        /// </summary>
        public string fingerid { set; get; } = "";
    }
    /// <summary>
    /// 流水审核自动推送数据
    /// </summary>
    public class VerifyWithdrawModel {
        
        public string withdrawOrderNo { set; get; }
        public string account { set; get; }
    }
}
