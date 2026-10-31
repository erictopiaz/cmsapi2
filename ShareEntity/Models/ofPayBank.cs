using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPayBank")]
    public partial class ofPayBank
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        public long group { get; set; }

        public DateTime createtime { get; set; }

        [StringLength(50)]
        public string createperson { get; set; }
    }
}
