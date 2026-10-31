using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.KG
{
    public class KGParamsModel
    {
        public string VendorId { set; get; }
        public string VendorSite { set; get; }
        public string ApiUrl { set; get; }
        public string Apikey { set; get; }
        public string Trial { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "KG3";

        public string kg3Proxy { set; get; }
        public string proxy { set; get; }
    }
}
