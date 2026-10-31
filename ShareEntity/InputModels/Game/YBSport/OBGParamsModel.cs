using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.YBSport
{
    /// <summary>
    /// yabo api configs
    /// </summary>
    public class OBGParamsModel
    {
        public string merchantCode { set; get; }
        public string md5Key { set; get; }
        public string apiUrl { set; get; }
        /// <summary>
        /// 通过代理ip 去请求数据
        /// </summary>
        public string apiProxy { set; get; }
        public int porxyPort { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "PM";// "OBG";
        /// <summary>
        /// 通过代理ip 去请求数据
        /// </summary>
        public string proxy { set; get; }
    }


//    商户名称：awc888
//商户编号：LG68
//商户秘钥：oF$yQO1xPmQyc$CLb1LkM2kz_2M7yT
//商户后台登录用户名：awc888admin
//商户后台登录密码：awc888adminLG68
//商户后台地址 http://merchant-own-backstage.sportxxx278gwf4.com/
//对接API域名： https://sandbox-gateway.sportxxxw1box.com


}
