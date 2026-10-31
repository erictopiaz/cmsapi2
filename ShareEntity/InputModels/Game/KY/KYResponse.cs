using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.KY
{
    
    public class KYResponse<T>
    {
        public int s { set; get; }
        public string m { set; get; }
        public T d { set; get; }
    }
    public class LoginData
    {
        public int code { set; get; }
        public string url { set; get; }
    }
    public class BalanceData
    {
        public string totalMoney { set; get; }
        public string freeMoney { set; get; }
        public int status { set; get; }
        public int gameStatus { set; get; }
        public int code { set; get; }
    }
    /// <summary>
    /// 存取返回的d结构
    /// </summary>
    public class TransferData
    {
        public int code { set; get; }
        /// <summary>
        /// 转账号可下分的金额 
        /// </summary>
        public string money { set; get; }
    }
    /// <summary>
    /// 核实订单返回的d结构
    /// </summary>
    public class CheckOrderData
    {
        public int code { set; get; }
        public int status { set; get; }
        public string money { set; get; }
    }

    public class KYGameRecord
    {
        public int code { set; get; }
        public int count { set; get; }
        public KYResponseModel list { set; get; }
        public long start { set; get; }
        public long end { set; get; }
    }

    public class KYResponseModel
    {
        public string[] GameID { set; get; }
        public string[] Accounts { set; get; }
        public int[] ServerID { set; get; }
        public int[] KindID { set; get; }
        public int[] TableID { set; get; }
        public int[] ChairID { set; get; }
        public int[] UserCount { set; get; }
        public string[] CardValue { set; get; }
        public decimal[] CellScore { set; get; }
        public decimal[] AllBet { set; get; }
        public decimal[] Profit { set; get; }
        public decimal[] Revenue { set; get; }
        public string[] GameStartTime { set; get; }
        public string[] GameEndTime { set; get; }
        public int[] ChannelID { set; get; }
        public string[] LineCode { set; get; }

    }
}
