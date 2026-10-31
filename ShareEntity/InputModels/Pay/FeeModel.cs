using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Pay
{
    /// <summary>
    /// 手续费数据
    /// </summary>
    public class FeeModel
    {
        /// <summary>
        /// 手续费比例 , 存款金额*feecent = 手续费
        /// </summary>
        public decimal feecent { set; get; } = 0;
        /// <summary>
        /// 最低收款次数，如3 第三次就要收手续费用
        /// </summary>
        public int times { set; get; } = 0;
    }
}
