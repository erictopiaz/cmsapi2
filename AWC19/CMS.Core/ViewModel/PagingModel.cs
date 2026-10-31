using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core.ViewModel
{
    public class PagingModel
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

    public class PagingModel<T>
    {
        public IEnumerable<T> Data { get; set; }

        public long TotalItems { get; set; }

        public TotalAmount totalAmount { get; set; } = new TotalAmount();
        public string? MiscData { get; set; } = "";

    }

    public  class TotalAmount {
        public float item_1 { get; set; } = 0f;
    }
}
