using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    public partial class ofMinu
    {
        public long id { get; set; }

        public string currency { get; set; }

        public string account { get; set; }

        public decimal amount { get; set; }

        public decimal amountbefore { get; set; }

        public decimal amountafter { get; set; }

        public DateTime createtime { get; set; }

        public string createperson { get; set; }

        public DateTime? reviewtime { get; set; }

        public string reviewperson { get; set; }

        public string remarks { get; set; }

        public int status { get; set; }

        public string referrer { get; set; }

        public string referrerreg { get; set; }

        public int level { get; set; }

        public string referenceguid { get; set; }

        public int? gametype { get; set; }

        public string cancelreason { get; set; }

        public int? decreasetype { get; set; }
        public int accountType { set; get; }
        public int amountType { set; get; }
        public string orderNo { get; set; }
    }

    public class ofMinuEx: ofMinu
    {
        public string merchantAccount { get; set; }
        public int currencyType { set; get; }
    }
}
