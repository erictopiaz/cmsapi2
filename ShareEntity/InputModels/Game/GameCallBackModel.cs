using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Game
{
    public class GameCallBackModel
    {
        /// <summary>
        /// lb 网页版回调认数据
        /// </summary>
        public string lbXmlData { set; get; }
        /// <summary>
        ///  ag回调认证数据
        /// </summary>
        public string agXmlData { set; get; }
        #region xj登录验证
        /// <summary>
        /// xj  token
        /// </summary>
        public string xjToken { set; get; }

        public object carrier { set; get; }
        public XJTokenModel data { set; get; }
        #endregion

        /// <summary>
        /// ysb  token
        /// </summary>
        public string ysbToken { set; get; }

        /// <summary>
        /// gg  json
        /// </summary>
        public string ggJsonData { set; get; }

        [CharAndDigital(12, MinimumLength = 6, IsAllowedEmpty = true, ErrorMessage = "账号要求6到12个字符")]
        public string account { set; get; }
        public int gameid { set; get; }
    }

    #region xj登录验证
    
    public class XJTokenModel {
        public string token { set; get; }
    }
    #endregion
}
