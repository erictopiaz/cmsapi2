using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.PP
{
    public class PPParamsModel
    {
        public string apiurl { set; get; }
        public string SecretKey { set; get; }
        public string secureLogin { set; get; }
        public string ProviderId { set; get; }
        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }
        public string prefix { set; get; }
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }

        public string gameName { set; get; } = "PP";
    }
}
