using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    public partial class ofPromotion
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal amount { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public DateTime? reviewtime { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        [Required]
        [StringLength(250)]
        public string remarks { get; set; }

        public int status { get; set; }

        [Required]
        [StringLength(150)]
        public string referrer { get; set; }

        [Required]
        [StringLength(150)]
        public string referrerreg { get; set; }

        public int level { get; set; }

        [Required]
        [StringLength(150)]
        public string referenceguid { get; set; }

        public int? gametype { get; set; }

        public int? visible { get; set; }
    }
}
