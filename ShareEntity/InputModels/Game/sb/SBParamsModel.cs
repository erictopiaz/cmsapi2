using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.sb
{
    public class SBParamsModel {
        //{"apiurl": "x","vendorid": "x","loginurl": "x","mobileurl": "x","currency": "x","oddtype": "x"}
        #region api二个可用api 地址
        //p654api.gb688.com
        //http://p654api.bw6688.com/api
        #endregion

        public string apiurl { set; get; }
        public string vendorid { set; get; }
        public string loginurl { set; get; }
        public string mobileurl { set; get; }
        public string currency { set; get; }

        /// <summary>
        /// 厂商 ID。若為子網站則需帶入子網站名稱
        /// </summary>
        public string operatorId { set; get; } = "";

        #region 单一钱包游戏登录
        public string currency_inr { set; get; }
        public string apiurl_inr { set; get; }
        public string vendorid_inr { set; get; }
        #endregion
        /// <summary>
        /// 游戏 平台账号使用会员id生成 ,默认为0
        /// 0:否，1：是
        /// </summary>
        public int isIdAccount { set; get; } = 0;

        public string oddtype { set; get; }
        /// <summary>
        /// 极速源数据api
        /// </summary>
        public string oddsApi { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "OB";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }

    }

    public class OBCVParamsModel {
        public string apiurl { set; get; }
        public string vendorid { set; get; }
        public string operatorId { set; get; }
        public string loginurl { set; get; }
        public string mobileurl { set; get; }
        public string currency { set; get; }
        /// <summary>
        /// 是否让id 游戏平台的会员账号
        /// </summary>
        public int isIdAccount { set; get; }
        public string oddtype { set; get; }
        /// <summary>
        /// 极速源数据api
        /// </summary>
        public string oddsApi { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "OBCV";

        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }



    public class FBParamsModel {

        public string apiurl { set; get; }
        public string ChannelID { set; get; }
        public string secretKey { set; get; }
        public string MerchantName { set; get; }
        /// <summary>
        /// 1:cny
        /// </summary>
        public int currencyId { set; get; }
        public string oddsApi { set; get; }
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

        public string gameName { set; get; } = "FB";
    }


}
