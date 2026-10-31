using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofBonusPromoAuto")]
    public partial class ofBonusPromoAuto
    {
        public long id { get; set; }

        [Required]
        public long bonuspromoid { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal reward { get; set; }

        public DateTime createtime { get; set; }
        public DateTime? transferintime { get; set; }
        public DateTime? transferouttime { get; set; }

        public int gametype { get; set; }

        [Required]
        [StringLength(250)]
        public string remarks { get; set; }

        public int bet { get; set; }

        public int status { get; set; }

    }
}
