using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPromoCrash")]
    public partial class ofPromoCrash
    {
        public long id { get; set; }

        public long code { get; set; }

        [Required]
        [StringLength(500)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string ipaddress { get; set; }

        [Required]
        [StringLength(250)]
        public string macaddress { get; set; }
    }
}
