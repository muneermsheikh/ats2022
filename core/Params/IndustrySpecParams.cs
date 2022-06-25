using System;
using core.Params;

namespace core.Params
{
    public class IndustrySpecParams: ParamPages
    {
        public int? IndustryId { get; set; }
        public string IndustryNameLike {get; set;}
        //public int? ProfessionId {get; set;}

    }
}