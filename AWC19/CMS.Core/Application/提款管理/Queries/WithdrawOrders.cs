using AutoMapper;
using ShareEntity.OutputModels;
using ShareEntity.SqlCommandText;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilsLib.lang;

namespace CMS.Core.Application.提款管理.Queries {
    
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class WithdrawOrders : IRequest<ResponseModel<WithdrawOrdersReturn>> {
        /// <summary>
        /// 0:提款审核，等审核的人数
        /// 1：新会员  代付人数
        /// 2：普通会员，代付人数
        /// 3：vip     代付人数
        /// 4：vip3 代付人数
        /// 5：usdt 代付人数
        /// </summary>
        public int type { get; set; } = -1;
       
        [JsonIgnore]
        public string Admin { get; set; }
    }


    //var newDPayNumbers = 0;
    //var putongDPayNumbers = 0;
    //var vipDPayNumbers = 0;
    //var vip3DPayNumbers = 0;

    //var newInrDPayNumbers = 0;
    //var putongInrDPayNumbers = 0;
    //var vipInrDPayNumbers = 0;
    //var vip3InrDPayNumbers = 0;

    //xfvipNumbers:0,
    //    xfputongNumbers:0,
    //    xfinrNumbers:0

    public class WithdrawOrdersReturn {
        public int xfvipNumbers { get; set; } = 0;
        public int xfputongNumbers { get; set; } = 0;
        public int xfinrNumbers { get; set; } = 0;
        public int putongNumbers { get; set; } = 0;
        public int vipNumbers { get; set; } = 0;
        public int inrNumbers { get; set; } = 0;

        public int newDPayNumbers { get; set; } = 0;
        public int putongDPayNumbers { get; set; } = 0;
        public int vipDPayNumbers { get; set; } = 0;
        public int vip3DPayNumbers { get; set; } = 0;
        public int newInrDPayNumbers { get; set; } = 0;
        public int putongInrDPayNumbers { get; set; } = 0;
        public int vipInrDPayNumbers { get; set; } = 0;
        public int vip3InrDPayNumbers { get; set; } = 0;
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class WithdrawOrdersValidator : AbstractValidator<WithdrawOrders> {
        public WithdrawOrdersValidator() {
            ////RuleFor(x => x.rowid)
            ////    .NotEmpty().WithMessage("ID不可为空")
            ////    .GreaterThan(0).WithMessage("不可比0还低");
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class WithdrawOrdersHandler : BaseWithdraw<WithdrawOrders, ResponseModel<WithdrawOrdersReturn>> {
        public WithdrawOrdersHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<WithdrawOrdersReturn> Handle(WithdrawOrders request) {

            
            var currentDate = DateTime.UtcNow.AddHours(8).AddHours(-24);
            var querySql = string.Empty;

            /// <summary>
            /// 0:提款审核，等审核的人数
            /// 1：新会员  代付人数
            /// 2：普通会员，代付人数
            /// 3：vip     代付人数
            /// 4：vip3 代付人数
            /// 5：usdt 代付人数
            /// 6: 提款风控
            /// </summary>

            if(request.type>6 || request.type <0) {

                return new ResponseModel<WithdrawOrdersReturn>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "未查询到提款单! ",
                    Data = new WithdrawOrdersReturn()
                };
            }

            if (request.type == 0 || request.type == 6) {
                querySql = string.Format(@"select u.account,u.level,u.currencyType,w.misc from ofWithdrawal w with(nolock) 
left join ofUser u with(nolock) on u.account=w.account
where requesttime>'{0}' and w.status=0", currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else {
                querySql = string.Format(@"select u.account,u.level,u.currencyType,checkperson from ofWithdrawal w with(nolock) 
left join ofUser u with(nolock) on u.account=w.account
where requesttime>'{0}' and w.status=1", currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            

            

            var waittingOrders = DapperBase.Query<WaittingOrders>(querySql, param: request, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            if (waittingOrders != null) {

                if(request.type == 0) {
                    return WithdrawalVerify(waittingOrders);
                }
                else if(request.type == 6) {
                    return WithdrawalXFVerify(waittingOrders);
                }
                else {
                    return WaitDirectPay(waittingOrders, request.type);
                }

            }
            else {
                
                return new ResponseModel<WithdrawOrdersReturn>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "未查询到提款单! ",
                    Data = new WithdrawOrdersReturn()
                };
            }


        }
        /// <summary>
        /// 提款风控页面，等审核定单
        /// </summary>
        /// <param name="waittingOrders"></param>
        /// <returns></returns>
        private ResponseModel<WithdrawOrdersReturn> WithdrawalXFVerify(IList<WaittingOrders> waittingOrders) {
            var xfvipNumbers = 0;
            var xfputongNumbers = 0;
            var xfinrNumbers = 0;


            foreach (var order in waittingOrders) {
                #region 排除风控，风控等审核的，会在风控列表里显示
                var miscStruct = JsonConvert.DeserializeObject<WithdrawMisc>(order.misc);
                if (!string.IsNullOrEmpty(miscStruct.xferriskmgt) && miscStruct.xferriskmgt.ToLower().Equals("true")) {
                    //被风控
                }
                else
                    continue;
                #endregion

                if (order.currencyType == 356) {
                    xfinrNumbers++;
                }

            }

            foreach (var order in waittingOrders) {
                #region 排除风控，风控等审核的，会在风控列表里显示
                var miscStruct = JsonConvert.DeserializeObject<WithdrawMisc>(order.misc);
                if (!string.IsNullOrEmpty(miscStruct.xferriskmgt) && miscStruct.xferriskmgt.ToLower().Equals("true")) { 
                    //被风控
                }
                else
                    continue;
                #endregion

                if (order.currencyType == 156) {
                    if (order.level == 10007 || order.level == 10001)
                        xfputongNumbers++;
                    else
                        xfvipNumbers++;
                }
            }

            return new ResponseModel<WithdrawOrdersReturn>() {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "有未处理数据!",
                Data = new WithdrawOrdersReturn() { xfvipNumbers = xfvipNumbers, xfputongNumbers = xfputongNumbers, inrNumbers = xfinrNumbers },
            };
        }
        /// <summary>
        /// 等代付的定单
        /// </summary>
        /// <param name="waittingOrders"></param>
        /// <returns></returns>
        private ResponseModel<WithdrawOrdersReturn> WaitDirectPay(IList<WaittingOrders> waittingOrders, int type) {
            var newDPayNumbers = 0;
            var putongDPayNumbers = 0;
            var vipDPayNumbers = 0;
            var vip3DPayNumbers = 0;

            var newInrDPayNumbers = 0;
            var putongInrDPayNumbers = 0;
            var vipInrDPayNumbers = 0;
            var vip3InrDPayNumbers = 0;


            //--新会员    10001,10007
            //-- 普通会员   10001,10007,10026
            //-- vip        10027, 10028, 10002, 10011, 10012
            //-- vip3       10003, 10004, 10005, 10025

            if(type == 1) {
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 356) {
                        if (order.level == 10007 || order.level == 10001) {
                            if(string.IsNullOrEmpty(order.checkperson))
                                newInrDPayNumbers++;
                        }
                    }
                }
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 156) {
                        if (order.level == 10007 || order.level == 10001)
                            if (string.IsNullOrEmpty(order.checkperson))
                                newDPayNumbers++;
                    }
                }
            }
            else if (type == 2) {
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 356) {
                        if (order.level == 10007 || order.level == 10001 || order.level == 10026) {
                            if (string.IsNullOrEmpty(order.checkperson))
                                putongInrDPayNumbers++;
                        }
                    }
                }
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 156) {
                        if (order.level == 10007 || order.level == 10001 || order.level == 10026)
                            if (string.IsNullOrEmpty(order.checkperson))
                                putongDPayNumbers++;
                    }
                }
            }
            else if (type == 3) {
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 356) {
                        if (order.level == 10027 || order.level == 10028 || order.level == 10002 || order.level == 10011 || order.level == 10012) {
                            if (string.IsNullOrEmpty(order.checkperson))
                                vipInrDPayNumbers++;
                        }
                    }
                }
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 156) {
                        if (order.level == 10027 || order.level == 10028 || order.level == 10002 || order.level == 10011 || order.level == 10012)
                            if (string.IsNullOrEmpty(order.checkperson))
                                vipDPayNumbers++;
                    }
                }
            }
            else if (type == 4) {
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 356) {
                        if (order.level == 10003 || order.level == 10004 || order.level == 10005 || order.level == 10025) {
                            if (string.IsNullOrEmpty(order.checkperson))
                                vip3InrDPayNumbers++;
                        }
                    }
                }
                foreach (var order in waittingOrders) {
                    if (order.currencyType == 156) {
                        if (order.level == 10003 || order.level == 10004 || order.level == 10005 || order.level == 10025)
                            if (string.IsNullOrEmpty(order.checkperson))
                                vip3DPayNumbers++;
                    }
                }
            }


            return new ResponseModel<WithdrawOrdersReturn>() {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "有未处理数据!",
                Data = new WithdrawOrdersReturn() {
                    newDPayNumbers = newDPayNumbers,
                    putongDPayNumbers = putongDPayNumbers,
                    vipDPayNumbers = vipDPayNumbers,
                    vip3DPayNumbers = vip3DPayNumbers,

                    newInrDPayNumbers = newInrDPayNumbers,
                    putongInrDPayNumbers = putongInrDPayNumbers,
                    vipInrDPayNumbers = vipInrDPayNumbers,
                    vip3InrDPayNumbers = vip3InrDPayNumbers,
                },
            };
        }
        /// <summary>
        /// 提款审核页面，等审核定单
        /// </summary>
        /// <param name="waittingOrders"></param>
        /// <returns></returns>
        private ResponseModel<WithdrawOrdersReturn> WithdrawalVerify(IList<WaittingOrders> waittingOrders) {
            var putongNumbers = 0;
            var vipNumbers = 0;
            var inrNumbers = 0;
            foreach (var order in waittingOrders) {
                #region 排除风控，风控等审核的，会在风控列表里显示
                var miscStruct = JsonConvert.DeserializeObject<WithdrawMisc>(order.misc);
                if (!string.IsNullOrEmpty(miscStruct.xferriskmgt) && miscStruct.xferriskmgt.ToLower().Equals("true"))
                    continue;
                #endregion

                if (order.currencyType == 356) {
                    inrNumbers++;
                }
                    
            }

            foreach (var order in waittingOrders) {

                #region 排除风控，风控等审核的，会在风控列表里显示
                var miscStruct = JsonConvert.DeserializeObject<WithdrawMisc>(order.misc);
                if (!string.IsNullOrEmpty(miscStruct.xferriskmgt) && miscStruct.xferriskmgt.ToLower().Equals("true"))
                    continue;
                #endregion

                if (order.currencyType == 156) {
                    if (order.level == 10007 || order.level == 10001)
                        putongNumbers++;
                    else
                        vipNumbers++;
                }
            }

            return new ResponseModel<WithdrawOrdersReturn>() {
                code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                message = "有未处理数据!",
                Data = new WithdrawOrdersReturn() { putongNumbers = putongNumbers, vipNumbers = vipNumbers, inrNumbers = inrNumbers },
            };
        }

    }

    public struct WithdrawMisc {
        public string xferriskmgt;
    }

    public struct WaittingOrders{
        public string account;
        public int level;
        public int currencyType;
        public string checkperson;
        public string misc;
    };
}
