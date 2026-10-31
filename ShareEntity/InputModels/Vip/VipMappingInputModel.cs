//using CMS.Data.PageModel;
using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Vip
{
    /// <summary>
    /// 分配管理员联系方式到VIP
    /// </summary>
    public class VipMappingInputModel : PageBaseModel
    {
        [CharAndDigital(20, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "QQ格式有误")]
        public string qq { set; get; }

        [CharAndDigitalAndUnderScore(30, MinimumLength = 5, IsAllowedEmpty = true, ErrorMessage = "WX格式有误")]
        public string wx { set; get; }
        
        [UrlValidate(150, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "专员热线格式有误")]
        public string line { set; get; }

    }

    /// <summary>
    /// 分配管理员联系方式到VIP  查询返回数据
    /// </summary>
    public class VipMappingOutputModel
    {
        public long id { set; get; }
        public string account { set; get; }
        public string manage_account { set; get; }
        public string statusDesc { set; get; }
        private int _c_status;
        public int c_status {
            set {
                statusDesc = value == 0 ? "否" : "是"; 
                _c_status = value; 
            }
            get { return _c_status; }
        }
        private string _qq;
        public string qq { 
            set { _qq = string.IsNullOrEmpty(value) ? "" : Common.HideSensitiveInfo(value); }
            get { return _qq; }
        }
        private string _wx;
        public string wx {
            set { _wx = string.IsNullOrEmpty(value) ? "" : Common.HideSensitiveInfo(value); }
            get { return _wx; }
        }
        public string conformed_by { set; get; }
        public string conformed_at { set; get; }
        public string create_at { set; get; }
        public string rsDesc { set; get; }
        private int _r_status;
        public int r_status { 
            set {
                _r_status = value;
                rsDesc = _r_status == 0 ? "已经取消" : "可使用";
            }
            get { return _r_status; }
        }
        /// <summary>
        /// 专员热线
        /// </summary>
        public string line { set; get; }

        private string _message;
        public string message
        {
            set
            {
                _message = string.IsNullOrEmpty(value) ? "尊敬的客户，本月vip俱乐部推出特色大酬宾，参与就有机会中大奖，详情联系专属客服。" : value;
            }
            get { return _message; }
        }

    }
    /// <summary>
    /// 对已分配会员操作请求数
    /// </summary>
    public class ConformActionModel: AccountModel
    {
        /// <summary>
        /// 操作id
        /// </summary>
        public long id { set; get; }
    }

    /// <summary>
    /// 对已分配会员操作请求数
    /// </summary>
    public class ImportVIPContactModel : AccountModel
    {
        /// <summary>
        /// 会员列表
        /// </summary>
        public string content { set; get; }
        /// <summary>
        /// QQ id
        /// </summary>
        public long qid { set; get; }
        /// <summary>
        /// wx id
        /// </summary>
        public long wid { set; get; }
        /// <summary>
        /// 专员热线id （方便找网址）
        /// </summary>
        public long lid { set; get; }
        /// <summary>
        /// facebook id
        /// </summary>
        public long fid { set; get; }
        /// <summary>
        /// 电报 id
        /// </summary>
        public long tid { set; get; }
    }
}
