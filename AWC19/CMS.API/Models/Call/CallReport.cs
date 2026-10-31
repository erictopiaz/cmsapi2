using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Call
{
    public class CallReport
    {
        public int normalTotalCalls { set; get; } = 0;
        public int normalPickCalls { set; get; } = 0; //普通电话
        public int vipTotalCalls { set; get; } = 0;//vip电话
        public int vipPickCalls { set; get; } = 0;//vip电话  接通
        public int totalCalls { set; get; } = 0;//总拔打电话 

        public int vipHJTotalCalls { set; get; } = 0;//黄金VIP
        public int vipHJPickCalls { set; get; } = 0;//黄金VIP  接通



        public string np { set; get; } = "--%";//普通电话  接通率
        public string vp { set; get; } = "--%";//vip电话  接通率
        public string tp { set; get; } = "--%";//总电话  接通率
        public string hjp { set; get; } = "--%";//黄金VIP  接通率

        public int vipPTTotalCalls { set; get; } = 0;//普通玩家
        public int vipPTPickCalls { set; get; } = 0;//普通玩家  接通
        public string ptp { set; get; } = "--%";//普通玩家  接通率

        public int vipZSTotalCalls { set; get; } = 0;//钻石VIP
        public int vipZSPickCalls { set; get; } = 0;//钻石VIP  接通
        public string zsp { set; get; } = "--%";//钻石VIP  接通率

        public int vipTYTotalCalls { set; get; } = 0;//特邀VIP
        public int vipTYPickCalls { set; get; } = 0;//特邀VIP  接通
        public string typ { set; get; } = "--%";//特邀VIP  接通率

        public int vipZZTotalCalls { set; get; } = 0;//至尊VIP
        public int vipZZPickCalls { set; get; } = 0;//至尊VIP  接通
        public string zzp { set; get; } = "--%";//至尊VIP  接通率

        public int vipJXTotalCalls { set; get; } = 0;//见习VIP
        public int vipJXPickCalls { set; get; } = 0;//见习VIP  接通
        public string jxp { set; get; } = "--%";//见习VIP  接通率

        public int vipBYINTotalCalls { set; get; } = 0;//白银VIP
        public int vipBYINPickCalls { set; get; } = 0;//白银VIP  接通
        public string byinp { set; get; } = "--%";//白银VIP  接通率

        public int vipBYTotalCalls { set; get; } = 0;//白玉VIP
        public int vipBYPickCalls { set; get; } = 0;//白玉VIP  接通
        public string byp { set; get; } = "--%";//白玉VIP  接通率

        public int vipWXTotalCalls { set; get; } = 0;//万象会员
        public int vipWXPickCalls { set; get; } = 0;//万象会员  接通
        public string wxp { set; get; } = "--%";//万象会员  接通率

        public int vipHCTotalCalls { set; get; } = 0;//核查级别
        public int vipHCPickCalls { set; get; } = 0;//核查级别  接通
        public string hcp { set; get; } = "--%";//核查级别  接通率

        public int vipBJTotalCalls { set; get; } = 0;//铂金VIP
        public int vipBJPickCalls { set; get; } = 0;//铂金VIP  接通
        public string bjp { set; get; } = "--%";//铂金VIP  接通率

    }
    public class CallReportV : CallReport
    {
        public int normalTotalCallsV { set; get; } = 0;
        public int normalPickCallsV { set; get; } = 0; //普通电话
        public int vipTotalCallsV { set; get; } = 0;//vip电话
        public int vipPickCallsV { set; get; } = 0;//vip电话  接通
        public int totalCallsV { set; get; } = 0;//总拔打电话 

        public int vipHJTotalCallsV { set; get; } = 0;//黄金VIP
        public int vipHJPickCallsV { set; get; } = 0;//黄金VIP  接通



        public string npv { set; get; } = "--%";//普通电话  接通率
        public string vpv { set; get; } = "--%";//vip电话  接通率
        public string tpv { set; get; } = "--%";//总电话  接通率
        public string hjpv { set; get; } = "--%";//黄金VIP  接通率

        public int vipPTTotalCallsV { set; get; } = 0;//普通玩家
        public int vipPTPickCallsV { set; get; } = 0;//普通玩家  接通
        public string ptpv { set; get; } = "--%";//普通玩家  接通率

        public int vipZSTotalCallsV { set; get; } = 0;//钻石VIP
        public int vipZSPickCallsV { set; get; } = 0;//钻石VIP  接通
        public string zspv { set; get; } = "--%";//钻石VIP  接通率

        public int vipTYTotalCallsV { set; get; } = 0;//特邀VIP
        public int vipTYPickCallsV { set; get; } = 0;//特邀VIP  接通
        public string typv { set; get; } = "--%";//特邀VIP  接通率

        public int vipZZTotalCallsV { set; get; } = 0;//至尊VIP
        public int vipZZPickCallsV { set; get; } = 0;//至尊VIP  接通
        public string zzpv { set; get; } = "--%";//至尊VIP  接通率

        public int vipJXTotalCallsV { set; get; } = 0;//见习VIP
        public int vipJXPickCallsV { set; get; } = 0;//见习VIP  接通
        public string jxpv { set; get; } = "--%";//见习VIP  接通率

        public int vipBYINTotalCallsV { set; get; } = 0;//白银VIP
        public int vipBYINPickCallsV { set; get; } = 0;//白银VIP  接通
        public string byinpv { set; get; } = "--%";//白银VIP  接通率

        public int vipBYTotalCallsV { set; get; } = 0;//白玉VIP
        public int vipBYPickCallsV { set; get; } = 0;//白玉VIP  接通
        public string bypv { set; get; } = "--%";//白玉VIP  接通率

        public int vipWXTotalCallsV { set; get; } = 0;//万象会员
        public int vipWXPickCallsV { set; get; } = 0;//万象会员  接通
        public string wxpv { set; get; } = "--%";//万象会员  接通率

        public int vipHCTotalCallsV { set; get; } = 0;//核查级别
        public int vipHCPickCallsV { set; get; } = 0;//核查级别  接通
        public string hcpv { set; get; } = "--%";//核查级别  接通率

        public int vipBJTotalCallsV { set; get; } = 0;//铂金VIP
        public int vipBJPickCallsV { set; get; } = 0;//铂金VIP  接通
        public string bjpv { set; get; } = "--%";//铂金VIP  接通率

    }
}
