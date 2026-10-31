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
using CMS.Core.Application.系统管理.Queries;
using ShareEntity.Models;

namespace CMS.Core.Application.账号管理.Transact
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 更新游戏状态 : IRequest<更新游戏状态Return>
    {
        public string id { get; set; }
        public string Account { get; set; }
        public int[] allowedGames { get; set; }
        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class 更新游戏状态Return : ResponseModel
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 更新游戏状态Validator : AbstractValidator<更新游戏状态>
    {
        public 更新游戏状态Validator()
        {
            RuleFor(x => x.Account).Account();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 更新游戏状态Handler : BaseAccount<更新游戏状态, 更新游戏状态Return>
    {
        public 更新游戏状态Handler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger)
        {
        }


        protected override 更新游戏状态Return Handle(更新游戏状态 request)
        {
            _logger.LogInformation("Admin:{0}, Request:{1}", request.Admin, Newtonsoft.Json.JsonConvert.SerializeObject(request));
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {


                var querySql = string.Format(@"select gameid,gamename2 from ofgameclass
where class=1 and useable=1 ");

                var result = connection.Query<GameIdMapNameModel>(querySql);
                var gameNames = new List<GameIdMapNameModel>();//所有的游戏 平台
                foreach (var gameName in result) {
                    if (gameName.gamename2.Equals("TTG")) {
                        gameNames.Add(new GameIdMapNameModel { gameid = gameName.gameid, gamename2 = "TT" });
                    }
                    else
                        gameNames.Add(gameName);
                }
                var limitedBoxes = new List<string>();
                foreach(var gn in gameNames) {
                    if (!request.allowedGames.Contains(gn.gameid))
                        limitedBoxes.Add(gn.gamename2);
                }
                //有游戏要被禁
                var disabledGames = "";
                if (limitedBoxes.Count > 0) {
                    
                    foreach (var g in limitedBoxes) {
                        disabledGames = string.IsNullOrEmpty(disabledGames) ? g : string.Format(@"{0}|{1}", disabledGames, g);
                    }
                   
                }
                else {

                }


                var userDetail = connection.Query<ofUser>($"Select * from ofUser where id={request.id}").First();
                string misc = userDetail.misc;
                var miscDyn = string.IsNullOrEmpty(misc) ? JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(new { gamedisabledstat = "" })) : JsonConvert.DeserializeObject<dynamic>(misc);
                miscDyn.gamedisabledstat = disabledGames;
                string resultMisc = JsonConvert.SerializeObject(miscDyn, Formatting.None);
                userDetail.misc = resultMisc;
                bool updateResult = connection.Update<ofUser>(userDetail);
                return new 更新游戏状态Return() {
                    code = updateResult ? ShareEntity.Global.ResponseCode.code.SUCCESS : ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = updateResult ? "更新成功" : "更新失败"
                };


               
            }
        }
    }
}

