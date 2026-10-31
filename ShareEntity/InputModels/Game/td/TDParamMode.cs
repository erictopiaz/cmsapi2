using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.td
{
    public class TDParamMode
    {
        //{"hashcode":"888825_c5795840db501d5135055019c51c27fe",
        //"apiKey":"5b054c4e37c2dc4edde0c1d0984344a4",
        //"siteid":"888825",
        //"apiUrl":"http://203.60.12.210:19007/dsapi/index",
        //"loginUrl":"https://p.win1618.com/Default/IndexNew",
        //"mobileloginurl":"https://wap.win1618.com/Default/IndexNew",
        //"jumpurl":"https://www.awcxj.com/game/td.html",
        //"prefix":"",
        //"gamestatus":1}

        public string hashcode { set; get; }
        public string apiKey { set; get; }
        public string siteid { set; get; }
        public string apiUrl { set; get; }
        public string loginUrl { set; get; }
        public string mobileloginurl { set; get; }
        public string jumpurl { set; get; }
        public string apiBetUrl { set; get; }
        public string prefix { set; get; }
        public int gamestatus { set; get; }
        public string gameName { set; get; } = "TD";
    }


    public class TDResponse
    {
        public int status { set; get; }
        public string message { set; get; }
        public string balance { set; get; }

    }
}
