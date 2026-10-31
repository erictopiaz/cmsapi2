using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Message
{

    public class LMessageInputModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { get; set; }
        public int accountType { set; get; } = -1;
        public int messageType { set; get; } = -1;

        public int replyStatus { set; get; } = -1;

        public int pageSize { set; get; }
        public int pageIndex { set; get; }
        public int currencycode { set; get; } = -1;

    }

    public class LMessageOutModel
    {
        
        public long id { get; set; }
        public string account { get; set; }
        public string createperson { get; set; }
        public string title { get; set; }
        private string _content;
        public string content
        {
            set
            {
                _content = Common.DESDeCode(value, "Msg*6Wc#");
            }
            get
            {
                return _content;
            }
        }
        public DateTime createtime { get; set; }
        public int status { get; set; }
        public DateTime? readtime { get; set; }
        public int type { get; set; }
        public string typename { get; set; }
        private string _replyMessage;
        public string replyMessage
        {
            set
            {
                _replyMessage = string.IsNullOrEmpty(value) ? "" : Common.DESDeCode(value, "Msg*6Wc#");
            }
            get
            {
                return _replyMessage;
            }
        }
        public string replyStatusDesc { set; get; }
        private int _replyStatus;
        public int replyStatus
        {
            set
            {
                _replyStatus = value;
                replyStatusDesc = value == 0 ? "等待回复" : value == 1 ? "已回复" : value == 2 ? "已取消" : "未知";
            }
            get
            {
                return _replyStatus;
            }
        }

        public string messageTypeDesc { set; get; }
        private int _messageType;
        public int messageType
        {
            set
            {
                _messageType = value;
                messageTypeDesc = value == 0 ? "充提款" : value == 1 ? "游戏" : value == 2 ? "客服" : value == 3 ? "VIP" : value == 4 ? "优惠" : value == 5 ? "其它" : "未知";
            }
            get
            {
                return _messageType;
            }
        }

        public string f_id { set; get; }

        public List<string> picBase64 { set; get; }

        public int device { set; get; }

        public string riskRemark { set; get; }
        public int riskTypesAready { set; get; }
        public int anomaly { set; get; }

    }


    public class LMessageReplyInputModel 
    {
        public long id { set; get; }
        /// <summary>
        /// 回复的内容
        /// </summary>
        public string reply { get; set; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string Admin { get; set; } = null;
    }


}
