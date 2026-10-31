using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel.TableModel.TF
{
    
  
    public class order_tf
    {
        [Key]
        public long selfid { set; get; }
        /// <summary>
        /// 注单号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public decimal? odds { get; set; }
        /// <summary>
        /// 马来赔率
        /// </summary>
        public decimal? malay_odds { get; set; }
        /// <summary>
        /// 欧盘赔率
        /// </summary>
        public decimal? euro_odds { get; set; }
        /// <summary>
        /// 会员下的赔率
        /// </summary>
        public decimal? member_odds { get; set; }
        /// <summary>
        /// 会员下的盘
        /// </summary>
        public string member_odds_style { get; set; }
        /// <summary>
        /// 游戏ID
        /// </summary>
        public int? game_type_id { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public string game_type_name { get; set; }
        /// <summary>
        /// 盘口名称
        /// </summary>
        public string game_market_name { get; set; }
        /// <summary>
        /// 盘口局分
        /// </summary>
        public string market_option { get; set; }
        /// <summary>
        /// 第几局
        /// </summary>
        public string map_num { get; set; }
        /// <summary>
        /// 盘口类型
        /// </summary>
        public string bet_type_name { get; set; }
        /// <summary>
        /// 比赛名称
        /// </summary>
        public string competition_name { get; set; }
        /// <summary>
        /// 赛事ID
        /// </summary>
        public int? event_id { get; set; }
        /// <summary>
        /// 赛事名称
        /// </summary>
        public string event_name { get; set; }
        /// <summary>
        /// 赛事开始时间
        /// </summary>
        public long? event_datetime { get; set; }
        /// <summary>
        /// 下注时间
        /// </summary>
        public long? date_created { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public long? settlement_datetime { get; set; }
        /// <summary>
        /// 更改时间
        /// </summary>
        public long? modified_datetime { get; set; }
        /// <summary>
        /// 下注选项
        /// </summary>
        public string bet_selection { get; set; }
        /// <summary>
        /// 货币
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 下注金额
        /// </summary>
        public decimal? amount { get; set; }

        /// <summary>
        /// 注单状况
        /// </summary>
        public string settlement_status { get; set; }
        /// <summary>
        /// true = 已重新结算
        /// </summary>
        public bool? is_unsettled { get; set; }
        /// <summary>
        /// 注单结果
        /// </summary>
        public string result_status { get; set; }
        /// <summary>
        /// 盘口结果
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 输赢额
        /// </summary>
        public decimal? earnings { get; set; }
        /// <summary>
        /// 让分数
        /// </summary>
        public string handicap { get; set; }
        /// <summary>
        /// 会员号
        /// </summary>
        public string member_code { get; set; }
        /// <summary>
        /// 下注的管道
        /// </summary>
        public string request_source { get; set; }

        /// <summary>
        /// 是否连串
        /// </summary>
        public bool? is_combo { get; set; }
        /// <summary>
        /// 注单下注状况
        /// </summary>
        public string ticket_type { get; set; }

       
        /// <summary>
        /// 小游戏桌子ID
        /// </summary>
        public int? mini_game_table_id { get; set; }
        /// <summary>
        /// 小游戏桌子名
        /// </summary>
        public string mini_game_table_name { get; set; }
        /// <summary>
        /// 游戏前端克制赛事显示ID
        /// </summary>
        public string event_sid { get; set; }
        /// <summary>
        /// 奖池 0, 1, 2, 3
        /// </summary>
        public int? jackpot_type { get; set; }
        /// <summary>
        /// coin, mini, minor, major
        /// </summary>
        public string jackpot_type_name { get; set; }
        /// <summary>
        /// 奖池金额
        /// </summary>
        public decimal? jackpot_amount { get; set; }
        /// <summary>
        /// 会员输赢 (本地货币)
        /// 这个是扣除下注金额了
        /// </summary>
        public decimal? member_win_loss_f { get; set; }
        /// <summary>
        /// 会员输赢 
        /// 这个是扣除下注金额了
        /// </summary>
        public decimal? member_win_loss_l { get; set; }
        /// <summary>
        /// 小游戏税 (本地货币)
        /// </summary>
        public decimal? tax_amount { get; set; }
        /// <summary>
        /// 游戏前端克制煮蛋显示ID
        /// </summary>
        public string ticket_sid { get; set; }

        /// <summary>
        /// 会员上级代理
        /// </summary>
        public string parents { get; set; }
    }
}
