using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.AG
{
    public class AGParamsModel
    {
        //{"cagent": "x","md5key": "x","deskey": "x","oddtype": "x","lang": "x","dm": "x","apichkcreate": "x","apifwdgame": "x"}

        public string cagent { set; get; }
        public string productid { set; get; }
        public string md5key { set; get; }
        public string deskey { set; get; }
        public string oddtype { set; get; }
        public string lang { set; get; }
        public string dm { set; get; }

        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }

        /// <summary>
        /// api url
        /// </summary>
        public string apichkcreate { set; get; }
        /// <summary>
        /// login url
        /// </summary>
        public string apifwdgame { set; get; }
        /// <summary>
        /// 注单拉取API
        /// </summary>
        public string apibetrec { set; get; }

        public string prefix { set; get; }
        /// <summary>
        /// 创建session token url
        /// </summary>
        public string createSessionUrl { set; get; } = "";

        public string gameName { set; get; } = "AG";
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }
}
