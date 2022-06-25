using System;
using System.Collections.Generic;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class OrderBriefDto
    {
        public int Id { get; set; }
        public int ContractReviewId { get; set; }
        public int OrderNo { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } 
        public string CityOfWorking {get; set;}
        public DateTime CompleteBy { get; set; }
        public string ReviewStatus { get; set; }
        public string Status {get; set;}
        public string ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public DateTime ForwardedToHRDeptOn {get; set;}

    }
}