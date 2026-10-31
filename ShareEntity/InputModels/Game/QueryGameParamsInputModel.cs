using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game
{
    public class QueryGameParamsInputModel
    {
        public string gamename { set; get; }
        public string param {set;get;}
        public int gameid { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string g_merchantAccounts { set; get; }

        public List<int> currencys { set; get; }= new List<int>();
        private string _currencyTypes;
        /// <summary>
        /// 货币代码集合，如[156,356]
        /// </summary>
        public string currencyTypes
        {
            set
            {
                if(string.IsNullOrEmpty(value))
                    currencys = new List<int>();
                else
                {
                    currencys = JsonConvert.DeserializeObject<List<int>>(value);
                }
                _currencyTypes = value;
            }
            get { return _currencyTypes; }
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        public DateTime timeOut { set; get; }
    }


    public class MiscModel {
        public decimal bonusbalance { set; get; }
        public string prevlogin { set; get; }
        public int phoneverify { set; get; }
        public string recommendcode { set; get; }
        public string withdrawpromo18 { set; get; }
        public string fonverpro2020 { set; get; }
        public string gamedisabledstat { set; get; }
    }
}
