using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    public partial class ofBonu
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal bonus { get; set; }

        [Required]
        [StringLength(250)]
        public string remarks { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }

        public int type { get; set; }

        public int status { get; set; }
    }
}
