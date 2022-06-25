using System;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class CVReviewByHRMDto
    {
        public int CVReviewId { get; set; }
        public int ChecklistHRId { get; set; }
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        //public int ReviewResultId { get; set; }
        public int AssignedToId { get; set; }
        public int HRMId {get; set;}
        public EnumSelStatus HRMReviewResultId { get; set; }
        public int HRMTaskId { get; set; }
        public DateTime HRMReviewedOn { get; set; }
        public string HRMRemarks { get; set; }
        public int DocControllerAdminTaskId {get; set;} 
        public ApplicationTask ParentTask { get; set; }
        public int ParentTaskId { get; set; }
        //public bool NoReviewBySupervisor { get; set; }
        public EnumTaskType enumTaskType { get; set; }
        public int TaskOwnerId { get; set; }    //doc controller
        public CommonDataDto CommonDataDto {get; set;}
    }
}