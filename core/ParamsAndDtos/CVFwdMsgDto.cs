using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CVFwdMsgDto
    {
          public CVFwdMsgDto()
          {
          }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public int OfficialId { get; set; }
        public string OfficialTitle { get; set; }
        public string OfficialName { get; set; }
        public int OfficialUserId { get; set; }
        public string Designation { get; set; }
        public string OfficialEmail { get; set; }
        public int OrderItemId {get; set;}
        public int OrderNo { get; set; }
        public DateTime OrderDated { get; set; }
        public int ItemSrNo { get; set; }
        public string CategoryName { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string PPNo { get; set; }
        public int CumulativeSentSoFar { get; set; }
        public EnumCandidateAssessmentResult AssessmentGrade { get; set; }
    }
}