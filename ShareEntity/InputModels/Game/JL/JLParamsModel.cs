using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.JL {
    public class JLParamsModel {
        /// <summary>
        /// api 请求地址
        /// </summary>
        public string apiUrl { set; get; } 
        /// <summary>
        /// dc 下的代理账号 cny
        /// </summary>
        public string agentKey { set; get; }

        public string agentId { set; get; }

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
