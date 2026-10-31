using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofAffiliate")]
    public partial class ofAffiliate
    {
        public long id { get; set; }

        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [StringLength(50)]
        public string bbaccount { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        [StringLength(50)]
        public string realname { get; set; }

        public int? sex { get; set; }

        [StringLength(50)]
        public string birthday { get; set; }

        [StringLength(50)]
        public string country { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string telephone { get; set; }

        [StringLength(50)]
        public string qq { get; set; }

        [StringLength(50)]
        public string website { get; set; }

        [StringLength(50)]
        public string adlink { get; set; }

        [StringLength(50)]
        public string adcode { get; set; }

        [StringLength(50)]
        public string cardno { get; set; }

        [StringLength(50)]
        public string cardname { get; set; }

        [StringLength(50)]
        public string cardaddress { get; set; }

        [StringLength(50)]
        public string ew_uuid { get; set; }

        [StringLength(50)]
        public string pt_uuid { get; set; }

        [StringLength(50)]
        public string sun_uuid { get; set; }

        [StringLength(250)]
        public string remarks { get; set; }

        public int status { get; set; }

        public int verify { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public decimal alloc { get; set; }

        [StringLength(50)]
        public string verifyperson { get; set; }

        public DateTime? verifytime { get; set; }

        public decimal? balance { get; set; }

        public decimal? unavailable { get; set; }

        [StringLength(50)]
        public string ipaddress { get; set; }

        [StringLength(50)]
        public string macaddress { get; set; }

        [StringLength(50)]
        public string lastipaddress { get; set; }

        [StringLength(50)]
        public string lastmacaddress { get; set; }

        public int? adlinkhit { get; set; }

        public int? adcodelock { get; set; }

        [StringLength(200)]
        public string commission { get; set; }

        [StringLength(50)]
        public string upperlevel { get; set; }

        public string agentMarket { get; set; }
    }
}
