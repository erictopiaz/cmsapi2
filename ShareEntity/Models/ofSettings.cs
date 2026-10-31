
using Dapper.Contrib.Extensions;

namespace ShareEntity.Models
{


    [Table("ofSettings")]
    public partial class ofSettings
    {
        //[Key]
        //public long id { get; set; }
        [ExplicitKey]
        public string code { get; set; }
        public string body { get; set; }
        public string description { get; set; }
        public int status { get; set; }
    }
}
