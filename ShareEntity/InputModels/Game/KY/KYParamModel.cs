using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.KY
{
    public class KYParamModel
    {
        //{"agent": "x","apiUrl": "x","deskey": "x","md5key": "x","apiGRUrl": "x","prefix": "x"}
        public string agent { set; get; }
        public string apiUrl { set; get; }
        public string deskey { set; get; }
        public string md5key { set; get; }
        public string apiGRUrl { set; get; }
        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "KY";
        /// <summary>
        /// 自定义区分网站
        /// </summary>
        public string lineCode { set; get; } = "";
        public string proxy { set; get; }
    }
}
