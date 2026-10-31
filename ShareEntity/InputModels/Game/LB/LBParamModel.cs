using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.LB
{
    public class LBParamModel
    {
        //{"secret_key": "x","operator_id": "x","site_code": "x","product_code": "x","api": "x","url": "x"}
        public string secret_key { set; get; }
        public string operator_id { set; get; }
        public string site_code { set; get; }
        public string product_code { set; get; }
        public string api { set; get; }
        public string url { set; get; }
        public string prefix { set; get; }
        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        /// <summary>
        /// 单一钱包使用， 转账钱包这里空没有这个参数
        /// </summary>
        public string operator_code { set; get; } = "";
        public string gameName { set; get; } = "LB";
        /// <summary>
        /// 代理反向api
        /// </summary>
        public string proxy { set; get; }

    }
}
