using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
    public class TaskControlledServices: ITaskControlledService
    {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IValidateTaskService _validateTaskService;
          private readonly IComposeMessagesForInternalReviewHR _composeMsgHR;
          private readonly IEmailService _emailService;
          
          public TaskControlledServices(ATSContext context, IUnitOfWork unitOfWork, ICommonServices commonServices, IValidateTaskService validateTaskService, IComposeMessagesForInternalReviewHR composeMsgHR, IEmailService emailService)
          {
               _emailService = emailService;
               _composeMsgHR = composeMsgHR;
               _validateTaskService = validateTaskService;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
               _context = context;
          }

          public async Task<Pagination<ApplicationTask>> GetApplicationTasksPaginated(TaskParams taskParams)
        {

          var qry =  _context.Tasks.AsQueryable();
               
          if (taskParams.OrderId > 0) qry = qry.Where(x => x.OrderId == taskParams.OrderId);
          if(taskParams.OrderItemId.HasValue) qry = qry.Where(x => x.OrderItemId == taskParams.OrderItemId);
          if(taskParams.CandidateId.HasValue && taskParams.PersonType?.ToLower() == "candidate") 
               if(taskParams.PersonType == "candidate") qry = qry.Where(x => x.CandidateId == taskParams.CandidateId);
          if(taskParams.TaskOwnerId.HasValue) qry = qry.Where(x => x.TaskOwnerId == taskParams.TaskOwnerId);
          if(taskParams.AssignedToId.HasValue) qry = qry.Where(x => x.AssignedToId == taskParams.AssignedToId);
          
          var totalCount = await qry.CountAsync();
          var obj = await qry.Include(x => x.TaskItems).Skip((taskParams.PageIndex - 1)*taskParams.PageSize).Take(taskParams.PageSize).ToListAsync();
         
          return new Pagination<ApplicationTask>(taskParams.PageIndex, taskParams.PageSize, totalCount, obj);
        }

     public async Task<ICollection<ApplicationTask>> GetApplicationTasksWOPagination(TaskParams taskParams)
     {
          var qry =  _context.Tasks.AsQueryable();
               
          if (taskParams.OrderId > 0) qry = qry.Where(x => x.OrderId == taskParams.OrderId);
          if(taskParams.OrderItemId.HasValue) qry = qry.Where(x => x.OrderItemId == taskParams.OrderItemId);
          if(taskParams.CandidateId.HasValue) {
               if(taskParams.PersonType?.ToLower()=="candidate") {
                    qry = qry.Where(x => x.CandidateId == taskParams.CandidateId && x.PersonType.ToLower()=="candidate");
               } else if(taskParams.PersonType?.ToLower()=="prospective") {
                    qry = qry.Where(x => x.CandidateId == taskParams.CandidateId && x.PersonType.ToLower()=="prospective");
               } else if(taskParams.PersonType?.ToLower()=="associate") {
                    qry = qry.Where(x => x.CandidateId == taskParams.CandidateId && x.PersonType.ToLower()=="associate");
               }
          }
          if(taskParams.TaskOwnerId.HasValue) qry = qry.Where(x => x.TaskOwnerId == taskParams.TaskOwnerId);
          if(taskParams.AssignedToId.HasValue) qry = qry.Where(x => x.AssignedToId == taskParams.AssignedToId);
          
          if (taskParams.IncludeItems) {
               return await qry.Include(x => x.TaskItems).ToListAsync();   
          } else {
               return await qry.ToListAsync();
          }
     }

     public async Task<Pagination<ApplicationTask>> GetApplicationPendingTasksPaginated(string taskStatus, int pageIndex, int pageSize)
     {
          var specs = new TaskSpecs(taskStatus);
          var countSpec = new TaskForCountSpecs(taskStatus);
          var tasks = await _unitOfWork.Repository<ApplicationTask>().ListAsync(specs);
          var totalCount = await _unitOfWork.Repository<ApplicationTask>().CountAsync(countSpec);

          return new Pagination<ApplicationTask>(pageIndex, pageSize, totalCount, tasks);
     }

     public async Task<ICollection<EmailMessage>> CreateTaskForHRExecOnOrderItemIds(ICollection<OrderAssignmentDto> assignments, int loggedInEmployeeId)
     {
          var tasks = new List<ApplicationTask>();
          var task = new ApplicationTask();

          foreach(var t in assignments)
          {
               if (string.IsNullOrEmpty(t.CustomerName)) t.CustomerName=await _commonServices.CustomerNameFromCustomerId(t.CustomerId);
               if (string.IsNullOrEmpty(t.CategoryName)) t.CategoryName=await _commonServices.CategoryNameFromCategoryId(t.CategoryId);
               if (string.IsNullOrEmpty(t.ProjectManagerPosition)) t.ProjectManagerPosition=await _commonServices.GetEmployeePositionFromEmployeeId(t.CustomerId);
               var taskitems = new List<TaskItem>();
               
               taskitems.Add(new TaskItem((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now, "not started", "task initiated", 
                    t.OrderId, t.OrderItemId, t.OrderNo, loggedInEmployeeId, DateTime.Now.AddDays(7), t.HrExecId,t.Quantity));

               task = new ApplicationTask((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now, t.ProjectManagerId, t.HrExecId, (int)t.OrderId, (int)t.OrderNo,
                    (int)t.OrderItemId, "Task assigned for " + t.Quantity + " CVs of " + t.CategoryName + ", Category Reference " + t.CategoryRef + " for " + t.CustomerName, 
                    t.CompleteBy, "Not Started", 0, taskitems);
               string ErrString = await _validateTaskService.ValidateTaskObject(task);
               if (string.IsNullOrEmpty(ErrString)) tasks.Add(task);

          }
          
          if(tasks==null || tasks.Count==0) return null;

          foreach(var tsk in tasks) {
               _unitOfWork.Repository<ApplicationTask>().Add(tsk);
          }
          
          var recordsAffected = await _unitOfWork.Complete();

          if (recordsAffected == 0) return null;  // throw new Exception("Failed to create the task");

          var emailMsgs = (List<EmailMessage>) await _composeMsgHR .ComposeMessagesToHRExecToSourceCVs(assignments);
          if (emailMsgs != null && emailMsgs.Count ==0) return null;

          foreach(var msg in emailMsgs)
          {
               if ((msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmail || 
                    msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmailComposeAndSendSMS ))
               {
                    var attachments = new List<string>();
                    _emailService.SendEmail(msg, attachments);
               }
          }
          /*   //TODO - test for recipient particulars before allowing direct send
          if (msg != null) {
               var attachments = new List<string>();        // TODO - should this be auto-sent?
               msg = await _emailService.SendEmail(msg, attachments);
          }
          */
          return emailMsgs;

     }
     
     public async Task<bool> CreateTaskForHRExecOnOrderItemId_s(Order modelOrder, int loggedInEmployeeId)
     {
          //var orderassignmentdto = await GetAssignmentDtoFromItemIds(OrderItemIds);
          var ids = modelOrder.OrderItems.Select(x => x.Id).ToList();
          var existingOrderItems = await _context.OrderItems.Where(x => ids.Contains(x.Id)).ToListAsync();
          var tasks = new List<ApplicationTask>();
          int added=0;
          var orderitemid = modelOrder.OrderItems.Select(x => x.Id).FirstOrDefault();
          var CustomerName = await _context.Customers.Where(x => x.Id == modelOrder.CustomerId).Select(x => x.CustomerName).FirstOrDefaultAsync();

          var cats = await _context.Categories.Where(x => modelOrder.OrderItems.Select(x => x.CategoryId).ToList().Contains(x.Id)).Select(x => new {x.Id, x.Name}).ToListAsync();

          foreach(var model in modelOrder.OrderItems)
          {
               var existingOrderItem = existingOrderItems.Where(x => x.Id == model.Id).FirstOrDefault();
               if (existingOrderItem.HrExecId != model.HrExecId) {
                    existingOrderItem.HrExecId = model.HrExecId;
                    _unitOfWork.Repository<OrderItem>().Update(existingOrderItem);

                    var task = new ApplicationTask((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now,
                         loggedInEmployeeId, (int)model.HrExecId, model.OrderId, model.OrderNo,
                         model.Id, "Category Ref " + modelOrder.OrderNo + "-" + model.SrNo + " " + 
                         cats.Where(x => x.Id == model.CategoryId).Select(x => x.Name).FirstOrDefault() +
                         " total " + model.Quantity + " for " + CustomerName + " assigned to you", 
                         model.CompleteBefore.Date, "Open", 0, null);
                    //task.PostTaskAction = EnumPostTaskAction.ComposeAndSendEmail;
               
                    _unitOfWork.Repository<ApplicationTask>().Add(task);
                    added++;
               }
          }

          if(added == 0) return false;   // throw new Exception("no valid records found to generate order assignments");
          var recordsAffected = await _unitOfWork.Complete();

          if (recordsAffected == 0) return false;  // throw new Exception("Failed to create the task");

          var dtos = new List<OrderAssignmentDto>();
          foreach(var model in modelOrder.OrderItems)
          {
               dtos.Add(new OrderAssignmentDto{
                    OrderId = model.OrderId, OrderNo = modelOrder.OrderNo, OrderDate = modelOrder.OrderDate, 
                    CityOfWorking = modelOrder.CityOfWorking, ProjectManagerId = modelOrder.ProjectManagerId, 
                    ProjectManagerPosition="not retrieved from DB", OrderItemId = model.Id,
                    HrExecId = (int)model.HrExecId, CategoryRef=modelOrder.OrderNo + "-" + model.SrNo,
                    CategoryName = cats.Where(x => x.Id == model.CategoryId).Select(x => x.Name).FirstOrDefault(),
                    CustomerId = modelOrder.CustomerId, CustomerName  = CustomerName, CompleteBy = model.CompleteBefore});
          }
          

          //var orderassignmentdto = _mapper.Map<ICollection<OrderItem>, ICollection<OrderAssignmentDto>>(orderitems);
          var msgs = (List<EmailMessage>) await _composeMsgHR.ComposeMessagesToHRExecToSourceCVs(dtos);
          
          //for this function, the post action is: composeandsendemail
          if(msgs!=null && msgs.Count > 0)
          {
               var attachments = new List<string>();
               foreach(var msg in msgs)
               {
                    _emailService.SendEmail(msg, attachments);
               }
          }
          
          /*   ** TODO ** - test for recipient particulars before allowing direct send
          if (msg != null) {
               var attachments = new List<string>();        // TODO - should this be auto-sent?
               msg = await _emailService.SendEmail(msg, attachments);
          }
          */               

          return recordsAffected > 0 ;
     }



    }
}