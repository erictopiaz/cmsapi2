using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.CR
{
    public class CRParamModel
    {
        public string AGID { set; get; }
        public string AGName { set; get; }
        public string apiUrl { set; get; }
        public string AGPassword { set; get; }
        public string secretKey { set; get; }
        /// <summary>
        /// 用户id account
        /// </summary>
        public int isIdAccount { set; get; }

        public string prefix { set; get; }

        public int gamestatus { set; get; }
        public string gameName { set; get; } = "CR";
        public string iv { set; get; } = "";
        /// <summary>
        /// 反向api
        /// </summary>
        public string proxy { set; get; }
        /// <summary>
        /// 重新获取token 1：是
        /// </summary>
        public int reloadToken { set; get; } = 0;
    }
    /// <summary>
    /// cr 特别版
    /// </summary>
    public class CRTParamModel: CRParamModel
    {
        public string gameName { set; get; } = "AWSP";
        public string oddsApi { set; get; }
    }
}
