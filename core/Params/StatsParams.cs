using System;

namespace core.Params
{
    public class StatsParams: ParamPages
    {
        public string CurrentUserName { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerNameLike { get; set; }
        public int ApplicationNo { get; set; }
        public string ProfessionIds { get; set; }
        public string OrderBy { get; set; }

    }
}