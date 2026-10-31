using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using CMS.Data.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.提款管理;
using CMS.Core.ViewModel;
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetUserMenu : IRequest<ResponseModel<IEnumerable<GetUserMenuReturn>>>
    {
        public string account { get; set; }
        //[JsonIgnore]
        //public Dictionary<string, string> MenuList { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

    }

    public class GetUserMenuReturn
    {
        public string menukey { get; set; }
        public string group { get; set; }
        public string menuname { get; set; }
        public bool status { get; set; }

    }

    public class GetUserMenuValidator : AbstractValidator<GetUserMenu>
    {
        public GetUserMenuValidator()
        {

            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetUserMenuHandler : CMS.Core.BaseRequestHandler<GetUserMenu, ResponseModel<IEnumerable<GetUserMenuReturn>>>
    {
        public static readonly Dictionary<string,string> MENULIST = new Dictionary<string, string>() {
            {"/dashboard", "首页"},
            {"/member", "会员管理"},
            {"/member/list", "会员列表"},
            {"/member/relate", "关联搜索"},
            {"/member/depositfirst", "首存用户"},
            {"/member/depositors", "账户存款"},
            {"/member/withdeposit", "存款用户"},
            {"/member/sendmail", "邮件发送"},
            {"/member/transactionrecord", "会员交易"},
            {"/member/level", "账户等级"},
            {"/member/report", "会员报表"},
            {"/deposit", "存款管理"},
            {"/deposit/internal", "内充列表"},
            {"/deposit/atm", "网银/ATM"},
            {"/deposit/thirdpay", "第三方存款列表"},
            {"/deposit/thirdpayfail", "第三方失败"},
            {"/deposit/crosstransact", "互转列表"},
            {"/deposit/crosstransactalipay", "互转【支付宝】"},
            {"/deposit/crosstransactwechat", "互转【微信】"},
            {"/withdraw", "提款管理"},
            {"/withdraw/NewUserRecord", "新会员"},
            {"/withdraw/NewUserDirectPayRequest", "代付【新会员】"},
            {"/withdraw/RegularUserDirectPayRequest", "代付【普通】"},
            {"/withdraw/VipUserDirectPayRequest", "代付【VIP】"},
            {"/withdraw/VIP3DirectPayRecord", "代付【VIP3】"},
            {"/withdraw/USDTDirectPayRecord", "代付【USDT】"},
            {"/withdraw/QRManualPay", "代付【支付宝/微信】"}, //ching add at 20241029
            {"/withdraw/RegularBigWithdrawRequest", "大额【普通】"},
            {"/withdraw/VipBigWithdrawRequest", "大额【VIP】"},
            {"/withdraw/Vip3BigWithdrawRequest", "大额【VIP3】"},
            {"/withdraw/bdetailed", "代付【详细】"},
            {"/withdraw/bdetailedusdt", "代付【USDT详细】"},
            {"/withdraw/PaymentRecord", "支付记录"},
            {"/withdraw/ManualPayRequestRegular", "卡付【普通】"},
            {"/withdraw/ManualPayRequestVIP", "卡付【VIP】"},

            //{"/withdraw/RegularUserRecord", "万象会员"},
            {"/withdraw/riskmgmtrecord", "提款【风控】"},
            {"/withdraw/allunprocessed", "提款审核"},
            //{"/withdraw/DirectPayRequest", "代付【所有】"},
            //{"/withdraw/AllUserDirectPayRequest", "代付【所有】"},
            //{"/withdraw/VIP1Record", "VIP1提款"},
            //{"/withdraw/VIP2Record", "VIP2提款"},
            //{"/withdraw/VIP2DirectPayRecord", "代付【VIP2】"},
            //{"/withdraw/VIP3Record", "VIP3提款"},
            //{"/withdraw/VIP4Record", "VIP4提款"},
            {"/vip", "VIP管理"},
            {"/vip/month", "VIP活跃统计"},
            {"/vip/levelup", "VIP月升级数据"},
            {"/vip/leveldown", "VIP月降级数据"},
            {"/vip/vipmapping", "VIP联系分配置"},
            {"/vip/vipsummary", "每天存款次数"},
            {"/vip/freebet", "免费筹码申请"},


            {"/call", "电销管理"},
            {"/call/report", "电销报表"},
            {"/call/saleList", "电销会员【查询】"},
            {"/call/log", "备注记录"},
            {"/call/log_stg", "备注记录【旧】"},
            {"/call/logreport", "备注报表"},
            {"/call/activity", "活跃记录"},
            {"/call/jobs", "电销任务"},

            {"/game", "游戏管理"},
            {"/game/transferold", "游戏转账(旧)"},
            {"/game/report", "盈利报表"},
            {"/game/record", "游戏记录"},
            {"/game/gameReportDaily", "游戏日报表记录"},
            {"/game/gameresize", "游戏金额变动INTL"},
            {"/game/gameorders", "游戏注单INTL"},
            {"/game/platform", "游戏分类"},
            {"/game/platproxy", "游戏代理配置"},
            {"/game/gamelistedit", "游戏列表"},

            {"/base", "优惠管理"},
            {"/promo/bonusv", "红利优惠"},
            {"/promo/selfbonus", "福利中心【自助】"},
            {"/promo/bonussettings", "自助优惠设置"},
            {"/promo/bonusold", "红利优惠(旧)"},
            {"/promo/bonuspending", "周周挑战"},
            {"/promo/turnover", "返水列表"},
            {"/promo/resize", "余额调整"},
            {"/promo/promocode", "优惠代码"},
            {"/promo/promomisc", "积分管理"},
            {"/promo/promospread", "推荐管理"},
            {"/promo/spreadbilled", "推荐月结算"},
            {"/promo/spreadbilledintl", "推荐月结算INTL"},
            {"/promo/spreadmembersintl", "推荐下线INTL"},
            {"/promo/promocodeset", "优惠代码设定"},
            {"/promo/eurocup", "欧洲杯竞猜"},

            {"/website", "网站管理"},
            {"/website/notice", "公告管理"},
            {"/website/promoit", "优惠活动管理"},
            {"/website/store", "积分商城"},
            {"/website/appconfig", "APP管理"},
            {"/website/weblinks", "网站链接设置"},
            {"/website/appbanner", "APP优惠图片"},
            {"/website/message", "站内信"},
            {"/website/lmessage", "留言管理"},
            {"/website/netbank", "银行卡管理"},
            {"/website/wthirdpay", "商户管理"},
            {"/website/directPay", "代付管理[调"},
            {"/website/directPayINR", "代付管理【INR】"},//ching add 2023-11-24
            {"/website/validatecode", "验证码查询"},
            {"/website/webvariables", "网站参数"},
            {"/website/blacklist", "黑名单配置"},
             {"/website/domainauth", "域名登录统计"},//ching add 2023-0607
            {"/website/analytics", "统计计算"},

            {"/agent", "合营管理"},
            {"/agent/player", "下线会员"},
            {"/agent/aregister", "合营注册"},
            {"/agent/agentverified", "现有合营"},
            {"/agent/agentSpread", "推广报表"},
            {"/agent/agentfinancedaily", "合营财务报表"},
            {"/agent/agentgamedaily", "合营游戏数据"},
            {"/agent/agentbillmonthauto", "合营月佣金报表"},
            {"/agent/agentbillmonth", "佣金结算记录"},
            {"/agent/agentwithdraw", "提款记录"},
            {"/agent/agentpointsrecord", "合营账变记录"},
            {"/agent/agentresize", "合营微调"},
            {"/agent/agentbonus", "合营优惠红利"},
            {"/agent/playerbonus", "下线会员优惠红利"},

             #region 代理菜单
            {"/aff", "代理管理"},
            {"/aff/affplayer", "代理会员"},
            {"/aff/affplayerbonus", "会员优惠红利"},
            {"/aff/affregister", "待审核代理"},
            {"/aff/affverified", "已审核代理"},
            {"/aff/affSpread", "推广报表"},
            {"/aff/afffinancedaily", "代理财务报表"},
            {"/aff/affgamedaily", "代理游戏数据"},
            {"/aff/affbillmonth", "代理结算记录"},
            {"/aff/affwithdraw", "代理提款记录"},
            {"/aff/affplayerdeposit", "代理会员充值记录"},
            {"/aff/affplayerwithdraw", "代理会员提款记录"},
            #endregion

            {"/system", "系统管理"},
            {"/system/userlist", "后台用户管理"},
            {"/system/iplist", "IP白名单列表"},
            {"/system/bankcode", "银行代码列表"},
            {"/system/adminrecord", "管理员日志列表"},
            {"/system/settings", "参数配置"},
            {"/system/ipblack", "IP黑名单"},
            {"/system/cardblack", "银行卡黑名单"},

           

        };

        public GetUserMenuHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<GetUserMenuReturn>> Handle(GetUserMenu request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var row = connection.Get<ofManage>(request.account);
                List<string> privilegeList = new List<string>();

                if (!string.IsNullOrEmpty(row.access))
                {
                    var access = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.access);
                    if (access.ContainsKey("menu"))
                        privilegeList.AddRange(access["menu"].Split('|'));
                }

                var responseList = new List<GetUserMenuReturn>();

                foreach (var item in MENULIST)
                {
                    string _group = "";
                    if (item.Key.IndexOf('/', 1) > 0)
                        _group = item.Key.Substring(1, item.Key.IndexOf('/', 1));
                    else
                        _group = item.Key.Substring(1);
                    _group = _group.Replace("/", "");
                    // Extract GROUP
                    responseList.Add(new GetUserMenuReturn()
                    {
                        menukey = item.Key,
                        menuname = item.Value,
                        group = _group,
                        status = request.Admin=="king" && request.account== "king" ? true:privilegeList.Contains(item.Key)
                    });
                }

                return new ResponseModel<IEnumerable<GetUserMenuReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = responseList
                };
            }

        }
    }
}
