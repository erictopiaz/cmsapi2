using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    /// <summary>
    /// 查看代理提款记录并无查看银行卡号的权限
    /// </summary>
    public class BankCardNo: QueryAgentWithdrawOutModel
    {
        private string _cardno;
        public string cardno
        {
            set
            {
                _cardno = string.IsNullOrEmpty(value) ? "" : string.Format(@"{0} **** {1}", value.Substring(0, 4), value.Substring(value.Length - 4, 4));
            }
            get
            {
                return _cardno;
            }
        }
    }
    /// <summary>
    /// 查看代理提款记录并---有--查看银行卡号的权限
    /// </summary>
    public class AccessBankCardNo : QueryAgentWithdrawOutModel
    {
        public string cardno { set; get; }
    }
    public class QueryAgentWithdrawOutModel
    {
        public long id { set; get; }
        public string account { set; get; }
        public string realname { set; get; }
        
        public decimal amount { set; get; }
        public string cardbank { set; get; }
        public string requesttime { set; get; }
        public string cardaddress { set; get; }
        public string statusDesc { set; get; }
        private int _status;
        public int status {
            set
            {
                statusDesc = value==2 ? "提款成功" : value==0 ? "等待审核" : value==1 ? "支付中" : "取消提款";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string payperson { set; get; }
        public string paytime { set; get; }

        public string reviewperson { set; get; }
        public string reviewtime { set; get; }

        public string cancelreason { set; get; }
        public long aid { set; get; }
        public string amountDesc { set; get; } = "";
        
        public decimal rate {
            set;
            get;
        }
        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal exchangeAmount { set; get; }


    }


    public class ExportAgentWithdrawOutModel
    {
        public string account { set; get; }
        public string realname { set; get; }
       
        public string amount { set; get; }
        private string _requesttime;
        public string requesttime
        {
            set
            {
                _requesttime = string.IsNullOrEmpty(value) ? "" : Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get
            {
                return _requesttime;
            }
        }
       
        public string statusDesc { set; get; }
        private int _status;
        public int status
        {
            set
            {
                statusDesc = value == 2 ? "提款成功" : value == 0 ? "等待审核" : value == 1 ? "支付中" : "取消提款";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
       
        public string parentAccount { set; get; }
    }
}
