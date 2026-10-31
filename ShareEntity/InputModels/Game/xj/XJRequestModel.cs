using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJRequestModel<T>
    {
        public object carrier { get; set; }

        public T data { get; set; }
    }

    public class XJResponseModel<T>
    {
        //public object carrier { get; set; }

        public T data { get; set; }
        public string code { get; set; }

        public string msg { get; set; }
    }

    public class XJLoginResponseData
    {
        public string url { get; set; } = "";
    }


    public class XJAppLoginModel
    {
        /// <summary>
        /// 新版api : settingUrl
        /// </summary>
        public string url { set; get; }
        public string productUrl { set; get; }
        public string Xauth { get; set; }
        /// <summary>
        /// 新版api : sb-wlapi
        /// </summary>
        public object SessionId { get; set; }
        /// <summary>
        /// 新版api : Authorization
        /// </summary>
        public string token { set; get; }

        public string Referer { get; set; }

    }

    
}
