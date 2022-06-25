using core.Entities.Orders;

namespace core.Specifications
{
     public class ContractReviewItemSpecs: BaseSpecification<ContractReviewItem>
    {
        public ContractReviewItemSpecs(ContractReviewItemSpecParams cParams)
            : base(x => 
                (cParams.OrderItemIds.Count > 0 || cParams.OrderItemIds.Contains(x.OrderItemId))
                && (!cParams.ReviewItemStatus.HasValue || x.ReviewItemStatus == (int)cParams.ReviewItemStatus)
            )
        {
            
            //ApplyPaging(cParams.PageSize * (cParams.PageIndex -1), cParams.PageSize);


            if (!string.IsNullOrEmpty(cParams.Sort))
            {
                switch (cParams.Sort)
                {
                    case "orderitemid": AddOrderBy(p => p.OrderItemId);
                        break;
                    case "orderitemiddesc": AddOrderByDescending(p => p.OrderItemId);
                        break;
                    case "status": AddOrderBy(p => p.ReviewItemStatus);
                        break;
                    case "statusdesc": AddOrderByDescending(p => p.ReviewItemStatus);
                        break;
                    default:
                        AddOrderBy(n => n.OrderItemId);
                        break;
                }
            }
        }

        
      }
}