using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShareEntity.Models
{







    [Table("ofManage")]
    public partial class ofManage
    {
        [ExplicitKey]
        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        [Required]
        [StringLength(50)]
        public string sex { get; set; }

        public int level { get; set; }

        [Required]
        [StringLength(50)]
        public string belongs { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public int status { get; set; }

        public int online { get; set; }

        public int forcequit { get; set; }

        public int loginnumber { get; set; }

        public DateTime? currentlogintime { get; set; }

        [StringLength(50)]
        public string currentloginip { get; set; }

        [StringLength(50)]
        public string currentloginaddress { get; set; }

        public DateTime? lastactivetime { get; set; }

        [StringLength(250)]
        public string remarks { get; set; }

        public int? failednumber { get; set; }

        public string log { get; set; }

        [StringLength(150)]
        public string lastloginoutip { get; set; }

        public DateTime? lastloginouttime { get; set; }
        public string access { get; set; }

        public string currencyTypes { get; set; }

        /// <summary>
        /// 市场人员管理 代理权限
        /// </summary>
        public int allowedToManageAgent { get; set; } = 0;

    }
}
