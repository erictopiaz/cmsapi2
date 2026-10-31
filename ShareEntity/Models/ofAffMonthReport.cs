using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofAffMonthReport")]
    public partial class ofAffMonthReport
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

        public decimal earn { get; set; }

        public DateTime createtime { get; set; }
    }
}
