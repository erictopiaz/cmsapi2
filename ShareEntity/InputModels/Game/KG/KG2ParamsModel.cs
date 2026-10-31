using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.KG
{
    /// <summary>
    /// kg2 和 kg1 平台参数model
    /// </summary>
    public class KG2ParamsModel
    {
        public string enterkeno { set; get; }
        public string fundinout { set; get; }
        public string getcredit { set; get; }
        public string profitloss { set; get; }
        public string vendorid { set; get; }
        public string currency { set; get; }
        public string language { set; get; }
        public string stake { set; get; }
        public string vendorsite { set; get; }
        public string fundlink { set; get; }
        /// <summary>
        /// 获取注单的API url
        /// </summary>
        public string betapi { set; get; }
        /// <summary>
        /// kg提供的token
        /// </summary>
        public string token { set; get; }
        public string prefix { set; get; }
    }


   
}
