//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.agent
{
    public class QueryAgentPlayerInputModel : PageBaseModel
    {
        //[CharAndDigitalAndUnderScore(12, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string adminAccount { set; get; }

        [IpValidate(20, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipaddress { set; get; }

        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string referrerreg { set; get; }
        public int dstatus { set; get; } = -1;

        [CharAndDigitalAndUnderScore(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string affaccount { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        [ChineseAndChar(10,0,MinimumLength =2,IsAllowedEmpty = true,ErrorMessage ="name_format_error")]
        public string realname { set; get; }

        public int currencyType { set; get; } = -1;

        public int walletType { set; get; } = -1;

        public string merchantAccount { set; get; } = "-1";
    }

    public class QueryAgentPointsInputModel : PageBaseModel
    {
       
        [IpValidate(50, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "ip_format_error")]
        public string ipaddress { set; get; }

        //[CharAndDigitalAndUnderScore(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        //public string affaccount { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string startTime { set; get; }

        [DateTimeValidate(19, true, IsAllowedEmpty = true, ErrorMessage = "time_format_error")]
        public string endTime { set; get; }
        /// <summary>
        /// 账变类型
        /// </summary>
        public int summaryType { set; get; } = -1;
        public int currencyType { set; get;} =156;
    }
    /// <summary>
    /// 账变记录输出
    /// </summary>
    public class QueryAgentPointsOutModel
    {
        public string account { set; get; }
        public string createtime { set; get; }
        public string transactiontypeDesc { set; get; }
        private int _transactiontype;
        public int transactiontype 
        {
            set
            {
                _transactiontype = value;
                transactiontypeDesc = value == 0 ? "扣除" : "新增";
            }
            get
            {
                return _transactiontype;
            }
        }
        public decimal preBalance { set; get; }
        public decimal bonus { set; get; }
        public decimal afterBalance { set; get; }
        public string summary { set; get; }

        

    }

}
