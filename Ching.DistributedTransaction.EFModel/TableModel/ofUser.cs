using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel
{
    [Table("ofUser")]
    public partial class ofUser
    {
        [Key]
        public long id { get; set; }
        public string account { get; set; }
    }
}
