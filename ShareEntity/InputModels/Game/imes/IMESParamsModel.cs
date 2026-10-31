using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.imes
{
    public class IMESParamsModel
    {
        //{"merchantname": "x","merchantcode": "x","api": "x","prefix": "x"}

        public string merchantname { set; get; }
        public string merchantcode { set; get; }
        public string api { set; get; }
        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        /// <summary>
        /// cny
        /// </summary>
        public string currency { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "IMES";
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }

    public class IMSportParamsModel
    {

        public string merchantname { set; get; }
        public string merchantcode { set; get; }
        public string api { set; get; }

        public string prefix { set; get; }

        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        /// <summary>
        /// cny
        /// </summary>
        public string currency { set; get; }

        public string gameName { set; get; } = "IMSP";
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }

    public class IMSWParamsModel
    {

        public string merchantname { set; get; }
        public string merchantcode { set; get; }
        public string api { set; get; }

        public string prefix { set; get; }
        public string gameName { set; get; } = "SW";
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }

    public class IMPokerParamsModel
    {
        //{"merchantname": "x","merchantcode": "1526","apiUrl": "https://yshandle.qipaipub.com/index",desKey:"ZHJoWk9U",md5Key:"bWKFQyeGv2UpRlreqonqZZ0Xjpo=","prefix": "x"}

        public string merchantname { set; get; }
        public string merchantcode { set; get; }
        public string desKey { set; get; }
        public string md5Key { set; get; }
        public string apiUrl { set; get; }
        public string homeUrl { set; get; }

        public string prefix { set; get; }
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }
//    {"ac":1,"me":"agentHandle","data":{"code":0,"fullUrl":"http://<server>?userCode=10001&token=FBE54A7273EE4F15B363
//C3F98F32B19F&lang=zh-CN&gameId=0","token":"FBE54A7273EE4F15B363C3F98F32B19F","path":"http://<server2>,http:/
///<server3>,http://<server4>","money":100.81,"freeMoney":100.56,"moneyStr":"100.81","freeMoneyStr":”100.56”}}

    public class IMPokerResponseModel<T>
    {
        public int ac { set; get; }
        public string me { set; get; }
        public T data { set; get; }
    }
    public class ResponseData
    {
        public int code { set; get; }
        public string fullUrl { set; get; }
        public string token { set; get; }
        public string path { set; get; }
        public decimal money { set; get; }
        public decimal freeMoney { set; get; }
        public string freeMoneyStr { set; get; }
    }
}
