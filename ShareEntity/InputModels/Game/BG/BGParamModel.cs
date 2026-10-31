using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.BG
{
    public class BGParamModel
    {
        //{ "apiurl":"http://am.bgvip55.com/open-cloud/api/",
        //"sn":"am00",
        //"secretCode":"8153503006031672EF300005E5EF6AEF",
        //"apigamerurl":"",
        //"prefix":"",
        //"gamestatus":1 }
        public string sn { set; get; }
        public string agent { set; get; }
        public string apiUrl { set; get; }
        public string secretCode { set; get; }
        public string apigamerurl { set; get; }
        public string prefix { set; get; }
        public string secretKey { set; get; }
        public int gamestatus { set; get; }
        public string gameName { set; get; } = "BG";
        /// <summary>
        /// 反向访问游戏 api
        /// </summary>
        public string proxy { set; get; }
    }
}
