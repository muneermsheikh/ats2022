using System;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class CVRefToAddDto
    {
        public string LoggedInAppUserId { get; set; }
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public int Charges { get; set; }
        public DateTime ReferredOn {get; set;}
        //public ApplicationTask hrAssignedTask {get; set;}
    }
}