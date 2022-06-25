using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class CVRefPostedDto
    {
        public int OrderItemId { get; set; }
        public int OrderNo {get; set;}
        public string CustomerName {get; set;}
        public string CategoryName {get; set;}
        public int ApplicationNo {get; set;}
        public string CandidateName {get; set;}
        public DateTime ReferredOn { get; set; }
        public int Charges {get; set;}
        public EnumCVRefStatus RefStatus { get; set; }
    }
}