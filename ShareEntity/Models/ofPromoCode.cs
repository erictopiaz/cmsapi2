using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPromoCode")]
    public partial class ofPromoCode
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string promocode { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public decimal amount { get; set; }

        public decimal? lowamount { get; set; }

        public int bet { get; set; }

        [StringLength(50)]
        public string account { get; set; }

        public DateTime? claimdate { get; set; }

        [StringLength(500)]
        public string condition { get; set; }

        public DateTime? expiredate { get; set; }

        public int? status { get; set; }
    }
}
