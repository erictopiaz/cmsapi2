using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Pay
{
   
    /// <summary>
    /// 查询，存款或提款UI参数配置
    /// </summary>
    public class QueryPaySettingInputModel : BaseInputModel
    {

    }

    public class QueryBanksInputModel : BaseInputModel
    {
        /// <summary>
        /// 0:rmb, 1:虚拟币
        /// </summary>
        public int category { set; get; }
    }
}
