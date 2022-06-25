using System;
using System.Linq;
using core.Entities.HR;
using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
     public class InterviewSpecs : BaseSpecification<Interview>
     {
          public InterviewSpecs(InterviewSpecParams IParams)
            : base(x => 
                (!IParams.InterviewId.HasValue || x.Id == IParams.InterviewId) &&
                (!IParams.InterviewItemId.HasValue || 
                  x.InterviewItems.Select(x => x.Id == IParams.InterviewItemId).FirstOrDefault()) 
                && (!IParams.OrderId.HasValue || x.OrderId == IParams.OrderId)
                && (!IParams.OrderItemId.HasValue || 
                  x.InterviewItems.Select(x => x.OrderItemId == IParams.OrderItemId).FirstOrDefault() )
                && (!IParams.CustomerId.HasValue || x.CustomerId==IParams.CustomerId)
                && (!IParams.CategoryId.HasValue || 
                  x.InterviewItems.Select(x => x.CategoryId == IParams.CategoryId).FirstOrDefault())
                && (string.IsNullOrEmpty(IParams.InterviewStatus) || x.InterviewStatus.ToLower() == IParams.InterviewStatus.ToLower())
                && (string.IsNullOrEmpty(IParams.CustomerName) || x.CustomerName.ToLower().Contains(IParams.CustomerName.ToLower()))
             )
          {
              if (IParams.IncludeItems) AddInclude(x => x.InterviewItems);
              
              if (IParams.PageSize > 0) ApplyPaging(IParams.PageSize * (IParams.PageIndex - 1), IParams.PageSize);

              if (!string.IsNullOrEmpty(IParams.Sort)) {
                switch(IParams.Sort.ToLower()) {
                  case "orderid":
                    AddOrderBy(x => x.OrderId);
                    break;
                  case "orderitemid":
                    AddOrderByDescending(x => x.InterviewItems.OrderByDescending(x => x.OrderItemId));
                    break;
                  case "categoryid":
                    AddOrderBy(x => x.InterviewItems.OrderBy(x => x.CategoryId));
                    break;
                  case "customerid":
                    AddOrderBy(x => x.CustomerId);
                    break;
                  case "status":
                    AddOrderBy(x => x.InterviewStatus);
                    break;
                  default:
                    break;
                }
              }
          }

     }
}