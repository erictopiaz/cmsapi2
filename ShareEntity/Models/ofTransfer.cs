using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofTransfer")]
    public partial class ofTransfer
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        public decimal amount { get; set; }

        public int type { get; set; }

        [StringLength(150)]
        public string promotioncode { get; set; }

        public DateTime requesttime { get; set; }

        public DateTime? endtime { get; set; }

        public decimal leftamountbefore { get; set; }

        public decimal leftamountafter { get; set; }

        public decimal rightamountbefore { get; set; }

        public decimal rightamountafter { get; set; }

        public int status { get; set; }

        [StringLength(250)]
        public string acode { get; set; }

        [Required]
        [StringLength(250)]
        public string elementid { get; set; }

        [Required]
        [StringLength(250)]
        public string referrerno { get; set; }

        [StringLength(250)]
        public string paymentid { get; set; }

        [StringLength(250)]
        public string errordesc { get; set; }

        [Required]
        [StringLength(150)]
        public string requestip { get; set; }

        [Required]
        [StringLength(150)]
        public string requestaddress { get; set; }

        [Required]
        [StringLength(150)]
        public string referrer { get; set; }

        [Required]
        [StringLength(150)]
        public string referrerreg { get; set; }

        public int level { get; set; }

        [StringLength(50)]
        public string referenceguid { get; set; }

        [StringLength(8000)]
        public string xmlapi { get; set; }

        [Column("operator")]
        [StringLength(50)]
        public string _operator { get; set; }

        public int? transtatus { get; set; }

        public int? process { get; set; }

        public int? alert { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }
    }
}
