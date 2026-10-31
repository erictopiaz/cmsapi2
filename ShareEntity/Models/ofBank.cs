using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofBank")]
    public partial class ofBank
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string cnname { get; set; }

        [Required]
        [StringLength(50)]
        public string enname { get; set; }

        [Required]
        [StringLength(50)]
        public string twname { get; set; }

        public DateTime time { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }
    }
}
