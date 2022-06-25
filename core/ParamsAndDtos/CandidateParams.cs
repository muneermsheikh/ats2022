using System;
using core.Params;

namespace core.ParamsAndDtos
{
    public class CandidateParams: ParamPages
    {
        public int? CandidateId { get; set; }
        public int? ApplicationNoFrom {get; set;}
        public int? ApplicationNoUpto {get; set;}
        public int? ProfessionId {get; set;}
        public int? AppUserId {get; set;}
        public DateTime? RegisteredDate { get; set; }
        public DateTime? RegisteredUpto {get; set;}
        public string Email {get; set;}
        public string MobileNo {get; set;}
    }
}