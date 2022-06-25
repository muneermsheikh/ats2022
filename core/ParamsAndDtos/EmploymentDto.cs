using System;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class EmploymentDto
    {
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string CompanyName { get; set; }
        public string CategoryRef { get; set; }
        public Employment Employment {get; set;}

        /*
        public int Id { get; set; }
        public int CVRefId { get; set; }
        public int SelectionDecisionId { get; set; }
        public int Charges {get; set;}
        public string SalaryCurrency { get; set; }
        public int Salary { get; set; }
        public int ContractPeriodInMonths { get; set; }
        public bool HousingProvidedFree { get; set; }
        public int HousingAllowance { get; set; }
        public bool FoodProvidedFree { get; set; }
        public int FoodAllowance { get; set; }
        public bool TransportProvidedFree { get; set; }
        public int TransportAllowance { get; set; }
        public int OtherAllowance { get; set; }
        public int LeavePerYearInDays { get; set; }
        public int LeaveAirfareEntitlementAfterMonths {get; set;}
        public DateTime OfferAcceptedOn { get; set; }
        public string Remarks { get; set; }
        */

    }
}