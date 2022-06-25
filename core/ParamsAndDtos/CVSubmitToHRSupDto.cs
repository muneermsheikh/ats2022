using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class CVSubmitToHRSupDto
    {
        public int ChecklistHRId { get; set; }
        public int TaskOwnerId { get; set; }
        [Required]
        public int AssignedToId { get; set; }
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public bool NoReviewBySupervisor {get; set;}
        public int Charges {get; set;}
        public ApplicationTask ParentTask { get; set; }
        public EnumTaskType enumTaskType {get; set;}
        public CommonDataDto CommonDataDto {get; set;}
    }
}