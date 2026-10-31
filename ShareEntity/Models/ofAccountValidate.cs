using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofAccountValidate")]
    public partial class ofAccountValidate
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public int type { get; set; }

        [Required]
        [StringLength(250)]
        public string validate { get; set; }

        public DateTime createtime { get; set; }

        public DateTime updatetime { get; set; }

        public DateTime? validatetime { get; set; }

        public int status { get; set; }

        [StringLength(50)]
        public string ipaddress { get; set; }

        [StringLength(500)]
        public string macaddress { get; set; }

        [StringLength(250)]
        public string body { get; set; }
    }
}
