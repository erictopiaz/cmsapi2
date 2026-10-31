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
    public class ThirdpayEdit : IRequest<ThirdpayEditReturn>
    {
        public long id { get; set; }
        public string merchantid { get; set; }
        public string merchantkey { get; set; }
        public decimal paymentcap { get; set; }
        public string requestaddress { get; set; }
        public string returnaddress { get; set; }
        public int status { get; set; }
        public int? sort { get; set; }
        public int? appstatus { get; set; }
        public int? minamount { get; set; }
        public int? maxamount { get; set; }
        public bool deductfee { get; set; }
        public bool noduplicateamount { get; set; }
        public bool newusers{ get; set; }
        public string method { get; set; }
        public string levels { get; set; }
        public string banks { get; set; }
        public string callbackip { get; set; }
        public string fastpay { get; set; }
        public string fastdeposit { set; get; }
        public double self_feecent { get; set; }
        public double feecent { get; set; }
        public double rebate { get; set; }
        public int? alteramount { get; set; }
        public int predeductcount { get; set; }
        public long mindepositcount { get; set; }
        public int mindepositcountmerchant { get; set; }
        public string mustcomplymerchantid { get; set; }
        public bool requirenote { get; set; }
        public bool requirebank { get; set; }
        public bool requirename { get; set; }
        public bool requirecardno { get; set; }
        public bool appnewwindow { get; set; }
        public bool mustterminate { get; set; }
        public bool mustnotbedivisiblebyzero { get; set; }
        public bool allowedamountinput { get; set; }
        public string merchantids { get; set; }
        public int merchantlevel { get; set; }
        public int dailyrequestlimit { get; set; }
        public bool registeredname { get; set; }
        public string merchantAccount { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class ThirdpayEditReturn : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class ThirdpayEditValidator : AbstractValidator<ThirdpayEdit>
    {
        public ThirdpayEditValidator()
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
    public class ThirdpayEditHandler : BaseRequestHandler<ThirdpayEdit, ThirdpayEditReturn>
    {
        public ThirdpayEditHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ThirdpayEditReturn Handle(ThirdpayEdit request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                // Validate merAccount
                var merAccount = connection.Query<string>("SELECT merAccount from ofMerchantSettings WITH(NOLOCK)");
                var postedMerAccount = request.merchantAccount.Split("#").ToList();
                bool errorValidation = false;
                postedMerAccount.ForEach(x =>
                {
                    if (!merAccount.Contains(x))
                        errorValidation = true;
                });
                if(errorValidation)
                    return new ThirdpayEditReturn()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "品牌錯誤"
                    };

                var bankbaseList = connection.GetAll<ofBankBase>();

                var id = request.id;
                var row_justtogetclass = connection.Get<ofThirdpay>(id);
                var row = connection.Get<ofThirdpay_WithoutClass>(id);

                //row.merchantid = request.merchantid;
                //row.merchantkey = request.merchantkey;
                row.paymentcap = request.paymentcap;
                row.fastpay = request.fastpay;
                row.fastdeposit = request.fastdeposit;
                //row.requestaddress = request.requestaddress;
                //row.returnaddress = request.returnaddress;
                //row.status = request.status;
                row.sort = request.sort;
                //row.appstatus = request.appstatus;
                row.minamount = request.minamount;
                row.maxamount = request.maxamount;
                row.t_merchantAccounts = request.merchantAccount;
                //row.method = request.method;




                // translating levelname
                if (request.levels.Length > 0)
                {
                    var levels = request.levels.Split(',');
                    var levelname = new List<string>();
                    foreach (var item in levels)
                        levelname.Add(CMS.Core.CachedData.LevelNameDict.FirstOrDefault(x => x.Value == item).Key.ToString());
                    row.levels = string.Join(",", levelname);
                }

                //request.merchantlevel = string.IsNullOrEmpty(request.merchantids) ? 0 : 1;
                row.merchantids = request.merchantids;
                row.merchantlevel = request.merchantlevel; 

                var miscDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(row.misc);

                var jsonObj = new JObject()
                {
                    ["feecent"] = request.feecent,
                    ["mindepositcount"] = request.mindepositcount,
                    ["alteramount"] = request.alteramount,
                    ["rebate"] = request.rebate,
                    ["requirenote"] = request.requirenote ? 1 : 0,
                    ["requirebank"] = request.requirebank ? 1 : 0,
                    ["requirename"] = request.requirename ? 1 : 0,
                    ["requirecardno"] = request.requirecardno ? 1 : 0,
                    ["mustterminate"] = request.mustterminate ? 1 : 0,
                    ["mustnotbedivisiblebyzero"] = request.mustnotbedivisiblebyzero ? 1 : 0,
                    ["allowedamountinput"] = request.allowedamountinput ? 1 : 0,
                    ["registeredname"] = request.registeredname ? 1 : 0,
                    ["deductfee"] = request.deductfee ? 1 : 0,
                    ["appnewwindow"] = request.appnewwindow ? 1 : 0,
                    ["noduplicateamount"] = request.noduplicateamount ? 1 : 0,
                    ["newusers"] = request.newusers ? 1 : 0,
                    ["mustcomplymerchantid"] = request.mustcomplymerchantid,
                    ["mindepositcountmerchant"] = request.mindepositcountmerchant,
                    ["self_feecent"] = request.self_feecent,
                    ["predeductcount"] = request.predeductcount,
                    ["dailyrequestlimit"] = request.dailyrequestlimit
                };

                if (miscDict.ContainsKey("defaultbank"))
                    jsonObj.Add("defaultbank", miscDict["defaultbank"]);


                if (miscDict.ContainsKey("proxy"))
                    jsonObj.Add("proxy", miscDict["proxy"]);

                if (miscDict.ContainsKey("altersubmittedamount"))
                    jsonObj.Add("altersubmittedamount", miscDict["altersubmittedamount"]);

                string misc = jsonObj.ToString(Formatting.None);
                row.misc = misc;





                var banklists = connection.Get<ofSettings>("PAYMENT_THIRDPAYBANK");
                var banklistsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(banklists.body);

                if (!banklistsDict.ContainsKey(id.ToString())) 
                    banklistsDict.Add(request.id.ToString(), "");


                request.banks = request.banks == "," ? "" : request.banks;
                if (!string.IsNullOrEmpty(request.banks))
                {
                    JArray banklistArry = new JArray();
                    foreach (var choseitem in request.banks.Split(','))
                    {

                        foreach (var bankbaseitem in bankbaseList)
                        {
                            if (choseitem == bankbaseitem.name)
                            /*
[{"id":"100","name":"USDT-ERC20","icon":"https://www.cdnimg9.com/img/{0}/bank/bk_100.png"}]
                             */
                            {
                                string icon = bankbaseitem.url.ToString();
                                icon = string.Format(icon, "####", bankbaseitem.icon);
                                icon = icon.Replace("####", "{0}");
                                banklistArry.Add(new JObject() {
                                    ["id"] = bankbaseitem.bid.ToString(),
                                    ["name"] = bankbaseitem.name.ToString(),
                                    ["icon"] = icon,
                                });
                            }
                        }
                    }
                    row.banklist = banklistArry.ToString(Formatting.None);

                    string tosavebanklist = request.banks.Replace(",", "|");
                    banklistsDict[request.id.ToString()] = tosavebanklist;
                    banklists.body = Newtonsoft.Json.JsonConvert.SerializeObject(banklistsDict);
                    connection.Update(banklists);
                }
                else {
                    row.banklist = "";
                    banklistsDict[request.id.ToString()] = "";
                    banklists.body = Newtonsoft.Json.JsonConvert.SerializeObject(banklistsDict);
                    connection.Update(banklists);
                }

                var ofsettingsRow = connection.Get<ofSettings>("PAYMENT_CROSSTRANSACT");
                var ofsettingsRow_body = JObject.Parse(ofsettingsRow.body);
                ofsettingsRow_body["withdraw_amount"] = request.fastdeposit;
                ofsettingsRow.body = ofsettingsRow_body.ToString(Formatting.None);

                var payTypeRow = connection.Get<ofPayType>(row_justtogetclass._class);
                payTypeRow.callbackip = request.callbackip;
                var ofThirdpay_updateresult = connection.Update(row);
                var ofPaytype_updateresult = connection.Update(payTypeRow);
                var ofSettings_updateresult = connection.Update(ofsettingsRow);

                return new ThirdpayEditReturn()
                {
                    code = ofThirdpay_updateresult && ofPaytype_updateresult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.FAIL,
                    message = ""
                };
            }
        }
    }
}

