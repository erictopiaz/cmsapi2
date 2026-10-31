
using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShareEntity.Models
{

    [Table("ofPayType")]
    public partial class ofPayType
    {
        //public long id { get; set; }

        [ExplicitKey]
        public long mainid { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(150)]
        public string action { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }


        [StringLength(500)]
        public string callbackip { get; set; }
    }
}
