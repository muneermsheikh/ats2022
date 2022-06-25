using System;
using System.Linq;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrdersBriefSpecs: BaseSpecification<Order>
    {
        public OrdersBriefSpecs(OrdersSpecParams specParams) 
            : base(x =>
            (!specParams.OrderId.HasValue || x.Id == specParams.OrderId)
                && (string.IsNullOrEmpty(specParams.Search) || 
                  x.CustomerName.ToLower().Contains(specParams.Search.ToLower()) )
                && ((!specParams.OrderNoFrom.HasValue && !specParams.OrderNoUpto.HasValue)||
                    x.OrderNo >= specParams.OrderNoFrom &&
                    x.OrderNo <= specParams.OrderNoUpto) 
                && (string.IsNullOrEmpty(specParams.CityOfWorking) || x.CityOfWorking.ToLower() == specParams.CityOfWorking.ToLower())
                /* && (!specParams.OrderDateFrom.HasValue && specParams.OrderDateUpto.HasValue || 
                    x.OrderDate.Date ==specParams.OrderDateFrom) 
                && (!specParams.OrderDateFrom.HasValue && !specParams.OrderDateUpto.HasValue ||
                    x.OrderDate.Date <= specParams.OrderDateUpto && x.OrderDate.Date >= specParams.OrderDateFrom)
                */
            )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex -1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "orderno": AddOrderBy(p => p.OrderNo);
                        break;
                    case "ordernodesc": AddOrderByDescending(p => p.OrderNo);
                        break;
                    case "orderdate": AddOrderBy(p => p.OrderDate);
                        break;
                    case "orderdatedesc": AddOrderByDescending(p => p.OrderDate);
                        break;
                    case "city": AddOrderBy(p => p.CityOfWorking);
                        break;
                    case "citydesc": AddOrderByDescending(p => p.CityOfWorking);
                        break;
                    case "country": AddOrderBy(p => p.Country);
                        break;
                    case "countrydesc": AddOrderByDescending(p => p.Country);
                        break;
                    case "status": AddOrderBy(p => p.Status);
                        break;
                    case "statusdesc": AddOrderByDescending(p => p.Status);
                        break;
                    default:
                        AddOrderBy(n => n.OrderNo);
                        break;
                }
            }
        }

        public OrdersBriefSpecs(int customerid) 
            : base(o => o.CustomerId == customerid)
        {
        }
        public OrdersBriefSpecs()             
        {
        }

    }
}