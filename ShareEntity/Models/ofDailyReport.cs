using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{


    [Table("ofDailyReport")]
    public partial class ofDailyReport
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public DateTime time { get; set; }

        public int hit { get; set; }

        public int newregister { get; set; }

        public int newdeposit { get; set; }

        public int newbetph { get; set; }

        public int newbetcasino { get; set; }

        public int newbetlottor { get; set; }

        public int activemember { get; set; }

        public int depositnumber { get; set; }

        public decimal depositamount { get; set; }

        public int withdrawnumber { get; set; }

        public decimal withdrawamount { get; set; }

        public decimal casino_totalbets { get; set; }

        public decimal casino_earnbets { get; set; }

        public decimal casino_calcbets { get; set; }

        public decimal casino_bets_recalc { get; set; }

        public decimal casino_earn_recalc { get; set; }

        public decimal casino_affwin_recalc { get; set; }

        public decimal keno_totalbets { get; set; }

        public decimal keno_earnbets { get; set; }

        public DateTime createtime { get; set; }

        public decimal ea_bets { get; set; }

        public decimal ea_win { get; set; }

        public decimal kg_bets { get; set; }

        public decimal kg_win { get; set; }

        public decimal bbin_bets { get; set; }

        public decimal bbin_win { get; set; }

        public decimal spb_bets { get; set; }

        public decimal spb_win { get; set; }

        public decimal pt_bets { get; set; }

        public decimal pt_win { get; set; }

        public decimal tt_bets { get; set; }

        public decimal tt_win { get; set; }

        public decimal lb_bets { get; set; }

        public decimal lb_win { get; set; }

        public decimal ag_bets { get; set; }

        public decimal ag_win { get; set; }

        public decimal lbssc_bets { get; set; }

        public decimal lbssc_win { get; set; }


        public decimal imes_bets { get; set; }

        public decimal imes_win { get; set; }

        public decimal ysb_bets { get; set; }

        public decimal ysb_win { get; set; }


        public decimal bonus { get; set; }
    }
}
