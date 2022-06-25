using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.HR
{
    public class CVRvw: BaseEntity
    {
        public CVRvw()
        {
        }

        public CVRvw(int? checklistHRId, int candidateId, bool ecnr, int orderItemId, int orderId, int hrExecutiveId, 
        int hrExecTaskId, int hrSupId, int hrmId, DateTime submittedByHRExecOn, string hRExecRemarks)
        {
            ChecklistHRId = checklistHRId;
            CandidateId = candidateId;
            Ecnr = ecnr;
            OrderItemId = orderItemId;
            OrderId = orderId;
            HRExecutiveId = hrExecutiveId;
            HRExecTaskId = hrExecTaskId;
            SubmittedByHRExecOn = submittedByHRExecOn;
            HRExecRemarks = hRExecRemarks;
            HRSupId = hrSupId;
            HRMId = hrmId;
        }

        [Required]
        public int HRExecutiveId { get; set; }
        [Required]
        public int HRExecTaskId { get; set; }
        public int? ChecklistHRId { get; set; }      //checklistHR may not be mandatory, so this has to allow null/zero
        [Required]
        public int CandidateId { get; set; }
        public bool Ecnr { get; set; }
        public int Charges { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime SubmittedByHRExecOn { get; set; }=DateTime.Now;
        public string HRExecRemarks { get; set; }
        public bool NoReviewBySupervisor { get; set; }
        public int HRSupId {get; set;}
        public DateTime? ReviewedBySupOn { get; set; }
        public EnumSelStatus? SupReviewResultId { get; set; } = EnumSelStatus.NotReviewed;
        public int? SupTaskId { get; set; }
        public string SupRemarks { get; set; }
    //
        public int? HRMId {get; set;}
        public DateTime? HRMReviewedOn { get; set; }
        public EnumSelStatus? HRMReviewResultId { get; set; }=EnumSelStatus.NotReviewed;
        public int? HRMTaskId { get; set; }
        public string HRMRemarks { get; set; }
        public int DocControllerAdminEmployeeId { get; set; }
        public int? DocControllerAdminTaskId {get; set;}     //for CV Fwd
        public DateTime? CVReferredOn { get; set; }
        public int? CVRefId { get; set; }
    }
}