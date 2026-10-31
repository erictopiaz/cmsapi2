using CMS.Core.Application.Account.Queries;
using CMS.Core.Application.存款管理.Transact;
using CMS.Data;
using DBLibs;
using DBLibs.WXDapper;
using LogLib.Helper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using PromoShare;
using ShareEntity;
using ShareEntity.InputModels.TurnOver;
using ShareEntity.Models;
using ShareEntity.OutModels.TurnOver;
using ShareEntity.SqlCommandText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.lang;
using UtilsLib.Merchant;
using UtilsLib.SqlInject;

namespace CMS.API.BLL.Promo
{
    public class TurnOverData
    {
        private string _account;
        public string account {  
            set { _account = string.IsNullOrEmpty(value) ? "": value.Trim(); }
            get { return _account; }
        }
        public int currencyType { set; get; }
        public string merchantAccount { set; get; }
        public string affaccount { set; get; }
        public decimal reward { set; get; }
        public int gameType { set; get; }
        public string gamename { set; get; }
        public int platformGame { set; get; }
        public decimal effectivebets { set; get; }
        public decimal winlost { set; get; }
        public decimal cmBet { set; get; } = 0;

        public decimal bonusPoints { set; get; }


        public DateTime begindate { set; get; }
        public long tid { set; get; }
        /// <summary>
        /// bbin账号  发返水时要用这个账号去车为客人的账号
        /// </summary>
        public string bbinaccount { set; get; }
        public string pttaccount { set; get; }

        /// <summary>
        /// 控用户是否可以参加一些优惠设置
        /// </summary>
        public string pmisc { set; get; }

        /// <summary>
        /// 电销账号
        /// </summary>
        public string saler { set; get; }

        /// <summary>
        /// 印度币反水比例
        /// </summary>

        public string misc_inr { set; get; } = "";
        public string referenceguid { set; get; }
        public int currencycode { set; get; }
       
    }
    /// <summary>
    /// 当开统计
    /// </summary>
    public class TurnOverReward
    {
        public string account { set; get; }
        public decimal rewards { set; get; }
    }
    /// <summary>
    /// 返水比例列表
    /// </summary>
    public class TurnOverModel : TurnOverData
    {
        public long userid { set; get; }
        public int level { set; get; }
        public string referrer { set; get; }
        public string referrerreg { set; get; }
        public decimal? ea_turnover { get; set; }

        public decimal? ea_turnovermax { get; set; }

        public decimal? pt_turnover { get; set; }

        public decimal? pt_turnovermax { get; set; }

        public decimal? pttg_turnover { get; set; }

        public decimal? pttg_turnovermax { get; set; }

        public decimal? sun_turnover { get; set; }

        public decimal? sun_turnovermax { get; set; }

        public decimal? kg_turnover { get; set; }

        public decimal? kg_turnovermax { get; set; }

        public decimal? hg_turnover { get; set; }

        public decimal? hg_turnovermax { get; set; }

        public decimal? gd_turnover { get; set; }

        public decimal? gd_turnovermax { get; set; }

        public decimal? bbin_turnover { get; set; }

        public decimal? bbin_turnovermax { get; set; }

        public decimal spb_turnover { get; set; }

        public decimal spb_turnovermax { get; set; }

        public decimal ttg_turnover { get; set; }

        public decimal ttg_turnovermax { get; set; }

        public decimal? lb_turnover { get; set; }

        public decimal? lb_turnovermax { get; set; }

        public decimal ag_turnover { get; set; }

        public decimal ag_turnovermax { get; set; }

        public decimal bonuspercent { get; set; }

        public int betnumber { get; set; }

        public decimal bonusmax { get; set; }

        public decimal? lbssc_turnover { get; set; }

        public decimal? lbssc_turnovermax { get; set; }

        public decimal? bsg_turnover { get; set; }

        public decimal bsg_turnovermax { get; set; }

        public decimal? kg2_turnover { get; set; }

        public decimal? kg2_turnovermax { get; set; }

        public decimal? vr_turnover { get; set; }

        public decimal? vr_turnovermax { get; set; }

        public decimal? xj_turnover { get; set; }

        public decimal? xj_turnovermax { get; set; }

        public decimal? pg_turnover { get; set; }

        public decimal? pg_turnovermax { get; set; }

        public decimal? imes_turnover { get; set; }

        public decimal? imes_turnovermax { get; set; }

        public decimal? gg_turnover { get; set; }

        public decimal? gg_turnovermax { get; set; }

        public decimal? dscp_turnover { get; set; }

        public decimal? dscp_turnovermax { get; set; }

        public decimal? dses_turnover { get; set; }

        public decimal? dses_turnovermax { get; set; }

        public decimal? ob_turnover { get; set; }

        public decimal? ob_turnovermax { get; set; }
        public decimal? ky_turnover { get; set; }

        public decimal? ky_turnovermax { get; set; }
        public decimal? rg_turnover { get; set; }

        public decimal? rg_turnovermax { get; set; }
        public decimal? ysb_turnover { get; set; }

        public decimal? ysb_turnovermax { get; set; }
        public decimal? bg_turnover { get; set; }

        public decimal? bg_turnovermax { get; set; }
        public decimal? eb_turnover { get; set; }

        public decimal? eb_turnovermax { get; set; }
        public decimal? td_turnover { get; set; }

        public decimal? td_turnovermax { get; set; }
        public decimal? tf_turnover { get; set; }

        public decimal? tf_turnovermax { get; set; }
        public decimal? cq_turnover { get; set; }

        public decimal? cq_turnovermax { get; set; }
        public decimal? aw_turnover { get; set; }

        public decimal? aw_turnovermax { get; set; }

        public decimal? kg3_turnover { get; set; }

        public decimal? kg3_turnovermax { get; set; }

        public decimal? impk_turnover { get; set; }

        public decimal? impk_turnovermax { get; set; }
        public decimal? jdb_turnover { get; set; }

        public decimal? jdb_turnovermax { get; set; }

        public decimal? obg_turnover { get; set; }

        public decimal? obg_turnovermax { get; set; }

        public decimal? pm_turnover { get; set; }

        public decimal? pm_turnovermax { get; set; }

        public decimal? imsp_turnover { get; set; }

        public decimal? imsp_turnovermax { get; set; }

        public decimal? obcv_turnover { get; set; }

        public decimal? obcv_turnovermax { get; set; }
        public decimal? sw_turnover { get; set; }

        public decimal? sw_turnovermax { get; set; }

        public decimal? cr_turnover { get; set; }

        public decimal? cr_turnovermax { get; set; }

        public decimal? fb_turnover { get; set; }

        public decimal? fb_turnovermax { get; set; }

        public decimal? db_turnover { get; set; }

        public decimal? db_turnovermax { get; set; }

        public decimal? rsg_turnover { get; set; }

        public decimal? rsg_turnovermax { get; set; }
        public decimal? bti_turnover { get; set; }

        public decimal? bti_turnovermax { get; set; }

        public decimal? dbkeno_turnover { get; set; }

        public decimal? dbkeno_turnovermax { get; set; }

        public decimal? pp_turnover { get; set; }

        public decimal? pp_turnovermax { get; set; }

        public decimal? iaes_turnover { get; set; }

        public decimal? iaes_turnovermax { get; set; }


        #region 指定平台的某个游戏
        public decimal? ssc_turnover { get; set; }

        public decimal? ssc_turnovermax { get; set; }
        public decimal? pk10_turnover { get; set; }

        public decimal? pk10_turnovermax { get; set; }
        public decimal? hk49_turnover { get; set; }

        public decimal? hk49_turnovermax { get; set; }
        public decimal? bc_turnover { get; set; }

        public decimal? bc_turnovermax { get; set; }
        public decimal? ag_slot_turnover { get; set; }

        public decimal? ag_slot_turnovermax { get; set; }
        public decimal? ag_fish_turnover { get; set; }

        public decimal? ag_fish_turnovermax { get; set; }
        public decimal? jdb_fish_turnover { get; set; }

        public decimal? jdb_fish_turnovermax { get; set; }
        #endregion

        #region  投注积分
        /// <summary>
        /// 投注换算积分比例
        /// </summary>
        public string misc { set; get; }
        #endregion

        #region 额外返水设置
        /// <summary>
        /// 额外返水比例设置
        /// </summary>
        public string betCodeDetail { set; get; } = "";
        /// <summary>
        /// 额外返水比例是否开启
        /// </summary>
        public int b_status { set; get; } = -1;
        #endregion



    }

    /// <summary>
    /// 游戏平台的反水比例，不同货币的，可能有不同的反水比例 数据存放在 ofUserGroup.misc_inr 
    /// 不同的货币对应的反水比例，放到不同列保存
    /// </summary>s
    public class TurnOverRateModel {
        public decimal? ea_turnover { get; set; }

        public decimal? ea_turnovermax { get; set; }

        public decimal? pt_turnover { get; set; }

        public decimal? pt_turnovermax { get; set; }

        public decimal? pttg_turnover { get; set; }

        public decimal? pttg_turnovermax { get; set; }

        public decimal? sun_turnover { get; set; }

        public decimal? sun_turnovermax { get; set; }

        public decimal? kg_turnover { get; set; }

        public decimal? kg_turnovermax { get; set; }

        public decimal? hg_turnover { get; set; }

        public decimal? hg_turnovermax { get; set; }

        public decimal? gd_turnover { get; set; }

        public decimal? gd_turnovermax { get; set; }

        public decimal? bbin_turnover { get; set; }

        public decimal? bbin_turnovermax { get; set; }

        public decimal spb_turnover { get; set; }

        public decimal spb_turnovermax { get; set; }

        public decimal ttg_turnover { get; set; }

        public decimal ttg_turnovermax { get; set; }

        public decimal? lb_turnover { get; set; }

        public decimal? lb_turnovermax { get; set; }

        public decimal ag_turnover { get; set; }

        public decimal ag_turnovermax { get; set; }

        public decimal bonuspercent { get; set; }

        public int betnumber { get; set; }

        public decimal bonusmax { get; set; }

        public decimal? lbssc_turnover { get; set; }

        public decimal? lbssc_turnovermax { get; set; }

        public decimal? bsg_turnover { get; set; }

        public decimal bsg_turnovermax { get; set; }

        public decimal? kg2_turnover { get; set; }

        public decimal? kg2_turnovermax { get; set; }

        public decimal? vr_turnover { get; set; }

        public decimal? vr_turnovermax { get; set; }

        public decimal? xj_turnover { get; set; }

        public decimal? xj_turnovermax { get; set; }

        public decimal? pg_turnover { get; set; }

        public decimal? pg_turnovermax { get; set; }

        public decimal? imes_turnover { get; set; }

        public decimal? imes_turnovermax { get; set; }

        public decimal? gg_turnover { get; set; }

        public decimal? gg_turnovermax { get; set; }

        public decimal? dscp_turnover { get; set; }

        public decimal? dscp_turnovermax { get; set; }

        public decimal? dses_turnover { get; set; }

        public decimal? dses_turnovermax { get; set; }

        public decimal? ob_turnover { get; set; }

        public decimal? ob_turnovermax { get; set; }
        public decimal? ky_turnover { get; set; }

        public decimal? ky_turnovermax { get; set; }
        public decimal? rg_turnover { get; set; }

        public decimal? rg_turnovermax { get; set; }
        public decimal? ysb_turnover { get; set; }

        public decimal? ysb_turnovermax { get; set; }
        public decimal? bg_turnover { get; set; }

        public decimal? bg_turnovermax { get; set; }
        public decimal? eb_turnover { get; set; }

        public decimal? eb_turnovermax { get; set; }
        public decimal? td_turnover { get; set; }

        public decimal? td_turnovermax { get; set; }
        public decimal? tf_turnover { get; set; }

        public decimal? tf_turnovermax { get; set; }
        public decimal? cq_turnover { get; set; }

        public decimal? cq_turnovermax { get; set; }
        public decimal? aw_turnover { get; set; }

        public decimal? aw_turnovermax { get; set; }

        public decimal? kg3_turnover { get; set; }

        public decimal? kg3_turnovermax { get; set; }

        public decimal? impk_turnover { get; set; }

        public decimal? impk_turnovermax { get; set; }
        public decimal? jdb_turnover { get; set; }

        public decimal? jdb_turnovermax { get; set; }

        public decimal? obg_turnover { get; set; }

        public decimal? obg_turnovermax { get; set; }

        public decimal? pm_turnover { get; set; }

        public decimal? pm_turnovermax { get; set; }

        public decimal? imsp_turnover { get; set; }

        public decimal? imsp_turnovermax { get; set; }

        public decimal? obcv_turnover { get; set; }

        public decimal? obcv_turnovermax { get; set; }
        public decimal? sw_turnover { get; set; }

        public decimal? sw_turnovermax { get; set; }

        public decimal? cr_turnover { get; set; }

        public decimal? cr_turnovermax { get; set; }

        public decimal? fb_turnover { get; set; }

        public decimal? fb_turnovermax { get; set; }

        #region 指定平台的某个游戏
        public decimal? ssc_turnover { get; set; }

        public decimal? ssc_turnovermax { get; set; }
        public decimal? pk10_turnover { get; set; }

        public decimal? pk10_turnovermax { get; set; }
        public decimal? hk49_turnover { get; set; }

        public decimal? hk49_turnovermax { get; set; }
        public decimal? bc_turnover { get; set; }

        public decimal? bc_turnovermax { get; set; }
        public decimal? ag_slot_turnover { get; set; }

        public decimal? ag_slot_turnovermax { get; set; }
        public decimal? ag_fish_turnover { get; set; }

        public decimal? ag_fish_turnovermax { get; set; }
        public decimal? jdb_fish_turnover { get; set; }

        public decimal? jdb_fish_turnovermax { get; set; }
        #endregion
    }

    public class BetModel
    {
        public decimal totalBets { set; get; }
        public decimal totalWinlost { set; get; }
        public decimal totalReward { set; get; }
        public DateTime begindate { set; get; }
        public string gamename { set; get; }
        public string affaccount { set; get; }
        public long id { set; get; }
    }
    public class TurnOverBLL
    {


        /// <summary>
        /// 139, 140, 145, 128, 150, 144, 146, 155, 151,  152,  154, 158, 159,160, 161, 165
        /// LB,  AG,   XJ, EA, IMES, VR,   PG,  KY, DSCP, DSES, RG,  TD,  TF, AW,  CQ,  OBG
        /// 这几个平台的输赢入库时，要在输赢前加上负号
        /// 159:tf, 160:aw, 161: cq
        /// </summary>
        private static List<int> gameTypeList = new List<int>() { 139, 140, 128, 150, 144, 146, 155, 151, 152, 154, 158, 159,160, 161, 164, 165, 167, 169, 173,179,180, 182 };

        /// <summary>
        /// 游戏平台商的内部游戏
        /// </summary>
        private static Dictionary<int, string> gameTurnoverDict = new Dictionary<int, string>()
        {
            { 1,"ssc_turnover" },
            { 2,"pk10_turnover" },
            { 3,"hk49_turnover" },
            { 4,"bc_turnover" },
            { 5,"ag_slot_turnover" },
            { 6,"ag_fish_turnover" },
            { 7,"jdb_fish_turnover" }
        };
        /// <summary>
        /// 生成优惠或红利
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> TurnOverRecords(QueryTurnOverInputModel inputModel,
            string token,
            string loginKey)
        {
            string whereSql = string.Empty;
            var viewName = " ofTurnover a with(nolock) ";
            string orderBy = " a.id desc";
            var sortKeyDict = new Dictionary<int, string>()
            {
                { 0, " a.winlost desc " },
                { 1, " a.winlost asc " },
                { 2, " a.effectivebets asc " },
                { 3, " a.effectivebets desc " }
            };
            string keyName = " a.id ";

            orderBy = sortKeyDict.ContainsKey(inputModel.sortKey) ? sortKeyDict[inputModel.sortKey] : " a.id desc ";

            inputModel.adminAccount = JwtHelper.GetName(token, loginKey);
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token, loginKey);
            if (!currencys.Contains(inputModel.currencyType)) {
                return new AgentApiResultModel<object, object>() {
                    code = "false",
                    message = "请选择货币查询",
                    message_code = "error_currency_search",
                    levels = "",
                };
            }

            var accounts = string.IsNullOrEmpty(inputModel.account) ? "" : MerchantCodes.GetAccounts(inputModel.account);
            var affaccounts = string.IsNullOrEmpty(inputModel.affaccount) ? "" : MerchantCodes.GetAccounts(inputModel.affaccount);

            if (string.IsNullOrEmpty(inputModel.merchantAccount))
                inputModel.merchantAccount = "-1";
            else
            {
                inputModel.merchantAccount = inputModel.merchantAccount.Replace("'", "''");
            }

            if (!string.IsNullOrEmpty(inputModel.uid)) {
                var account = AccountBase.QueryAccountById(Convert.ToInt64(inputModel.uid));
                inputModel.account = string.IsNullOrEmpty(account) ? inputModel.account : account;
            }



            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account in {0} ", accounts) 
                    : string.Format(@" {0} and a.account in {1} ", whereSql, accounts);
            }

            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount in {0} ", affaccounts) 
                    : string.Format(@" {0} and b.affaccount in {1} ", whereSql, affaccounts);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.gameType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.gametype = {0} ", inputModel.gameType) : string.Format(@" {0} and a.gametype = {1} ", whereSql, inputModel.gameType);

           
            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" begindate >= '{0}' and enddate <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and begindate >= '{1}' and enddate <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }
            else {
                if (string.IsNullOrEmpty(inputModel.startTime)
                && string.IsNullOrEmpty(inputModel.endTime)) {
                    var currentDay = DateTime.UtcNow.AddHours(8);
                    var startTime = currentDay.AddDays(-6).ToString("yyyy-MM-01 00:00:00");
                    var endTime = currentDay.ToString("yyyy-MM-dd 23:59:59");

                    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" begindate >= '{0}' and enddate <= '{1}'", startTime, endTime) 
                        : string.Format(@" {0} and begindate >= '{1}' and enddate <= '{2}'", whereSql, startTime, endTime);

                }
            }

            if (inputModel.eturnover == 0)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.eCalc = {0} ", 0) : string.Format(@" {0} and a.eCalc = {1} ", whereSql, 0);
            else if (inputModel.eturnover == 1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.eCalc > {0} ", 0) : string.Format(@" {0} and a.eCalc > {1} ", whereSql, 0);

            //platformGame
            //{ 1,"ssc" },{ 2,"pk10" },{ 3,"hk49" },{ 4,"bc" },{ 5,"电子" },{ 6,"捕鱼" }
            //平台对应的游戏过虑
            if (inputModel.platformGame == 1
                || inputModel.platformGame == 2
                || inputModel.platformGame == 3
                || inputModel.platformGame == 4
                || inputModel.platformGame == 5
                || inputModel.platformGame == 6)
                whereSql = string.IsNullOrEmpty(whereSql)
                    ? string.Format(@" a.platformGame = {0} ", inputModel.platformGame)
                    : string.Format(@" {0} and a.platformGame = {1} ", whereSql, inputModel.platformGame);
            

            if (inputModel.platformType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" turnOverType = {0} ", inputModel.platformType) : string.Format(@" {0} and turnOverType = {1} ", whereSql, inputModel.platformType);

            if (inputModel.currencyType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.currencyType = {0} ", inputModel.currencyType) : string.Format(@" {0} and b.currencyType = {1} ", whereSql, inputModel.currencyType);

            if (!inputModel.merchantAccount.Equals("-1"))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.merchantAccount = '{0}' ", inputModel.merchantAccount)
                    : string.Format(@" {0} and b.merchantAccount = '{1}' ", whereSql, inputModel.merchantAccount);
            }

            if (inputModel.walletType != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.walletType = {0} ", inputModel.walletType)
                    : string.Format(@" {0} and b.walletType = {1} ", whereSql, inputModel.walletType);
            }

            var leftjoinString = string.Format(@" left join ofUser b with(nolock) on a.account = b.account
left join ofUserExtention ue with(nolock) on a.account = ue.account 
left join ofUserGroup ug with(nolock) on a.level = ug.levelnumber
left join ofGameClass gc with(nolock) on a.gametype = gc.gameid");
            var fields = " a.id as tid,b.id uid,a.account,a.effectivebets, a.level, a.reward,a.bonuspoints,a.pCalc,a.winlost,a.leftbets, a.begindate,a.enddate,a.requesttime,a.gametype,a.status,a.cancelperson,a.reviewperson,a.cancelreason,a.calc,a.eCalc,a.platformGame,b.affaccount,b.walletType,b.merchantAccount,gc.gamename2 as gamename ,ug.levels,b.anomaly,ue.riskRemark,ue.riskTypesAready";

            var qResult = await DapperPaging<QueryTurnOverOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);

            if (inputModel.currencyType == (int)LangCodes.CurrencyCode.INR
                || inputModel.currencyType == (int)LangCodes.CurrencyCode.USD) {
                if (qResult.data.modelList.Count > 0) {
                    foreach (var m in qResult.data.modelList) {
                        if (VipLevelINTL.levelIntlName.ContainsKey(m.level))
                            m.levels = VipLevelINTL.levelIntlName[m.level];
                    }
                }
            }
            return qResult;
        }

        /// <summary>
        /// 批量审核操作
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> VerifyTurnOver(VerifyTurnOverInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);

                #region 找出指定id的实体  turnOverData
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadUserTurnOverModelByIdSql, whereSql);
                var turnOverData = await DapperBase.QueryAsync<TurnOverData>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                #endregion

                #region  检测当前是否有账号在同一平台多次记录

                var repeatModels = turnOverData.GroupBy(g => (g.account, g.gamename, g.gameType, g.platformGame)).Where(s => s.Count() > 1).ToList();

                if (repeatModels.Count() > 0) //请确认当前账号
                {
                    var accountTurnover = string.Format(@"请确认账号:{0}，游戏平台:{1} 是否多次派发!",
                        repeatModels.First().Key.account, repeatModels.First().Key.Item2);
                    return ApiResultModel<object>.GetResult("false", accountTurnover, "verify_turnover_failed");

                }
                #endregion

                if (turnOverData != null && turnOverData.Count > 0)
                {
                    var sqlArray = new List<string>();
                    var param = new List<object> { };

                    var turnoverModel = turnOverData.First();
                    var defModel = MerchantCodes.GetDefaultModel(turnoverModel.merchantAccount, turnoverModel.currencycode);

                    //来一个个代理计算
                    foreach (var m in turnOverData)
                    {
                        if(m.currencycode != defModel.currencyType 
                            || !m.merchantAccount.Equals(defModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                        {
                            return new ApiResultModel<object>
                            {
                                code = "false",
                                message = "返水审核-失败，检测到不同品牌的会员",
                                message_code = "error merchant account or currency"
                            };
                        }
                        var reviewtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                        //返水定单状态更新
                        var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateOfTurnOverStatus, string.Format(@"({0})", m.tid));
                        sqlArray.Add(UpdateOfTurnOverStatus);
                        param.Add(new { status = 1, reviewperson = inputModel.adminAccount, reviewtime = reviewtime });

                        var isTurnoverControll = User.ControllUser(m.pmisc, 3);
                        var isPointsControll = User.ControllUser(m.pmisc, 2);
                        m.reward = isTurnoverControll ? 0 : m.reward;//风控的优惠账号拦取返水金额为0
                        m.bonusPoints = isTurnoverControll ? 0 : m.bonusPoints;//风控的优惠账号拦取积分为0

                        //账变
                        var preBalance = string.Format(UserSqlCommandText.preBalance, m.account);
                        var afterBalance = string.Format(UserSqlCommandText.afterBalanceA, m.reward, m.account);
                        var CreatePaymentSql = string.Format(PaymentSqlCommandText.CreatePaymentSql, preBalance, afterBalance);
                        sqlArray.Add(CreatePaymentSql);
                        param.Add(new
                        {
                            currency = defModel.currency,// "CNY",
                            account = m.account,
                            transactiontype = 1,
                            summary = isTurnoverControll ? "风控返水审核" : "返水审核",
                            referenceguid = Common.GuidToLongString(),
                            bonus = m.reward,
                            status = 1,
                            summaryType = 4
                        });


                        //新增积分
                        //var bonusPointsSql = string.Format(@"(select bonuspoints+{0} from ofUser where account='{1}')", m.bonusPoints, m.account);
                        //var CreatePromoMiscSql = string.Format(PromoSqlCommandText.CreatePromoMiscSql, bonusPointsSql);

                        //sqlArray.Add(CreatePromoMiscSql);
                        //param.Add(new
                        //{
                        //    account = m.account,
                        //    type = 9,//投注积分
                        //    content = m.gamename+"投注" +m.effectivebets,
                        //    createtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                        //    points = m.bonusPoints,
                        //    status = 0,
                        //    createperson = inputModel.adminAccount,
                        //    rowstatus = 1
                        //});

                        #region 记录充值账号时余额结点 如果我余额<=10才做记录 2024-11-28 新
                        if (!defModel.merchantAccount.Equals("Awc", StringComparison.OrdinalIgnoreCase))
                        {
                            var tpSqls = await AmoutRecord.ConfirmBalanceAsync(m.account, defModel.merchantAccount, defModel.currencyType,
                                referenceguid:m.referenceguid, r_type: 2, currentDate: reviewtime, remark: "后台反水到账");
                            if (!string.IsNullOrEmpty(tpSqls.Item1))
                            {
                                sqlArray.Add(tpSqls.Item1);
                                param.Add(tpSqls.Item2);
                            }
                        }

                        #endregion


                        //更新主账号余额
                        sqlArray.Add(UserSqlCommandText.UpdateBalanceAWithPointsSql);
                        param.Add(new { account = m.account, amount = m.reward, bonuspoints = m.bonusPoints });
                    }

                    var result = await DapperBase.ExecuteTransctionAsync(sqlArray,
                        param, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    return new ApiResultModel<object>
                    {
                        code = result ? "true" : "false",
                        message = result ? "返水审核-成功" : "返水审核-失败",
                        message_code = result ? "promo_turnover_verify_success" : "promo_turnover_verify_failed"
                    };

                }
            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "审核返水失败[ids]",
                message_code = "verify_turnover_failed",

            };
        }

        public static async Task<object> CancelTurnOver(VerifyTurnOverInputModel inputModel)
        {
            var tids = GetTids(inputModel.ids);
            if (tids.Item1)
            {
                var whereSql = string.Empty;
                foreach (var m in tids.Item2)
                {
                    if (string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"{0}", m);
                    else
                        whereSql = string.Format(@"{0},{1}", whereSql, m);
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //返水定单状态取消
                var UpdateOfTurnOverStatus = string.Format(PromoSqlCommandText.UpdateOfTurnOverStatusCancel, whereSql);
                var result = await DapperBase.ExecuteTransctionAsync(UpdateOfTurnOverStatus,
                      new { status=2, cancelperson = inputModel.adminAccount, cancelreason=inputModel.cancelreason, canceltime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                
                return new ApiResultModel<object>()
                {
                    code = result ? "true" : "false",
                    message = result ? "取消返水成功" : "取消返水失败",
                    message_code = result ? "cancel_turnover_success" : "cancel_turnover_failed"

                };
            }
            return new ApiResultModel<object>()
            {
                code = "false",
                message = "取消返水失败[ids]",
                message_code = "cancel_turnover_failed",

            };
        }

        /// <summary>
        /// 根据选择的id集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private static Tuple<bool,List<long>> GetTids(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return new Tuple<bool, List<long>>(false, null);
            try
            {
                var idList = ids.Split(",");
                var list = new List<long>();
                foreach (var item in idList)
                    list.Add(Convert.ToInt64(item));
                return new Tuple<bool, List<long>>(true, list);

            }
            catch (Exception)
            {
                return new Tuple<bool, List<long>>(false, null);
            }
            
        }
        #region 批量创建返水 旧
        ///// <summary>
        ///// 批量创建返水
        ///// 给指定平台的---特殊游戏
        ///// </summary>
        ///// <param name="inputModel"></param>
        ///// <returns></returns>
        //public static async Task<object> CreateTurnOver_old(CreateTurnOverInputModel inputModel)
        //{
        //    var access = AccessPlatformGameTurnover(inputModel);
        //    if (access.Item1)
        //        return ApiResultModel<object>.GetResult("false", access.Item2, "no_access_add_game_turnover", new turnoverResult { alreadyAccouts = "", errorAccounts = "" });
                
        //    #region 反水时间只能是昨天0点开始到到最后一秒
        //    var now = DateTime.UtcNow.AddHours(8).AddDays(-1);
        //    var st = Convert.ToDateTime(inputModel.startTime).ToString("yyyyMMddHHmmss");
        //    var et = Convert.ToDateTime(inputModel.endTime).ToString("yyyyMMddHHmmss");

        //    if (string.Compare(now.ToString("yyyyMMdd000000"), st) == 0
        //        && string.Compare(now.ToString("yyyyMMdd235959"), et) == 0)
        //    {
        //        var models = inputModel.gameType == 134 ? CalcBBinTurnOverData(inputModel.content) :
        //            inputModel.gameType == 138 ? CalcPtTurnOverData(inputModel.content) : CalcTurnOverData(inputModel.content);
        //        if (!string.IsNullOrEmpty(models.Item3))
        //        {
        //            return new ApiResultModel<object>()
        //            {
        //                code = "false",
        //                message = "计算返水失败,错误账号信息：",
        //                message_code = "calc_turnover_failed",
        //                data = new turnoverResult { alreadyAccouts = "", errorAccounts = models.Item3 }
        //            };
        //        }

        //        if (models.Item1)
        //        {
        //            //这里有点重
        //            var whereSql = string.Empty;
        //            foreach (var m in models.Item2)
        //            {
        //                if (string.IsNullOrEmpty(whereSql))
        //                    whereSql = string.Format(@"'{0}'", m.Key);
        //                else
        //                    whereSql = string.Format(@"{0},'{1}'", whereSql, m.Key);
        //            }
        //            if (!string.IsNullOrEmpty(whereSql))
        //                whereSql = string.Format(@"({0})", whereSql);

        //            //从读库取出当前加返水的人员信息
        //            var LoadUserTurnOverPercentSql = string.Empty;

        //            LoadUserTurnOverPercentSql = inputModel.platformGame == 0
        //                ? string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByAccountWithSalerSql, whereSql)
        //                : string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByGameWithSalerSql, whereSql);


        //            var turnOverModels = await DapperBase.QueryAsync<TurnOverModel>(LoadUserTurnOverPercentSql,
        //                  null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
        //            var errorAccounts = string.Empty;
        //            if (turnOverModels.Count != models.Item2.Count)
        //            {
        //                //不存在的账号
        //                var query = models.Item2.Where(p => !turnOverModels.Any(g => p.Key.ToLower() == g.account.ToLower()));
        //                foreach (var item in query)
        //                {
        //                    errorAccounts = string.IsNullOrEmpty(errorAccounts) ? item.Key : string.Format(@"{0},{1}", errorAccounts, item.Key);
        //                }
        //            }
        //            #region 已经发放的当天返水[已经审核和未审核的]
        //            var turnOverRewards = await DapperBase.QueryAsync<TurnOverReward>(PromoSqlCommandText.LoadTotalRewardsGroupByAccountSql_new,
        //                  new { gameType = inputModel.gameType, platformGame= inputModel.platformGame }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
        //            var turnOverRewardsDict = GetTotalRewardsDict(turnOverRewards); //如果有发放，今天 不会再做发放
        //            #endregion

        //            var insertModels = new List<ofTurnover>();
        //            var alreadyAccouts = string.Empty;

        //            foreach (var m in turnOverModels)
        //            {
        //                var account = m.account.ToLower();
        //                var isTurnoverControll = User.ControllUser(m.pmisc, 3);
        //                var isPointsControll = User.ControllUser(m.pmisc, 2);
        //                //获取额外返水
        //                if (turnOverRewardsDict.ContainsKey(account))
        //                {
        //                    alreadyAccouts = string.IsNullOrEmpty(alreadyAccouts) ? account : string.Format(@"{0},{1}", alreadyAccouts, account);
        //                    continue;
        //                }
        //                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(m));
        //                var p_name = string.Format(@"{0}_turnover", inputModel.gamename).ToLower();
        //                if (dict.ContainsKey(p_name))
        //                {
        //                    var p_name_max = string.Format(@"{0}max", p_name);
        //                    var order = Common.GuidToLongString();
        //                    var platformGameName = inputModel.platformGame == 0 ? "" : gameTurnoverDict[inputModel.platformGame];
        //                    var p = inputModel.platformGame == 0 ? dict[p_name] : dict[platformGameName];
        //                    var bpModel = JsonConvert.DeserializeObject<BonusPointsModel>(m.misc);
        //                    var t = new ofTurnover();

        //                    var totalRewards = turnOverRewardsDict.ContainsKey(account) ? turnOverRewardsDict[account].rewards : 0;
        //                    var limitReward = Convert.ToDecimal(dict[p_name_max]);
        //                    var winlost = models.Item2[account].winlost;
        //                    t.account = account;
        //                    t.calc = Convert.ToDecimal(p);
        //                    t.totalbets = models.Item2[account].effectivebets;
        //                    t.winlost = gameTypeList.Contains(inputModel.gameType) ? -winlost : winlost;
        //                    t.effectivebets = models.Item2[account].effectivebets;

        //                    t.weekmaxbets = limitReward;
        //                    var tp = CalcRewards(account, totalRewards, limitReward, Convert.ToDecimal(p), t.effectivebets, bpModel);
        //                    t.reward = isTurnoverControll ? 0 : tp.Item1;
        //                    t.leftbets = tp.Item2;
        //                    t.begindate = Convert.ToDateTime(inputModel.startTime);
        //                    t.enddate = Convert.ToDateTime(inputModel.endTime);
        //                    t.gametype = inputModel.gameType;
        //                    t.referrer = m.referrer;
        //                    t.referrerreg = m.referrerreg;
        //                    t.guid = order;
        //                    t.referenceguid = order;
        //                    t.level = m.level;
        //                    t.requesttime = DateTime.UtcNow.AddHours(8);
        //                    t.platformGame = inputModel.platformGame;
        //                    t.bonusPoints = isPointsControll ? 0 : 0;// tp.Item3;
        //                    t.pCalc = 0; //bpModel.turnoverpercentage;
        //                    t.saler = string.IsNullOrEmpty(m.saler) ? "awcsystem" : m.saler;
        //                    t.aaffaccount = string.IsNullOrEmpty(m.affaccount) ? "allwincity" : m.affaccount;
        //                    insertModels.Add(t);
        //                }
        //            }

        //            var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(PromoSqlCommandText.OfTurnoverInsertWithSalerSql, insertModels, commandTimeout: 15000,
        //                ConnectionString: DapperStringBase.ConnectionString);

        //            return new ApiResultModel<turnoverResult>()
        //            {
        //                code = insertResult ? "true" : "false",
        //                message = "计算返水结束,当前新增:" + insertModels.Count + "个记录" + st + "---" + now.ToString("yyyyMMddHHmmss"),
        //                message_code = "calc_turnover_finished",
        //                data = new turnoverResult { alreadyAccouts = alreadyAccouts, errorAccounts = errorAccounts }
        //            };

        //        }
        //        else
        //        {
        //            return new ApiResultModel<object>()
        //            {
        //                code = "false",
        //                message = "计算返水失败",
        //                message_code = "calc_turnover_failed",
        //                data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
        //            };
        //        }
        //    }
        //    else
        //    {
        //        return new ApiResultModel<object>()
        //        {
        //            code = "false",
        //            message = string.Format(@"创建返水-失败，不在有效时间内[{0}]-[{1}]", st, et),
        //            message_code = "calc_turnover_time_area_failed",
        //            data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
        //        };
        //    }
        //    #endregion

        //}

        #endregion

        public static async Task<object> CreateTurnOver(CreateTurnOverInputModel inputModel, WriteToFile writeToFile) {
            
            if(inputModel.currencyType == -1 )
                return ApiResultModel<object>.GetResult("false", "请选择货币", "error_currencyType", 
                    new turnoverResult { alreadyAccouts = "", errorAccounts = "" });

            return inputModel.currencyType == 156 
                ? await CreateTurnOverCNY(inputModel, writeToFile)
                : await CreateTurnOverIntl(inputModel, writeToFile);
        }

        /// <summary>
        /// 计算会员账号列表
        /// </summary>
        /// <param name="content">上传的反水内容</param>
        /// <param name="gameType">游戏平台id</param>
        /// <param name="merModel">品牌数据</param>
        /// <returns></returns>
        private static async Task<Tuple<bool, Dictionary<string, TurnOverData>, string>> QueryAccountsBycontent(string content, 
            int gameType,
            DefaultModel merModel)
        {
            await Task.Delay(1);
            if (merModel.merchantAccount.Equals("Boss")) {
                return CalcBossTurnOverDataV(content);
            }
            else
            {
                var models = (gameType == 134 || gameType == 179 || gameType == 180) ? CalcBBinTurnOverDataV(content) :
                    gameType == 164 ? CalcJDBTurnOverData(content) :
                    gameType == 138 ? CalcPtTurnOverData(content) :
                    (gameType == 145 || gameType == 181 || gameType == 169) ? CalcAwcTurnOverDataV(content):
                    CalcTurnOverData(content);
                return models;
            }
        }

        /// <summary>
        /// 批量创建返水
        /// 给指定平台的---特殊游戏
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        private static async Task<object> CreateTurnOverCNY(CreateTurnOverInputModel inputModel, WriteToFile writeToFile)
        {
            var access = AccessPlatformGameTurnover(inputModel);
            if (access.Item1)
                return ApiResultModel<object>.GetResult("false", access.Item2, "no_access_add_game_turnover", new turnoverResult { alreadyAccouts = "", errorAccounts = "" });

            #region
            if ( inputModel.gameType == 164 && inputModel.platformGame != 7)//ag 捕鱼额外加码
                return ApiResultModel<object>.GetResult("false", "请选择JDB，捕鱼反水!", "no_access_add_game_turnover", new turnoverResult { alreadyAccouts = "", errorAccounts = "" });

            #endregion
            var merModel = MerchantCodes.GetDefaultModel(inputModel.merchantAccount,inputModel.currencyType);
            if (merModel == null)
                return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");

            #region 反水时间只能是昨天0点开始到到最后一秒
            var now = DateTime.UtcNow.AddHours(8).AddDays(-1);
            var st = Convert.ToDateTime(inputModel.startTime).ToString("yyyyMMddHHmmss");
            var et = Convert.ToDateTime(inputModel.endTime).ToString("yyyyMMddHHmmss");

            if (string.Compare(now.ToString("yyyyMMdd000000"), st) == 0
                && string.Compare(now.ToString("yyyyMMdd235959"), et) == 0) 
            {

                var models = await QueryAccountsBycontent(inputModel.content, inputModel.gameType, merModel);
                if (!string.IsNullOrEmpty(models.Item3))
                {
                    return new ApiResultModel<object>()
                    {
                        code = "false",
                        message = "计算返水失败,错误账号信息：",
                        message_code = "calc_turnover_failed",
                        data = new turnoverResult { alreadyAccouts = "", errorAccounts = models.Item3 }
                    };
                }

                if (models.Item1)
                {
                    //这里有点重
                    var whereSql = string.Empty;
                    foreach (var m in models.Item2)
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", m.Key);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, m.Key);
                    }
                    if (!string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"({0})", whereSql);

                    //从读库取出当前加返水的人员信息
                    var LoadUserTurnOverPercentSql = string.Empty;

                    LoadUserTurnOverPercentSql = inputModel.platformGame == 0
                        ? string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByAccountWithSalerSql, whereSql)
                        : string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByGameWithSalerSql, whereSql);


                    var turnOverModels = await DapperBase.QueryAsync<TurnOverModel>(LoadUserTurnOverPercentSql,
                          null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    var errorAccounts = string.Empty;
                    if (turnOverModels.Count != models.Item2.Count)
                    {
                        //不存在的账号
                        var query = models.Item2.Where(p => !turnOverModels.Any(g => p.Key.ToLower() == g.account.ToLower()));
                        foreach (var item in query)
                        {
                            errorAccounts = string.IsNullOrEmpty(errorAccounts) ? item.Key : string.Format(@"{0},{1}", errorAccounts, item.Key);
                        }
                    }
                    #region 已经发放的当天返水[已经审核和未审核的]
                    var turnOverRewards = await DapperBase.QueryAsync<TurnOverReward>(PromoSqlCommandText.LoadTotalRewardsGroupByAccountSql_new,
                          new { gameType = inputModel.gameType, platformGame = inputModel.platformGame }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    var turnOverRewardsDict = GetTotalRewardsDict(turnOverRewards); //如果有发放，今天 不会再做发放
                    #endregion

                    var insertModels = new List<ofTurnover>();
                    var alreadyAccouts = string.Empty;

                    foreach (var m in turnOverModels)
                    {
                        if (m.currencyType != merModel.currencyType
                            || !m.merchantAccount.Equals(merModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                        {
                            return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                        }

                        var account = m.account.ToLower();
                        var isTurnoverControll = User.ControllUser(m.pmisc, 3);
                        var isPointsControll = User.ControllUser(m.pmisc, 2);


                        if (turnOverRewardsDict.ContainsKey(account)) {
                            alreadyAccouts = string.IsNullOrEmpty(alreadyAccouts) ? account : string.Format(@"{0},{1}", alreadyAccouts, account);
                            continue;
                        }

                        //获取额外返水
                        var extention_reward = 0m;
                        var extention_p = 0m;
                        if (m.b_status == 1)
                        {
                            var gamename = inputModel.gamename;
                            if (inputModel.platformGame == 6 && inputModel.gameType == 140)//ag 捕鱼额外加码
                                gamename = "agfish";

                            var tp = QueryTurnoverExtraP(m.betCodeDetail, gamename, models.Item2[account].effectivebets, writeToFile);
                            extention_reward = tp.Item1;
                            extention_p = tp.Item2;
                        }

                        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(m));
                        var p_name = string.Format(@"{0}_turnover", inputModel.gamename).ToLower();
                        if (dict.ContainsKey(p_name))
                        {
                            var p_name_max = string.Format(@"{0}max", p_name);
                            var order = Common.GuidToLongString();
                            var platformGameName = inputModel.platformGame == 0 ? "" : gameTurnoverDict[inputModel.platformGame];
                            var p = inputModel.platformGame == 0 ? dict[p_name] : dict[platformGameName];
                            var bpModel = JsonConvert.DeserializeObject<BonusPointsModel>(m.misc);
                            var t = new ofTurnover();

                            var totalRewards = turnOverRewardsDict.ContainsKey(account) ? turnOverRewardsDict[account].rewards : 0;
                            var limitReward = Convert.ToDecimal(dict[p_name_max]);
                            var winlost = models.Item2[account].winlost;
                            t.account = account;
                            t.calc = Convert.ToDecimal(p);
                            t.eCalc = extention_p;
                            t.totalbets = models.Item2[account].effectivebets;
                            t.winlost = gameTypeList.Contains(inputModel.gameType) ? -winlost : winlost;
                            t.effectivebets = models.Item2[account].effectivebets;

                            t.weekmaxbets = limitReward;
                            var tp = CalcRewards(account, totalRewards, limitReward, Convert.ToDecimal(p), t.effectivebets, bpModel);
                            t.reward = isTurnoverControll ? 0 : tp.Item1+ extention_reward;//返水+额外返水
                            t.leftbets = tp.Item2;
                            t.begindate = Convert.ToDateTime(inputModel.startTime);
                            t.enddate = Convert.ToDateTime(inputModel.endTime);
                            t.gametype = inputModel.gameType;
                            t.referrer = m.referrer;
                            t.referrerreg = m.referrerreg;
                            t.guid = order;
                            t.referenceguid = order;
                            t.level = m.level;
                            t.requesttime = DateTime.UtcNow.AddHours(8);
                            t.platformGame = inputModel.platformGame;
                            t.bonusPoints = isPointsControll ? 0 : 0;// tp.Item3;
                            t.pCalc = 0; //bpModel.turnoverpercentage;
                            t.saler = string.IsNullOrEmpty(m.saler) ? "awcsystem" : m.saler;
                            t.aaffaccount = string.IsNullOrEmpty(m.affaccount) ? "allwincity" : m.affaccount;
                            t.cmBet = 0;
                            t.t_f_uid = m.userid;
                            insertModels.Add(t);
                        }
                    }

                    var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(PromoSqlCommandText.OfTurnoverInsertWithSalerSql_V, 
                        insertModels, commandTimeout: 15000,
                        ConnectionString: DapperStringBase.ConnectionString);

                    return new ApiResultModel<turnoverResult>()
                    {
                        code = insertResult ? "true" : "false",
                        message = "计算返水结束,当前新增:" + insertModels.Count + "个记录" + st + "---" + now.ToString("yyyyMMddHHmmss"),
                        message_code = "calc_turnover_finished",
                        data = new turnoverResult { alreadyAccouts = alreadyAccouts, errorAccounts = errorAccounts }
                    };

                }
                else
                {
                    return new ApiResultModel<object>()
                    {
                        code = "false",
                        message = "计算返水失败",
                        message_code = "calc_turnover_failed",
                        data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
                    };
                }
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = string.Format(@"创建返水-失败，不在有效时间内[{0}]-[{1}]", st, et),
                    message_code = "calc_turnover_time_area_failed",
                    data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
                };
            }
            #endregion

        }


        /// <summary>
        /// 批量创建返水  国际版...
        /// 给指定平台的---特殊游戏
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="writeToFile"></param>
        /// <returns></returns>
        private static async Task<object> CreateTurnOverIntl(CreateTurnOverInputModel inputModel, WriteToFile writeToFile) {
            var access = AccessPlatformGameTurnover(inputModel);
            if (access.Item1)
                return ApiResultModel<object>.GetResult("false", access.Item2, "no_access_add_game_turnover", 
                    new turnoverResult { alreadyAccouts = "", errorAccounts = "" });

            #region
            if (inputModel.gameType == 164 && inputModel.platformGame != 7)//
                return ApiResultModel<object>.GetResult("false", "请选择JDB，捕鱼反水!", "no_access_add_game_turnover",
                    new turnoverResult { alreadyAccouts = "", errorAccounts = "" });

            #endregion

            var merModel = MerchantCodes.GetDefaultModel(inputModel.merchantAccount, inputModel.currencyType);

            #region 反水时间只能是昨天0点开始到到最后一秒
            var currentDay = DateTime.UtcNow.AddHours(8);
            var now = currentDay.AddDays(-1);
            var st = Convert.ToDateTime(inputModel.startTime).ToString("yyyyMMddHHmmss");
            var et = Convert.ToDateTime(inputModel.endTime).ToString("yyyyMMddHHmmss");

            if (string.Compare(now.ToString("yyyyMMdd000000"), st) == 0
                && string.Compare(now.ToString("yyyyMMdd235959"), et) == 0) {

                var models = currentDay>= Convert.ToDateTime("2024-04-24 00:00:00") 
                    ? CalcTurnOverDataIntlV(inputModel.content) //2024-04-24 这后 bayo+ofUser.id
                    : CalcTurnOverDataIntl(inputModel.content); //2024-04-24 之前使用会员账号

                if (!string.IsNullOrEmpty(models.Item3)) {
                    return new ApiResultModel<object>() {
                        code = "false",
                        message = "计算返水失败,错误账号信息：",
                        message_code = "calc_turnover_failed",
                        data = new turnoverResult { alreadyAccouts = "", errorAccounts = models.Item3 }
                    };
                }

                if (models.Item1) {
                    //这里有点重
                    var whereSql = string.Empty;
                    foreach (var m in models.Item2) {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", m.Key);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, m.Key);
                    }
                    if (!string.IsNullOrEmpty(whereSql))
                        whereSql = string.Format(@"({0})", whereSql);

                    //从读库取出当前加返水的人员信息
                    var LoadUserTurnOverPercentSql = string.Empty;

                    LoadUserTurnOverPercentSql = inputModel.platformGame == 0
                        ? string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByAccountWithSalerSql, whereSql)
                        : string.Format(PromoSqlCommandText.LoadUserTurnOverPercentByGameWithSalerSql, whereSql);


                    var turnOverModels = await DapperBase.QueryAsync<TurnOverModel>(LoadUserTurnOverPercentSql,
                          null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    var errorAccounts = string.Empty;
                    if (turnOverModels.Count != models.Item2.Count) {
                        //不存在的账号
                        var query = models.Item2.Where(p => !turnOverModels.Any(g => p.Key.ToLower() == g.account.ToLower()));
                        foreach (var item in query) {
                            errorAccounts = string.IsNullOrEmpty(errorAccounts) ? item.Key : string.Format(@"{0},{1}", errorAccounts, item.Key);
                        }
                    }

                    #region 检测会员的货币类型是否和当前发放反水的货币类型相同 如果货币类型有误，数据会直接返回
                    foreach (var m in turnOverModels) { 
                        if(m.currencyType != inputModel.currencyType) {
                            return new ApiResultModel<object>() {
                                code = "false",
                                message = $"会员 {m.account} 货币类型有误!",
                                message_code = "calc_turnover_failed"
                            };
                        }
                    }
                    #endregion

                    #region 已经发放的当天返水[已经审核和未审核的]
                    var turnOverRewards = await DapperBase.QueryAsync<TurnOverReward>(PromoSqlCommandText.LoadTotalRewardsGroupByAccountSql_new,
                          new { gameType = inputModel.gameType, platformGame = inputModel.platformGame }, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    var turnOverRewardsDict = GetTotalRewardsDict(turnOverRewards); //如果有发放，今天 不会再做发放
                    #endregion

                    var insertModels = new List<ofTurnover>();
                    var alreadyAccouts = string.Empty;

                    foreach (var m in turnOverModels) {

                        if (m.currencyType != merModel.currencyType
                           || !m.merchantAccount.Equals(merModel.merchantAccount, StringComparison.OrdinalIgnoreCase))
                        {
                            return ApiResultModel<object>.GetResult("false", "请选择正确的品牌+货币", "error merchant account or currency");
                        }

                        var account = m.account.ToLower();
                        var isTurnoverControll = User.ControllUser(m.pmisc, 3);
                        var isPointsControll = User.ControllUser(m.pmisc, 2);

                        if (turnOverRewardsDict.ContainsKey(account)) {
                            alreadyAccouts = string.IsNullOrEmpty(alreadyAccouts) ? account : string.Format(@"{0},{1}", alreadyAccouts, account);
                            continue;
                        }

                        //获取额外返水
                        var extention_reward = 0m;
                        var extention_p = 0m;
                        if (m.b_status == 1) {
                            var gamename = inputModel.gamename;
                            if (inputModel.platformGame == 6 && inputModel.gameType == 140)//ag 捕鱼额外加码
                                gamename = "agfish";

                            var tp = QueryTurnoverExtraP(m.betCodeDetail, gamename, models.Item2[account].effectivebets, writeToFile);
                            extention_reward = tp.Item1;
                            extention_p = tp.Item2;
                        }

                        //获取对应货币的反水比例
                        var dict = inputModel.currencyType == 156 ? JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(m)) 
                            : inputModel.currencyType == 356 ? JsonConvert.DeserializeObject<Dictionary<string, object>>(m.misc_inr) 
                            : new Dictionary<string, object>() ;

                        if (dict == null && dict.Count <= 0) {//没有指定到反水货币

                            return new ApiResultModel<object>() {
                                code = "false",
                                message = "计算返水失败,请确认反水货币",
                                message_code = "calc_turnover_failed"
                            };
                        }

                        if(inputModel.currencyType == 356) {
                            dict.Add("jdb_fish_turnover", 0.006);
                            dict.Add("jdb_fish_turnovermax", m.jdb_fish_turnovermax);
                        }
                        var plat = inputModel.gamename.Equals("9W") ? "_9w" : inputModel.gamename;

                        var p_name = string.Format(@"{0}_turnover", plat).ToLower();
                        if (dict.ContainsKey(p_name)) {
                            var p_name_max = string.Format(@"{0}max", p_name);
                            var order = Common.GuidToLongString();
                            var platformGameName = inputModel.platformGame == 0 ? "" : gameTurnoverDict[inputModel.platformGame];
                            var p = inputModel.platformGame == 0 ? dict[p_name] : dict[platformGameName];
                            var bpModel = JsonConvert.DeserializeObject<BonusPointsModel>(m.misc);
                            var t = new ofTurnover();

                            var totalRewards = turnOverRewardsDict.ContainsKey(account) ? turnOverRewardsDict[account].rewards : 0;
                            var limitReward = Convert.ToDecimal(dict[p_name_max]);
                            var winlost = models.Item2[account].winlost;
                            t.account = account;
                            t.calc = Convert.ToDecimal(p);
                            t.eCalc = extention_p;
                            t.totalbets = models.Item2[account].effectivebets;
                            t.winlost = gameTypeList.Contains(inputModel.gameType) ? -winlost : winlost;
                            t.effectivebets = models.Item2[account].effectivebets;
                            t.cmBet = models.Item2[account].cmBet; //2024-03-13 ching add 投注金额 ，区别 有效投注 effectivebets \ totalbets 

                            t.weekmaxbets = limitReward;
                            var tp = CalcRewards(account, totalRewards, limitReward, Convert.ToDecimal(p), t.effectivebets, bpModel);
                            t.reward = isTurnoverControll ? 0 : tp.Item1 + extention_reward;//返水+额外返水
                            t.leftbets = tp.Item2;
                            t.begindate = Convert.ToDateTime(inputModel.startTime);
                            t.enddate = Convert.ToDateTime(inputModel.endTime);
                            t.gametype = inputModel.gameType;
                            t.referrer = m.referrer;
                            t.referrerreg = m.referrerreg;
                            t.guid = order;
                            t.referenceguid = order;
                            t.level = m.level;
                            t.requesttime = DateTime.UtcNow.AddHours(8);
                            t.platformGame = inputModel.platformGame;
                            t.bonusPoints = isPointsControll ? 0 : 0;// tp.Item3;
                            t.pCalc = 0; //bpModel.turnoverpercentage;
                            t.saler = string.IsNullOrEmpty(m.saler) ? "awcsystem" : m.saler;
                            t.aaffaccount = string.IsNullOrEmpty(m.affaccount) ? "omg356" : m.affaccount;
                            t.t_f_uid = m.userid;
                            insertModels.Add(t);
                        }
                    }

                    var insertResult = await DapperBase.ExecuteTransctionMultipleAsync(PromoSqlCommandText.OfTurnoverInsertWithSalerSql_V, insertModels, commandTimeout: 15000,
                        ConnectionString: DapperStringBase.ConnectionString);

                    return new ApiResultModel<turnoverResult>() {
                        code = insertResult ? "true" : "false",
                        message = "计算返水结束,当前新增:" + insertModels.Count + "个记录" + st + "---" + now.ToString("yyyyMMddHHmmss"),
                        message_code = "calc_turnover_finished",
                        data = new turnoverResult { alreadyAccouts = alreadyAccouts, errorAccounts = errorAccounts }
                    };

                }
                else {
                    return new ApiResultModel<object>() {
                        code = "false",
                        message = "计算返水失败",
                        message_code = "calc_turnover_failed",
                        data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
                    };
                }
            }
            else {
                return new ApiResultModel<object>() {
                    code = "false",
                    message = string.Format(@"创建返水-失败，不在有效时间内[{0}]-[{1}]", st, et),
                    message_code = "calc_turnover_time_area_failed",
                    data = new turnoverResult { alreadyAccouts = "", errorAccounts = "" }
                };
            }
            #endregion

        }

        /// <summary>
        /// 获取客人额外各平台的返水比例
        /// </summary>
        /// <param name="betCodeDetail"></param>
        /// <param name="gamename">游戏平台名</param>
        /// <param name="effectivebets">投注额</param>
        /// <param name="writeToFile">投注额</param>
        /// <returns>item1:返水金额, item2: 返水比例</returns>
        private static Tuple<decimal,decimal> QueryTurnoverExtraP(string betCodeDetail,
            string gamename, decimal effectivebets, WriteToFile writeToFile)
        {

            //            string gameSql = string.Format(@"select gamename2 from ofGameClass
            //where class=1 and useable=1");
            //            var queryGameResult = await DapperBase.QueryAsync<string>(gameSql, param: null, commandTimeout: 15000,
            //                ConnectionString: DapperStringBase.ConnectionString);

            try
            {
                var betcodeS = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(betCodeDetail);


                var key_p = string.Format(@"{0}_p", gamename.ToLower());
                var key_m = string.Format(@"{0}_m", gamename.ToLower());

                if (betcodeS.ContainsKey(key_p))
                {
                    var rwd = effectivebets * betcodeS[key_p] * 0.01m;
                    rwd = rwd > betcodeS[key_m] ? betcodeS[key_m] : rwd;//检测是否达到上限
                    return new Tuple<decimal, decimal>(rwd, betcodeS[key_p]);
                }
                else
                    return new Tuple<decimal, decimal>(0, 0);
            }
            catch (Exception ex)
            {
                writeToFile.Done("QueryTurnoverExtraP_ex", "QueryTurnoverExtraP ",
                    string.Format(@"betCodeDetail = {0} ,  gamename={1}  e.message={1}", betCodeDetail, gamename, ex.Message), true);
                return new Tuple<decimal, decimal>(0, 0);
            }



        }


        /// <summary>
        /// 限制只有kg2/kg3可以指定游戏加返水
        /// 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        private static Tuple<bool,string> AccessPlatformGameTurnover(CreateTurnOverInputModel inputModel)
        {
            var code = (inputModel.platformGame > 0 
                && (inputModel.gameType != 143 && inputModel.gameType != 140 && inputModel.gameType != 162 && inputModel.gameType != 164)) ;
            return new Tuple<bool, string>(code, "目前只有kg2/ag/kg3支持-[指定平台的游戏]");
        }
        /// <summary>
        /// 计算当前应给多少返水
        /// 和当前结余多少金额
        /// 当前可换积分多少
        /// </summary>
        /// <param name="account"></param>
        /// <param name="totalRewards">当天某平台已经发放的返水</param>
        /// <param name="limitRewards">日限</param>
        /// <param name="p">返水比例</param>
        /// <param name="effectbet">有效果投注</param>
        /// <param name="bpModel">投注转积分比例</param>
        /// <returns></returns>
        private static Tuple<decimal,decimal,decimal> CalcRewards(string account, decimal totalRewards, 
            decimal limitRewards, decimal p, decimal effectbet, BonusPointsModel bpModel)
        {
            var reward = effectbet * p;//理论返水金额
            var bonusPoints = effectbet * bpModel.bonuspercentage;
            var t = reward + totalRewards; //当天总返水金额
            if (t <= limitRewards)
                return new Tuple<decimal, decimal, decimal>(reward, limitRewards - totalRewards - reward, bonusPoints);
            else
            {
                return new Tuple<decimal, decimal, decimal>(limitRewards - totalRewards, 0, bonusPoints);
            }
        }
        private static Dictionary<string, TurnOverReward> GetTotalRewardsDict(IList<TurnOverReward> turnOverRewards)
        {
            var dict = new Dictionary<string, TurnOverReward>();
            try
            {
                foreach (var m in turnOverRewards)
                    dict.Add(m.account, m);
            }
            catch (Exception)
            {

            }
            return dict;
        }
        /// <summary>
        /// 获取bbin返水数据模型 透过bbinaccount, 这样处理要多一层更新操作
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool,Dictionary<string,TurnOverData>,string> CalcBBinTurnOverData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length-1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // bbin 特别处理 bbin游戏只有 这边客人账号的bbinaccount

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var bbinaccount = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(bbinaccount, 4, 18))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", bbinaccount);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, bbinaccount);
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadBBinUserTurnOverPercentByAccountSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                Dictionary<string, string> bbinUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {
                    if (bbinUser.ContainsKey(b.bbinaccount))
                        bbinUser.Remove(b.bbinaccount);
                    bbinUser.Add(b.bbinaccount, b.account);
                }


                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i];
                    var account = bbinUser[ant];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 18) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>,string>(false, null,"");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }
            
        }

        /// <summary>
        /// 获取bbin返水数据模型 直接透过id 查找会员账号 20221223 更新
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcBBinTurnOverDataV(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // bbin 特别处理 bbin游戏只有 这边客人账号的bbinaccount

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var bbinaccount = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(bbinaccount, 4, 22))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"{0}", bbinaccount.ToLower().Replace("aw",""));
                        else
                            whereSql = string.Format(@"{0},{1}", whereSql, bbinaccount.ToLower().Replace("aw", ""));//去掉游戏账号前缀
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadBBinUserTurnOverPercentByAccountVSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                var bbinUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {
                    var userid = b.userid.ToString();
                    if (bbinUser.ContainsKey(userid))
                        bbinUser.Remove(userid);
                    bbinUser.Add(userid, b.account);
                }


                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i].ToLower().Replace("aw","");
                    var account = bbinUser[ant];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }

        /// <summary>
        /// Boss 品牌，游戏账号获取 透过uid 映射到会员账号
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcBossTurnOverDataV(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // bbin 特别处理 bbin游戏只有 这边客人账号的bbinaccount

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var bbinaccount = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(bbinaccount, 4, 22))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"{0}", bbinaccount.ToLower().Replace("boss", ""));
                        else
                            whereSql = string.Format(@"{0},{1}", whereSql, bbinaccount.ToLower().Replace("boss", ""));//去掉游戏账号前缀
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadBBinUserTurnOverPercentByAccountVSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                var bbinUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {
                    var userid = b.userid.ToString();
                    if (bbinUser.ContainsKey(userid))
                        bbinUser.Remove(userid);
                    bbinUser.Add(userid, b.account);
                }


                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i].ToLower().Replace("boss", "");
                    var account = bbinUser[ant];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }

        /// <summary>
        /// Awc 品牌，游戏账号获取 透过uid 映射到会员账号
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcAwcTurnOverDataV(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // bbin 特别处理 bbin游戏只有 这边客人账号的bbinaccount

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var bbinaccount = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(bbinaccount, 4, 22))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"{0}", bbinaccount.ToLower().Replace("awc", ""));
                        else
                            whereSql = string.Format(@"{0},{1}", whereSql, bbinaccount.ToLower().Replace("awc", ""));//去掉游戏账号前缀
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadBBinUserTurnOverPercentByAccountVSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                var bbinUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {
                    var userid = b.userid.ToString();
                    if (bbinUser.ContainsKey(userid))
                        bbinUser.Remove(userid);
                    bbinUser.Add(userid, b.account);
                }


                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i].ToLower().Replace("awc", "");
                    var account = bbinUser[ant];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }

        private static Tuple<bool, Dictionary<string, TurnOverData>,string> CalcTurnOverData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>,string>(false, null,"");
                }
                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var account = arrays[i];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>,string>(false, null,"");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>,string>(true, models,"");
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }

        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcTurnOverDataIntl(string content) {
            try {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 4 != 0) {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 4) {
                    var account = arrays[i];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    var cmBet = arrays[i + 3].Trim();
                    decimal eb, wl,cb;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 16) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl) ||
                        !ExpressSqlInject.DecimalValidate(cmBet, out cb))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl, cmBet=cb });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception) {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }


        /// <summary>
        /// 返水数据模型 直接透过id 查找会员账号 20240424 更新
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcTurnOverDataIntlV(string content) {
            try {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 4 != 0) {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // 

                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 4) {
                    var accountWithPrefix = arrays[i];
                    if (ExpressSqlInject.CharAndDigital(accountWithPrefix, 4, 20)) {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"{0}", accountWithPrefix.ToLower().Replace("bayo", ""));
                        else
                            whereSql = string.Format(@"{0},{1}", whereSql, accountWithPrefix.ToLower().Replace("bayo", ""));//去掉游戏账号前缀
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadBBinUserTurnOverPercentByAccountVSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                var bbinUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels) {
                    var userid = b.userid.ToString();
                    if (bbinUser.ContainsKey(userid))
                        bbinUser.Remove(userid);
                    bbinUser.Add(userid, b.account);
                }


                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 4) {
                    var ant = arrays[i].ToLower().Replace("bayo", "");
                    var account = bbinUser[ant];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    var cmBet = arrays[i + 3].Trim();
                    decimal eb, wl, cb;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 18) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl) ||
                         !ExpressSqlInject.DecimalValidate(cmBet, out cb))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl, cmBet = cb });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, "");
            }
            catch (Exception) {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }






        /// <summary>
        /// 获取pt返水数据模型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcPtTurnOverData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // pt 特别处理 pt 游戏只有 这边客人账号的pt account
                var errorAccounts = string.Empty;//不存在的账号
                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var pttaccount = arrays[i];
                    if (!string.IsNullOrEmpty(pttaccount) &&
                        pttaccount.IndexOf("IM0AY_") >= 0 )
                        pttaccount = pttaccount.Substring(6, pttaccount.Length - 6);
                    if (ExpressSqlInject.CharAndDigital(pttaccount, 4, 23))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", pttaccount);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, pttaccount);
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadPtUserTurnOverPercentByAccountSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                Dictionary<string, string> ptUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {

                    if (ptUser.ContainsKey(b.account.ToLower()))
                        ptUser.Remove(b.account.ToLower());
                    ptUser.Add(b.account.ToLower(), b.account);
                }

                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i];

                    if (!string.IsNullOrEmpty(ant) &&
                        ant.IndexOf("IM0AY_") >= 0)
                        ant = ant.Substring(6, ant.Length - 6);
                    if (!ptUser.ContainsKey(ant.ToLower()))
                    {
                        errorAccounts = string.IsNullOrEmpty(errorAccounts) ? string.Format(@"{0}", ant) : string.Format(@"{0},{1}", errorAccounts, ant);
                        continue;
                    }

                    var account = ptUser[ant.ToLower()];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>,string>(false, null,"");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, errorAccounts);
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }
        /// <summary>
        /// 获取JDB返水数据模型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Tuple<bool, Dictionary<string, TurnOverData>, string> CalcJDBTurnOverData(string content)
        {
            try
            {
                var arrays = content.Split("#");
                for (int o = 0; o < arrays.Length; o++)
                    arrays[o] = arrays[o].Trim();

                if (arrays.Length <= 1 || (arrays.Length - 1) % 3 != 0)
                {
                    //数据格式有误
                    return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                }
                // pt 特别处理 pt 游戏只有 这边客人账号的pt account
                var errorAccounts = string.Empty;//不存在的账号
                var whereSql = string.Empty;
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var jdbAccount = arrays[i];
                    if (!string.IsNullOrEmpty(jdbAccount) &&
                        jdbAccount.StartsWith("awc"))
                        jdbAccount = jdbAccount.Substring(3, jdbAccount.Length - 3);

                    if (ExpressSqlInject.CharAndDigital(jdbAccount, 4, 23))
                    {
                        if (string.IsNullOrEmpty(whereSql))
                            whereSql = string.Format(@"'{0}'", jdbAccount);
                        else
                            whereSql = string.Format(@"{0},'{1}'", whereSql, jdbAccount);
                    }
                }
                if (!string.IsNullOrEmpty(whereSql))
                    whereSql = string.Format(@"({0})", whereSql);
                //从读库取出当前加返水的人员信息
                var LoadUserTurnOverPercentSql = string.Format(PromoSqlCommandText.LoadPtUserTurnOverPercentByAccountSql, whereSql);

                var turnOverModels = DapperBase.Query<TurnOverModel>(LoadUserTurnOverPercentSql,
                      null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                Dictionary<string, string> ptUser = new Dictionary<string, string>();

                foreach (var b in turnOverModels)
                {

                    if (ptUser.ContainsKey(b.account.ToLower()))
                        ptUser.Remove(b.account.ToLower());
                    ptUser.Add(b.account.ToLower(), b.account);
                }

                var models = new Dictionary<string, TurnOverData>();
                for (int i = 1; i < arrays.Length; i += 3)
                {
                    var ant = arrays[i];

                    if (!string.IsNullOrEmpty(ant) &&
                        ant.StartsWith("awc"))
                        ant = ant.Substring(3, ant.Length - 3);

                    if (!ptUser.ContainsKey(ant.ToLower()))
                    {
                        errorAccounts = string.IsNullOrEmpty(errorAccounts) ? string.Format(@"{0}", ant) : string.Format(@"{0},{1}", errorAccounts, ant);
                        continue;
                    }

                    var account = ptUser[ant.ToLower()];
                    var effectbet = arrays[i + 1].Trim();
                    var winlost = arrays[i + 2].Trim();
                    decimal eb, wl;
                    if (!ExpressSqlInject.CharAndDigital(account, 4, 22) ||
                        !ExpressSqlInject.DecimalValidate(effectbet, out eb) ||
                        !ExpressSqlInject.DecimalValidate(winlost, out wl))
                        return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
                    else
                        models.Add(account.ToLower(), new TurnOverData { account = account.ToLower(), effectivebets = eb, winlost = wl });
                }

                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(true, models, errorAccounts);
            }
            catch (Exception)
            {
                return new Tuple<bool, Dictionary<string, TurnOverData>, string>(false, null, "");
            }

        }

        
    }
    /// <summary>
    /// 返水结果
    /// </summary>
    public class turnoverResult
    {
        public string errorAccounts { set; get; }
        public string alreadyAccouts { set; get; }
        /// <summary>
        /// 被风控的账号
        /// </summary>
        public string controllUsers { set; get; }
    }
    /// <summary>
    /// 兑换机分比例模型数据
    /// </summary>
    public class BonusPointsModel
    {
        //{"bonuspercentage":0.01}
        //{"bonuspercentage":0.00,"depositpercentage":0.00,"turnoverpercentage":0.00}
        public decimal bonuspercentage { set; get; }
        public decimal depositpercentage { set; get; }
        /// <summary>
        /// 返水积分比
        /// </summary>
        public decimal turnoverpercentage { set; get; }
    }
}
