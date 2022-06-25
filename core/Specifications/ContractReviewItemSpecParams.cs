using System;
using System.Collections.Generic;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class ContractReviewItemSpecParams: ParamPages
    {
        public ICollection<int?> OrderItemIds {get; set;}
        public EnumReviewItemStatus? ReviewItemStatus {get; set;}
    }
}