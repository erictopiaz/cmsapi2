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
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.电销管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 活跃记录Query : PagingModel, IRequest<ResponseModel<PagingModel<活跃记录Return>>>
    {
        public string account { get; set; }
        public string affaccount { get; set; }
        public DateTime? starttime { get; set; }
        public int level { get; set; } = -1;
        public string Admin { get; set; }
        //public DateTime? endtime { get; set; }
        public string email { get; set; }
    }

    public class 活跃记录Return
    {
        public DateTime? transtatus { get; set; }
        public string email { get; set; }
        public string affaccount { get; set; }
        public decimal winloss{ get; set; }
        public decimal depositamount { get; set; }
        public decimal withdrawalamount { get; set; }

        public long uid { get; set; }

        public int depositnumber { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string referrerreg { get; set; }
        public int level { get; set; }
        public DateTime createtime { get; set; }
        public DateTime? lastdeposittime { get; set; }
        public int status { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public string birthday { get; set; }
        public string telephone { get; set; }
        public string levelname { get; set; }
        public long TotalItems { get; set; }
        public int riskTypesAready { set; get; }
        public string riskRemark { set; get; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 活跃记录QueryValidator : AbstractValidator<活跃记录Query>
    {
        public 活跃记录QueryValidator()
        {
            //RuleFor(x => x.Account).Account();
            //RuleFor(x => x.CardNo).CardNo();
            //RuleFor(x => x.Bank).CharAndDigital(1, 10);
            //RuleFor(x => x.VerifyTime).Datetime();
            //RuleFor(x => x.Status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 活跃记录Handler : BaseRequestHandler<活跃记录Query, ResponseModel<PagingModel<活跃记录Return>>>
    {
        public 活跃记录Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<活跃记录Return>> Handle(活跃记录Query request)
        {
           
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            bool withWhere = false;

            // FILL UP THE WHERE CLAUSE

            if (request.starttime != null)
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
            else
                builder.Where("requesttime >= @starttime", new { starttime = DateTime.UtcNow.AddHours(8).AddDays(-7).ToString("yyyy-MM-dd") });


            builder.Where("fi.status=1");
            if (!string.IsNullOrEmpty(request.account)) { withWhere = true; builder.Where("fi.account = @account", new { request.account }); }

            if (!string.IsNullOrEmpty(request.affaccount)) { withWhere = true; builder.Where("affaccount = @affaccount", new { request.affaccount }); }
            if (!string.IsNullOrEmpty(request.email)) { withWhere = true; builder.Where("email = @email", new { request.email }); }
            if (request.level > 0) { withWhere = true; builder.Where("level = @level", new { request.level}); }


            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new
            {
                Offset = offset,
                Next = request.PageSize,
            });
            //builder.OrderBy("transtatus ASC");


            //jobj.Add("id", item.id);
            //jobj.Add("account", item.account);
            //jobj.Add("telephone", "");
            //jobj.Add("level", item.level);
            //jobj.Add("referrerreg", item.referrerreg);        
            //jobj.Add("realname", item.realname);
            //jobj.Add("status", item.status);
            //jobj.Add("createtime", item.createtime);
            //jobj.Add("did", item.depositamount);
            //jobj.Add("dnum", item.depositnumber);
            //jobj.Add("transtatus", item.transtatus);
            //jobj.Add("affaccount", item.affaccount);
            //jobj.Add("ipaddress", item.ipaddress);
            //jobj.Add("macaddress", item.macaddress);

            SqlBuilder.Template template;
            if (!withWhere)
            {

                template = builder.AddTemplate("" +
                    "SELECT riskTypesAready,riskRemark,a.account as account,email,telephone,affaccount,ofuser.id uid,ofuser.level as level,referrerreg,realname,ofuser.status,createtime,depositamount,depositnumber,withdrawalamount,a.transtatus as lastdeposittime,affaccount,ipaddress,macaddress, TotalItems = 10000 FROM(" +
                    "select fi.account as account,MAX(requesttime) AS transtatus " +
                    "from ofdeposit fi with(nolock) " +
                    "left join ofUserExtention ue WITH(NOLOCK) on fi.account=ue.account " +
                    "/**where**/ " +
                    "group by fi.account " +
                    "order by transtatus desc " +
                    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
                    "LEFT JOIN ofuser WITH(NOLOCK) ON a.account=ofuser.account " +
                    "left join ofUserExtention ue WITH(NOLOCK) on ofuser.account=ue.account");
                //template = builder.AddTemplate("" +
                //"SELECT id,account,telephone,affaccount,level,referrerreg,realname,status,createtime,depositamount,depositnumber,transtatus,affaccount,ipaddress,macaddress, TotalItems = 10000 " +
                //"FROM ofUser WITH(NOLOCK) /**where**/ /**orderby**/ " +
                //"OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                //"FETCH NEXT @Next ROWS ONLY");
                //builder.Where("id > 417120");
            }
            else
            {
                template = builder.AddTemplate(
                    "SELECT lastdeposittime,id,riskTypesAready,riskRemark,b.account,email,telephone,ofUser.id uid,level,referrerreg,realname,status,createtime,depositamount,depositnumber,withdrawalamount,affaccount,ipaddress,macaddress,TotalItems " +
                    "FROM (select fi.account as account, COUNT(*) OVER() as TotalItems ,MAX(requesttime) AS lastdeposittime " +
                    "from ofdeposit fi with(nolock) LEFT JOIN ofuser WITH(NOLOCK) ON fi.account=ofuser.account /**where**/ group by fi.account  order by lastdeposittime desc OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)b " +
                    "LEFT JOIN ofUser WITH(NOLOCK) ON b.account=ofuser.account " +
                    "left join ofUserExtention ue WITH(NOLOCK) on ofuser.account=ue.account ");
            }
                //template = builder.AddTemplate("" +
                //    "SELECT * FROM (SELECT id,a.account,email,telephone,ofuser.level as level,referrerreg,realname,status,createtime,depositamount,depositnumber,transtatus,affaccount,ipaddress,macaddress, COUNT(*) OVER() as TotalItems FROM(" +
                //    "select account,MAX(requesttime) AS lastdeposittime " +
                //    "from ofdeposit fi with(nolock) " +
                //    "/**where**/ " +
                //    "group by account " +
                //    "order by lastdeposittime desc " +
                //    "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
                //    "JOIN ofuser WITH(NOLOCK) ON a.account=ofuser.account WHERE ofuser.level = @Level)dd");

            //}
            //else {



            //    template = builder.AddTemplate("" +
            //        "SELECT * FROM (SELECT id,a.account,email,telephone,ofuser.level as level,referrerreg,realname,status,createtime,depositamount,depositnumber,transtatus,affaccount,ipaddress,macaddress, COUNT(*) OVER() as TotalItems FROM(" +
            //        "select account,MAX(requesttime) AS lastdeposittime " +
            //        "from ofdeposit fi with(nolock) " +
            //        "/**where**/ " +
            //        "group by account " +
            //        "order by lastdeposittime desc " +
            //        "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY)a " +
            //        "JOIN ofuser WITH(NOLOCK) ON a.account=ofuser.account )dd");
            //template = builder.AddTemplate("" +
            //    "SELECT id,account,telephone,affaccount,level,referrerreg,realname,status,createtime,depositamount,depositnumber,transtatus,affaccount,ipaddress,macaddress, COUNT(*) OVER() as TotalItems " +
            //    "FROM ofUser WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //    "FETCH NEXT @Next ROWS ONLY");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<活跃记录Return>(template.RawSql, template.Parameters);
                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool showTelephone = false;
                bool showEmail = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;
                if (adminAccess.Contains("/api/Account/AllowToSeeEmail")) showEmail = true;



                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(
                        x =>
                        {

                            x.winloss = x.withdrawalamount - x.depositamount;
                            if (x.account == x.telephone)
                                x.account = Common.Security.ObscureString(x.account);
                            if (!showTelephone)
                                x.telephone = x.depositamount == 0 ? x.telephone : "****";

                            if (!showEmail && !string.IsNullOrEmpty(x.email)) x.email = "***" + x.email.Substring(x.email.IndexOf('@'));

                            //if (!showEmail)
                            //    x.email =  "****";

                            x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        });
                        


                return new ResponseModel<PagingModel<活跃记录Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<活跃记录Return>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }
    }
}

