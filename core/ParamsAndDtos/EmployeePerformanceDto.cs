using System;
using System.Runtime.Serialization;
using core.Entities.Orders;

//HR
//Employee Name  CVs submitted                      CVs approved                        CVs rejected 
//               Within  15 days 30 days >30 days   Within  15 days 30 days >30 days    Within  15 days 30 days >30 days
//Process
//Employee Name  Med Mobilized                      Visa Submitted                      Emig submitted                  Traveled
//               Within  15 days 30 days >30 days   Within  15 days 30 days >30 days    Within  15 days 30 days >30 days

//Admin
//Employee Name  Contract Reviewed                      CVs Forwarded
//               Within  15 days 30 days >30 days       Within  15 days 30 days >30 days

namespace core.ParamsAndDtos
{
    public class EmployeePerformanceDto
    {
        public int UserId { get; set; }
        public EnumTaskType TaskType { get; set; }
        //public string PerformanceType { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TaskItemId {get; set;}
        public int CandidateId {get; set;}
        public string CandidateName { get; set; }
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string CategoryName { get; set; }
        public int Quantity {get; set;}
    }

    //mapped to TaskType.cs
}