using AutoMapper;
using CMS.Core.Application.账号管理;
using Dapper;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using ShareEntity.OutputModels;
using AWC2019.Core.Validation;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;
using CMS.Data.Models;
using MongoDB.Driver;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact {
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 黑白名单更新 : IRequest<黑白名单更新Return> {
        
        [MinLength(4)]
        [MaxLength(16)]
        public string account { get; set; }

        [MaxLength(66)]
        public string ip { get; set; }

        [MaxLength(60)]
        public string fg { get; set; }

        public int ip_status { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }

        /// <summary>
        /// 0:上锁，1：解锁
        /// </summary>
        public int type { get; set; } 

    }

    public class 黑白名单更新Return : ResponseModel {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 黑白名单更新Validator : AbstractValidator<黑白名单更新> {
        public 黑白名单更新Validator() {
            RuleFor(x => x.account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 黑白名单更新Handler : BaseAccount<黑白名单更新, 黑白名单更新Return> {
        public 黑白名单更新Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) {
        }


        protected override 黑白名单更新Return Handle(黑白名单更新 request) {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {

                if(request.type == 0) {
                    var rows = connection.Insert<ofIPBlackList>(new ofIPBlackList() {
                        account = request.account.Replace("'", "''"),
                        blackip = request.ip.Replace("'", "''"),
                        fg = string.IsNullOrEmpty(request.fg) ? "" : request.fg.Replace("'", "''"),
                        ip_status = 1,
                        limit_type = 0
                    });

                    return new 黑白名单更新Return() {
                        code = rows > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                        message = rows > 0 ? "锁定IP-成功" : "锁定IP-失败"
                    };
                }
                else {

                    var udpateSql = string.Format(@"update ofIPBlackList
set ip_status=0
where account=@account and blackip =@ip and limit_type=0");
                    var upResult = connection.Execute(udpateSql,
                        new { account = request.account.Replace("'", "''"), ip = request.ip.Replace("'", "''") });

                    return new 黑白名单更新Return() {
                        code = upResult > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                        message = upResult > 0 ? "解锁IP-成功" : "解锁IP-失败"
                    };
                }
                

            }

        }
    }
}
