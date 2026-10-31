using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofMail")]
    public partial class ofMail
    {
        public long id { get; set; }
        public string account { get; set; }
        public string createdby{ get; set; }
        public DateTime createtime { get; set; }
        public DateTime? sendtime { get; set; }
        public DateTime? canceltime { get; set; }
        public int status { get; set; }
        public int mailtype { get; set; }
        public string template { get; set; }
        public string email{ get; set; }
        public string remark { get; set; }
        public string templatedata { get; set; }
    }
}
