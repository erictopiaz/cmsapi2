using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{




    [Table("ofThirdpay")]
    public partial class ofThirdpay_WithoutClass
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string merchantid { get; set; }

        public string merchantkey { get; set; }

        public decimal paymentcap { get; set; }

        [Required]
        [StringLength(150)]
        public string requestaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string returnaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string refundaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string signedweb { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        [StringLength(500)]
        public string t_merchantAccounts { get; set; }

        public int status { get; set; }

        public int? sort { get; set; }

        public int? appstatus { get; set; }

        public int? minamount { get; set; }

        public int? maxamount { get; set; }

        [StringLength(10)]
        public string method { get; set; }

        [StringLength(200)]
        public string levels { get; set; }

        [StringLength(5)]
        public string appcode { get; set; }
        public string fastpay { get; set; }
        public string fastdeposit { set; get; }
        public string misc { get; set; }
        public string banklist { get; set; }
        public string merchantids { get; set; }
        public int merchantlevel { get; set; }
        public int isAgentUsed { get; set; }

    }


    [Table("ofThirdpay")]
    public partial class ofThirdpay
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string merchantid { get; set; }

        public string merchantkey { get; set; }

        public decimal paymentcap { get; set; }

        [Required]
        [StringLength(150)]
        public string requestaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string returnaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string refundaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string signedweb { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public int status { get; set; }

        [Column("class")]
        //[Column(Name = "class")]
        public int? _class { get; set; }

        public int? sort { get; set; }

        public int? appstatus { get; set; }

        public int? minamount { get; set; }

        public int? maxamount { get; set; }
        public string fastpay { get; set; }
        public string banklist{ get; set; }
        public int? paycategory { get; set; }

        [StringLength(10)]
        public string method { get; set; }

        [StringLength(200)]
        public string levels { get; set; }

        [StringLength(5)]
        public string appcode { get; set; }
        public string queryMerchantWhere { get; set; }
        public string misc { get; set; }
        public string merchantids { get; set; }
        public int merchantlevel { get; set; }

    }
}
