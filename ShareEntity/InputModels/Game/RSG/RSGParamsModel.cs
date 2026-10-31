using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.RSG {
    public class RSGParamsModel {
        public string ClientID { set; get; }
        public string ClientSecret { set; get; }
        public string DesKey { set; get; }
        public string DesIV { set; get; }
        /// <summary>
        /// 系統代碼(只限英數)
        /// </summary>
        public string SystemCode { set; get; }
        /// <summary>
        /// 站台代碼(只限英數)
        /// </summary>
        public string WebId { set; get; }

        public string apiUrl { set; get; }
      
        public string loginUrl { set; get; }
       
        public string prefix { set; get; }
        public string gameName { set; get; } = "RSG";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }
}
