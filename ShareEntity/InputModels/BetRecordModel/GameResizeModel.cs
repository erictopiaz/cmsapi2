using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.BetRecordModel {
    public class GameResizeModel {
        //account , actionName,platform,amount,amountType, create_at 
        public string account { set; get; }
        public string actionName { set; get; }
        public string platform { set; get; }
        public decimal amount { set; get; }
        public string description { set; get; }
        private int _amountType;
        public int amountType {
            set {
                _amountType = value;
                description = value == 1 ? "increase" : "decrease";
            }
            get { return _amountType; }
        }
        private string _create_at;
        public string create_at { 
            set { 
                _create_at = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            get { return _create_at; }
        }
    }
}
