using System.ComponentModel.DataAnnotations;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class CVReviewBySupDto
    {
        public int CVReviewId { get; set; }
        public int ChecklistHRId { get; set; }
        public int ReviewResultId {get; set;}
        public int AssignedToId { get; set; }
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public int ParentTaskId { get; set; }
        public string SupRemarks { get; set; }
        public ApplicationTask ParentTask { get; set; }
        public EnumTaskType enumTaskType { get; set; }
        public bool NoReviewBySupervisor { get; set; }
        public int Charges { get; set; }
        public int TaskOwnerId { get; set; }
        public CommonDataDto CommonDataDto {get; set;}
    }
}