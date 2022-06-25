using System;
using core.Params;

namespace core.Params
{
    public class UserProfessionsSpecParams: ParamPages
    {
        public int? CandidateId { get; set; }
        public int? CategoryId {get; set;}
        public int? IndustryId {get; set;}
        public bool? IsMain { get; set; }
    }
}