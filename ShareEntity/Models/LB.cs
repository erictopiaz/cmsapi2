using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShareEntity.Models
{
    /// <summary>
    /// Database Columns
    /// </summary>
    [Table("LB")]
    public sealed class LBs
    {
        public int id { get; set; }

        public string Account { get; set; }

        public int bet_type { get; set; }

        public long bet_id { get; set; }

        public int bet_win { get; set; }

        public decimal bet_money { get; set; }

        public decimal bet_winning { get; set; }

        public int bet_status { get; set; }

        public DateTime bet_time { get; set; }

        public DateTime trans_time { get; set; }

        public string match_area { get; set; }

        public long match_no { get; set; }

        public long? bet_no { get; set; }
    }
}
