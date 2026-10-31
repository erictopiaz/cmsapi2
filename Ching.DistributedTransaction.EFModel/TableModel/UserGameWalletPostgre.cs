using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel
{
    [Table("UserGameWallet")]
    public class UserGameWalletPostgre
    {
        [Key]
        public string account { get; set; }
        public long uid { get; set; }
        public string affaccount { set; get; }
        public decimal wallet { set; get; }
        public string merchant_account { set; get; }
        public int currency_type { set; get; }
    }




}
