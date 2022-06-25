using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;

namespace core.Entities.HR
{
    public class CandidateAssessment: BaseEntity
    {
        public CandidateAssessment()
          {
          }

          public CandidateAssessment(int candidateId, int orderItemId, int assessedById, string assessedByName, 
              DateTime assessedOn, bool requirereview, int hrchecklistid, EnumCandidateAssessmentResult assessmentResult)
          {
              CandidateId = candidateId;
              OrderItemId = orderItemId;
              AssessedById = assessedById;
              AssessedByName = assessedByName;
              AssessedOn = assessedOn;
              requireInternalReview = requirereview;
              ChecklistHRId = hrchecklistid;
              AssessResult=assessmentResult;
          }

          public CandidateAssessment(int candidateId, int orderItemId, int assessedById, string assessedByName, DateTime assessedOn, bool requirereview, int hrchecklistid,
            ICollection<CandidateAssessmentItem> candidateassessmentitems)
          {
               CandidateId = candidateId;
               OrderItemId = orderItemId;
               AssessedById = assessedById;
               AssessedByName = assessedByName;
               AssessedOn = assessedOn;
               CandidateAssessmentItems = candidateassessmentitems;
               requireInternalReview=requirereview;
          }

        [Required]
        public int OrderItemId { get; set; }
        public int AssessedById { get; set; }
        public string AssessedByName {get; set;}
        [Required]
        public int CandidateId {get; set;}
        public bool requireInternalReview {get; set;}
        public int ChecklistHRId {get; set;}
        //public int UserProfessionId { get; set; }
        public DateTime AssessedOn { get; set; }
        [Required]
        public EnumCandidateAssessmentResult AssessResult { get; set; }
        public string Remarks { get; set; }
        public int CvRefId { get; set; }
        public int TaskIdDocControllerAdmin {get; set;}
        public OrderItem OrderItem { get; set; }
        public ICollection<CandidateAssessmentItem> CandidateAssessmentItems { get; set; }
    }
}