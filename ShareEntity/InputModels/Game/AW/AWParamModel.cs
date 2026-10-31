using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.AW
{
    /// <summary>
    /// aw 游戏参数
    /// </summary>
    public class AWParamModel {
        //{"siteid":"1001",
        ////"key":"5a6ceb5c-04e7-4c28-9d56-d3ac4d6acfb9",
        //"apiOrder":"http://45.13.161.122:61282",
        //"link":"https://www.awcpbb.com",
        //"apiurl":"http://142.54.168.202:64981",
        //"transferapiurl":"http://45.13.161.234:58535",
        //"querybalanceapiurl":"http://103.75.47.50:64981",
        //"createuserapiurl":"http://103.75.47.50:64981",
        //"prefix":"",
        //"appTokenApi":"http://awcpaa.com:64981"}

        public string siteid { set; get; }
        public string key { set; get; }
        public string apiOrder { set; get; }
        public string link { set; get; }
        public string apiUrl { set; get; }
        public string transferapiurl { set; get; }
        public string querybalanceapiurl { set; get; }
        public string createuserapiurl { set; get; }
        public string prefix { set; get; }
        public int gamestatus { set; get; }
        /// <summary>
        /// app 登录token api
        /// </summary>
        public string appTokenApi { set; get; }
        public string gameName { set; get; } = "AW";
        /// <summary>
        /// 反向 呼叫游戏api
        /// </summary>
        public string proxy { set; get; }
        /// <summary>
        /// 目前2条，用 “|” 分开
        /// </summary>
        public string loginUrls { set; get; }
    }
}
