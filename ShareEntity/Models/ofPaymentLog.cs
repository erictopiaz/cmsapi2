
using Dapper.Contrib.Extensions;
using System;

namespace ShareEntity.Models
{

    [Table("ofPaymentLog")]
    public partial class ofPaymentLog
    {
        public int Id { get; set; }

        [ExplicitKey]
        public string TransactionId { get; set; }
        public long TransId{ get; set; }
        public string Account { get; set; }

        public string Realname { get; set; }

        public string Cardno { get; set; }

        public decimal Amount { get; set; }

        public DateTime Createtime { get; set; }

        public string Result { get; set; }

        public string StatusCode { get; set; }

        public string StatusMsg { get; set; }

        public string misc { get; set; }
        public string Bank { get; set; }
        public string orderid { get; set; }
        public string platform { get; set; }
        public string payadmin { get; set; }
        public int transtype { get; set; }
    }
}
