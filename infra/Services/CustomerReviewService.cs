using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Admin;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class CustomerReviewService : ICustomerReviewService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private ICommonServices _commonService;
          public CustomerReviewService(IUnitOfWork unitOfWork, ATSContext context, ICommonServices commonService)
          {
               _context = context;
               _commonService = commonService;
               _unitOfWork = unitOfWork;
          }

          public async Task<CustomerReview> AddCustomerReview(int CustomerId, string CustomerName, string CustomerReviewStatus, string Remarks, ICollection<CustomerReviewItem> CustomerReviewItems)
          {
               if (await _context.CustomerReviews.Where(x => x.CustomerId == CustomerId).FirstOrDefaultAsync() !=null) return null;
               
               var rvw = new CustomerReview(CustomerId, CustomerName, CustomerReviewStatus, Remarks, CustomerReviewItems);

               _unitOfWork.Repository<CustomerReview>().Add(rvw);

               if (await _unitOfWork.Complete() > 0) return rvw;

               return null;

          }

          public async Task<bool> AddNewCustomerReviewItem(int CustomerId, DateTime ReviewDate, int UserId, int CustomerReviewDataId, string Remarks)
          {
               var rvwId = await _context.CustomerReviews.Where(x => x.CustomerId == CustomerId).Select(x => x.Id).FirstOrDefaultAsync();
               if (rvwId==0) return false;

               var rvwItem = new CustomerReviewItem(rvwId, ReviewDate, UserId, CustomerReviewDataId, Remarks);

               _unitOfWork.Repository<CustomerReviewItem>().Add(rvwItem);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> ApproveCustomerReviewTransaction(int CustomerReviewItemId, bool Approved, int ApprovedByUserId)
          {
               var item = await _context.CustomerReviewItems.FindAsync(CustomerReviewItemId);
               item.ApprovedBySup = Approved;
               _unitOfWork.Repository<CustomerReviewItem>().Update(item);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> DeleteCustomerReviewItem(CustomerReviewItem customerReviewItem)
          {
               _unitOfWork.Repository<CustomerReviewItem>().Delete(customerReviewItem);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditCustomerReview(CustomerReview model)
          {
               var existingReview = await _context.CustomerReviews.Where(x => x.Id == model.Id)
                    .Include(x => x.CustomerReviewItems)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
               
               if (existingReview == null) throw new Exception("Failed to find matching review record in database");

               _context.Entry(existingReview).CurrentValues.SetValues(model);   //saves only parent

               foreach(var existingItem in existingReview.CustomerReviewItems.ToList())
               {
                    if (!model.CustomerReviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.CustomerReviewItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               foreach(var item in model.CustomerReviewItems)
               {
                    var existingItem = existingReview.CustomerReviewItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(item);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    } else {
                         var newItem = new CustomerReviewItem(item.Id,
                              item.ReviewTransactionDate, item.UserId, item.CustomerReviewDataId,
                              item.Remarks);
                         existingReview.CustomerReviewItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }

               _context.Entry(existingReview).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;

          }

          public async Task<bool> EditCustomerReviewItem(CustomerReviewItem customerReviewItem)
          {
               if (customerReviewItem.Id == 0) return false;
               _unitOfWork.Repository<CustomerReviewItem>().Update(customerReviewItem);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<CustomerReviewData>> GetCustomerReviewStatusData()
          {
               return await _context.CustomerReviewDatas.OrderBy(x => x.CustomerReviewStatusName).ToListAsync();
          }

          public async Task<CustomerReview> GetOrAddCustomerReview(int CustomerId)
          {
               var review = await _context.CustomerReviews.Where(x => x.CustomerId == CustomerId)
                    .Include(x => x.CustomerReviewItems.OrderByDescending(x => x.ReviewTransactionDate)).FirstOrDefaultAsync();

               if (review == null) {
                    var items = new List<CustomerReviewItem>();   
                    var customerName = await _commonService.CustomerNameFromCustomerId(CustomerId);
                    review = new CustomerReview(CustomerId, customerName, "Not Reviewed", "", items);
                    _unitOfWork.Repository<CustomerReview>().Add(review);

                    if (await _unitOfWork.Complete() > 0) return review;
                    return null;
               } else {
                    if (string.IsNullOrEmpty(review.CustomerName)) {
                         var customerName = await _commonService.CustomerNameFromCustomerId(CustomerId);
                         review.CustomerName = customerName;
                    }
                    return review;
               }
          }
     }
}