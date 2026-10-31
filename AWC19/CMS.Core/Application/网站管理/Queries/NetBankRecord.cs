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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using CMS.Core;
using CMS.Core.ViewModel;
using Dapper.Contrib.Extensions;
using System.ComponentModel;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class NetBankRecord : PagingModel, IRequest<ResponseModel<PagingModel<NetBankRecordReturn>>>
    {
        public string name { get; set; }
        public string bank { get; set; }
        public string realname { get; set; }
        public string cardno { get; set; }
        public int status { get; set; } = -1;
        public int appstatus { get; set; } = -1;
        public int? _class { get; set; }
        public string range { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }

    public class NetBankRecordReturn
    {
        public long id { get; set; }
        public string name { get; set; }
        public string bank { get; set; }
        public string realname { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public DateTime createtime { get; set; }
        public decimal feecent { get; set; }
        public decimal feemax { get; set; }
        public decimal paymentcap { get; set; }
        public int status { get; set; }
        public int appstatus { get; set; }
        public decimal singlemaxdepositamount { get; set; }
        public decimal singlemindepositamount { get; set; }
        public float totalamountdeposit { get; set; }
        public float totaldepositpercent { get; set; }
        [Description("class")]
        public int classtype { get; set; }
        public string classname { get; set; }
        public DateTime? roundtime { get; set; }
        public string range { get; set; }
        public string misc { get; set; }
        public int mindepositcount { get; set; }
        public long TotalItems { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class NetBankRecordValidator : AbstractValidator<NetBankRecord>
    {
        public NetBankRecordValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class NetBankRecordHandler : BaseRequestHandler<NetBankRecord, ResponseModel<PagingModel<NetBankRecordReturn>>>
    {
        public NetBankRecordHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<NetBankRecordReturn>> Handle(NetBankRecord request)
        {

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT *,class as classtype, COUNT(*) OVER() as TotalItems " +
                "FROM ofNetbank WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            var depositBuilder = new SqlBuilder();
            var depositTemplate = depositBuilder.AddTemplate("" +
                "SELECT receiveperson AS name, SUM(amount) AS totalamountdeposit " +
                "FROM ofDeposit WITH(NOLOCK)  " +
                "/**where**/ GROUP BY receiveperson");
            if (request.starttime.Year>=2002)
                depositBuilder.Where("requesttime >= @STARTTIME AND reviewtime >= @STARTTIME AND reviewtime <= @ENDTIME",
                    new { STARTTIME = request.starttime, ENDTIME = request.endtime });
            else 
                depositBuilder.Where("requesttime >= @STARTTIME AND reviewtime >= @STARTTIME AND reviewtime <= @ENDTIME",
                    new { STARTTIME = Common.CurrentTime.ToString("yyyy-MM-dd"), ENDTIME = Common.CurrentTime.ToString("yyyy-MM-dd") + " 23:59:59" });
            depositBuilder.Where("deposittype > 0 AND status=1");
            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.name)) builder.Where("name LIKE @name", new { name = string.Concat("%", request.name, "%") });


            //if (!string.IsNullOrEmpty(request.name)) builder.Where("name = @name", new { request.name });
            if (!string.IsNullOrEmpty(request.bank)) builder.Where("bank = @bank", new { request.bank });
            if (!string.IsNullOrEmpty(request.realname)) builder.Where("realname = @realname", new { request.realname });
            if (!string.IsNullOrEmpty(request.cardno)) builder.Where("cardno = @cardno", new { request.cardno });
            if (request.status >= 0) builder.Where("status = @status", new { request.status });
            if (request.appstatus >= 0) builder.Where("appstatus  = @appstatus ", new { request.appstatus });
            if (request._class >= 0) builder.Where("class  = @classes ", new { classes = request._class });

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("CASE WHEN status =1 THEN '0' WHEN status = 0 THEN '1' ELSE STATUS END");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                var rows = connection.Query<NetBankRecordReturn>(template.RawSql, template.Parameters);
                var depositRows = connection.Query<NetBankRecordReturn>(depositTemplate.RawSql, depositTemplate.Parameters);
                var totalamountDict = depositRows.ToDictionary(x => x.name, x => x.totalamountdeposit);
                

                long totalItems = 0;
                if (rows.Count() > 0)
                {
                    totalItems = rows.First().TotalItems;
                    rows.ToList().ForEach(
                        x =>
                        {
                            if (totalamountDict.ContainsKey(x.realname))
                            {
                                x.totalamountdeposit = totalamountDict[x.realname];
                                x.totaldepositpercent = (x.totalamountdeposit / CachedData.MAX_DEPOSIT_PER_BANK) * 100;
                                x.totaldepositpercent = (float)Math.Floor(x.totaldepositpercent);
                            }

                            x.classname = CachedData.NetBankTypeDict[x.classtype];
                            if (!string.IsNullOrEmpty(x.range))
                            {
                                var levels = x.range.Split(',');
                                var levelname = new List<string>();
                                foreach (var item in levels)
                                {
                                    if (CMS.Core.CachedData.LevelNameDict.ContainsKey(int.Parse(item)))
                                        levelname.Add(CMS.Core.CachedData.LevelNameDict[int.Parse(item)]);
                                }
                                x.range = string.Join(",", levelname);
                            }

                            if (!string.IsNullOrEmpty(x.misc))
                            {
                                var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(x.misc);
                                x.mindepositcount = miscDict.ContainsKey("mindepositcount") ? (int)miscDict["mindepositcount"] : 0;
                                x.singlemaxdepositamount = miscDict.ContainsKey("singlemaxdepositamount") ? (decimal)miscDict["singlemaxdepositamount"] : 0;
                                x.singlemindepositamount = miscDict.ContainsKey("singlemindepositamount") ? (decimal)miscDict["singlemindepositamount"] : 0;
                            };

                        });
                }

                return new ResponseModel<PagingModel<NetBankRecordReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<NetBankRecordReturn>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }
    }
}

