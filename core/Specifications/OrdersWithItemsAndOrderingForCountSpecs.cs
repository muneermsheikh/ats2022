using System;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrdersWithItemsAndOrderingForCountSpecs: BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingForCountSpecs(OrdersSpecParams specParams) 
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
        }

        public OrdersWithItemsAndOrderingForCountSpecs(string customerid) 
            : base(o => o.CustomerId == Convert.ToInt32(customerid))
        {
        }
        public OrdersWithItemsAndOrderingForCountSpecs()             
        {
        }
    }
}