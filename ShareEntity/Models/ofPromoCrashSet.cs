using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPromoCrashSet")]
    public partial class ofPromoCrashSet
    {
        public long id { get; set; }

        [Required]
        [StringLength(500)]
        public string title { get; set; }

        public DateTime? begintime { get; set; }

        public int maxnumber { get; set; }

        public int? curnumber { get; set; }

        public decimal condition { get; set; }

        public DateTime conditiontime { get; set; }

        public DateTime conditionendtime { get; set; }

        public DateTime? expiretime { get; set; }

        public DateTime createtime { get; set; }

        public int status { get; set; }

        [StringLength(50)]
        public string createperson { get; set; }
    }
}
