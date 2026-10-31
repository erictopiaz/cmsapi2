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
using UtilsLib;
using ShareEntity.Models;

namespace CMS.Core.Application.系统管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetGameCode : IRequest<ResponseModel<GameIdMapNameResponseModel>> {
        public int currencyType { set; get; }
        public long id { set; get; }
    }

    public class GameIdMapNameModel {
        public int gameid { set; get; }
        public string gamename2 { set; get; }
        public string currencyTypes { set; get; }
    }
    
    public class GameIdMapNameResponseModel {
        public List<GameIdMapNameModel> allowedGames { set; get; }
        public List<int> limitedBoxes { set; get; }

        public List<string> allowedCurrencyGames { set; get; }
    }

    public class GetGameCodeHandler : CMS.Core.BaseRequestHandler<GetGameCode, ResponseModel<GameIdMapNameResponseModel>>
    {
        public GetGameCodeHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        protected override ResponseModel<GameIdMapNameResponseModel> Handle(GetGameCode request) {

            //var querySql = string.Format(@"select gamename2 from ofgameclass
            //where class=1 and useable=1 and currencyTypes like '%{0}%' ", request.currencyType);

            var allowedCurrencyGames = new List<string>();

            var querySql = string.Format(@"select gameid,gamename2,currencyTypes from ofgameclass
where class=1 and useable=1 ");
            var limitedBoxes = new List<int>();
            var allowedBoxes = new List<int>();
            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var result = connection.Query<GameIdMapNameModel>(querySql);
                var gameNames = new List<GameIdMapNameModel>();//所有的游戏 平台
                foreach(var gameName in result) {
                    if (gameName.gamename2.Equals("TTG")) {
                        gameNames.Add(new GameIdMapNameModel { gameid =gameName.gameid, gamename2="TT",currencyTypes=gameName.currencyTypes });
                    }
                    else
                        gameNames.Add(gameName);
                }

                foreach(var gameName in gameNames) {
                    if (gameName.currencyTypes.Contains(request.currencyType.ToString()))
                        allowedCurrencyGames.Add(gameName.gamename2);
                }

                
                var userDetail = connection.Query<ofUser>($"Select * from ofUser where id={request.id}").First();
                string misc = userDetail.misc;
                var miscDict = string.IsNullOrEmpty(misc) ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(misc);
                if (miscDict.ContainsKey("gamedisabledstat")) {
                    var limitGames = miscDict["gamedisabledstat"].Split('|');
                    foreach(var lg in limitGames) {
                        foreach(var gn in gameNames) {
                            if (lg.ToUpper().Equals(gn.gamename2.ToUpper())) {
                                limitedBoxes.Add(gn.gameid);
                            }
                        }
                    }
                }
                else {
                   //没有被禁游戏
                }

                foreach(var gn in gameNames) {
                    if(!limitedBoxes.Contains(gn.gameid)) {
                        allowedBoxes.Add(gn.gameid);
                    }
                }
                

                var responseData = new ResponseModel<GameIdMapNameResponseModel>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new GameIdMapNameResponseModel() { 
                        allowedGames = gameNames, 
                        limitedBoxes = allowedBoxes, 
                        allowedCurrencyGames= allowedCurrencyGames }
                };

                return responseData;
            }

            
        }

        #region 没有区分货币
        private List<string> test(GetGameCode request) {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("" +
                "SELECT gamename " +
                "FROM ofGameClass WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            builder.Where("class = 1");
            builder.Where("useable = 1");
            builder.Where("gamename NOT IN @EXCLUDENAME", new {
                EXCLUDENAME = new List<string>() {
                    "SPB", "主钱包", "万象城娱乐","PT老虎机","SPT", "BSG"
                }
            });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var result = connection.Query<string>(template.RawSql, template.Parameters);
                var resultArray = result.ToArray();
                for (int i = 0; i < resultArray.Count(); i++)
                    resultArray[i] = resultArray[i].ToUpper() == "UED" ? "LB" : resultArray[i];


                return resultArray.ToList();
            }
        }



        //private  ResponseModel<IEnumerable<string>> test(GetGameCode request) {
        //    // 创建DAPPER TEMPLATE 
        //    var builder = new SqlBuilder();
        //    var template = builder.AddTemplate("" +
        //        "SELECT gamename " +
        //        "FROM ofGameClass WITH(NOLOCK) /**where**/ ");

        //    // FILL UP THE WHERE CLAUSE
        //    builder.Where("class = 1");
        //    builder.Where("useable = 1");
        //    builder.Where("gamename NOT IN @EXCLUDENAME", new {
        //        EXCLUDENAME = new List<string>() {
        //            "SPB", "主钱包", "万象城娱乐","PT老虎机","SPT", "BSG"
        //        }
        //    });

        //    using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
        //        var result = connection.Query<string>(template.RawSql, template.Parameters);
        //        var resultArray = result.ToArray();
        //        for (int i = 0; i < resultArray.Count(); i++)
        //            resultArray[i] = resultArray[i].ToUpper() == "UED" ? "LB" : resultArray[i];


        //        var responseData = new ResponseModel<IEnumerable<string>>() {
        //            code = ShareEntity.Global.ResponseCode.code.SUCCESS,
        //            message = "",
        //            Data = resultArray
        //        };

        //        return responseData;
        //    }
        //}
        #endregion
    }
}

