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
using CMS.Core.ViewModel;

namespace CMS.Core.Application.Account.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetOutboundRecordQuery : PagingModel, IRequest<IEnumerable<GetOutboundRecordReturn>> 
    {
        public string Account { get; set; }
        public string Realname { get; set; }
        public string Affiliate { get; set; }
        public string Affiliate_UpperLevel { get; set; }
        public int Status { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class GetOutboundRecordReturn
    {
        public string account { get; set; }
        public string realname { get; set; }
        public int sex { get; set; }
        public string birthday { get; set; }
        public int status { get; set; }
        public int level { get; set; }
        public DateTime createtime { get; set; }
        public string referrerreg { get; set; }
        public decimal amountactive { get; set; }
        public decimal amountsuspend { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public int depositnumber { get; set; }
        public decimal depositamount { get; set; }
        public int withdrawalnumber { get; set; }
        public decimal withdrawalamount { get; set; }
        public string affaccount { get; set; }
        public decimal bonuspoints { get; set; }
        public string marcode { get; set; }
        public string withdrawcards { get; set; }
    }


    /// <summary>
    /// API Model Validation
    /// </summary>
    public class GetOutboundRecordValidator : AbstractValidator<GetOutboundRecordQuery>
    {
        public GetOutboundRecordValidator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.Affiliate).Affiliate();
            RuleFor(x => x.Affiliate_UpperLevel).Affiliate();
            RuleFor(x => x.Status).Status();
        }
    }


    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetOutboundRecordHandler : IRequestHandler<GetOutboundRecordQuery, IEnumerable<GetOutboundRecordReturn>>
    {
        private readonly CMS.Data.MainDB _context;
        private readonly IMapper _mapper;
        public GetOutboundRecordHandler(CMS.Data.MainDB context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<IEnumerable<GetOutboundRecordReturn>> Handle(GetOutboundRecordQuery request, CancellationToken cancellationToken)
        {

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT account,createtime,affaccount " +
                "FROM ofUser /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.Account)) builder.Where("Account = @Account", new { request.Account });
            if (!string.IsNullOrEmpty(request.Affiliate)) builder.Where("affaccount = @Affiliate", new { request.Affiliate });
            if (!string.IsNullOrEmpty(request.Realname)) builder.Where("Realname = @Realname", new { request.Realname });
            if (request.End != null) builder.Where("createtime <= @End", new { request.End });
            if (request.Start != null) builder.Where("createtime >= @Start", new { request.Start });

            // PAGING FUNCTION
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("createtime ASC");
            

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var accountRows = connection.Query<GetOutboundRecordReturn>(template.RawSql, template.Parameters);
                //var mappedResult1 = _mapper.Map<ofUser[], GetOutboundRecordReturn[]>(accountRows.ToArray());
                return Task.FromResult(accountRows.AsEnumerable());
            }


            //return Task.FromResult(new GetOutboundRecordReturn());
        }

    }
}

