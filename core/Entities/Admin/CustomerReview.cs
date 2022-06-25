using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class CustomerReview: BaseEntity
    {
        public CustomerReview()
        {
        }

        public CustomerReview(int customerId, string customerName, string currentStatus, string remarks, ICollection<CustomerReviewItem> customerReviewItems)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            CurrentStatus = currentStatus;
            Remarks = remarks;
            CustomerReviewItems = customerReviewItems;
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CurrentStatus {get; set;} = "In Service";
        public string Remarks { get; set; }
        public ICollection<CustomerReviewItem> CustomerReviewItems { get; set; }
    }
}