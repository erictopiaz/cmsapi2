using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofAffWithdraw")]
    public partial class ofAffWithdraw
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(50)]
        public string realname { get; set; }

        public decimal amount { get; set; }

        public decimal fee { get; set; }

        [Required]
        [StringLength(50)]
        public string cardno { get; set; }

        [Required]
        [StringLength(150)]
        public string cardaddress { get; set; }

        [Required]
        [StringLength(50)]
        public string cardbank { get; set; }

        public DateTime requesttime { get; set; }

        [Required]
        [StringLength(50)]
        public string requestip { get; set; }

        [Required]
        [StringLength(150)]
        public string requestaddress { get; set; }

        public int status { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }

        [StringLength(50)]
        public string payperson { get; set; }

        public DateTime? paytime { get; set; }

        public decimal? balance { get; set; }

        [StringLength(250)]
        public string cancelreason { get; set; }

        public int? alert { get; set; }
    }
}
