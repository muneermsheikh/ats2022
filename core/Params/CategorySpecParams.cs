using System;
using core.Params;

namespace core.Params
{
    public class CategorySpecParams: ParamPages
    {
        public int? CategoryId { get; set; }
        public string CategoryNameLike {get; set;}
        //public int? IndustryId {get; set;}
    }
}