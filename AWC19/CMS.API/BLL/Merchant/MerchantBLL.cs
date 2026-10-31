using ShareEntity.InputModels.website;
using DBLibs.WXDapper;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL.Merchant
{
    /// <summary>
    /// 商户设置更新
    /// </summary>
    public class MerchantBLL
    {
        /// <summary>
        /// 附加存款记录查询支付通道
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> SaveQueryMerchantWhere(QueryMerchantWhereModel inputModel)
        {
            var qr = Newtonsoft.Json.JsonConvert.SerializeObject(new { days = inputModel.days, status = inputModel.status });
            var updateSql = string.Format(@"update ofThirdpay
set queryMerchantWhere='{0}'
where id=@id ", qr);

            var updateResult = await DapperBase.ExecuteTransctionAsync(updateSql, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionString);
            return ApiResponseModel<object>.GetResult(
                updateResult ? 1:0,
                updateResult ? "设置成功!": "设置失败!",
                updateResult ? "save_merchant_where_success": "save_merchant_where_failed");


        }
    }
}
