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
using Dapper.Contrib.Extensions;
using CMS.Core.Application.系统管理.Queries;
using System.Transactions;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SyncAffiliate : IRequest<ResponseModel>
    {
        public string type { get; set; }
        public long Id { get; set; }

    }


    public class SyncAffiliateValidator : AbstractValidator<SyncAffiliate>
    {
        public SyncAffiliateValidator()
        {
        }
    }



    /// <summary>
    /// Request Handler
    /// </summary>
    public class SyncAffiliateHandler : BaseRequestHandler<SyncAffiliate, ResponseModel>
    {
        public SyncAffiliateHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel Handle(SyncAffiliate request)
        {
            switch (request.type) {
                case "deposit":
                    return Deposit(request.Id);
                case "withdraw":
                    return Withdraw(request.Id);
            }

            return new ResponseModel() { code = ShareEntity.Global.ResponseCode.code.FAIL, message="" };
        }

        private ResponseModel Deposit(long id) { 
            _logger.LogInformation("Syncing - [Deposit] id:{0}", id);



            string account = "";
            string affaccount = "";
            decimal fee = 0;
            decimal amount = 0;
            //DateTime requesttime;
            long financeMonth = 0;
            long financeDay = 0;

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var depositRow = connection.Get<ofDeposit>(id);
                if (depositRow.status != 1)
                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "已经处理过了",
                    };
                fee = depositRow.fee;
                affaccount = connection.Get<ofUser>(depositRow.account).affaccount;
                if (string.IsNullOrEmpty(affaccount))
                {
                    affaccount = connection.QueryFirst<string>("SELECT TOP 1 account from ofAffiliate WITH(NOLOCK) WHERE defaultAgent=1");
                }

                amount = depositRow.amount;
                account = depositRow.account;
                financeMonth = long.Parse(depositRow.requesttime.ToString("yyyyMM"));
                financeDay = long.Parse(depositRow.requesttime.ToString("yyyyMMdd"));

            }


            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    #region PlayerReport
                    
                    var playerDailyBuilder = new SqlBuilder();
                    var playerDailytemplate = playerDailyBuilder.AddTemplate("SELECT id FROM ofPlayerDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    playerDailyBuilder.AddParameters(new { ACCOUNT = account, FINANCEDAILY = financeDay });
                    var playerDailyID = connection.QueryFirstOrDefault<long?>(playerDailytemplate.RawSql, playerDailytemplate.Parameters);

                    long playerResult = 0;
                    long affiliateResult = 0;

                    if (playerDailyID != null)
                    {
                        // UPDATE
                        var playerDailyRow = connection.Get<ofPlayerDailyReportFinance>(playerDailyID);
                        playerDailyRow.depositAmount += amount;
                        playerDailyRow.depositFee += fee;
                        playerDailyRow.depositTimes++;
                        playerDailyRow.update_time = Common.CurrentTime;
                        var result = connection.Update(playerDailyRow);
                        _logger.LogInformation("Syncing - [Deposit] PlayerUpdate id:{0}, result:{1}", id, result);

                    }
                    else
                    {
                        // INSERT
                        var playerDaily = new ofPlayerDailyReportFinance()
                        {
                            account = account,
                            depositTimes = 1,
                            financeDay = financeDay,
                            depositFee = fee,
                            affaccount = affaccount,
                            depositAmount = amount,
                            update_time = Common.CurrentTime,
                            create_at = Common.CurrentTime
                        };
                        var result = connection.Insert<ofPlayerDailyReportFinance>(playerDaily);
                        _logger.LogInformation("Syncing - [Deposit] PlayerInsert id:{0}, result:{1}", id, result);
                    }
                    #endregion

                    #region AffiliateReport
                    var AffiliateDailyBuilder = new SqlBuilder();
                    var AffiliateDailytemplate = AffiliateDailyBuilder.AddTemplate("SELECT id FROM ofAffDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    AffiliateDailyBuilder.AddParameters(new { ACCOUNT = affaccount, FINANCEDAILY = financeDay });
                    var AffiliateDailyID = connection.QueryFirstOrDefault<long?>(AffiliateDailytemplate.RawSql, AffiliateDailytemplate.Parameters);

                    if (AffiliateDailyID != null)
                    {
                        // UPDATE
                        var AffiliateDailyRow = connection.Get<ofAffDailyReportFinance>(AffiliateDailyID);
                        AffiliateDailyRow.depositAmount += amount;
                        AffiliateDailyRow.depositFee += fee;
                        AffiliateDailyRow.depositTimes++;
                        AffiliateDailyRow.update_time = Common.CurrentTime;
                        var result = connection.Update(AffiliateDailyRow);

                        _logger.LogInformation("Syncing - [Deposit] AffiliateUpdate id:{0}, result:{1}", id, result);
                    }
                    else
                    {
                        // INSERT
                        var AffiliateDaily = new ofAffDailyReportFinance()
                        {
                            account = affaccount,
                            depositTimes = 1,
                            financeDay = financeDay,
                            depositFee = fee,
                            depositAmount = amount,
                            update_time = Common.CurrentTime,
                            create_at = Common.CurrentTime
                        };
                        var result = connection.Insert<ofAffDailyReportFinance>(AffiliateDaily);
                        _logger.LogInformation("Syncing - [Deposit] AffiliateUpdate id:{0}, result:{1}", id, result);
                    }
                    #endregion


                    scope.Complete();

                }
                catch (Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "运行出错 " + errorStamp,
                    };
                }
                return new ResponseModel()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "运行出错 ",
                };


            }

        }

        private ResponseModel Withdraw(long id)
        {
            _logger.LogInformation("Syncing - [Withdraw] id:{0}", id);

            string account = "";
            string affaccount = "";
            decimal fee = 0;
            decimal amount = 0;
            //DateTime requesttime;
            long financeMonth = 0;
            long financeDay = 0;

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var withdrawRow = connection.Get<ofWithdrawal>(id);
                if (withdrawRow.status != 2)
                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "已经处理过了",
                    };

                fee = withdrawRow.fee;
                affaccount = connection.Get<ofUser>(withdrawRow.account).affaccount;
                if (string.IsNullOrEmpty(affaccount))
                {
                    affaccount = connection.QueryFirst<string>("SELECT TOP 1 account from ofAffiliate WITH(NOLOCK) WHERE defaultAgent=1");
                }

                amount = withdrawRow.amount;
                account = withdrawRow.account;
                financeMonth = long.Parse(withdrawRow.requesttime.ToString("yyyyMM"));
                financeDay = long.Parse(withdrawRow.requesttime.ToString("yyyyMMdd"));

            }


            using (TransactionScope scope = new TransactionScope())
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                try
                {
                    #region PlayerReport

                    var playerDailyBuilder = new SqlBuilder();
                    var playerDailytemplate = playerDailyBuilder.AddTemplate("SELECT id FROM ofPlayerDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    playerDailyBuilder.AddParameters(new { ACCOUNT = account, FINANCEDAILY = financeDay });
                    var playerDailyID = connection.QueryFirstOrDefault<long?>(playerDailytemplate.RawSql, playerDailytemplate.Parameters);



                    if (playerDailyID != null)
                    {
                        // UPDATE
                        var playerDailyRow = connection.Get<ofPlayerDailyReportFinance>(playerDailyID);
                        playerDailyRow.withdrawAmount += amount;
                        playerDailyRow.withdrawFee += fee;
                        playerDailyRow.withdrawTimes++;
                        playerDailyRow.update_time = Common.CurrentTime;
                        var result = connection.Update(playerDailyRow);
                        _logger.LogInformation("Syncing - [Withdraw] PlayerUpdate id:{0}, result:{1}", id, result);

                    }
                    else
                    {
                        // INSERT
                        var playerDaily = new ofPlayerDailyReportFinance()
                        {
                            account = account,
                            withdrawTimes = 1,
                            financeDay = financeDay,
                            withdrawFee = fee,
                            affaccount = affaccount,
                            withdrawAmount = amount,
                            update_time = Common.CurrentTime,
                            create_at = Common.CurrentTime
                        };
                        var result = connection.Insert<ofPlayerDailyReportFinance>(playerDaily);
                        _logger.LogInformation("Syncing - [Withdraw] PlayerInsert id:{0}, result:{1}", id, result);

                    }
                    #endregion

                    #region AffiliateReport

                    var AffiliateDailyBuilder = new SqlBuilder();
                    var AffiliateDailytemplate = AffiliateDailyBuilder.AddTemplate("SELECT id FROM ofAffDailyReportFinance WITH(NOLOCK) WHERE account=@ACCOUNT and financeDay=@FINANCEDAILY");
                    AffiliateDailyBuilder.AddParameters(new { ACCOUNT = affaccount, FINANCEDAILY = financeDay });
                    var AffiliateDailyID = connection.QueryFirstOrDefault<long?>(AffiliateDailytemplate.RawSql, AffiliateDailytemplate.Parameters);



                    if (AffiliateDailyID != null)
                    {
                        // UPDATE
                        var AffiliateDailyRow = connection.Get<ofAffDailyReportFinance>(AffiliateDailyID);
                        AffiliateDailyRow.withdrawAmount += amount;
                        AffiliateDailyRow.withdrawFee += fee;
                        AffiliateDailyRow.withdrawTimes++;
                        AffiliateDailyRow.update_time = Common.CurrentTime;
                        var result = connection.Update(AffiliateDailyRow);
                        _logger.LogInformation("Syncing - [Withdraw] AffiliateUpdate id:{0}, result:{1}", id, result);

                    }
                    else
                    {
                        // INSERT
                        var AffiliateDaily = new ofAffDailyReportFinance()
                        {
                            account = affaccount,
                            withdrawTimes = 1,
                            financeDay = financeDay,
                            withdrawFee = fee,
                            withdrawAmount = amount,
                            update_time = Common.CurrentTime,
                            create_at = Common.CurrentTime
                        };
                        var result = connection.Insert<ofAffDailyReportFinance>(AffiliateDaily);
                        _logger.LogInformation("Syncing - [Withdraw] AffiliateInsert id:{0}, result:{1}", id, result);
                    }
                    #endregion


                    scope.Complete();

                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                        message = "",
                    };
                }


                catch(Exception ex)
                {
                    string errorStamp = Common.TimeStamp;
                    _logger.LogError("Exception:{0}, Stamp:{1}", ex.Message, errorStamp);
                    return new ResponseModel()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "运行出错 " + errorStamp,
                    };
                }

            }



        }


    }

}

