using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel {

    /// <summary>
    /// 所有游戏操作行为记录，防重复操作
    /// </summary>
    [Table("GameTranferUnionLog")]
    public class GameTranferUnionLog {
        /// <summary>
        /// 唯一  md5(tid+actionName+platform)
        /// </summary>
        [Key]
        public string actionId { set; get; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 平台交易唯一id
        /// </summary>
        public string tid { set; get; }
        /// <summary>
        /// 交易行为名称
        /// </summary>
        public string actionName { set; get; }
        /// <summary>
        /// 平台名
        /// </summary>
        public string platform { set; get; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 结算金额类型 默认为 1
        /// 0：减分， 1：加分
        /// </summary>
        public int amountType { set; get; }
        /// <summary>
        /// 下注货币类型： 156，356 等，默认356
        /// </summary>
        public int g_currencyType { set; get; } = 356;

        /// <summary>
        /// 代理账号
        /// </summary>
        public string affaccount { set; get; } = "";
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchantAccount { set; get; } = "";

    }


    /// <summary>
    /// 所有游戏操作行为记录，防重复操作
    /// </summary>
    [Table("GameTranferUnionLog")]
    public class GameTranferUnionLogPostgre
    {
        /// <summary>
        /// 唯一  md5(tid+actionName+platform)
        /// </summary>
        [Key]
        public string action_id { set; get; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 平台交易唯一id
        /// </summary>
        public string tid { set; get; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime create_at { set; get; }
        /// <summary>
        /// 交易行为名称
        /// </summary>
        public string action_name { set; get; }
        /// <summary>
        /// 平台名
        /// </summary>
        public string platform { set; get; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 结算金额类型 默认为 1
        /// 0：减分， 1：加分
        /// </summary>
        public int amount_type { set; get; }
        /// <summary>
        /// 下注货币类型： 156，356 等，默认356
        /// </summary>
        public int g_currency_type { set; get; } = 356;

        /// <summary>
        /// 代理账号
        /// </summary>
        public string affaccount { set; get; } = "";
        /// <summary>
        /// 品牌
        /// </summary>
        public string merchant_account { set; get; } = "";

    }


    //transfer_id
    //game_id
    //source_type
    //source_id
    //amount
    //transfer_type
    //change_type
    //balance
    //user_name
    //user_parents
    //addtime
    //remarks
    //merchant
    //currency
    //state
    //original_time

    //string transfer_id, int game_id, int source_type, string source_id, decimal amount,
    //int transfer_type, int change_type, decimal balance, string user_name, string user_parents, DateTimeOffset addtime,
    //string remarks, string merchant, int currency, int state, DateTimeOffset original_time

    /// <summary>
    /// 游戏id 
    /// fb 8,  pa  9,  imes 10, imsp 11, tf  12
    /// </summary>
    public class GameIds
    {
        public const int boss = -1;
        public const int fb = 8;
        public const int pa = 9;
        public const int imes = 10;
        public const int imsp = 11;
        public const int tf = 12;
        public const int lb = 13;
        public const int ky = 14;
        public const int xj = 15;
        public const int db = 16;

        public static Dictionary<int, string> gameNames = new Dictionary<int, string>()
        {
            { 8, "FB" },
            { 9, "PA" },
            {10, "IMES" },
            {11, "IMSP" },
            {12, "TF" },
            {13, "LB" },
            {14, "KY" },
            {15, "XJ" },
            {16, "DB" },
            {1, "PG" },
            {4, "JDB" },
            {17, "DBKeno" }
        };

        public static Dictionary<string, int> gameIdDict = new Dictionary<string, int>()
        {
            { "FB",8 },
            { "PA", 9 },
            { "IMES", 10 },
            { "IMSP", 11 },
            { "TF", 12 },
            { "LB", 13 },
            { "KY", 14 },
            { "XJ", 15 },
            { "DB", 16 },
            { "PG", 1 },
            { "JDB", 4  },
            { "DBKeno", 17 }
        };
    }

    //transfer_id 訂單號
    //game_id 遊戲id 沒有默認給0
    //source_type源類型 默認注單0
    //source_id 帳變源訂單號 
    //amount 帳變金額 只支持正數
    //transfer_type 帳變類型
    //change_type 金額方向0.扣錢 1.加錢
    //balance 用戶餘額
    //user_name 用戶名
    //user_parents 用戶上級
    //addtime 插入時間
    //remarks 備註
    //merchant 廠商固定值Boss
    //currency 貨幣值
    //state 狀態 0.無效 1.有效
    //original_time 源時間 沒有就默認按插入時間



    /// <summary>
    /// 所有游戏操作行为记录，防重复操作
    /// </summary>
    [Table("game_wallet_transaction")]
    public class GameWalletTranferPostgre
    {
        /// <summary>
        /// 唯一 自增
        /// </summary>
        [Key]
        public long id { set; get; }
        /// <summary>
        /// 交易id  訂單號
        /// </summary>
        public string transfer_id { set; get; }
        /// <summary>
        /// 游戏id
        /// </summary>
        public int game_id { set; get; }
        /// <summary>
        /// 源類型 默認注單0
        /// </summary>
        public int source_type { set; get; }
        /// <summary>
        /// 帳變源訂單號 
        /// </summary>
        public string source_id { set; get; }

        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 帳變類型
        /// </summary>
        public int transfer_type { set; get; }
        /// <summary>
        /// 金額方向0.扣錢 1.加錢
        /// </summary>
        public int change_type { set; get; }
        /// <summary>
        /// 用戶餘額
        /// </summary>
        public decimal balance { set; get; }
        /// <summary>
        /// 账号
        /// </summary>
        public string user_name { set; get; }
        /// <summary>
        /// 上级
        /// </summary>
        public string user_parents { set; get; }
        /// <summary>
        /// 插入時間
        /// </summary>
        public DateTimeOffset addtime { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { set; get; }
        /// <summary>
        /// 廠商固定值Boss
        /// </summary>
        public string merchant { set; get; }
        /// <summary>
        /// 货币值 
        /// </summary>
        public int currency { set; get; }
        /// <summary>
        /// 狀態 0.無效 1.有效
        /// </summary>
        public int state { set; get; }
        /// <summary>
        /// 源時間 沒有就默認按插入時間
        /// </summary>
        public DateTimeOffset original_time { set; get; }

    }




    /// <summary>
    /// 只是做记录，放到下线再做处理
    /// 注：平台一次推过来多单，如im 一次最多过来100单，这样记录定单后，分配到下级再做处理。
    /// </summary>
    [Table("GameTranferUnionLogWait")]
    public class GameTranferUnionLogWait
    {
        /// <summary>
        /// 唯一  md5(tid+actionName+platform)
        /// </summary>
        [Key]
        public string actionId { set; get; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 平台交易唯一id
        /// </summary>
        public string tid { set; get; }
        /// <summary>
        /// 交易行为名称
        /// </summary>
        public string actionName { set; get; }
        /// <summary>
        /// 平台名
        /// </summary>
        public string platform { set; get; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 结算金额类型 默认为 1
        /// 0：减分， 1：加分
        /// </summary>
        public int amountType { set; get; }
        /// <summary>
        /// 下注货币类型： 156，356 等，默认356
        /// </summary>
        public int g_currencyType { set; get; } = 356;
       
        /// <summary>
        /// 0:需要，再次对订单结算一次，当前订单只是做记录，1：不需要
        /// 防平台一次推过来多单，如im 一次最多过来100单，这样记录定单后，分配到下级再做处理。
        /// </summary>
        public int orderStatus { set; get; } = 1;

        /// <summary>
        /// 代理账号
        /// </summary>
        public string affaccount { set; get; } = "";
    }
}
