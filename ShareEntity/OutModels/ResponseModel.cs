using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.lang;
using static UtilsLib.lang.LangCodes;

namespace ShareEntity.OutputModels
{
    public class ResponseModel<T>
    {
        public Global.ResponseCode.code code { get; set; } = Global.ResponseCode.code.FAIL;
        public string message{ get; set; } = "";
        public T Data { get; set; }
    }

    public class ResponseModel
    {
        public Global.ResponseCode.code code { get; set; } = Global.ResponseCode.code.FAIL;
        public string message { get; set; } = "";
    }


    #region 游戏注单返回结果 2025-02-27
    public class BetResponeModel<T,P,B,C> {
        public string message { set; get; }
        public T result { set; get; }
        public P pageInfo { set; get; }
        public B totalData { set; get; }
        public C pageTotalData { set; get; }
    }

    public class BetResponeModel<T>
    {
        public string message { set; get; }
        public T result { set; get; }
    }

    public class UserBetReportModel
    {
        public string userName { set; get; } = "";
        public decimal totalBetAmount { set; get; } = 0;
        public decimal betAmount { set; get; }
        public decimal prizeAmount { set; get; } = 0;
    }
    public class GameModel
    {
        /// <summary>
        /// 游戏平台id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 游戏名
        /// </summary>
        public string gameName { get; set; }
        /// <summary>
        /// 0.正常显示日期 1.未结算选项显示 > 所有
        /// </summary>
        public int showType { get; set; }
    }


    public class PageInfo {
        public int pageIndex { set; get; }
        public int totalCount { set; get; }
        public int pageSize { set; get; }
        public int pageCount { set; get; }
    }

    public class TotalData {
        public bool? settled { set; get; }
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal betAmount { set; get; } = 0;
        public decimal prizeAmount { set; get; } = 0;
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal totalBetAmount { set; get; } = 0;
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winLost { set; get; } = 0;
        public decimal maxWinAmount { set; get; }
        public decimal count { set; get; }
    }

    public class TotalDataV
    {
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal validAmount { set; get; } = 0;
        public decimal prizeAmount { set; get; } = 0;
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal betAmount { set; get; } = 0;
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal winLost { set; get; } = 0;
        public decimal maxWinAmount { set; get; }
        public decimal count { set; get; }
    }


    public class PageTotalData : TotalData { }



    public class BaseBetModel
    {
        public string id { set; get; }
        /// <summary>
        /// 游戏id
        /// </summary>
        public int gameId { set; get; }
        public string gameName { set; get; }

        public string betId { set; get; }
        public int? game { set; get; }
        public string gameTitle { set; get; }
        public string round { set; get; }
        public decimal? betAmount { set; get; }
        public decimal? validAmount { set; get; }
        public decimal? maxWinAmount { set; get; }
        public decimal? prizeAmount { set; get; }
        public decimal? winLost { set; get; }
        public decimal? cancelAmount { set; get; }
        public string gameContent { set; get; }
        public string betContent { set; get; }
        public string betTime { set; get; }
        public string settledTime { set; get; }
        /// <summary>
        /// 取消时间
        /// </summary>
        public string cancelTime { set; get; }
        public int device { set; get; }
        public string platform { set; get; }
        public int status { set; get; }
        public string statusDesc { set; get; }
        public int currencyCode { set; get; }
        public string currencyDesc { set; get; }

        public int settled { set; get; }
        public string settledDesc { set; get; }
        public int fbOrderStatus { set; get; }

       public string confirmStatusDesc { set; get; }

    }
    /// <summary>
    /// 会员游戏平台日报表统计
    /// </summary>
    public class DailyGameReportModel
    {
        private string _userName;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string userName
        {
            set
            {
                //if (value.ToLower().StartsWith("boaa")
                //    || value.ToLower().StartsWith("bocc"))
                //{
                //    _userName = value.Remove(0, 4);
                //}
                //else
                //{
                //    _userName = value;
                //}
                _userName = value;
            }
            get { return _userName; }
        }

        private string _parents;
        /// <summary>
        /// 合营账号
        /// </summary>
        public string parents
        {
            set
            {
                //if (value.ToLower().StartsWith("boaa")
                //    || value.ToLower().StartsWith("bocc"))
                //{
                //    _parents = value.Remove(0, 4);
                //}
                //else
                //{
                //    _parents = value;
                //}
                _parents = value;
            }
            get { return _parents; }
        }
        private int _currencyCode;
        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencyCode
        {
            set
            {
                _currencyCode = value;
                currencyDesc = LangCodes.CurrencyMapLang[value].currency;
            }
            get { return _currencyCode; }
        }
        /// <summary>
        /// 货币代码
        /// </summary>
        public string currency { set; get; }
        public string currencyDesc { set; get; }
        /// <summary>
        /// 报表日期
        /// </summary>
        public string date { set; get; }
        /// <summary>
        /// 报表日期
        /// </summary>
        public string endDate { set; get; }
        /// <summary>
        /// 游戏ID
        /// </summary>
        public int gameId { set; get; }
        /// <summary>
        /// 游戏名
        /// </summary>
        public string gameName { set; get; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal betAmount { set; get; }
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal validAmount { set; get; }
        /// <summary>
        /// 派彩金额
        /// </summary>
        public decimal prizeAmount { set; get; }
        /// <summary>
        /// 输赢(会员视角)
        /// </summary>
        public decimal winLost { set; get; }
    }


    public class BetModel:BaseBetModel
    {
        private string _userName;
        /// <summary>
        /// 会员账号
        /// </summary>
        public string userName
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _userName = value.Remove(0, 4);
                }
                else
                {
                    _userName = value;
                }
            }
            get { return _userName; }
        }

        private string _parents;
        /// <summary>
        /// 合营账号
        /// </summary>
        public string parents
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _parents = value.Remove(0, 4);
                }
                else
                {
                    _parents = value;
                }
            }
            get { return _parents; }
        }

    }

    public class BetWithPrefixModel : BaseBetModel
    {
      
        /// <summary>
        /// 会员账号
        /// </summary>
        public string userName
        {
            set;
            get;
        }
        /// <summary>
        /// 合营账号
        /// </summary>
        public string parents
        {
            set;
            get;
        }

    }

    #endregion


    //"gameId": "fb",
    //  "totalBetAmount": 0,
    //  "betAmount": 0,
    //  "prizeAmount": 0,
    //  "winLost": 0,
    //  "currencyCode": 156,
    //  "currencyDesc": "UNKNOWN"


    /// <summary>
    /// 投注报表
    /// </summary>
    public class BetReportModel
    {
        /// <summary>
        /// 游戏id
        /// </summary>
        public string gameId { set; get; }
        /// <summary>
        /// 投注额
        /// </summary>
        public decimal? totalBetAmount { set; get; }
        /// <summary>
        /// 有效投注
        /// </summary>
        public decimal? betAmount { set; get; }
        public decimal? prizeAmount { set; get; }
        /// <summary>
        /// 输赢
        /// </summary>
        public decimal? winLost { set; get; }
        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencyCode { set; get; }
        /// <summary>
        /// 货币描述
        /// </summary>
        public string currencyDesc { set; get; } = "";

    }


}
