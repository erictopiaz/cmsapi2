using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.BB
{
    public class BBParamsModel
    {
        public string website { set; get; }
        public string createmember_key { set; get; }
        public string login_key { set; get; }
        public string lobby_key { set; get; }
        public string logout_key { set; get; }

        public string record_key { set; get; }

        public string checkusrbalance_key { set; get; }
        public string transfer_key { set; get; }
        public string api_url { set; get; }
        public string uppername { set; get; }
        public string api_login_url { set; get; }
        public string page_site { set; get; }

        public string prefix { set; get; }
        public string gameName { set; get; } = "BBIN";
        /// <summary>
        /// 反向api,访问游戏平台
        /// </summary>
        public string proxy { set; get; }
    }
}
