using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.Application.Account.Queries;
using CMS.Core.ViewModel;
using CMS.Data.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using DBLibs;
using DBLibs.WXDapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Org.BouncyCastle.Ocsp;
using ShareEntity.Models;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.ValidationAttr;
using static CMS.Core.Common;

namespace CMS.Core.Application.账号管理.Queries
{


    #region 新 未测试
    ///// <summary>
    ///// MediatR Request that returns a List of LBModel
    ///// </summary>
    //public class 关联搜索 : PagingModel, IRequest<ResponseModel<PagingModel<关联搜索Return>>>
    //{
    //    public string account { get; set; }
    //    public string ip { get; set; }
    //    public string uid { get; set; }
    //    public string location { get; set; }
    //    public string fingerprintlist { get; set; }
    //    public DateTime createtime { get; set; }
    //    public string Admin { get; set; }
    //    public string remark { get; set; }
    //    public List<int> currencys { get; set; }
    //    public int currencycode { get; set; }
    //}

    //public class 关联搜索Return : ofLog
    //{
    //    public string eventType { get; set; }
    //    public string location { get; set; }
    //    public long TotalItems { get; set; }
    //    public int riskTypesAready { set; get; }
    //    /// <summary>
    //    /// ofuser.id
    //    /// </summary>
    //    public long uid { set; get; }

    //}

    ///// <summary>
    ///// API Model Validation
    ///// </summary>
    //public class 关联搜索Validator : AbstractValidator<关联搜索>
    //{
    //    public 关联搜索Validator()
    //    {
    //        RuleFor(x => x.account).Account();
    //        RuleFor(x => x.uid).Account();
    //        RuleFor(x => x.ip).IP();
    //        //RuleFor(x => x.PageSize).PageSize();


    //    }
    //}

    ///// <summary>
    ///// Request Handler
    ///// </summary>
    //public class 关联搜索Handler : BaseRequestHandler<关联搜索, ResponseModel<PagingModel<关联搜索Return>>>
    //{


    //    public 关联搜索Handler(IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
    //        : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger, env) { }


    //    protected override ResponseModel<PagingModel<关联搜索Return>> Handle(关联搜索 request)
    //    {
    //        if (!request.currencys.Contains(request.currencycode))
    //        {
    //            return new ResponseModel<PagingModel<关联搜索Return>>()
    //            {
    //                code = ShareEntity.Global.ResponseCode.code.FAIL,
    //                message = "请选择货币查询"
    //            };
    //        }

    //        var remarks = string.IsNullOrEmpty(request.remark) ? "" : request.remark.Replace("'", "''");

    //        #region Vin 操作查询


    //        string whereSql = string.Empty;
    //        var viewName = " ofLog a ";
    //        string orderBy = " a.createtime desc";
    //        string keyName = " a.account ";


    //        if (!string.IsNullOrEmpty(request.uid))
    //        {
    //            var account = AccountBase.QueryAccountById(Convert.ToInt64(request.uid));
    //            request.account = string.IsNullOrEmpty(account) ? request.account : account;
    //        }

    //        if (!string.IsNullOrEmpty(request.account))
    //        {
    //            var accounts = "";
    //            if (request.account.Contains(","))
    //            {
    //                var ants = request.account.Split(',');
    //                foreach (var ant in ants)
    //                {
    //                    accounts = string.IsNullOrEmpty(accounts)
    //                        ? string.Format(@"'{0}'", ant)
    //                        : string.Format(@"{0},'{1}'", accounts, ant);
    //                }
    //            }
    //            else
    //                accounts = string.Format(@"'{0}'", request.account);

    //            whereSql = string.Format(@" a.account in ({0}) ", accounts);
    //        }

    //        if (request.currencycode > 0)
    //        {
    //            whereSql = string.IsNullOrEmpty(whereSql) 
    //                ? string.Format(@" a.currencycode = {0} ", request.currencycode) 
    //                : string.Format(@" {0} and a.currencycode = {1} ", whereSql, request.currencycode);
    //        }

    //        if (!string.IsNullOrEmpty(request.fingerprintlist))
    //        {

    //            var fids = "";
    //            if (request.fingerprintlist.Contains(","))
    //            {
    //                var fds = request.fingerprintlist.Split(',');
    //                foreach (var fid in fds)
    //                {
    //                    fids = string.IsNullOrEmpty(fids)
    //                        ? string.Format(@"'{0}'", fid)
    //                        : string.Format(@"{0},'{1}'", fids, fid);
    //                }
    //            }
    //            else
    //                fids = string.Format(@"'{0}'", request.fingerprintlist);

    //            whereSql = string.Format(@" a.comment in ({0}) ", fids);

    //        }


    //        if (!string.IsNullOrEmpty(request.remark))
    //        {
    //            whereSql = string.IsNullOrEmpty(whereSql) 
    //                ? string.Format(@" a.remark like '%{0}%' ", request.remark) 
    //                : string.Format(@" {0} and a.remark like '%{1}%' ", whereSql, request.remark);
    //        }


    //        if (!string.IsNullOrEmpty(request.ip))
    //        {
    //            var ipaddress = "";
    //            if (request.ip.Contains(","))
    //            {
    //                var ips = request.ip.Split(',');
    //                foreach (var ip in ips)
    //                {
    //                    ipaddress = string.IsNullOrEmpty(ipaddress)
    //                        ? string.Format(@"'{0}'", ip)
    //                        : string.Format(@"{0},'{1}'", ipaddress, ip);
    //                }
    //            }
    //            else
    //                ipaddress = string.Format(@"'{0}'", request.ip);

    //            whereSql = string.Format(@" a.ip in ({0}) ", ipaddress);


    //        }

    //        whereSql = string.IsNullOrEmpty(whereSql)
    //            ? string.Format(@" a.createtime>'2020-01-01 00:00:00' and a.event < 1000 ")
    //            : string.Format(@" {0} and a.createtime>'2020-01-01 00:00:00' and a.event < 1000 ", whereSql);


    //        var leftjoinString = " left join ofUser b on a.account = b.account  " +
    //            " left join ofUserExtention ue WITH(NOLOCK) on a.account=ue.account ";

    //        var fields = " a.account,a.ip,a.event a._event,a.comment,a.createtime ,a.l_device,a.event_type,a.remark,a.appDeviceId,a.appDeviceBrand,a.appDeviceModel,a.devType, ue.*, b.id ";

    //        var s =  DapperPaging<关联搜索Return>.QueryPageSync(whereSql, viewName, leftjoinString,
    //            fields, orderBy, keyName, request.PageSize, request.Page);

    //        var r = s as ApiResultModel<PageModel<关联搜索Return>>;
    //        if(r.rCode == 1)
    //        {
    //            //取到数据
    //            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
    //            {
    //                //var rows = connection.Query<关联搜索Return>(template.RawSql, template.Parameters);
    //                //var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
    //                var adminAccess = connection.Get<ofManage>(request.Admin).access;
    //                bool showTelephone = false;
    //                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;

    //                if (r.data.modelList.Count() > 0)
    //                    r.data.modelList.ToList().ForEach(x => {

    //                        x.location = Network.GetLocation(x.ip, _env.ContentRootPath + "//qqwry.dat");
    //                        if (!showTelephone)
    //                        {
    //                            if (UtilsLib.Common.IsIncludedTel(x.account))
    //                                x.account = Common.Security.ObscureString(x.account);
    //                        }
    //                        /// 0 = PC successful Login
    //                        /// 1 = PC wrong password
    //                        /// 2 = PC locked account too any retries
    //                        /// 3 = PC changed to 普通等级
    //                        /// 4 = 全网APP successful Login
    //                        /// 5 = 全网APP wrong password
    //                        /// 6 = 全网APP locked account too any retries
    //                        /// 7 = 全网APP changed to 普通等级
    //                        /// 8 = 体育APP successful Login
    //                        /// 9 = 体育APP wrong password
    //                        /// 10 = 体育APP locked account too any retries
    //                        /// 11 = 体育APP changed to 普通等级
    //                        switch (x._event)
    //                        {
    //                            case 0: x.eventType = "【PC】登入成功"; break;
    //                            case 1: x.eventType = "【PC】密码错误"; break;
    //                            case 2: x.eventType = "【PC】多次错误-锁"; break;
    //                            case 3: x.eventType = "【PC】等级自动升级"; break;
    //                            case 4: x.eventType = "【APP】登入成功"; break;
    //                            case 5: x.eventType = "【APP】密码错误"; break;
    //                            case 6: x.eventType = "【APP】多次错误-锁"; break;
    //                            case 7: x.eventType = "【APP】等级自动升级"; break;
    //                            case 8: x.eventType = "【体育】登入成功"; break;
    //                            case 9: x.eventType = "【体育】密码错误"; break;
    //                            case 10: x.eventType = "【体育】多次错误-锁"; break;
    //                            case 11: x.eventType = "【体育】等级自动升级"; break;
    //                            case 1000: x.eventType = "【*后台*】登入成功"; break;
    //                            default:
    //                                break;
    //                        }
    //                        x.eventType = x.remark;

    //                    });

    //                return new ResponseModel<PagingModel<关联搜索Return>>()
    //                {
    //                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
    //                    message = "",
    //                    Data = new PagingModel<关联搜索Return>()
    //                    {
    //                        Data = r.data.modelList,
    //                        TotalItems = r.data.TotalRecord
    //                    }
    //                };
    //            }
    //        }
    //        else
    //        {
    //            return new ResponseModel<PagingModel<关联搜索Return>>()
    //            {
    //                code = ShareEntity.Global.ResponseCode.code.FAIL,
    //                message = "",
    //                Data = new PagingModel<关联搜索Return>()
    //                {
    //                    Data = new List<关联搜索Return>(),
    //                    TotalItems = 0
    //                }
    //            };
    //        }

    //        #endregion



    //    }





    //}

    #endregion


    #region 旧 

    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 关联搜索 : PagingModel, IRequest<ResponseModel<PagingModel<关联搜索Return>>>
    {
        public string account { get; set; }
        public string ip { get; set; }
        public string uid { get; set; }
        public string location { get; set; }
        public string fingerprintlist { get; set; }
        public DateTime createtime { get; set; }
        public string Admin { get; set; }
        public string remark { get; set; }
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }

    public class 关联搜索Return : ofLog
    {
        public string eventType { get; set; }
        public string location { get; set; }
        public long TotalItems { get; set; }
        public int riskTypesAready { set; get; }
        /// <summary>
        /// ofuser.id
        /// </summary>
        public long uid { set; get; }

    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 关联搜索Validator : AbstractValidator<关联搜索>
    {
        public 关联搜索Validator()
        {
            RuleFor(x => x.account).Account();
            RuleFor(x => x.uid).Account();
            RuleFor(x => x.ip).IP();
            //RuleFor(x => x.PageSize).PageSize();


        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 关联搜索Handler : BaseRequestHandler<关联搜索, ResponseModel<PagingModel<关联搜索Return>>>
    {


        public 关联搜索Handler(IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
            : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger, env) { }


        protected override ResponseModel<PagingModel<关联搜索Return>> Handle(关联搜索 request)
        {
            if (!request.currencys.Contains(request.currencycode))
            {
                return new ResponseModel<PagingModel<关联搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }

            var remarks = string.IsNullOrEmpty(request.remark) ? "" : request.remark.Replace("'", "''");

            #region Vin 操作查询

            // 创建DAPPER TEMPLATE
            var builder = new SqlBuilder();

            bool withWhere = false;

            if (!string.IsNullOrEmpty(request.uid))
            {
                var account = AccountBase.QueryAccountById(Convert.ToInt64(request.uid));
                request.account = string.IsNullOrEmpty(account) ? request.account : account;
            }


            if (request.currencycode > 0)
            {
                builder.Where("currencycode = @currencycode", new { currencycode = request.currencycode });
            }

            if (!string.IsNullOrEmpty(request.account))
            {
                builder.Where("account IN @account",
                    new { account = request.account.Contains(",") ? request.account.Split(',') : new string[] { request.account } });
                withWhere = true;
            }

            if (!string.IsNullOrEmpty(request.location))
            {
                builder.Where("account IN @account",
                    new { account = request.account.Contains(",") ? request.account.Split(',') : new string[] { request.account } });
                withWhere = true;
            }

            if (!string.IsNullOrEmpty(request.fingerprintlist))
            {
                builder.Where("comment IN @comments",
                    new { comments = request.fingerprintlist.Contains(",") ? request.fingerprintlist.Split(',') : new string[] { request.fingerprintlist } });
                withWhere = true;

            }


            if (!string.IsNullOrEmpty(request.remark)) builder.Where("remark LIKE @remark", new { remark = string.Concat("%", request.remark, "%") });



            if (!string.IsNullOrEmpty(request.ip))
            {
                builder.Where("ip IN @ip",
                    new { ip = request.ip.Contains(",") ? request.ip.Split(',') : new string[] { request.ip } });
                withWhere = true;

            }

            builder.Where("event < 1000");


            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new
            {
                Offset = offset,
                Next = request.PageSize
            });
            builder.OrderBy(" createtime DESC");


            SqlBuilder.Template template;
            if (!withWhere)
            {
                template = builder.AddTemplate("" +
                "SELECT a.*,ue.*, u.id FROM (SELECT account,ip,event _event,comment,createtime ,l_device,event_type,remark,appDeviceId,appDeviceBrand,appDeviceModel,devType, TotalItems = 10000 " +
                "FROM ofLog WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY) a " +
                "left join ofUser u WITH(NOLOCK) on a.account=u.account " +
                "left join ofUserExtention ue WITH(NOLOCK) on a.account=ue.account ");

                //builder.Where("id > 417120");  appDeviceId,appDeviceBrand,appDeviceModel,devType
            }
            else
                template = builder.AddTemplate("" +
                "SELECT a.*,ue.*, u.id " +
                "FROM (SELECT account,ip,event _event,comment,createtime,l_device,event_type,remark,appDeviceId,appDeviceBrand,appDeviceModel,devType, COUNT(*) OVER() as TotalItems " +
                "FROM ofLog WITH(NOLOCK) /**where**/ /**orderby**/ " +
                "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                "FETCH NEXT @Next ROWS ONLY) a " +
                "left join ofUser u WITH(NOLOCK) on a.account=u.account " +
                "left  join ofUserExtention ue WITH(NOLOCK) on a.account=ue.account " +
                "ORDER BY a.createtime DESC");

            //builder.OrderBy("createtime DESC");

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING))
            {
                var rows = connection.Query<关联搜索Return>(template.RawSql, template.Parameters);
                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;
                var adminAccess = connection.Get<ofManage>(request.Admin).access;
                bool showTelephone = false;
                if (adminAccess.Contains("/api/Account/AllowToSeePhone")) showTelephone = true;

                if (rows.Count() > 0)
                    rows.ToList().ForEach(x =>
                    {

                        x.location = Network.GetLocation(x.ip, _env.ContentRootPath + "//qqwry.dat");
                        if (!showTelephone)
                        {
                            if (UtilsLib.Common.IsIncludedTel(x.account))
                                x.account = Common.Security.ObscureString(x.account);
                        }
                        /// 0 = PC successful Login
                        /// 1 = PC wrong password
                        /// 2 = PC locked account too any retries
                        /// 3 = PC changed to 普通等级
                        /// 4 = 全网APP successful Login
                        /// 5 = 全网APP wrong password
                        /// 6 = 全网APP locked account too any retries
                        /// 7 = 全网APP changed to 普通等级
                        /// 8 = 体育APP successful Login
                        /// 9 = 体育APP wrong password
                        /// 10 = 体育APP locked account too any retries
                        /// 11 = 体育APP changed to 普通等级
                        switch (x._event)
                        {
                            case 0: x.eventType = "【PC】登入成功"; break;
                            case 1: x.eventType = "【PC】密码错误"; break;
                            case 2: x.eventType = "【PC】多次错误-锁"; break;
                            case 3: x.eventType = "【PC】等级自动升级"; break;
                            case 4: x.eventType = "【APP】登入成功"; break;
                            case 5: x.eventType = "【APP】密码错误"; break;
                            case 6: x.eventType = "【APP】多次错误-锁"; break;
                            case 7: x.eventType = "【APP】等级自动升级"; break;
                            case 8: x.eventType = "【体育】登入成功"; break;
                            case 9: x.eventType = "【体育】密码错误"; break;
                            case 10: x.eventType = "【体育】多次错误-锁"; break;
                            case 11: x.eventType = "【体育】等级自动升级"; break;
                            case 1000: x.eventType = "【*后台*】登入成功"; break;
                            default:
                                break;
                        }
                        x.eventType = x.remark;

                    });

                return new ResponseModel<PagingModel<关联搜索Return>>()
                {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "",
                    Data = new PagingModel<关联搜索Return>()
                    {
                        Data = rows,
                        TotalItems = totalItems
                    }
                };
            }

            #endregion



        }





    }


    #endregion
}
