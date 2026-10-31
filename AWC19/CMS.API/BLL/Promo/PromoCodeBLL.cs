using ShareEntity.InputModels.Promo;
using ShareEntity.OutModels.Promo;
using DBLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.BLL.Promo
{
    public class PromoCodeBLL
    {
        /// <summary>
        /// 优惠代码查询返回
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static async Task<object> PromoCodeRecords(QueryPromoCodeInputModel inputModel)
        {
            string whereSql = string.Empty;
            var viewName = " ofPromoCode a ";
            string orderBy = " a.id desc";
            string keyName = " a.id ";

            if (!string.IsNullOrEmpty(inputModel.account))
            {
                whereSql = string.Format(@" a.account = '{0}' ", inputModel.account);
            }
            if (!string.IsNullOrEmpty(inputModel.affaccount))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" b.affaccount = '{0}' ", inputModel.affaccount) : string.Format(@" {0} and b.affaccount = '{1}' ", whereSql, inputModel.affaccount);
            
            if (!string.IsNullOrEmpty(inputModel.promocode))
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.promocode = '{0}' ", inputModel.promocode) : string.Format(@" {0} and a.promocode = '{1}' ", whereSql, inputModel.promocode);

            if (inputModel.status != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" a.status = {0} ", inputModel.status) : string.Format(@" {0} and a.status = {1} ", whereSql, inputModel.status);

            if (!string.IsNullOrEmpty(inputModel.remark))
            {
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" condition like '%{0}%' ", inputModel.remark) : string.Format(@" {0} and condition like '%{1}%' ", whereSql, inputModel.remark);
            }

            if (inputModel.platformType != -1)
                whereSql = string.IsNullOrEmpty(whereSql) ? string.Format(@" platformType = {0} ", inputModel.platformType) : string.Format(@" {0} and platformType = {1} ", whereSql, inputModel.platformType);

            var leftjoinString = " left join ofUser b on a.account = b.account  ";
            var fields = " a.id as tid,a.account,a.promocode,a.amount,a.bet,a.lowamount,a.condition as remark,a.createtime,a.expiredate,a.createperson,a.status,b.affaccount ";

            return await DapperPaging<QueryPromoCodeOutModel>.QueryPage(whereSql, viewName, leftjoinString,
                fields, orderBy, keyName, inputModel.pageSize, inputModel.pageIndex);
        }
    }
}
