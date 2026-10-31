using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofBonusPromo")]
    public partial class ofBonusPromo: ofBonusPromoEx 
    {
        public long id { set; get; }
        public string currency { get; set; }
        public string account { get; set; }

        public string affaccount { set; get; }
        public decimal amount { get; set; }

        public decimal reward { get; set; }

        public DateTime createtime { get; set; }

        public string remarks { get; set; }

        public int level { get; set; }
        public string misc { set; get; }

        public int bet { get; set; }

        public string createperson { get; set; }

        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }

        public int? gametype { get; set; }

        public string referrer { get; set; }

        public string referrerreg { get; set; }

        public string referenceguid { get; set; }

        public int status { get; set; }

        public int visible { get; set; }

        public string ipaddress { get; set; }

        public string macaddress { get; set; }

        public int alert { get; set; }

        public string reason { get; set; }

        public long orderno { get; set; }
        /// <summary>
        /// 0:ºìÀû
        /// 1£ºÓÅ»İ
        /// </summary>
        public int bonusPromoType { set; get; }
       
    }

    public class ofBonusPromoEx {
        public int currencyType { get; set; }
        public int bonusPromoCode { set; get; }
        public string merchantAccount  { get; set; }
    }
}
