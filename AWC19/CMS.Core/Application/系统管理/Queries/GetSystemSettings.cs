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

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetSystemSettings : IRequest<ResponseModel<GetSystemSettingsReturn>> { }

    public class GetSystemSettingsReturn {
        public List<Tuple<int, string>> walletType { get; set; }
        public List<Tuple<int, string>> merchantAccount { get; set; }

    }


    public class GetSystemSettingsHandler : CMS.Core.BaseRequestHandler<GetSystemSettings, ResponseModel<GetSystemSettingsReturn>>
    {
        public GetSystemSettingsHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<GetSystemSettingsReturn> Handle(GetSystemSettings request)
        {
            // 创建DAPPER TEMPLATE 
            //var builder = new SqlBuilder();

            //var template = builder.AddTemplate("" +
            //    "SELECT gamename,gamestatus,gameid " +
            //    "FROM ofGameClass WITH(NOLOCK) /**where**/ ");


            //// FILL UP THE WHERE CLAUSE
            ////builder.Where("class = 1");
            ////builder.Where("gamestatus = 1");
            //builder.Where("gameid NOT IN @EXCLUDENAME", new
            //{
            //    EXCLUDENAME = new List<int>() {
            //        136,142,2039,3039147,3044,2044,1024,1025,1028,130,100,2027,2028,3027,3028,2038,3038,141,3039,147,129
            //    }
            //});

            //using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            //{
            //    var result = connection.Query<GetSystemSettingsReturn>(template.RawSql, template.Parameters);


            //    var responseData = new ResponseModel<IEnumerable<GetSystemSettingsReturn>>()
            //    {
            //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
            //        message = "",
            //        Data = result
            //    };

            //    return responseData;
            //}
            var data = new GetSystemSettingsReturn()
            {
                walletType = new List<Tuple<int, string>>
                {
                    Tuple.Create(0, "单一钱包"),
                    Tuple.Create(1, "转账钱包")
                },
                merchantAccount = new List<Tuple<int, string>>
                {
                    Tuple.Create(0, "Bayo"),
                    Tuple.Create(1, "Awc"),
                    Tuple.Create(2, "Boss")
                }
                };

            //return data;
            var responseData = new ResponseModel<GetSystemSettingsReturn>()
            {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "",
                Data = data
            };
            return responseData;

        }

    }
}

