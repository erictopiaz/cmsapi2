using ShareEntity.InputModels.Account;
using ShareEntity.InputModels.agent;
using ShareEntity.InputModels.BetRecordModel;
using ShareEntity.InputModels.Bonus;
using ShareEntity.InputModels.TurnOver;
using ShareEntity.OutModels.agent;
using ShareEntity.OutModels.Bonus;
using ShareEntity.SqlCommandText;
using DBLibs;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using ShareEntity.Agent.Finance;

namespace CMS.API.BLL.Export
{
    /// <summary>
    /// 数据导出业务处理
    /// </summary>
    public class ExportBLL
    {


        public static async Task<object> ExportBonusRecords(QueryBonusInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofBonusPromo a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account = '{0}' ", inputModel.account);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and u.affaccount = '{1}' ", whereSql, inputModel.affaccount);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (inputModel.gametype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.gametype = {0} ", inputModel.gametype) : string.Format(@" {0} and a.gametype = {1} ", whereSql, inputModel.gametype);
            
            if (inputModel.bonusPromoType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.bonusPromoType = {0} ", inputModel.bonusPromoType) : string.Format(@" {0} and a.bonusPromoType = {1} ", whereSql, inputModel.bonusPromoType);

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.reviewtime >= '{0}' and a.reviewtime <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and a.reviewtime >= '{1}' and a.reviewtime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            if (!string.IsNullOrEmpty(inputModel.remarks))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" remarks like '%{0}%' ", inputModel.remarks) : string.Format(@" {0} and remarks like '%{1}%' ", whereSql, inputModel.remarks);
            }

            var leftjoinString = string.Format(@" left join ofUserGroup b on a.level = b.levelnumber  
left join ofUser u on a.account = u.account 
left join ofAffiliateTree oat on u.affaccount = oat.agentAccount 
left join ofGameClass gc on a.gametype = gc.gameid ");
            var fields = " a.id as tid,a.account,a.amount,a.reward,a.remarks,a.createtime,a.bet,a.level,a.gametype,a.bonusPromoType,a.status,a.referenceguid,a.createperson,a.reviewperson,a.reviewtime,a.reason,a.ipaddress,b.levels,u.affaccount,oat.parentAccount,gc.gamename ";

            return await DapperPaging<ExportBonusOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }
        /// <summary>
        /// 代理提款导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentWithdrawDataFromDB(QueryAgentWithdrawInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofAffWithdraw a ";
            string orderBy = " a.requesttime desc ";
            string keyName = " a.id ";

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.Format(@" a.account in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount);
            }

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account='{0}' ", inputModel.account) : string.Format(@" {0} and a.account='{1}' ",whereSql, inputModel.account);

            if (!string.IsNullOrEmpty(inputModel.realname))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.realname=N'{0}' ", inputModel.realname) : string.Format(@" {0} and a.realname=N'{1}' ", whereSql, inputModel.realname);

            if (!string.IsNullOrEmpty(inputModel.cardno))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.cardno='{0}' ", inputModel.cardno) : string.Format(@" {0} and a.cardno='{1}' ", whereSql, inputModel.cardno);

            if (!string.IsNullOrEmpty(inputModel.parentAccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.parentAccount='{0}' ", inputModel.parentAccount) : string.Format(@" {0} and b.parentAccount='{1}' ", whereSql, inputModel.parentAccount);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status='{0}' ", inputModel.status) : string.Format(@" {0} and a.status='{1}' ", whereSql, inputModel.status);

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" requesttime >= '{0}' and requesttime <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and requesttime >= '{1}' and requesttime <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            var leftjoinString = string.Format(@" left join ofAffiliateTree b on a.account=b.agentAccount ");
            var fields = " a.*,b.parentAccount ";

            return await DapperPaging<ExportAgentWithdrawOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理结算数据导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentBillDataFromDB(QueryAgentBilledInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofAffBillReport a ";
            string orderBy = " a.createtime desc ";
            string keyName = " a.id ";
            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.Format(@" a.account in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount);
            }

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account='{0}' ", inputModel.account) : string.Format(@" {0} and a.account='{1}' ", whereSql, inputModel.account);


            if (!string.IsNullOrEmpty(inputModel.parentAccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.parentAccount='{0}' ", inputModel.parentAccount) : string.Format(@" {0} and b.parentAccount='{1}' ", whereSql, inputModel.parentAccount);

            if (!string.IsNullOrEmpty(inputModel.remark))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.remark like '%{0}%' ", inputModel.remark) : string.Format(@" {0} and a.remark like '%{1}%' ", whereSql, inputModel.remark);

            if (inputModel.startTime > 0 && inputModel.endTime > 0
                && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" billedTime >= {0} and billedTime <= {1}", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and billedTime >= {1} and billedTime <= {2}", whereSql, inputModel.startTime, inputModel.endTime);
            }
            if (inputModel.proportion != -1)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" proportion = {0} ", inputModel.proportion) : string.Format(@" {0} and proportion = {1} ", whereSql, inputModel.proportion);
            }
            if (inputModel.afpaymentlivetype != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" afpaymentlivetype = {0}", inputModel.afpaymentlivetype) : string.Format(@" {0} and afpaymentlivetype = {1}", whereSql, inputModel.afpaymentlivetype);

            var leftjoinString = string.Format(@" left join ofAffiliateTree b on a.account=b.agentAccount ");
            var fields = " a.*,b.parentAccount ";

            return await DapperPaging<ExportAgentBilledOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理日自动结算数据导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentBillAutoDataFromDB(QueryAgentBilledInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "afBillReport a ";
            string orderBy = " a.billDay desc ";
            string keyName = " a.account,a.billDay ";

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.Format(@" a.account in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount);
            }

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.account='{0}' ", inputModel.account) : string.Format(@" {0} and a.account='{1}' ", whereSql, inputModel.account);


            if (!string.IsNullOrEmpty(inputModel.parentAccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.parentAccount='{0}' ", inputModel.parentAccount) : string.Format(@" {0} and b.parentAccount='{1}' ", whereSql, inputModel.parentAccount);

            //if (!string.IsNullOrEmpty(inputModel.remark))
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.remark like '%{0}%' ", inputModel.remark) : string.Format(@" {0} and a.remark like '%{1}%' ", whereSql, inputModel.remark);

            if (inputModel.startTime > 0 && inputModel.endTime > 0
                && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" billDay >= {0} and billDay <= {1}", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and billDay >= {1} and billDay <= {2}", whereSql, inputModel.startTime, inputModel.endTime);
            }

            //if (inputModel.proportion != -1)
            //{
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" proportion = {0} ", inputModel.proportion) : string.Format(@" {0} and proportion = {1} ", whereSql, inputModel.proportion);
            //}
            //if (inputModel.afpaymentlivetype != -1)
            //    whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" afpaymentlivetype = {0}", inputModel.afpaymentlivetype) : string.Format(@" {0} and afpaymentlivetype = {1}", whereSql, inputModel.afpaymentlivetype);

            var leftjoinString = string.Format(@" left join ofAffiliateTree b on a.account=b.agentAccount ");
            var fields = " a.*,b.parentAccount ";

            return await DapperPaging<ExportAFBillReportOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理下线会员月游戏报表 ---导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersGameMonthDataFromDB(AgentReportQueryInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofPlayerMonthReportGame a ";
            string orderBy = " a.gameReportMonth desc ";
            string keyName = " a.id ";

            if (inputModel.startTime > 0 && inputModel.endTime > 0
                && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.Format(@" gameReportMonth >= {0} and gameReportMonth <= {1}", inputModel.startTime, inputModel.endTime);
            }

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount = '{0}' ", inputModel.account) : string.Format(@" {0} and a.affaccount = '{1}' ", whereSql, inputModel.account);
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "不存在的代理信息-查询失败",
                    message_code = "query_page_failed",
                };
            }
            var leftjoinString = " left join ofPlayerMonthReportFinance b on a.account=b.account and a.gameReportMonth = b.financeMonth ";
            var fields = " a.account,a.affaccount,a.gameReportMonth,xj_bet,xj_winlost,ag_bet,ag_winlost,ea_bet,ea_winlost,bbin_bet,bbin_winlost,kg_bet ,kg_winlost,dscp_bet ,dscp_winlost,dses_bet ,dses_winlost,kg2_bet ,kg2_winlost,lb_bet,lb_winlost,vr_bet,vr_winlost,pt_bet,pt_winlost,tt_bet,tt_winlost,pg_bet,pg_winlost,ysb_bet,ysb_winlost,imes_bet,imes_winlost,gg_bet,gg_winlost,sb_bet,sb_winlost,ky_bet,ky_winlost,rg_bet,rg_winlost ,xj_turnover,ag_turnover,ea_turnover,bbin_turnover ,kg_turnover ,dscp_turnover ,dses_turnover ,kg2_turnover,lb_turnover,vr_turnover,pt_turnover,tt_turnover,pg_turnover,ysb_turnover,imes_turnover,gg_turnover,sb_turnover,ky_turnover,rg_turnover ";

            return await DapperPaging<ExportPlayersMonthReportGameOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }
        /// <summary>
        /// 代理下线会员日游戏报表 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersGameDailayDataFromDB_old(AgentReportQueryInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofPlayerDailyReportGame a ";
            string orderBy = " a.gameReportDay desc ";
            string keyName = " a.id ";

            var minTime = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-6).ToString("yyyyMMdd"));

            if (inputModel.startTime > 0 && inputModel.endTime > 0 && inputModel.startTime > minTime
                && inputModel.endTime >= inputModel.startTime )
            {
                whereSql = string.Format(@" a.gameReportDay >= {0} and a.gameReportDay <= {1}", inputModel.startTime, inputModel.endTime);
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "6个月内的数据-导出失败",
                    message_code = "query_page_failed",
                };
            }

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount)
                    : string.Format(@" {0} and a.affaccount in (select account from ofAffiliate where verifyperson='{1}') ", whereSql, inputModel.adminAccount);
            }
            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount = '{0}' ", inputModel.account) : string.Format(@" {0} and a.affaccount = '{1}' ", whereSql, inputModel.account);
            //else
            //{
            //    return new ApiResultModel<object>()
            //    {
            //        code = "false",
            //        message = "不存在的代理信息-查询失败",
            //        message_code = "query_page_failed",
            //    };
            //}
            var leftjoinString = " left join ofPlayerDailyReportFinance b on a.account=b.account and a.gameReportDay = b.financeDay ";
            var fields = " a.account,a.affaccount,gameReportDay,xj_bet,xj_winlost,ag_bet,ag_winlost,ea_bet,ea_winlost,bbin_bet,bbin_winlost,kg_bet,dscp_bet ,dscp_winlost,dses_bet ,dses_winlost,kg2_bet ,kg2_winlost ,kg_winlost,lb_bet,lb_winlost,vr_bet,vr_winlost,pt_bet,pt_winlost,tt_bet,tt_winlost,pg_bet,pg_winlost,ysb_bet,ysb_winlost,imes_bet,imes_winlost,gg_bet,gg_winlost,sb_bet,sb_winlost,xj_turnover,ag_turnover,ea_turnover,bbin_turnover ,kg_turnover ,dscp_turnover ,dses_turnover ,kg2_turnover,lb_turnover,vr_turnover,pt_turnover,tt_turnover,pg_turnover,ysb_turnover,imes_turnover,gg_turnover,sb_turnover,ky_turnover,rg_turnover ";

            return await DapperPaging<ExportPlayersDailyReportGameOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理下线会员日游戏报表 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersGameDailayDataFromDB(AgentReportQueryInputModel inputModel)
        {
            var minTime = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-6).ToString("yyyyMMdd"));

            if (inputModel.startTime > 0 && inputModel.endTime > 0 && inputModel.startTime > minTime
                && inputModel.endTime >= inputModel.startTime)
            {
                //okay
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "6个月内的数据-导出失败",
                    message_code = "query_page_failed",
                };
            }

            var sqlCount = string.Empty;
            var sql = string.Empty;
            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                var sqlCheckRightSql = string.Format(@" select account from ofAffiliate where account='{0}' and verifyperson='{1}' ", inputModel.account, inputModel.adminAccount);
                var ant = await DapperBase.ExecuteScalarValueAsync(sqlCheckRightSql, null, commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                if (ant != null && string.Compare(ant.ToString(), inputModel.account, true) == 0)
                {
                    //okay
    //                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount)
    //: string.Format(@" {0} and a.affaccount in (select account from ofAffiliate where verifyperson='{1}') ", whereSql, inputModel.adminAccount);

                }
                else
                {
                    return new ApiResultModel<object>()
                    {
                        code = "false",
                        message = "无权导出代理数据-导出失败",
                        message_code = "query_page_failed",
                    };
                }
            }
           

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                sqlCount = string.Format(AgentSqlCommandText.PlayersGameExportReportCountSql, string.Format(@" and a.affaccount = '{0}' ", inputModel.account), "");
                sql = string.Format(AgentSqlCommandText.PlayersGameExportReportSql, string.Format(@" and a.affaccount = '{0}' ", inputModel.account), "");
            }
            else if (!string.IsNullOrEmpty(inputModel.parentAccount))
            {
                var parentQuerySql = string.Format(@" agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount='{0}'),'%') and agentLevel >= (select agentLevel from ofAffiliateTree where agentAccount='{0}') ", inputModel.parentAccount);
                sqlCount = string.Format(AgentSqlCommandText.PlayersGameExportReportCountSql, "", string.Format(@" where {0} ", parentQuerySql));
                sql = string.Format(AgentSqlCommandText.PlayersGameExportReportSql, "", string.Format(@" and {0} ", parentQuerySql));

            }
            else
            {
                sqlCount = string.Format(AgentSqlCommandText.PlayersGameExportReportCountSql, "", "");
                sql = string.Format(AgentSqlCommandText.PlayersGameExportReportSql, "", "");
            }

            var count = await DapperBase.QueryAsync<RecordsCount>(sqlCount,
                        new
                        {
                            startTime = inputModel.startTime,
                            endTime = inputModel.endTime
                        },
                   commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (count != null && count.Count > 0 && count.First().cnt > 0)//有记录，查询出来
            {
                var startIndex = 1;// (inputModel.pageIndex - 1) * inputModel.pageSize + 1;
                var endIndex = 12000; // inputModel.pageIndex * inputModel.pageSize;
                //var totalPages = count.First().cnt % inputModel.pageSize > 0 ? count.First().cnt / inputModel.pageSize + 1 : count.First().cnt / inputModel.pageSize;

                var resultGame = await DapperBase.QueryAsync<ExportPlayersDailyReportGameOutModel>(sql,
                new
                {
                    startTime = inputModel.startTime,
                    endTime = inputModel.endTime,
                    startIndex = startIndex,
                    endIndex = endIndex
                },
           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);



                var code = (resultGame != null && resultGame.Count > 0);
                return new AgentApiResultModel<PageModel<ExportPlayersDailyReportGameOutModel>, object>()
                {
                    code = code ? "true" : "false",
                    message = code ? "游戏记录导出-成功" : "游戏记录导出-失败",
                    message_code = code ? "load_agent_game_success" : "load_agent_game_failed",
                    levels = "",
                    data = new PageModel<ExportPlayersDailyReportGameOutModel>
                    {
                        TotalRecord = count.First().cnt,
                        TotalPages = 0,//totalPages,
                        PageIndex = inputModel.pageIndex,
                        modelList = resultGame.ToList()
                    },
                    total = null
                };
            }
            else
            {
                return new AgentApiResultModel<object, object>()
                {
                    code = "false",
                    message = "没有查找到相关记录",
                    message_code = "load_agent_wd_no_records",
                    levels = "",
                };
            }

        }

        /// <summary>
        /// 会员游戏注单 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadUserBetRecord(QueryUserBetLogInputModel inputModel)
        {
            var minTime = DateTime.UtcNow.AddHours(8).AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            #region "前一天内的数据,对传入的时间检测
            if (!string.IsNullOrEmpty(inputModel.startTime) && !string.IsNullOrEmpty(inputModel.endTime)
                && Convert.ToDateTime(inputModel.startTime) >= Convert.ToDateTime(minTime)
                && Convert.ToDateTime(inputModel.endTime) >= Convert.ToDateTime(inputModel.startTime))
            {
                //okay
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "前一天数据-导出失败",
                    message_code = "query_page_failed",
                };
            }
            #endregion


            inputModel.startTime = Convert.ToDateTime(inputModel.startTime).AddHours(-12).ToString("yyyy-MM-dd HH:mm:ss");
            inputModel.endTime = Convert.ToDateTime(inputModel.endTime).AddHours(-12).ToString("yyyy-MM-dd HH:mm:ss");

            string whereSql = string.Empty;
            var viewName = string.Format(@" {0} a ", "awc_sp_cr_bets");

            string orderBy = " a.adddate desc ";
            string keyName = " a.crId ";


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" userName = '{0}' ", inputModel.account)
                    : string.Format(@" {0} and userName = '{1}' ", whereSql, inputModel.account);
            }
            //未结算时，不分查询时间区间
            //if (!string.IsNullOrEmpty(inputModel.startTime)
            //    && !string.IsNullOrEmpty(inputModel.endTime) && inputModel.settled == 1)
            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" adddate >= '{0}' and adddate<'{1}' ", inputModel.startTime, inputModel.endTime)
                    : string.Format(@" {0} and adddate >= '{1}' and adddate<'{2}' ", whereSql, inputModel.startTime, inputModel.endTime);
            }

            var leftjoinString = "";
            var fields = " crId settleId, abs(wingold-gold) stakeAccurate, gold stake , wingold-gold winlost,userName memberCode, adddate createTime , resultdate settleTime, settle, gtype sprot_type, wtype betType, ip, crdata , iif(parlaynum=0,ioratio,0) odds";
            

            return await DapperPaging<CRBetRecordExportModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1, DapperStringBase.ConnectionGameRecordString);


        }


        /// <summary>
        /// 代理下线会员日游戏报表 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentGameDailayDataFromDB(AgentReportQueryInputModel inputModel)
        {
            var minTime = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-6).ToString("yyyyMMdd"));
            #region 6个月内的数据,对传入的时间检测
            if (inputModel.startTime > 0 && inputModel.endTime > 0 && inputModel.startTime > minTime
                && inputModel.endTime >= inputModel.startTime)
            {
                //okay
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "6个月内的数据-导出失败",
                    message_code = "query_page_failed",
                };
            }
            #endregion
            var sqlCount = string.Empty;
            var sql = string.Empty;

            #region 确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                var sqlCheckRightSql = string.Format(@" select account from ofAffiliate where account='{0}' and verifyperson='{1}' ", 
                    inputModel.account, inputModel.adminAccount);
                var ant = await DapperBase.ExecuteScalarValueAsync(sqlCheckRightSql, null, 
                    commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                if (ant != null && string.Compare(ant.ToString(), inputModel.account, true) == 0)
                {
                    //okay
                    //                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.affaccount in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount)
                    //: string.Format(@" {0} and a.affaccount in (select account from ofAffiliate where verifyperson='{1}') ", whereSql, inputModel.adminAccount);

                }
                else
                {
                    return new ApiResultModel<object>()
                    {
                        code = "false",
                        message = "无权导出代理数据-导出失败",
                        message_code = "query_page_failed",
                    };
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                sqlCount = string.Format(AgentSqlCommandText.AgentGameExportReportCountSql, string.Format(@" and a.account = '{0}' ", inputModel.account), "");
                sql = string.Format(AgentSqlCommandText.AgentGameExportReportSqlV, string.Format(@" and a.account = '{0}' ", inputModel.account), "");
            }
            else if (!string.IsNullOrEmpty(inputModel.parentAccount))
            {
                var parentQuerySql = string.Format(@" agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount='{0}'),'%') and agentLevel >= (select agentLevel from ofAffiliateTree where agentAccount='{0}') ", inputModel.parentAccount);
                sqlCount = string.Format(AgentSqlCommandText.AgentGameExportReportCountSql, "", string.Format(@" where {0} ", parentQuerySql));
                sql = string.Format(AgentSqlCommandText.AgentGameExportReportSqlV, "", string.Format(@" and {0} ", parentQuerySql));

            }
            else
            {
                sqlCount = string.Format(AgentSqlCommandText.AgentGameExportReportCountSql, "", "");
                sql = string.Format(AgentSqlCommandText.AgentGameExportReportSqlV, "", "");
            }

            var count = await DapperBase.QueryAsync<RecordsCount>(sqlCount,
                        new
                        {
                            startTime = inputModel.startTime,
                            endTime = inputModel.endTime,
                            currencyType = inputModel.currencyType
                        },
                   commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (count != null && count.Count > 0 && count.First().cnt > 0)//有记录，查询出来
            {
                var startIndex = 1;
                var endIndex = 12000; 

                var resultGame = await DapperBase.QueryAsync<ExportPlayersDailyReportGameOutModel>(sql,
                new
                {
                    startTime = inputModel.startTime,
                    endTime = inputModel.endTime,
                    startIndex = startIndex,
                    endIndex = endIndex,
                    currencyType = inputModel.currencyType
                },
           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);



                var code = (resultGame != null && resultGame.Count > 0);
                return new AgentApiResultModel<PageModel<ExportPlayersDailyReportGameOutModel>, object>()
                {
                    code = code ? "true" : "false",
                    message = code ? "游戏记录导出-成功" : "游戏记录导出-失败",
                    message_code = code ? "load_agent_game_success" : "load_agent_game_failed",
                    levels = "",
                    data = new PageModel<ExportPlayersDailyReportGameOutModel>
                    {
                        TotalRecord = count.First().cnt,
                        TotalPages = 0,//totalPages,
                        PageIndex = inputModel.pageIndex,
                        modelList = resultGame.ToList()
                    },
                    total = null
                };
            }
            else
            {
                return new AgentApiResultModel<object, object>()
                {
                    code = "false",
                    message = "没有查找到相关记录",
                    message_code = "load_agent_wd_no_records",
                    levels = "",
                };
            }

        }

        /// <summary>
        /// 代理下线会员月财务报表 --- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersFinanceMonthDataFromDB(AgentReportQueryInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofPlayerMonthReportFinance ";
            string orderBy = " financeMonth desc ";
            string keyName = " id ";

            if (inputModel.startTime > 0 && inputModel.endTime > 0
                && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.Format(@" financeMonth >= {0} and financeMonth <= {1}", inputModel.startTime, inputModel.endTime);
            }

            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" affaccount = '{0}' ", inputModel.account) : string.Format(@" {0} and affaccount = '{1}' ", whereSql, inputModel.account);
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "不存在的代理信息-查询失败",
                    message_code = "query_page_failed",
                };
            }
            var leftjoinString = "";
            var fields = " id,account,affaccount,financeMonth,depositAmount,depositTimes,depositFee,withdrawAmount,withdrawTimes,withdrawFee,promo,bonus,turnover ";

            return await DapperPaging<ExportPlayersMonthReportFinanceOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理下线会员日财务报表 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersFinanceDailayDataFromDB_old(AgentReportQueryInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = "ofPlayerDailyReportFinance u ";
            string orderBy = " u.financeDay desc ";
            string keyName = " u.id ";
            var minTime = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-6).ToString("yyyyMMdd"));

            if (inputModel.startTime > 0 && inputModel.endTime > 0 && inputModel.startTime > minTime
                && inputModel.endTime >= inputModel.startTime)
            {
                whereSql = string.Format(@" u.financeDay >= {0} and u.financeDay <= {1}", inputModel.startTime, inputModel.endTime);
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "6个月内的数据-导出失败",
                    message_code = "query_page_failed",
                };
            }
            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount)
                    : string.Format(@" {0} and u.affaccount in (select account from ofAffiliate where verifyperson='{1}') ", whereSql, inputModel.adminAccount);
            }
            if (!string.IsNullOrEmpty(inputModel.account))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount = '{0}' ", inputModel.account) : string.Format(@" {0} and u.affaccount = '{1}' ", whereSql, inputModel.account);
            

            var leftjoinString = string.Format(@" left join ofAffiliateTree oat on u.affaccount = oat.agentAccount
 left join ofPlayerDailyReportGame g on u.account = g.account and u.financeDay = g.gameReportDay ");

            var fields = string.Format(@" u.account,u.affaccount,oat.parentAccount,u.financeDay,u.depositAmount,u.depositTimes,u.depositFee,u.withdrawAmount,u.withdrawTimes,u.withdrawFee,u.promo,u.bonus,u.turnover, (ea_winlost+kg_winlost+sb_winlost+bbin_winlost+kg2_winlost+pt_winlost+ttg_winlost+gg_winlost+ag_winlost+vr_winlost+xj_winlost+imes_winlost+ob_winlost+ysb_winlost+pg_winlost+lb_winlost+ky_winlost+rg_winlost) as winlost,
(ea_bet + kg_bet + sb_bet + bbin_bet + kg2_bet + pt_bet + ttg_bet + gg_bet + ag_bet + vr_bet + xj_bet + imes_bet + ob_bet + ysb_bet + pg_bet + lb_bet + ky_bet + rg_bet) as bets ");

            return await DapperPaging<ExportPlayersDailyReportFinanceOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 8000, 1);
        }

        /// <summary>
        /// 代理下线会员日财务报表 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentPlayersFinanceDailayDataFromDB(AgentReportQueryInputModel inputModel)
        {
            var minTime = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-6).ToString("yyyyMMdd"));
            if (inputModel.startTime > 0 && inputModel.endTime > 0 && inputModel.startTime > minTime
                && inputModel.endTime >= inputModel.startTime)
            {
                //
            }
            else
            {
                return new ApiResultModel<object>()
                {
                    code = "false",
                    message = "6个月内的数据-导出失败",
                    message_code = "query_page_failed",
                };
            }
            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                var sqlCheckRightSql = string.Format(@" select account from ofAffiliate where account='{0}' and verifyperson='{1}' ", inputModel.account, inputModel.adminAccount);
                var ant = await DapperBase.ExecuteScalarValueAsync(sqlCheckRightSql,null,commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                if(ant != null && string.Compare(ant.ToString(),inputModel.account,true) == 0)
                {
                    //okay
                }
                else
                {
                    return new ApiResultModel<object>()
                    {
                        code = "false",
                        message = "无权导出代理数据-导出失败",
                        message_code = "query_page_failed",
                    };
                }
            }
            var sqlCount = string.Empty;
            var sql = string.Empty;

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                sqlCount = string.Format(AgentSqlCommandText.PlayersFinanceReportExportCountSql, string.Format(@" and a.affaccount = '{0}' ", inputModel.account), "");
                sql = string.Format(AgentSqlCommandText.PlayersFinanceReportExportSql, string.Format(@" and a.affaccount = '{0}' ", inputModel.account), "");
            }
            else if(!string.IsNullOrEmpty(inputModel.parentAccount))
            {
                var parentQuerySql = string.Format(@" agentIds like CONCAT('%',(select agentId from ofAffiliateTree where agentAccount='{0}'),'%') and agentLevel >= (select agentLevel from ofAffiliateTree where agentAccount='{0}') ", inputModel.parentAccount);
                sqlCount = string.Format(AgentSqlCommandText.PlayersFinanceReportExportCountSql, "", string.Format(@" where ", parentQuerySql));
                sql = string.Format(AgentSqlCommandText.PlayersFinanceReportExportSql, "", string.Format(@" and ", parentQuerySql));

            }
            else
            {
                sqlCount = string.Format(AgentSqlCommandText.PlayersFinanceReportExportCountSql, "", "");
                sql = string.Format(AgentSqlCommandText.PlayersFinanceReportExportSql, "", "");
            }


            var count = await DapperBase.QueryAsync<RecordsCount>(sqlCount,
                new
                {
                    startTime = inputModel.startTime,
                    endTime = inputModel.endTime
                },
           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

            if (count != null && count.Count > 0 && count.First().cnt > 0)//有记录，查询出来
            {
                var startIndex = 1;// (inputModel.pageIndex - 1) * inputModel.pageSize + 1;
                var endIndex = 12000;// inputModel.pageIndex * inputModel.pageSize;
                //var totalPages = count.First().cnt % inputModel.pageSize > 0 ? count.First().cnt / inputModel.pageSize + 1 : count.First().cnt / inputModel.pageSize;
                #region 获取当前财务报表的代理分组统计数据
                var resultFinance = await DapperBase.QueryAsync<ExportPlayersDailyReportFinanceOutModel>(sql,
                new
                {
                    startTime = inputModel.startTime,
                    endTime = inputModel.endTime,
                    startIndex = startIndex,
                    endIndex = endIndex
                },
           commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);
                #endregion

                
                var code = (resultFinance != null);

                return new AgentApiResultModel<PageModel<ExportPlayersDailyReportFinanceOutModel>, object>()
                {
                    code = code ? "true" : "false",
                    message = code ? "财务总表加载-成功" : "财务总表加载-失败",
                    message_code = code ? "load_agent_finance_success" : "load_agent_finance_failed",
                    levels = "",
                    data = new PageModel<ExportPlayersDailyReportFinanceOutModel>
                    {
                        TotalRecord = count.First().cnt,
                        TotalPages = 0,
                        PageIndex = inputModel.pageIndex,
                        modelList = resultFinance.ToList()
                    },
                    total = null
                };
            }
            else
            {
                return new AgentApiResultModel<object, object>()
                {
                    code = "false",
                    message = "没有查找到相关记录",
                    message_code = "load_agent_finance_no_records",
                    levels = "",
                };
            }

        }

        /// <summary>
        /// 代理返水数据 ---- 导出
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> LoadAgentTurnoverFromDB(QueryTurnOverInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofTurnover t ";
            string orderBy = " t.id desc";
            string keyName = " t.id ";


            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.account = '{0}' ", inputModel.account) : string.Format(@" {0} and t.account = '{1}' ", whereSql, inputModel.account);
            }

            //确认当前管理员账号---是否只能查看直所开代理的会员账号
            if (await LoadAllowedPathByAccount(inputModel.adminAccount, "/api/Admin/QueryAgent"))//账号已经被控了，只能看到自己开的代理和代理下线
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount in (select account from ofAffiliate where verifyperson='{0}') ", inputModel.adminAccount)
                    : string.Format(@" {0} and u.affaccount in (select account from ofAffiliate where verifyperson='{1}') ", whereSql, inputModel.adminAccount);
            }

            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" u.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and u.affaccount = '{1}' ", whereSql, inputModel.affaccount);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.status = {0} ", inputModel.status) : string.Format(@" {0} and t.status = {1} ", whereSql, inputModel.status);

            if (inputModel.gameType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.gametype = {0} ", inputModel.gameType) : string.Format(@" {0} and t.gametype = {1} ", whereSql, inputModel.gameType);

            if (!string.IsNullOrEmpty(inputModel.startTime)
                && !string.IsNullOrEmpty(inputModel.endTime))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.begindate >= '{0}' and t.enddate <= '{1}'", inputModel.startTime, inputModel.endTime) : string.Format(@" {0} and t.begindate >= '{1}' and t.enddate <= '{2}'", whereSql, inputModel.startTime, inputModel.endTime);
            }

            if (inputModel.platformType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" t.turnOverType = {0} ", inputModel.platformType) : string.Format(@" {0} and t.turnOverType = {1} ", whereSql, inputModel.platformType);

            var leftjoinString = string.Format(@" left join ofUser u on t.account = u.account
left join ofAffiliateTree oat on u.affaccount = oat.agentAccount 
left join ofGameClass gc on t.gametype = gc.gameid ");
            var fields = " t.account,t.effectivebets, t.reward,t.winlost,t.leftbets, t.begindate,t.enddate,t.requesttime,t.gametype,t.status,t.cancelperson,t.reviewperson,t.cancelreason,t.calc,u.affaccount,oat.agentAccount,oat.parentAccount,gc.gamename ";

            return await DapperPaging<ExportTurnOverOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, 30000, 1);

        }

        public static async Task<bool> LoadAllowedPathByAccount(string account, string requestPath)
        {
            try
            {
                var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
                var result = await DapperBase.QueryAsync<RightModel>(accessSql, new { account = account }, commandType: CommandType.Text,
                            commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

                if (result != null && result.Count == 1 && result.First().access.IndexOf(requestPath) >= 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public class RightModel
    {
        public string account { set; get; }
        public string access { set; get; }
    }
}
