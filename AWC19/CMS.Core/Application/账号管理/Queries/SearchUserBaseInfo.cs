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
    public class SearchUserBaseInfo : IRequest<ResponseModel<SearchUserBaseInfoReturn>>
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
    public class SearchUserBaseInfoReturn
    {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { get; set; }
        private string _telephone;
        /// <summary>
        /// 电话号 139****1234
        /// </summary>
        public string telephone {
            set {
                _telephone = string.Format(@"{0}****{1}", value.Substring(0, 3), value.Substring(value.Length - 2, 2));
                
            }
            get { return _telephone; }
        }
        /// <summary>
        /// 当前主账号金额
        /// </summary>
        public decimal amountactive { get; set; }
        /// <summary>
        /// 当前总充值
        /// </summary>
        public decimal depositamount {  get; set; }
        /// <summary>
        /// 当前总充值次数
        /// </summary>
        public int depositnumber { get; set; }
        /// <summary>
        /// 当前总提款
        /// </summary>
        public decimal withdrawalamount { get; set; }
        /// <summary>
        /// 当前总提款次数
        /// </summary>
        public int withdrawalnumber { get; set; }
        /// <summary>
        /// 当前会员备注信息
        /// </summary>
        public string riskRemark { get; set; }
        /// <summary>
        /// 当前vip状态备注
        /// </summary>
        public string vipStatusRemark { get; set; }
        /// <summary>
        /// 注册ip,对应实际地址（参考）
        /// </summary>
        public string macaddress {  get; set; }
        /// <summary>
        /// 会员注册时间
        /// </summary>
        public string createtime { get; set; }
        /// <summary>
        /// 当前代理
        /// </summary>
        public string affaccount { get; set; }
        /// <summary>
        /// 会员级别
        /// </summary>
        public string levels { get; set; }
    }



    /// <summary>
    /// API Model Validation
    /// </summary>
    public class SearchUserBaseInfoValidator : AbstractValidator<SearchUserBaseInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchUserBaseInfoValidator()
        {
            RuleFor(x => x.account).Account();
        }
    }

   
    /// <summary>
    /// 
    /// </summary>
    public class SearchUserBaseInfoTextSql
    {
        /// <summary>
        /// 查询账号基本信息 SQL
        /// </summary>
        public const string SearcharUserInfoSql = @"select a.account,u.amountactive,u.telephone,a.t_amount depositamount,iif(a.depositnumber is not null, a.depositnumber,0) depositnumber,
  iif(b.withdrawalnumber is not null, b.withdrawalnumber, 0) withdrawalnumber, iif(b.w_amount is not null, w_amount, 0) withdrawalamount ,
  ue.riskRemark,ue.vipStatusRemark,u.createtime,u.macaddress,u.affaccount, ug.levels
from (
 select aa.account,sum(aa.t_amount) t_amount,count(id) depositnumber from (
	 select id,account,amount t_amount from ofDeposit
	where account=@account and status=1
	union all
	select id,account,amount t_amount from ofDeposit2014
	where account=@account and status=1) aa
	group by aa.account
) a
left join (
select account,sum(amount) w_amount,count(id) withdrawalnumber from ofWithdrawal
where account=@account and status=2
group by account) b on a.account=b.account
left join ofUser u with(nolock) on a.account=u.account
left join ofUserExtention ue with(nolock) on a.account=ue.account
left join ofUserGroup ug with(nolock) on u.level = ug.levelnumber ";

        
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class SearchUserBaseInfoHandler : BaseWithdraw<SearchUserBaseInfo, ResponseModel<SearchUserBaseInfoReturn>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public SearchUserBaseInfoHandler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<SearchUserBaseInfoReturn> Handle(SearchUserBaseInfo request)
        {


            //开始切割金额
            var r = DapperBase.Query<SearchUserBaseInfoReturn>(SearchUserBaseInfoTextSql.SearcharUserInfoSql, new {account=request.account},
                        commandTimeout: 1500, ConnectionString: CMS.Data.MainDB.WITHWRITE_CONNECTIONSTRING);
            if (r != null)
            {
                return new ResponseModel<SearchUserBaseInfoReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "操作成功",
                    Data = r.First()
                };
            }
            else
            {
                return new ResponseModel<SearchUserBaseInfoReturn>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "操作失败",
                    Data = new SearchUserBaseInfoReturn()
                };
            }

        }

    }
}
