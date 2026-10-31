using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPassModify")]
    public partial class ofPassModify
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        public int gameid { get; set; }

        [Required]
        [StringLength(50)]
        public string gametype { get; set; }

        public int status { get; set; }

        [StringLength(50)]
        public string ipaddress { get; set; }

        [StringLength(50)]
        public string macaddress { get; set; }

        public DateTime createtime { get; set; }

        [StringLength(50)]
        public string verify { get; set; }

        public DateTime? verifytime { get; set; }

        public int? prompt { get; set; }

        [StringLength(50)]
        public string accountex { get; set; }
    }
}
