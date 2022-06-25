using System;
using System.Collections.Generic;
using core.Entities.MasterEntities;

namespace core.Entities.Orders
{
    public class ContractReview: BaseEntity
    {
        public ContractReview()
        {
        }

        public ContractReview(int enquiryId, int reviewedBy, int reviewStatusId)
        {
            OrderId = enquiryId;
            ReviewedBy = reviewedBy;
            ReviewedOn = DateTime.Now;
            RvwStatusId = reviewStatusId;
        }

        public ContractReview(int enquiryId, int enquiryNo, DateTime enquiryDate, 
            int customerId, string customerName)
        {
            OrderId = enquiryId;
            OrderNo = enquiryNo;
            OrderDate = enquiryDate;
            CustomerId = customerId;
            CustomerName = customerName;
        }

            public ContractReview(int enquiryId, int enquiryNo, DateTime enquiryDate, 
            int customerId, string customerName, ICollection<ContractReviewItem> reviewItems)
        {
            OrderId = enquiryId;
            OrderNo = enquiryNo;
            OrderDate = enquiryDate;
            CustomerId = customerId;
            CustomerName = customerName;
            ContractReviewItems=reviewItems;
        }

        public int OrderId { get; set; }
        public int OrderNo {get; set; }
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string CustomerName {get; set;}
        public int? ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; } = DateTime.Now;
        public int RvwStatusId { get; set; } = (int)EnumReviewStatus.NotReviewed;    //not reviewed
        //public ReviewStatus ReviewStatus {get; set;}
        public bool ReleasedForProduction { get; set; }=false;
        public ICollection<ContractReviewItem> ContractReviewItems {get; set; }
    }
}