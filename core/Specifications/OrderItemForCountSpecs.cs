using System;
using System.Linq;
using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
    public class OrderItemForCountSpecs: BaseSpecification<OrderItem>
    {
        public OrderItemForCountSpecs(OrderItemSpecParams specParams) 
            : base(x =>
                (!specParams.Id.HasValue || x.Id == specParams.Id) &&
                (!specParams.OrderNo.HasValue || x.OrderNo  == specParams.OrderNo) &&
                (!specParams.SrNo.HasValue || (int)x.SrNo == specParams.SrNo) &&
                (!specParams.OrderId.HasValue || (int)x.OrderId == specParams.OrderId)
                ) 
        {
        }

    }
}