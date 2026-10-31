using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleAuth.Models {
    public class GoogleAuthOutModel {

        /// <summary>
        /// 谷歌验证器 扫码
        /// </summary>
        public string qrCodeUrl { get; set; }
        /// <summary>
        /// 手动输入 验证密钥
        /// </summary>
        public string manualCode { set; get; }


    }
}
