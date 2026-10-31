using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Euro
{
    public class BetTeamModel
    {
        public long id { set; get; }
        public string account { set; get; }
        public string betTeams { set; get; }

        public decimal reward { set; get; }

        public int groupid { set; get; }
        public int euroname { set; get; }
    }
}
