using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel
{
    [Table("ofUserGameWallet")]
    public partial class UserGameWallet
    {
        [Key]
        public long id { get; set; }
        public string account { get; set; }
        public decimal wallet { set; get; }
        public string affaccount { set; get; } = "";
    }
}
