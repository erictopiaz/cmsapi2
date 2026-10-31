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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class NetBankAdd : IRequest<NetBankAddReturn>
    {
        public string name { get; set; }
        public string bank { get; set; }
        public string realname { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public decimal feecent { get; set; }
        public decimal feemax { get; set; }
        public decimal singlemaxdepositamount { get; set; }
        public decimal singlemindepositamount { get; set; }
        public decimal mindepositcount { get; set; }
        public decimal paymentcap { get; set; }
        public int? classtype { get; set; }
        public string range { get; set; }
        public string password { get; set; }
        public string misc { get; set; }
        [JsonIgnore]
        public string Admin { get; set; } = null;
    }

    public class NetBankAddReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class NetBankAddValidator : AbstractValidator<NetBankAdd>
    {
        public NetBankAddValidator()
        {
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class NetBankAddHandler : BaseRequestHandler<NetBankAdd, NetBankAddReturn>
    {
        public NetBankAddHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override NetBankAddReturn Handle(NetBankAdd request)
        {
            if (request.password != "m3m@") {
                return new NetBankAddReturn()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "密码错误"
                };
            }

            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                //var builder = new SqlBuilder();
                //var template = builder.AddTemplate("" +
                //    "SELECT *,class as classtype, COUNT(*) OVER() as TotalItems " +
                //    "FROM ofNetbank /**where**/ /**orderby**/ " +
                //    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                //    "FETCH NEXT @Next ROWS ONLY");

                //x.classname = CachedData.NetBankTypeDict[x.classtype];
                if (request.range.Length > 0)
                {
                    var levels = request.range.Split(',');
                    var levelname = new List<string>();
                    foreach (var item in levels)
                        levelname.Add(CMS.Core.CachedData.LevelNameDict.FirstOrDefault(x => x.Value == item).Key.ToString());
                    request.range = string.Join(",", levelname);
                }



                var jsonObj = new JObject()
                {
                    ["singlemindepositamount"] = request.singlemindepositamount,
                    ["singlemaxdepositamount"] = request.singlemaxdepositamount,
                    ["mindepositcount"] = request.mindepositcount
                };
                string misc = jsonObj.ToString(Formatting.None);

                var x = connection.Insert(new ofNetbank_Edit()
                {
                    appstatus = 0,
                    bank = request.bank,
                    cardaddress = request.cardaddress,
                    cardno = request.cardno,
                    createperson = request.Admin,
                    createtime = Core.Common.CurrentTime,
                    currency = "CNY",
                    feecent = request.feecent,
                    feemax = request.feemax,
                    name = request.name,
                    paymentcap = request.paymentcap,
                    status = 0,
                    range = request.range,
                    realname = request.realname,
                    misc = misc
                    //classtype = request.classtype,
                    //roundtime = Core.Common.CurrentTime
                });



                var updateCMD = new SqlCommand();
                updateCMD.Parameters.AddWithValue("@CLASS", request.classtype);
                updateCMD.Parameters.AddWithValue("@ID", x);
                connection.Execute("UPDATE ofNetbank SET class = @CLASSTYPE WHERE id=@ID", new { CLASSTYPE = request.classtype, ID = x });

                _logger.LogInformation("NetBankAdd - id:{0}", x.ToString());
                return new NetBankAddReturn()
                {
                    code = x > 0 ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

