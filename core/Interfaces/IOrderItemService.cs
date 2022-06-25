using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Orders;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemBriefDto> GetOrderItemBriefDtoFromOrderItem(OrderItem orderItem);
        Task<OrderItemBriefDto> GetOrderItemRBriefDtoFromOrderItemId(int OrderItemId);
    }
}