using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogLib.Models
{
    /// <summary>
    /// 日志数据结构
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// 日志单号
        /// </summary>
        public long logno { set; get; }
        /// <summary>
        /// 操作写日志的账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 账号类型
        /// 0：会员    1：管理员     2：代理
        /// </summary>
        public int accounttype { set; get; }
        /// <summary>
        /// 日志创建时间
        /// </summary>
        public string create_at { set; get; }
        /// <summary>
        /// 日志类型
        /// 0：存款，1：取款，2：红利 ,3：优惠，4：返水，5：会员 ，6：游戏，7：合营，8：佣金， 
        /// 9:公告，10：活动内容，11：站内信，12：其它,13:会员加码比例， 15：修改手机号，16：绑卡，17:删除卡, 18:会员商户特别调整, 
        /// 19:会员是否参与推荐好友结算，20:代理商户特别调整, 21：会员登录， 22： 黑名单调整，23：管理员登录, 25:修改会员实名列表
        /// </summary>
        public int logtype { set; get; }
        /// <summary>
        /// 0:全部，1：新增，2：修改，3：取消，4：审核，5：支付，6：导出，7：导入，8：其它,9:删除
        /// </summary>
        public int actionType { set; get; }
        /// <summary>
        /// 当前日志描述
        /// </summary>
        public string logmessage { set; get; }
        /// <summary>
        /// 请求日志的IP
        /// </summary>
        public string ipaddress { set; get; }
        /// <summary>
        /// 日志操作的相关账号
        /// </summary>
        public string relationAccount { set; get; }
        /// <summary>
        /// 日志的操作结果
        /// 默认为1:操作成功,0:操作失败
        /// </summary>
        public int logresult { set; get; } = 1;

        public string logoDetail { set; get; } = "";

    }


    public class LogResponseModel
    {
        /// <summary>
        /// 日志单号
        /// </summary>
        public long logno { set; get; }
        /// <summary>
        /// 操作写日志的账号
        /// </summary>
        public string account { set; get; }

        public string accounttypeDesc { set; get; }
        /// <summary>
        /// 账号类型
        /// 0：会员    1：管理员     2：代理
        /// </summary>
        private int _accounttype;
        public int accounttype {
            set
            {
                accounttypeDesc = value == 0 ? "会员" : value == 1 ? "管理员" : "代理";
                _accounttype = value;
            }
            get { return _accounttype; }
        }
        /// <summary>
        /// 日志创建时间
        /// </summary>
        public string create_at { set; get; }
        /// <summary>
        /// 日志类型
        /// 0：存款，1：取款，2：红利 ,3：优惠，4：返水，5：会员 ，6：游戏，7：合营，8：佣金， 9:公告，10：活动内容，11：站内信，12：其它
        /// </summary>
        public int logtype { set; get; }
        /// <summary>
        /// 0:全部，1：新增，2：修改，3：取消，4：审核，5：支付，6：导出，7：导入，8：其它
        /// </summary>
        public int actionType { set; get; }
        /// <summary>
        /// 当前日志描述
        /// </summary>
        public string logmessage { set; get; }
        /// <summary>
        /// 请求日志的IP
        /// </summary>
        public string ipaddress { set; get; }
        /// <summary>
        /// 日志操作的相关账号
        /// </summary>
        public string relationAccount { set; get; }
        /// <summary>
        /// 日志的操作结果
        /// 默认为1，操作成功
        /// </summary>
        public int logresult { set; get; } = 1;

    }
}
