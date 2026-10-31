using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofTurnover")]
    public partial class ofTurnover
    {
        public string currency { get; set; } = "CNY";

        public string account { get; set; }

        public decimal totalbets { get; set; }

        public decimal effectivebets { get; set; }

        public decimal cmBet { get; set; } = 0;

        public decimal reward { get; set; }

        public decimal? winlost { get; set; }

        public decimal? leftbets { get; set; }

        public decimal? weekmaxbets { get; set; }

        public DateTime? begindate { get; set; }

        public DateTime? enddate { get; set; }

        public DateTime requesttime { get; set; }

        public DateTime? endtime { get; set; }

        public int gametype { get; set; }

        public int status { get; set; }

        public string cancelreason { get; set; }

        public string guid { get; set; }

        public string referrer { get; set; }

        public string referrerreg { get; set; }

        public int level { get; set; }

        public string reviewperson { get; set; }

        public DateTime? reviewtime { get; set; }

        public string cancelperson { get; set; }

        public DateTime? canceltime { get; set; }

        public string referenceguid { get; set; }

        public int? process { get; set; }

        public int alert { get; set; }

        /// <summary>
        /// 返水比例
        /// </summary>
        public decimal? calc { get; set; }
        /// <summary>
        /// 额外返水比例
        /// </summary>
        public decimal? eCalc { set; get; }
        public int platformGame { set; get; } = 0;
        public decimal bonusPoints { set; get; }
        public decimal pCalc { set; get; }
        /// <summary>
        /// 电销人员
        /// </summary>
        public string saler { set; get; }
        /// <summary>
        /// 代理账号
        /// </summary>
        public string aaffaccount { set; get; }
        /// <summary>
        /// 关联uid
        /// </summary>
        public long t_f_uid { set; get; }
    }
}
