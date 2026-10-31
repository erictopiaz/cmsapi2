using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using ShareEntity.Models;
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

namespace CMS.Core.Application.存款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CrossTransactList : PagingModel, IRequest<ResponseModel<PagingModel<CrossTransactListReturn>>>
    {
        public int walletType { set; get; } = -1;
        public string? merchantAccount { set; get; } = "";
        public string account { get; set; }
        public string withdrawaccount { get; set; }
        public float depositamount { get; set; }
        public float withdrawamount { get; set; }
        public string orderid { get; set; } //商户单号
        //public string MerchantOrdernNo { get; set; } // 银行单号
        //public string Platform { get; set; } // 支付类型
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public int? level { get; set; } = -1;

        public int Status { get; set; } // 支付状态

        public string ipaddress { get; set; }  // 存款IP
        public string Affiliate { get; set; } // 代理
        //public int Website { get; set; } = -1; // 网址
        public string Admin { get; set; }
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }

    public class CrossTransactListReturn
    {
        public string walletType { get; set; }
        public string merchantAccount { get; set; }
        public long id { get; set; }
        public string depositaccount { get; set; }
        public string withdrawaccount { get; set; }
        public int imgupload { get; set; } // 支付状态
        public int requestcount { get; set; } // 支付状态
        public float depositamount { get; set; }
        public float withdrawamount { get; set; }
        //public long depositid { get; set; }
        public long withdrawid { get; set; }
        public int status { get; set; }
        public DateTime createtime { get; set; }
        public DateTime? reviewtime { get; set; }
        public DateTime? withdrawrequesttime { get; set; }
        public DateTime? conformedtime { get; set; }
        public double conformedtimediff { get; set; }
        public string orderid { get; set; }
        public string withdrawdetails { get; set; }
        public string reviewperson{ get; set; }
        public long TotalItems { get; set; }
        public long conformedStatus { get; set; }
        public double minutesdiff { get; set; }
        public double fixWithdrawP { get; set; }
        public string ipaddress { get; set; }
        public int depositstatus { get; set; } // 支付状态
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CrossTransactListValidator : AbstractValidator<CrossTransactList>
    {
        public CrossTransactListValidator()
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
    public class CrossTransactListHandler : BaseRequestHandler<CrossTransactList, ResponseModel<PagingModel<CrossTransactListReturn>>>
    {
        public CrossTransactListHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<CrossTransactListReturn>> Handle(CrossTransactList request)
        {
            request.starttime = request.starttime == null ? Common.CurrentTime.AddDays(-45) : request.starttime;


            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<CrossTransactListReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }


            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var totalItemsBuilder = new SqlBuilder();
            //var template = builder.AddTemplate("" +
            //    "SELECT * , COUNT(*) OVER() as TotalItems " +
            //    "FROM ofCrossTransact WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //    "FETCH NEXT @Next ROWS ONLY");
            SqlBuilder.Template template;
            SqlBuilder.Template totalItemsTemplate;


            if (string.IsNullOrEmpty(request.withdrawaccount) && 
                string.IsNullOrEmpty(request.account) && 
                request.currencycode <= 0)
            {
                template = builder.AddTemplate("" +
                    "SELECT  " +
                      "a.walletType, a.merchantAccount,a.reviewperson,b.account as depositaccount,ipaddress, b.amount as depositamount, b.imgupload,b.status as depositstatus, ISNULL(ISNULL(bp.createtime, paytime), NULL ) conformedtime, " +
                      "TotalItems, fixWithdrawP, withdrawrequesttime, conformedstatus, e.requestcount as requestcount, a.id as id, a.createtime as createtime, a.status as status, withdrawid, a.orderid as orderid, a.reviewtime as reviewtime, withdrawaccount, imgupload, a.amount as depositamount, a.amount as withdrawamount, CONCAT(a.realname, '/', a.cardbank, '/', a.cardno ) as withdrawdetails FROM " +
                      "( " +
                        "SELECT u.walletType, u.merchantAccount,0 as TotalItems,j.reviewperson, j.orderid as wd_orderid, c.fixWithdrawP, c.referenceguid as wd_refguid, c.paytime as paytime, c.requesttime as withdrawrequesttime, j.account as account, c.conformedStatus, c.realname, c.cardbank, c.cardno, c.amount, j.id, j.createtime, j.status, j.withdrawid, j.orderid, j.reviewtime, c.account as withdrawaccount  " +
                        "FROM ofcrosstransact j WITH(NOLOCK) " +
                        "LEFT JOIN ofWithdrawal c WITH(NOLOCK) c.id =j.withdrawid  " +
                        "LEFT JOIN ofUser u WITH(NOLOCK) on u.account = j.account " +
                        "LEFT JOIN ofDeposit b WITH(NOLOCK) on b.merchnatno = j.orderid " +

                        "ORDER BY CASE WHEN j.status = 0 THEN 0 ELSE 1 END asc, j.id desc OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY ) a  " +
                        //"outer apply(select account, amount, imgupload, ipaddress from ofdeposit od with(nolock) where od.merchantno = wd_orderid ) b  " +
                      "outer apply (select TOP 1 createtime from ofBonuspromo bb WITH(NOLOCK) WHERE bb.referenceguid = wd_refguid ) bp " +
                      "CROSS APPLY(SELECT COUNT(*) AS requestcount FROM ofcrosstransact WITH(NOLOCK) WHERE createtime > @TODAY AND account = a.account ) e ORDER BY " +
                      "CASE WHEN a.status = 0 THEN 0 ELSE 1 END asc "
                                    );
                totalItemsTemplate = totalItemsBuilder.AddTemplate("SELECT COUNT(1) " +
                    "FROM ofcrosstransact j WITH(NOLOCK)");
            }
            else
            {
                template = builder.AddTemplate("" +
                //"SELECT ISNULL(ISNULL(bp.createtime,c.paytime),NULL) conformedtime ,TotalItems,fixWithdrawP,withdrawrequesttime,conformedtime,conformedstatus,e.requestcount as requestcount,a.id as id, a.createtime as createtime,a.status as status,withdrawid,a.orderid as orderid,a.reviewtime as reviewtime,depositaccount, withdrawaccount,imgupload , a.amount as depositamount, a.amount as withdrawamount,CONCAT(a.realname,'/',a.cardbank,'/',a.cardno) as withdrawdetails " +
                //"FROM (" +
                //    "SELECT COUNT(*) OVER() as TotalItems, c.fixWithdrawP,     c.referenceguid as wd_refguid, c.requesttime as withdrawrequesttime, j.account as account, c.conformedStatus, c.realname,c.cardbank,c.cardno, c.amount,b.amount as depositamount,b.imgupload,j.id,j.createtime,j.status, j.withdrawid,j.orderid, j.reviewtime,b.account as depositaccount, c.account as withdrawaccount " +
                //    "FROM ofcrosstransact j  WITH(NOLOCK) " +
                //    "LEFT JOIN ofWithdrawal c  WITH(NOLOCK) on withdrawid = c.id " +
                //    //"LEFT JOIN ofBonusPromo bp on bp.referenceguid = c.referenceguid " +
                //    //"LEFT JOIN ofDeposit b on b.merchantno = j.orderid " +
                //    "/**where**/ ORDER BY CASE WHEN j.status=0 THEN 0 ELSE 1 END asc, j.id desc OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY) a " +
                //   "outer apply(select account, amount, imgupload from ofdeposit od with(nolock) where od.merchantno = j.orderid) b " +
                //    "outer apply (select TOP 1 createtime from ofBonuspromo bb WITH(NOLOCK) WHERE bb.referenceguid = wd_refguid) bp " +
                //    "CROSS APPLY(" +
                //    "   SELECT COUNT(*) AS requestcount " +
                //        "FROM ofcrosstransact WITH(NOLOCK) WHERE createtime > @TODAY AND account=a.account)e  ORDER BY CASE WHEN a.status=0 THEN 0 ELSE 1 END asc"

                "SELECT  " +
                  "a.walletType, a.merchantAccount,a.reviewperson, b.account as depositaccount, ipaddress, b.amount as depositamount, b.imgupload,b.status as depositstatus, " +
                  "ISNULL(ISNULL(bp.createtime, paytime), NULL ) conformedtime, " +
                  "TotalItems = 0, fixWithdrawP, withdrawrequesttime, conformedstatus, e.requestcount as requestcount, a.id as id, a.createtime as createtime, " +
                  "a.status as status, withdrawid, a.orderid as orderid, a.reviewtime as reviewtime, withdrawaccount, imgupload, a.amount as depositamount, " +
                  "a.amount as withdrawamount, CONCAT(a.realname, '/', a.cardbank, '/', a.cardno ) as withdrawdetails FROM " +
                  "( " +
                    "SELECT u.walletType, u.merchantAccount,j.reviewperson, j.orderid as wd_orderid, c.fixWithdrawP, " +
                    "c.referenceguid as wd_refguid, c.paytime as paytime, c.requesttime as withdrawrequesttime, j.account as account, c.conformedStatus, " +
                    "c.realname, c.cardbank, c.cardno, c.amount, j.id, j.createtime, j.status, j.withdrawid, j.orderid, j.reviewtime, c.account as withdrawaccount  " +
                    "FROM ofcrosstransact j WITH(NOLOCK)  " +
                    "LEFT JOIN ofUser u WITH(NOLOCK) on u.account = j.account " +
                    "LEFT JOIN ofWithdrawal c WITH(NOLOCK) on withdrawid = c.id  " +
                    "/**where**/ ORDER BY CASE WHEN j.status = 0 THEN 0 ELSE 1 END asc, j.id desc OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY ) a  " +
                    "outer apply(select account, amount, imgupload,ipaddress,status from ofdeposit od with(nolock) where od.merchantno = wd_orderid ) b  " +
                  "outer apply (select TOP 1 createtime from ofBonuspromo bb WITH(NOLOCK) WHERE bb.referenceguid = wd_refguid ) bp " +
                  "CROSS APPLY(SELECT COUNT(*) AS requestcount FROM ofcrosstransact WITH(NOLOCK) WHERE createtime > @TODAY AND account = a.account ) e ORDER BY " +
                  "CASE WHEN a.status = 0 THEN 0 ELSE 1 END asc "
                );

                totalItemsTemplate = totalItemsBuilder.AddTemplate("SELECT COUNT(1) as TotalItems " +
                    "FROM ofcrosstransact j WITH(NOLOCK)  " +
                    "LEFT JOIN ofUser u WITH(NOLOCK) on u.account = j.account " +
                    "LEFT JOIN ofWithdrawal c WITH(NOLOCK) on withdrawid = c.id  " +
                    "/**where**/");

            }

            //var template = builder.AddTemplate("" +
            //    "SELECT c.conformedstatus,e.requestcount as requestcount,a.id as id, a.createtime as createtime,a.status as status,withdrawid,a.orderid as orderid," +
            //        "a.reviewtime as reviewtime,b.account as depositaccount, c.account as withdrawaccount, " +
            //        "b.imgupload as imgupload, b.amount as depositamount, c.amount as withdrawamount,CONCAT(c.realname,'/',c.cardbank,'/',c.cardno) as withdrawdetails " +
            //    "FROM (SELECT * from ofcrosstransact order by " +
            //    "CASE WHEN status=0 THEN 0 ELSE 1 END asc, " +
            //    "id desc OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY) a " +
            //    "LEFT JOIN ofWithdrawal c on a.withdrawid = c.id " +
            //    "LEFT JOIN ofDeposit b on b.merchantno = a.orderid " +
            //    "CROSS APPLY(select count(*) AS requestcount FROM ofcrosstransact WITH(NOLOCK) WHERE createtime > @TODAY AND account=a.account)e " +
            //    "ORDER BY " +
            //    "CASE WHEN a.status=0 THEN 0 ELSE 1 END asc ");
            // PAGING FUNCTION
            builder.Where("c.wid NOT IN (4,5)");
            totalItemsBuilder.Where("c.wid NOT IN (4,5)");
            //totalItemsBuilder.Whjere("c.wid = 4");
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;

            if (request.currencycode > 0) { 
                builder.Where("u.currencyType = @currencycode", new { request.currencycode });
                totalItemsBuilder.Where("u.currencyType = @currencycode", new { request.currencycode }); 
            }

            if (!string.IsNullOrEmpty(request.account))
            {
                builder.Where("j.account = @account", new { account = request.account });
                totalItemsBuilder.Where("j.account = @account", new { account = request.account });
            }

            if (!string.IsNullOrEmpty(request.withdrawaccount))
            {
                builder.Where("c.account = @withdrawaccount", new { withdrawaccount = request.withdrawaccount });
                totalItemsBuilder.Where("c.account = @withdrawaccount", new { withdrawaccount = request.withdrawaccount });
            }

            if (request.starttime != null)
            {
                builder.Where("j.createtime > @starttime", new { starttime = request.starttime });
                totalItemsBuilder.Where("j.createtime > @starttime", new { starttime = request.starttime });
            }

            if (!string.IsNullOrEmpty(request.merchantAccount) && request.merchantAccount != "-1") { 
                builder.Where("u.merchantAccount = @merchantAccount", new { request.merchantAccount });
                totalItemsBuilder.Where("u.merchantAccount = @merchantAccount", new { request.merchantAccount }); 
            }

            if (request.walletType >= 0) { 
                builder.Where("u.walletType = @walletType", new { request.walletType });
                totalItemsBuilder.Where("u.walletType = @walletType", new { request.walletType }); 
            }

            //builder.Where("requesttime >= @starttime AND requesttime  <= @endtime", new { request.starttime, request.endtime });

            builder.AddParameters(new { Offset = offset, Next = request.PageSize, TODAY = Common.CurrentTime.ToString("yyyy-MM-dd") });
            //builder.OrderBy("id DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {

                var rows = connection.Query<CrossTransactListReturn>(template.RawSql, template.Parameters);
                var totalpagerows = connection.Query<CrossTransactListReturn>(totalItemsTemplate.RawSql, totalItemsTemplate.Parameters);

                var totalItems = rows.Count() == 0 ? 0 : totalpagerows.First().TotalItems;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;

                rows.ToList().ForEach(x =>
                {
                    if ((Common.CurrentTime - x.createtime).TotalHours > 9)
                    {
                        var y = x.withdrawdetails.Split('/');
                        var wd_cardname = string.IsNullOrEmpty(y[0]) ? "" : y[0].Substring(0, 1) + "**";
                        x.withdrawdetails = string.Concat(wd_cardname, "/", y[1], "/", Common.Security.ObscureString(y[2]));
                    }
                    if (x.status == 0)
                    {
                        x.minutesdiff = (Common.CurrentTime - x.createtime).TotalMinutes;
                        x.minutesdiff = Math.Floor(x.minutesdiff);
                    }
                    else if (x.status != 0)
                    {
                        DateTime temp_1 = (DateTime)x.reviewtime;
                        x.minutesdiff = (temp_1 - x.createtime).TotalMinutes;
                        x.minutesdiff = Math.Floor(x.minutesdiff);
                    }
                    else
                        x.minutesdiff = -1;

                    if (x.conformedtime != null && x.conformedStatus == 1)
                    {
                        DateTime temp_1 = (DateTime)x.withdrawrequesttime;
                        DateTime temp_2 = (DateTime)x.conformedtime;
                        x.conformedtimediff = (temp_2 - temp_1).TotalMinutes;
                        x.conformedtimediff = Math.Floor(x.conformedtimediff);
                    }

                });


                var result = new ResponseModel<PagingModel<CrossTransactListReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<CrossTransactListReturn>()
                    {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}

