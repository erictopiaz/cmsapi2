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
using System.Net;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新卡名称 : IRequest<更新卡名称Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int CardIndex { get; set; }
        public string CardName { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新卡名称Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新卡名称Validator : AbstractValidator<更新卡名称>
    {
        public 更新卡名称Validator()
        {
            RuleFor(x => x.Account).Account();
            //RuleFor(x => x.CardName).CharAndDigitalNullable(1,20);
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新卡名称Handler : BaseAccount<更新卡名称, 更新卡名称Return>
    {
        public 更新卡名称Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 更新卡名称Return Handle(更新卡名称 request)
        {
            request.CardName = string.IsNullOrEmpty(request.CardName) ? "" : request.CardName.Replace("'", "''");
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            string cardtosearch = "";
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
                    return new 更新卡名称Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "没绑定的卡"
                    };

                var withdrawCardList = new List<string>(withdrawCards.Split('|'));

                if (withdrawCardList.Count - 1 < request.CardIndex)
                {
                    return new 更新卡名称Return()
                    {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "超过范围"
                    };

                }

                cardtosearch = withdrawCardList[request.CardIndex];
                var cardtosearch_splitted = new List<string>(cardtosearch.Split(','));
                if (cardtosearch_splitted.Count == 3)
                {
                    // INSERT the new name
                    cardtosearch_splitted.Add(request.CardName);
                }
                else if (cardtosearch_splitted.Count == 4)
                { 
                    // UPDATE
                    cardtosearch_splitted[3] = request.CardName;
                }

                withdrawCardList[request.CardIndex] = string.Join(',', cardtosearch_splitted);

                string updatedcard = string.Join('|', withdrawCardList);

                userDetail.withdrawcards = updatedcard;
                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 更新卡名称Return()
                {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = updatedcard
                };
            }

        }
    }
}

