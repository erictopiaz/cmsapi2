using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.pg
{
    /// <summary>
    /// 游戏请求基本参数
    /// </summary>
    public class PGParamsModel
    {
        //{"token": "x","secret_key": "x","apiUrl": "x","loginUrl": "x","returnUrl": "x"}
        public string token { set; get; }
        public string secret_key { set; get; }
        public string token_inr { set; get; }
        public string secret_key_inr { set; get; }

        public string apiUrl { set; get; }
        public string apiUrl_inr { set; get; }
        public string loginUrl { set; get; }
        /// <summary>
        /// 进入游戏大厅链接
        /// </summary>
        public string publicUrl { set; get; }
        public string p_return_url { set; get; }
        public string m_return_url { set; get; }
        public int isIdAccount { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "PG";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }


    public class PGPostResponseBase<T>
    {
        public T data { set; get; }
        public Error error { set; get; }
    }
    public class Error
    {
        //1034 - Invalid Request
        //1035 - Operation Failed
        //1200 - Internal server error
        //1204 - Invalid operator
        public string code { set; get; }
        public string message { set; get; }
    }
    /// <summary>
    /// 创建玩家账号返回的数据
    /// </summary>
    public class PGCreatePlayerResponse
    {
        /// <summary>
        /// 1:成功，0：失败
        /// </summary>
        public int action_result { set; get; }
    }
    public class PGBalanceQueryResponse
    {
        //        "currencyCode": "CNY", 
        //"totalBalance": 997.00, 
        //"pointBalance": 0, 
        //"cashBalance": 997.00,
        //"totalBonusBalance": 0.0, 
        //"freeGameBalance": 0.0, 
        //"bonuses": [], 
        //"freeGames":[], 
        //"cashWallet": {
        //"cashId": 12055, 
        //"cashBalance": 997.0000
        public string currencyCode { set; get; }
        public string totalBalance { set; get; }
        public string pointBalance { set; get; }
        public string cashBalance { set; get; }
        public string totalBonusBalance { set; get; }
        public string freeGameBalance { set; get; }
        public CashWallet cashWallet { set; get; }
    }

    public class CashWallet
    {
        public string cashId { set; get; }
        public string cashBalance { set; get; }
    }

    #region 充值
    public class PGTransferInOrOut
    {
        //"transactionId": 35509540, 
        //"balanceAmountBefore": 1000.00, 
        //"balanceAmount": 1100.00,
        //"amount": 100.00
        public string transactionId { set; get; }
        public string balanceAmountBefore { set; get; }
        public string balanceAmount { set; get; }
        public string amount { set; get; }
    }
    #endregion

    #region 游戏登录验证返回结果
    public class LoginVerifyResponse
    {
        public string player_name { set; get; }
        public string nickname { set; get; }
        public string currency { set; get; }
        public string reminder_time { set; get; }
    }
    #endregion
}
