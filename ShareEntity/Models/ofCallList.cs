using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{






    [Table("ofCallList")]
    public partial class ofCallList
    {



        /*
	        [id] [bigint] IDENTITY(10000,1) NOT NULL,
	        [createtime] [datetime] NOT NULL,
	        [calltime] [datetime] NOT NULL,
	        [account] [varchar](50) NOT NULL,
	        [createperson] [varchar](50) NOT NULL,
	        [callperson] [varchar](50) NOT NULL,
	        [status] [int] NOT NULL,
        */


        public long id { get; set; }

        public DateTime createtime { get; set; }
        public DateTime calltime { get; set; }

        [Required]
        [StringLength(50)]
        public string account{ get; set; }

        [Required]
        [StringLength(50)]
        public string createperson { get; set; }

        [Required]
        [StringLength(50)]
        public string callperson { get; set; }

        public int status { get; set; }
        
    }
}
