using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShareEntity.Models
{


    [Table("ofGameClass")]
    public partial class ofGameClassUpdate
    {
        [ExplicitKey]
        public int gameid { get; set; }

        public int? gamesubid { get; set; }

        [Required]
        [StringLength(50)]
        public string gamename { get; set; }

        public int gamestatus { get; set; }
        public int useable { get; set; }

        public DateTime createtime { get; set; }

    }

    [Table("ofGameClassMerchant")]
    public partial class ofGameClassMerchantUpdate
    {
        [ExplicitKey]
        public long id { get; set; }
       
        public int gameid { get; set; }

        public int? gamesubid { get; set; }

        [Required]
        [StringLength(50)]
        public string gamename { get; set; }

        public int gamestatus { get; set; }
        public int useable { get; set; }

        public DateTime createtime { get; set; }

    }

    [Table("ofGameClass")]
    public partial class ofGameClassPorxyUpdate {
        [ExplicitKey]
        public int gameid { get; set; }

        public string param { get; set; }


        public DateTime createtime { get; set; }

    }



    [Table("ofGameClass")]

    public partial class ofGameClass
    {
        [ExplicitKey]
        public int gameid { get; set; }

        public int? gamesubid { get; set; }

        [Required]
        [StringLength(50)]
        public string gamename { get; set; }

        public string customerName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string gamename2 { get; set; }

        public int gamestatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("class")]
        public int _class { get; set; }

        public DateTime createtime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("operator")]
        [Required]
        [StringLength(50)]
        public string _operator { get; set; }

        public string g_merchantAccounts { set; get; }

        public string currencyTypes { get; set; }
    }
}
