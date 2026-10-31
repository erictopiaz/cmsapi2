using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLib.lang {
    /// <summary>
    /// 自定义资源ID
    /// </summary>
    public class ResourceCode {
        /// <summary>
        /// 代理结算状态
        /// </summary>
        public static readonly string aff_billed_record_status = "aff_billed_record_status";
        /// <summary>
        /// 存款列表，默认优惠选择
        /// </summary>
        public static readonly string deposit_promo_list = "deposit_promo_list";
        /// <summary>
        /// 游戏 转账最大最小值
        /// </summary>
        public static readonly string transfer_remark = "transfer_remark";
        /// <summary>
        /// 推荐好友规则
        /// </summary>
        public static readonly string spread_rules = "spread_rules";
        /// <summary>
        /// 代理页面佣金比例
        /// </summary>
        public static readonly string agent_page_data_commission_rate = "agent_page_data_CommissionRate";
        /// <summary>
        /// 合营推荐结算规则 【bayo   boss】+ 货币
        /// </summary>
        public static readonly string agent_billed_rules_desc = "agent_billed_rules_desc_{0}_{1}";
        /// <summary>
        /// 合营推荐结算比例 【bayo   boss】 + 货币
        /// </summary>
        public static readonly string agent_billed_rate_desc = "agent_billed_rate_desc_{0}_{1}";
        /// <summary>
        /// 代理页面，佣金规则
        /// </summary>
        public static readonly string agent_page_data_rules = "agent_page_data_rules";

        /// <summary>
        /// 存款记录状态列表
        /// </summary>
        public static readonly string deposit_record_status = "deposit_record_status";
        /// <summary>
        /// 存款状态列表
        /// </summary>
        public static readonly string withdraw_record_status = "withdraw_record_status";
        /// <summary>
        /// 转账状态列表
        /// </summary>
        public static readonly string transfer_record_status = "transfer_record_status";
        /// <summary>
        /// 红利优惠状态列表
        /// </summary>
        public static readonly string bonus_record_status = "bonus_record_status";
        /// <summary>
        /// 红利类型
        /// </summary>
        public static readonly string bonus_promo_type = "bonus_promo_type";

        /// <summary>
        /// 返水状态列表
        /// </summary>
        public static readonly string turnover_record_status = "turnover_record_status";

        /// <summary>
        /// 各国语言 简称
        /// </summary>
        public static readonly string lanuage_short_names = "lanuage_short_names";
        /// <summary>
        /// 国名
        /// </summary>
        public static readonly string country_short_names = "country_short_names";
        ///// <summary>
        ///// 绑定提款类别
        ///// </summary>
        //public static readonly string withdraw_category_json = "withdraw_category_json_{0}";

        /// <summary>
        /// 绑定提款类别(品牌+货币)
        /// </summary>
        public static readonly string withdraw_category_json_v = "withdraw_category_json_{0}_{1}";
    }

    /// <summary>
    /// 游戏相关 资源
    /// </summary>
    public class GameResourceCode {
        /// <summary>
        /// 游戏分类名
        /// </summary>
        public static readonly string game_slot_category_hot_names = "game_slot_category_hot_names";
        /// <summary>
        /// 电子游戏导航页  历史记录导航
        /// </summary>
        public static readonly string slot_game_nav_history = "slot_game_nav_history";

    }

    /// <summary>
    /// 直播api 资源
    /// </summary>
    public class LiveResourceCode {
        /// <summary>
        /// 直揪间礼物 资源
        /// </summary>
        public static readonly string gift_name_list = "gift_name_list";
        /// <summary>
        /// 直播间礼物分类 名称 资源
        /// </summary>
        public static readonly string gift_category_name = "gift_category_name";

        /// <summary>
        /// 礼物兑换单位 如 积分 资源
        /// </summary>
        public static readonly string gift_point_name = "gift_point_name";

        /// <summary>
        /// 完成任务获取的奖励  单位 资源  如积分或新密度
        /// </summary>
        public static readonly string task_point_name = "task_point_name";
        /// <summary>
        /// 任务名称  资源
        /// </summary>
        public static readonly string task_name_list = "task_name_list";

        /// <summary>
        /// 房间分类 名称
        /// </summary>
        public static readonly string live_room_category_name = "live_room_category_name";
    }


    public class PromoResourceCode
    {
        /// <summary>
        /// 查询任务中心 ui，根据货币
        /// </summary>
        public static readonly string query_promo_task_ui = "query_promo_task_ui_{0}";
        /// <summary>
        /// 分类福利中英文名
        /// </summary>
        public static readonly string query_promo_self_name = "query_promo_self_name";

        /// <summary>
        /// 领取按钮
        /// </summary>
        public static readonly string query_promo_submit_name = "query_promo_submit_name";
        /// <summary>
        /// 倍数
        /// </summary>
        public static readonly string query_promo_bet_time = "query_promo_bet_time";

        /// <summary>
        /// 首次充值任务优惠
        /// </summary>
        public static readonly string task_first_deposit_bonus = "task_first_deposit_bonus";
        /// <summary>
        /// 下载任务优惠
        /// </summary>
        public static readonly string task_download_app_bonus = "task_download_app_bonus";
        /// <summary>
        /// usdt首存任务优惠
        /// </summary>
        public static readonly string task_first_usdt_deposit_bonus = "task_first_usdt_deposit_bonus";
        /// <summary>
        /// 首次推荐会员任务优惠
        /// </summary>
        public static readonly string task_referral_first_active_player_bonus = "task_referral_first_active_player_bonus";
        /// <summary>
        /// 周首存任务优惠
        /// </summary>
        public static readonly string task_first_deposit_weekly_bonus = "task_first_deposit_weekly_bonus";
        /// <summary>
        /// 本周有充值，从app,申请优惠入口
        /// </summary>
        public static readonly string task_app_weekly_bonus = "task_app_weekly_bonus";
    }

}
