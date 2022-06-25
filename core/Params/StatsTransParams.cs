using System;
using System.Collections.Generic;

namespace core.Params
{
    public class StatsTransParams: ParamPages
    {
        public string CurrentUserName { get; set; }
        public ICollection<int> OrderIds {get; set;}
        public ICollection<int> OrderNos {get; set;}
        public ICollection<int> OrderItemIds {get; set;}
        public bool IncludeDeploys { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderBy { get; set; }
    }
}