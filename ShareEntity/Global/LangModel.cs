using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.Global {
    /// <summary>
    /// 多语系模型
    /// </summary>
    public class LangModel {
        public int lang { set; get; }
        public string body { set; get; }
    }



    /// <summary>
    /// 多语系模型
    /// </summary>
    public class MessageLangModel {
        public int lang { set; get; }
        public MessageModel messageModel { set; get; } = new MessageModel();
    }
    /// <summary>
    /// 消息标题和消息内容
    /// </summary>
    public class MessageModel {
        public string title { set; get; }
        public string content { set; get; }
    }
}
