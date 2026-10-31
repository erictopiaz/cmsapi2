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
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.OutputModels;
using DBLibs.WXDapper;
using System.Data;
using UtilsLib.ValidationAttr;
using ShareEntity.SqlCommandText;

namespace CMS.Core.Application.账号管理.Transact
{
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 查询会员加码比例 : IRequest<查询会员加码比例Return>
    {
        public string account { get; set; }

        public int? id { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class QueryOfSettingsModel {
        public string riskTypes { get; set; }
        public string vipStatusTypes { get; set; }
        public string fixAmountLevels { get; set; }
        public string fixWAmountLevels { get; set; }
    }

    /// <summary>
    /// 查询返回数据
    /// </summary>
    public class QueryOutModel
    {
        public string account { set; get; }
        public int b_status { set; get; }
        public string betCodeDetail {set;get;}
        /// <summary>
        /// 充值多实名
        /// </summary>
        public string realNames { set; get; }
        public Dictionary<string,object> kvs { set; get; }
        /// <summary>
        /// 会员特别的存款优惠
        /// </summary>
        public string specialDepositPromo { set; get; }
        /// <summary>
        /// 会员禁用的商户id列表
        /// </summary>
        public string disabledMids { set; get; }

        /// <summary>
        /// 已经配置的风控类型
        /// </summary>
        public int riskTypesAready { set; get; }

        /// <summary>
        /// 风控备注
        /// </summary>
        public string riskRemark { set; get; }

        /// <summary>
        /// vip状态类型
        /// </summary>
        public int vipTypeAready { set; get; }

        /// <summary>
        /// vip状态备注
        /// </summary>
        public string vipStatusRemark { set; get; } = "";

        /// <summary>
        /// 存款实名验证  0：否，1：是
        /// </summary>
        public int isneedcheckrealname { set; get; }
        /// <summary>
        /// 是否参与推荐优惠结算 1：参与，0：不参与
        /// </summary>
        public int isLimitSpreadBilled { set; get; }

        /// <summary>
        /// 查询定额存款金额的级别
        /// </summary>
        public int fixAmountLimitLevel { set; get; }
        /// <summary>
        /// 查询定额提款金额的级别
        /// </summary>
        public int fixWAmountLimitLevel { set; get; }

        /// <summary>
        /// 是否可以参与，定额的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedfixWithdrawal { set; get; }
        /// <summary>
        /// 是否可以参与，支付宝定额的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedAliFixWithdrawal { set; get; }
        /// <summary>
        /// 是否可以参与，微信定额的提款：0：可以，1：禁止参与
        /// </summary>
        public int isLimitedWXFixWithdrawal { set; get; }

        public int withdrawMinAmount { set; get; }
        public int withdrawMaxAmount { set; get; }
        
    }

    public class 查询会员加码比例Return : ResponseModel<object>
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    //public class 查询会员加码比例Validator : AbstractValidator<查询会员加码比例>
    //{
    //    public 查询会员加码比例Validator()
    //    {
    //        //RuleFor(x => x.account).AccountPlayer(4, 16, true);
    //    }
    //}

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 查询会员加码比例Handler : BaseAccount<查询会员加码比例, 查询会员加码比例Return>
    {
        public 查询会员加码比例Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override 查询会员加码比例Return Handle(查询会员加码比例 request)
        {
            //string querySql = (string.IsNullOrEmpty(request.account) && request.id <= 0) ? string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", request.id) :
            //    string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where account='{0}')", request.account);

            string querySql = string.Empty;

            if (request.id > 10) {
                querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where id={0})", request.id);
            }

            if (!string.IsNullOrEmpty(request.account)) {
                var account = request.account.Replace("'", "''");
                querySql = string.Format(@"SELECT * FROM ofUserExtention WHERE account=(select account from ofUser where account='{0}')", account);
            }

            

            var queryResult = DapperBase.Query<QueryOutModel>(querySql, param:null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            var code = (queryResult != null && queryResult.Count == 1);
            

            #region 获取可用游戏平台

            string gameSql = string.Format(@"select gamename2 from ofGameClass
where class=1 and useable=1");
            var queryGameResult = DapperBase.Query<string>(gameSql, param: null, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);

            var betcodeS = (code && !string.IsNullOrEmpty(queryResult.First().betCodeDetail)) ? JsonConvert.DeserializeObject<Dictionary<string,decimal>>(queryResult.First().betCodeDetail):new Dictionary<string, decimal>();
            var betCodeObject = new Dictionary<string, Dictionary<string, decimal>>();

            foreach (var name in queryGameResult.ToList())
            {
                var key = string.Format(@"{0}_p", name.ToLower());
                var key2 = string.Format(@"{0}_m", name.ToLower());
                betCodeObject.Add(name, new Dictionary<string, decimal>() { { key,0m }, { key2, 0m } });
            }
            //ag捕鱼额外添加
            if(!betCodeObject.ContainsKey("agfish"))
                betCodeObject.Add("agfish", new Dictionary<string, decimal>() { { "agfish_p", 0m }, { "agfish_m", 0m } });

            foreach (var name in queryGameResult.ToList())
            {
                foreach(var b in betcodeS)
                {
                    var n_t = string.Format(@"{0}_", name);
                    var bKeyName = b.Key.Split('_')[0];
                    if(b.Key.Contains(n_t, StringComparison.OrdinalIgnoreCase)
                        && name.ToLower().Equals(bKeyName.ToLower()))
                    {
                        betCodeObject[name][b.Key] =  b.Value;
                    }
                }
            }
            //ag捕鱼额外添加
            foreach (var b in betcodeS)
            {
                if (b.Key.Contains("agfish", StringComparison.OrdinalIgnoreCase))
                {
                    betCodeObject["agfish"][b.Key] = b.Value;
                }
            }

            #endregion

            #region  特定会员存款优惠列表

            DynamicParameters dp = new DynamicParameters();
            string endtime = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd hh:mm:ss");
            dp.Add("@endtime", endtime, DbType.AnsiString, ParameterDirection.Input, 30);

            var models = DapperBase.Query<dynamic>(PromoSqlCommandText.GetSpecialDepositPromoList, dp,
                            null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            var specialDepositPromos = new List<object>();
            var specialDepositAready = new List<int>();
            if (models != null && models.Count > 0)
            {
                foreach (var item in models)
                {
                    specialDepositPromos.Add(new { id = item.s_value, label = item.s_text });
                }
                if (specialDepositPromos.Count > 0
                    && queryResult != null && queryResult.Count > 0 && !string.IsNullOrEmpty(queryResult.First().specialDepositPromo))
                {
                    specialDepositAready = JsonConvert.DeserializeObject<List<int>>(queryResult.First().specialDepositPromo);
                }
            }
            #endregion

            #region  会员风控状态

            //var riskUserTypeSql = string.Format(@"select body from ofSettings where code='{0}' and status=1 ", "risk_user_type");
            var riskUserTypeSql = string.Format(@"select a.riskTypes,d.vipStatusTypes, b.fixAmountLevels,c.fixWAmountLevels from (
select body riskTypes from ofSettings with(nolock)  where code='risk_user_type' and status=1) a,
(select body vipStatusTypes from ofSettings with(nolock)  where code='vip_status_r_type' and status=1) d,
(select body fixAmountLevels from ofsettings with(nolock)
where code='fix_amount_level') b,(select body fixWAmountLevels from ofsettings with(nolock)
where code='fix_withdraw_amount_level') c");
            var riskTypsObject = DapperBase.Query<QueryOfSettingsModel>(riskUserTypeSql, param: null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            var riskModels = new List<object>();
            var riskTypeAready = 0;
            var riskRemark = string.Empty;

            var vipStatusModels = new List<object>();
            var vipTypeAready = 0;
            var vipStatusRemark = string.Empty;

            if (riskTypsObject != null && riskTypsObject.Count > 0 )
            {
                var dy = riskTypsObject.First();
                var riskTypeModels = JsonConvert.DeserializeObject<dynamic>(dy.riskTypes.ToString());

                foreach (var item in riskTypeModels)
                {
                    riskModels.Add(new { id = item.id.Value, label = item.label.Value });
                }

                var vipStatusTypes = JsonConvert.DeserializeObject<dynamic>(dy.vipStatusTypes.ToString());
                foreach (var item in vipStatusTypes)
                {
                    vipStatusModels.Add(new { id = item.id.Value, label = item.label.Value });
                }

                if (riskModels.Count > 0
                    && queryResult != null && queryResult.Count > 0 )
                {
                    riskTypeAready = queryResult.First().riskTypesAready;
                    riskRemark = queryResult.First().riskRemark;
                }

                if (vipStatusModels.Count > 0
                    && queryResult != null && queryResult.Count > 0)
                {
                    vipTypeAready = queryResult.First().vipTypeAready;
                    vipStatusRemark = queryResult.First().vipStatusRemark;
                }
            }

            //定额充值金额限制权限
            var fixAmountLimitLevel = 1;
            var fixAmountModels = new List<object>();
            if (riskTypsObject != null && riskTypsObject.Count > 0) {
                var dy = riskTypsObject.First();
                var fixAmountLevelModels = JsonConvert.DeserializeObject<dynamic>(dy.fixAmountLevels.ToString());

                foreach (var item in fixAmountLevelModels) {
                    //fixAmountModels.Add(new { id = item.level.Value, label = item.amount.Value });
                    //var label = item.minAmount.Value == item.maxAmount.Value
                    //    ? string.Format(@"{0}", item.minAmount.Value) : string.Format(@"{0} ~ {1}", item.minAmount.Value, item.maxAmount.Value);
                    fixAmountModels.Add(new { id = item.level.Value, label = item.amount.Value });
                }
                if(queryResult != null 
                    && queryResult.Count > 0)
                    fixAmountLimitLevel = queryResult.First().fixAmountLimitLevel;
            }

            //定额提款金额限制权限
            var fixWAmountLimitLevel = 1;
            var fixWAmountModels = new List<object>();
            if (riskTypsObject != null && riskTypsObject.Count > 0) {
                var dy = riskTypsObject.First();
                var fixAmountLevelModels = JsonConvert.DeserializeObject<dynamic>(dy.fixWAmountLevels.ToString());

                foreach (var item in fixAmountLevelModels) {
                    var label = string.Format(@"{0} ~ {1}", item.minAmount.Value, item.maxAmount.Value);
                    fixWAmountModels.Add(new { id = item.level.Value, label = label });
                }
                if (queryResult != null
                    && queryResult.Count > 0)
                    fixWAmountLimitLevel = queryResult.First().fixWAmountLimitLevel;
            }



            #endregion

            #region 已经禁用的商户ids 
            //var disabledMids = new List<int>();
            //var disabledMidsKV = new List<object>();
            //if (queryResult != null 
            //    && queryResult.Count ==1 
            //    && !string.IsNullOrEmpty(queryResult.First().disabledMids))
            //{

            //    disabledMids = JsonConvert.DeserializeObject<List<int>>(queryResult.First().disabledMids);
            //    var queryWhere = string.Empty;
            //    foreach (var item in disabledMids)
            //    {
            //        queryWhere = string.IsNullOrEmpty(queryWhere) ? item.ToString() : string.Format(@"{0},{1}", queryWhere, item.ToString());
            //    }
            //    if (!string.IsNullOrEmpty(queryWhere))
            //    {
            //        var sql = string.Format(@"select id,name from ofThirdpay with(nolock) where id in ({0})", queryWhere);
            //        var merchantModel = DapperBase.Query<dynamic>(sql, null,
            //                    null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            //        if (merchantModel != null && merchantModel.Count > 0)
            //        {
            //            foreach (var item in merchantModel)
            //            {
            //                disabledMidsKV.Add(new { id = item.id, label = item.name });
            //            }
            //        }
            //    }
            //}

            #endregion

            #region 已经禁用的商户ids + 次数限制 

            var disabledMids = new List<int>();
            var disabledMidsKV = new List<object>();
            var disabledMidsTimes = new Dictionary<int, int>();
            if (queryResult != null && queryResult.Count == 1&& !string.IsNullOrEmpty(queryResult.First().disabledMids)) {
                if (queryResult.First().disabledMids.Contains("#")) {
                    var disabledMidsV = JsonConvert.DeserializeObject<List<string>>(queryResult.First().disabledMids);
                    foreach (var item in disabledMidsV) {
                        var key = item.Split('#')[0];
                        var value = item.Split('#')[1];
                        if(!disabledMidsTimes.ContainsKey(Convert.ToInt32(key)))
                            disabledMidsTimes.Add(Convert.ToInt32(key), Convert.ToInt32(value));
                        else
                        {
                            disabledMidsTimes.Remove(Convert.ToInt32(key));
                            disabledMidsTimes.Add(Convert.ToInt32(key), Convert.ToInt32(value));
                        }
                    }
                }
                else{
                    disabledMids = JsonConvert.DeserializeObject<List<int>>(queryResult.First().disabledMids);
                    foreach (var item in disabledMids) {
                        if(!disabledMidsTimes.ContainsKey(item))
                            disabledMidsTimes.Add(item, 0);//之前全部默认为0 次，没有次数可用
                    }
                }
                
                var queryWhere = string.Empty;
                foreach (var item in disabledMidsTimes) {
                    queryWhere = string.IsNullOrEmpty(queryWhere) ? item.Key.ToString() : string.Format(@"{0},{1}", queryWhere, item.Key);
                }
                if (!string.IsNullOrEmpty(queryWhere)) {
                    var sql = string.Format(@"select id,name from ofThirdpay with(nolock) where id in ({0})", queryWhere);
                    var merchantModel = DapperBase.Query<dynamic>(sql, null,
                                null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
                    if (merchantModel != null && merchantModel.Count > 0) {
                        foreach (var item in merchantModel) {
                            var key = (int)(item.id);
                            var lable = string.Format(@"{0}-日次数{1}", item.name, disabledMidsTimes[key]);
                            disabledMidsKV.Add(new { id = item.id, label = lable });
                        }
                    }
                }
            }

            #endregion

            //存款实名验证
            var isneedcheckrealname = (queryResult != null && queryResult.Count == 1) ? queryResult.First().isneedcheckrealname : 0;
            //是否可以参加定额提款
            var isLimitedfixWithdrawal = (queryResult != null && queryResult.Count == 1) ? queryResult.First().isLimitedfixWithdrawal : 0;
            //是否可以参加支付宝定额提款
            var isLimitedAliFixWithdrawal = (queryResult != null && queryResult.Count == 1) ? queryResult.First().isLimitedAliFixWithdrawal : 0;
            //是否可以参加微信定额提款
            var isLimitedWXFixWithdrawal = (queryResult != null && queryResult.Count == 1) ? queryResult.First().isLimitedWXFixWithdrawal : 0;

            var isLimitSpreadBilled = (queryResult != null && queryResult.Count == 1) ?
                queryResult.First().isLimitSpreadBilled : (queryResult.Count == 0 ? 1 : 0);
            var realNames = "";
            if(queryResult != null && queryResult.Count == 1) {
                var names = JsonConvert.DeserializeObject<List<string>>(queryResult.First().realNames);
                foreach (var name in names) {
                    realNames = string.IsNullOrEmpty(realNames) ? name : string.Format(@"{0}#{1}", realNames, name);
                }
            }

            return new 查询会员加码比例Return()
            {
                code = code ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                message = code ? "查询加码比例-成功" : "查询加码比例-失败",
                Data = new { betCodeDetail= betCodeObject, b_status= code ? queryResult.First().b_status:0,
                    specialDepositPromos = specialDepositPromos,
                    specialDepositAready = specialDepositAready,
                    disabledMidsKV = disabledMidsKV,
                    disabledMids = disabledMids,
                    riskModels = riskModels,
                    riskTypeAready = riskTypeAready,
                    riskRemark = riskRemark,

                    vipStatusModels = vipStatusModels,
                    vipTypeAready = vipTypeAready,
                    vipStatusRemark = vipStatusRemark,

                    isneedcheckrealname = isneedcheckrealname,
                    isLimitSpreadBilled = isLimitSpreadBilled,
                    fixAmountLimitLevel = fixAmountLimitLevel,
                    fixAmountModels = fixAmountModels,
                    fixWAmountLimitLevel = fixWAmountLimitLevel,
                    fixWAmountModels = fixWAmountModels,
                    withdrawMinAmount = code ? queryResult.First().withdrawMinAmount : 0,
                    withdrawMaxAmount = code ? queryResult.First().withdrawMaxAmount : 0,
                    realNames = realNames,
                    isLimitedfixWithdrawal = isLimitedfixWithdrawal,
                    isLimitedAliFixWithdrawal = isLimitedAliFixWithdrawal,
                    isLimitedWXFixWithdrawal = isLimitedWXFixWithdrawal
                }
                
            };
        }
    }


    


}
