using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofAffBill")]
    public partial class ofAffBill
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal amount { get; set; }

        [Required]
        [StringLength(500)]
        public string remarks { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public decimal calcdue { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }

        public int? status { get; set; }

        public decimal leftbalance { get; set; }
    }
}
