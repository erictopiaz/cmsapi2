using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using ShareEntity.OutputModels;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetUsers : PagingModel, IRequest<ResponseModel<PagingModel<GetUsersReturn>>>
    {
        public string account { get; set; }
        public string belongs { get; set; }
        public int status { get; set; } = -1;
        public int online { get; set; } = -1;
        [JsonIgnore]
        public string Admin { get; set; }

    }

    public class GetUsersReturn
    {
        public long id { get; set; }
        public string account { get; set; }
        public string belongs { get; set; }
        public DateTime createtime { get; set; }
        public int status { get; set; }
        public int online { get; set; }
        public int forcequit { get; set; }
        public int loginnumber { get; set; }
        public DateTime? currentlogintime { get; set; }
        public string currentloginip { get; set; }
        public string currentloginaddress { get; set; }
        public DateTime? lastactivetime { get; set; }
        public string remarks { get; set; }
        public int? failednumber { get; set; }
        public string lastloginoutip { get; set; }
        public DateTime? lastloginouttime { get; set; }
        internal long TotalItems { get; set; }
        public string currencyTypes { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class GetUsersValidator : AbstractValidator<GetUsers>
    {
        public GetUsersValidator()
        {

            RuleFor(x => x.account).Account();
            RuleFor(x => x.belongs).CharAndDigitalNullable(1,10);
            RuleFor(x => x.status).Status();
            RuleFor(x => x.online).Status();
            RuleFor(x => x.PageSize).PageSize();
            RuleFor(x => x.Page).Page();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class GetUsersHandler : BaseRequestHandler<GetUsers, ResponseModel<PagingModel<GetUsersReturn>>>
    {
        public GetUsersHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }


        protected override ResponseModel<PagingModel<GetUsersReturn>> Handle(GetUsers request)
        {
            // Get Account Created based on the start Date

            // Get All Successful Deposit on that date onwards

            //TODO: Do This after everything else

            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT * , COUNT(*) OVER() as TotalItems " +
                "FROM ofManage WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY");

            // FILL UP THE WHERE CLAUSE
            if (!string.IsNullOrEmpty(request.belongs)) builder.Where("belongs = @belongs", new { request.belongs });
            if (!string.IsNullOrEmpty(request.account)) builder.Where("account LIKE @account", new { account = string.Format("%{0}%", request.account)  }) ;
            if (request.status >= 0) builder.Where("status = @STATUS", new { STATUS = request.status });
            if (request.online >= 0) builder.Where("online = @online", new { request.online });

            #region 市场人员如果有审核过代理，只显示自身

            var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
            var result = DapperBase.Query<RightModel>(accessSql, new { account = request.Admin }, commandType: CommandType.Text,
                        commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (result != null && result.Count == 1
                && result.First().access.IndexOf("/api/Admin/QueryCMSIndexAgentReport", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var sql = string.Format(@"select id from ofAffiliate where verifyperson='{0}'", request.Admin);
                if (DapperBase.ExecuteScalar(sql, param: null, commandTimeout: 15000,
                    ConnectionString: DapperStringBase.ConnectionString))//有审核过代理
                {
                    builder.Where("account = @Admin", new { request.Admin });
                }
            }

            #endregion


            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new
            {
                Offset = offset,
                Next = request.PageSize
            });
            builder.OrderBy("currentlogintime DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<GetUsersReturn>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                // Apply the LevelName 
                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(x.currentloginip))
                        {
                            x.currentloginaddress = x.currentloginip = "No IP Address";
                            return;
                        }
                        if (CachedData.IPTranslation.ContainsKey(x.currentloginip))
                            x.currentloginaddress = CachedData.IPTranslation[x.currentloginip];
                    });


                return new ResponseModel<PagingModel<GetUsersReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<GetUsersReturn>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }
        }





    }
}
