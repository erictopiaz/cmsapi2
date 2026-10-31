using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.xj
{
    public class XJParamsModel
    {
        //{"key": "x","mid": "x","spi": "x","url": "x"}
//        opcode
// apiurl
//partnerapiurl
//key
//iv
//partnerurl
//launchurl

        public string opcode { set; get; }
        public string apiurl { set; get; }
        public string apiUatUrl { set; get; }
        public string partnerapiurl { set; get; }
        public string key { set; get; }
        public string iv { set; get; }
        public string uatkey { set; get; }
        public string uativ { set; get; }
        public string partnerurl { set; get; }
        public string partneruaturl { set; get; }
        /// <summary>
        /// 加白的域名
        /// </summary>
        public string launchurl { set; get; }
        public string prefix { set; get; }
        public int isIdAccount { set; get; }
        public string gameName { set; get; } = "XJ";

    }
    
}
