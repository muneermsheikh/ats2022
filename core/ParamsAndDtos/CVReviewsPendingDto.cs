using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CVReviewsPendingDto
    {
        public int CVReviewId { get; set; }
        public int HRExecutiveId { get; set; }
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime SubmittedByHRExecOn { get; set; }
        public DateTime? ReviewedBySupOn { get; set; }
        public EnumSelStatus SupReviewResultId { get; set; } 
        public string ReviewPendingByEmpName { get; set; }
        
    }
}