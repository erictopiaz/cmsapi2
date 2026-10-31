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
using CMS.Core.Application;
using ShareEntity.OutputModels;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class UpdateUserDetails : IRequest<ResponseModel>
    {
        public string Account { get; set; }
        public string belongs { get; set; }
        public string[] currencies { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }

    }


    public class UpdateUserDetailsValidator : AbstractValidator<UpdateUserDetails>
    {
        public UpdateUserDetailsValidator()
        {
            RuleFor(x => x.Account).Account();
            RuleFor(x => x.belongs).CharAndDigitalNullable(1,10);
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class UpdateUserDetailsHandler : BaseRequestHandler<UpdateUserDetails, ResponseModel>
    {
        public UpdateUserDetailsHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(UpdateUserDetails request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))

            {
                var row = connection.Get<ofManage>(request.Account);
                if(!string.IsNullOrEmpty(request.belongs))
                    row.belongs= request.belongs.ToUpper();

                if (request.currencies !=null && request.currencies.Length > 0)
                    row.currencyTypes = string.Format("[{0}]", string.Join(',', request.currencies));

                var isSuccess = connection.Update(row);

                return new ResponseModel()
                {
                    code = isSuccess ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = isSuccess ? "更新成功" : "更新失败"
                };
            }
        }


    }
}

