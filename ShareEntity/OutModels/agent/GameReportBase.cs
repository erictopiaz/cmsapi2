using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;

namespace ShareEntity.OutModels.agent
{

    
    public class GameFee
    {
        /// <summary>
        /// 平台结算平台费用比例
        ///  20260711  pp, iaes
        /// 20250205  bti
        /// </summary>
        public static Dictionary<string, decimal> platfeeMap = new Dictionary<string, decimal>()
        {
            { "xj", 0.13m },
            { "ob", 0.12m },
            { "ysb", 0.1m },
            { "ag", 0.08m },
            { "bbin", 0.08m },
            { "ea", 0.08m },
            { "eb", 0.06m },
            { "bg", 0.06m },
            { "kg2", 0.06m },
            { "kg", 0.06m },
            { "vr", 0.06m },
            { "dbkeno", 0.06m },
            { "dscp", 0.08m },
            { "lb", 0.06m },
            { "ttg", 0.08m },
            { "pt", 0.12m },
            { "pg", 0.1m },
            { "imes", 0.12m },
            { "rg", 0.12m },
            { "ky", 0.08m },
            { "td", 0.05m },
            { "aw", 0.05m },
            { "cq", 0.07m },
            { "tf", 0.08m },
            { "kg3", 0.06m },
            { "obg", 0.10m },//obg 改名为pm
            { "pm", 0.10m },//obg 改名为pm
            { "obcv", 0.10m },
            { "imsp", 0.10m },
            { "sw", 0.06m },
            { "cr", 0.10m },
            { "fb", 0.10m },
            { "jdb", 0.08m },
            { "evo", 0.08m },
            { "db", 0.06m },
            { "rsg", 0.08m },
            { "bti", 0.08m },
            { "9w",0.10m },
            { "turbo",0.08m },
            { "jl", 0.08m },
            { "spribe",0.08m },
            { "pp",0.07m },
            { "iaes",0.09m }
        };
    }
    public class GameData
    {
        public string  gname { set; get; }
        public decimal bet { set; get; }
        public decimal winlost { set; get; }
        public decimal turnover { set; get; }
        public decimal p_p { set; get; }
        public decimal p_fee { set; get; }
        /// <summary>
        /// 是否发放了反水  1：是，0：否 , 默认为0 全部没有发放
        /// </summary>
        public int already { set; get; } = 0;
    }
    /// <summary>
    /// 20260711  pp, iaes
    /// 20230327  fb,jdb
    /// 20241116  rsg
    /// 20250205  bti
    /// </summary>
    public class GameReportBase : GameReportBase_
    {
        
        private string _account;
        /// <summary>
        /// 账号
        /// </summary>
        public string account
        {
            set
            {
                if (value.ToLower().StartsWith("boaa")
                    || value.ToLower().StartsWith("bocc"))
                {
                    _account = value.Remove(0, 4);
                }
                else
                {
                    _account = value;
                }
            }
            get { return _account; }
        }
        

    }


    /// <summary>
    /// 20260711  pp, iaes
    /// 20230327  fb,jdb
    /// 20241116  rsg
    /// 20250205  bti dbkeno
    /// </summary>
    public class GameReportBase_
    {
        public List<GameData> gameDatas { set; get; } = new List<GameData>();
        public long tid { set; get; }
        /// <summary>
        /// 平台手续费用
        /// </summary>
        public decimal totalGameFee { set; get; } = 0;

        #region iaes
        private decimal _iaes_bet;
        public decimal iaes_bet
        {
            set
            {
                _iaes_bet = value;
                var pm = gameDatas.Find(x => x.gname == "IAES");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IAES", bet = value });
            }
            get { return _iaes_bet; }
        }
        public decimal iaes_p { get; set; } = GameFee.platfeeMap["iaes"];
        private decimal _iaes_turnover;
        public decimal iaes_turnover
        {
            set
            {
                _iaes_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "IAES");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IAES", turnover = value });
            }
            get { return _iaes_turnover; }
        }
        private decimal _iaes_winlost;
        public decimal iaes_winlost
        {
            set
            {
                iaes_fee = Convert.ToDecimal((iaes_p * value).ToString("F2"));
                totalGameFee += iaes_fee > 0 ? iaes_fee : 0;
                _iaes_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "IAES");
                if (pm != null)
                {
                    pm.winlost = _iaes_winlost;
                    pm.p_p = iaes_p;
                    pm.p_fee = iaes_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "IAES", winlost = _iaes_winlost, p_p = iaes_p, p_fee = iaes_fee });
            }
            get { return _iaes_winlost; }
        }
        public decimal iaes_fee
        {
            set; get;
        }

        #endregion


        #region pp
        private decimal _pp_bet;
        public decimal pp_bet
        {
            set
            {
                _pp_bet = value;
                var pm = gameDatas.Find(x => x.gname == "PP");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PP", bet = value });
            }
            get { return _pp_bet; }
        }
        public decimal pp_p { get; set; } = GameFee.platfeeMap["pp"];
        private decimal _pp_turnover;
        public decimal pp_turnover
        {
            set
            {
                _pp_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "PP");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PP", turnover = value });
            }
            get { return _pp_turnover; }
        }
        private decimal _pp_winlost;
        public decimal pp_winlost
        {
            set
            {
                pp_fee = Convert.ToDecimal((pp_p * value).ToString("F2"));
                totalGameFee += pp_fee > 0 ? pp_fee : 0;
                _pp_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "PP");
                if (pm != null)
                {
                    pm.winlost = _pp_winlost;
                    pm.p_p = pp_p;
                    pm.p_fee = pp_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "PP", winlost = _pp_winlost, p_p = pp_p, p_fee = pp_fee });
            }
            get { return _pp_winlost; }
        }
        public decimal pp_fee
        {
            set; get;
        }

        #endregion


        #region xj
        private decimal _xj_bet;
        public decimal xj_bet
        {
            set
            {
                _xj_bet = value;
                var pm = gameDatas.Find(x => x.gname == "XJ");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "XJ", bet = value });
            }
            get { return _xj_bet; }
        }
        public decimal xj_p { get; set; } = GameFee.platfeeMap["xj"];
        private decimal _xj_turnover;
        public decimal xj_turnover
        {
            set
            {
                _xj_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "XJ");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "XJ", turnover = value });
            }
            get { return _xj_turnover; }
        }
        private decimal _xj_winlost;
        public decimal xj_winlost
        {
            set
            {
                xj_fee = Convert.ToDecimal((xj_p * value).ToString("F2"));
                totalGameFee += xj_fee > 0 ? xj_fee : 0;
                _xj_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "XJ");
                if (pm != null)
                {
                    pm.winlost = _xj_winlost;
                    pm.p_p = xj_p;
                    pm.p_fee = xj_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "XJ", winlost = _xj_winlost, p_p = xj_p, p_fee = xj_fee });
            }
            get { return _xj_winlost; }
        }
        public decimal xj_fee
        {
            set; get;
        }

        #endregion

        #region dbkeno
        private decimal _dbkeno_bet;
        public decimal dbkeno_bet
        {
            set
            {
                _dbkeno_bet = value;
                var pm = gameDatas.Find(x => x.gname == "DBKeno");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "DBKeno", bet = value });
            }
            get { return _dbkeno_bet; }
        }
        public decimal dbkeno_p { get; set; } = GameFee.platfeeMap["dbkeno"];
        private decimal _dbkeno_turnover;
        public decimal dbkeno_turnover
        {
            set
            {
                _dbkeno_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "DBKeno");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "DBKeno", turnover = value });
            }
            get { return _dbkeno_turnover; }
        }
        private decimal _dbkeno_winlost;
        public decimal dbkeno_winlost
        {
            set
            {
                dbkeno_fee = Convert.ToDecimal((dbkeno_p * value).ToString("F2"));
                totalGameFee += dbkeno_fee > 0 ? dbkeno_fee : 0;
                _dbkeno_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "DBKeno");
                if (pm != null)
                {
                    pm.winlost = _dbkeno_winlost;
                    pm.p_p = dbkeno_p;
                    pm.p_fee = dbkeno_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "DBKeno", winlost = _dbkeno_winlost, p_p = dbkeno_p, p_fee = dbkeno_fee });
            }
            get { return _dbkeno_winlost; }
        }
        public decimal dbkeno_fee
        {
            set; get;
        }

        #endregion

        #region bti 开启
        private decimal _bti_bet;
        public decimal bti_bet
        {
            set
            {
                _bti_bet = value;
                var pm = gameDatas.Find(x => x.gname == "BTI");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "BTI", bet = value });
            }
            get { return _bti_bet; }
        }
        public decimal bti_p { get; set; } = GameFee.platfeeMap["bti"];
        private decimal _bti_turnover;
        public decimal bti_turnover
        {
            set
            {
                _bti_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "BTI");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "BTI", turnover = value });
            }
            get { return _bti_turnover; }
        }
        private decimal _bti_winlost;
        public decimal bti_winlost
        {
            set
            {
                bti_fee = Convert.ToDecimal((bti_p * value).ToString("F2"));
                totalGameFee += bti_fee > 0 ? bti_fee : 0;
                _bti_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "BTI");
                if (pm != null)
                {
                    pm.winlost = _bti_winlost;
                    pm.p_p = bti_p;
                    pm.p_fee = bti_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "BTI", winlost = _bti_winlost, p_p = bti_p, p_fee = bti_fee });

            }
            get { return _bti_winlost; }
        }
        public decimal bti_fee
        {
            set; get;
        }
        #endregion

        #region rsg 开启
        private decimal _rsg_bet;
        public decimal rsg_bet
        {
            set
            {
                _rsg_bet = value;
                var pm = gameDatas.Find(x => x.gname == "RSG");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "RSG", bet = value });
            }
            get { return _rsg_bet; }
        }
        public decimal rsg_p { get; set; } = GameFee.platfeeMap["rsg"];
        private decimal _rsg_turnover;
        public decimal rsg_turnover
        {
            set
            {
                _rsg_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "RSG");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "RSG", turnover = value });
            }
            get { return _rsg_turnover; }
        }
        private decimal _rsg_winlost;
        public decimal rsg_winlost
        {
            set
            {
                rsg_fee = Convert.ToDecimal((rsg_p * value).ToString("F2"));
                totalGameFee += rsg_fee > 0 ? rsg_fee : 0;
                _rsg_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "RSG");
                if (pm != null)
                {
                    pm.winlost = _rsg_winlost;
                    pm.p_p = rsg_p;
                    pm.p_fee = rsg_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "RSG", winlost = _rsg_winlost, p_p = rsg_p, p_fee = rsg_fee });

            }
            get { return _rsg_winlost; }
        }
        public decimal rsg_fee
        {
            set; get;
        }
        #endregion
        #region bbin 开启
        private decimal _bbin_bet;
        public decimal bbin_bet
        {
            set
            {
                _bbin_bet = value;
                var pm = gameDatas.Find(x => x.gname == "BBIN");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "BBIN", bet = value });
            }
            get { return _bbin_bet; }
        }
        public decimal bbin_p { get; set; } = GameFee.platfeeMap["bbin"];
        private decimal _bbin_turnover;
        public decimal bbin_turnover
        {
            set
            {
                _bbin_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "BBIN");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "BBIN", turnover = value });
            }
            get { return _bbin_turnover; }
        }
        private decimal _bbin_winlost;
        public decimal bbin_winlost
        {
            set
            {
                bbin_fee = Convert.ToDecimal((bbin_p * value).ToString("F2"));
                totalGameFee += bbin_fee > 0 ? bbin_fee : 0;
                _bbin_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "BBIN");
                if (pm != null)
                {
                    pm.winlost = _bbin_winlost;
                    pm.p_p = bbin_p;
                    pm.p_fee = bbin_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "BBIN", winlost = _bbin_winlost, p_p = bbin_p, p_fee = bbin_fee });

            }
            get { return _bbin_winlost; }
        }
        public decimal bbin_fee
        {
            set; get;
        }
        #endregion

        #region ttg 开启
        private decimal _ttg_bet;
        public decimal ttg_bet
        {
            set
            {
                _ttg_bet = value;
                var pm = gameDatas.Find(x => x.gname == "TTG");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TTG", bet = value });
            }
            get { return _ttg_bet; }
        }
        public decimal ttg_p { get; set; } = GameFee.platfeeMap["ttg"];
        private decimal _ttg_turnover;
        public decimal ttg_turnover
        {
            set
            {
                _ttg_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "TTG");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TTG", turnover = value });
            }
            get { return _ttg_turnover; }
        }
        private decimal _ttg_winlost;
        public decimal ttg_winlost
        {
            set
            {
                ttg_fee = Convert.ToDecimal((ttg_p * value).ToString("F2"));
                totalGameFee += ttg_fee > 0 ? ttg_fee : 0;
                _ttg_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "TTG");
                if (pm != null)
                {
                    pm.winlost = _ttg_winlost;
                    pm.p_p = ttg_p;
                    pm.p_fee = ttg_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "TTG", winlost = _ttg_winlost, p_p = ttg_p, p_fee = ttg_fee });
            }
            get { return _ttg_winlost; }
        }
        public decimal ttg_fee
        {
            set; get;
        }
        #endregion

        #region lb 开启
        private decimal _lb_bet;
        public decimal lb_bet
        {
            set
            {
                _lb_bet = value;
                var pm = gameDatas.Find(x => x.gname == "LB");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "LB", bet = value });
            }
            get { return _lb_bet; }
        }
        public decimal lb_p { get; set; } = GameFee.platfeeMap["lb"];
        private decimal _lb_turnover;
        public decimal lb_turnover
        {
            set
            {
                _lb_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "LB");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "LB", turnover = value });
            }
            get { return _lb_turnover; }
        }
        private decimal _lb_winlost;
        public decimal lb_winlost
        {
            set
            {
                lb_fee = Convert.ToDecimal((lb_p * value).ToString("F2"));
                totalGameFee += lb_fee > 0 ? lb_fee : 0;
                _lb_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "LB");
                if (pm != null)
                {
                    pm.winlost = _lb_winlost;
                    pm.p_p = lb_p;
                    pm.p_fee = lb_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "LB", winlost = _lb_winlost, p_p = lb_p, p_fee = lb_fee });
            }
            get { return _lb_winlost; }
        }
        public decimal lb_fee
        {
            set; get;
        }

        #endregion

        #region ag 开启中
        private decimal _ag_bet;
        public decimal ag_bet
        {
            set
            {
                _ag_bet = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", bet = value });
            }
            get { return _ag_bet; }
        }
        public decimal ag_p { get; set; } = GameFee.platfeeMap["ag"];
        private decimal _ag_turnover;
        public decimal ag_turnover
        {
            set
            {
                _ag_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", turnover = value });
            }
            get { return _ag_turnover; }
        }
        private decimal _ag_winlost;
        public decimal ag_winlost
        {
            set
            {
                ag_fee = Convert.ToDecimal((ag_p * value).ToString("F2"));
                totalGameFee += ag_fee > 0 ? ag_fee : 0;
                _ag_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null)
                {
                    pm.winlost = _ag_winlost;
                    pm.p_p = ag_p;
                    pm.p_fee = ag_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", winlost = _ag_winlost, p_p = ag_p, p_fee = ag_fee });

            }
            get { return _ag_winlost; }
        }
        public decimal ag_fee
        {
            set; get;
        }

        #endregion
        #region vr 开启
        private decimal _vr_bet;
        public decimal vr_bet
        {
            set
            {
                _vr_bet = value;
                var pm = gameDatas.Find(x => x.gname == "VR");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "VR", bet = value });
            }
            get { return _vr_bet; }
        }
        public decimal vr_p { get; set; } = GameFee.platfeeMap["vr"];
        private decimal _vr_turnover;
        public decimal vr_turnover
        {
            set
            {
                _vr_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "VR");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "VR", turnover = value });
            }
            get { return _vr_turnover; }
        }
        private decimal _vr_winlost;
        public decimal vr_winlost
        {
            set
            {
                vr_fee = Convert.ToDecimal((vr_p * value).ToString("F2"));
                totalGameFee += vr_fee > 0 ? vr_fee : 0;
                _vr_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "VR");
                if (pm != null)
                {
                    pm.winlost = _vr_winlost;
                    pm.p_p = vr_p;
                    pm.p_fee = vr_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "VR", winlost = _vr_winlost, p_p = vr_p, p_fee = vr_fee });

                //gameDatas.Add(new GameData { gname = "VR", bet = vr_bet, winlost = _vr_winlost, turnover = vr_turnover, p_p = vr_p, p_fee = vr_fee });

            }
            get { return _vr_winlost; }
        }
        public decimal vr_fee
        {
            set; get;
        }
        #endregion
        #region pg 开启中

        private decimal _pg_bet;
        public decimal pg_bet
        {
            set
            {
                _pg_bet = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", bet = value });
            }
            get { return _pg_bet; }
        }
        public decimal pg_p { get; set; } = GameFee.platfeeMap["pg"];
        private decimal _pg_turnover;
        public decimal pg_turnover
        {
            set
            {
                _pg_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", turnover = value });
            }
            get { return _pg_turnover; }
        }
        private decimal _pg_winlost;
        public decimal pg_winlost
        {
            set
            {
                pg_fee = Convert.ToDecimal((pg_p * value).ToString("F2"));
                totalGameFee += pg_fee > 0 ? pg_fee : 0;
                _pg_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null)
                {
                    pm.winlost = _pg_winlost;
                    pm.p_p = pg_p;
                    pm.p_fee = pg_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", winlost = _pg_winlost, p_p = pg_p, p_fee = pg_fee });
                //gameDatas.Add(new GameData { gname = "PG", bet = pg_bet, winlost = _pg_winlost, turnover = pg_turnover, p_p = pg_p, p_fee = pg_fee });

            }
            get { return _pg_winlost; }
        }
        public decimal pg_fee
        {
            set; get;
        }

        #endregion

        #region imes 开启
        private decimal _imes_bet;
        public decimal imes_bet
        {
            set
            {
                _imes_bet = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", bet = value });
            }
            get { return _imes_bet; }
        }
        public decimal imes_p { get; set; } = GameFee.platfeeMap["imes"];
        private decimal _imes_turnover;
        public decimal imes_turnover
        {
            set
            {
                _imes_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", turnover = value });
            }
            get { return _imes_turnover; }
        }
        private decimal _imes_winlost;
        public decimal imes_winlost
        {
            set
            {
                imes_fee = Convert.ToDecimal((imes_p * value).ToString("F2"));
                totalGameFee += imes_fee > 0 ? imes_fee : 0;
                _imes_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null)
                {
                    pm.winlost = _imes_winlost;
                    pm.p_p = imes_p;
                    pm.p_fee = imes_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", winlost = _imes_winlost, p_p = imes_p, p_fee = imes_fee });
            }
            get { return _imes_winlost; }
        }
        public decimal imes_fee
        {
            set; get;
        }
        #endregion
        #region ob 开启
        private decimal _ob_bet;
        public decimal ob_bet
        {
            set
            {
                _ob_bet = value;
                var pm = gameDatas.Find(x => x.gname == "OB");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "OB", bet = value });
            }
            get { return _ob_bet; }
        }
        public decimal ob_p { get; set; } = GameFee.platfeeMap["ob"];
        private decimal _ob_turnover;
        public decimal ob_turnover
        {
            set
            {
                _ob_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "OB");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "OB", turnover = value });
            }
            get { return _ob_turnover; }
        }
        private decimal _ob_winlost;
        public decimal ob_winlost
        {
            set
            {
                ob_fee = Convert.ToDecimal((ob_p * value).ToString("F2"));
                totalGameFee += ob_fee > 0 ? ob_fee : 0;
                _ob_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "OB");
                if (pm != null)
                {
                    pm.winlost = _ob_winlost;
                    pm.p_p = ob_p;
                    pm.p_fee = ob_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "OB", winlost = _ob_winlost, p_p = ob_p, p_fee = ob_fee });
            }
            get { return _ob_winlost; }
        }
        public decimal ob_fee
        {
            set; get;
        }

        #endregion
        #region ky 开启
        private decimal _ky_bet;
        public decimal ky_bet
        {
            set
            {
                _ky_bet = value;
                var pm = gameDatas.Find(x => x.gname == "KY");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "KY", bet = value });
            }
            get { return _ky_bet; }
        }
        public decimal ky_p { get; set; } = GameFee.platfeeMap["ky"];
        private decimal _ky_turnover;
        public decimal ky_turnover
        {
            set
            {
                _ky_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "KY");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "KY", turnover = value });
            }
            get { return _ky_turnover; }
        }
        private decimal _ky_winlost;
        public decimal ky_winlost
        {
            set
            {
                ky_fee = Convert.ToDecimal((ky_p * value).ToString("F2"));
                totalGameFee += ky_fee > 0 ? ky_fee : 0;
                _ky_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "KY");
                if (pm != null)
                {
                    pm.winlost = _ky_winlost;
                    pm.p_p = ky_p;
                    pm.p_fee = ky_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "KY", winlost = _ky_winlost, p_p = ky_p, p_fee = ky_fee });
            }
            get { return _ky_winlost; }
        }
        public decimal ky_fee
        {
            set; get;
        }
        #endregion


        #region tf 开启
        private decimal _tf_bet;
        public decimal tf_bet
        {
            set
            {
                _tf_bet = value;
                var pm = gameDatas.Find(x => x.gname == "TF");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TF", bet = value });
            }
            get { return _tf_bet; }
        }
        public decimal tf_p { get; set; } = GameFee.platfeeMap["tf"];
        private decimal _tf_turnover;
        public decimal tf_turnover
        {
            set
            {
                _tf_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "TF");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TF", turnover = value });
            }
            get { return _tf_turnover; }
        }
        private decimal _tf_winlost;
        public decimal tf_winlost
        {
            set
            {
                tf_fee = Convert.ToDecimal((tf_p * value).ToString("F2"));
                totalGameFee += tf_fee > 0 ? tf_fee : 0;
                _tf_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "TF");
                if (pm != null)
                {
                    pm.winlost = _tf_winlost;
                    pm.p_p = tf_p;
                    pm.p_fee = tf_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "TF", winlost = _tf_winlost, p_p = tf_p, p_fee = tf_fee });
            }
            get { return _tf_winlost; }
        }
        public decimal tf_fee
        {
            set; get;
        }
        #endregion

        #region aw 开启
        private decimal _aw_bet;
        public decimal aw_bet
        {
            set
            {
                _aw_bet = value;
                var pm = gameDatas.Find(x => x.gname == "AW");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AW", bet = value });
            }
            get { return _aw_bet; }
        }
        public decimal aw_p { get; set; } = GameFee.platfeeMap["aw"];
        private decimal _aw_turnover;
        public decimal aw_turnover
        {
            set
            {
                _aw_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "AW");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AW", turnover = value });
            }
            get { return _aw_turnover; }
        }
        private decimal _aw_winlost;
        public decimal aw_winlost
        {
            set
            {
                aw_fee = Convert.ToDecimal((aw_p * value).ToString("F2"));
                totalGameFee += aw_fee > 0 ? aw_fee : 0;
                _aw_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "AW");
                if (pm != null)
                {
                    pm.winlost = _aw_winlost;
                    pm.p_p = aw_p;
                    pm.p_fee = aw_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "AW", winlost = _aw_winlost, p_p = aw_p, p_fee = aw_fee });
            }
            get { return _aw_winlost; }
        }
        public decimal aw_fee
        {
            set; get;
        }
        #endregion
        #region kg3
        private decimal _kg3_bet;
        public decimal kg3_bet
        {
            set
            {
                _kg3_bet = value;
                var pm = gameDatas.Find(x => x.gname == "KG3");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "KG3", bet = value });
            }
            get { return _kg3_bet; }
        }
        public decimal kg3_p { get; set; } = GameFee.platfeeMap["kg3"];

        private decimal _kg3_turnover;
        public decimal kg3_turnover
        {
            set
            {
                _kg3_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "KG3");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "KG3", turnover = value });
            }
            get { return _kg3_turnover; }
        }
        private decimal _kg3_winlost;
        public decimal kg3_winlost
        {
            set
            {
                kg3_fee = Convert.ToDecimal((kg3_p * value).ToString("F2"));
                totalGameFee += kg3_fee > 0 ? kg3_fee : 0;
                _kg3_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "KG3");
                if (pm != null)
                {
                    pm.winlost = _kg3_winlost;
                    pm.p_p = kg3_p;
                    pm.p_fee = kg3_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "KG3", winlost = _kg3_winlost, p_p = kg3_p, p_fee = kg3_fee });
            }
            get { return _kg3_winlost; }
        }
        public decimal kg3_fee
        {
            set; get;
        }

        #endregion

        #region obg 后改名为 pm 了
        private decimal _pm_bet;
        public decimal pm_bet
        {
            set
            {
                _pm_bet = value;
                var pm = gameDatas.Find(x => x.gname == "PM");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PM", bet = value });
            }
            get { return _pm_bet; }
        }
        public decimal pm_p { get; set; } = GameFee.platfeeMap["pm"];
        private decimal _pm_turnover;
        public decimal pm_turnover
        {
            set
            {
                _pm_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "PM");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PM", turnover = value });
            }
            get { return _pm_turnover; }
        }
        private decimal _pm_winlost;
        public decimal pm_winlost
        {
            set
            {
                pm_fee = Convert.ToDecimal((pm_p * value).ToString("F2"));
                totalGameFee += pm_fee > 0 ? pm_fee : 0;
                _pm_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "PM");
                if (pm != null)
                {
                    pm.winlost = _pm_winlost;
                    pm.p_p = pm_p;
                    pm.p_fee = pm_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "PM", winlost = _pm_winlost, p_p = pm_p, p_fee = pm_fee });
            }
            get { return _pm_winlost; }
        }
        public decimal pm_fee
        {
            set; get;
        }

        #endregion


        #region imsp
        private decimal _imsp_bet;
        public decimal imsp_bet
        {
            set
            {
                _imsp_bet = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", bet = value });
            }
            get { return _imsp_bet; }
        }
        public decimal imsp_p { get; set; } = GameFee.platfeeMap["imsp"];
        private decimal _imsp_turnover;
        public decimal imsp_turnover
        {
            set
            {
                _imsp_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", turnover = value });
            }
            get { return _imsp_turnover; }
        }
        private decimal _imsp_winlost;
        public decimal imsp_winlost
        {
            set
            {
                imsp_fee = Convert.ToDecimal((imsp_p * value).ToString("F2"));
                totalGameFee += imsp_fee > 0 ? imsp_fee : 0;
                _imsp_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null)
                {
                    pm.winlost = _imsp_winlost;
                    pm.p_p = imsp_p;
                    pm.p_fee = imsp_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", winlost = _imsp_winlost, p_p = imsp_p, p_fee = imsp_fee });
            }
            get { return _imsp_winlost; }
        }
        public decimal imsp_fee
        {
            set; get;
        }

        #endregion

        

        #region cr
        private decimal _cr_bet;
        public decimal cr_bet
        {
            set
            {
                _cr_bet = value;
                var pm = gameDatas.Find(x => x.gname == "CR");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "CR", bet = value });
            }
            get { return _cr_bet; }
        }
        public decimal cr_p { get; set; } = GameFee.platfeeMap["cr"];
        private decimal _cr_turnover;
        public decimal cr_turnover
        {
            set
            {
                _cr_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "CR");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "CR", turnover = value });
            }
            get { return _cr_turnover; }
        }
        private decimal _cr_winlost;
        public decimal cr_winlost
        {
            set
            {
                cr_fee = Convert.ToDecimal((cr_p * value).ToString("F2"));
                totalGameFee += cr_fee > 0 ? cr_fee : 0;
                _cr_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "CR");
                if (pm != null)
                {
                    pm.winlost = _cr_winlost;
                    pm.p_p = cr_p;
                    pm.p_fee = cr_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "CR", winlost = _cr_winlost, p_p = cr_p, p_fee = cr_fee });
            }
            get { return _cr_winlost; }
        }
        public decimal cr_fee
        {
            set; get;
        }

        #endregion

        #region obcv
        private decimal _obcv_bet;
        public decimal obcv_bet
        {
            set
            {
                _obcv_bet = value;
                var pm = gameDatas.Find(x => x.gname == "OBCV");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "OBCV", bet = value });
            }
            get { return _obcv_bet; }
        }
        public decimal obcv_p { get; set; } = GameFee.platfeeMap["obcv"];
        private decimal _obcv_turnover;
        public decimal obcv_turnover
        {
            set
            {
                _obcv_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "OBCV");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "OBCV", turnover = value });
            }
            get { return _obcv_turnover; }
        }
        private decimal _obcv_winlost;
        public decimal obcv_winlost
        {
            set
            {
                obcv_fee = Convert.ToDecimal((obcv_p * value).ToString("F2"));
                totalGameFee += obcv_fee > 0 ? obcv_fee : 0;
                _obcv_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "OBCV");
                if (pm != null)
                {
                    pm.winlost = _obcv_winlost;
                    pm.p_p = obcv_p;
                    pm.p_fee = obcv_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "OBCV", winlost = _obcv_winlost, p_p = obcv_p, p_fee = obcv_fee });
            }
            get { return _obcv_winlost; }
        }
        public decimal obcv_fee
        {
            set; get;
        }

        #endregion

        #region fb
        private decimal _fb_bet;
        public decimal fb_bet
        {
            set
            {
                _fb_bet = value;
                var pm = gameDatas.Find(x => x.gname == "FB");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "FB", bet = value });
            }
            get { return _fb_bet; }
        }
        public decimal fb_p { get; set; } = GameFee.platfeeMap["fb"];
        private decimal _fb_turnover;
        public decimal fb_turnover
        {
            set
            {
                _fb_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "FB");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "FB", turnover = value });
            }
            get { return _fb_turnover; }
        }
        private decimal _fb_winlost;
        public decimal fb_winlost
        {
            set
            {
                fb_fee = Convert.ToDecimal((fb_p * value).ToString("F2"));
                totalGameFee += fb_fee > 0 ? fb_fee : 0;
                _fb_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "FB");
                if (pm != null)
                {
                    pm.winlost = _fb_winlost;
                    pm.p_p = fb_p;
                    pm.p_fee = fb_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "FB", winlost = _fb_winlost, p_p = fb_p, p_fee = fb_fee });
            }
            get { return _fb_winlost; }
        }
        public decimal fb_fee
        {
            set; get;
        }

        #endregion

        #region jdb
        private decimal _jdb_bet;
        public decimal jdb_bet
        {
            set
            {
                _jdb_bet = value;
                var pm = gameDatas.Find(x => x.gname == "JDB");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "JDB", bet = value });
            }
            get { return _jdb_bet; }
        }
        public decimal jdb_p { get; set; } = GameFee.platfeeMap["jdb"];
        private decimal _jdb_turnover;
        public decimal jdb_turnover
        {
            set
            {
                _jdb_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "JDB");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "JDB", turnover = value });
            }
            get { return _jdb_turnover; }
        }
        private decimal _jdb_winlost;
        public decimal jdb_winlost
        {
            set
            {
                jdb_fee = Convert.ToDecimal((jdb_p * value).ToString("F2"));
                totalGameFee += jdb_fee > 0 ? jdb_fee : 0;
                _jdb_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "JDB");
                if (pm != null)
                {
                    pm.winlost = _jdb_winlost;
                    pm.p_p = jdb_p;
                    pm.p_fee = jdb_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "JDB", winlost = _jdb_winlost, p_p = jdb_p, p_fee = jdb_fee });
            }
            get { return _jdb_winlost; }
        }
        public decimal jdb_fee
        {
            set; get;
        }

        #endregion

        #region db
        private decimal _db_bet;
        public decimal db_bet
        {
            set
            {
                _db_bet = value;
                var pm = gameDatas.Find(x => x.gname == "DB");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "DB", bet = value });
            }
            get { return _db_bet; }
        }
        public decimal db_p { get; set; } = GameFee.platfeeMap["db"];
        private decimal _db_turnover;
        public decimal db_turnover
        {
            set
            {
                _db_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "DB");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "DB", turnover = value });
            }
            get { return _db_turnover; }
        }
        private decimal _db_winlost;
        public decimal db_winlost
        {
            set
            {
                db_fee = Convert.ToDecimal((db_p * value).ToString("F2"));
                totalGameFee += db_fee > 0 ? db_fee : 0;
                _db_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "DB");
                if (pm != null)
                {
                    pm.winlost = _db_winlost;
                    pm.p_p = db_p;
                    pm.p_fee = db_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "DB", winlost = _db_winlost, p_p = db_p, p_fee = db_fee });
            }
            get { return _db_winlost; }
        }
        public decimal db_fee
        {
            set; get;
        }

        #endregion

        #region evo
        private decimal _evo_bet;
        public decimal evo_bet
        {
            set
            {
                _evo_bet = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", bet = value });
            }
            get { return _evo_bet; }
        }
        public decimal evo_p { get; set; } = GameFee.platfeeMap["evo"];
        private decimal _evo_turnover;
        public decimal evo_turnover
        {
            set
            {
                _evo_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", turnover = value });
            }
            get { return _evo_turnover; }
        }
        private decimal _evo_winlost;
        public decimal evo_winlost
        {
            set
            {
                evo_fee = Convert.ToDecimal((evo_p * value).ToString("F2"));
                totalGameFee += evo_fee > 0 ? evo_fee : 0;
                _evo_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null)
                {
                    pm.winlost = _evo_winlost;
                    pm.p_p = evo_p;
                    pm.p_fee = evo_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", winlost = _evo_winlost, p_p = evo_p, p_fee = evo_fee });
            }
            get { return _evo_winlost; }
        }
        public decimal evo_fee
        {
            set; get;
        }

        #endregion

        #region spribe
        private decimal _spribe_bet;
        public decimal spribe_bet
        {
            set
            {
                _spribe_bet = value;
                var pm = gameDatas.Find(x => x.gname == "SPRIBE");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "SPRIBE", bet = value });
            }
            get { return _spribe_bet; }
        }
        public decimal spribe_p { get; set; } = GameFee.platfeeMap["spribe"];
        private decimal _spribe_turnover;
        public decimal spribe_turnover
        {
            set
            {
                _spribe_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "SPRIBE");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "SPRIBE", turnover = value });
            }
            get { return _spribe_turnover; }
        }
        private decimal _spribe_winlost;
        public decimal spribe_winlost
        {
            set
            {
                spribe_fee = Convert.ToDecimal((spribe_p * value).ToString("F2"));
                totalGameFee += spribe_fee > 0 ? spribe_fee : 0;
                _spribe_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "SPRIBE");
                if (pm != null)
                {
                    pm.winlost = _spribe_winlost;
                    pm.p_p = spribe_p;
                    pm.p_fee = spribe_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "SPRIBE", winlost = _spribe_winlost, p_p = spribe_p, p_fee = spribe_fee });
            }
            get { return _spribe_winlost; }
        }
        public decimal spribe_fee
        {
            set; get;
        }

        #endregion
        #region turbo
        private decimal _turbo_bet;
        public decimal turbo_bet
        {
            set
            {
                _turbo_bet = value;
                var pm = gameDatas.Find(x => x.gname == "TURBO");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TURBO", bet = value });
            }
            get { return _turbo_bet; }
        }
        public decimal turbo_p { get; set; } = GameFee.platfeeMap["turbo"];
        private decimal _turbo_turnover;
        public decimal turbo_turnover
        {
            set
            {
                _turbo_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "TURBO");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "TURBO", turnover = value });
            }
            get { return _turbo_turnover; }
        }
        private decimal _turbo_winlost;
        public decimal turbo_winlost
        {
            set
            {
                turbo_fee = Convert.ToDecimal((turbo_p * value).ToString("F2"));
                totalGameFee += turbo_fee > 0 ? turbo_fee : 0;
                _turbo_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "TURBO");
                if (pm != null)
                {
                    pm.winlost = _turbo_winlost;
                    pm.p_p = turbo_p;
                    pm.p_fee = turbo_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "TURBO", winlost = _turbo_winlost, p_p = turbo_p, p_fee = turbo_fee });
            }
            get { return _turbo_winlost; }
        }
        public decimal turbo_fee
        {
            set; get;
        }

        #endregion

        #region 9w 别名  b9w
        private decimal _b9w_bet;
        public decimal b9w_bet
        {
            set
            {
                _b9w_bet = value;
                var pm = gameDatas.Find(x => x.gname == "9W");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "9W", bet = value });
            }
            get { return _b9w_bet; }
        }
        public decimal b9w_p { get; set; } = GameFee.platfeeMap["9w"];
        private decimal _b9w_turnover;
        public decimal b9w_turnover
        {
            set
            {
                _b9w_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "9W");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "9W", turnover = value });
            }
            get { return _b9w_turnover; }
        }
        private decimal _b9w_winlost;
        public decimal b9w_winlost
        {
            set
            {
                b9w_fee = Convert.ToDecimal((b9w_p * value).ToString("F2"));
                totalGameFee += b9w_fee > 0 ? b9w_fee : 0;
                _b9w_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "9W");
                if (pm != null)
                {
                    pm.winlost = _b9w_winlost;
                    pm.p_p = b9w_p;
                    pm.p_fee = b9w_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "9W", winlost = _b9w_winlost, p_p = b9w_p, p_fee = b9w_fee });
            }
            get { return _b9w_winlost; }
        }
        public decimal b9w_fee
        {
            set; get;
        }

        #endregion

        #region jl
        private decimal _jl_bet;
        public decimal jl_bet
        {
            set
            {
                _jl_bet = value;
                var pm = gameDatas.Find(x => x.gname == "JL");
                if (pm != null)
                {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "JL", bet = value });
            }
            get { return _jl_bet; }
        }
        public decimal jl_p { get; set; } = GameFee.platfeeMap["jl"];
        private decimal _jl_turnover;
        public decimal jl_turnover
        {
            set
            {
                _jl_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "JL");
                if (pm != null)
                {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "JL", turnover = value });
            }
            get { return _jl_turnover; }
        }
        private decimal _jl_winlost;
        public decimal jl_winlost
        {
            set
            {
                jl_fee = Convert.ToDecimal((jl_p * value).ToString("F2"));
                totalGameFee += jl_fee > 0 ? jl_fee : 0;
                _jl_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "JL");
                if (pm != null)
                {
                    pm.winlost = _jl_winlost;
                    pm.p_p = jl_p;
                    pm.p_fee = jl_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "JL", winlost = _jl_winlost, p_p = jl_p, p_fee = jl_fee });
            }
            get { return _jl_winlost; }
        }
        public decimal jl_fee
        {
            set; get;
        }

        #endregion

       
        public string levelDesc { set; get; }
        private int _isLeaf;
        /// <summary>
        /// 1: 有子
        /// 0：无子
        /// </summary>
        public int isLeaf
        {
            set
            {
                levelDesc = value == 1 ? "双击展开" : "";
                _isLeaf = value;
            }
            get
            {
                return _isLeaf;
            }
        }


        public int id { set; get; }
        public int pId { set; get; }

    }


    /// <summary>
    /// 20230327  fb,jdb
    /// </summary>
    public class GameReportBaseIntl {
        public List<GameData> gameDatas { set; get; } = new List<GameData>();
        public long tid { set; get; }
        public decimal totalGameFee { set; get; } = 0;
        public string account { get; set; }

        
        #region ag 开启中
        private decimal _ag_bet;
        public decimal ag_bet {
            set {
                _ag_bet = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null) {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", bet = value });
            }
            get { return _ag_bet; }
        }
        public decimal ag_p { get; set; } = GameFee.platfeeMap["ag"];
        private decimal _ag_turnover;
        public decimal ag_turnover {
            set {
                _ag_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null) {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", turnover = value });
            }
            get { return _ag_turnover; }
        }
        private decimal _ag_winlost;
        public decimal ag_winlost {
            set {
                ag_fee = Convert.ToDecimal((ag_p * value).ToString("F2"));
                totalGameFee += ag_fee > 0 ? ag_fee : 0;
                _ag_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "AG");
                if (pm != null) {
                    pm.winlost = _ag_winlost;
                    pm.p_p = ag_p;
                    pm.p_fee = ag_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "AG", winlost = _ag_winlost, p_p = ag_p, p_fee = ag_fee });

            }
            get { return _ag_winlost; }
        }
        public decimal ag_fee {
            set; get;
        }

        #endregion


        
        #region pg 开启中

        private decimal _pg_bet;
        public decimal pg_bet {
            set {
                _pg_bet = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null) {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", bet = value });
            }
            get { return _pg_bet; }
        }
        public decimal pg_p { get; set; } = GameFee.platfeeMap["pg"];
        private decimal _pg_turnover;
        public decimal pg_turnover {
            set {
                _pg_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null) {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", turnover = value });
            }
            get { return _pg_turnover; }
        }
        private decimal _pg_winlost;
        public decimal pg_winlost {
            set {
                pg_fee = Convert.ToDecimal((pg_p * value).ToString("F2"));
                totalGameFee += pg_fee > 0 ? pg_fee : 0;
                _pg_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "PG");
                if (pm != null) {
                    pm.winlost = _pg_winlost;
                    pm.p_p = pg_p;
                    pm.p_fee = pg_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "PG", winlost = _pg_winlost, p_p = pg_p, p_fee = pg_fee });
                //gameDatas.Add(new GameData { gname = "PG", bet = pg_bet, winlost = _pg_winlost, turnover = pg_turnover, p_p = pg_p, p_fee = pg_fee });

            }
            get { return _pg_winlost; }
        }
        public decimal pg_fee {
            set; get;
        }

        #endregion

        #region imes 开启
        private decimal _imes_bet;
        public decimal imes_bet {
            set {
                _imes_bet = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null) {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", bet = value });
            }
            get { return _imes_bet; }
        }
        public decimal imes_p { get; set; } = GameFee.platfeeMap["imes"];
        private decimal _imes_turnover;
        public decimal imes_turnover {
            set {
                _imes_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null) {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", turnover = value });
            }
            get { return _imes_turnover; }
        }
        private decimal _imes_winlost;
        public decimal imes_winlost {
            set {
                imes_fee = Convert.ToDecimal((imes_p * value).ToString("F2"));
                totalGameFee += imes_fee > 0 ? imes_fee : 0;
                _imes_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "IMES");
                if (pm != null) {
                    pm.winlost = _imes_winlost;
                    pm.p_p = imes_p;
                    pm.p_fee = imes_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMES", winlost = _imes_winlost, p_p = imes_p, p_fee = imes_fee });
            }
            get { return _imes_winlost; }
        }
        public decimal imes_fee {
            set; get;
        }
        #endregion


        #region evo 开启
        private decimal _evo_bet;
        public decimal evo_bet {
            set {
                _evo_bet = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null) {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", bet = value });
            }
            get { return _evo_bet; }
        }
        public decimal evo_p { get; set; } = GameFee.platfeeMap["evo"];
        private decimal _evo_turnover;
        public decimal evo_turnover {
            set {
                _evo_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null) {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", turnover = value });
            }
            get { return _evo_turnover; }
        }
        private decimal _evo_winlost;
        public decimal evo_winlost {
            set {
                evo_fee = Convert.ToDecimal((evo_p * value).ToString("F2"));
                totalGameFee += evo_fee > 0 ? evo_fee : 0;
                _evo_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "EVO");
                if (pm != null) {
                    pm.winlost = _evo_winlost;
                    pm.p_p = evo_p;
                    pm.p_fee = evo_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "EVO", winlost = _evo_winlost, p_p = evo_p, p_fee = evo_fee });
            }
            get { return _evo_winlost; }
        }
        public decimal evo_fee {
            set; get;
        }

        #endregion

        

        #region imsp
        private decimal _imsp_bet;
        public decimal imsp_bet {
            set {
                _imsp_bet = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null) {
                    pm.bet = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", bet = value });
            }
            get { return _imsp_bet; }
        }
        public decimal imsp_p { get; set; } = GameFee.platfeeMap["imsp"];
        private decimal _imsp_turnover;
        public decimal imsp_turnover {
            set {
                _imsp_turnover = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null) {
                    pm.turnover = value;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", turnover = value });
            }
            get { return _imsp_turnover; }
        }
        private decimal _imsp_winlost;
        public decimal imsp_winlost {
            set {
                imsp_fee = Convert.ToDecimal((imsp_p * value).ToString("F2"));
                totalGameFee += imsp_fee > 0 ? imsp_fee : 0;
                _imsp_winlost = value;
                var pm = gameDatas.Find(x => x.gname == "IMSP");
                if (pm != null) {
                    pm.winlost = _imsp_winlost;
                    pm.p_p = imsp_p;
                    pm.p_fee = imsp_fee;
                }
                else
                    gameDatas.Add(new GameData { gname = "IMSP", winlost = _imsp_winlost, p_p = imsp_p, p_fee = imsp_fee });
            }
            get { return _imsp_winlost; }
        }
        public decimal imsp_fee {
            set; get;
        }

        #endregion

        


        public string levelDesc { set; get; }
        private int _isLeaf;
        /// <summary>
        /// 1: 有子
        /// 0：无子
        /// </summary>
        public int isLeaf {
            set {
                levelDesc = value == 1 ? "双击展开" : "";
                _isLeaf = value;
            }
            get {
                return _isLeaf;
            }
        }


        public int id { set; get; }
        public int pId { set; get; }

    }

    /// <summary>
    /// 20250205 bti  dbkeno
    /// 20260805 iaes+ pp 
    /// </summary>
    public class GameBase
    {
        
        public decimal ea_bet { get; set; }
        public decimal ea_turnover { get; set; }
        public decimal ea_winlost { get; set; }
        
        public decimal kg_bet { get; set; }
        public decimal kg_turnover { get; set; }
        public decimal kg_winlost { get; set; }
       
        public decimal kg2_bet { get; set; }
        public decimal kg2_turnover { get; set; }
        public decimal kg2_winlost { get; set; }
       

        public decimal bbin_bet { get; set; }
        public decimal bbin_turnover { get; set; }
        public decimal bbin_winlost { get; set; }
        

        public decimal sb_bet { get; set; }
        public decimal sb_turnover { get; set; }
        public decimal sb_winlost { get; set; }
        

        public decimal pt_bet { get; set; }
        public decimal pt_turnover { get; set; }
        public decimal pt_winlost { get; set; }
        

        public decimal ttg_bet { get; set; }
        public decimal ttg_turnover { get; set; }
        public decimal ttg_winlost { get; set; }
        
        //dscp
        public decimal dscp_bet { get; set; }
        public decimal dscp_turnover { get; set; }
        public decimal dscp_winlost { get; set; }
       

        public decimal dses_bet { get; set; }
        public decimal dses_turnover { get; set; }
        public decimal dses_winlost { get; set; }
       

        public decimal lb_bet { get; set; }
        public decimal lb_turnover { get; set; }
        public decimal lb_winlost { get; set; }
       

        public decimal gg_bet { get; set; }
        public decimal gg_turnover { get; set; }
        public decimal gg_winlost { get; set; }
       


        public decimal ag_bet { get; set; }
        public decimal ag_turnover { get; set; }
        public decimal ag_winlost { get; set; }
        

        public decimal lbssc_bet { get; set; }
        public decimal lbssc_turnover { get; set; }
        public decimal lbssc_winlost { get; set; }
        

        public decimal? bsg_bet { get; set; }
        public decimal bsg_turnover { get; set; }
        public decimal? bsg_winlost { get; set; }
        

        public decimal? keno2_bet { get; set; }
        public decimal keno2_turnover { get; set; }
        public decimal? keno2_winlost { get; set; }
        

        public decimal? vr_bet { get; set; }
        public decimal vr_turnover { get; set; }
        public decimal? vr_winlost { get; set; }
        


        public decimal? xj_bet { get; set; }
        public decimal xj_turnover { get; set; }
        public decimal? xj_winlost { get; set; }
        

        public decimal? pg_bet { get; set; }
        public decimal pg_turnover { get; set; }
        public decimal? pg_winlost { get; set; }
        

        public decimal? imes_bet { get; set; }
        public decimal imes_turnover { get; set; }
        public decimal? imes_winlost { get; set; }



        public decimal? ysb_bet { get; set; }
        public decimal ysb_turnover { get; set; }
        public decimal? ysb_winlost { get; set; }


        public decimal? ky_bet { get; set; }
        public decimal ky_turnover { get; set; }
        public decimal? ky_winlost { get; set; }
        

        public decimal? rg_bet { get; set; }
        public decimal rg_turnover { get; set; }

        public decimal? rg_winlost { get; set; }
        
        public decimal? ob_bet { get; set; }
        public decimal ob_turnover { set; get; }
        public decimal? ob_winlost { get; set; }

        public decimal? bg_bet { get; set; }
        public decimal bg_turnover { set; get; }
        public decimal? bg_winlost { get; set; }

        public decimal? eb_bet { get; set; }
        public decimal eb_turnover { set; get; }
        public decimal? eb_winlost { get; set; }


        public decimal? td_bet { get; set; }
        public decimal td_turnover { set; get; }
        public decimal? td_winlost { get; set; }

        public decimal? cq_bet { get; set; }
        public decimal cq_turnover { set; get; }
        public decimal? cq_winlost { get; set; }

        public decimal? aw_bet { get; set; }
        public decimal aw_turnover { set; get; }
        public decimal? aw_winlost { get; set; }

        public decimal? tf_bet { get; set; }
        public decimal tf_turnover { set; get; }
        public decimal? tf_winlost { get; set; }

        public decimal? kg3_bet { get; set; }
        public decimal kg3_turnover { set; get; }
        public decimal? kg3_winlost { get; set; }

        public decimal? obg_bet { get; set; }
        public decimal obg_turnover { set; get; }
        public decimal? obg_winlost { get; set; }

        public decimal? obcv_bet { get; set; }
        public decimal obcv_turnover { set; get; }
        public decimal? obcv_winlost { get; set; }

        public decimal? imsp_bet { get; set; }
        public decimal imsp_turnover { set; get; }
        public decimal? imsp_winlost { get; set; }

        public decimal? sw_bet { get; set; }
        public decimal sw_turnover { set; get; }
        public decimal? sw_winlost { get; set; }


        public decimal? cr_bet { get; set; }
        public decimal cr_turnover { set; get; }
        public decimal? cr_winlost { get; set; }

        public decimal? fb_bet { get; set; }
        public decimal fb_turnover { set; get; }
        public decimal? fb_winlost { get; set; }

        public decimal? jdb_bet { get; set; }
        public decimal jdb_turnover { set; get; }
        public decimal? jdb_winlost { get; set; }
        public decimal? db_bet { get; set; }
        public decimal db_turnover { set; get; }
        public decimal? db_winlost { get; set; }

        public decimal? rsg_bet { get; set; }
        public decimal rsg_turnover { set; get; }
        public decimal? rsg_winlost { get; set; }

        public decimal? bti_bet { get; set; }
        public decimal bti_turnover { set; get; }
        public decimal? bti_winlost { get; set; }

        public decimal? dbkeno_bet { get; set; }
        public decimal dbkeno_turnover { set; get; }
        public decimal? dbkeno_winlost { get; set; }

        public decimal? iaes_bet { get; set; }
        public decimal iaes_turnover { set; get; }
        public decimal? iaes_winlost { get; set; }

        public decimal? pp_bet { get; set; }
        public decimal pp_turnover { set; get; }
        public decimal? pp_winlost { get; set; }

    }

    public class GameWinBetModel
    {
        public string gameName { set; get; }
        public decimal gameBet { set; get; }
        public decimal gameWinlost { set; get; }
    }
    



    public class GameReportBase100
    {
        public long tid { set; get; }
        public decimal totalGameFee { set; get; } = 0;
        public string account { get; set; }
        public List<object> gameBetWin
        {
            set { }
            get
            {
                //平台   投注额  输赢   返水   比例   平台费
                var betWin = new List<object>()
                {
                    new { gameName="xj", gameBet = xj_bet, gameWinlost = xj_winlost, gameTurnover = xj_turnover, pp= xj_p*100+"%", p_fee= xj_fee },
                    new { gameName="ob", gameBet = ob_bet, gameWinlost = ob_winlost,gameTurnover = ob_turnover, pp= ob_p*100+"%", p_fee= ob_fee },
                    new { gameName="ag", gameBet = ag_bet, gameWinlost = ag_winlost,gameTurnover = ag_turnover, pp= ag_p*100+"%", p_fee= ag_fee },
                    new { gameName="ea", gameBet = ea_bet, gameWinlost = ea_winlost,gameTurnover = ea_turnover, pp= ea_p*100+"%", p_fee= ea_fee },
                    new { gameName="bg", gameBet = bg_bet, gameWinlost = bg_winlost,gameTurnover = bg_turnover, pp= bg_p*100+"%", p_fee= bg_fee },
                    new { gameName="eb", gameBet = eb_bet, gameWinlost = eb_winlost,gameTurnover = eb_turnover, pp= eb_p*100+"%", p_fee= eb_fee },
                    new { gameName="bbin", gameBet = bbin_bet, gameWinlost = bbin_winlost,gameTurnover = bbin_turnover, pp= bbin_p*100+"%", p_fee= bbin_fee },
                    new { gameName="kg", gameBet = kg_bet, gameWinlost = kg_winlost,gameTurnover = kg_turnover, pp= kg_p*100+"%", p_fee= kg_fee },
                    new { gameName="kg2", gameBet = kg2_bet, gameWinlost = kg2_winlost,gameTurnover = kg2_turnover, pp= kg2_p*100+"%", p_fee= kg2_fee },
                    new { gameName="dscp", gameBet = dscp_bet, gameWinlost = dscp_winlost,gameTurnover = dscp_turnover, pp= dscp_p*100+"%", p_fee= dscp_fee },
                    new { gameName="lb", gameBet = lb_bet, gameWinlost = lb_winlost,gameTurnover = lb_turnover, pp= lb_p*100+"%", p_fee= lb_fee },
                    new { gameName="aw", gameBet = aw_bet, gameWinlost = aw_winlost,gameTurnover = aw_turnover, pp= aw_p*100+"%", p_fee= aw_fee },
                    new { gameName="pt", gameBet = pt_bet, gameWinlost = pt_winlost,gameTurnover = pt_turnover, pp= pt_p*100+"%", p_fee= pt_fee },
                    new { gameName="ttg", gameBet = ttg_bet, gameWinlost = ttg_winlost,gameTurnover = ttg_turnover, pp= ttg_p*100+"%", p_fee= ttg_fee },
                    new { gameName="dses", gameBet = dses_bet, gameWinlost = dses_winlost,gameTurnover = dses_turnover, pp=dses_p*100+"%", p_fee= dses_fee },
                    new { gameName="gg", gameBet = gg_bet, gameWinlost = gg_winlost,gameTurnover = gg_turnover, pp= gg_p*100+"%", p_fee= gg_fee },
                    new { gameName="vr", gameBet = vr_bet, gameWinlost = vr_winlost,gameTurnover = vr_turnover, pp= vr_p*100+"%", p_fee= vr_fee },
                    new { gameName="pg", gameBet = pg_bet, gameWinlost = pg_winlost,gameTurnover = pg_turnover, pp= pg_p*100+"%", p_fee= pt_fee },
                    new { gameName="imes", gameBet = imes_bet, gameWinlost = imes_winlost,gameTurnover = imes_turnover, pp= imes_p*100+"%", p_fee= imes_fee },
                    new { gameName="ysb", gameBet = ysb_bet, gameWinlost = ysb_winlost,gameTurnover = ysb_turnover, pp= ysb_p*100+"%", p_fee= ysb_fee },
                    new { gameName="ky", gameBet = ky_bet, gameWinlost = ky_winlost,gameTurnover = ky_turnover, pp= ky_p*100+"%", p_fee= ky_fee },
                    new { gameName="rg", gameBet = rg_bet, gameWinlost = rg_winlost,gameTurnover = rg_turnover, pp= rg_p*100+"%", p_fee= rg_fee },
                    new { gameName="td", gameBet = td_bet, gameWinlost = td_winlost,gameTurnover = td_turnover, pp= td_p*100+"%", p_fee= td_fee },
                    new { gameName="tf", gameBet = tf_bet, gameWinlost = tf_winlost,gameTurnover = tf_turnover, pp= tf_p*100+"%", p_fee= tf_fee },
                    new { gameName="cq", gameBet = cq_bet, gameWinlost = cq_winlost,gameTurnover = cq_turnover, pp= cq_p*100+"%", p_fee= cq_fee },
                    new { gameName="kg3", gameBet = kg3_bet, gameWinlost = kg3_winlost,gameTurnover = kg3_turnover, pp= kg3_p*100+"%", p_fee= kg3_fee },
                    new { gameName="obg", gameBet = obg_bet, gameWinlost = obg_winlost,gameTurnover = obg_turnover, pp= obg_p*100+"%", p_fee= obg_fee },
                    new { gameName="imsp", gameBet = imsp_bet, gameWinlost = imsp_winlost,gameTurnover = imsp_turnover, pp= imsp_p*100+"%", p_fee= imsp_fee },
                    new { gameName="sw", gameBet = sw_bet, gameWinlost = sw_winlost,gameTurnover = sw_turnover, pp= sw_p*100+"%", p_fee= sw_fee },
                    new { gameName="cr", gameBet = cr_bet, gameWinlost = cr_winlost,gameTurnover = cr_turnover, pp= cr_p*100+"%", p_fee= cr_fee },
                    new { gameName="obcv", gameBet = obcv_bet, gameWinlost = obcv_winlost,gameTurnover = obcv_turnover, pp= obcv_p*100+"%", p_fee= obcv_fee }

                };
                return betWin;
            }
        }
        private decimal _ea_bet;
        public decimal ea_bet {
            set
            {
                _ea_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ea_bet;
            }
        }
        public decimal ea_p { get; set; } = GameFee.platfeeMap["ea"];
        private decimal _ea_turnover;
        public decimal ea_turnover {
            set
            {
                _ea_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ea_turnover;
            }
        }
        private decimal _ea_winlost;
        public decimal ea_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ea_p * value).ToString("F2"));
                ea_fee = Math.Round(fee / 100, 4);
                totalGameFee += ea_fee;
                _ea_winlost = Math.Round(value / 100, 4);
            }
            get { return _ea_winlost; }
        }
        public decimal ea_fee
        {
            set; get;
        }

        private decimal _kg_bet;
        public decimal kg_bet {
            set
            {
                _kg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg_bet;
            }
        }
        public decimal kg_p { get; set; } = GameFee.platfeeMap["kg"];
        private decimal _kg_turnover;
        public decimal kg_turnover {
            set
            {
                _kg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg_turnover;
            }
        }
        private decimal _kg_winlost;
        public decimal kg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((kg_p * value).ToString("F2"));
                kg_fee = Math.Round(fee / 100, 4);
                totalGameFee += kg_fee;
                _kg_winlost = Math.Round(value / 100, 4);
            }
            get { return _kg_winlost; }
        }
        public decimal kg_fee
        {
            set; get;
        }

        private decimal _kg2_bet;
        public decimal kg2_bet {
            set
            {
                _kg2_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg2_bet;
            }
        }
        public decimal kg2_p { get; set; } = GameFee.platfeeMap["kg2"];

        private decimal _kg2_turnover;
        public decimal kg2_turnover
        {
            set
            {
                _kg2_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg2_turnover;
            }
        }
        private decimal _kg2_winlost;
        public decimal kg2_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((kg2_p * value).ToString("F2"));
                kg2_fee = Math.Round(fee / 100, 4);
                totalGameFee += kg2_fee;
                _kg2_winlost = Math.Round(value / 100, 4);

            }
            get { return _kg2_winlost; }
        }
        public decimal kg2_fee
        {
            set; get;
        }
        private decimal _bbin_bet;
        public decimal bbin_bet {
            set
            {
                _bbin_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _bbin_bet;
            }
        }
        public decimal bbin_p { get; set; } = GameFee.platfeeMap["bbin"];
        private decimal _bbin_turnover;
        public decimal bbin_turnover {
            set
            {
                _bbin_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _bbin_turnover;
            }
        }
        private decimal _bbin_winlost;
        public decimal bbin_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((bbin_p * value).ToString("F2"));
                bbin_fee = Math.Round(fee / 100, 4);
                totalGameFee += bbin_fee;
                _bbin_winlost = Math.Round(value / 100, 4);
            }
            get { return _bbin_winlost; }
        }
        public decimal bbin_fee
        {
            set; get;
        }


        private decimal _pt_bet;
        public decimal pt_bet {
            set
            {
                _pt_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _pt_bet;
            }
        }
        public decimal pt_p { get; set; } = GameFee.platfeeMap["pt"];
        private decimal _pt_turnover;
        public decimal pt_turnover {
            set
            {
                _pt_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _pt_turnover;
            }
        }
        private decimal _pt_winlost;
        public decimal pt_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((pt_p * value).ToString("F2"));
                pt_fee = Math.Round(fee / 100, 4);
                totalGameFee += pt_fee;
                _pt_winlost = Math.Round(value / 100, 4); 

            }
            get { return _pt_winlost; }
        }
        public decimal pt_fee
        {
            set; get;
        }

        private decimal _ttg_bet;
        public decimal ttg_bet
        {
            set
            {
                _ttg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ttg_bet;
            }
        }
        public decimal ttg_p { get; set; } = GameFee.platfeeMap["ttg"];
        private decimal _ttg_turnover;
        public decimal ttg_turnover
        {
            set
            {
                _ttg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ttg_turnover;
            }
        }
        private decimal _ttg_winlost;
        public decimal ttg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ttg_p * value).ToString("F2"));
                ttg_fee = Math.Round(fee / 100, 4);
                totalGameFee += ttg_fee;
                _ttg_winlost = Math.Round(value / 100, 4);
            }
            get { return _ttg_winlost; }
        }
        public decimal ttg_fee
        {
            set; get;
        }
        //dscp
        private decimal _dscp_bet;
        public decimal dscp_bet {
            set
            {
                _dscp_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _dscp_bet;
            }
        }
        public decimal dscp_p { get; set; } = GameFee.platfeeMap["dscp"];
        private decimal _dscp_turnover;
        public decimal dscp_turnover
        {
            set
            {
                _dscp_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _dscp_turnover;
            }
        }
        private decimal _dscp_winlost;
        public decimal dscp_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((dscp_p * value).ToString("F2"));
                dscp_fee = Math.Round(fee / 100, 4);
                totalGameFee += dscp_fee;
                _dscp_winlost = Math.Round(value / 100, 4); 

            }
            get { return _dscp_winlost; }
        }
        public decimal dscp_fee
        {
            set; get;
        }
        private decimal _dses_bet;
        public decimal dses_bet {
            set
            {
                _dses_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _dses_bet;
            }
        }
        public decimal dses_p { get; set; } = GameFee.platfeeMap.ContainsKey("dses") ? GameFee.platfeeMap["dses"] : 0;
        private decimal _dses_turnover;
        public decimal dses_turnover {
            set
            {
                _dses_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _dses_turnover;
            }
        }
        private decimal _dses_winlost;
        public decimal dses_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((dses_p * value).ToString("F2"));
                dses_fee = Math.Round(fee / 100, 4); 
                totalGameFee += dses_fee;
                _dses_winlost = Math.Round(value / 100, 4);
            }
            get { return _dses_winlost; }
        }
        public decimal dses_fee
        {
            set; get;
        }

        private decimal _lb_bet;
        public decimal lb_bet
        {
            set
            {
                _lb_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _lb_bet;
            }
        }
        public decimal lb_p { get; set; } = GameFee.platfeeMap["lb"];
        private decimal _lb_turnover;
        public decimal lb_turnover
        {
            set
            {
                _lb_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _lb_turnover;
            }
        }
        private decimal _lb_winlost;
        public decimal lb_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((lb_p * value).ToString("F2"));
                lb_fee = Math.Round(fee / 100, 4);
                totalGameFee += lb_fee;
                _lb_winlost = Math.Round(value / 100, 4);
            }
            get { return _lb_winlost; }
        }
        public decimal lb_fee
        {
            set; get;
        }

        private decimal _gg_bet;
        public decimal gg_bet
        {
            set
            {
                _gg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _gg_bet;
            }
        }
        public decimal gg_p { get; set; } = 0;
        private decimal _gg_turnover;
        public decimal gg_turnover
        {
            set
            {
                _gg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _gg_turnover;
            }
        }
        private decimal _gg_winlost;
        public decimal gg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((gg_p * value).ToString("F2"));
                gg_fee = Math.Round(fee / 100, 4);
                totalGameFee += gg_fee;
                _gg_winlost = Math.Round(value / 100, 4);
            }
            get { return _gg_winlost; }
        }
        public decimal gg_fee
        {
            set; get;
        }

        private decimal _ag_bet;
        public decimal ag_bet {
            set
            {
                _ag_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ag_bet;
            }
        }
        public decimal ag_p { get; set; } = GameFee.platfeeMap["ag"];
        private decimal _ag_turnover;
        public decimal ag_turnover {
            set
            {
                _ag_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ag_turnover;
            }
        }
        private decimal _ag_winlost;
        public decimal ag_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ag_p * value).ToString("F2"));
                ag_fee = Math.Round(fee / 100, 4);
                totalGameFee += ag_fee;
                _ag_winlost = Math.Round(value / 100, 4);

            }
            get { return _ag_winlost; }
        }
        public decimal ag_fee
        {
            set; get;
        }


        private decimal _vr_bet;
        public decimal vr_bet {
            set
            {
                _vr_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _vr_bet;
            }
        }
        public decimal vr_p { get; set; } = GameFee.platfeeMap["vr"];
        private decimal _vr_turnover;
        public decimal vr_turnover {
            set
            {
                _vr_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _vr_turnover;
            }
        }
        private decimal _vr_winlost;
        public decimal vr_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((vr_p * value).ToString("F2"));
                vr_fee = Math.Round(fee / 100, 4);
                totalGameFee += vr_fee;
                _vr_winlost = Math.Round(value / 100, 4);
            }
            get { return _vr_winlost; }
        }
        public decimal vr_fee
        {
            set; get;
        }
        private decimal _xj_bet;
        public decimal xj_bet
        {
            set
            {
                _xj_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _xj_bet;
            }
        }
        public decimal xj_p { get; set; } = GameFee.platfeeMap["xj"];
        private decimal _xj_turnover;
        public decimal xj_turnover
        {
            set
            {
                _xj_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _xj_turnover;
            }
        }
        private decimal _xj_winlost;
        public decimal xj_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((xj_p * value).ToString("F2"));
                xj_fee = Math.Round(fee / 100, 4);
                totalGameFee += xj_fee;
                _xj_winlost = Math.Round(value / 100, 4);

            }
            get { return _xj_winlost; }
        }
        public decimal xj_fee
        {
            set; get;
        }

        private decimal _pg_bet;
        public decimal pg_bet {
            set
            {
                _pg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _pg_bet;
            }
        }
        public decimal pg_p { get; set; } = GameFee.platfeeMap["pg"];
        private decimal _pg_turnover;
        public decimal pg_turnover {
            set
            {
                _pg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _pg_turnover;
            }
        }
        private decimal _pg_winlost;
        public decimal pg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((pg_p * value).ToString("F2"));
                pg_fee = Math.Round(fee / 100, 4);
                totalGameFee += pg_fee;
                _pg_winlost = Math.Round(value / 100, 4);
            }
            get { return _pg_winlost; }
        }
        public decimal pg_fee
        {
            set; get;
        }

        private decimal _imes_bet;
        public decimal imes_bet {
            set
            {
                _imes_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _imes_bet;
            }
        }
        public decimal imes_p { get; set; } = GameFee.platfeeMap["imes"];
        private decimal _imes_turnover;
        public decimal imes_turnover {
            set
            {
                _imes_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _imes_turnover;
            }
        }
        private decimal _imes_winlost;
        public decimal imes_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((imes_p * value).ToString("F2"));
                imes_fee = Math.Round(fee / 100, 4);
                totalGameFee += imes_fee;
                _imes_winlost = Math.Round(value / 100, 4);
            }
            get { return _imes_winlost; }
        }
        public decimal imes_fee
        {
            set; get;
        }

        private decimal _ob_bet;
        public decimal ob_bet {
            set
            {
                _ob_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ob_bet;
            }
        }
        public decimal ob_p { get; set; } = GameFee.platfeeMap["ob"];
        private decimal _ob_turnover;
        public decimal ob_turnover {
            set
            {
                _ob_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ob_turnover;
            }
        }
        private decimal _ob_winlost;
        public decimal ob_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ob_p * value).ToString("F2"));
                ob_fee = Math.Round(fee / 100, 4);
                totalGameFee += ob_fee;
                _ob_winlost = Math.Round(value / 100, 4);

            }
            get { return _ob_winlost; }
        }
        public decimal ob_fee
        {
            set; get;
        }

        private decimal _ysb_bet;
        public decimal ysb_bet {
            set
            {
                _ysb_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ysb_bet;
            }
        }
        public decimal ysb_p { get; set; } = GameFee.platfeeMap["ysb"];
        private decimal _ysb_turnover;
        public decimal ysb_turnover {
            set
            {
                _ysb_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ysb_turnover;
            }
        }
        private decimal _ysb_winlost;
        public decimal ysb_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ysb_p * value).ToString("F2"));
                ysb_fee = Math.Round(fee / 100, 4);
                totalGameFee += ysb_fee;
                _ysb_winlost = Math.Round(value / 100, 4);

            }
            get { return _ysb_winlost; }
        }
        public decimal ysb_fee
        {
            set; get;
        }

        private decimal _ky_bet;
        public decimal ky_bet {
            set
            {
                _ky_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _ky_bet;
            }
        }
        public decimal ky_p { get; set; } = GameFee.platfeeMap["ky"];
        private decimal _ky_turnover;
        public decimal ky_turnover {
            set
            {
                _ky_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _ky_turnover;
            }
        }
        private decimal _ky_winlost;
        public decimal ky_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((ky_p * value).ToString("F2"));
                ky_fee = Math.Round(fee / 100, 4);
                totalGameFee += ky_fee;
                _ky_winlost = Math.Round(value / 100, 4);

            }
            get { return _ky_winlost; }
        }
        public decimal ky_fee
        {
            set; get;
        }

        private decimal _rg_bet;
        public decimal rg_bet {
            set
            {
                _rg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _rg_bet;
            }
        }
        public decimal rg_p { get; set; } = GameFee.platfeeMap["rg"];
        private decimal _rg_turnover;
        public decimal rg_turnover {
            set
            {
                _rg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _rg_turnover;
            }
        }
        private decimal _rg_winlost;
        public decimal rg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((rg_p * value).ToString("F2"));
                rg_fee = Math.Round(fee / 100, 4);
                totalGameFee += rg_fee;
                _rg_winlost = Math.Round(value / 100, 4);

            }
            get { return _rg_winlost; }
        }
        public decimal rg_fee
        {
            set; get;
        }

        private decimal _bg_bet;
        public decimal bg_bet {
            set
            {
                _bg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _bg_bet;
            }
        }
        public decimal bg_p { get; set; } = GameFee.platfeeMap["bg"];
        private decimal _bg_turnover;
        public decimal bg_turnover {
            set
            {
                _bg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _bg_turnover;
            }
        }
        private decimal _bg_winlost;
        public decimal bg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((bg_p * value).ToString("F2"));
                bg_fee = Math.Round(fee / 100, 4);
                totalGameFee += bg_fee;
                _bg_winlost = Math.Round(value / 100, 4);

            }
            get { return _bg_winlost; }
        }
        public decimal bg_fee
        {
            set; get;
        }

        private decimal _eb_bet;
        public decimal eb_bet {
            set
            {
                _eb_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _eb_bet;
            }
        }
        public decimal eb_p { get; set; } = GameFee.platfeeMap["eb"];
        private decimal _eb_turnover;
        public decimal eb_turnover {
            set
            {
                _eb_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _eb_turnover;
            }
        }
        private decimal _eb_winlost;
        public decimal eb_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((eb_p * value).ToString("F2"));
                eb_fee = Math.Round(fee / 100, 4);
                totalGameFee += eb_fee;
                _eb_winlost = Math.Round(value / 100, 4);

            }
            get { return _eb_winlost; }
        }
        public decimal eb_fee
        {
            set; get;
        }

        private decimal _td_bet;
        public decimal td_bet {
            set
            {
                _td_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _td_bet;
            }
        }
        public decimal td_p { get; set; } = GameFee.platfeeMap["td"];
        private decimal _td_turnover;
        public decimal td_turnover {
            set
            {
                _td_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _td_turnover;
            }
        }
        private decimal _td_winlost;
        public decimal td_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((td_p * value).ToString("F2"));
                td_fee = Math.Round(fee / 100, 4);
                totalGameFee += td_fee;
                _td_winlost = Math.Round(value / 100, 4);

            }
            get { return _td_winlost; }
        }
        public decimal td_fee
        {
            set; get;
        }

        private decimal _tf_bet;
        public decimal tf_bet {
            set
            {
                _tf_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _tf_bet;
            }
        }
        public decimal tf_p { get; set; } = GameFee.platfeeMap["tf"];
        private decimal _tf_turnover;
        public decimal tf_turnover {
            set
            {
                _tf_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _tf_turnover;
            }
        }
        private decimal _tf_winlost;
        public decimal tf_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((tf_p * value).ToString("F2"));
                tf_fee = Math.Round(fee / 100, 4);
                totalGameFee += tf_fee;
                _tf_winlost = Math.Round(value / 100, 4);
            }
            get { return _tf_winlost; }
        }
        public decimal tf_fee
        {
            set; get;
        }

        private decimal _aw_bet;
        public decimal aw_bet
        {
            set
            {
                _aw_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _aw_bet;
            }
        }
        public decimal aw_p { get; set; } = GameFee.platfeeMap["aw"];
        private decimal _aw_turnover;
        public decimal aw_turnover
        {
            set
            {
                _aw_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _aw_turnover;
            }
        }
        private decimal _aw_winlost;
        public decimal aw_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((aw_p * value).ToString("F2"));
                aw_fee = Math.Round(fee / 100, 4);
                totalGameFee += aw_fee;
                _aw_winlost = Math.Round(value / 100, 4);
            }
            get { return _aw_winlost; }
        }
        public decimal aw_fee
        {
            set; get;
        }

        private decimal _cq_bet;
        public decimal cq_bet
        {
            set
            {
                _cq_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _cq_bet;
            }
        }
        public decimal cq_p { get; set; } = GameFee.platfeeMap["cq"];
        private decimal _cq_turnover;
        public decimal cq_turnover
        {
            set
            {
                _cq_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _cq_turnover;
            }
        }
        private decimal _cq_winlost;
        public decimal cq_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((cq_p * value).ToString("F2"));
                cq_fee = Math.Round(fee / 100, 4);
                totalGameFee += cq_fee;
                _cq_winlost = Math.Round(value / 100, 4);

            }
            get { return _cq_winlost; }
        }
        public decimal cq_fee
        {
            set; get;
        }

        private decimal _kg3_bet;
        public decimal kg3_bet
        {
            set
            {
                _kg3_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg3_bet;
            }
        }
        public decimal kg3_p { get; set; } = GameFee.platfeeMap["kg3"];
        private decimal _kg3_turnover;
        public decimal kg3_turnover
        {
            set
            {
                _kg3_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _kg3_turnover;
            }
        }
        private decimal _kg3_winlost;
        public decimal kg3_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((kg3_p * value).ToString("F2"));
                kg3_fee = Math.Round(fee / 100, 4);
                totalGameFee += kg3_fee;
                _kg3_winlost = Math.Round(value / 100, 4);

            }
            get { return _kg3_winlost; }
        }
        public decimal kg3_fee
        {
            set; get;
        }


        #region obg 平台
        private decimal _obg_bet;
        public decimal obg_bet
        {
            set
            {
                _obg_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _obg_bet;
            }
        }
        public decimal obg_p { get; set; } = GameFee.platfeeMap["obg"];
        private decimal _obg_turnover;
        public decimal obg_turnover
        {
            set
            {
                _obg_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _obg_turnover;
            }
        }
        private decimal _obg_winlost;
        public decimal obg_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((obg_p * value).ToString("F2"));
                obg_fee = Math.Round(fee / 100, 4);
                totalGameFee += obg_fee;
                _obg_winlost = Math.Round(value / 100, 4);

            }
            get { return _obg_winlost; }
        }
        public decimal obg_fee
        {
            set; get;
        }
        #endregion

        #region imsp 平台
        private decimal _imsp_bet;
        public decimal imsp_bet
        {
            set
            {
                _imsp_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _imsp_bet;
            }
        }
        public decimal imsp_p { get; set; } = GameFee.platfeeMap["imsp"];
        private decimal _imsp_turnover;
        public decimal imsp_turnover
        {
            set
            {
                _imsp_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _imsp_turnover;
            }
        }
        private decimal _imsp_winlost;
        public decimal imsp_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((imsp_p * value).ToString("F2"));
                imsp_fee = Math.Round(fee / 100, 4);
                totalGameFee += imsp_fee;
                _imsp_winlost = Math.Round(value / 100, 4);

            }
            get { return _imsp_winlost; }
        }
        public decimal imsp_fee
        {
            set; get;
        }
        #endregion

        #region sw 平台
        private decimal _sw_bet;
        public decimal sw_bet
        {
            set
            {
                _sw_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _sw_bet;
            }
        }
        public decimal sw_p { get; set; } = GameFee.platfeeMap["sw"];
        private decimal _sw_turnover;
        public decimal sw_turnover
        {
            set
            {
                _sw_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _sw_turnover;
            }
        }
        private decimal _sw_winlost;
        public decimal sw_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((sw_p * value).ToString("F2"));
                sw_fee = Math.Round(fee / 100, 4);
                totalGameFee += sw_fee;
                _sw_winlost = Math.Round(value / 100, 4);

            }
            get { return _sw_winlost; }
        }
        public decimal sw_fee
        {
            set; get;
        }
        #endregion

        #region cr 平台
        private decimal _cr_bet;
        public decimal cr_bet
        {
            set
            {
                _cr_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _cr_bet;
            }
        }
        public decimal cr_p { get; set; } = GameFee.platfeeMap["cr"];
        private decimal _cr_turnover;
        public decimal cr_turnover
        {
            set
            {
                _cr_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _cr_turnover;
            }
        }
        private decimal _cr_winlost;
        public decimal cr_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((cr_p * value).ToString("F2"));
                cr_fee = Math.Round(fee / 100, 4);
                totalGameFee += cr_fee;
                _cr_winlost = Math.Round(value / 100, 4);

            }
            get { return _cr_winlost; }
        }
        public decimal cr_fee
        {
            set; get;
        }
        #endregion

        #region obcv 平台
        private decimal _obcv_bet;
        public decimal obcv_bet
        {
            set
            {
                _obcv_bet = Math.Round(value / 100, 4);
            }
            get
            {
                return _obcv_bet;
            }
        }
        public decimal obcv_p { get; set; } = GameFee.platfeeMap["obcv"];
        private decimal _obcv_turnover;
        public decimal obcv_turnover
        {
            set
            {
                _obcv_turnover = Math.Round(value / 100, 4);
            }
            get
            {
                return _obcv_turnover;
            }
        }
        private decimal _obcv_winlost;
        public decimal obcv_winlost
        {
            set
            {
                var fee = Convert.ToDecimal((obcv_p * value).ToString("F2"));
                obcv_fee = Math.Round(fee / 100, 4);
                totalGameFee += obcv_fee;
                _obcv_winlost = Math.Round(value / 100, 4);

            }
            get { return _obcv_winlost; }
        }
        public decimal obcv_fee
        {
            set; get;
        }
        #endregion
        public string levelDesc { set; get; }
        private int _isLeaf;
        /// <summary>
        /// 1: 有子
        /// 0：无子
        /// </summary>
        public int isLeaf
        {
            set
            {
                levelDesc = value == 1 ? "双击展开" : "";
                _isLeaf = value;
            }
            get
            {
                return _isLeaf;
            }
        }


        public int id { set; get; }
        public int pId { set; get; }

    }
}
