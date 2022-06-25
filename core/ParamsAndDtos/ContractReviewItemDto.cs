using System;
using System.Collections.Generic;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class ContractReviewItemDto
    {
          public ContractReviewItemDto()
          {
          }

          public ContractReviewItemDto(int id, int contractReviewId, int orderId, int orderItemId,  int srNo, string professionName,  
            string sourceFrom, int quantity, bool ecnr, bool requireAssess,  ICollection<ReviewItem> reviewIems, int reviewItemStatus)
        {
            Id = id;
            ContractReviewId = contractReviewId;
            OrderId = orderId;
            OrderItemId = orderItemId;
            SrNo = srNo;
            CategoryName = professionName;
            Quantity = quantity;
            Ecnr = ecnr;
            RequireAssess = requireAssess;
            ReviewItems = reviewIems;
            ReviewItemStatus = reviewItemStatus;
        }
 
        public int Id { get; set; }
        public int SrNo { get; set; }
        public int ContractReviewId {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate {get; set;}
        public int OrderItemId { get; set; }
        public string CustomerName {get; set;}
        public string CategoryName {get; set;}
        public string SourceFrom { get; set; }
        public bool RequireAssess { get; set; }
        public bool Ecnr {get; set;}
        public int Quantity {get; set; }
        public DateTime CompleteBefore { get; set; }
        public int ReviewItemStatus {get; set;}
        public ICollection<ReviewItem> ReviewItems {get; set;}
      
    }
}