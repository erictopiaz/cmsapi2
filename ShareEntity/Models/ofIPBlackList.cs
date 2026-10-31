using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShareEntity.Models {
    [Table("ofBank")]
    public partial class ofIPBlackList {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(50)]
        public string blackip { get; set; }

        [StringLength(150)]
        public string fg { get; set; }

        [StringLength(50)]
        public string who_add { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public int ip_status { get; set; }
        /// <summary>
        /// 默认为 0  存款刷单黑名单
        /// </summary>
        public int limit_type { get; set; }
    }
}
