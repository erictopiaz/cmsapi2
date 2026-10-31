using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.agent
{
    public class QueryAgentBilledOutModel
    {
        public long id { set; get; }
        public string account { set; get; }
        public string billedTime { set; get; }
        public string amount { set; get; }
        public decimal subTotalAmount { set; get; }
        public decimal subTotalAmountPre { set; get; }
        public decimal subTotalAmountAfter { set; get; }

        public decimal unSettleAmount { set; get; }
        public decimal unSettleAmountPre { set; get; }
        public decimal unSettleAmountAfter { set; get; }
        
        public decimal balancePre { set; get; }
        public decimal balanceAfter { set; get; }
        public decimal balancePreLeft
        {
            get { return balancePre + subTotalAmountPre; }
        }
        public decimal balanceAfterLeft
        {
            get { return balanceAfter + subTotalAmountAfter; }
        }

        public string statusDesc { set; get; }
        private string _status;
        public string status
        {
            set
            {
                statusDesc = value.Equals("1") ? "审核通过" : value.Equals("0") ? "等待审核" : "已取销";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        public string verifytime { set; get; }
        public string verifycustomer { set; get; }
        public string cancelRemarks { set; get; }
        public string remark { set; get; }
        public string proportionToP { set; get; }
        
        public decimal promotion { set; get; }
        private decimal _proportion;
        public decimal proportion
        {
            set
            {
                proportionToP = string.Format(@"{0}%", (value * 100).ToString());
                _proportion = value;
            }
            get
            {
                return _proportion;
            }
        }
        public long aid { set; get; }
        /// <summary>
        /// 是否为多账号0：否，1： 是
        /// </summary>
        public int isMultiAccount { set; get; }
        /// <summary>
        /// 审核后代理账号可用余额
        /// </summary>
        public decimal leftamount { set; get; }

        public decimal totalTurnover { set; get; }

        /// <summary>
        /// 流水奖励比例
        /// </summary>
        public decimal betBonusP { set; get; } = 0;
        /// <summary>
        /// 流水奖励
        /// </summary>
        public decimal betBonus { set; get; } = 0;

        public string _regYYYYMM;
        /// <summary>
        /// 合营注册年月
        /// </summary>
        public string regYYYYMM {
            set {
                _regYYYYMM = value;
            }
            get { return Convert.ToDateTime(_regYYYYMM).ToString("yyyy-MM"); }
        }
        public int billPromoMonth { set; get; }
        public string billPromoPDesc { set; get; }
        private decimal _billPromoP;
        public decimal billPromoP { 
            set {
                _billPromoP = value;
                billPromoPDesc = string.Format(@"{0}%", (value * 100).ToString("0"));
            } 
            get { return _billPromoP; }
        }
        public string riskTypeAreadyDesc { set; get; }
        public int riskTypeAready { set; get; }
    }

    public class ExportAgentBilledOutModel
    {
        public string account { set; get; }
        public string billedTime { set; get; }
        public string amount { set; get; }
        public string statusDesc { set; get; }
        private string _status;
        public string status
        {
            set
            {
                statusDesc = value.Equals("1") ? "审核通过" : value.Equals("0") ? "等待审核" : "已取销";
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        
        public string remark { set; get; }
        public string proportionToP { set; get; }
        private decimal _proportion;
        public decimal proportion
        {
            set
            {
                proportionToP = string.Format(@"{0}%", (value * 100).ToString());
                _proportion = value;
            }
            get
            {
                return _proportion;
            }
        }
        public string parentAccount { set; get; }
    }

    /// <summary>
    /// 代理日结明细
    /// </summary>
    public class AFBillReportModel
    {
        public string account { set; get; }
        public int billDay { set; get; }
        public decimal amount { set; get; }
        public string remark { set; get; }
        public string createcustomer { set; get; }
        public string createtime { set; get; }
        public string verifycustomer { set; get; }
        public string verifytime { set; get; }
        public int status { set; get; }
        public decimal proportion { set; get; }
        public decimal leftamount { set; get; }
        public string ipaddress { set; get; }
        public string macaddress { set; get; }
        public decimal grossamount { set; get; }
        public decimal turnover { set; get; }
        public decimal bonus { set; get; }
        public decimal platformfee { set; get; }
        public decimal promotion { set; get; }
        public decimal withdrawfee { set; get; }
        public decimal betcodeamount { set; get; }
        //public int afpaymentlivetype { set; get; }
        public string update_at { set; get; }
        public string profit { set; get; }
    }
    /// <summary>
    /// 查询结果的统计
    /// </summary>
    public class AFBillReportData
    {
        public decimal amount { set; get; }
        public decimal grossamount { set; get; }
        public decimal turnover { set; get; }
        public decimal bonus { set; get; }
        public decimal platformfee { set; get; }
        public decimal fee { set; get; }
        public decimal profit { set; get; }
    }

    public class ExportAFBillReportOutModel : AFBillReportModel
    {
        public string parentAccount { set; get; }
    }
}
