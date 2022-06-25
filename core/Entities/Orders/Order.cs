using System;
using System.Collections.Generic;
using core.Entities.Identity;

namespace core.Entities.Orders
{
    public class Order: BaseEntity
    {
          private List<OrderItem> items;
          private string buyerEmail;
          private int subtotal;

          public Order()
          {
          }

          public Order(int orderNo, int customerId, string customerName, string cityOfWorking, string orderRef, DateTime orderRefDate,
            int salesmanId, int estimatedRevenue, DateTime completeBy, ICollection<OrderItem> orderItems)
          {
               OrderNo = orderNo;
               CustomerId = customerId;
               CustomerName = customerName;
               CityOfWorking = cityOfWorking;
               OrderRef = orderRef;
               OrderRefDate = orderRefDate;
               SalesmanId = salesmanId;
               CompleteBy = completeBy;
               EstimatedRevenue = estimatedRevenue;
               OrderItems = orderItems;
          }

        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }=DateTime.Now;
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}
        public string BuyerEmail {get; set;}
        public string OrderRef { get; set; }
        public DateTime OrderRefDate {get; set;}
        public string SalesmanName { get; set; }
        public int ProjectManagerId { get; set; }
        public int? MedicalProcessInchargeEmpId { get; set; }
        public int? VisaProcessInchargeEmpId { get; set; }
        public int? EmigProcessInchargeId { get; set; }
        public int? TravelProcessInchargeId { get; set; }
        public int? SalesmanId { get; set; }
        
        //public string AppUserId { get; set; }
        public DateTime CompleteBy { get; set; }
        public string Country { get; set; }
        public string CityOfWorking { get; set; }
        public int ContractReviewId { get; set; }
        public int ContractReviewStatusId {get; set;}
        public int EstimatedRevenue {get; set;}
        public Customer Customer {get; set;}
        public EnumOrderStatus Status { get; set; } = EnumOrderStatus.AwaitingReview;
        public DateTime? ForwardedToHRDeptOn { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        
    }
}