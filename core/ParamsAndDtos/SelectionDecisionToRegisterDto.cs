using System;

namespace core.ParamsAndDtos
{
    public class SelectionDecisionToRegisterDto
    {
        public int CVRefId { get; set; }
        public int CandidateId {get; set;}
        public DateTime DecisionDate {get; set;}
        public int SelectionStatusId { get; set; }
        public string SalaryCurrency {get; set;}
        public int Salary {get; set;}
        public int Charges {get; set;}
        public string Remarks {get; set;}
        
        public int OrderItemId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        public int OrderId { get; set; }
        public int OrderNo {get; set;}
        public int ApplicationNo {get; set;}
        public string CandidateName {get; set;}
        
    }
}