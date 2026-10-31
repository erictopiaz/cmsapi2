using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofMonthReport")]
    public partial class ofMonthReport
    {
        public long id { get; set; }

        public int year { get; set; }

        public int month { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal ew_turnover { get; set; }

        public decimal ew_lostwin { get; set; }

        public decimal kg_turnover { get; set; }

        public decimal kg_lostwin { get; set; }

        [StringLength(50)]
        public string affaccount { get; set; }

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
    }
}
