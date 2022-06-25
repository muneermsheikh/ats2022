using System;

namespace core.ParamsAndDtos
{
    public class CVRefPendingDto
    {
        public int CandidateId { get; set; }
        public string CandidateDetails { get; set; }
        public string CategoryRef { get; set; }
        public string ToBeReferredToCustomer { get; set; }
        public string CVApprovedByUsername { get; set; }
        public DateTime DateCVApprovedToForward { get; set; }
        public bool NoReviewBySupervisor { get; set; }
        public int HRSupId { get; set; }
        public int HRMId { get; set; }
        public int HRExecutiveId { get; set; }
    }
}