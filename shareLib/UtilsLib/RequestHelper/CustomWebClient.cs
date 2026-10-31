using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UtilsLib.RequestHelper
{
    public class CustomWebClient : WebClient
    {
        // 超时时间（毫秒），默认为 100 秒
        public int Timeout { get; set; } = 100000;
        public CustomWebClient()
        {
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
}
