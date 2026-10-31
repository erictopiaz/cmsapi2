using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.BetRecordModel
{
    public class CRBetRecordExportModel : CRBetRecordModel
    {
        /// <summary>
        /// 如足球/篮球...
        /// </summary>
        public string sprot_type { set; get; }

        /// <summary>
        /// 下注ip
        /// </summary>
        public string ip { set; get; }

        public string crdata
        {
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    var crDataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CRDataModel>(value);
                    selecttion = crDataModel.order;
                }
            }
            get { return ""; }
        }
    }
    public class CRDataModel
    {
        /// <summary>
        //  下注大小或哪个队
        /// </summary>
        public string order { set; get; } = "";
    }
    public class CRBetRecordModel
    {

        public string settleId { set; get; }
        public decimal stake { set; get; }
        public decimal StakeAccurate { set; get; }
        public decimal odds { set; get; }
        /// <summary>
        /// 如大小盘，
        /// 选择了小  
        /// </summary>
        public string selecttion { set; get; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string betType { set; get; }
        public decimal winlost { set; get; }
        public string awc_account { set; get; }
        public string memberCode { set; get; }

        public string settleTime { set; get; }
        private string _createTime;
        public string createTime
        {
            set
            {
                _createTime = string.IsNullOrEmpty(value)?"":Convert.ToDateTime(value).AddHours(12).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _createTime; }
        }
        public int home_id { set; get; }
        public int away_id { set; get; }
        /// <summary>
        /// 主队名
        /// </summary>
        public string awc_hn { set; get; }
        /// <summary>
        /// 客队名
        /// </summary>
        public string awc_an { set; get; }
        public string wagerStatusDes { set; get; }

        /// <summary>
        /// 注单状态
        /// </summary>
        private int _ticket_status;
        public int settle
        {
            set
            {
                _ticket_status = value;
                wagerStatusDes = value == 1 ? "已结算" : "未结算";
            }
            get
            {
                return _ticket_status;
            }
        }
        public string gamePlatform { set; get; } = "169";

    }
}
