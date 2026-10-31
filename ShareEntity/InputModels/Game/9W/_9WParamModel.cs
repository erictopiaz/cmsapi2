using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace ShareEntity.InputModels.Game._9W {
    public class _9WParamModel {
        public string agent { set; get; }
        public string website { set; get; }
        public string cert { set; get; }
        public string apiServerHost { set; get; }
        public string exchangeHost { set; get; }
        public string prefix { set; get; }
        public int gamestatus { set; get; }

        public string gameName { set; get; } = "9W";
        /// <summary>
        /// 反向 呼叫游戏api
        /// </summary>
        public string proxy { set; get; }
    }


    public class BalanceResModel {
        public string status { set; get; }
        public int count { set; get; } = 0;
        public string querytime { set; get; }
        public List<BalanceModel> results { set; get; }
    }

    public class BalanceModel {
        public string userId { set; get; }
        public decimal balance { set; get; }
        public decimal exposure { set; get; }
    }


    public class TransferResModel {
        public string status { set; get; }
        public float withdrawBalance { set; get; }
        public float remainingBalance { set; get; }
    }

    public class GetKeyResModel {
        public string status { set; get; }
        public string key { set; get; } 
    }

    public class GetDomainResModel {
        public string status { set; get; }
        public List<DomainModel> privateDomains { set; get; }
        public List<DomainModel> domains { set; get; }
    }

    public class DomainModel {
        public string domain { set; get; }
        public int priority { set; get; }
    }
}
