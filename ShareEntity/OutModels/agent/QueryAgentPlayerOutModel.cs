using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    public class QueryAgentPlayerOutModel
    {
        public string account { set; get; }
        public string realname { set; get; }
        public string referrerreg { set; get; }
        public string ipaddress { set; get; }
        public string createtime { set; get; }
        public string affaccount { set; get; }
        public int level { set; get; }
        public string levels { set; get; }
        public string statusDesc { set; get; }
        private int _status { set; get; }
        public int status {
            set
            {
                statusDesc = value == 0 ? "挂起" : value == 1 ? "正常" : value == 2 ? "冻结" : "作废";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        /// <summary>
        /// 是否被风控
        /// </summary>
        public int anomaly { set; get; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public string prevlogin { set; get; }
        public string misc {
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var jsonDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                        if (jsonDic != null && jsonDic.Count > 0)
                        {
                            if (jsonDic.ContainsKey("prevlogin"))
                                prevlogin = jsonDic["prevlogin"];
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            get { return ""; }
        }

        /// <summary>
        /// 负盈利
        /// </summary>
        public decimal profit { set; get; } = 0;

        public long tid { set; get; } = 0;
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;
    }
    // oa.account,u.affaccount,u.createtime,od.amount
    public class QueryFirstDepositPlayerOutModel
    {
        public string account { set; get; }
        public string affaccount { set; get; }
        public string createtime { set; get; }

        public decimal amount { set; get; }
        public string depositTime { set; get; }

        public int d_times { set; get; } = 0; 
        public decimal d_amount { set; get; } = 0;
        public int w_times { set; get; } = 0;
        public decimal w_amount { set; get; } = 0;
        public int riskTypesAready { set; get; } 

    }
}
