using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    public partial class ofUserStatu
    {
        public long id { get; set; }

        public int statusnumber { get; set; }

        [Required]
        [StringLength(50)]
        public string status { get; set; }

        [Required]
        [StringLength(500)]
        public string description { get; set; }

        public DateTime time { get; set; }

        [Column("operator")]
        [Required]
        [StringLength(50)]
        public string _operator { get; set; }
    }
}
