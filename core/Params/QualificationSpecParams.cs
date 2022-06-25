using System;
using core.Params;

namespace core.Params
{
    public class QualificationSpecParams: ParamPages
    {
        public int? QualificationId { get; set; }
        public string QualificationNameLike {get; set;}
    }
}