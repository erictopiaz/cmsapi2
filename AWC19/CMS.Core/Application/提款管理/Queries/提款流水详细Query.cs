using AutoMapper;
using ShareEntity.OutputModels;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using static CMS.Core.Common;

namespace CMS.Core.Application.提款管理.Queries {
    
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 提款流水详细Query : IRequest<ResponseModel<提款流水详细Return>> {
        public long Id { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class WithdrawRowModel {
        //account, verify_guid
        public string account { get; set; }

        public string verify_guid { get; set; }
        public int currencycode { get; set; }
        public string merchantAccount { get; set; }
        public int currencyType { get; set; }
    }

    public class 提款流水详细Return {
        //v_status,r_data,verifyGuid
        public int v_status { get; set; }
        public string r_data { 
            set {
                if(string.IsNullOrEmpty(value)) {
                    betModel = new VerifyWithdrawDataModel();
                }
                else {
                    var unescapeString = Regex.Unescape(value);
                    betModel = JsonConvert.DeserializeObject<VerifyWithdrawDataModel>(unescapeString);
                }
            }
            get { return ""; }
        }
        public string verifyGuid { get; set; }

        public VerifyWithdrawDataModel betModel { get; set; } = new VerifyWithdrawDataModel();
    }

    /// <summary>
    /// 存款金额      红利金额      返水金额       要求流水，实际投注流水    起始时间       结束时间
    /// </summary>
    public class VerifyWithdrawDataModel {
        public decimal totalDeposit { set; get; } = 0;
        public decimal totalPromo { set; get; } = 0;
        public string startTime { set; get; } = "";
        public string endTime { set; get; } = "";
        public decimal minTotalBet { set; get; } = 0;

        public GameReportResponseModel<GameReport, GameData> result { set; get; } = new GameReportResponseModel<GameReport, GameData>();
        public string remark { set; get; } = "";
    }

    public class GameReportResponseModel<T, K> {
        public string message { get; set; }
        public List<T> result { get; set; }
        public K totalData { get; set; }
    }

    public class GameData {
        public decimal totalBetAmount { get; set; }
        public decimal betAmount { get; set; }
        public decimal prizeAmount { get; set; }
        public decimal winLost { set; get; }
    }

    public class GameReport : GameData {
        public string gameId { get; set; }
    }


    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 提款流水详细QueryValidator : AbstractValidator<提款流水详细Query> {
        public 提款流水详细QueryValidator() {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID不可为空")
                .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 提款流水详细Handler : BaseWithdraw<提款流水详细Query, ResponseModel<提款流水详细Return>> {
        public 提款流水详细Handler(IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger, env) { }

        protected override ResponseModel<提款流水详细Return> Handle(提款流水详细Query request) {

            var withdrawRowSql = string.Format(@"select w.account, verify_guid, u.currencyType,u.merchantAccount from ofWithdrawal w with(nolock) 
left join ofUser u with(nolock) on w.account=u.account  where w.id=@id");
            var verifyWithdrawRowSql = string.Format(@"select top 1 v_status,r_data,verifyGuid from ofWithdrawalAutoVerify
where verifyGuid=@verify_guid order by id desc");
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var withdrawRow = connection.QueryFirst<WithdrawRowModel>(withdrawRowSql, new {id= request.Id});
                //if (withdrawRow != null && withdrawRow.currencycode == 156)
                //{
                //    //rmb
                //    return new ResponseModel<提款流水详细Return>()
                //    {
                //        code = ShareEntity.Global.ResponseCode.code.FAIL,
                //        message = "",
                //        Data = new 提款流水详细Return()
                //    };
                //}

                if (withdrawRow != null 
                    && withdrawRow.merchantAccount.Equals("Awc",StringComparison.OrdinalIgnoreCase))
                {
                    //rmb
                    return new ResponseModel<提款流水详细Return>()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "",
                        Data = new 提款流水详细Return()
                    };
                }


                var verifyWithdrawModel = connection.QueryFirstOrDefault<提款流水详细Return>(verifyWithdrawRowSql,
                    new { verify_guid = withdrawRow.verify_guid });

                return new ResponseModel<提款流水详细Return>() {
                    code = verifyWithdrawModel != null ? ShareEntity.Global.ResponseCode.code.SUCCESS: ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "",
                    Data = verifyWithdrawModel
                };
            }
        }
    }
}
