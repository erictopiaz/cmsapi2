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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 邮件发送列表 : PagingModel, IRequest<ResponseModel<PagingModel<邮件发送列表Return>>>
    {
        public string account { get; set; }
        public int status { get; set; }
        public DateTime createtime { get; set; }
    }

    public class 邮件发送列表Return
    {
        public long id { get; set; }
        public string account { get; set; }
        public DateTime createtime { get; set; }
        public DateTime? sendtime { get; set; }
        public int status { get; set; }
        public string template { get; set; }
        public DateTime? canceltime { get; set; }
        public string remark { get; set; }
        public string templatedata { get; set; }
        internal long TotalItems { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 邮件发送列表Validator : AbstractValidator<邮件发送列表>
    {
        public 邮件发送列表Validator()
        {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 邮件发送列表Handler : BaseRequestHandler<邮件发送列表, ResponseModel<PagingModel<邮件发送列表Return>>>
    {
        public 邮件发送列表Handler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<邮件发送列表Return>> Handle(邮件发送列表 request)
        {
            // Get Account Created based on the start Date

            // Get All Successful Deposit on that date onwards

            //TODO: Do This after everything else

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofMail WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Concat("%", request.account, "%") });

            //if (request.startdate == null || ((DateTime)request.startdate).Year < 2008)
            //{
            //    request.startdate = Common.CurrentTime.AddDays(-90);
            //    request.enddate = Common.CurrentTime;
            //}
            //if (request.enddate == null)
            //    request.enddate = Common.CurrentTime;

            //builder.Where("createtime BETWEEN @STARTDATE AND @ENDDATE", new { STARTDATE=request.startdate, ENDDATE=request.enddate});
            //builder.Where("depositnumber > 0");


            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("id DESC");


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<邮件发送列表Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;


                return new ResponseModel<PagingModel<邮件发送列表Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<邮件发送列表Return>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }
    }
}
