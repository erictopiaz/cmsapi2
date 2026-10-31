using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.Evolution {
    public class EVOParamsModel {
        public string apiurl { set; get; } 
        public string MerchantID { set; get; } 
        public string CasinoKey { set; get; } 
        public string token { set; get; }
        public string GameHistoryUrl { set; get; } 

        #region 
        public string apiurl_inr { set; get; } //= "http://staging.evolution.asia-live.com";
        public string MerchantID_inr { set; get; } //= "620001STA";
        public string CasinoKey_inr { set; get; } //= "3ng65szsawrehb0h";
        public string token_inr { set; get; } //= "d0d8c180a20e762c17800875c33c51af";
        public string GameHistoryUrl_inr { set; get; } //= "https://stage-admin.asia-live.com";
        #endregion

        public string proxy { set; get; }
        public string prefix { set; get; } = "";
        public string gameName { set; get; } = "EVO";
    }
}
