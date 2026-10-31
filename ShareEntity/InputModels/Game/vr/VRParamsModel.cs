using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.vr
{
    public class VRParamsModel
    {
        //{"merchantID": "x","merchantKey": "x","apiURL": "x","apiVersion": "x"}
        public string merchantID { set; get; }
        public string merchantKey { set; get; }
        public string apiURL { set; get; }
        public string apiVersion { set; get; }
      
        public string prefix { set; get; }
        public string gameName { set; get; } = "VR";
        /// <summary>
        /// 反向访问 游戏api
        /// </summary>
        public string proxy { set; get; }
    }
}
