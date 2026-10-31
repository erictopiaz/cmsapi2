using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{

    [Table("ofPaymentRecord")]
    public partial class ofPaymentRecord
    {
     //   [id] [bigint] IDENTITY(10000,1) NOT NULL,
     //   [currency] [varchar] (20) NOT NULL,
     //   [account] [varchar] (50) NOT NULL,
     //   [transactiontype] [int] NOT NULL,
     //   [createtime] [datetime]        NOT NULL,
     //   [summary] [nvarchar] (250) NOT NULL,
     //   [referenceguid] [varchar] (50) NOT NULL,
     //   [preBalance] [decimal](18, 2) NULL,
	    //[bonus] [decimal](18, 2) NULL,
	    //[afterBalance] [decimal](18, 2) NOT NULL,
     //   [status] [int] NOT NULL,
     //   [summaryType] [int] NOT NULL,

        public long id { get; set; }

        [Required]
        [StringLength(20)]
        public string currency { get; set; }

        [Required]
        [StringLength(50)]
        public string account { get; set; }

        [Required]
        public int transactiontype { get; set; }

        public DateTime createtime { get; set; }

        [Required]
        [StringLength(250)]
        public string summary { get; set; }

        [Required]
        [StringLength(50)]
        public string referenceguid { get; set; }



        public decimal preBalance { get; set; }

        public decimal bonus { get; set; }

        public decimal afterBalance { get; set; }

        public int status { get; set; }
        public int summaryType { get; set; }
    }
}
