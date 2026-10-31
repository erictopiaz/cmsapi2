using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofWithdrawal")]
    public class ofWithdrawal
    {
        public DateTime? delayToTime { get; set; }
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(150)]
        public string realname { get; set; }

        public decimal amount { get; set; }

        public decimal fee { get; set; }

        [Required]
        [StringLength(50)]
        public string cardno { get; set; }

        [Required]
        [StringLength(150)]
        public string cardaddress { get; set; }

        [Required]
        [StringLength(50)]
        public string cardbank { get; set; }

        public DateTime requesttime { get; set; }

        [Required]
        [StringLength(50)]
        public string requestip { get; set; }

        [Required]
        [StringLength(150)]
        public string requestaddress { get; set; }

        public int status { get; set; }

        public DateTime? reviewtime { get; set; }

        [StringLength(50)]
        public string reviewperson { get; set; }

        public DateTime? paytime { get; set; }

        [StringLength(50)]
        public string payperson { get; set; }

        public DateTime? canceltime { get; set; }

        [StringLength(50)]
        public string cancelperson { get; set; }

        [StringLength(250)]
        public string cancelreason { get; set; }

        public decimal? balance { get; set; }

        [Required]
        [StringLength(150)]
        public string referrer { get; set; }

        [Required]
        [StringLength(150)]
        public string referrerreg { get; set; }

        public int level { get; set; }

        [StringLength(150)]
        public string referenceguid { get; set; }

        public int? gametype { get; set; }

        public int alert { get; set; }
        public int wid { get; set; }
        public int isTransferToManual { get; set; }
        public int pid { get; set; }
        public decimal fixWithdrawP { get; set; }
        public int usedFixDelayPromo { get; set; }

        public int? isview { get; set; }

        [StringLength(50)]
        public string affaccount { get; set; }

        [StringLength(50)]
        public string checkperson { get; set; }
        public string misc { get; set; }

        public int currencycode { get; set; }

        public string saler { get; set; }
    }


    public class ofWithdrawalEx: ofWithdrawal
    {
        /// <summary>
        /// ª·‘±id
        /// </summary>
        public long uid { get; set; }
    }
}
