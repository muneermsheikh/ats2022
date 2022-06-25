using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CVRvwDto
    {
        public int Id { get; set; }
        public int HRExecutiveId { get; set; }
        public int? ChecklistHRId { get; set; }      
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int HRExecTaskId { get; set; }
        public string HRExecRemarks { get; set; }
      
        public DateTime SubmittedByHRExecOn { get; set; }
    //
        public int HRSupId {get; set;}
        public DateTime ReviewedBySupOn { get; set; }
        public EnumSelStatus SupReviewResultId { get; set; } 
        public int SupTaskId { get; set; }
        public string SupRemarks { get; set; }
        
        public int HRMId {get; set;}
        public EnumSelStatus HRMReviewResultId { get; set; }
        public int HRMTaskId { get; set; }
        public DateTime HRMReviewedOn { get; set; }
        public string HRMRemarks { get; set; }
        public int DocControllerAdminTaskId {get; set;}     //for CV Fwd
    }
}