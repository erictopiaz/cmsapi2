using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.tf
{
    public class TFParamModel
    {
        public string private_token { set; get; }
        public string public_token { set; get; }
        public int operator_id { set; get; }
        public string apiUrl { set; get; }

        public string apigamerurl { set; get; }
        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        /// <summary>
        /// 登录后，要验证token
        /// 1：配置token redis  0:否
        /// </summary>
        public int tokenValidate { set; get; } = 0;
        public string prefix { set; get; }
        public int gamestatus { set; get; }
       
        public string gameName { set; get; } = "TF";
        /// <summary>
        /// 反向 调用 游戏 api
        /// </summary>
        public string proxy { set; get; }

    }
}
