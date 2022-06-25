using System.ComponentModel.DataAnnotations;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class CVReviewSubmitByHRExecDto
    {
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public string ExecRemarks { get; set; }
        public int Charges { get; set; }
        public int AssignedToId { get; set; }
        public CommonDataDto CommonDataDto {get; set;}      //passed on as null to api
    }
}