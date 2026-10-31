using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofManageGroup")]
    public partial class ofManageGroup
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        public int valid { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }
    }
}
