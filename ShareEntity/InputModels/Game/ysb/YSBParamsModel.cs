using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.ysb
{
    public class YSBParamsModel
    {
        //{"key": "x","mid": "x","spi": "x","url": "x"}
        // string vendorId, string loginUrl, string apiUrl, string prefix
        //{"prefix": "x","webloginurl": "x","mobileloginurl": "x","vendorid": "x","apiurl": "x"}
        public string vendorid { set; get; }
        public string webloginurl { set; get; }
        public string mobileloginurl { set; get; }
        public string apiurl { set; get; }
        public string thirdPartyID { set; get; }
        public string prefix { set; get; }
    }
}
