using ShareEntity.InputModels.Goods;
using DBLibs;
using DBLibs.WXDapper;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;

namespace CMS.API.BLL.Goods
{
    /// <summary>
    /// 积分业务
    /// </summary>
    public class PointsGoodsBLL
    {
        /// <summary>
        /// 查询礼品
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> QueryExchangeGoods(QueryGoodsInputModel inputModel)
        {
            var viewName = " ofPointsExchangeGoods a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";

            // 创建DAPPER TEMPLATE 
            var whereSql = string.Empty;
            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status={0} ", inputModel.status) : string.Format(@" {0} and a.status={1} ", whereSql, inputModel.status);


            if (inputModel.goodsType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.goodsType={0} ", inputModel.goodsType) : string.Format(@" {0} and a.goodsType={1} ", whereSql, inputModel.goodsType);

            if (!string.IsNullOrEmpty(inputModel.titleName))
            {
                var alisa = Common.GetMd5(inputModel.titleName.Replace("'", "''"));
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.alisa='{0}' ", alisa) : string.Format(@" {0} and a.alisa='{1}' ", whereSql, alisa);

            }

            var leftjoinString = string.Format(@"");
            var fields = " a.* ";

            return await DapperPaging<QueryGoodsOutputModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex, connectStringBase: DapperStringBase.ConnectionLogs);
        }


        /// <summary>
        /// 新增礼品
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> CreatePointsGoods(CreateGoodsInputModel inputModel)
        {


            var sql = string.Format(@"select body specialPromUrl from ofSettings with(nolock)
where code='GAME_PIC_URL'");
            var url = await DapperBase.ExecuteScalarValueAsync(sql,
                    null, null, 15000, ConnectionString: DapperStringBase.ConnectionStringReadOnly);


            if (inputModel.titleName.Length > 20)
                return ApiResultModel<object>.GetResult("false", "失败：礼品名（0~20位）", "limit_goods_name_failed");

            inputModel.url = url == null ? "" : url.ToString();
            inputModel.titleName = inputModel.titleName.Replace("'", "''");
            inputModel.alisa = Common.GetMd5(inputModel.titleName);
            inputModel.pid = Common.GuidToLong();
            inputModel.pic = string.IsNullOrEmpty(inputModel.pic) ? "" : inputModel.pic.Replace("'", "''");

            var insertGoodsSal = string.Format(@"INSERT INTO [dbo].[ofPointsExchangeGoods]
           ([pointType],[titleName],[url],[points],[status],[goodsType],[amount],[create_by],[pid],[alisa],[pic])
     VALUES
           (@pointType,@titleName,@url,@points,@status,@goodsType,@amount
           ,@create_by,@pid,@alisa,@pic)");

            var insertResult = await DapperBase.ExecuteTransctionAsync(insertGoodsSal, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            return ApiResultModel<object>.GetResult(
                insertResult ? "true" : "false",
                insertResult ? "新增礼品-成功!" : "新增礼品-失败!",
                insertResult ? "create_goods_success" : "create_goods_failed");
        }

        /// <summary>
        /// 积分礼品启用/关闭
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> OffOrOn(OffOrOnInputModel inputModel)
        {

            var offonGoodsSal = string.Format(@"UPDATE [dbo].[ofPointsExchangeGoods]
   SET [status] = @status
 WHERE id=@id");
            
            var insertResult = await DapperBase.ExecuteTransctionAsync(offonGoodsSal, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            return ApiResultModel<object>.GetResult(
                insertResult ? "true" : "false",
                insertResult ? "启用/关闭-成功!" : "启用/关闭-失败!",
                insertResult ? "off_on_goods_success" : "off_on_goods_failed");
        }

        /// <summary>
        /// 积分礼品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<object> QueryGoodsById(long id)
        {
            var querySql = string.Format(@"select * from ofPointsExchangeGoods where id = @id");
            var models = await DapperBase.QueryAsync<QueryGoodsOutputModel>(querySql, new { id = id }, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            var code = models != null && models.Count == 1;
            return ApiResultModel<object>.GetResult(
               code ? "true" : "false",
               code ? "积分礼品详情-成功" : "积分礼品详情-失败",
               code ? "query_goods_detail_success" : "query_goods_detail_failed",
               code ? models.First() : null);
        }

        /// <summary>
        /// 更新礼品
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> ModifyGoods(UpdateGoodsInputModel inputModel)
        {

            if (inputModel.titleName.Length > 20)
                return ApiResultModel<object>.GetResult("false", "失败：礼品名（0~20位）", "limit_goods_name_failed");

            inputModel.titleName = inputModel.titleName.Replace("'", "''");
            inputModel.alisa = Common.GetMd5(inputModel.titleName);
            inputModel.pic = string.IsNullOrEmpty(inputModel.pic) ? "" : inputModel.pic.Replace("'", "''");


            var updateGoodsSal = string.Format(@"UPDATE [dbo].[ofPointsExchangeGoods]
   SET [pointType] = @pointType,[titleName] =@titleName,[points] = @points
      ,[status] = @status, [goodsType] = @goodsType, [amount] = @amount,[alisa] = @alisa, [update_by]=@update_by,[pic]=@pic
 WHERE id=@id");

            var insertResult = await DapperBase.ExecuteTransctionAsync(updateGoodsSal, param: inputModel, commandTimeout: 15000,
                ConnectionString: DapperStringBase.ConnectionLogs);
            return ApiResultModel<object>.GetResult(
                insertResult ? "true" : "false",
                insertResult ? "更新礼品-成功!" : "更新礼品-失败!",
                insertResult ? "update_goods_success" : "update_goods_failed");
        }
    }
}
