using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofComment")]
    public partial class ofComment
    {
        //id, comment, createtime, createperson, status, maxamount, starttime

        public long id { get; set; }

        [Required]
        [StringLength(500)]
        public string comment { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public int status { get; set; }

        public decimal? maxamount { get; set; }

        public DateTime? starttime{ get; set; }
        
    }
}
