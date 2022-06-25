using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Admin;

namespace core.Interfaces
{
    public interface ICustomerReviewService
    {
        Task<CustomerReview> GetOrAddCustomerReview (int CustomerId);
        Task<CustomerReview> AddCustomerReview(int CustomerId, string CustomerName, string CustomerReviewStatus, string Remarks, ICollection<CustomerReviewItem> CustomerReviewItems);
        Task<bool> ApproveCustomerReviewTransaction(int CustomerReviewItemId, bool Approved, int ApprovedByUserId);

        Task<bool> AddNewCustomerReviewItem(int CustomerId, DateTime ReviewDate, int UserId, int CustomerReviewDataId, string Remarks);
        Task<bool> EditCustomerReviewItem(CustomerReviewItem customerReviewItem);
        Task<bool> DeleteCustomerReviewItem(CustomerReviewItem customerReviewItem);
        Task<ICollection<CustomerReviewData>> GetCustomerReviewStatusData();
        
        Task<bool> EditCustomerReview (CustomerReview customerReview);


    }
}