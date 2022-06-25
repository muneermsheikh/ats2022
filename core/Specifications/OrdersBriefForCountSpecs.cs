using System;
using System.Linq;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrdersBriefForCountSpecs: BaseSpecification<Order>
    {
        public OrdersBriefForCountSpecs(OrdersSpecParams specParams) 
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
        }

        public OrdersBriefForCountSpecs(int customerid) 
            : base(o => o.CustomerId == customerid)
        {
        }
        public OrdersBriefForCountSpecs()             
        {
        }

    }
}