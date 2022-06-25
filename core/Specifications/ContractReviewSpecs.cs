using core.Entities.Orders;

namespace core.Specifications
{
     public class ContractReviewSpecs: BaseSpecification<ContractReview>
    {
        public ContractReviewSpecs(ContractReviewSpecParams cParams)
            : base(x => 
                (!cParams.OrderId.HasValue || x.OrderId == cParams.OrderId) 
                //&& (!cParams.OrderItemId.HasValue || x.OrderItemId == cParams.OrderItemId)
                && (cParams.OrderIdInts.Count == 0) || cParams.OrderIdInts.Contains(x.OrderId)
                && (!cParams.ReviewStatus.HasValue || x.RvwStatusId == (int)cParams.ReviewStatus)
            )
        {
            ApplyPaging(cParams.PageSize * (cParams.PageIndex -1), cParams.PageSize);

            if (!string.IsNullOrEmpty(cParams.Sort))
            {
                switch (cParams.Sort)
                {
                    case "orderid": AddOrderBy(p => p.OrderId);
                        break;
                    case "orderiddesc": AddOrderByDescending(p => p.OrderId);
                        break;
                    case "orderdate": AddOrderBy(p => p.OrderDate);
                        break;
                    case "orderdatedesc": AddOrderByDescending(p => p.OrderDate);
                        break;
                    case "status": AddOrderBy(p => p.RvwStatusId);
                        break;
                    case "statusdesc": AddOrderByDescending(p => p.RvwStatusId);
                        break;
                    default:
                        AddOrderBy(n => n.OrderId);
                        break;
                }
            }
        }

        
      }
}