using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.BetRecordModel {


    public class BetRcordBaseModel {
        public string account { set; get; }
        public string orderno { set; get; }
        public decimal bet_amount { set; get; }
        public decimal win_amount { set; get; }
        public decimal transfer_amount { set; get; }
        public string bet_time { set; get; }
        public string bet_time_end { set; get; }
        public string device { set; get; } = "--";

        public string game { set; get; }

        public string statusDesc { set; get; } = "Settle";
        public string plat { set; get; }
    }

    /// <summary>
    /// 下注数据统计
    /// </summary>
    public class OrderReportModel {
        public decimal totalBet { set; get; } = 0;
        public decimal totalWinAmount { set; get; } = 0;
        public decimal totalWinlost { set; get; } = 0;
        public int rowss { set; get; } = 0;
    }

    public class PGBetRcordModel: BetRcordBaseModel {
       
        private int _platform;
        public int platform {
            set {
                _platform = value;
                var ps = new Dictionary<int, string>() {
                    { 1, "web windows" },
                    { 2, "web macOS" },
                    { 3, "web Android" },
                    { 4, "web ios" },
                    { 5, "unknow web" },
                    { 6, "Cordova Android" },
                    { 7, "Cordova ios" },
                    { 8, "Electron windows" },
                    { 9, "Electron macOS" },
                    { 10, "native Windows" },
                    { 11, "native macOS" },
                    { 12, "native Android" },
                    { 13, "native ios" },
                    { 98, "other system" },
                    { 99, "other gua ji" }
                };
                if (ps.ContainsKey(value))
                    device = ps[value];
                else
                    device = "unknow";
            }
            get { return _platform; }
        }
       

        public string en {
            set {
                game = value;
            }
            get { return game; }
        }
        public string cn { set; get; }
    }

    public class SpribeBetRcordModel : BetRcordBaseModel 
    {
        private string _platform;
        public string platform {
            set {
                device = string.IsNullOrEmpty(value) ? "--" : value;
                _platform = value;
            }
            get { return _platform; }
        }

        private string _cancel_time;
        public string cancel_time { 
            set {
                _cancel_time = (value == null || string.IsNullOrEmpty(value)) ? "" : value;
                if (value != null && !string.IsNullOrEmpty(value))
                    statusDesc = "Cancel";

            }
            get { return _cancel_time; }
        }
    }

    public class JDBBetRcordModel : BetRcordBaseModel {
        private string _platform;
        public string platform {
            set {
                device = string.IsNullOrEmpty(value) ? "--" : value;
                _platform = value;
            }
            get { return _platform; }
        }

        private string _cancel_time;
        public string cancel_time {
            set {
                _cancel_time = (value == null || string.IsNullOrEmpty(value)) ? "" : value;
                if (value != null && !string.IsNullOrEmpty(value))
                    statusDesc = "Cancel";

            }
            get { return _cancel_time; }
        }
        public string en {
            set {
                game = value;
            }
            get { return game; }
        }
        public string cn { set; get; }
    }

    public class EVOBetRcordModel : BetRcordBaseModel {
        
        private string _cancel_time;
        public string cancel_time {
            set {
                _cancel_time = (value == null || string.IsNullOrEmpty(value)) ? "" : value;
                if (value != null && !string.IsNullOrEmpty(value))
                    statusDesc = "Cancel";

            }
            get { return _cancel_time; }
        }
      
    }

    public class TurboBetRcordModel : BetRcordBaseModel {

        private string _cancel_time;
        public string cancel_time {
            set {
                _cancel_time = (value == null || string.IsNullOrEmpty(value)) ? "" : value;
                if (value != null && !string.IsNullOrEmpty(value))
                    statusDesc = "Cancel";

            }
            get { return _cancel_time; }
        }

    }

}
