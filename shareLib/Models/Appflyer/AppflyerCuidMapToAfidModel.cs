using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Appflyer {
    public class AppflyerCuidMapToAfidModel {
        public long id {  get; set; }
        public string account { set; get; } = "h1290";
        public string cuid { get; set; }
        public string afid { get; set; }
        public long create_at { get; set; }
        public string create_at_str { set; get; }
        public string app_name { get; set; }


    }

    public class AppflyerInterCuidMapToAfid {
        public long id { get; set; }
        public string cuid { get; set; }
        public string afid { get; set; }

        /// <summary>
        /// 事件名
        /// </summary>
        public string event_name { set; get; }

        public string account { set; get; } = "";
        /// <summary>
        /// 1:android，2：ios,   3:h5  , 4: pc , 5: 体育ios,6: 体育android
        /// </summary>
        public int device { set; get; }
        public long eventTime { set; get; }

    }
}
