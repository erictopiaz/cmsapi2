
using Dapper.Contrib.Extensions;
using System;

namespace CMS.Data.Models
{
    /*
CREATE TABLE [dbo].[ofStatus](
	[id] [bigint] IDENTITY(10000,1) NOT NULL,
	[identifier] [varchar](25) NOT NULL,
	
	[idtype] int NOT NULL DEFAULT 0,
	[status] int NOT NULL DEFAULT 0,
	[body] varchar(100), NULL,
	[createtime] [datetime] NOT NULL
	[createperson] varchar NOT NULL
	)
     */

    [Table("ofStatus")]
    public partial class ofStatus
    {
        //[Key]
        //public long id { get; set; }
        [ExplicitKey]
        public string identifier { get; set; }
        public int idtype { get; set; }
        public int status { get; set; }
        public string body { get; set; }
        public DateTime createtime { get; set; }
        public string createperson { get; set; }
    }
}
