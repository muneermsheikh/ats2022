using System;
using System.Linq;
using core.Entities;
using core.Entities.Orders;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class RemunerationForCountSpecs: BaseSpecification<Remuneration>
    {
        public RemunerationForCountSpecs(RemunerationSpecParams rParams)
            : base(x => 
                (!rParams.OrderId.HasValue || x.OrderId == rParams.OrderId) &&
                (!rParams.OrderItemId.HasValue || x.OrderItemId == rParams.OrderItemId) &&
                (!rParams.OrderNo.HasValue || x.OrderNo == rParams.OrderNo)
                )
        {
        }

        public RemunerationForCountSpecs(int orderItemId) 
            : base(x => x.OrderItemId == orderItemId)
        {
        }
        
      }
}