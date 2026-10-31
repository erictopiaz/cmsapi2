using System;
using System.Collections.Generic;
using System.Text;

namespace GamePlatformBillFee.models
{
    public class AFMontBillReportInrModel : BillReportBaseModel
    {
        
        public decimal? pg_bet { get; set; }
        public decimal? pg_winlost { get; set; }

        public decimal? jdb_bet { get; set; }
        public decimal? jdb_winlost { get; set; }

        public decimal? evo_bet { get; set; }
        public decimal? evo_winlost { get; set; }

        public decimal? turbo_bet { get; set; }
        public decimal? turbo_winlost { get; set; }


        public decimal? spribe_bet { get; set; }
        public decimal? spribe_winlost { get; set; }


        public decimal? b9w_bet { get; set; }
        public decimal? b9w_winlost { get; set; }


        public decimal? jl_bet { get; set; }
        public decimal? jl_winlost { get; set; }



    }
}
