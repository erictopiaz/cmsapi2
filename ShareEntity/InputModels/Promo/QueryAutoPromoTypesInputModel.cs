using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Promo
{
    /// <summary>
    /// 加载当前的自助优惠的分类菜单
    /// </summary>
    public class QueryAutoPromoTypesInputModel
    {
        /// <summary>
        /// 菜单开启的状态
        /// </summary>
        public int status { set; get; } = 1;
        public int pageSize { set; get; }
        public int pageIndex { set; get; }
    }
    /// <summary>
    /// 返回 当前的自助优惠的分类菜单
    /// </summary>
    public class QueryAutoPromoTypesOutModel
    {
        
        public int promoMenuCode { set; get; }
        public string promoMenuName { set; get; }
    }
}
