using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.rg
{
    public class RGParamsModel
    {
        // { "apiurl":"https://openapi.espapi.com/gateway",
        //"app_key":"d4bbedef2227cb7b",
        //"app_secret":"33c5bb4ca4be76b25d40b54c9bc8fd8b",
        //"lobby_url":"https://www.awc5.com",
        //"pc_login_url":"https://pc.egaming1.com",
        //"mobile_login_url":"https://h5.egaming1.com"}
        public string apiurl { set; get; }
        public string app_key { set; get; }
        public string app_secret { set; get; }
        public string lobby_url { set; get; }
        public string pc_login_url { set; get; }
        public string mobile_login_url { set; get; }
        public string prefix { set; get; }

        public string gameName { set; get; } = "RG";
    }
}
