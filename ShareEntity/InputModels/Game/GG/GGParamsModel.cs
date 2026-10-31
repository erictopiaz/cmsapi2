using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.GG
{
    public class GGParamsModel
    {
        //{"brandid": "x","api": "x","apikey": "x"}
        public string brandid { set; get; }
        public string api { set; get; }
        public string apikey { set; get; }
        public string version { set; get; }
        public string prefix { set; get; }
    }
}
