using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Orders;

namespace core.Entities.HR
{
    public class Employment: BaseEntity
    {
        public Employment()
        {
        }

          public Employment(int cVRefId, DateTime selectedOn, string salaryCurrency, int salary, 
                int contractPeriodInMonths, bool housingProvidedFree, int housingAllowance, bool foodProvidedFree, 
                int foodAllowance, bool transportProvidedFree, int transportAllowance, int otherAllowance, 
                int leavePerYearInDays, int leaveAirfareEntitlementAfterMonths, int charges)
          {
               CVRefId = cVRefId;
               SelectedOn = selectedOn;
               SalaryCurrency = salaryCurrency;
               Salary = salary;
               ContractPeriodInMonths = contractPeriodInMonths;
               HousingProvidedFree = housingProvidedFree;
               HousingAllowance = housingAllowance;
               FoodProvidedFree = foodProvidedFree;
               FoodAllowance = foodAllowance;
               TransportProvidedFree = transportProvidedFree;
               TransportAllowance = transportAllowance;
               OtherAllowance = otherAllowance;
               LeavePerYearInDays = leavePerYearInDays;
               LeaveAirfareEntitlementAfterMonths = leaveAirfareEntitlementAfterMonths;
                Charges = charges;
          }

          public int CVRefId { get; set; }
          public int SelectionDecisionId {get; set;}
          public DateTime SelectedOn { get; set; }
          public int Charges {get; set;}
          public string SalaryCurrency { get; set; }
          public int Salary { get; set; }
          [Range(1,36)]
          public int ContractPeriodInMonths { get; set; }=24;
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
          public string OfferAttachmentUrl { get; set; }
          public string OfferAcceptanceUrl { get; set; }
          //related - to delete once navigation properties can be activated
          public string CategoryName { get; set; }
          public int OrderItemId { get; set; }
          public int OrderId {get; set;}
          public int OrderNo {get; set;}
          public int CustomerId {get; set;}
          public string CustomerName {get; set;}
          public int CategoryId {get; set;}
          public int CandidateId { get; set; }
          public int ApplicationNo { get; set; }
          public string CandidateName { get; set; }
          public string CompanyName { get; set; }

          //[ForeignKey("SelectionDecisionId")]
          //public SelectionDecision SelectionDecision {get; set;}
      }
}