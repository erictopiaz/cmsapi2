using AutoMapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CMS.Core.Application.提款管理
{
    public class BaseWithdrawReturnModel
    {
        public long id { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public decimal amount { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public string cardbank { get; set; }
        public DateTime requesttime { get; set; }
        public int status { get; set; }
        public long TotalItems { get; set; }
        public long TotalAmount { get; set; }


        //public string requestip { get; set; }
        //public string requestaddress { get; set; }
        //public DateTime? reviewtime { get; set; }
        //public string reviewperson { get; set; }
        //public DateTime? paytime { get; set; }
        //public string payperson { get; set; }
        //public DateTime? canceltime { get; set; }
        //public string cancelperson { get; set; }
        //public string cancelreason { get; set; }
        //public decimal? balance { get; set; }
        //public string referrer { get; set; }
        //public string referrerreg { get; set; }
        public int level { get; set; }
        public string levelname { get; set; }
        //public string affaccount { get; set; }
        public string checkperson { get; set; }
        public int riskTypesAready { get; set; }

    }
}
