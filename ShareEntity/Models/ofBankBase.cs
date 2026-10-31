using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{




    [Table("ofBankBase")]
    public partial class ofBankBase
    {
        public long id { get; set; }
        public long bid { get; set; }
        public string name{ get; set; }
        public string icon { get; set; }
        public string url{ get; set; }
        public int currencycode { get; set; }

    }

}
