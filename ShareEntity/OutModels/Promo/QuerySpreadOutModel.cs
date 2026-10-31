using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.Promo
{
    public class QuerySpreadOutModel 
    {
        public long tid { set; get; }
        public string account { set; get; }
        public string marcode { set; get; }
        public int depositnumber  { set; get; }
        public decimal depositamount { set; get; }
        public decimal withdrawalamount { set; get; }
        public string createtime { set; get; }
        public decimal wd { set; get; }
        public int status { set; get; }
        /// <summary>
        /// 会员的品牌
        /// </summary>
        public string merchantAccount { set; get; } = "-1";
        public int walletType { set; get; } = -1;

    }
}
