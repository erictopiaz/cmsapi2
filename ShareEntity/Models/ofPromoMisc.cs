using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofPromoMisc")]
    public partial class ofPromoMisc
    {
        public long id { get; set; }

        public string account { get; set; }

        public int type { get; set; }

        public string content { get; set; }
        public DateTime createtime { get; set; }

        public decimal points { get; set; }

        public int status { get; set; }
        public string createperson { get; set; }


        public string reviewperson { set; get; }
    }
}
