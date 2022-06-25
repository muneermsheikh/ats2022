using System;
using System.Collections.Generic;
using core.Entities.Orders;
using core.ParamsAndDtos;

namespace core.ParamsAndDtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string CustomerName {get; set;}
        public string CityOfEmployment {get; set;}
        public int Subtotal { get; set; }
        public EnumOrderStatus Status { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<OrderItemToReturnDto> OrderItems { get; set; }
    }
}