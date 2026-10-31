using AutoMapper;
using ShareEntity.OutputModels;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CMS.Core.Application.系统管理.Queries {

    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class GetGameProxyConfig : IRequest<ResponseModel<IEnumerable<GetGameProxyConfigReturn>>> { }

    public class GetGameProxyConfigReturn {
        public string gamename { get; set; }
        public int gamestatus { get; set; }
        public string gameid { get; set; }

        /// <summary>
        /// 游戏api 反向 代理
        /// </summary>
        public string proxyUrl { get; set; } = "";
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { get; set; } = "";
        /// <summary>
        /// 游戏配置数据id 唯一
        /// </summary>
        public long gameClassRowId { get; set; }

        public string game_params { set; get; } = "";
    }


    public class GetGameProxyConfigHandler : CMS.Core.BaseRequestHandler<GetGameProxyConfig, ResponseModel<IEnumerable<GetGameProxyConfigReturn>>> {
        public GetGameProxyConfigHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<IEnumerable<GetGameProxyConfigReturn>> Handle(GetGameProxyConfig request) {
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            var template = builder.AddTemplate("" +
                "SELECT id gameClassRowId, gamename,gamestatus,gameid,params game_params,class gameClass, useable, g_merchantAccounts merchantAccount " +
                "FROM ofGameClassMerchant WITH(NOLOCK) /**where**/ ");

            // FILL UP THE WHERE CLAUSE
            //builder.Where("class = 1");
            //builder.Where("gamestatus = 1");
            builder.Where("class=1 and useable=1 and g_merchantAccounts='Boss' and gameid NOT IN @EXCLUDENAME", new {
                EXCLUDENAME = new List<int>() {
                    136,142,2039,3039147,3044,2044,1024,1025,1028,130,100,2027,2028,3027,3028,2038,3038,141,3039,147,129
                }
            });

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var result = connection.Query<GetGameProxyConfigReturn>(template.RawSql, template.Parameters);

                foreach (var item in result) {
                    var pObj = JsonConvert.DeserializeObject<dynamic>(item.game_params);
                    item.proxyUrl = pObj.proxy;
                    item.game_params = "";
                }

                var responseData = new ResponseModel<IEnumerable<GetGameProxyConfigReturn>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = result
                };

                return responseData;
            }
        }

    }


}
