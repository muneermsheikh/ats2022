using System;
using System.Collections.Generic;
using core.Entities.Orders;

namespace api.DTOs
{
    public class aOrderToCreateDto
    {
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string OrderRef { get; set; }
        public DateTime CompleteBy { get; set; }
        public int? SalesmanId { get; set; }
        public string Remarks { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        
    }
}