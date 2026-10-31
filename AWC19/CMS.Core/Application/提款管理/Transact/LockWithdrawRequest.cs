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
using ShareEntity.OutputModels;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
//using System.Runtime.Serialization;

namespace CMS.Core.Application.提款管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class LockWithdrawRequest : IRequest<ResponseModel<LockWithdrawRequestReturn>>
    {
        public long Id { get; set; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

    public class LockWithdrawRequestReturn
    {
        public long id { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public decimal amount { get; set; }
        public string cardno { get; set; }
        public string cardaddress { get; set; }
        public string cardbank { get; set; }
        public DateTime requesttime { get; set; }
        public int status { get; set; }

        public string requestip { get; set; }
        //public string requestaddress { get; set; }
        public DateTime? reviewtime { get; set; }
        public string reviewperson { get; set; }
        public DateTime? paytime { get; set; }
        public string payperson { get; set; }
        public DateTime? canceltime { get; set; }
        public string cancelperson { get; set; }
        public string cancelreason { get; set; }
        //public decimal? balance { get; set; }
        //public string referrer { get; set; }
        //public string referrerreg { get; set; }
        public int level { get; set; }
        //public string affaccount { get; set; }
        public string checkperson { get; set; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class LockWithdrawRequestValidator : AbstractValidator<LockWithdrawRequest>
    {
        public LockWithdrawRequestValidator()
        {
            //RuleFor(x => x.Id)
            //    .NotEmpty().WithMessage("ID不可为空")
            //    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class LockWithdrawRequestHandler : BaseWithdraw<LockWithdrawRequest, ResponseModel<LockWithdrawRequestReturn>>
    {
        public LockWithdrawRequestHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<LockWithdrawRequestReturn> Handle(LockWithdrawRequest request)
        {

            var resultDetail = GetWithdrawDetails(request.Id);
            if (resultDetail == null)
            {
                var result = new ResponseModel<LockWithdrawRequestReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "没数据",
                    Data = null
                };
                return result;
            }



            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING))
            {
                var result = new ResponseModel<LockWithdrawRequestReturn>();
                // Check if there's any pending transaction

                var checkbuilder = new SqlBuilder();
                var checktemplate = checkbuilder.AddTemplate("" +
                    "SELECT count(*) " +
                    "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");
                checkbuilder.Where("requesttime > @requesttime", new { requesttime = Common.CurrentTime.AddDays(-30)});
                checkbuilder.Where("status = 1", new { Id = request.Id });
                checkbuilder.Where("checkperson = @CHECKPERSON", new { CHECKPERSON = request.Admin });
                var checkResult = connection.QueryFirstOrDefault<long>(checktemplate.RawSql, checktemplate.Parameters);
                if (checkResult != null && checkResult > 4) {
                    result.code = ShareEntity.Global.ResponseCode.code.FAIL;
                    result.message = "有未处理的订单";
                    return result;
                }



                int isSuccess = 0;

                if (resultDetail.wid == 3 || resultDetail.wid == 4) //3:定额网银转人工，4：定额支付宝转人工
                {

                    isSuccess = connection.Execute("" +
                    "UPDATE ofWithdrawal " +
                    "SET checkperson=@CHECKPERSON, isTransferToManual=1  " +
                    "WHERE id=@ID AND checkperson IS NULL ", (new
                    {
                        ID = resultDetail.id,
                        CHECKPERSON = request.Admin
                    }));
                }
                else {

                    isSuccess = connection.Execute("" +
                    "UPDATE ofWithdrawal " +
                    "SET checkperson=@CHECKPERSON  " +
                    "WHERE id=@ID AND checkperson IS NULL ", (new
                    {
                        ID = resultDetail.id,
                        CHECKPERSON = request.Admin
                    }));

                }
                //int count = 0;
                //using (var transaction = connection.BeginTransaction())
                //{
                //    try
                //    {
                //        count = connection.Execute("" +
                //            "UPDATE ofWithdrawal " +
                //            "SET checkperson=@CHECKPERSON  " +
                //            "WHERE id=@ID AND checkperson IS NULL ", (new
                //            {
                //                ID = resultDetail.id,
                //                CHECKPERSON = request.Admin
                //            }), transaction);
                //        if (count > 0)
                //        {
                //            //提交事务
                //            transaction.Commit();
                //        }
                //        else
                //        {
                //            //提交事务
                //            transaction.Rollback();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        //出现异常，事务Rollback
                //        transaction.Rollback();
                //    }
                //}

                switch (isSuccess)
                {
                    case 0:
                        result.code = ShareEntity.Global.ResponseCode.code.FAIL;
                        result.message = "锁定失败";
                        break;
                    case 1:
                        var builder = new SqlBuilder();
                        var template = builder.AddTemplate("" +
                            "SELECT * " +
                            "FROM ofWithdrawal WITH(NOLOCK) /**where**/ ");
                        builder.Where("id = @Id", new { Id = request.Id });
                        var details = connection.QueryFirst<LockWithdrawRequestReturn>(template.RawSql, template.Parameters);


                        result.code = ShareEntity.Global.ResponseCode.code.SUCCESS;
                        result.message = "锁定成功";
                        result.Data = details;
                        break;
                    default:
                        result.code = ShareEntity.Global.ResponseCode.code.FAIL;
                        result.message = "锁定失败";
                        break;
                }

                return result;

            }

        }


    }
}

