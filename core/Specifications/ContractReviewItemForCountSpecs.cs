using System;
using System.Linq;
using core.Entities;
using core.Entities.Orders;
using core.Entities.Users;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class ContractReviewItemForCountSpecs: BaseSpecification<ContractReviewItem>
    {
        public ContractReviewItemForCountSpecs(ContractReviewItemSpecParams cParams)
            : base(x => 
                (cParams.OrderItemIds.Count == 0 || cParams.OrderItemIds.Contains(x.OrderItemId))
                && (!cParams.ReviewItemStatus.HasValue || x.ReviewItemStatus == (int)cParams.ReviewItemStatus)
            )
        {
        }

       

      }
}