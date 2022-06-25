using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class OrderAssessmentQObj
    {
        public int EmployeeId { get; set; }
        public int AppUserId {get; set;}
        public string AppUserEmail { get; set; }
        public string EmployeeFullName {get; set;}
        public string AppUserName {get; set;}
        public string AppUserPhoneNo {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }

        public string AssessmentQDesignTaskText {get; set;}
        public ICollection<OrderAssessmentQDetails> AssessmentQDetails { get; set; }
    }
    public class OrderAssessmentQDetails 
    {
        public int SrNo { get; set; }
        public int OrderItemId { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public string JDUrl { get; set; }
    }
}