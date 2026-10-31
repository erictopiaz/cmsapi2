using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.TurnOver
{
   
    public class QueryTurnOverOutModel
    {
        public long tid { set; get; }
        public string account { set; get; }
        public decimal effectivebets { set; get; }
        public decimal reward { set; get; }
        public decimal bonuspoints { set; get; }
        public decimal winlost { set; get; }
        public decimal leftbets { set; get; }
        public string begindate { set; get; }
        public string enddate { set; get; }
        public string requesttime { set; get; }
        public int gametype { set; get; }
        public string gamename { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status {
            set
            {
                statusDesc = value == 1 ? "审核通过" : value == 0 ? "等待审核" : "取消审核";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string cancelreason { set; get; }
        public string cancelperson { set; get; }
        public string reviewperson { set; get; }
        public string affaccount { set; get; }
        public string calcDesc { set; get; }
        private decimal _calc;
        public decimal calc
        {
            set
            {
                calcDesc = string.Format(@"{0}‰", (value * 1000).ToString("F2"));
                _calc = value;
            }
            get
            {
                return _calc;
            }
        }
        private decimal _ecalc;
        public decimal eCalc
        {
            set
            {
                if(value>0)
                {
                    calcDesc = string.Format(@"({0}+{1})‰", (_calc * 1000).ToString("F2"), (value * 10).ToString("F2"));
                }
                _ecalc = value;
            }
            get
            {
                return _ecalc;
            }
        }
        public string pCalcDesc { set; get; }
        private decimal _pCalc;
        public decimal pCalc
        {
            set
            {
                pCalcDesc = string.Format(@"{0}‰", (value * 1000).ToString("F2"));
                _pCalc = value;
            }
            get
            {
                return _pCalc;
            }
        }
        public int turnOverType { set; get; }
        public string platformGameDesc { set; get; }
        /// <summary>
        /// 当前游戏平台，指定的游戏种类
        /// </summary>
        public int platformGame {
            set
            {
                platformGameDesc = GameName.gameTurnoverDict.ContainsKey(value) ? GameName.gameTurnoverDict[value] : "";
            }
        }
        public string levels { set; get; }
        public int level { set; get; }
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        /// <summary>
        /// 会员id
        /// </summary>
        public long uid { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }

    public class GameName
    {
        public static Dictionary<int, string> gameTurnoverDict = new Dictionary<int, string>()
        {
            { 1,"ssc" },
            { 2,"pk10" },
            { 3,"hk49" },
            { 4,"bc" },
            { 5,"电子" },
            { 6,"捕鱼" }
        };
    }
}
