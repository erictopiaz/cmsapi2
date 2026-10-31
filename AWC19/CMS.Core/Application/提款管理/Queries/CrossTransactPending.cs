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
using Microsoft.Extensions.Logging;
using ShareEntity.OutputModels;
using Newtonsoft.Json.Linq;
using Dapper.Contrib.Extensions;
using UtilsLib.lang;
using ShareEntity.Models;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class CrossTransactPendingQuery : BaseWithdrawQuery, IRequest<ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
    }

    //public class CrossTransactPendingReturn: BaseWithdrawReturn
    //{
    //}

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class CrossTransactPendingQueryValidator : AbstractValidator<CrossTransactPendingQuery>
    {
        public CrossTransactPendingQueryValidator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.cardno).CardNo();
            RuleFor(x => x.status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class CrossTransactPendingHandler : BaseWithdraw<CrossTransactPendingQuery, ResponseModel<PagingModel<BaseWithdrawReturn>>>
    {
        public CrossTransactPendingHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<BaseWithdrawReturn>> Handle(CrossTransactPendingQuery request)
        {

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                try
                {
                    // 创建DAPPER TEMPLATE 
                    // ofSettings Configuration
                    var cmd_ofsettings = new SqlCommand("SELECT body FROM ofSettings WITH(NOLOCK) WHERE code = 'PAYMENT_CROSSTRANSACT'");
                    var ofsettings = connection.QueryFirstOrDefault<string>(cmd_ofsettings.CommandText);
                    var payment_crosstransact = JObject.Parse(ofsettings);

                    //crosstransact_past_minutes
                    DateTime withdraw_past_few_hours = DateTime.UtcNow.AddHours(8).AddHours(-1 * int.Parse(payment_crosstransact["withdraw_past_few_hours"].ToString()));
                    //DateTime crosstransact_past_minutes = DateTime.UtcNow.AddHours(8).AddMinutes(-1 * int.Parse(payment_crosstransact["crosstransact_past_minutes"].ToString()));

                    var fastpay = connection.QueryFirstOrDefault("SELECT fastdeposit FROM ofThirdpay WITH(NOLOCK) WHERE id=10530");


                    //string cmd_available =
                    //  "SELECT * " +
                    //  "FROM ofWithdrawal WITH(NOLOCK) " +
                    //  "WHERE requesttime > '" + withdraw_past_few_hours.ToString("yyyy-MM-dd HH:mm:ss") + "' AND status=1 AND " +
                    //  "checkperson IS NULL AND wid NOT IN (4,19,18) AND CARDBANK <> 'GP' AND CARDBANK NOT LIKE '%usdt%' AND amount IN (" + fastpay.fastdeposit + ") AND currencycode=156 ";


                    string cmd_available =
                      "SELECT * " +
                      "FROM ofWithdrawal WITH(NOLOCK) " +
                      "WHERE requesttime > '" + withdraw_past_few_hours.ToString("yyyy-MM-dd HH:mm:ss") + "' AND status=1 AND " +
                      "checkperson IS NULL AND wid=3 AND currencycode=156 ";


                    var rows = connection.Query<BaseWithdrawReturn>(cmd_available);

                    var totalItems = rows.Count() <= 0 ? 0 : rows.First().TotalItems;

                    // Apply the LevelName 
                    if (rows.Count() > 0)
                        rows.ToList().ForEach(x =>
                        {
                            x.TotalAmount = 0;

                            x.minutesdiff = (Common.CurrentTime - x.requesttime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);


                            #region 根据当前货币查询 显示不同的货币等级
                            if (request.currencycode == (int)LangCodes.CurrencyCode.INR
                            || request.currencycode == (int)LangCodes.CurrencyCode.USD) 
                            {
                                if (VipLevelINTL.levelIntlName.ContainsKey(x.level))
                                    x.levelname = VipLevelINTL.levelIntlName[x.level];
                            }
                            else {
                                x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                            }
                            #endregion
                            //x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];


                            switch (x.status)
                            {
                            //case 0: x.cardno = "*****"; break;
                            case 1:
                                    if (string.IsNullOrEmpty(x.checkperson))
                                    {
                                        x.cardno = "*****";
                                        //x.amount = "*****";
                                    }
                                    else
                                    {
                                        x.cardno = request.Admin == x.checkperson.ToLower() ? x.cardno : Common.Security.ObscureWithdrawCard(x.cardno);
                                        x.cardno = "*****";
                                        x.amount = request.Admin == x.checkperson.ToLower() ? x.amount : "*****";
                                    }
                                    break;
                                default:
                                    x.cardno = "*****";
                                    x.amount = "*****";
                                    break;
                            }

                        });
                    else
                    {
                        return new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                        {
                            code = ShareEntity.Global.ResponseCode.code.FAIL,
                            message = "没",
                            Data = new PagingModel<BaseWithdrawReturn>()
                            {
                                TotalItems = 0,
                                Data = null
                            }
                        };
                    }


                    var result = new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "测试",
                        Data = new PagingModel<BaseWithdrawReturn>()
                        {
                            TotalItems = totalItems,
                            Data = rows
                        }
                    };

                    // TODO: Optimization here
                    return result;

                }
                catch (Exception)
                {
                    return new ResponseModel<PagingModel<BaseWithdrawReturn>>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没",
                        Data = new PagingModel<BaseWithdrawReturn>()
                        {
                            TotalItems = 0,
                            Data = new List<BaseWithdrawReturn>()
                        }
                    };
                }
            }
        }
    }

}

