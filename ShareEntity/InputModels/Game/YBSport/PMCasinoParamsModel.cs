using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.YBSport {
    /// <summary>
    /// pm 真人游戏请求参数
    /// </summary>
    public class PMCasinoParamsModel {

        //{"apiurl":"https://api.obothapi.com","merchantCode":"8RK76","Aeskey":"","MD5Key":"","prefix":"","gamestatus":1 }

        /// <summary>
        /// 商户编码 ，会员账号 loginName =   merchantCode + 会员账号
        /// </summary>
        public string merchantCode { set; get; }
        public string Aeskey { set; get; }
        public string MD5Key { set; get; }
        public string apiUrl { set; get; }

        /// <summary>
        /// 是否id 为会员游戏 平台账 号
        /// 0：否，1：是
        /// </summary>
        public int isIdAccount { set; get; }

        /// <summary>
        /// 暂无使用
        /// </summary>
        public string loginUrl { set; get; }
        /// <summary>
        /// 失败后返回页面
        /// </summary>
        public string backUrl { set; get; } = "";

        public string prefix { set; get; }

        public string gameName { set; get; } = "DB";
        public string proxy { set; get; }
    }


    public class PMCasinoPostModel {
        public string merchantCode { set; get; }
        public string signature { set; get;}

        [JsonProperty(PropertyName= "params")]
        public string param { set; get; }
    }

    public class PMCasinoResponseBase {
        public string code { set; get; }
        public string message { set; get; }
    }

    public class PMCasinoResponseModel<T,K> : PMCasinoResponseBase 
    {
        public K request { set; get; }
        public T data { set; get; }
    }
    public class PMCBalance {
        public string balance { set; get; }
    }
    public class PMCCreateRequest {
        public string loginName { set; get; }
        public string loginPassword { set; get; }
        public int oddType { set; get; }
        public int lang { set; get; }
        public long timestamp { set; get; }

        public int deviceType { set; get; }

        public string transferNo { set; get; }

        public decimal amount { set; get; }

        // "loginName":"j7l6clark00007",
        //"loginPassword":"",
        //"oddType":4333,
        //"lang":1,
        //"timestamp":1581849859956
    }
    public class PMCCreate {
        public string create { set; get; } = "";
        public string deposit { set; get; } = "";
        public string url { set; get; } = "";
    }

    public class PMCTransfer {
        public string deposit { set; get; } = "";
        public decimal balance { set; get; }
    }
}
