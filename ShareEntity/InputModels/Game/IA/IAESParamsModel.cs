using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.IA
{
    /// <summary>
    /// 小艾电竞
    /// </summary>
    public class IAESParamsModel
    {
        public string AES_KEY { set; get; }
        public string AES_IV { set; get; }

        public string apiUrl { set; get; }
        public string pch { set; get; }

        public int isIdAccount { set; get; }
        public string prefix { set; get; }
        public string gameName { set; get; } = "IAES";
        /// <summary>
        /// 反向 访问游戏api
        /// </summary>
        public string proxy { set; get; }
    }

    public class IAESResponseModel<T>
    {
        /// <summary>
        /// //1 –成功
        /// </summary>
        public int code { set; get; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string msg { set; get; }
        public T data { set; get; }
    }

    public class IAESBody
    {
        /// <summary>
        /// 1 –成功
        /// </summary>
        public int status { set; get; }
    }

//    { 
//    ‘code’:1, //1 –成功，其他-失敗，參見錯誤碼約定
//    ‘msg’: ’ 注册成功’, //信息描述
//    ‘data’: {
//        "status": 1 //1 –成功
//    }, 
//}

    public class IAESLoginBody
    {
        public string token { set; get; }
        public string url  { set; get; }
    }

    public class IAESAesModel
    {
        /// <summary>
        /// 名文
        /// </summary>
        public string md5string { set; get; }
        public string auth_key { set; get; }
        /// <summary>
        /// aes加密名文
        /// </summary>
        public string aesString { set; get; }
        public string aesResult { set; get; }
    }

}
