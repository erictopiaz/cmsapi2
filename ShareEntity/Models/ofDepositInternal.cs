using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofDepositInternal")]
    public partial class ofDepositInternal
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }
        public string bankname { get; set; }

        public decimal amount { get; set; }

        [StringLength(50)]
        public string depositmethod { get; set; }

        public DateTime requesttime { get; set; }

        [StringLength(50)]
        public string merchantno { get; set; }

        public DateTime? orderendtime { get; set; }
        public DateTime? reviewtime { get; set; }

        public int status { get; set; }

        public long? payplatform { get; set; }
    }
}
