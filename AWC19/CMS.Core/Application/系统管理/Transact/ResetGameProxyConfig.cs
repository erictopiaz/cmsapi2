using AutoMapper;
using AWC2019.Core.Validation;
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
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact {
   
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ResetGameProxyConfig : IRequest<ResponseModel> {
        public string proxyUrl { get; set; }
        public string gameid { get; set; }


        [JsonIgnore]
        public string Admin { get; set; }

    }


    public class ResetGameProxyConfigValidator : AbstractValidator<ResetGameProxyConfig> {
        public ResetGameProxyConfigValidator() {
            RuleFor(x => x.gameid).CharAndDigital(2, 5);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ResetGameProxyConfigHandler : BaseRequestHandler<ResetGameProxyConfig, ResponseModel> {
        public ResetGameProxyConfigHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(ResetGameProxyConfig request) {

            return new ResponseModel()
            {
                code = ShareEntity.Global.ResponseCode.code.FAIL,
                message = "功能已经关闭"
            };

            var querySql = string.Format(@"select gameid, params param,createtime from ofGameClass with(nolock)
where gameid={0}", request.gameid);
            var updateSql = string.Format(@"update ofGameClass set params=@param where gameid=@gameid");
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING)) {

                var rows = connection.Query<ofGameClassPorxyUpdate>(querySql);
                if (rows == null || rows.Count() <= 0) {
                    return new ResponseModel() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "错误的平台代码"
                    };
                }

                var row = rows.First();
                if (!string.IsNullOrEmpty(row.param)) {
                    var parsms = JsonConvert.DeserializeObject<dynamic>(row.param);
                    parsms.proxy = request.proxyUrl.Replace("'", "''");
                    row.param = JsonConvert.SerializeObject(parsms);
                }
                else {
                    row.param = JsonConvert.SerializeObject(new {proxy= request.proxyUrl.Replace("'", "''") });
                }
                var affected = connection.Execute(updateSql,row);
                return new ResponseModel() {
                    code = affected>0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = affected > 0 ? "success" : "failed"
                };
            }
        }


    }

}
