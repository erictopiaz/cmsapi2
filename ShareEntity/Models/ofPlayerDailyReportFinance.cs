using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{


    [Table("ofPlayerDailyReportFinance")]
    public partial class ofPlayerDailyReportFinance
    {
        public long id { get; set; }
        public string account { get; set; }
        public string affaccount { get; set; }
        public long financeDay { get; set; }
        public decimal depositAmount { get; set; }
        public int depositTimes { get; set; }
        public decimal depositFee { get; set; }
        public decimal withdrawAmount { get; set; }
        public int withdrawTimes { get; set; }
        public decimal withdrawFee { get; set; }
        public decimal promo { get; set; }
        public decimal bonus{ get; set; }
        public decimal turnover { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_time { get; set; }
    }
}
