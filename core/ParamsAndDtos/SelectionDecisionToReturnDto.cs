using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class SelectionDecisionToReturnDto
    {
        //public int CVRefId { get; set; }
        //public int OrderItemId { get; set; }
        //public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        //public int OrderId { get; set; }
        public int OrderNo {get; set;}
        public int ApplicationNo {get; set;}
        //public int CandidateId {get; set;}
        public string CandidateName {get; set;}
        DateTime DecisionDate {get; set;}
        public int SelectionStatusId { get; set; }
        public EmploymentDto Employment {get; set;}
        public string Remarks {get; set;}
    }
}