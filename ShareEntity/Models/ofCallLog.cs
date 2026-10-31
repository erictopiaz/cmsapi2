using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofCallLog")]
    public partial class ofCallLog
    {



        //id, comment, createtime, createperson, status, maxamount, starttime
        /*
	            [id] [bigint] IDENTITY(10000,1) NOT NULL,
	            [comment] [text] NOT NULL,
	            [createtime] [datetime] NOT NULL,
	            [account] [varchar](50) NOT NULL,
	            [telephone] [varchar](50) NOT NULL,
	            [createperson] [varchar](50) NOT NULL,
	            [status] [int] NOT NULL,
             */
        public long id { get; set; }

        [Required]
        [StringLength(8000)]
        public string comment { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        [Required]
        [StringLength(50)]
        public string account{ get; set; }

        [Required]
        [StringLength(50)]
        public string telephone{ get; set; }


        //[Required]
        //[StringLength(50)]


        public int status { get; set; }

        public int contactStatus { set; get; } = -1;
        /// <summary>
        /// ��עʱ��
        /// </summary>
        public DateTime calltime { set; get; }
    }
}
