using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofLog")]
    public partial class ofLog
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
        [StringLength(20)]
        public string account{ get; set; }

        
        [Required]
        [StringLength(15)]
        public string ip{ get; set; }
        
        [Required]
        public byte[] ipbin{ get; set; }

        //[DisplayColumn("event")]
        [Required]
        public int _event { get; set; }
        
        [Required]
        public int l_device{ get; set; }
        
        [Required]
        public int event_type{ get; set; }


        //[Required]
        //public int event { get; set; }

        [Required]
        [StringLength(100)]
        public string comment { get; set; }
        
        [Required]
        [StringLength(100)]
        public string remark { get; set; }


        public DateTime createtime { get; set; }

    }
}
