using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.cq
{
    public class CQParamModel
    {
        public string token { set; get; }
        
        public string apiUrl { set; get; }
        
        public string apigamerurl { set; get; }
        public string prefix { set; get; }
        public int gamestatus { set; get; }
        public string gameName { set; get; } = "CQ9";
    }


    #region 创建账号返回数据
    public class CQ9ResponseModel<T>
    {
        public T data { set; get; }
        public CQ9Status status { set; get; }
    }

    public class Account
    {
        public string account { set; get; }
        public string nickname { set; get; }
        public string password { set; get; }
    }

    #endregion
    public class CQ9Status
    {
        public string code { set; get; }
        public string message { set; get; }
        public string datetime { set; get; }
    }

    public class CQ9Balance
    {
        public string balance { set; get; }
        public string currency { set; get; }
    }
    public class CQ9GameToken
    {
        public string usertoken { set; get; }
    }
    public class CQ9GameUrl
    {
        public string url { set; get; }
        public string token { set; get; }
    }
}
