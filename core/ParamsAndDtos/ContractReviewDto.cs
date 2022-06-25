using System;
using System.Collections.Generic;
using core.Entities.Orders;

namespace core.ParamsAndDtos
{
    public class ContractReviewDto
    {
          public ContractReviewDto()
          {
          }

          public ContractReviewDto(int id, int orderNo, DateTime orderDate, int customerId, string customerName, 
            DateTime completeBy, int reviewStatusId, string reviewStatus)
        {
            Id = id;
            //ContractReviewId = contractReviewId;
            OrderNo = orderNo;
            OrderDate = orderDate;
            CustomerName = customerName;
            //CompleteBy = completeBy;
            CustomerId = customerId;
            ReviewStatusId = reviewStatusId;
            ReviewStatus = reviewStatus;
        }

        public int Id { get; set; }
        //public int ContractReviewId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public int ReviewStatusId { get; set; }
        //public DateTime CompleteBy { get; set; }
        //[Ignore]
        public string ReviewStatus { get; set; }
        
        
        public ICollection<ContractReviewItem> ContractReviewItems {get; set;}
    }
}
