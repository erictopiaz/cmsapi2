using AutoMapper;
using AWC2019.Core.Validation;
using AWC2019.Core.ViewModel;
using CMS.Core.Application.账号管理;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 账号详细Query : IRequest<账号详细Return>
    {
        /// <summary>
        /// 会员id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string Account { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
        public int allowedToManageAgent { get; set; }
    }

    public class 账号详细Return
    {
        public string levelurl { get; set; }
        public long id { get; set; }
        public string currency { get; set; }
        public int? accounttype { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public int sex { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string country { get; set; }
        public int status { get; set; }
        public int level { get; set; }
        public string levelname { get; set; }

        public string bbinaccount { get; set; }
        public string bbaffaccount { get; set; }
        public DateTime createtime { get; set; }
        public string referrerreg { get; set; }
        public decimal amountactive { get; set; }
        public string wallet { set; get; }
        public decimal amountsuspend { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public int depositnumber { get; set; }
        public decimal depositamount { get; set; }
        public int withdrawalnumber { get; set; }
        public decimal withdrawalamount { get; set; }
        public int loginnumber { get; set; }
        public string affaccount { get; set; }
        public string parentAccount { set; get; }
        public string freezereason { get; set; }
        public decimal bonuspoints { get; set; }
        public string marcode { get; set; }
        //public string withdrawcards { get; set; }
        public string[] withdrawcards { get; set; }
        public string[] delcards { get; set; }
        public string[] gamedisabledstat { get; set; }
        public string gamecontrol { get; set; }
        public string misc { get; set; }
        public string qq { get; set; }
        public decimal winloss { get; set; }
        public string promodisabled { get; set; }
        public string prevphone { get; set; }
        public string fonverpro2020 { get; set; }
        public DateTime lasttimedeposit { get; set; }
        public string riskRemark { get; set; }
        /// <summary>
        /// 电销人员
        /// </summary>
        public string saler { set; get; } = "";
        public string tags { get; set; }
        public int riskTypesAready { get; set; }
        public string[] depositrules { get; set; }
        public string[] alipaydepositrules { get; set; }
        public string[] wxdepositrules { set; get; }
        public int allowopenvpn { get; set; }

        public int currencyType { get; set; }
        /// <summary>
        /// 游戏钱包类型
        /// </summary>
        public int walletType { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 账号详细QueryValidator : AbstractValidator<账号详细Query>
    {
        public 账号详细QueryValidator()
        {
            RuleFor(x => x.Account).Account();
            //RuleFor(x => x.id).DigitalNullable(1000, 100000000);
            //RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账号详细Handler : BaseAccount<账号详细Query, 账号详细Return>
    {
        public 账号详细Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 账号详细Return Handle(账号详细Query request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {

                var rowsSettings = connection.QueryFirst<string>("" +
                    "SELECT body " +
                    "FROM ofSettings WITH(NOLOCK) " +
                    "WHERE code = 'GROUP_LEVELURL'");
                var rowSettingsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(rowsSettings);

                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }

                #region ching add for display affaccount top 显示当前代理的上级代理
                var querySal = string.Format(@"select u.*,agt.parentAccount,iif(( ug.wallet IS not NULL and LEN(RTRIM(LTRIM(ug.wallet))) != 0), ug.wallet,0) wallet from ofUser u with(nolock)
left join ofAffiliate ag with(nolock) on u.affaccount =ag.account
left join ofAffiliateTree agt with(nolock) on ag.account = agt.agentAccount
left join ofUserGameWallet ug with(nolock) on u.account=ug.account
where u.account='{0}' ", request.Account);
                var userDetail = connection.QueryFirst<ofUserx>(querySal);
                #endregion

                #region 特定的市场人员, 只能查看到指定的代理下面的会员, 否则返回空数据
                //if (request.Admin.ToLower().Equals("awcty000")){
                //    var agents = new List<string>() { "awcty101", "awcty102", "awcty103", "awcty106", "awcty108", "awcty109" };
                //    var agent = userDetail.affaccount;
                //    if (!agents.Contains(agent.ToLower()))
                //        return new 账号详细Return();

                //}
                if (request.allowedToManageAgent == 1) { //当前登发的账号为市场人员登录

                    var agentMarketSql = string.Format(@"select agentMarket from ofAffiliate where account=@account");
                    var agentMarketObj = DapperBase.ExecuteScalarValue(agentMarketSql, new { account = userDetail.affaccount }, commandType: CommandType.Text,
                                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                    if(agentMarketObj == null || !agentMarketObj.ToString().ToLower().Equals(request.Admin.ToLower()))
                        return new 账号详细Return();

                }
                #endregion

                #region 查询统计 会员总存和总提
                var queryWDReportSql = string.Format(UserSqlCommandText.UserDepositWithdrawReportSql, request.Account);
                var queryWDReportModel = connection.Query<ofUserx>(queryWDReportSql);
                if(queryWDReportModel != null && queryWDReportModel.Count() == 1) {
                    userDetail.depositamount = queryWDReportModel.First().depositamount;
                    userDetail.withdrawalamount = queryWDReportModel.First().withdrawalamount;
                    userDetail.depositnumber = queryWDReportModel.First().depositnumber;
                    userDetail.withdrawalnumber = queryWDReportModel.First().withdrawalnumber;
                }
                #endregion
               
                var userExtention = connection.QueryFirstOrDefault("SELECT * FROM ofUserExtention WITH(NOLOCK) WHERE account='" + userDetail.account + "'");
                var result = _mapper.Map<账号详细Return>(userDetail);
                result.id = userDetail.id;//_mapper.Map  没有映射到id
                result.wallet = userDetail.wallet;
                result.walletType = userDetail.walletType;
                if (userExtention != null)
                {
                    result.riskRemark = userExtention.riskRemark;
                    result.riskTypesAready = userExtention.riskTypesAready;
                }
                result.levelurl = rowSettingsDict.ContainsKey(result.level.ToString()) ? rowSettingsDict[result.level.ToString()] : "";
               
                #region
                var saleModel = connection.QueryFirstOrDefault($"select top 1 account,createperson from ofCallLog with(nolock) where account='{userDetail.account}' order by createtime desc");
                if (saleModel != null)
                {
                    result.saler = saleModel.createperson;
                }
                #endregion

                // Extract Last Deposit
                if (userDetail.depositnumber > 0)
                {
                    try
                    {
                        var depositRowSingle = connection.QuerySingle("SELECT TOP 1 requesttime FROM ofDeposit WITH(NOLOCK) WHERE account='" + request.Account + "' ORDER by requesttime desc");
                        result.lasttimedeposit = depositRowSingle.requesttime;
                    }
                    catch (Exception)
                    {
                    }
                }


                // Extract Deleted Cards
                if (!string.IsNullOrEmpty(userDetail.misc))
                {
                    // Game Names
                    List<string> gamenames = new List<string>();

                    foreach (var item in CachedData.GameName)
                        gamenames.Add(item);

                    var misc = userDetail.misc.ToString();
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                    if (miscDict.ContainsKey("delcards"))
                        result.delcards = miscDict["delcards"].Split('|');

                    if (miscDict.ContainsKey("promodisabled"))
                    {
                        result.promodisabled = miscDict["promodisabled"];
                        // correcting
                        if (!string.IsNullOrEmpty(result.promodisabled))
                        {
                            if (!result.promodisabled.StartsWith("["))
                                result.promodisabled = string.Concat("[", result.promodisabled, "]");
                        }
                    }
                    else
                        result.promodisabled = "[]";

                    if (miscDict.ContainsKey("gamedisabledstat"))
                    {
                        //List<string> enabledgames = new List<string>();
                        var disabledgames = miscDict["gamedisabledstat"].Split('|');
                        //var disabledgames = new string[] { "EA", "XJ" };
                        gamenames.RemoveAll(x => !disabledgames.Contains(x));
                        result.gamedisabledstat = gamenames.ToArray();
                    }
                    else
                        result.gamedisabledstat = new string[] { };
                    
                    if (miscDict.ContainsKey("depositrules") && !string.IsNullOrEmpty(miscDict["depositrules"]))
                        result.depositrules = miscDict["depositrules"].Split(',');
                    else
                        result.depositrules = new string[] { };

                    if (miscDict.ContainsKey("alipaydepositrules") && !string.IsNullOrEmpty(miscDict["alipaydepositrules"]))
                        result.alipaydepositrules = miscDict["alipaydepositrules"].Split(',');
                    else
                        result.alipaydepositrules = new string[] { };

                    if (miscDict.ContainsKey("wxdepositrules") && !string.IsNullOrEmpty(miscDict["wxdepositrules"]))
                        result.wxdepositrules = miscDict["wxdepositrules"].Split(',');
                    else
                        result.wxdepositrules = new string[] { };

                    result.allowopenvpn = (miscDict.ContainsKey("allowopenvpn") && miscDict["allowopenvpn"].ToString() == "1") ? 1 : 0;

                    if ((miscDict.ContainsKey("prevphone") && !string.IsNullOrEmpty(miscDict["prevphone"])))
                        result.prevphone = miscDict["prevphone"];

                    if ((miscDict.ContainsKey("tags") && !string.IsNullOrEmpty(miscDict["tags"])))
                    {
 
                            result.tags = miscDict["tags"];
                    }


                    if ((miscDict.ContainsKey("fonverpro2020") && !string.IsNullOrEmpty(miscDict["fonverpro2020"])))
                        result.fonverpro2020 = miscDict["fonverpro2020"];
                }
                else
                {
                    result.depositrules = new string[] { };
                }

                if (result.sex > 1) result.sex = 1;

                if (result.account == result.telephone)
                    result.account = Common.Security.ObscureString(result.account);


                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                if (!adminAccess.Contains("/api/Account/AllowToSeePhone"))
                {
                    if (adminAccess.Contains("/api/Account/ShowNoDepositPhone"))
                    {
                        if (result.depositamount > 0)
                            result.telephone = Common.Security.ObscureString(result.telephone);
                    }
                    else
                    {
                        result.telephone = Common.Security.ObscureString(result.telephone);
                    }
                    result.qq = Common.Security.ObscureString(result.qq);

                    if (!string.IsNullOrEmpty(result.prevphone))
                        result.prevphone = Common.Security.ObscureString(result.prevphone);

                }




                if (!adminAccess.Contains("/api/Account/AllowToSeeEmail"))
                    result.email = Common.Security.ObscureString(result.email);

                string withdrawcardsString = userDetail.withdrawcards;
                if (!string.IsNullOrEmpty(withdrawcardsString))
                {
                    var withdrawcards = withdrawcardsString.Split('|');

                    for (int i = 0; i < withdrawcards.Length; i++)
                    {
                        // Obscure Cards
                        // Sample Card format 
                        // 工商银行, 4024007175245884,黑龙江
                        // Should look
                        var carddata = withdrawcards[i].Split(',');
                        var cardno = string.Concat(carddata[1].Substring(0, 4), "*******", carddata[1].Substring(carddata[1].Length - 4, 4));
                        var cardbank = carddata[0];
                        var cardaddress = carddata[2];
                        var cardname = carddata.Length >= 4 ? carddata[3] : userDetail.realname;

                        if (cardbank.ToLower().Contains("usdt") && 
                            adminAccess.Contains("/api/Account/AllowToSeeWalletAddress"))
                        {
                            cardno = carddata[1];
                        }


                        withdrawcards[i] = string.Concat(cardno, ",", cardbank, ",", cardaddress, ",", cardname);

                        //cardno
                        //" 【",w
                        //cardbank,
                        //"】",
                        //cardaddress
                        //);

                    }


                    result.withdrawcards = withdrawcards;
                }

                result.levelname = CachedData.LevelNameDict[result.level];
                result.winloss = result.withdrawalamount - result.depositamount;
                result.bbinaccount = string.Format(@"aw{0}", userDetail.id);//bbin 游戏 平台会员账号是由  aw+id生成
                
                return result;
            }
        }
    }
}

