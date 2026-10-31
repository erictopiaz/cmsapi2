using System;
using System.Collections.Generic;
using System.Text;

namespace AWC2019.Core.ViewModel
{
    public class LBModel
    {
        public long? BetID { get; set; }

        public string Account { get; set; }

        public decimal Bet { get; set; }

        public decimal Winnings { get; set; }

        public int isWin { get; set; }

    }
}
