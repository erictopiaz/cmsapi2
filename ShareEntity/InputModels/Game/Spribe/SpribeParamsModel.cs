using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.Spribe {
    public class SpribeParamsModel {
        //{"launch_url":"https://dev-test.spribe.io/games/launch",
        //"apiUrl":"https://secure-ga.staging.spribe.io/v3",
        //"operator_key":"omgstg",
        //"secret_token":"S5K8iqdbUG3u8aiStldtkBEjhuQIOLrN",
        //"prefix":"",
        //"gamestatus":1,
        //"proxy":"http://172.233.91.151:65531" }

        public string launch_url { set; get; }
        public string apiUrl { set; get; }
        public string operator_key { set; get; }
        
        public string secret_token { set; get; }
        public string return_url { set; get; } = "";
        public int isIdAccount { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "Spribe";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }
}
