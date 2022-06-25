using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class SelDecisionToAddDto
    {
        public int CVRefId { get; set; }
        public DateTime DecisionDate { get; set; }
        public int SelectionStatusId { get; set; }
        public string Remarks { get; set; }
        
        //public Employment employment {get; set;}
    }
}