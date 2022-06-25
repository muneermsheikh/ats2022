using System;
using System.Linq;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrderItemSpecs: BaseSpecification<OrderItem>
    {
        public OrderItemSpecs(OrderItemSpecParams specParams) 
            : base(x =>
                (!specParams.Id.HasValue || x.Id == specParams.Id) &&
                (!specParams.OrderNo.HasValue || x.OrderNo  == specParams.OrderNo) &&
                (!specParams.SrNo.HasValue || (int)x.SrNo == specParams.SrNo) &&
                (!specParams.OrderId.HasValue || (int)x.OrderId == specParams.OrderId)
                ) 
        {
            AddOrderBy(x => x.SrNo);
        }
        public OrderItemSpecs(int orderid) 
            : base(x => x.OrderId == orderid) 
        {
            AddOrderBy(x => x.SrNo);
        }
        
    }
}