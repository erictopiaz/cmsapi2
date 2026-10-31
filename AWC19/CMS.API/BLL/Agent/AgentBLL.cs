using ShareEntity.InputModels;
using ShareEntity.InputModels.agent;
using ShareEntity.SqlCommandText;
using Dapper;
using DBLibs.WXDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL.Agent
{
    /// <summary>
    /// cms 代理业务管理
    /// </summary>
    public class AgentBLL
    {
        /// <summary>
        /// 代理审核返回结果
        /// </summary>
        private static readonly Dictionary<string, string> VerifyAgentStatus = new Dictionary<string, string>()
        {
            {"agent_verify_success","代理审核成功" },
            {"agent_verify_failed_mult_verify","代理账号重复" },
            {"agent_verify_find_top_agent_failed","没有找到上级代理" },
            {"agent_verify_load_agentdata_failed","获取上级代理信息失败" },
            {"upper_agent_not_allowed_create_agent_failed","上级代理没有开代理权限" },
            {"agent_verify_update_agent_failed","更新代理状态失败" },
            {"agent_verify_load_agentid_failed","获取同级代理ID失败" },
            {"agent_verify_update_agentlevel_failed","'更新代理层级 - 失败" },
            {"agent_verify_undefined_failed","未知错误" }
        };
        
        /// <summary>
        /// 创建代理账号
        /// </summary>
        /// <returns></returns>
        public static async Task<object> CreateAccountAsync(RegisterAffiliateModel model)
        {
            model.createperson = model.account;
            model.password = Common.GetMd5(model.password);
            var result = await DapperBase.ExecuteTransctionAsync(AgentSqlCommandText.RegisterAgentSql, model, 
                commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);

            return new ApiResultModel<object>
            {
                code = result.ToString().ToLower(),
                message = result ? "代理注册-成功" : "代理注册-失败",
                message_code = result ? "agent_register_success" : "agent_register_failed"
            };
        }
        /// <summary>
        /// 代理审核
        /// </summary>
        /// <param name="agentAccount"></param>
        /// <returns></returns>
        public static async Task<object> AgentVerify(VerifyAgentInputModel agentAccount)
        {
            #region sp params
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@account", agentAccount.account, DbType.AnsiString, ParameterDirection.Input, 50);
            dp.Add("@adminAccount", agentAccount.adminAccount, DbType.AnsiString, ParameterDirection.Input, 50);
            dp.Add("@re", "", dbType: DbType.AnsiString, direction: ParameterDirection.Output, 50);
            #endregion
            await DapperBase.QueryAsync<int>("SP_AgentVerify", dp, commandType: CommandType.StoredProcedure,
                ConnectionString:DapperStringBase.ConnectionString);
            var e_code = dp.Get<string>("@re");
            return new ApiResultModel<object>
            {
                code = string.Compare(e_code, "agent_verify_success") == 0 ? "true":"false",
                message = VerifyAgentStatus[e_code],
                message_code = e_code
            };
        }

        /// <summary>
        /// 代理开启或停用
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> AgentDisableOrEnable(AgentDisableOrEnableInputModel inputModel)
        {
            var result = await DapperBase.ExecuteTransctionAsync(AgentSqlCommandText.AgentDisableOrEnableSql, inputModel,
               commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionString);
            return new ApiResultModel<object>
            {
                code = result.ToString().ToLower(),
                message = result ? "代理状态更新-成功" : "代理状态更新-失败",
                message_code = result ? "agent_disable_enable_success" : "agent_disable_enable_failed"
            };
        }

        /// <summary>
        /// 验证密码是否正常
        /// </summary>
        /// <param name="pwdObject"></param>
        /// <param name="md5Password"></param>
        /// <returns></returns>
        private static bool checkPassword(object pwdObject, string md5Password)
        {
            if (pwdObject != null && string.Compare(pwdObject.ToString(), md5Password) == 0)
                return true;
            return false;
        }

        public static async Task<bool> LoadAllowedPathByAccount(string account, string requestPath) {
            try {
                var accessSql = string.Format(@"select * from ofManage where account=@account and status = 1");
                var result = await DapperBase.QueryAsync<RightModel>(accessSql, new { account = account }, commandType: CommandType.Text,
                            commandTimeout: 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);

                if (result != null && result.Count == 1 && result.First().access.IndexOf(requestPath, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                return false;
            }
            catch (Exception ex) {
                return false;
            }
        }


    }
    public class RightModel {
        public string account { set; get; }
        public string access { set; get; }
    }
}
