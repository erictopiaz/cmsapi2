using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPayment")]
    public partial class ofPayment
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(500)]
        public string transactiontype { get; set; }

        public DateTime time { get; set; }

        [Required]
        [StringLength(500)]
        public string summary { get; set; }

        [Required]
        [StringLength(50)]
        public string referenceguid { get; set; }

        public decimal? income { get; set; }

        public decimal? outcome { get; set; }

        public decimal balance { get; set; }

        public int? status { get; set; }
    }
}
