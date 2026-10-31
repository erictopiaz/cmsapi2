using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Models
{
    public partial class OfLMessage
    {
        public long id { get; set; }

        public string account { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        public DateTime createtime { get; set; }

        public string createperson { get; set; }

        public int status { get; set; }

        public DateTime? readtime { get; set; }

        public string location { get; set; }

        public int? type { get; set; }

        public int? visible { get; set; }
    }
}
