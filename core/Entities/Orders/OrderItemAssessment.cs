using System.Collections.Generic;

namespace core.Entities.Orders
{
    public class OrderItemAssessment: BaseEntity
    {
        public OrderItemAssessment()
        {
        }

        public OrderItemAssessment(int orderItemId, int orderId, int orderNo, 
            int categoryId, string categoryName, ICollection<OrderItemAssessmentQ> orderItemAssessmentQs)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            OrderNo = orderNo;
            CategoryId = categoryId;
            CategoryName = categoryName;
            OrderItemAssessmentQs = orderItemAssessmentQs;
        }

        public OrderItemAssessment(int orderItemId, int orderAssessmentId, int orderId, int categoryId)     //, string categoryName)
        {
            OrderAssessmentId = orderAssessmentId;
            OrderItemId = orderItemId;
            OrderId = orderId;
            CategoryId = categoryId;
            //CategoryName = categoryName;
        }

        public int OrderAssessmentId { get; set; }
        public int OrderItemId {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<OrderItemAssessmentQ> OrderItemAssessmentQs { get; set; }
        //public OrderAssessment OrderAssessment {get; set;}
    }
}