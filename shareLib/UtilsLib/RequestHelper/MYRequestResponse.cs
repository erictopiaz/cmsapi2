using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLib.RequestHelper {
    public class MYRequestResponse {
        /// <summary>
        /// 回应的http code
        /// </summary>
        public int httpCode { set; get; } = 200;
        /// <summary>
        /// 回应数据
        /// </summary>
        public string responseContent { set; get; }

        public Dictionary<string , string> headers { set; get; } = new Dictionary<string , string>();
    }



}
