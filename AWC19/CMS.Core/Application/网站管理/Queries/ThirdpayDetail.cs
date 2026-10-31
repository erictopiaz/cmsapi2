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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Extensions.Logging;
using CMS.Core;
using NSwag.Annotations;
using Dapper.Contrib.Extensions;
using ShareEntity.Models;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.网站管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class ThirdpayDetail : IRequest<ResponseModel<ThirdpayDetailReturn>>
    {
        public long Id { get; set; }
    }




    public class ThirdpayDetailReturn
    {
        public long id { get; set; }
        public string name { get; set; }
        //public string merchantid { get; set; }
        //public string merchantkey { get; set; }
        public decimal paymentcap { get; set; }
        public string requestaddress { get; set; }
        public string returnaddress { get; set; }
        public int status { get; set; }
        public int? sort { get; set; }
        public int? appstatus { get; set; }
        public int? minamount { get; set; }
        public int? maxamount { get; set; }
        public string method { get; set; }
        public string levels { get; set; }
        public double feecent { get; set; }
        public int deductfee { get; set; }
        public int noduplicateamount { get; set; }
        public int newusers { get; set; }
        public double rebate { get; set; }
        public long mindepositcount { get; set; }
        public long mindepositcountmerchant { get; set; }
        public long mustcomplymerchantid { get; set; }
        public long alteramount { get; set; }
        public string callbackip { get; set; }
        public string banks{ get; set; }
        public string fastpay { get; set; }
        public string banklist { get; set; }
        public int dailyrequestlimit { get; set; }
        public int queryWhereModel_status { get; set; }
        public int queryWhereModel_day { get; set; }
        public int? paycategory { get; set; }

        internal string misc { get; set; }
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayDetailValidator : AbstractValidator<ThirdpayDetail>
    {
        public ThirdpayDetailValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class ThirdpayDetailHandler : BaseRequestHandler<ThirdpayDetail, ResponseModel<ThirdpayDetailReturn>>
    {
        public ThirdpayDetailHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<ThirdpayDetailReturn> Handle(ThirdpayDetail request)
        {
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var id = request.Id;
                var row = connection.Get<ofThirdpay>(id);
                var mappedRow = _mapper.Map<ThirdpayDetailReturn>(row);
                var paytyperow = connection.Get<ofPayType>(row._class);
                var banklists = connection.Get<ofSettings>("PAYMENT_THIRDPAYBANK");
                var banklistsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(banklists.body);
                if (banklistsDict.ContainsKey(id.ToString()) && !string.IsNullOrEmpty(banklistsDict[id.ToString()]))
                {
                    mappedRow.banks = banklistsDict[id.ToString()].Replace("|", ",");
                }
                else
                    mappedRow.banks = "";

                mappedRow.callbackip = paytyperow.callbackip;

                // translating levelname
                if (mappedRow.levels.Length > 0) {
                    var levels = mappedRow.levels.Split(',');
                    var levelname = new List<string>();
                    foreach (var item in levels)
                        if (CMS.Core.CachedData.LevelNameDict.ContainsKey(int.Parse(item)))
                            levelname.Add(CMS.Core.CachedData.LevelNameDict[int.Parse(item)]);
                    mappedRow.levels = string.Join(",", levelname);
                }

                if (!string.IsNullOrEmpty(row.misc))
                {
                    var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.queryMerchantWhere);
                    //{ "days":10,"status":0}
                    var queryWhereModelDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(row.queryMerchantWhere);
                    double miscDictFeeCent = 0;
                    double miscDictRebate = 0;
                    int miscnewusers = 0;
                    int miscdeductfee = 0;
                    int miscnoduplicateamount = 0;

                    if (miscDict.ContainsKey("feecent")) {
                        miscDictFeeCent = miscDict["feecent"] > 0 ? miscDict["feecent"] : 0;
                    }
                    if (miscDict.ContainsKey("rebate"))
                        miscDictRebate = miscDict["rebate"] > 0 ? miscDict["rebate"] : 0;

                    if (miscDict.ContainsKey("deductfee"))
                        miscdeductfee = miscDict["deductfee"] > 0 ? miscDict["deductfee"] : 0;

                    if (miscDict.ContainsKey("newusers"))
                        miscnewusers = miscDict["newusers"] > 0 ? miscDict["newusers"] : 0;
                    
                    if (miscDict.ContainsKey("noduplicateamount"))
                        miscnoduplicateamount = miscDict["noduplicateamount"] > 0 ? miscDict["noduplicateamount"] : 0;

                    mappedRow.newusers = miscnewusers;
                    mappedRow.deductfee = miscdeductfee;
                    mappedRow.noduplicateamount = miscnoduplicateamount;
                    mappedRow.feecent = miscDictFeeCent;
                    mappedRow.rebate = miscDictRebate;
                    mappedRow.mindepositcount = miscDict.ContainsKey("mindepositcount") ? miscDict["mindepositcount"] : 0;
                    mappedRow.alteramount = miscDict.ContainsKey("alteramount") ? miscDict["alteramount"] : 0;
                    mappedRow.queryWhereModel_day = queryWhereModelDict.ContainsKey("days") ? queryWhereModelDict["days"] : 0;
                    mappedRow.queryWhereModel_status = queryWhereModelDict.ContainsKey("status") ? queryWhereModelDict["status"] : 0;
                    mappedRow.dailyrequestlimit = queryWhereModelDict.ContainsKey("dailyrequestlimit") ? queryWhereModelDict["dailyrequestlimit"] : 0;
                }




                return new ResponseModel<ThirdpayDetailReturn>()
                {
                    code = row != null ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = mappedRow
                };
            }

        }
    }
}

