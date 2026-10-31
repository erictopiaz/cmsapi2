using AutoMapper;
using AWC2019.Core.Validation;
using CMS.Core.ViewModel;
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

namespace CMS.Core.Application.存款管理.Queries {
    
    public class 第三方代理Query : 第三方代理QueryBase, IRequest<ResponseModel<PagingModel<第三方代理Return>>> {
        [JsonIgnore]
        public List<int> currencys { get; set; }
        public int currencycode { get; set; }
    }
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class 第三方代理QueryBase : PagingModel {
        //public string password { get; set; }

        public string Account { get; set; }
        public string OrderNo { get; set; } //商户单号
        //public string MerchantOrdernNo { get; set; } // 银行单号
        public string Platform { get; set; } // 支付类型
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public int? level { get; set; } = -1;
        public int? paycategory { get; set; } = -1;

        public int samename { get; set; } = 0;// 支付状态
        public int Status { get; set; } // 支付状态
        public int promostatus { get; set; } // 支付状态
        public int depositCategory { get; set; } // 支付状态
        public string realname { get; set; }  // 存款IP
        public string ipaddress { get; set; }  // 存款IP
        public string amount { get; set; }  // 存款IP
        public string Affiliate { get; set; } // 代理
        //public int Website { get; set; } = -1; // 网址
        public string Admin { get; set; }
        // 7	虚拟币
        // 13	C2C钱包
        // 1	网银自动
        // 9	极速网银
        //public int paycategory { get; set; }
    }

    public class 第三方代理Return {
        public int depositCategory { get; set; } // 支付状态
        public float self_fee { get; set; }
        public int crccheck { get; set; }

        public string submitted_realname { get; set; }
        public int same_realname { get; set; }
        public string crc { get; set; }
        public long id { get; set; }
        public string currency { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
        public decimal amount { get; set; }
        public decimal fee { get; set; }
        public decimal rebate { get; set; }
        public int? deposittype { get; set; }
        public string depositmethod { get; set; }
        public string additional { get; set; }
        public string receiveperson { get; set; }
        public string receivecardno { get; set; }
        public DateTime? deposittime { get; set; }
        public DateTime requesttime { get; set; }
        public string bankname { get; set; }
        public DateTime? reviewtime { get; set; }
        public double minutesdiff { get; set; }
        public double withdrawconformed_minutesdiff { get; set; }
        public string reviewperson { get; set; }
        public string merchantno { get; set; }
        public string bankorderno { get; set; }
        public DateTime? orderendtime { get; set; }
        public int status { get; set; }
        public string referrer { get; set; }
        public string referrerreg { get; set; }
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string supplement { get; set; }
        public string referenceguid { get; set; }
        public long? payplatform { get; set; }
        public string payplatformname { get; set; }
        public int alert { get; set; }
        public string affaccount { get; set; }
        public string levelname { get; set; }
        public int joinbonus { get; set; }
        public int level { get; set; }
        public int bonusbet { get; set; }

        public long TotalItems { get; set; }

        public decimal TotalUSDT { get; set; }
        public decimal TotalFee { get; set; }
        public decimal depositpoints { get; set; }
        public decimal TotalRebate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool? VerifyFailedRequestWithAlterAmount { get; set; } = false;
        public bool? VerifyFailedRequest { get; set; } = false;

        public string amountString { get; set; }
        public string misc { get; set; }
        public string respresult { get; set; }
        public string bonuspromocode { get; set; }
        public int device { get; set; }
        public int imgupload { get; set; }
        public int riskTypesAready { get; set; }
        //public int ofwithdrawal_conformedstatus { get; set; }
        public int withdrawal_conformedstatus { get; set; }
        public DateTime bonuspromo_createtime { get; set; }
        public float bonuspromo_amount { get; set; }

        public int alreadyResizeTimes { get; set; }


    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class 第三方代理QueryValidator : AbstractValidator<第三方代理Query> {
        public 第三方代理QueryValidator() {

            RuleFor(x => x.Account).Account();
            RuleFor(x => x.OrderNo).CharAndDigitalNullable(20, 40);
            RuleFor(x => x.Status).Status();
            RuleFor(x => x.realname).Realname();
            RuleFor(x => x.ipaddress).IP();
            //RuleFor(x => x.amount).AmountStr();
            RuleFor(x => x.Affiliate).Affiliate();

            //RuleFor(x => x.Account).Account();
            //RuleFor(x => x.CardNo).CardNo();
            //RuleFor(x => x.Bank).CharAndDigital(1, 10);
            //RuleFor(x => x.VerifyTime).Datetime();
            //RuleFor(x => x.Status).Status();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class 第三方代理Handler : BaseRequestHandler<第三方代理Query, ResponseModel<PagingModel<第三方代理Return>>> {
        public 第三方代理Handler(IMapper mapper, ILoggerFactory logger) : base(CMS.Data.MainDB.CONNECTIONSTRING, mapper, logger) { }

        protected override ResponseModel<PagingModel<第三方代理Return>> Handle(第三方代理Query request) {


            if (!request.currencys.Contains(request.currencycode)) {
                return new ResponseModel<PagingModel<第三方代理Return>>() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }


            if (request.Status == 5) {
                return DepositFailedLog(request);
            }
            // 创建DAPPER TEMPLATE 
            var builder = new SqlBuilder();

            bool withWhere = false;
            bool withTimeRange = false;



            if (request.paycategory >= 0) {
                switch (request.paycategory) {
                    case 7:
                        builder.Where("ofdeposit.bankname LIKE @bankname", new { bankname = "USDT%" });
                        break;
                    case 0:
                        builder.Where("ofdeposit.bankname LIKE @bankname", new { bankname = "USDT%" });
                        break;
                    default:
                        break;
                }
            }



            // Check Range
            if (request.starttime != null) {
                if ((DateTime.UtcNow.AddHours(8) - (DateTime)request.starttime).TotalDays > 31) {

                    var result = new ResponseModel<PagingModel<第三方代理Return>>() {
                        code = ShareEntity.Global.ResponseCode.code.FAIL,
                        message = "不允许搜索超过一个月",
                        Data = null
                    };
                }
            }


            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null) {
                if (request.Status == 1) {
                    if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account = @account", new { account = request.Account });
                    builder.Where("requesttime >= @starttime AND requesttime  <= @endtime", new { request.starttime, request.endtime });
                }
                else {
                    builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                    if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account = @account", new { account = request.Account });
                }
                withWhere = true;
                withTimeRange = request.Status == 1;
            }
            else if (request.starttime != null) {
                withWhere = true;
                withTimeRange = request.Status == 1;
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
                if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account = @account", new { account = request.Account });
                //if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account LIKE @account", new { account = string.Concat("%", request.Account, "%") });
            }
            else // Limit to 3 Months
            {
                withWhere = true;
                builder.Where("requesttime >= @starttime", new { starttime = Common.CurrentTime.AddDays(-3) });
                if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account LIKE @account", new { account = string.Concat("%", request.Account, "%") });
            }




            //if (request.samename == 0)

            // FILL UP THE WHERE CLAUSE

            if (request.currencycode > 0) { builder.Where("ofdeposit.currencycode = @currencycode", new { request.currencycode }); }

            if (!string.IsNullOrEmpty(request.OrderNo)) { withWhere = true; builder.Where("merchantno = @OrderNo", new { request.OrderNo }); }

            if (!string.IsNullOrEmpty(request.amount)) {
                float amountstart = 0;
                if (request.amount.Contains("-")) {
                    var amountarray = request.amount.Split('-');
                    amountstart = float.Parse(amountarray[0]);
                    float amountend = float.Parse(amountarray[1]);
                    withWhere = true; builder.Where("amount between @amountstart and @amountend", new { amountstart = amountstart, amountend = amountend });
                }
                else if (float.TryParse(request.amount, out amountstart)) {
                    withWhere = true; builder.Where("amount = @Amount", new { Amount = amountstart });
                }

            }



            if (!string.IsNullOrEmpty(request.realname) && request.realname == "*") {
                withWhere = true; builder.Where("ofdeposit.misc NOT LIKE '%' + ofUser.realname + '%' ");
            }
            else if (!string.IsNullOrEmpty(request.realname)) {
                withWhere = true; builder.Where("ofdeposit.misc LIKE @REALNAME", new { REALNAME = string.Concat("%", request.realname, "%") });
            }



            if (!string.IsNullOrEmpty(request.Platform)) {
                withWhere = true;
                var platformIDs = new List<string>();
                foreach (var item in CMS.Core.CachedData.PayPlatformDict) {
                    if (item.Value.Contains(request.Platform) && item.Value != "ATM汇款")
                        platformIDs.Add(item.Key);
                }
                builder.Where("payplatform IN @platformIDs", new { platformIDs });

            }

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            var adminAccess = CachedData.AccessString(request.Admin);
            if (adminAccess.IndexOf("/api/Admin/QueryAgent", StringComparison.OrdinalIgnoreCase) >= 0)//账号已经被控了，只能看到自己开的代理和代理下线
            {
                withWhere = true; builder.Where("ofdeposit.affaccount in (select account from ofAffiliate where verifyperson=@admin)", new { admin = request.Admin });

            }

            if (request.depositCategory >= 0) { withWhere = true; builder.Where("ofdeposit.depositCategory=@depositCategory", new { request.depositCategory }); }
            if (request.promostatus > 0) { withWhere = true; builder.Where("ofdeposit.bonuspromocode >0 "); }
            if (request.promostatus == 0) { withWhere = true; builder.Where("ofdeposit.bonuspromocode=0"); }
            if (!string.IsNullOrEmpty(request.ipaddress)) { withWhere = true; builder.Where("ofdeposit.ipaddress = @ipaddress", new { request.ipaddress }); }
            if (!string.IsNullOrEmpty(request.Affiliate)) { withWhere = true; builder.Where("ofdeposit.affaccount = @Affiliate", new { Affiliate = request.Affiliate }); }
            builder.Where("deposittype = 0");
            if (request.level > 0) { withWhere = true; builder.Where("level = @LEVEL", new { LEVEL = request.level }); }

            if (request.Status >= 0) {
                withWhere = true;

                if (request.Status == 2) {
                    builder.Where("reviewperson IS NULL");
                }

                //查询补单
                if (request.Status == 100) {
                    withTimeRange = true;
                    builder.Where("merchantno LIKE '补单%'");
                }
                else
                    builder.Where("ofDeposit.Status = @Status", new { request.Status });
            }


            switch (request.samename) {
                case 0:
                    builder.Where("ofDeposit.misc LIKE '%posted_realname%'");
                    builder.Where("ofDeposit.realname <> JSON_VALUE(.ofDeposit.misc,'$.posted_realname')"); break;
                case 1:
                    builder.Where("ofDeposit.realname == JSON_VALUE(ofDeposit.misc,'$.posted_realname')"); break;
                default:
                    break;
            }

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            builder.OrderBy("ofDeposit.ID DESC");


            SqlBuilder.Template template;
            if (withWhere && withTimeRange && request.paycategory == 7) {

                template = builder.AddTemplate("" +
                            "SELECT wd.conformedstatus AS withdrawal_conformedstatus,bp.createtime AS bonuspromo_createtime,fixpromop AS bonuspromo_amount, " +
                            "depositcategory,alreadyResizeTimes,self_fee,a.bonuspromocode, imgupload,a.realname,crc,c.riskTypesAready,device,a.bankname,a.misc,TotalAmount,TotalItems," +
                            "TotalUSDT,TotalFee,TotalRebate, ISNULL(b.points, depositpoints) as depositpoints,additional,a.id AS id, " +
                            "a.account AS account,a.ipaddress AS ipaddress,a.amount AS amount,a.requesttime AS requesttime ,a.orderendtime AS orderendtime,a.merchantno AS merchantno," +
                            "a.status AS status,a.payplatform AS payplatform,a.fee AS fee,a.rebate AS rebate,a.level AS level,a.reviewperson AS reviewperson FROM (SELECT depositcategory, alreadyResizeTimes," +
                            "fixpromop,self_fee,crc,ofdeposit.realname AS realname,device,bankname,ofdeposit.misc AS misc,imgupload,depositpoints,additional,ofdeposit.id AS id, " +
                            "ofdeposit.account AS account,ofdeposit.ipaddress AS ipaddress,amount,requesttime,orderendtime,merchantno,ofdeposit.status AS status," +
                            "payplatform,fee,rebate,level,reviewperson,bonuspromocode, " +
                                "COUNT(*) OVER() as TotalItems, " +
                                "SUM(amount) OVER() as TotalAmount, " +
                                "sum(ROUND(amount / CAST(JSON_VALUE(ofDeposit.misc,'$.usdt_erc20_xrate') AS float),2)) OVER() as TotalUSDT, " +
                                "SUM(fee) OVER() as TotalFee, " +
                                "SUM(rebate) OVER() as TotalRebate " +
                            "FROM ofDeposit WITH(NOLOCK) inner join ofuser WITH(NOLOCK) on ofdeposit.account=ofuser.account /**where**/ /**orderby**/ " +
                            "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                            "FETCH NEXT @Next ROWS ONLY)a " +
                            "LEFT JOIN ofPromoMisc b WITH(NOLOCK) ON a.merchantno = b.orderno " +
                            "LEFT JOIN ofcrosstransact ct WITH(NOLOCK) ON a.merchantno = ct.orderid " +
                            "LEFT JOIN ofwithdrawal wd WITH(NOLOCK) ON ct.withdrawid = wd.id " +
                            "outer apply (select top 1 createtime, remarks from ofBonuspromo bb WITH(NOLOCK) WHERE wd.referenceguid = bb.referenceguid) bp " +
                            //"LEFT JOIN ofBonusPromo bp WITH(NOLOCK) ON bp.referenceguid = wd.referenceguid " +
                            "LEFT Join ofuserExtention c on c.account=a.account");

            }
            else if (withWhere && withTimeRange) {
                template = builder.AddTemplate("" +
                    "SELECT wd.conformedstatus AS withdrawal_conformedstatus,bp.createtime AS bonuspromo_createtime,bp.remarks AS bonuspromo_remarks,depositcategory,alreadyResizeTimes,self_fee, " +
                    "a.bonuspromocode, imgupload,a.realname,crc,c.riskTypesAready,device,a.bankname,a.misc,TotalAmount,TotalItems,TotalFee,TotalRebate, ISNULL(b.points, depositpoints) as depositpoints,additional,a.id AS id, " +
                    "a.account AS account,a.ipaddress AS ipaddress,a.amount AS amount,a.requesttime AS requesttime ,a.orderendtime AS orderendtime,a.merchantno AS merchantno," +
                    "a.status AS status,a.payplatform AS payplatform,a.fee AS fee,a.rebate AS rebate,a.level AS level,a.reviewperson AS reviewperson FROM (SELECT depositcategory,alreadyResizeTimes,self_fee,crc,ofdeposit.realname AS realname,device,bankname,ofdeposit.misc AS misc,imgupload,depositpoints,additional,ofdeposit.id AS id, ofdeposit.account AS account,ofdeposit.ipaddress AS ipaddress," +
                    "amount,requesttime,orderendtime,merchantno,ofdeposit.status AS status,payplatform,fee,rebate,level,reviewperson,bonuspromocode, " +
                        //"(SELECT COUNT(*) FROM ofDeposit WITH(NOLOCK) /**where**/ ) AS TotalItems, " +
                        //"(SELECT SUM(amount) FROM ofDeposit WITH(NOLOCK) /**where**/ ) AS TotalAmount, " +
                        //"(SELECT SUM(fee) FROM ofDeposit WITH(NOLOCK) /**where**/ ) AS TotalFee " +
                        "TotalUSDT = 0, " +
                        "COUNT(*) OVER() as TotalItems, " +
                        "SUM(amount) OVER() as TotalAmount, " +
                        "SUM(fee) OVER() as TotalFee, " +
                        "SUM(rebate) OVER() as TotalRebate " +
                    "FROM ofDeposit WITH(NOLOCK) inner join ofuser WITH(NOLOCK) on ofdeposit.account=ofuser.account /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                    "FETCH NEXT @Next ROWS ONLY)a " +
                    "LEFT JOIN ofPromoMisc b WITH(NOLOCK) ON a.merchantno = b.orderno " +
                    "LEFT JOIN ofcrosstransact ct WITH(NOLOCK) ON a.merchantno = ct.orderid " +
                    "LEFT JOIN ofwithdrawal wd WITH(NOLOCK) ON ct.withdrawid = wd.id " +
                    "outer apply (select top 1 createtime, remarks from ofBonuspromo bb WITH(NOLOCK) WHERE wd.referenceguid = bb.referenceguid) bp " +
                    //"LEFT JOIN ofBonusPromo bp WITH(NOLOCK) ON bp.referenceguid = wd.referenceguid " +
                    "LEFT Join ofuserExtention c on c.account=a.account");
                //"SELECT * , " +
                //    "COUNT(*) OVER() as TotalItems, " +
                //    "SUM(amount) OVER() as TotalAmount, " +
                //    "SUM(fee) OVER() as TotalFee " +
                //"FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
                //"OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                //"FETCH NEXT @Next ROWS ONLY");
            }
            else if (withWhere) {
                template = builder.AddTemplate("" +
                    "SELECT  wd.conformedstatus AS withdrawal_conformedstatus,bp.createtime AS bonuspromo_createtime,bp.remarks AS bonuspromo_remarks,depositcategory,alreadyResizeTimes,self_fee,a.bonuspromocode,imgupload,a.realname,crc,c.riskTypesAready,device,respresult,a.bankname,a.misc,TotalItems, ISNULL(b.points, depositpoints) as depositpoints," +
                    "additional,a.id AS id, a.account AS account,a.ipaddress AS ipaddress,a.amount AS amount,a.requesttime AS requesttime ,a.orderendtime AS orderendtime," +
                    "a.merchantno AS merchantno,a.status AS status,a.payplatform AS payplatform,a.fee AS fee,a.rebate AS rebate,a.level AS level,a.reviewperson AS reviewperson " +
                    "FROM ((SELECT depositcategory,alreadyResizeTimes,self_fee,crc,ofdeposit.realname AS realname,device,respresult,bankname,ofdeposit.misc AS misc,depositpoints,additional,ofdeposit.id AS id, ofdeposit.account AS account,ofdeposit.ipaddress AS ipaddress, amount,imgupload,requesttime,orderendtime,merchantno,ofdeposit.status AS status,payplatform,fee,rebate,level,reviewperson,bonuspromocode , " +
                        //"TotalItems=10000 " +
                        "(SELECT COUNT(*) FROM ofDeposit WITH(NOLOCK) /**where**/ ) AS TotalItems " +
                    //"SUM(amount) OVER() as TotalAmount, " +
                    //"SUM(fee) OVER() as TotalFee " +
                    "FROM ofDeposit WITH(NOLOCK) inner join ofuser WITH(NOLOCK) on ofdeposit.account=ofuser.account /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                    "FETCH NEXT @Next ROWS ONLY)a " +
                    "LEFT JOIN ofPromoMisc b WITH(NOLOCK) ON a.merchantno = b.orderno) " +
                    "LEFT JOIN ofcrosstransact ct WITH(NOLOCK) ON a.merchantno = ct.orderid " +
                    "LEFT JOIN ofwithdrawal wd WITH(NOLOCK) ON ct.withdrawid = wd.id " +
                    "outer apply (select top 1 createtime,remarks from ofBonuspromo bb WITH(NOLOCK) WHERE wd.referenceguid = bb.referenceguid) bp " +
                    //"LEFT JOIN ofBonusPromo bp WITH(NOLOCK) ON bp.referenceguid = wd.referenceguid " +
                    "LEFT Join ofuserExtention c on c.account=a.account");
            }

            //"SELECT * , COUNT(*) OVER() as TotalItems " +
            //"FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
            //"OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
            //"FETCH NEXT @Next ROWS ONLY");
            else {
                template = builder.AddTemplate("" +
                    "SELECT  wd.conformedstatus AS withdrawal_conformedstatus,bp.createtime AS bonuspromo_createtime,bp.remarks AS bonuspromo_remarks,depositcategory,alreadyResizeTimes,self_fee,imgupload, a.realname,crc,c.riskTypesAready,a.bankname,a.misc,TotalItems, ISNULL(b.points, depositpoints) as depositpoints,additional,a.id AS id, a.account AS account,a.ipaddress AS ipaddress,a.amount AS amount,a.requesttime AS requesttime ,a.orderendtime AS orderendtime,a.merchantno AS merchantno,a.status AS status,a.payplatform AS payplatform,a.fee AS fee,a.rebate AS rebate,a.level AS level,a.reviewperson AS reviewperson,device,respresult " +
                    "FROM ((SELECT depositcategory,alreadyResizeTimes,self_fee,crc,ofdeposit.realname AS realname,bankname,ofdeposit.misc AS misc,depositpoints,ofdeposit.id AS id,device,respresult ofdeposit.account AS account,ofdeposit.ipaddress AS ipaddress,amount,imgupload,requesttime,orderendtime,merchantno,ofdeposit.status AS status,payplatform,fee,rebate,level , " +
                        "TotalItems=10000 " +
                    //"SUM(amount) OVER() as TotalAmount, " +
                    //"SUM(fee) OVER() as TotalFee " +
                    "FROM ofDeposit WITH(NOLOCK) inner join ofuser  WITH(NOLOCK) on ofdeposit.account=ofuser.account /**where**/ /**orderby**/ " +
                    "OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                    "FETCH NEXT @Next ROWS ONLY)a LEFT JOIN ofPromoMisc b WITH(NOLOCK) ON a.merchantno = b.orderno) " +
                    "LEFT JOIN ofcrosstransact ct WITH(NOLOCK) ON a.merchantno = ct.orderid " +
                    "LEFT JOIN ofwithdrawal wd WITH(NOLOCK) ON ct.withdrawid = wd.id " +
                    "outer apply (select top 1 createtime, remarks from ofBonuspromo bb WITH(NOLOCK) WHERE wd.referenceguid = bb.referenceguid) bp " +
                    //"LEFT JOIN ofBonusPromo bp WITH(NOLOCK) ON bp.referenceguid = wd.referenceguid " +
                    "LEFT Join ofuserExtention c on c.account=a.account");

                //"SELECT * , TotalItems=10000 " +
                //"FROM ofDeposit WITH(NOLOCK) /**where**/ /**orderby**/ " +
                //"OFFSET @Offset ROWS " +                        // OFFSET Method for Paging
                //"FETCH NEXT @Next ROWS ONLY");
            }

            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var rows = connection.Query<第三方代理Return>(template.RawSql, template.Parameters);

                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                //var adminAccess = connection.Get<ofManage>(request.Admin).access;

                bool showVerifyfailedrequestButton = false;
                bool showVerifyfailedrequestwithalteramountButton = false;
                if (adminAccess.Contains("api/Deposit/VerifyFailedRequest")) showVerifyfailedrequestButton = true;
                if (adminAccess.Contains("api/Deposit/VerifyWithAlterAmountFailedRequest")) showVerifyfailedrequestwithalteramountButton = true;

                if (rows.Count() > 0) {
                    rows.ToList().ForEach(x => {
                        string amountStr = "";
                        //amountStr = amountStr.EndsWith(".00") ? amountStr.Split('.')[0] : amountStr;
                        //x.amountString = amountStr;
                        if (x.riskTypesAready == null)
                            x.riskTypesAready = 0;

                        var ofdeposit_misc = Common.Misc.FromJsonToDict(x.misc);
                        if (x.bankname.ToUpper() == "USDT" && !string.IsNullOrEmpty(x.misc) ||
                            x.bankname.ToUpper() == "USDT-ERC20" && !string.IsNullOrEmpty(x.misc) ||
                            x.bankname.ToUpper() == "USDTTRC" && !string.IsNullOrEmpty(x.misc) ||
                            x.bankname.ToUpper() == "USDT-TRC20" && !string.IsNullOrEmpty(x.misc)) {
                            if (ofdeposit_misc != null && ofdeposit_misc.ContainsKey("usdt_erc20_xrate")) {
                                var usdt_equiv = x.amount / decimal.Parse(ofdeposit_misc["usdt_erc20_xrate"]);
                                x.amountString = ofdeposit_misc["usdt_erc20_xrate"];
                                x.amountString = string.Concat(" [$", usdt_equiv.ToString("0.0000")
                                   , " @ ", Common.Misc.FromJsonToDict(x.misc)["usdt_erc20_xrate"], "]");
                            }


                        }

                        x.same_realname = 0;
                        if (ofdeposit_misc != null && ofdeposit_misc.ContainsKey("posted_realname")) {
                            x.same_realname = ofdeposit_misc["posted_realname"] == x.realname ? 1 : 2;
                            x.submitted_realname = ofdeposit_misc["posted_realname"];
                        }

                        if (ofdeposit_misc != null && ofdeposit_misc.ContainsKey("submit_amount")) {
                            x.amountString = string.Concat(x.amountString, "付", ofdeposit_misc["submit_amount"]);
                            x.same_realname = ofdeposit_misc["posted_realname"] == x.realname ? 1 : 2;
                            x.submitted_realname = ofdeposit_misc["posted_realname"];
                        }

                        x.VerifyFailedRequest = showVerifyfailedrequestButton;
                        x.VerifyFailedRequestWithAlterAmount = showVerifyfailedrequestwithalteramountButton;
                        x.payplatformname = CMS.Core.CachedData.PayPlatformDict[x.payplatform.ToString()];
                        x.levelname = CMS.Core.CachedData.LevelNameDict[x.level];
                        // If More than 3 hours. it will automatically be cancelled
                        x.crccheck = 0;
                        if (x.crc != null)
                            x.crccheck = x.crc.ToLower() == Common.Security.MD5Hash(string.Concat(x.id, "_", x.account, "_", x.amount, "_", x.payplatform)).ToLower() ? 1 : 2;

                        if (x.status == 0 && (Common.CurrentTime - x.requesttime).TotalMinutes > 180) {
                            x.status = 3;
                        }

                        if (x.status == 2 && (ofdeposit_misc != null && ofdeposit_misc.ContainsKey("admincancel") && ofdeposit_misc["admincancel"] == "true")) {
                            x.status = 4;
                        }


                        if (x.orderendtime != null) {
                            x.minutesdiff = ((DateTime)x.orderendtime - x.requesttime).TotalMinutes;
                            x.minutesdiff = Math.Floor(x.minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;

                        if (x.bonuspromo_createtime != null) {
                            x.withdrawconformed_minutesdiff = ((DateTime)x.bonuspromo_createtime - x.requesttime).TotalMinutes;
                            x.withdrawconformed_minutesdiff = Math.Floor(x.withdrawconformed_minutesdiff);
                        }
                        else
                            x.minutesdiff = -1;
                    });
                }

                //if (rows.Count() > 0)
                //    rows.ToList().ForEach(x =>
                //    {

                //    });

                var result = new ResponseModel<PagingModel<第三方代理Return>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<第三方代理Return>() {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };

                // TODO: Optimization here
                return result;
            }
        }

        private ResponseModel<PagingModel<第三方代理Return>> DepositFailedLog(第三方代理Query request) {
            var builder = new SqlBuilder();

            bool withWhere = false;
            bool withTimeRange = false;


            // FILL UP THE WHERE CLAUSE
            if (request.starttime != null && request.endtime != null) {
                builder.Where("requesttime BETWEEN @starttime AND @endtime", new { request.starttime, request.endtime });
                if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdepositlog.account = @account", new { account = request.Account });
                withWhere = true;
            }
            else if (request.starttime != null) {
                withWhere = true;
                builder.Where("requesttime >= @starttime", new { starttime = request.starttime });
                if (!string.IsNullOrEmpty(request.Account)) builder.Where("d.account = @account", new { account = request.Account });
                //if (!string.IsNullOrEmpty(request.Account)) builder.Where("ofdeposit.account LIKE @account", new { account = string.Concat("%", request.Account, "%") });
            }
            else // Limit to 3 Months
            {
                withWhere = true;
                builder.Where("requesttime >= @starttime", new { starttime = Common.CurrentTime.AddDays(-3) });
                if (!string.IsNullOrEmpty(request.Account)) builder.Where("d.account LIKE @account", new { account = string.Concat("%", request.Account, "%") });
            }

            if (request.currencycode > 0) { builder.Where("currencycode = @currencycode", new { request.currencycode }); }


            if (request.level > 0) { withWhere = true; builder.Where("b.level = @LEVEL", new { LEVEL = request.level }); }
            //if (!string.IsNullOrEmpty(request.realname)) { withWhere = true; builder.Where("realname = @REALNAME", new { REALNAME = request.realname }); }


            if (!string.IsNullOrEmpty(request.Platform)) {
                withWhere = true;
                var platformIDs = new List<string>();
                foreach (var item in CMS.Core.CachedData.PayPlatformDict) {
                    if (item.Value.Contains(request.Platform) && item.Value != "ATM汇款")
                        platformIDs.Add(item.Key);
                }
                builder.Where("payplatform IN @platformIDs", new { platformIDs });

            }

            // PAGING FUNCTION
            request.Page = request.Page == 0 ? 0 : request.Page - 1;
            int offset = request.Page * request.PageSize;
            builder.AddParameters(new { Offset = offset, Next = request.PageSize });
            //builder.OrderBy("ofdepositlog.ID DESC");


            SqlBuilder.Template template;
            {
                template = builder.AddTemplate("" +
                    "SELECT level,a.account,amount,requesttime,bankname,merchantno,ipaddress, macaddress, a.payplatform,respresult,device,TotalItems, levels as levelname FROM( " +
                    "SELECT b.level,d.account,amount,requesttime,bankname,merchantno,d.ipaddress, d.macaddress, payplatform,respresult,device, " +
                    "(SELECT COUNT(*) FROM ofDepositLog WITH(NOLOCK) /**where**/ ) AS TotalItems " +
                    "FROM ofDepositLog d WITH(NOLOCK)  " +
                    "inner join ofuser b WITH(NOLOCK) on d.account=b.account /**where**/ order by d.id desc " +
                    "OFFSET @OFFSET ROWS " +
                    "FETCH NEXT @NEXT ROWS ONLY) a " +
                    "LEFT Join ofuserExtention c with(nolock) on c.account=a.account " +
                    //"LEFT Join ofUser e with(nolock) on e.account=a.account " +
                    "LEFT Join ofUsergroup f with(nolock) on a.level=f.levelnumber"

                    );
            }


            using (SqlConnection connection = new SqlConnection(CMS.Data.MainDB.CONNECTIONSTRING)) {
                var rows = connection.Query<第三方代理Return>(template.RawSql, template.Parameters);

                rows.ToList().ForEach(x => {
                    x.payplatformname = CMS.Core.CachedData.PayPlatformDict[x.payplatform.ToString()];
                });

                var totalItems = rows.Count() == 0 ? 0 : rows.First().TotalItems;

                var result = new ResponseModel<PagingModel<第三方代理Return>>() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = "测试",
                    Data = new PagingModel<第三方代理Return>() {
                        TotalItems = totalItems,
                        Data = rows
                    }
                };

                // TODO: Optimization here
                return result;
            }
        }
    }
}
