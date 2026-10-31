using ShareEntity.Agent.Finance;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.ValidationAttr;

namespace ShareEntity.InputModels.Euro
{
    /// <summary>
    /// 查询竞猜球队数据
    /// </summary>
    public class QueryEuroCupBilledInputModel : PageBaseModel
    {
        public int status { set; get; } = -1;
        public int matchs { set; get; } = -1;
        public int euroname { set; get; } = -1;
        /// <summary>
        /// 结算金额排序
        /// </summary>
        public int billSort { set; get; } = -1;
    }

//    account: "hongkuzi666"
//amount: null
//betTeams: "[1001,1003,1006,1008,1013,1012,1016,1023,1022,1007,1009,1017,1021,1010,1018,1004]"
//billTime: null
//create_at: "2021-06-11T14:25:09.76"
//euroname: 2021
//groupid: 24
    public class EuroCupBilledResponse
    {
        public long id { set; get; }
        public string account { set; get; }

        public decimal? amount { set; get; }
        public string teams { set; get; }

        private string _betTeams;
        public string betTeams {
            set
            {
                #region 欧洲杯 球队
                //var ts = new Dictionary<int, string>()
                //{
                //    { 1000, "土耳其" },
                //    { 1001, "意大利" },
                //    { 1002, "威尔士" },
                //    { 1003, "瑞士" },
                //    { 1004, "丹麦" },
                //    { 1005, "芬兰" },
                //    { 1006, "比利时" },
                //    { 1007, "俄罗斯" },
                //    { 1008, "荷兰" },
                //    { 1009, "乌克兰" },
                //    { 1010, "奥地利" },
                //    { 1011, "北马其顿" },
                //    { 1012, "英格兰" },
                //    { 1013, "克罗地亚" },
                //    { 1014, "苏格兰" },
                //    { 1015, "捷克" },
                //    { 1016, "西班牙" },
                //    { 1017, "瑞典" },
                //    { 1018, "波兰" },
                //    { 1019, "斯洛伐克" },
                //    { 1020, "匈牙利" },
                //    { 1021, "葡萄牙" },
                //    { 1022, "法国" },
                //    { 1023, "德国" }
                //};
                #endregion

                #region 卡尔世界杯
                //var ts = new Dictionary<int, string>()
                //{
                //    { 1000, "阿根廷" },
                //    { 1001, "澳大利亚" },
                //    { 1002, "巴西" },
                //    { 1003, "比利时" },
                //    { 1004, "波兰" },
                //    { 1005, "丹麦" },
                //    { 1006, "德国" },
                //    { 1007, "厄瓜多尔" },
                //    { 1008, "法国" },
                //    { 1009, "哥斯达黎加" },
                //    { 1010, "韩国" },
                //    { 1011, "荷兰" },
                //    { 1012, "加拿大" },
                //    { 1013, "加纳" },
                //    { 1014, "喀麦隆" },
                //    { 1015, "卡塔尔" },
                //    { 1016, "克罗地亚" },
                //    { 1017, "美国" },
                //    { 1018, "摩洛哥" },
                //    { 1019, "墨西哥" },
                //    { 1020, "葡萄牙" },
                //    { 1021, "日本" },
                //    { 1022, "瑞士" },
                //    { 1023, "塞尔维亚" },
                //    { 1024, "塞内加尔" },
                //    { 1025, "沙特阿拉伯" },
                //    { 1026, "突尼斯" },
                //    { 1027, "威尔士" },
                //    { 1028, "乌拉圭" },
                //    { 1029, "西班牙" },
                //    { 1030, "伊朗" },
                //    { 1031, "英格兰" }
                //};
                #endregion


                #region 2026美墨.世界杯
                var ts = new Dictionary<int, string>()
                {
                    {1000,  "阿根廷"},
                    {1001,  "澳大利亚"},
                    {1002,  "巴西"},
                    {1003,  "比利时"},
                    {1006,  "德国"},
                    {1007,  "厄瓜多尔"},
                    {1008,  "法国"},
                    {1010,  "韩国"},
                    {1011,  "荷兰"},
                    {1012,  "加拿大"},
                    {1013,  "加纳"},
                    {1015,  "卡塔尔"},
                    {1016,  "克罗地亚"},
                    {1017,  "美国"},
                    {1018,  "摩洛哥"},
                    {1019,  "墨西哥"},
                    {1020,  "葡萄牙"},
                    {1021,  "日本"},
                    {1022,  "瑞士"},
                    {1024,  "塞内加尔"},
                    {1025,  "沙特阿拉伯"},
                    {1026,  "突尼斯"},
                    {1028,  "乌拉圭"},
                    {1029,  "西班牙"},
                    {1030,  "伊朗"},
                    {1031,  "英格兰"},
                    {1032,  "南非"},
                    {1033,  "捷克"},
                    {1034,  "波黑"},
                    {1035,  "海地"},
                    {1036,  "苏格兰"},
                    {1037,  "巴拉圭"},
                    {1038,  "土耳其"},
                    {1039,  "库拉索"},
                    {1040,  "科特迪瓦"},
                    {1041,  "瑞典"},
                    {1042,  "埃及"},
                    {1043,  "新西兰"},
                    {1044,  "佛得角"},
                    {1045,  "伊拉克"},
                    {1046,  "挪威"},
                    {1047,  "阿尔及利亚"},
                    {1048,  "奥地利"},
                    {1049,  "约旦"},
                    {1050,  "刚果民主共和国"},
                    {1051,  "乌兹别克斯坦"},
                    {1052,  "哥伦比亚"},
                    {1053,  "巴拿马"}
                };
                #endregion

                var bts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(value);
                var t = string.Empty;
                foreach(var b in bts)
                {
                    t = string.IsNullOrEmpty(t) ? ts[b] : string.Format(@"{0},{1}", t, ts[b]);
                }
                teams = t;
                _betTeams = "";
            }
            get { return _betTeams; }
        }
        public string verify_at { set; get; }
        public string create_at { set; get; }
        public string matchs { set; get; }
        public int euroname { set; get; }
        private int _groupid;
        public int groupid {
            set
            {
                matchs = value == 48 ? "48晋级16" : value == 32 ? "32晋级8" : value == 16 ? "16晋级4" : "排名";
                _groupid = value;
            }
            get { return _groupid; }
        }

        public int b_status { set; get; }
        public int betCount { set; get; }
        public string who_verify { set; get; }
        public int anomaly { set; get; }
    }


    /// <summary>
    /// 修改请求数据
    /// </summary>
    public class EuroUpdateInputModel
    {
        public long id { set; get; }

        [CharAndDigitalAndUnderScore(16, MinimumLength = 4, IsAllowedEmpty = true, ErrorMessage = "account_format_error")]
        public string adminAccount { set; get; }

        [IpValidate(20, MinimumLength = 9, IsAllowedEmpty = true, ErrorMessage = "IP格式有误")]
        public string ipAddress { set; get; }
    }
}
