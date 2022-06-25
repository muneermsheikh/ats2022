using System;
using core.Entities.HR;
using core.Params;

namespace core.ParamsAndDtos
{
    public class CandidateAssessmentParams: ParamPages
    {
        public int CandidateId {get; set;}
        public int OrderDetailId {get; set;}
        public bool requireInternalReview {get; set;}
        public EnumCandidateAssessmentResult AssessmentResult {get; set;}
        public int AssessedById {get; set;}
        public DateTime DateAssessed {get; set;}
        public int LoggedInIdentityUserId {get; set;}
    }
}