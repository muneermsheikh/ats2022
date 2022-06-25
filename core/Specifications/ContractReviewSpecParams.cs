using System;
using System.Collections.Generic;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class ContractReviewSpecParams: ParamPages
    {
        public int? OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? CustomerId {get; set;}
        public string OrderIds {get; set;}
        public ICollection<int> OrderIdInts {get; set;}
        public DateTime? OrderDate {get; set;}
        public EnumReviewItemStatus? ReviewStatus {get; set;}
    }
}