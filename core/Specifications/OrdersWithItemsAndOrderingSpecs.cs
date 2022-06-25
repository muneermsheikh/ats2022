using System;
using System.Linq;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrdersWithItemsAndOrderingSpecs: BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingSpecs(OrdersSpecParams specParams) 
            : base(x =>
                (!specParams.Id.HasValue || x.Id == specParams.Id) &&
                (!specParams.CustomerId.HasValue || x.CustomerId  == specParams.CustomerId) &&
                (!specParams.Status.HasValue || (int)x.Status == specParams.Status) &&
                (string.IsNullOrEmpty(specParams.CityOfWorking) || x.CityOfWorking.ToLower() == specParams.CityOfWorking.ToLower() )
                /* &&
                (!specParams.OrderDateFrom.HasValue && specParams.OrderDateUpto.HasValue || 
                    x.OrderDate.Date ==specParams.OrderDateFrom) &&
                (!specParams.OrderDateFrom.HasValue && !specParams.OrderDateUpto.HasValue ||
                    x.OrderDate.Date <= specParams.OrderDateUpto && x.OrderDate.Date >= specParams.OrderDateFrom)
                */
            )
        {
            AddInclude(o => o.OrderItems);
            ApplyPaging(specParams.PageSize * (specParams.PageIndex -1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "OrderNoAsc": AddOrderBy(p => p.OrderNo);
                        break;
                    case "OrderDesc": AddOrderByDescending(p => p.OrderNo);
                        break;
                    /* 
                    todo - SQLITE cannot order on dates
                    case "OrderDateAsc": AddOrderBy(p => p.OrderDate);
                        break;
                    case "OrderDateDesc": AddOrderByDescending(p => p.OrderDate);
                        break;
                    */
                    case "CityAsc": AddOrderBy(p => p.CityOfWorking);
                        break;
                    case "CityDesc": AddOrderByDescending(p => p.CityOfWorking);
                        break;
                    case "CountryAsc": AddOrderBy(p => p.Country);
                        break;
                    case "CountryDesc": AddOrderByDescending(p => p.Country);
                        break;
                    case "StatusAsc": AddOrderBy(p => p.Status);
                        break;
                    case "StatusDesc": AddOrderByDescending(p => p.Status);
                        break;
                    default:
                        AddOrderBy(n => n.OrderNo);
                        break;
                }
            }
        }

        public OrdersWithItemsAndOrderingSpecs(string customerid) 
            : base(o => o.CustomerId == Convert.ToInt32(customerid))
        {
            AddInclude(o => o.OrderItems);
        }
        public OrdersWithItemsAndOrderingSpecs()             
        {
            AddInclude(o => o.OrderItems);
            AddOrderBy(o => o.OrderNo);
        }

    }
}