using System.Collections.Generic;

namespace CMS.API.Models {
  
    public class CurrencyModel {
        /// <summary>
        /// 货币名称
        /// </summary>
        public string currencyname { get; set; }
        /// <summary>
        /// 货币代码
        /// </summary>
        public int currencycode { get; set; }

    }

    public class MerchantAccountModel
    {
        public string merAccount { get; set; }
        public string merCode { get; set; }
        public List<int> currencys { get; set; }
    }
    public class WalletTypeModel
    {
        public int WalletType { set; get; }
        public string WalletName { set; get; }
    }

    /// <summary>
    /// 基本的查询类型
    /// </summary>
    public class BaseSearchModel { 
        public List<CurrencyModel> currencyModels { get; set; }
        public List<MerchantAccountModel> merAccountModels { get; set; } = new List<MerchantAccountModel>()
        {
            new MerchantAccountModel() { merAccount="-1",merCode="请选择品牌名" },
            new MerchantAccountModel() { merAccount="Awc",merCode="Awc" },
            new MerchantAccountModel() { merAccount="Bayo",merCode="Bayo" },
            new MerchantAccountModel() { merAccount="Boss",merCode="Boss" }
        };
        public List<WalletTypeModel> walletTypes { get; set; } = new List<WalletTypeModel>() {
            new WalletTypeModel() { WalletType=-1,WalletName="请选择钱包类型" },
            new WalletTypeModel() { WalletType=1,WalletName="转账钱包" },
            new WalletTypeModel() { WalletType=0,WalletName="单一钱包" },
        };
    }

}
