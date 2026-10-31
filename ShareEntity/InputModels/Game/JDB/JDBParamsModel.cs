using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.JDB
{
    public class JDBParamsModel
    { 
        /// <summary>
        /// api 请求地址
        /// </summary>
        public string apiUrl { set; get; }
        /// <summary>
        /// dc 下的代理账号 cny
        /// </summary>
        public string agentAccount { set; get; }
       
        public string aesKey { set; get; }
        public string aesIV { set; get; }
        public string dc { set; get; }

        #region 
        /// <summary>
        /// api 请求地址
        /// </summary>
        public string apiUrl_inr { set; get; }
        /// <summary>
        /// dc 下的代理账号 inr
        /// </summary>
        public string agentAccount_inr { set; get; }
        public string aesKey_inr { set; get; }
        public string aesIV_inr { set; get; }
        public string dc_inr { set; get; }
        #endregion

        public int isIdAccount { set; get; }

        /// <summary>
        /// 会员账号前缀
        /// </summary>
        public string prefix { set; get; }

        /// <summary>
        /// 反向 访问游戏 api
        /// </summary>
        public string proxy { set; get; }
    }
}
