//using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofMessage")]
    public partial class ofMessage
    {
        public long id { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        [StringLength(500)]
        public string title { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string content { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        public int status { get; set; }
        //public int accountType{ get; set; }

        public DateTime? readtime { get; set; }

        [StringLength(50)]
        public string location { get; set; }
        /// <summary>
        /// 0:默认， 1：优惠活动，2：游戏比赛， 3：紧急通知，4：其它，5：sms 短信， 6：留言回复
        /// </summary>
        public int? type { get; set; }

        public int? visible { get; set; }

        public int accountType { set; get; } = 0;
      

        public string messageLangs { get; set; }
    }



    public partial class ofLMessage : ofMessage
    {
        
        /// <summary>
        /// 0:未回复留言，1：已经回复
        /// </summary>
        public int replyStatus { set; get; }
    }
}
