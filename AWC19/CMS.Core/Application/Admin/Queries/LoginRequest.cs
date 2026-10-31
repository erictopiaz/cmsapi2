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
using System.ComponentModel;
using CMS.Core;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using LogLib.Helper;
using System.Net;
using ShareEntity.Models;

namespace CMS.Core.Application.Admin.Queries
{
    public class LoginRequestQuery : IRequest<LoginRequestReturn>
    {
        [Description("用户账号")]
        public string Account { get; set; }
        [Description("用户密码")]
        public string Password { get; set; }
        [JsonIgnore]
        public string ipaddress { get; set; }
        [JsonIgnore]
        public string fp { get; set; }

        public int gaCode { set; get; } = 0;
    }

    public class LoginRequestReturn
    {
        public int Status { get; set; }
        public string message { set; get; }
        public string access { get; set; }
        public string currencyTypes { get; set; }
        public int allowedToManageAgent { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequestQuery>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Account).CharAndDigital(3, 12);
            //RuleFor(x => x.Password).Password();
        }
    }


    public class LoginRequestHandler : IRequestHandler<LoginRequestQuery, LoginRequestReturn>
    {
        private readonly CMS.Data.MainDB _context;
        private readonly IMapper _mapper;
        public LoginRequestHandler(IMapper mapper)
        {
            //_context = context;
            _mapper = mapper;
        }

        public Task<LoginRequestReturn> Handle(LoginRequestQuery request, CancellationToken cancellationToken)
        {
            string hashedPassword = Common.Security.DoubleMD5Hash(request.Password);
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {

                #region ching add ip account 检测

                var queryMapIPSql = string.Format(@"select ipaddress from ofManageMapIP with(nolock)
where account=@account");

                var ips = connection.Query<string>(queryMapIPSql,
                    new
                    {
                        account = request.Account
                    });
                if (ips != null && ips.Contains(request.ipaddress))
                {
                    //白名单验证通过
                }
                else
                {
                    return Task.FromResult(new LoginRequestReturn()
                    {
                        Status = 0,
                        access = "",
                    });
                }

                #endregion


                var result = connection.QueryFirstOrDefault<ofManage>("" +
                    "SELECT * " +
                    "FROM ofManage " +
                    "WHERE account=@account AND password=@password",
                    new
                    {
                        account = request.Account,
                        password = hashedPassword
                    });

                string access = result ==null? null: result.access;

                if (result != null) {
                    if (result.status != 1) {
                        return Task.FromResult(new LoginRequestReturn()
                        {
                            Status = 0,
                            access = ""
                        });
                    }
                    var row = connection.Get<ofManage>(request.Account);
                    row.currentloginip = request.ipaddress;
                    row.currentlogintime = Common.CurrentTime;
                    row.online = 1;
                    row.loginnumber = row.loginnumber + 1;
                    row.currentloginaddress = request.ipaddress;
                    connection.Update<ofManage>(row);
                }

                System.Net.IPAddress ipaddress;
                if (!System.Net.IPAddress.TryParse(request.ipaddress, out ipaddress))
                    request.ipaddress = "0.0.0.0";


                //var oflog = new oflo
                var insert = new SqlCommand("" +
                    "INSERT INTO ofLog(account,ip,ipbin,event,comment,createtime) " +
                    "VALUES(@ACCOUNT,@IP,@IPBIN,@EVENT,@COMMENT,@CREATETIME)");
                //insert.Parameters.AddWithValue("@ACCOUNT",request.Account);
                //    insert.Parameters.AddWithValue("@IP",request.ipaddress);
                //insert.Parameters.AddWithValue("@IPBIN", System.Net.IPAddress.Parse(request.ipaddress).GetAddressBytes());
                //insert.Parameters.AddWithValue("@EVENT","1000");
                //insert.Parameters.AddWithValue("@COMMENT",request.fp ?? "");
                //insert.Parameters.AddWithValue("@CREATETIME",Common.CurrentTime);

                connection.Execute(insert.CommandText, new {
                    ACCOUNT = request.Account,
                    IPBIN = System.Net.IPAddress.Parse(request.ipaddress).GetAddressBytes(),
                    IP = request.ipaddress,
                    EVENT = 1000,
                    COMMENT = request.fp ?? "",
                    CREATETIME = Common.CurrentTime
                });

                return Task.FromResult(new LoginRequestReturn() {
                    Status = result == null ? 0 : 1,
                    access = access,
                    currencyTypes = result.currencyTypes,
                    allowedToManageAgent = result.allowedToManageAgent
                });
            }
        }



    }
}
