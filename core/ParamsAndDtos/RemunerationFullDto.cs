using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.Orders
{
    public class RemunerationFullDto
    {
          public RemunerationFullDto()
          {
          }


        public int Id {get; set;}
        public DateTime OrderDate {get; set;}
        public string CustomerName {get; set;}
        public string CategoryName {get; set;}
        public int OrderItemId {get; set;}
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public int CategoryId {get; set;}
        public int WorkHours { get; set; }
        public string SalaryCurrency { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public int ContractPeriodInMonths { get; set; }=24;
        public bool HousingProvidedFree { get; set; }
        public int HousingAllowance { get; set; }
        public bool HousingNotProvided { get; set; }
        public bool FoodProvidedFree { get; set; }
        public int FoodAllowance { get; set; }
        public bool FoodNotProvided { get; set; }
        public bool TransportProvidedFree { get; set; }
        public int TransportAllowance { get; set; }
        public bool TransportNotProvided { get; set; }
        public int OtherAllowance { get; set; }
        public int LeavePerYearInDays { get; set; }
        public int LeaveAirfareEntitlementAfterMonths { get; set; }
        
    }
}