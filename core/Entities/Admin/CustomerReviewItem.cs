using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class CustomerReviewItem: BaseEntity
    {
        public CustomerReviewItem()
        {
        }

        public CustomerReviewItem(int customerReviewId, DateTime reviewTransactionDate, int userId, int customerReviewDataId, 
        string remarks)
        {
            CustomerReviewId = customerReviewId;
            ReviewTransactionDate = reviewTransactionDate;
            UserId = userId;
            CustomerReviewDataId = customerReviewDataId;
            Remarks = remarks;
        }

        public int CustomerReviewId { get; set; }
        public DateTime ReviewTransactionDate { get; set; }
        public int UserId { get; set; }
        public int CustomerReviewDataId { get; set; }
        public string Remarks { get; set; }
        public bool ApprovedBySup { get; set; }
        public int ApprovedById {get; set;}

    }
}