using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofDailyReportGame")]
    public partial class ofDailyReportGame
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [StringLength(50)]
        public string affaccount { get; set; }

        public DateTime? datetime { get; set; }

        public decimal ea_bet { get; set; }

        public decimal ea_winlost { get; set; }

        public decimal kg_bet { get; set; }

        public decimal kg_winlost { get; set; }

        public decimal bbin_bet { get; set; }

        public decimal bbin_winlost { get; set; }

        public decimal spb_bet { get; set; }

        public decimal spb_winlost { get; set; }

        public decimal pt_bet { get; set; }

        public decimal pt_winlost { get; set; }

        public decimal tt_bet { get; set; }

        public decimal tt_winlost { get; set; }

        public decimal lb_bet { get; set; }

        public decimal lb_winlost { get; set; }

        public DateTime updatetime { get; set; }

        public decimal totalbet { get; set; }

        public decimal winlost { get; set; }

        public decimal ag_bet { get; set; }

        public decimal ag_winlost { get; set; }

        public decimal lbssc_bet { get; set; }

        public decimal lbssc_winlost { get; set; }

        public decimal? bonus { get; set; }

        public decimal? bsg_bet { get; set; }

        public decimal? bsg_winlost { get; set; }

        public decimal? keno2_bet { get; set; }

        public decimal? keno2_winlost { get; set; }

        public decimal? vr_bet { get; set; }

        public decimal? vr_winlost { get; set; }

        public decimal? xj_bet { get; set; }

        public decimal? xj_winlost { get; set; }

        public decimal? pg_bet { get; set; }

        public decimal? pg_winlost { get; set; }

        public decimal? imes_bet { get; set; }

        public decimal? imes_winlost { get; set; }

        public decimal? ysb_bet { get; set; }

        public decimal? ysb_winlost { get; set; }

    }
}
