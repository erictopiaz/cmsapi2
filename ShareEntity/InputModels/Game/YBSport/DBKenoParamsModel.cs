using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.YBSport
{
    public class DBKenoParamsModel
    {

        //{"apiurl":"https://api.obothapi.com","merchantCode":"8RK76","Aeskey":"","MD5Key":"","prefix":"","gamestatus":1 }

        /// <summary>
        /// 商户编码 ，会员账号 loginName =   merchantCode + 会员账号
        /// </summary>
        public string merchantCode { set; get; }
        public string Aeskey { set; get; }
        public string MD5Key { set; get; }
        public string apiUrl { set; get; }

        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }

        /// <summary>
        /// 暂无使用
        /// </summary>
        public string loginUrl { set; get; }
        /// <summary>
        /// 失败后返回页面
        /// </summary>
        public string backUrl { set; get; } = "";

        public string prefix { set; get; }

        public string gameName { set; get; } = "DBKeno";
        public string proxy { set; get; }
    }

    public class DBKenoLoginModel 
    {
        /// <summary>
        /// Web 跳转地址
        /// </summary>
        public string pc { set; get; }
        /// <summary>
        /// H5 跳转地址
        /// </summary>
        public string h5 { set; get; }
        public string returnUrl { set; get; }

    }
    public class DBKenoCreateModel : DBKenoResponseBase
    {
    }
    public class DBKenoResponseBase
    {
        public int code { set; get; }
        public string msg { set; get; }

        
    }
    public class DBKenoModel<T> : DBKenoResponseBase
    {
        public T data { set; get; }
    }

    public class DBKenoBalance
    {
        /// <summary>
        /// 玩家帐号
        /// </summary>
        public string member { set; get; }
        /// <summary>
        /// 玩家余额
        /// </summary>
        public string amount { set; get; }
    }

    public class DBKenoTransferModel : DBKenoResponseBase
    {
        /// <summary>
        /// 玩家帐变后的余额,  当 code 不为０ 时，则为 null。
        /// </summary>
        public string balance { set; get; }
    }

}
