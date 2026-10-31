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
    public class NetBankEdit : IRequest<NetBankEditReturn>
    {
        public long id { get; set; }
        //public string name { get; set; }
        //public string bank { get; set; }
        //public string realname { get; set; }
        //public string cardno { get; set; }
        //public string cardaddress { get; set; }
        public decimal feecent { get; set; }
        public decimal feemax { get; set; }
        public decimal paymentcap { get; set; }
        public decimal singlemaxdepositamount { get; set; }
        public decimal singlemindepositamount { get; set; }
        public int mindepositcount { get; set; }
        //public int? _class { get; set; }
        //public DateTime? roundtime { get; set; }
        public string range { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class NetBankEditReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class NetBankEditValidator : AbstractValidator<NetBankEdit>
    {
        public NetBankEditValidator()
        {
            //TODO: Validation populate
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class NetBankEditHandler : BaseRequestHandler<NetBankEdit, NetBankEditReturn>
    {
        public NetBankEditHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override NetBankEditReturn Handle(NetBankEdit request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var id = request.id;
                var row = connection.Get<ofNetbank_Edit>(id);

                //row.name = request.name;
                //row.bank = request.bank;
                //row.realname = request.realname;
                //row.cardno = request.cardno;
                //row.cardaddress = request.cardaddress;
                row.feecent = request.feecent;
                row.feemax = request.feemax;
                row.paymentcap = request.paymentcap;
                //row.roundtime = request.roundtime;

                // translating levelname
                if (request.range.Length > 0)
                {
                    var levels = request.range.Split(',');
                    var levelname = new List<string>();
                    foreach (var item in levels)
                        levelname.Add(CMS.Core.CachedData.LevelNameDict.FirstOrDefault(x => x.Value == item).Key.ToString());
                    row.range = string.Join(",", levelname);
                }



                var jsonObj = new JObject()
                {
                    ["singlemindepositamount"] = request.singlemindepositamount,
                    ["singlemaxdepositamount"] = request.singlemaxdepositamount,
                    ["mindepositcount"] = request.mindepositcount
                };
                string misc = jsonObj.ToString(Formatting.None);
                row.misc = misc;

                //row.range = Uri.UnescapeDataString(request.range);

                var updateResult = connection.Update(row);

                return new NetBankEditReturn()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

