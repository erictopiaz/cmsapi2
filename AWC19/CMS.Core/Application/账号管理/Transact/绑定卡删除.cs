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
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.OutputModels;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 绑定卡删除 : IRequest<绑定卡删除Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int CardIndex { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 绑定卡删除Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 绑定卡删除Validator : AbstractValidator<绑定卡删除>
    {
        public 绑定卡删除Validator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 绑定卡删除Handler : BaseAccount<绑定卡删除, 绑定卡删除Return>
    {
        public 绑定卡删除Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 绑定卡删除Return Handle(绑定卡删除 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                if (!string.IsNullOrEmpty(request.id))
                {
                    var accountRow = connection.QuerySingle("SELECT account FROM ofuser WITH(NOLOCK) WHERE id=" + request.id);
                    request.Account = accountRow.account;
                }
                var userDetail = connection.Get<ofUser>(request.Account);
                string withdrawCards = userDetail.withdrawcards;
                if (string.IsNullOrEmpty(withdrawCards))
                    return new 绑定卡删除Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没绑定的卡"
                    };

                var withdrawCardList = new List<string>(withdrawCards.Split('|'));
                if (withdrawCardList.Count - 1 < request.CardIndex)
                {
                    return new 绑定卡删除Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "超过范围"
                    };

                }

                withdrawCardList.RemoveAt(request.CardIndex);
                string updatedWithdrawCards = string.Join('|', withdrawCardList);
                userDetail.withdrawcards = updatedWithdrawCards;
                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 绑定卡删除Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }
        }
    }
}

