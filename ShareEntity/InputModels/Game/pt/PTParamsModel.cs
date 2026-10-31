using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.pt
{
    public class PTParamsModel
    {
        //{"clientname": "x","merchantcode": "x","api_url": "x","webgamelogin": "x","webgamelaunch": "x"}
        public string clientname { set; get; }
        public string merchantcode { set; get; }
        public string api_url { set; get; }
        public string webgamelogin { set; get; }
        public string webgamelaunch { set; get; }
        public string prefix { set; get; }
    }
}
