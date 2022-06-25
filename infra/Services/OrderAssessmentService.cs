using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class OrderAssessmentService : IOrderAssessmentService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly ITaskService _taskService;
          private readonly IOrderItemService _orderItemService;
          public OrderAssessmentService(IUnitOfWork unitOfWork, ATSContext context, ITaskService taskService, IOrderItemService orderItemService)
          {
               _taskService = taskService;
               _context = context;
               _unitOfWork = unitOfWork;
               _orderItemService = orderItemService;
          }

          public Task<ICollection<EmailMessage>> AssignTasksToHRExecutives(ICollection<HRTaskAssignmentDto> assignmentsDto)
          {
               throw new System.NotImplementedException();
          }

          public async Task<ICollection<OrderItemAssessmentQ>> GetAssessmentQsOfOrderItemId(int orderitemid)
          {
               var item = await _context.OrderItemAssessments.Where(x => x.OrderItemId == orderitemid)
                    .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo)).FirstOrDefaultAsync();
               if (item==null) return null;
               return item.OrderItemAssessmentQs;
          }
        
          public async Task<OrderItemAssessment> CopyStddQToOrderAssessmentItem(int orderitemid)
          {
               var assessmentitem = await _context.OrderItemAssessments.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();
               if (assessmentitem != null) return assessmentitem;

               var qs = await _context.AssessmentStandardQs.OrderBy(x => x.QNo).ToListAsync();
               var lst = new List<OrderItemAssessmentQ>();
               foreach (var q in qs)
               {
                    lst.Add(new OrderItemAssessmentQ(orderitemid, q.QNo, q.AssessmentParameter, q.Question, q.MaxPoints));
               }
               var orderitem = await _context.OrderItems.Where(x => x.Id == orderitemid)
                    .Select(x => new { x.OrderId, x.CategoryName, x.CategoryId }).FirstOrDefaultAsync();
               var orderNo = await _context.Orders.Where(x => x.Id == orderitem.OrderId).Select(x => x.OrderNo).FirstOrDefaultAsync();
               assessmentitem = new OrderItemAssessment(orderitemid, orderitem.OrderId, orderNo, orderitem.CategoryId,
                    orderitem.CategoryName, lst);

               _unitOfWork.Repository<OrderItemAssessment>().Add(assessmentitem);

               if (await _unitOfWork.Complete() > 0)
               {
                    return await _context.OrderItemAssessments.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();
               }
               else
               {
                    return null;
               }
          }

          public async Task<bool> DeleteAssessmentItemQ(int orderitemid)
          {
               var assessmentItem = await _context.OrderItemAssessments.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();
               _unitOfWork.Repository<OrderItemAssessment>().Delete(assessmentItem);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditOrderAssessmentItem(OrderItemAssessment assessmentItem)
          {
               var existingItem = await _context.OrderItemAssessments
                    .Where(x => x.Id == assessmentItem.Id)
                    .Include(x => x.OrderItemAssessmentQs)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
               if (existingItem==null) return false;

               _context.Entry(existingItem).CurrentValues.SetValues(assessmentItem); //save parent object

               //check existingItem (i.e. from DB) - if it contains a record not present in model, then delete
               foreach(var exsitingQ in existingItem.OrderItemAssessmentQs.ToList())
               {
                    if (!assessmentItem.OrderItemAssessmentQs.Any(c => c.Id == exsitingQ.Id && c.Id != default(int)))
                    {
                         _context.OrderItemAssessmentQs.Remove(exsitingQ);
                         _context.Entry(exsitingQ).State = EntityState.Deleted;
                    }
               }

               //children now are eitehr added or updated
               foreach(var q in assessmentItem.OrderItemAssessmentQs)
               {
                    var existingQ = existingItem.OrderItemAssessmentQs
                         .Where(c => c.Id == q.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingQ != null)
                    {
                         _context.Entry(existingQ).CurrentValues.SetValues(q);
                         _context.Entry(existingQ).State = EntityState.Modified;
                    } else {
                         var newQ = new OrderItemAssessmentQ(
                              assessmentItem.OrderItemId, q.OrderId, q.OrderAssessmentItemId,
                              q.QuestionNo, q.Subject, q.Question, q.MaxMarks, q.IsMandatory);
                         existingItem.OrderItemAssessmentQs.Add(newQ);
                         _context.Entry(newQ).State = EntityState.Added;
                    }
               }

               _context.Entry(existingItem).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;
               
          }

          public async Task<bool> EditOrderAssessmentQs(ICollection<OrderItemAssessmentQ> assessmentQs)
          {
               foreach (var q in assessmentQs)
               {
                    _unitOfWork.Repository<OrderItemAssessmentQ>().Update(q);
               }

               return await _unitOfWork.Complete() > 0;

          }
          public async Task<bool> EditOrderAssessmentQ(OrderItemAssessmentQ assessmentQ)
          {
               _unitOfWork.Repository<OrderItemAssessmentQ>().Update(assessmentQ);

               return await _unitOfWork.Complete() > 0;
          }


          public async Task<IReadOnlyList<AssessmentQBank>> GetAssessmentQsFromBankBySubject(AssessmentStddQsParams qsParams)
          {
               var qs = await _context.AssessmentQBank
                    .Include(x => x.AssessmentQBankItems
                         .Where(x => x.AssessmentParameter.ToLower() == qsParams.Subject.ToLower())
                         .OrderBy(x => x.QNo)
                         )
                    .OrderBy(x => x.CategoryName)
                    .ToListAsync();
               return qs;
          }

          public async Task<OrderItemAssessment> GetOrAddOrderAssessmentItem(int orderItemId)
          {
               var assessmt = await _context.OrderItemAssessments.Where(x => x.OrderItemId == orderItemId)
                    .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                    .FirstOrDefaultAsync();

               if (assessmt == null) {
                    //orderid, orderno, categoryid, categoryname
                    var details = await _orderItemService.GetOrderItemRBriefDtoFromOrderItemId(orderItemId);
                    assessmt = new OrderItemAssessment(orderItemId, details.OrderId, details.OrderNo, details.CategoryId, details.CategoryName,null);
                    _unitOfWork.Repository<OrderItemAssessment>().Add(assessmt);
                    if (await _unitOfWork.Complete() == 0) return null;
               }

               return assessmt;
          }

          public async Task<bool> DeleteAssessmentQ(int assessmentQid)
          {
               var assessmentq = await _context.OrderItemAssessmentQs.FindAsync(assessmentQid);
               if (assessmentq == null) return false;
               _unitOfWork.Repository<OrderItemAssessmentQ>().Delete(assessmentq);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<OrderItemAssessment>> CreateNewOrderAssessment(int orderid)
          {
               var OrderItemIds = await _context.OrderItems.Where(x => x.OrderId == orderid)
                    .Select(x => x.Id).ToListAsync();
               var MissingItems = await _context.OrderItemAssessments.Where(
                    x => !OrderItemIds.Contains(x.OrderItemId)).ToListAsync();
               
               if(MissingItems.Count==0) throw new System.Exception("All order items have defined Assessment Questions");

               var assessments = new List<OrderItemAssessment>();

               foreach(var item in MissingItems)
               {
                    var newAssessmt =
                         new OrderItemAssessment(item.Id, item.OrderId, 
                         item.OrderNo, item.CategoryId, item.CategoryName, null);
                    assessments.Add(newAssessmt);
                    _unitOfWork.Repository<OrderItemAssessment>().Add(newAssessmt);
               }

               if (await _unitOfWork.Complete() > 0) return assessments;
               return null;
          }

          public Task<OrderItemAssessment> AddNewOrderAssessmentItem(int orderitemid)
          {
               throw new System.NotImplementedException();
          }

          public async Task<ICollection<OrderItemAssessment>> GetOrderAssessment(int orderId)
          {
               return await _context.OrderItemAssessments.Where(x => x.OrderId == orderId)
                    .Include(x => x.OrderItemAssessmentQs).ToListAsync();
          }
     }
}
