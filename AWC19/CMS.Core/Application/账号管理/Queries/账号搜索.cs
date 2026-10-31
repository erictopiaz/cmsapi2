using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib.lang;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 账号搜索 : PagingModel, IRequest<ResponseModel<PagingModel<账号搜索Return>>>
    {
        public int currencycode { get; set; }
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public long id { get; set; } = 0;
        /// <summary>
        /// 游戏账号 
        /// inr:  bayo+ofUser.id
        /// awc:  awc+ofUser.id
        /// </summary>
        public string uid { get; set; } 
        public string account { get; set; }
        
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string realname { get; set; }

        public int level { get; set; }
        public int currencytype { get; set; }
        public int status { get; set; } = -1;
        public string referrerreg { get; set; }
        public string affaccount { get; set; }
        public string marcode { get; set; }
        public string qq { get; set; }
        //public int trynumber { get; set; }
        public string bbinaccount { get; set; }
        public string birthday { get; set; }
        public float? depositamount { get; set; } = -1;
        public string ipaddress { get; set; }
        public string withdrawcard { get; set; }
        public string anomally { get; set; }
        /// <summary>
        /// 默认风控
        /// </summary>
        public int anomaly { set; get; }
        /// <summary>
        /// 新增风控类型
        /// </summary>
        public int riskTypesAready { set; get; }
        /// <summary>
        /// vip状态
        /// </summary>
        public int vipTypeAready { set; get; } = -1;
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";

        public string Admin { get; set; }

    }

    public class 账号搜索Return
    {
        //public long id { get; set; }
        //public int? accounttype { get; set; }
        //public string nickname { get; set; }
        //public int sex { get; set; }
        //public string birthday { get; set; }
        //public decimal amountactive { get; set; }
        //public decimal amountsuspend { get; set; }
        //public string macaddress { get; set; }
        //public int depositnumber { get; set; }
        //public decimal depositamount { get; set; }
        //public int withdrawalnumber { get; set; }
        //public decimal withdrawalamount { get; set; }
        //public int loginnumber { get; set; }
        //public string affaccount { get; set; }
        //public string freezereason { get; set; }
        //public decimal bonuspoints { get; set; }
        //public string marcode { get; set; }
        //public DateTime? transtatus { get; set; }
        //public string withdrawcards { get; set; }
        //public string misc { get; set; }
        //public string qq { get; set; }
        //public string bbinaccount { get; set; }
        public string account { get; set; }
        public long id { get; set; }
        public string realname { get; set; }
        public int status { get; set; }
        public int trynumber { get; set; }
        public int lockseconds { get; set; }
        public int level { get; set; }
        public string birthday { get; set; }
        public string levelname { get; set; }
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        public DateTime createtime { get; set; }
        public string referrerreg { get; set; }
        public string ipaddress { get; set; }
        internal long TotalItems { get; set; }
        public long winloss { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public DateTime lockdate { get; set; }

        public string verified { get; set; }
        [JsonIgnore]
        public string misc { get; set; }

        public string anomally { get; set; }
        /// <summary>
        /// 默认风控
        /// </summary>
        public int anomaly { set; get; }
        public int regdevice { set; get; }
        /// <summary>
        /// 新增风控类型
        /// </summary>
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
        public string prevphone { get; set; }
        public string currencytype { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 账号搜索Validator : AbstractValidator<账号搜索>
    {
        public 账号搜索Validator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.uid).Account();
            RuleFor(x => x.id).Id();
            //RuleFor(x => x.account).AccountRegex();
            RuleFor(x => x.starttime).Datetime();
            RuleFor(x => x.endtime).Datetime();
            RuleFor(x => x.telephone).DigitOnly(10, 19);
            RuleFor(x => x.email).EmailAddressNullable();
            RuleFor(x => x.realname).Realname();
            RuleFor(x => x.level).Level();
            RuleFor(x => x.status).Status();
            //RuleFor(x => x.referrerreg).();
            RuleFor(x => x.affaccount).Affiliate();
            RuleFor(x => x.marcode).CharAndDigital(6, 20);
            RuleFor(x => x.qq).DigitOnly(6, 20);
            RuleFor(x => x.bbinaccount).Account();
            RuleFor(x => x.birthday).DateTimeStr();
            RuleFor(x => x.ipaddress).IP();
            RuleFor(x => x.withdrawcard).CharAndDigitalNullable(1, 49);
            //RuleFor(x => x.anomaly{ set;LKCS get; }).;

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账号搜索Handler : BaseRequestHandler<账号搜索, ResponseModel<PagingModel<账号搜索Return>>>
    {
        public 账号搜索Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<账号搜索Return>> Handle(账号搜索 request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<账号搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // Get Account Created based on the start Date

            // Get All Successful Deposit on that date onwards

            //TODO: Do This after everything else

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();


            /*
                    public string account { get; set; }
                    public DateTime? createtime { get; set; }
                    public string telephone { get; set; }
                    public string realname { get; set; }
                    public int level { get; set; }
                    public int status { get; set; } = -1;
                    public string referrerreg { get; set; }
                    public string affaccount { get; set; }
                    public string marcode { get; set; }
                    public string qq { get; set; }
                    public string bbinaccount { get; set; }
                    public float? depositamount { get; set; } = -1;
                    public string ipaddress { get; set; }
             */


            bool withWhere = false;

            // FILL UP THE WHERE CLAUSE

            if (request.starttime != null && request.endtime != null)
            {
                if (request.starttime.Year > 2000)
                {
                    withWhere = true;
                    builder.Where("u.createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                }
            }
            else if (request.starttime != null)
            {
                withWhere = true;
                builder.Where("u.createtime >= @starttime", new { starttime = request.starttime });
            }

            if(!string.IsNullOrEmpty(request.uid))
                request.id = Convert.ToInt64(request.uid);

            #region 处理bbin 账号查询   aw+用户ID 直接给到用户id 查询 
            if (!string.IsNullOrEmpty(request.bbinaccount)) {

                var uidString = request.bbinaccount.ToLower().Replace("aw", "");
                long ruid = 0L;
                if (Int64.TryParse(uidString, out ruid)) {
                    request.id = ruid;
                    request.bbinaccount = "";
                }
                else {
                    return new ResponseModel<PagingModel<账号搜索Return>>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "bbinaccount 格式有误 ",
                        Data = new PagingModel<账号搜索Return>() {
                            Data = new List<账号搜索Return>(),
                            TotalItems = 0
                        }
                    };
                }
            }
            #endregion

            if (request.id > 0)
                builder.Where("u.id = @ID", new { ID = request.id });

            


            if (!string.IsNullOrEmpty(request.account)) 
                builder.Where("u.account LIKE @account", new { account = string.Concat("%", request.account, "%") });
            if (!string.IsNullOrEmpty(request.withdrawcard)) 
                builder.Where("u.withdrawcards LIKE @withdrawcard", new { withdrawcard = string.Concat("%", request.withdrawcard, "%") });


            if (request.status >= 0 && request.status != 4)
            {
                withWhere = true;
                builder.Where("u.status = @STATUS", new { STATUS = request.status });
            }

            if (request.riskTypesAready >= 0)
            {
                withWhere = true;

                if(request.riskTypesAready != 0)
                    builder.Where("ue.riskTypesAready = @STATUS", new { STATUS = request.riskTypesAready });
                else
                    builder.Where("(ue.riskTypesAready=0 OR ue.riskTypesAready IS NULL)");
            }
            
            #region ching添加  vip状态修改
            if (request.vipTypeAready >= 0)
            {
                withWhere = true;
                if (request.vipTypeAready != -1)
                    builder.Where("ue.vipTypeAready = @VIP_STATUS", new { VIP_STATUS = request.vipTypeAready });
            }
            #endregion

            if (request.status == -100) { withWhere = true; builder.Where("u.trynumber > 0 "); };

            if (!string.IsNullOrEmpty(request.telephone)) { withWhere = true; builder.Where("u.telephone = @TELEPHONE", new { TELEPHONE = request.telephone }); }
            if (!string.IsNullOrEmpty(request.email)) { withWhere = true; builder.Where("u.email = @EMAIL", new { email = request.email }); }

            if (!string.IsNullOrEmpty(request.birthday))
            {
                withWhere = true;
                if (request.birthday.Length <= 5)
                {
                    if (request.birthday.StartsWith("0")) request.birthday = "-" + request.birthday;
                    builder.Where("u.birthday LIKE @birthday", new { birthday = string.Concat("%", request.birthday) });
                }

                else
                    builder.Where("u.birthday = @birthday", new { birthday = request.birthday });
            }

            if (request.level > 0) { withWhere = true; builder.Where("u.level = @LEVEL", new { LEVEL = request.level }); }
            //if (!string.IsNullOrEmpty(request.realname)) { withWhere = true; builder.Where("realname = @REALNAME", new { REALNAME = request.realname }); }

            if (!string.IsNullOrEmpty(request.realname)) { withWhere = true; builder.Where("u.realname LIKE @REALNAME", new { REALNAME = string.Concat(request.realname, "%") }); }

            if (request.currencycode > 0) { builder.Where("u.currencyType = @currencycode", new { request.currencycode }); }
            if (request.depositamount > 0) { withWhere = true; builder.Where("u.depositamount >= @depositamount", new { request.depositamount }); }
            if (request.depositamount == 0) { withWhere = true; builder.Where("u.depositamount = @depositamount", new { request.depositamount }); }
            if (!string.IsNullOrEmpty(request.referrerreg)) { withWhere = true; builder.Where("u.referrerreg = @REFERRERREG", new { REFERRERREG = request.referrerreg }); }
            if (!string.IsNullOrEmpty(request.affaccount)) { withWhere = true; builder.Where("u.affaccount = @AFFACCOUNT", new { AFFACCOUNT = request.affaccount }); }
            if (!string.IsNullOrEmpty(request.marcode)) { withWhere = true; builder.Where("u.marcode = @MARCODE", new { MARCODE = request.marcode }); }
            if (!string.IsNullOrEmpty(request.qq)) { withWhere = true; builder.Where("u.qq = @qq", new { request.qq }); }

            if (!string.IsNullOrEmpty(request.bbinaccount)) { withWhere = true; builder.Where("u.bbinaccount = @bbinaccount", new { request.bbinaccount }); }
            if (!string.IsNullOrEmpty(request.ipaddress)) { withWhere = true; builder.Where("u.ipaddress LIKE @ipaddress", new { ipaddress = string.Concat(request.ipaddress, "%") }); }

            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { withWhere = true; builder.Where("u.merchantAccount = @merchantAccount", new { request.merchantAccount }); }
            if (request.walletType >= 0) { withWhere = true; builder.Where("u.walletType = @walletType", new { request.walletType }); }
            //  if (request.currencytype > 0) { withWhere = true; builder.Where("u.currencytype = @currencytype", new { currencytype = string.Concat(request.currencytype,"") }); }
            if (request.status == 1)
            {
                withWhere = true;
                builder.Where("(u.misc NOT LIKE '%gamedisabledstat%' OR u.misc NOT LIKE '%promodisabled%')");
            }
            if (request.status == 4)
            {
                withWhere = true;
                builder.Where("(u.misc LIKE '%gamedisabledstat%' OR u.misc LIKE '%promodisabled%')");
            }
            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new
            {
                Offset = offset,
                Next = request.PageSize
            });
            builder.OrderBy("id DESC");


            SqlBuilder.Template template;
            if (!withWhere)
            {
                template = builder.AddTemplate("" +
                "SELECT walletType, merchantAccount,currencytype,lockdate,withdrawcards AS verified, id,trynumber,telephone,email,u.account,realname,status,level,createtime,anomaly,referrerreg,ipaddress,regdevice,birthday,misc,ue.riskTypesAready,ue.riskRemark, TotalItems = 10000 " +
                "FROM ofUser u WITH(NOLOCK) left join ofUserExtention ue WITH(NOLOCK) on u.account=ue.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");
                //builder.Where("id > 417120");
            }
            else
                template = builder.AddTemplate("" +
                "SELECT walletType, merchantAccount,currencytype,lockdate,withdrawcards AS verified,id,telephone,email,trynumber,u.account,realname,status,level,createtime,anomaly,referrerreg,ipaddress,regdevice,birthday,misc,ue.riskTypesAready,ue.riskRemark,(depositamount-withdrawalamount) AS winloss , COUNT(*) OVER() as TotalItems " +
                "FROM ofUser u WITH(NOLOCK) left join ofUserExtention ue WITH(NOLOCK) on u.account=ue.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<账号搜索Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool showTelephone = false;
                bool showEmail = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;
                if (adminAccess.Contains("/api/Account/AllowToSeeEmail")) showEmail = true;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        if (x.lockdate != null)
                        {
                            var remainingseconds = (x.lockdate.AddMinutes(15) - DateTime.UtcNow.AddHours(8)).TotalSeconds;
                            x.lockseconds = remainingseconds >= 0 ? (int)remainingseconds : 0;
                        }

                        if (x.account == x.telephone || UtilsLib.Common.IsIncludedTel(x.account))
                            x.account = Common.Security.ObscureString(x.account);

                        if (!showTelephone) x.telephone = "****";
                        if (!showEmail && !string.IsNullOrEmpty(x.email))
                        {
                            try
                            {
                                x.email = x.email.Contains('＠') ? x.email.Replace('＠', '@') : x.email;
                                x.email = "***" + x.email.Substring(x.email.IndexOf('@'));
                            }
                            catch (Exception)
                            {
                                x.email = "***";
                            }
                        }

                        #region 根据当前货币查询 显示不同的货币等级
                        if (request.currencycode == (int)LangCodes.CurrencyCode.INR
                        || request.currencycode == (int)LangCodes.CurrencyCode.USD) {
                            if (VipLevelINTL.levelIntlName.ContainsKey(x.level))
                                x.levelname = VipLevelINTL.levelIntlName[x.level];
                        }
                        else {
                            x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        }
                        #endregion
                        //x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];


                        x.currencytype = CMS.Core.CachedData.CurrencyTypeDict[int.Parse(x.currencytype)];
                        x.verified = string.IsNullOrEmpty(x.verified) ? "0" : "1";

                        Dictionary<string, string> tempMisc = new Dictionary<string, string>();
                        if (x.misc != null)
                            tempMisc = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(x.misc);

                        if (tempMisc != null)
                        {
                            x.anomally = "";
                            if ((tempMisc.ContainsKey("gamedisabledstat") && !string.IsNullOrEmpty(tempMisc["gamedisabledstat"])) ||
                                (tempMisc.ContainsKey("promodisabled") && tempMisc["promodisabled"] != "[]"))
                            {
                                x.anomally = "风控";
                            }

                            if ((tempMisc.ContainsKey("prevphone") && !string.IsNullOrEmpty(tempMisc["prevphone"])))
                            {
                                x.prevphone = tempMisc["prevphone"];
                            }

                        }

                        //if (x.trynumber >= 4)
                        //{
                        //    {
                        //        if (tempMisc != null && tempMisc.ContainsKey("lockdate"))
                        //        {
                        //            var remainingseconds = (DateTime.Parse(tempMisc["lockdate"]) - DateTime.UtcNow.AddHours(8)).TotalSeconds;
                        //            x.lockseconds = remainingseconds >= 0 ? (int)remainingseconds : 0;
                        //        }



                        //    }
                        //}
                    });





                return new ResponseModel<PagingModel<账号搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<账号搜索Return>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }





    }
}
