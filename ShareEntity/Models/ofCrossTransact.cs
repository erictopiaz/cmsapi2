using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtilsLib;

namespace ShareEntity.Models
{

    [Table("ofCrossTransact")]
    public partial class ofCrossTransact
    {
        
        //[Required]
        [StringLength(50)]
        public string orderid { get; set; }

        [Dapper.Contrib.Extensions.Key]
        public long id { get; set; }
        public long depositid { get; set; }
        public long withdrawid { get; set; }
        public int status { get; set; }

        [Required]
        [StringLength(20)]
        public string account{ get; set; }
        public string reviewperson{ get; set; }


        //[Required]
        //public int event { get; set; }

        //[Required]
        [StringLength(100)]
        public string comment { get; set; }

        public DateTime createtime { get; set; }
        public DateTime reviewtime { get; set; }

    }
}
