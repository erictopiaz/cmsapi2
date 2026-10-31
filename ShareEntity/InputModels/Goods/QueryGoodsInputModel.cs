using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Goods
{
    /// <summary>
    /// 实物兑换查询 
    /// </summary>
    public class QueryGoodsInputModel
    {
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string account { get; set; }
        /// <summary>
        /// 礼品分类
        /// </summary>
        public int goodsType { set; get; } = -1;
        /// <summary>
        /// 礼品状态
        /// </summary>
        public int status { set; get; } = -1;
        /// <summary>
        /// 礼品名
        /// </summary>
        public string titleName { set; get; }

        public int pageSize { set; get; }
        public int pageIndex { set; get; }
    }
    /// <summary>
    /// 礼品记录输出
    /// </summary>
    public class QueryGoodsOutputModel
    {
        public long id {  set; get; }
        /// <summary>
        /// 礼品类型
        /// </summary>
        public string pointTypeDesc { set; get; }
        private int _pointType;
        /// <summary>
        /// 礼品类型
        /// </summary>
        public int pointType
        {
            set
            {
                pointTypeDesc = value == 0 ? "礼金" : "礼品";
                this._pointType = value;

            }
            get { return _pointType; }
        }
        public string goodsTypeDesc { set; get; }
        private int _goodsType;
        public int goodsType
        {
            set
            {
                //{ value: -1, text: '==奖品分类==' },{ value: 0, text: '3C数码' },{ value: 1, text: '时尚休闲' },{ value: 2, text: '生活家电' },{ value: 3, text: '彩金' }
                goodsTypeDesc = value == 0 ? "3C数码" : value == 1 ? "时尚休闲" : value == 2 ? "生活家电" : value == 3 ? "彩金" : "--";
                this._goodsType = value;
            }
            get { return _goodsType; }
        }
        /// <summary>
        /// 礼品名
        /// </summary>
        public string titleName { set; get; }
        /// <summary>
        /// 礼品图
        /// </summary>
        public string url { set; get; }
        /// <summary>
        /// titleName 加密
        /// </summary>
        public string alisa { set; get; }
        /// <summary>
        /// 所需积分
        /// </summary>
        public decimal points { set; get; }
        /// <summary>
        /// 筹码金额 pointType=0时使用
        /// </summary>
        public decimal amount { set; get; }

        public string create_at { set; get; }  
        /// <summary>
        /// 物品id
        /// </summary>
        public long pid { set; get; }

        public string statusDesc {  set; get; }
        private int _status;
        public int status { 
            set
            {
                statusDesc = value == 0 ? "禁用" : value == 1 ? "可用" : "--";
                this._status = value;   
            }
            get {  return _status; }
        }

        public string create_by { set; get; }
        private string _pic;
        public string pic {
            set {
                _pic = value; 
                url = String.Format(@"{0}/goods/{1}", url, value);
            } 
            get {  return _pic; }
        }
    }

    public class CreateGoodsInputModel
    {
        /// <summary>
        /// 礼品类型
        /// </summary>
        public int pointType { set;get;}

        public int goodsType {  set; get; }
        /// <summary>
        /// 礼品名
        /// </summary>
        public string titleName { set; get; }
        /// <summary>
        /// 礼品图
        /// </summary>
        [UrlValidate(50, MinimumLength = 10, IsAllowedEmpty = true, ErrorMessage = "url_format_error")]
        public string url { set; get; }
        /// <summary>
        /// titleName 加密
        /// </summary>
        public string alisa { set; get; }
        /// <summary>
        /// 所需积分
        /// </summary>
        public decimal points { set; get; } = 0;
        /// <summary>
        /// 筹码金额 pointType=0时使用
        /// </summary>
        public decimal amount { set; get; } = 0;

        public int status {  set; get; }

        public long pid { set; get; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string create_by { set; get; }
        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string update_by { set; get; }

        public string pic {  set; get; }  
    }

    public class UpdateGoodsInputModel: CreateGoodsInputModel
    {
        public long id {  set; get; }
    }

    /// <summary>
    /// 启用或关闭  goods
    /// </summary>
    public class OffOrOnInputModel
    {
        public long id { set; get; }   
        public int status { set; get; }
    }


}
