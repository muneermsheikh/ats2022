using System;
using System.Collections.Generic;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class OrderBriefDtoR
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } 
        public ICollection<OrderItem_Dto> Items {get; set;}
    }

    public class OrderItem_Dto
    {
        public int OrderId {get; set;}
        public int OrderItemId {get; set;}
        public string CategoryRef { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public bool Checked { get; set; }
    }
}

