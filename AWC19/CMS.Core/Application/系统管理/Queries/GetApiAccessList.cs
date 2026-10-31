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
    public class GetApiAccessList : IRequest<ResponseModel<IEnumerable<GetApiAccessListReturn>>>
    {
        public string account { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> ApiList { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }

    }


    public class GetApiAccessListValidator : AbstractValidator<GetApiAccessList>
    {
        public GetApiAccessListValidator()
        {

            RuleFor(x => x.account).Account();
        }
    }


    public class GetApiAccessListReturn
    {
        public string group { get; set; }
        public string command { get; set; }
        public string description { get; set; }
        public bool status { get; set; }

    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetApiAccessListHandler : CMS.Core.BaseRequestHandler<GetApiAccessList, ResponseModel<IEnumerable<GetApiAccessListReturn>>>
    {
        public Dictionary<string, string> CommandNameTranslation = new Dictionary<string, string>(){
        {"api/Agent/AgentCreate","AgentCreate"},
         {"api/Agent/AgentVerify","AgentVerify"},
         {"api/Agent/AgentEdit","AgentEdit"},
         {"api/Agent/AgentDisableOrEnable","AgentDisableOrEnable"},
         {"api/Agent/AgentList","AgentList"},
         {"api/Agent/AgentDepositDailyList","AgentDepositDailyList"},
         {"api/Agent/AgentDepositMonthlyList","AgentDepositMonthlyList"},
         {"api/Agent/AgentGameDailyList","AgentGameDailyList"},
         {"api/Agent/AgentGameMonthlyList","AgentGameMonthlyList"},
         {"api/Agent/AgentLoseDailylyList","AgentLoseDailylyList"},
         {"api/Agent/AgentWithdrawList","AgentWithdrawList"},
         {"api/Account/VipLevelMonthRecords","VIP月升级数据查询"},//20200726 ching 新增
         

         {"api/Account/VipLevelDownMonthRecords","VIP月降级数据查询"},//20210707 ching 新增
         {"api/Account/VipLevelDownTo","VIP降级审核"},//20210707 ching 新增
         {"api/Account/VipLevelDownCancel","VIP降级取消"},//20210707 ching 新增
         {"api/Account/VipDepositTimesDailyRecords","每天存款次数"},//20210707 ching 新增
         {"api/Account/QueryVipMapping","查询管理员联分配系方式【QQ、微信】"},//20220426 ching 新增
         {"api/Account/CancelContactVIP","取消分配置到VIP联系方式"},//20220426 ching 新增
         {"api/Account/ConformedVIP","确认vip会员已经添加我们联系方式"},//20220426 ching 新增   
         {"api/Account/LoadContactsData","查询当前管理员的联系方式"},//20220426 ching 新增   
         {"api/Account/ImportVIPContacts","VIP专员导入"},//20220426 ching 新增   
         {"api/Account/QueryAllVIPContacts","查询所有已分配置联系VPI"},//20220426 ching 新增   

         //{"api/Account/SearchAccount","账号搜索"},
         //{"api/Account/InitialDepositAccounts","首存账号列表"},
         //{"api/Account/DepositorAccounts","存款账号列表"},
         //{"api/Account/AccountDetails","账号详细"},
         //{"api/Account/AccountLevel","账号等级"},
         //{"api/Account/AccountTransactionDetails","账号交易记录"},
         //{"api/Account/LevelList","等级List"},
         //{"api/Account/LevelEdit","等级编辑"},
         //{"api/Account/UpdateAccountPassword","更新玩家密码"},
         //{"api/Account/DeleteWithdrawCard","删除提款卡"},
         //{"api/Account/GetGameBalance","获取游戏金额"},

         {"api/BetRecord","Post"},
         {"api/Game/LoadTransferRecords","LoadTransferRecords"},
         {"api/Game/ViewTransferById","ViewTransferById"},
         {"api/Game/CancleTransfer","CancleTransfer"},
         {"api/Game/LoadGameConfig","LoadGameConfig"},
         {"api/Game/LoadGameConfigParams","LoadGameConfigParams"},
         {"api/Game/UpdateGameConfigStatus","UpdateGameConfigStatus"},
         {"api/Login","登入后台"},
         {"api/Login/CheckStatus","获取状态【比钩】"},
         {"api/Login/GetUser","获取用户【比钩】"},
         {"api/Login/GetUserPrivilege","GetUserPrivilege"},
         {"api/Promo/优惠记录/PromoCodeRecords","优惠记录"},
         {"api/Promo/PromoMiscRecords","PromoMiscRecords"},
         {"api/Promo/CreatePromoMisc","CreatePromoMisc"},
         {"api/Promo/VerifyPromoMisc","VerifyPromoMisc"},
         {"api/Promo/CancelPromoMisc","CancelPromoMisc"},
         {"api/Promo/SpreadRecords","SpreadRecords"},
         {"api/Promo/CreatePromoContent","CreatePromoContent"},
         {"api/Promo/LoadPromoContentById","LoadPromoContentById"},
         {"api/Promo/PromoContentRecords","PromoContentRecords"},

         {"api/Bonus/BonusRecords","红利列表"},
         {"api/Bonus/LoadBonusById","LoadBonusById"},
         {"api/Bonus/CreateBonus","CreateBonus"},
         {"api/Bonus/UpdateBonus","UpdateBonus"},
         {"api/Bonus/VerifyBonus","VerifyBonus"},
         {"api/Bonus/CancelBonus","CancelBonus"},
         {"api/Call/Log","呼叫记录"},
         {"api/Call/Activity","活跃记录"},
         {"api/Deposit/NetList","网银存款(列表)"},
         {"api/Deposit/CrossTransactVerify","互转审核"},
         {"api/Deposit/CrossTransactCancel","互转取消"},
         {"api/Deposit/CrossTransact","互转(列表)"},
         {"api/Deposit/ThirdpayList","第三方存款(列表)"},
         {"api/Deposit/ThirdpayPlatforms","商户名【比钩】"},
         {"api/Deposit/VerifyRequest","网银审核【更新】"},
         {"api/Deposit/CancelRequest","取消请求【更新】"},
         {"api/Export/LAWDF","LAWDF"},
         {"api/Export/LABILL","LABILL"},
         {"api/Export/LoadAgentPlayersGameMonthDataFromDB","载入代理会员"},
         {"api/Export/LAPGD","LAPGD"},
         {"api/Export/LoadAgentPlayersFinanceMonthDataFromDB","载入会员"},
         {"api/Export/LAPFD","LAPFD"},
         {"api/OldRecord/BonusPromoRecord","红利(列表)"},
         {"api/OldRecord/TransferRecord","游戏转账(列表)"},
         {"api/Resize/ResizeRecords","列表"},
         {"api/Resize/CreateResize","增加【更新】"},
         {"api/Resize/VerifyResize","审核【更新】"},
         {"api/Resize/CancelResize","取消【更新】"},
         {"api/System/Access/CommandList","命令表【比钩】"},
         {"api/System/Access","本用户权限【比钩】"},
         {"api/System/Access/Update","权限【更新】"},
         {"api/System/Access/TogglePrivilege","权限【更新】"},
         {"api/System/Code/GameCode","游戏代码【比钩】"},
         {"api/System/UsersRecord","后台用户(列表)"},
         {"api/System/UpdateAppSetting","更新APP参数配置"},
         {"api/System/QueryAppSetting","查询APP参数配置"},
         {"api/System/updateWebSiteSetting","更新网站参数配置"},
         {"api/System/QueryWebSiteSetting","查询网站参数配置"},
          {"api/System/UpdateAppIndexSetting","更新APP首页游戏配置"},
         {"api/System/QueryAppIndexSetting","查询APP首页游戏配置"},
          {"api/System/UpdateAppSlotSetting","更新APP老虎机游戏配置"},
         {"api/System/QueryAppSlotSetting","查询APP老虎机游戏配置"},
          {"api/System/UpdateGameUrlMode","更新APP游戏登录模式"},
         {"api/System/QueryGameUrlMode","查询APP游戏登录模式"},

         {"api/System/Access/UsersMenu","其他用户菜单"},
         {"api/System/Access/UsersMenuOwn","获取自己菜单【比钩】"},
         {"api/System/Access/UsersMenu/Toggle","菜单【更新】"},
         {"api/TurnOver/TurnOverRecords","返水(列表)"},
         {"api/TurnOver/CreateTurnOver","增加【更新】"},
         {"api/TurnOver/VerifyTurnOver","审核【更新】"},
         {"api/TurnOver/CancelTurnOver","取消【更新】"},
         {"api/Website/NoticeEdit","公告【更新】"},
         {"api/Website/NoticeAdd","公告【增加】"},
         {"api/Website/NoticeRecord","公告(列表)"},
         {"api/Website/NoticeSwitch","公告开关【更新】"},
         {"api/Website/AppConfig","APP参数(列表)"},
         {"api/Website/AppConfigUpdate","APP参数【更新】"},
         {"api/Website/BankCardRecord","收款银行卡(列表)"},
         {"api/Website/BankCardAdd","增加银行卡【更新】"},
         {"api/Website/BankCardSwitch","银行卡启用【更新】"},
         {"api/Website/BankCardUpdate","银行卡编辑【更新】"},
         {"api/Website/ThirdpayPlatformList","商户(列表)"},
         {"api/Website/ThirdpayPlatformDetail","商户详细"},
         {"api/Website/ThirdpayEdit","商会编辑【更新】"},
         {"api/Website/MessageRecord","站内讯息(列表)"},
         {"api/Website/MessageAdd","增加站内讯息【更新】"},
         {"api/Website/ValidateCodeRecord","验证码(列表)"},
         {"api/Website/GameControl","游戏管理(列表)"},
         {"api/Withdraw/RegularUserRecord","普通会员(列表)"},
         {"api/Withdraw/NewUserRecord","新会员(列表)"},
         {"api/Withdraw/VIP1Record","VIP1(列表)"},
         {"api/Withdraw/VIP2Record","VIP2(列表)"},
         {"api/Withdraw/VIP3Record","VIP3(列表)"},
         {"api/Withdraw/VIP4Record","VIP4(列表)"},
         //{"api/Withdraw/DirectPayRequest","代付所有(列表)"},
         {"api/Withdraw/AllUserDirectPayRequest","代付所有(列表)"},
         {"api/Withdraw/RegularUserDirectPayRequest","代付普通(列表)"},
         {"api/Withdraw/VipUserDirectPayRequest","代付VIP(列表)"},
         {"api/Withdraw/VipUserWithdrawRequest","VIP提款(列表)"},
         {"api/Withdraw/VIP3DirectPayRecord","VIP3代付款(列表)"},
         {"api/Withdraw/VIP2DirectPayRecord","VIP2代付款(列表)"},
         {"api/Withdraw/VipBigWithdrawRequest","大额VIP(列表)"},
         {"api/Withdraw/LockWithdrawRequest","锁订单【更新】"},
         {"api/Withdraw/ManualPay","手动支付【更新】"},
         {"api/Withdraw/DirectPayAffiliate","代理代付【*】"},
         {"api/Withdraw/DirectPay","进行代付【*】"},
         {"api/Withdraw/DiscloseAmount","显示金额【点选】"},
         {"api/Withdraw/HidesBankCard","隐私银行卡"},
         {"api/Withdraw/CancelRequest","取消提款【更新】"},
         {"api/Withdraw/ApproveRequest","审核提款【更新】"},
         {"api/Withdraw/AllRecord","所有提款(列表)"}
        };

        public GetApiAccessListHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<GetApiAccessListReturn>> Handle(GetApiAccessList request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var row = connection.Get<ofManage>(request.account);
                List<string> privilegeList = new List<string>();

                var access = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.access);
                if (!string.IsNullOrEmpty(row.access))
                {
                    if (access.ContainsKey("allowedpath"))
                        privilegeList.AddRange(access["allowedpath"].Split('|'));
                    else
                        access.Add("allowedpath", "");
                }
                else
                    access.Add("allowedpath", "");

                var responseList = new List<GetApiAccessListReturn>();

                foreach (var item in request.ApiList)
                {
                    // Extract GROUP
                    string _group = item.Key;
                    _group = _group.StartsWith("api/") ? _group.Trim().Remove(0, 4) : _group;
                    _group = _group.StartsWith("/api/") ? _group.Trim().Remove(0, 5) : _group;
                    _group = _group.Contains("/") ? _group.Substring(0, _group.IndexOf("/")) : _group;


                    responseList.Add(new GetApiAccessListReturn()
                    {
                        command = item.Key,
                        description = CommandNameTranslation.ContainsKey(item.Key) ? CommandNameTranslation[item.Key] : item.Value,
                        group = _group,
                        status = privilegeList.Contains(item.Key)
                    });
                }

                return new ResponseModel<IEnumerable<GetApiAccessListReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = responseList
                };
            }

        }


    }
}

