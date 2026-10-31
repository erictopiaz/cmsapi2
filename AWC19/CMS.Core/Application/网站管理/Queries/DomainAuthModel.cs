using AutoMapper;
using ShareEntity.OutputModels;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CMS.Core.Application.网站管理.Queries {

    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class DomainAuthModel : IRequest<ResponseModel<List<RowModel>>> {
        public int currencycode { get; set; } = 156;
        public List<int> currencys { get; set; } = new List<int>();
        [JsonIgnore]
        public string Admin { get; set; }
    }

    //public class DomainAuthModelReturn : ResponseModel {

    //    public int currenDay { get; set; }
    //    public string domain { get; set; }
    //    public int loginTimes { get; set; } = 0;
    //}

    public class DomainAuthModelReturn {

        public int currenDay { get; set; }
        public string domain { get; set; }
        public int loginTimes { get; set; } = 0;
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class DomainAuthModelValidator : AbstractValidator<DomainAuthModel> {
        public DomainAuthModelValidator() {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class DomainAuthModelHandler : BaseRequestHandler<DomainAuthModel, ResponseModel<List<RowModel>>> {
        public DomainAuthModelHandler(IMapper mapper, ILoggerFactory logger)
            : base(CMS.Data.MainDB.DapperConnectionLog, mapper, logger) { }

        protected override ResponseModel<List<RowModel>> Handle(DomainAuthModel request) {

            var currentDate = DateTime.UtcNow.AddHours(8);
            var days = new List<int>();
            for(int i =0;i<7;i++) {
                var index = -1 * i;
                var day = currentDate.AddDays(index).ToString("yyyyMMdd");
                days.Add(Convert.ToInt32(day));
            }
            var queryDomainSql = string.Format(@"select currenDay,domain, sum(loginTimes) loginTimes from DomainActionNameRcordsReport
where currenDay>={0} and currenDay<={1}
group by currenDay,domain
order by currenDay desc", days.Last(), days.First());

            //var models = new List<DomainAuthModelReturn>(); 

            var rowModels = new List<RowModel>();

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.DapperConnectionLog)) {

                var models = connection.Query<DomainAuthModelReturn>(queryDomainSql);
                if(models != null && models.Count() > 0) {
                    foreach (var m in models) {
                        var d = rowModels.Where(x => x.domain == m.domain);
                        if(!d.Any())
                            rowModels.Add(new RowModel() { domain = m.domain });
                    }
                }
                #region 整理出每个域名最近7天的登录情况
                foreach (var rw in rowModels) {
                    foreach(var m in models) {
                        if (rw.domain.ToLower().Equals(m.domain)) {
                            if (m.currenDay == days.First())
                                rw.Day1_Times = m.loginTimes;
                            else if (m.currenDay == days[1])
                                rw.Day2_Times = m.loginTimes;
                            else if (m.currenDay == days[2])
                                rw.Day3_Times = m.loginTimes;
                            else if (m.currenDay == days[3])
                                rw.Day4_Times = m.loginTimes;
                            else if (m.currenDay == days[4])
                                rw.Day5_Times = m.loginTimes;
                            else if (m.currenDay == days[5])
                                rw.Day6_Times = m.loginTimes;
                            else if (m.currenDay == days[6])
                                rw.Day7_Times = m.loginTimes;
                            else
                                continue;
                        }
                        else
                            continue;
                    }
                }
                #endregion

                #region 所有域名，整理到 rowModels  内 （已经存在不处理，否则追加在后面）
                var queryAllDomainSql = string.Format(@"select domain from [Domains] with(nolock) where id>0");
                var allModels = connection.Query<DomainAuthModelReturn>(queryAllDomainSql);
                var appendModels = new List<RowModel>();
                if (allModels != null && allModels.Count() > 0) {
                    foreach(var am in allModels) {
                        var a_any = rowModels.Where(x=>x.domain.ToLower().Equals(am.domain.ToLower())).ToList();
                        if (!a_any.Any())
                            appendModels.Add(new RowModel() { domain = am.domain });
                    }
                }

                rowModels.AddRange(appendModels);
                #endregion


                return new ResponseModel<List<RowModel>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = rowModels
                };
            }
        }
    }


    public class RowModel {
        public string domain { set; get; }
        public int Day1_Times { set; get; } = 0;
        public int Day2_Times { set; get; } = 0;
        public int Day3_Times { set; get; } = 0;
        public int Day4_Times { set; get; } = 0;
        public int Day5_Times { set; get; } = 0;
        public int Day6_Times { set; get; } = 0;
        public int Day7_Times { set; get; } = 0;
    }

}
