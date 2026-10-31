using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    /// 代理结算报表
    /// </summary>
    public class affBilledRecordsOutModel
    {
        public string account { set; get; }
        public decimal amount { set; get; }
        public decimal subTotalAmount { set; get; }
        public decimal unSettleAmount { set; get; }
        public long billedTime { set; get; }
        public decimal depositamount { set; get; }
        public decimal withdrawamount { set; get; }
        public decimal betamount { set; get; }
        public decimal bonus { set; get; }
        public decimal turnover { set; get; }
        public decimal platformfee { set; get; }
        public decimal grossamount { set; get; }
        public decimal promotion { set; get; }
        public decimal depositfee { set; get; }
        public string verifycustomer { set; get; }
        public string verifytime { set; get; }
        public string status { set; get; }
        public decimal proportion { set; get; }
    }
}
