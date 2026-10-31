using System;
using System.Collections.Generic;
using System.Text;

namespace SmsLib.Models {
    public class MessageConfigModel {
        #region 颂量
        public string appId { set; get; }
        public string Api_Key { set; get; }
        public string Api_Secret { set; get; }
        public string apiUrl { set; get; }
        #endregion

        #region 建周 awc
        public string account { set; get; }
        public string password { set; get; }
        public string plat { set; get; }
        /// <summary>
        /// 签名内容 【常宁市万象城音乐】 动态配置
        /// </summary>
        public string signString { set; get; }
        #endregion

        #region 建周 boss
        public string account_boss { set; get; }
        public string password_boss { set; get; }
        /// <summary>
        /// 签名内容 【】 动态配置
        /// </summary>
        public string signString_boss { set; get; }
        #endregion
    }

    public class MessageResponeModel {
        public string status { set; get; } 
    }

}
