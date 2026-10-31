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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib.lang;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Queries {
    
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 账号搜索电销 : PagingModel, IRequest<ResponseModel<PagingModel<账号搜索电销Return>>> {
        public int currencycode { get; set; }
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public long id { get; set; } = 0;
        public string account { get; set; }
        
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        //public string telephone { get; set; }
        //public string email { get; set; }
        //public string realname { get; set; }

        //public int level { get; set; }
        //public int currencytype { get; set; }
        public int status { get; set; } = -1;
        //public string referrerreg { get; set; }
        //public string affaccount { get; set; }
        //public string marcode { get; set; }
        //public string qq { get; set; }
        //public string bbinaccount { get; set; }
        //public string birthday { get; set; }
        public float? depositamount { get; set; } = 0;
        //public string ipaddress { get; set; }
        //public string withdrawcard { get; set; }
        //public string anomally { get; set; }
        ///// <summary>
        ///// 默认风控
        ///// </summary>
        //public int anomaly { set; get; }
        ///// <summary>
        ///// 新增风控类型
        ///// </summary>
        //public int riskTypesAready { set; get; }
        public string Admin { get; set; }

    }

    public class 账号搜索电销Return {
        
        public string account { get; set; }
        public long id { get; set; }
        public string realname { get; set; }
        public int status { get; set; }
        public int trynumber { get; set; }
        public int lockseconds { get; set; }
        public int level { get; set; }
        public string birthday { get; set; }
        public string levelname { get; set; }
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
    public class 账号搜索电销Validator : AbstractValidator<账号搜索电销> {
        public 账号搜索电销Validator() {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.id).Id();
            RuleFor(x => x.starttime).Datetime();
            RuleFor(x => x.endtime).Datetime();
            //RuleFor(x => x.telephone).DigitOnly(10, 19);
            //RuleFor(x => x.email).EmailAddressNullable();
            //RuleFor(x => x.realname).Realname();
            //RuleFor(x => x.level).Level();
            RuleFor(x => x.status).Status();
            //RuleFor(x => x.affaccount).Affiliate();
            //RuleFor(x => x.marcode).CharAndDigital(6, 20);
            //RuleFor(x => x.qq).DigitOnly(6, 20);
            //RuleFor(x => x.bbinaccount).Account();
            //RuleFor(x => x.birthday).DateTimeStr();
            //RuleFor(x => x.ipaddress).IP();
            //RuleFor(x => x.withdrawcard).CharAndDigitalNullable(1, 49);

        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 账号搜索电销Handler : BaseRequestHandler<账号搜索电销, ResponseModel<PagingModel<账号搜索电销Return>>> {
        public 账号搜索电销Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<账号搜索电销Return>> Handle(账号搜索电销 request) {
            if (!request.currencys.Contains(request.currencycode)) {
                return new ResponseModel<PagingModel<账号搜索电销Return>>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            bool withWhere = false;



            // FILL UP THE WHERE CLAUSE

            #region 今年注册且没有存款的会员
            request.depositamount = 0;//只能查询存款为0 会员

            var currentDate = DateTime.UtcNow.AddHours(8);
            var firtDayOfYear = currentDate.ToString("yyyy-01-01 00:00:00");
            if (Convert.ToDateTime(request.starttime) <= Convert.ToDateTime(firtDayOfYear))
                request.starttime = Convert.ToDateTime(firtDayOfYear);
            #endregion 

            if (request.starttime != null && request.endtime != null) {
                if (request.starttime.Year > 2000) {
                    withWhere = true;
                    builder.Where("u.createtime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                }
            }
            else if (request.starttime != null) {
                withWhere = true;
                builder.Where("u.createtime >= @starttime", new { starttime = request.starttime });
            }

            if (request.id > 0)
                builder.Where("u.id = @ID", new { ID = request.id });
            if (!string.IsNullOrEmpty(request.account)) builder.Where("u.account LIKE @account", new { account = string.Concat(request.account, "%") });
            
            if (request.status >= 0 && request.status != 4) {
                withWhere = true;
                builder.Where("u.status = @STATUS", new { STATUS = request.status });
            }

            

            if (request.status == -100) { withWhere = true; builder.Where("u.trynumber > 0 "); };

            
            if (request.currencycode > 0) { builder.Where("u.currencyType = @currencycode", new { request.currencycode }); }
            //if (request.depositamount > 0) { withWhere = true; builder.Where("u.depositamount >= @depositamount", new { request.depositamount }); }
            if (request.depositamount == 0) 
                { withWhere = true; builder.Where("u.depositamount = @depositamount", new { request.depositamount }); }
            
            if (request.status == 1) {
                withWhere = true;
                builder.Where("(u.misc NOT LIKE '%gamedisabledstat%' OR u.misc NOT LIKE '%promodisabled%')");
            }
            if (request.status == 4) {
                withWhere = true;
                builder.Where("(u.misc LIKE '%gamedisabledstat%' OR u.misc LIKE '%promodisabled%')");
            }
            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new {
                Offset = offset,
                Next = request.PageSize
            });
            builder.OrderBy("id DESC");


            SqlBuilder.Template template;
            if (!withWhere) {
                template = builder.AddTemplate("" +
                "SELECT currencytype,lockdate,withdrawcards AS verified, id,trynumber,telephone,email,u.account,realname,status,level,createtime,anomaly,referrerreg,ipaddress,regdevice,birthday,misc,ue.riskTypesAready,ue.riskRemark, TotalItems = 10000 " +
                "FROM ofUser u WITH(NOLOCK) left join ofUserExtention ue WITH(NOLOCK) on u.account=ue.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");
                //builder.Where("id > 417120");
            }
            else
                template = builder.AddTemplate("" +
                "SELECT currencytype,lockdate,withdrawcards AS verified,id,telephone,email,trynumber,u.account,realname,status,level,createtime,anomaly,referrerreg,ipaddress,regdevice,birthday,misc,ue.riskTypesAready,ue.riskRemark,(depositamount-withdrawalamount) AS winloss , COUNT(*) OVER() as TotalItems " +
                "FROM ofUser u WITH(NOLOCK) left join ofUserExtention ue WITH(NOLOCK) on u.account=ue.account /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var rows = connection.Query<账号搜索电销Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool showTelephone = false;
                bool showEmail = false;
                if (request.depositamount == 0) {
                    //if (adminAccess.Contains("/api/Account/AllowToSeePhone") ) showTelephone = true;
                    showTelephone = true;
                }
                else {
                    if (adminAccess.Contains("/api/Account/AllowToSeePhone")) 
                        showTelephone = true;
                }
                if (adminAccess.Contains("/api/Account/AllowToSeeEmail")) 
                    showEmail = true;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x => {
                        if (x.lockdate != null) {
                            var remainingseconds = (x.lockdate.AddMinutes(15) - DateTime.UtcNow.AddHours(8)).TotalSeconds;
                            x.lockseconds = remainingseconds >= 0 ? (int)remainingseconds : 0;
                        }

                        if (x.account == x.telephone)
                            x.account = Common.Security.ObscureString(x.account);
                        if (!showTelephone) 
                            x.telephone = "****";
                        if (!showEmail && !string.IsNullOrEmpty(x.email)) {
                            x.email = x.email.Contains('＠') ? x.email.Replace('＠', '@') : x.email;
                            x.email = "***" + x.email.Substring(x.email.IndexOf('@'));
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

                        if (tempMisc != null) {
                            x.anomally = "";
                            if ((tempMisc.ContainsKey("gamedisabledstat") && !string.IsNullOrEmpty(tempMisc["gamedisabledstat"])) ||
                                (tempMisc.ContainsKey("promodisabled") && tempMisc["promodisabled"] != "[]")) {
                                x.anomally = "风控";
                            }

                            if ((tempMisc.ContainsKey("prevphone") 
                            && !string.IsNullOrEmpty(tempMisc["prevphone"]))) {
                                x.prevphone = tempMisc["prevphone"];
                            }

                        }

                    });

                return new ResponseModel<PagingModel<账号搜索电销Return>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<账号搜索电销Return>() {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }





    }
}
