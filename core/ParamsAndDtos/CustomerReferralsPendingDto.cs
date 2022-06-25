using System;

namespace core.ParamsAndDtos
{
    public class CustomerReferralsPendingDto
    {
        public int CVReviewId { get; set; }
        public int OrderItemId { get; set; }
        public string CategoryRef { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string CustomerName { get; set; }
        public DateTime SentToDocControllerOn { get; set; }
        public int DocControllerAdminTaskId { get; set; }
        
    }
}