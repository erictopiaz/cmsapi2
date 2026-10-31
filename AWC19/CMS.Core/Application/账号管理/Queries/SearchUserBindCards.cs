using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.提款管理;
using CMS.Core.Application.提款管理.Queries;
using Dapper.Contrib.Extensions;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SearchUserBindCards : IRequest<ResponseModel<List<SearchUserBindCardsReturn>>>
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 当前登录账号，可空
        /// </summary>

        [JsonIgnore]
        public string Admin { get; set; }
    }

    /// <summary>
    /// 查询回应数据结构
    /// </summary>
    public class SearchUserBindCardsReturn
    {
        //withdrawcards[i] = string.Concat(cardno, ",", cardbank, ",", cardaddress, ",", cardname);

        /// <summary>
        /// 卡号
        /// </summary>
        public string cardno { get; set; }

        /// <summary>
        /// 开户行名
        /// </summary>
        public string cardbank { get; set; }

        /// <summary>
        /// 开户行支行
        /// </summary>
        public string cardaddress { get; set; }
       

    }



    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SearchUserBindCardsValidator : AbstractValidator<SearchUserBindCards>
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchUserBindCardsValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SearchUserBindCardsTextSql
    {
        /// <summary>
        /// 查询账号基本信息 SQL
        /// </summary>
        public const string SearcharUserBindCardsSql = @"select withdrawcards from ofUser with(nolock) where account=@account  ";


    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SearchUserBindCardsHandler : BaseWithdraw<SearchUserBindCards, ResponseModel<List<SearchUserBindCardsReturn>>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public SearchUserBindCardsHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override ResponseModel<List<SearchUserBindCardsReturn>> Handle(SearchUserBindCards request)
        {
            var currentDate = DateTime.UtcNow.AddHours(8);
            var cards = new List<SearchUserBindCardsReturn>();
            //开始切割金额
            var r = DapperBase.Query<ofUser>(SearchUserBindCardsTextSql.SearcharUserBindCardsSql,
                new { account = request.account },
                        commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
            if (r != null)
            {
                var withdrawcardsString = r.First().withdrawcards;

                if (!string.IsNullOrEmpty(withdrawcardsString))
                {
                    var withdrawcards = withdrawcardsString.Split('|');
                    

                    for (int i = 0; i < withdrawcards.Length; i++)
                    {
                        // Obscure Cards
                        // Sample Card format 
                        // 工商银行, 4024007175245884,黑龙江
                        // Should look
                        var carddata = withdrawcards[i].Split(',');
                        var cardno = string.Concat(carddata[1].Substring(0, 4), "*******", carddata[1].Substring(carddata[1].Length - 4, 4));
                        var cardbank = carddata[0];
                        var cardaddress = carddata[2];
                       //var cardname = carddata.Length >= 4 ? carddata[3] : userDetail.realname;

                        //if (cardbank.ToLower().Contains("usdt") &&
                        //    adminAccess.Contains("/api/Account/AllowToSeeWalletAddress"))
                        //{
                        //    cardno = carddata[1];
                        //}


                        cards.Add(new SearchUserBindCardsReturn()
                        {
                            cardno = cardno,
                            cardbank = cardbank,
                            cardaddress = cardaddress
                        });

                    }

                }


                return new ResponseModel<List<SearchUserBindCardsReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = cards
                };
            }
            else
            {
                return new ResponseModel<List<SearchUserBindCardsReturn>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作失败",
                    Data = cards
                };
            }

        }

    }
}
