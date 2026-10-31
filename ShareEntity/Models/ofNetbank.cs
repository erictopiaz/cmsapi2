using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofNetbank")]
    public partial class ofNetbank_Brief
    {
        public long id { get; set; }

        public int status { get; set; }

        public int appstatus { get; set; }

    }


    [Table("ofNetbank")]
    public partial class ofNetbank_Edit
    {
        public long id { get; set; }

        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string bank { get; set; }

        [Required]
        [StringLength(50)]
        public string realname { get; set; }

        [Required]
        [StringLength(50)]
        public string cardno { get; set; }

        [Required]
        [StringLength(50)]
        public string cardaddress { get; set; }

        public DateTime createtime { get; set; }

        public decimal feecent { get; set; }

        public decimal feemax { get; set; }

        public decimal paymentcap { get; set; }

        public int status { get; set; }

        public int appstatus { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public DateTime? roundtime { get; set; }

        [StringLength(500)]
        public string range { get; set; }

        public string misc { get; set; }
    }



    [Table("ofNetbank")]
    public partial class ofNetbank
    {
        public long id { get; set; }

        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string bank { get; set; }

        [Required]
        [StringLength(50)]
        public string realname { get; set; }

        [Required]
        [StringLength(50)]
        public string cardno { get; set; }

        [Required]
        [StringLength(50)]
        public string cardaddress { get; set; }

        public DateTime createtime { get; set; }

        public decimal feecent { get; set; }

        public decimal feemax { get; set; }

        public decimal paymentcap { get; set; }

        public int status { get; set; }

        public int appstatus { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        [Column("class")]
        public int? classtype { get; set; }
        //public int? _class { get; set; }

        public DateTime? roundtime { get; set; }

        [StringLength(250)]
        public string range { get; set; }

        public string misc { get; set; }
    }
}
