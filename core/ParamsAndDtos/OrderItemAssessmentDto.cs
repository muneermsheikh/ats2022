using System.Collections.Generic;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class OrderItemAssessmentDto
    {
        public int Id { get; set; }
        public int OrderAssessmentId { get; set; }
        public int OrderItemId {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string CustomerName {get; set;}
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<OrderItemAssessmentQ> OrderItemAssessmentQs { get; set; }
    }
}