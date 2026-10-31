using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.InputModels.Game.BB
{
    public class BBinCheckBalanceResponse {
        public bool result { set; get; }
        public List<Member> data { set; get; }
        public PageM pagination { set; get; }
    }

    public class BBinBalanceResponse {
        public bool result { set; get; }
        public object data { set; get; }
    }
    public class GameUrl {
        public string pc { set; get; }
        public string mobile { set; get; }
        public string rwd { set; get; }
    }
    public class ErrorData {
        public string Code { set; get; }
        public string Message { set; get; }
    }

    public class Member {
        public string LoginName { set; get; }
        public string Currency { set; get; }
        public decimal Balance { set; get; }
        public decimal TotalBalance { set; get; }
    }

    public class PageM {
        //pagination":{"Page":页
        //数,"PageLimit":每页数量,"TotalNumber":总数,"TotalPage":总页数
        public int Page { set; get; }
        public int PageLimit { set; get; }
        public int TotalNumber { set; get; }
        public int TotalPage { set; get; }
    }
}
