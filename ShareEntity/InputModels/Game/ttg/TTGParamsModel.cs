using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.ttg
{
    public class TTGParamsModel
    {
       

        public string api_url { set; get; }
        public string api2_url { set; get; }
        public string platform_url { set; get; }
        public string login_url { set; get; }
        public string url { set; get; }
        public string currency { set; get; }
        public string country { set; get; }
        public string lsdId { set; get; }
        public string datafeed_api { set; get; }
        public string lobbyurl { set; get; }
        public string platformUsername { set; get; }
        public string prefix { set; get; }
        public string partnerId { set; get; }
        public string gameName { set; get; } = "TTG";

        /// <summary>
        /// 反向api,访问游戏平台
        /// </summary>
        public string proxy { set; get; }
    }

}
