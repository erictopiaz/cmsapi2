using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofNotice")]
    public partial class ofNotice
    {
        public long id { get; set; }

        [Required]
        [StringLength(500)]
        public string body { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public DateTime? expiredate { get; set; }

        public int status { get; set; }

        public int? type { get; set; }
        public int currencyType { get; set; }
        public string bodyLangs { get; set; } = "[]";
        /// <summary>
        /// Æ·ÅÆ
        /// </summary>
        public string n_merchantAccounts { get; set; } = "";
    }
}
