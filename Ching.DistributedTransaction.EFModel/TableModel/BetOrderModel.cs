using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ching.DistributedTransaction.EFModel.TableModel 
{
    

    public class Turbo_BetOrderLog_withdraw : BetOrderModel {
        /// <summary>
        /// 下注游戏
        /// </summary>
        public string game_name { set; get; } = "";
        /// <summary>
        /// 游戏代码
        /// </summary>
        public string game_code { set; get; } = "";

        /// <summary>
        /// 下注时间
        /// </summary>
        public string bet_time { set; get; }

        public string code { set; get; }
    }


    public class Turbo_BetOrderLog_deposit : BetOrderModel {
        /// <summary>
        /// 下注游戏
        /// </summary>
        public string game_name { set; get; } = "";
        /// <summary>
        /// 游戏代码
        /// </summary>
        public string game_code { set; get; } = "";

        /// <summary>
        /// 注单更新时间
        /// </summary>
        public string update_time { set; get; }


        public string code { set; get; }
    }


    public class Turbo_BetOrderLog_rollback : BetOrderModel {
       
        /// <summary>
        /// 注单取消时间
        /// </summary>
        public string cancel_time { set; get; }


        public string code { set; get; }
    }


    /// <summary>
    /// 注单base
    /// </summary>
    public class BetOrderModel {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 玩家平台
        /// </summary>
        public string plat { set; get; }
        /// <summary>
        /// 平台交易单号 唯一
        /// </summary>
        public string dealid { set; get; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal money { set; get; } = 0;
       
        /// <summary>
        /// 注单状太
        /// 0:未结算，1：已经结算，2：取消
        /// </summary>
        public int transfer_status { set; get; } = 0;


        [Key]
        public string roundid { set; get; }


    }


    /// <summary>
    /// pg 注单base
    /// </summary>
    public class PGBetOrderModel {
        /// <summary>
        /// 会员账号
        /// </summary>
        public string account { set; get; }
        /// <summary>
        /// 玩家平台
        /// </summary>
        public string plat { set; get; }
        /// <summary>
        /// 平台交易单号 唯一
        /// </summary>
        public string transfer_id { set; get; }
        /// <summary>
        /// 下注金额
        /// </summary>
        public decimal bet_amount { set; get; } = 0;
        /// <summary>
        /// 赢得金额
        /// </summary>
        public decimal win_amount { set; get; } = 0;
        /// <summary>
        /// 输赢金额
        /// </summary>
        public decimal transfer_amount { set; get; } = 0;

        /// <summary>
        /// 下注游戏
        /// </summary>
        public string game_name { set; get; } = "";
        /// <summary>
        /// 游戏代码
        /// </summary>
        public string game_code { set; get; } = "";
        /// <summary>
        /// 注单状太
        /// 0:未结算，1：已经结算，2：取消
        /// </summary>
        public int transfer_status { set; get; } = 0;
        /// <summary>
        /// 下注时间
        /// </summary>
        public string bet_time { set; get; }
        /// <summary>
        /// 注单更新时间
        /// </summary>
        public string update_time { set; get; }


        public string roundid { set; get; }


    }



    public class PG_BetOrderLog_wd {

        /// <summary>
        /// 运营商独有的身份识别
        /// </summary>
        public string operator_token { get; set; }
        /// <summary>
        /// PG SOFT 与运营商之间共享密码
        /// </summary>
        public string secret_key { get; set; }
        /// <summary>
        /// 游戏启动时由运营商系统生成的令牌
        /// </summary>
        public string operator_player_session { get; set; }
        /// <summary>
        /// 玩家账号
        /// </summary>
        public string player_name { get; set; }
        /// <summary>
        /// 游戏的独有代码
        /// </summary>
        public int game_id { get; set; }
        /// <summary>
        /// 母注单的唯一标识符
        /// </summary>
        public string parent_bet_id { get; set; }
        /// <summary>
        /// 子投注的唯一标识符
        /// *注：投注的唯一键
        /// </summary>
        public string bet_id { get; set; }
        /// <summary>
        /// 玩家选择的币种
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// 投注金额
        /// 注：PG SOFT 将为免费旋转发送转账金额为零的投注请求
        /// </summary>
        public decimal bet_amount { get; set; }
        /// <summary>
        /// 赢得金额
        /// *对于以下情况，PG SOFT 将会发送转账金额为零的派彩请求：
        ///  ● 卡牌游戏里玩家进行的每次操作(例子： 分牌)
        ///  ● 一些老虎机游戏里的特色玩法
        /// </summary>
        public decimal win_amount { get; set; }
        /// <summary>
        /// 玩家的输赢金额
        /// 该金额可以是正数或负数,负数：扣除余额,正数：增加余额
        /// </summary>
        public decimal transfer_amount { get; set; }
        /// <summary>
        /// 交易的唯一标识符({BetId}-{ParentBetId}-{transactionType}-{balanceId})
        /// 交易类型：106：投付,400：红利转现金,403：免费游戏转现金,最多 50 字符
        /// </summary>
        [Key]
        public string transaction_id { get; set; }
        /// <summary>
        /// 表示该交易中使用的钱包类型
        /// C：现金,B：红利,G：免费游戏
        /// </summary>
        public string wallet_type { get; set; }
        /// <summary>
        /// 投注记录的投注类型：1：真实游戏
        /// </summary>
        public int bet_type { get; set; }
        /// <summary>
        /// 玩家的平台
        /// </summary>
        public string platform { get; set; }
        /// <summary>
        /// 投注的最近更新时间（时间戳，毫秒）
        /// </summary>
        public long create_time { get; set; }
        /// <summary>
        /// 投注的最近更新时间（时间戳，毫秒）
        /// </summary>
        public DateTime create_time_d { get; set; }
        /// <summary>
        /// updated_time 不等于投注的结束时间
        /// </summary>
        public long updated_time { get; set; }
        /// <summary>
        /// updated_time 不等于投注的结束时间
        /// </summary>
        public DateTime updated_time_d { get; set; }
        /// <summary>
        /// 表示该请求是否是为进行验证而重新发送的交易
        /// True: 重新发送的交易
        /// False: 正常交易
        /// </summary>
        public bool is_validate_bet { get; set; }
        /// <summary>
        /// 表示该请求是否是待处理投注的调整或正常交易
        /// True: 调整
        /// False: 正常交易
        /// </summary>
        public bool is_adjustment { get; set; }
        /// <summary>
        /// 表示该请求在第一轮投注中的投注金额是否为 0
        /// True: 在第一轮投注中的投注金额为 0（针对至尊百家乐的飞牌操作）
        /// False: 在第一轮投注中的投注金额大于 0
        /// </summary>
        public bool is_parent_zero_stake { get; set; }
        /// <summary>
        /// 表示旋转类型
        /// True: 特色旋转,False: 普通旋转
        /// </summary>
        public bool is_feature { set; get; }
        /// <summary>
        /// 表示购买奖金游戏
        /// 注：仅适用于具有购买奖金游戏功能的游戏
        /// </summary>
        public bool is_feature_buy { set; get; }
        /// <summary>
        /// 表示该交易是否为投注
        /// True: bet_amount > 0
        /// False: bet_amount = 0
        /// </summary>
        public bool is_wager { get; set; }
        /// <summary>
        /// 表示当前游戏投注是否已结束
        /// </summary>
        public bool is_end_round { get; set; }
        /// <summary>
        /// 免费游戏的唯一标识符
        /// 只有在免费游戏分配给玩家时才会出现
        /// 注：仅适用于以外部应用编程接口API 创建的免费游戏
        /// </summary>
        public string free_game_transaction_id { get; set; }
        /// <summary>
        /// 免费游戏名称
        /// 只有在免费游戏分配给玩家时才会出 现。
        /// </summary>
        public string free_game_name { get; set; }
        /// <summary>
        /// 免费游戏的唯一标识符
        /// 只有在免费游戏分配给玩家时才会出现
        /// </summary>
        public int free_game_id { get; set; }
        /// <summary>
        /// 免费游戏中的旋转类型
        /// True: 普通旋转(扣除免费游戏次数),False: 免费旋转
        /// </summary>
        public bool is_minus_count { get; set; }
        /// <summary>
        /// 红利游戏的唯一标识符,只有在红利分配给玩家时才会出现。
        /// 注：仅适用于以外部应用编程接口API 创建的红利游戏
        /// </summary>
        public string bonus_transaction_id { set; get; }
        /// <summary>
        /// 红利游戏名称
        /// 只有在红利分配给玩家时才会出现。
        /// </summary>
        public string bonus_name { get; set; }
        /// <summary>
        /// 红利游戏的唯一标识符
        /// 只有在红利分配给玩家时才会出现。
        /// </summary>
        public int bonus_id { get; set; }
        /// <summary>
        /// 红利钱包的红利总金额
        /// 只有在玩家选择用现金完成红利时才会出现。
        /// </summary>
        public decimal bonus_balance_amount { set; get; }
        /// <summary>
        /// 红利游戏中玩家需要达到的流水金额
        /// 只有在玩家选择用现金完成红利时才会出现。
        /// </summary>
        public decimal bonus_ratio_amount { set; get; }
    }


}
