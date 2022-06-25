using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Entities.Users;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class CVRefService : ICVRefService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly ICommonServices _commonService;
          private readonly IConfiguration _config;
          private readonly IEmployeeService _empService;
          private readonly int _docControllerAdminId;
          private readonly int _TargetDays_FollowUpWithClientForSelection;
          private readonly bool _ByepassCVReviewLevels;
          private readonly IComposeMessagesForAdmin _composeMsgAdmin;
          
          private readonly IEmailService _emailService;
          private readonly ITaskService _taskService;
          public CVRefService(IUnitOfWork unitOfWork, ATSContext context, ICommonServices commonService, IEmailService emailService,
                    IConfiguration config, IComposeMessagesForAdmin composeMsgAdmin, IEmployeeService empService, ITaskService taskService)
          {
               _composeMsgAdmin = composeMsgAdmin;
               _emailService = emailService;
               _empService = empService;
               _config = config;
               _commonService = commonService;
               _context = context;
               _unitOfWork = unitOfWork;
               _taskService = taskService;
               _docControllerAdminId = Convert.ToInt32(config.GetSection("EmpDocControllerAdminId").Value);
               _ByepassCVReviewLevels = config.GetSection("ByepassCVReviewLevels").Value.ToLower() == "true" ? true : false;
               _TargetDays_FollowUpWithClientForSelection = Convert.ToInt32(config.GetSection("TargetDays_FollowUpWithClientForSelection").Value);
          }

          public async Task<bool> EditReferral(CVRef cVRef)
          {
               var refStatus = await _context.CVRefs
               .Where(x => x.Id == cVRef.Id).Select(x => x.RefStatus)
               .FirstOrDefaultAsync();

               if (refStatus != (int)EnumCVRefStatus.Referred) return false;    //ref status changed, so no edits

               _unitOfWork.Repository<CVRef>().Update(cVRef);

               return (await _unitOfWork.Complete() > 0);
          }

          public async Task<CVRef> GetReferralById(int cvrefid)
          {
               return await _context.CVRefs.FindAsync(cvrefid);
          }
          public async Task<CVRef> GetReferralByCandidateAndOrderItem(int candidateId, int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
               .FirstOrDefaultAsync();
          }

          public async Task<ICollection<CVRef>> GetReferralsOfACandidate(int candidateId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId)
               .OrderBy(x => x.ReferredOn)
               .ToListAsync();
          }

          public async Task<ICollection<CVRef>> GetReferralsOfOrderItemId(int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.OrderItemId == orderItemId)
               .OrderBy(x => x.ReferredOn).ToListAsync();
          }

          //post actions - after inserting CVRef record
          //Update owner task of the DocController Task to forward the CV
          public async Task<MessagesDto> MakeReferralsAndCreateTask(ICollection<int> CandidateAssessmentIds, LoggedInUserDto loggedInUserDto)
          {
               //todo - implement CVRefRestriction checking
               var dto = new MessagesDto();
               DateTime dateTimeNow = DateTime.Now;
               
               var cvrefs = new List<CVRef>();
               int DocControllerAdminId=4;

               //the qry object at 127 below will not return records if 
               var candAssessmentsWithoutTaskIds = await _context.CandidateAssessments.Where(x => CandidateAssessmentIds.Contains(x.Id) && x.TaskIdDocControllerAdmin == 0 ).ToListAsync();
               var dbChanged=false;

               if (candAssessmentsWithoutTaskIds != null && candAssessmentsWithoutTaskIds.Count > 0) {     //create new tasks and update CandidateAssessments if AdminTask not available
                    var itemdetails = await (from r in _context.CandidateAssessments where CandidateAssessmentIds.Contains(r.Id) && r.TaskIdDocControllerAdmin == 0
                         join i in _context.OrderItems on r.OrderItemId equals i.Id 
                         join cat in _context.Categories on i.CategoryId equals cat.Id  
                         join ordr in _context.Orders on i.OrderId equals ordr.Id 
                         join c in _context.Customers on ordr.CustomerId equals c.Id
                         join cand in _context.Candidates on r.CandidateId equals cand.Id
                         select new {candassessmentid=r.Id, orderid=ordr.Id, orderitemid=i.Id,
                              orderno=ordr.OrderNo, candidateid=r.CandidateId, taskdescription=
                              "CV approved to send to client: Application No.:" + cand.ApplicationNo + ", Candidate: " + cand.FullName +
                              "forward to: " + c.CustomerName + " against requirement " + ordr.OrderNo + "-" + i.SrNo + "-" + cat.Name +
                              ", Cleared to send by: " + r.AssessedByName + " on " + r.AssessedOn }
                    ).ToListAsync();



                    foreach(var id in candAssessmentsWithoutTaskIds) {
                         //check if the task exists, but CandidateAssessment.TaskIdDocControllerAdmin is not updated;
                         var task = await _context.Tasks.Where(x => x.OrderItemId == id.OrderItemId && 
                              x.CandidateId == id.CandidateId && x.TaskTypeId == (int)EnumTaskType.CVForwardToCustomers).FirstOrDefaultAsync();
                         if (task == null){
                              var itemdetail = itemdetails.Find(x => x.candassessmentid == id.Id);
                              task= new ApplicationTask((int)EnumTaskType.CVForwardToCustomers,
                                   dateTimeNow, loggedInUserDto.LoggedInEmployeeId, _docControllerAdminId, itemdetail.orderid, itemdetail.orderno, 
                                   itemdetail.orderitemid, itemdetail.taskdescription, dateTimeNow.AddDays(2), "Not Started", 
                                   itemdetail.candidateid, id.Id);
                              _unitOfWork.Repository<ApplicationTask>().Add(task);
                              await _unitOfWork.Complete();
                         }
                         //await _unitOfWork.Complete();
                         id.TaskIdDocControllerAdmin = task.Id;
                         _unitOfWork.Repository<CandidateAssessment>().Update(id);
                         dbChanged=true;
                    }
                    //now update taskids in candidateassessments
               
                    if (dbChanged) await _unitOfWork.Complete();      //line 129 needs to be saved to DB
               }
               
               var qryOfAssessment = await (from r in _context.CandidateAssessments where CandidateAssessmentIds.Contains(r.Id) && r.TaskIdDocControllerAdmin > 0
                    join i in _context.OrderItems on r.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    join lst in _context.ChecklistHRs on new {a=cand.Id, b=i.Id} equals new {a=lst.CandidateId,b=lst.OrderItemId} into chklst 
                    from checklist in chklst.DefaultIfEmpty()
                    orderby r.AssessedOn
                    select new {
                         OrderItemId=i.Id,
                         CategoryId = i.CategoryId,
                         OrderId = ordr.Id,
                         OrderNo = ordr.OrderNo,
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         CandidateId = cand.Id,
                         SrNo = i.SrNo,
                         Ecnr = cand.Ecnr,
                         ApplicationNo = cand.ApplicationNo,
                         CandidateName = cand.FullName,
                         HRExecId=(int)i.HrExecId,
                         CandidateAssessmentId=r.Id,
                         candassessment = r,
                         DocControllerAdminTaskId = r.TaskIdDocControllerAdmin,
                         ChargesAgreed = checklist.ChargesAgreed,
                         Charges = checklist == null || checklist.Charges == 0 ? "Undefined" 
                              : checklist.Charges == checklist.ChargesAgreed ? "Agreed" : "Disparity"
                    }).ToListAsync();
               
               if (qryOfAssessment.Count==0){
                    dto.ErrorString = "Failed to retrieve Assessment Data to forward";
                    return dto;
               }

               dbChanged=false;     
               var candidateAssessmentChanged=false;

               //save to CVRefs
               foreach(var q in qryOfAssessment)
               {
                    var cvref = await _context.CVRefs.Where(x => x.CandidateId == q.CandidateId && x.OrderItemId == q.OrderItemId).FirstOrDefaultAsync();
                    if (cvref != null) {          //logic error - candidateAssessment.Id has CvRefId == 0, but there is a record in CVRef.  
                         //this should never happen
                         var candass = await _context.CandidateAssessments.FindAsync(q.CandidateAssessmentId);
                         if (candass != null) {
                              candass.CvRefId = cvref.Id;
                              _unitOfWork.Repository<CandidateAssessment>().Update(candass);
                              candidateAssessmentChanged=true;
                         }
                    } else {
                         cvref =new CVRef(q.OrderItemId, q.CategoryId, q.OrderId, q.OrderNo,
                              q.CustomerName, q.CategoryName, q.CandidateId, q.Ecnr, q.ApplicationNo,
                              q.CandidateName, dateTimeNow, (int)q.ChargesAgreed, q.HRExecId, q.CandidateAssessmentId);
                         _unitOfWork.Repository<CVRef>().Add(cvref);
                         
                         var t = await _context.Tasks.FindAsync(q.DocControllerAdminTaskId);
                         t.TaskStatus = "Completed";
                         t.CompletedOn=dateTimeNow;
                         var candidatedescription = "Application " + q.ApplicationNo + " - " + q.CandidateName + " referred to " +
                              q.CustomerName + " for " + q.OrderNo + "-" + q.SrNo + "-" + q.CategoryName + " on " + dateTimeNow;
                         
                         var taskitem = new TaskItem((int)EnumTaskType.SubmitCVToDocControllerAdmin, t.Id, dateTimeNow, "Completed",
                              "Completed task: " + candidatedescription, cvref.OrderId, cvref.OrderItemId,
                              q.OrderNo, loggedInUserDto.LoggedInEmployeeId, dateTimeNow.AddDays(2), q.CandidateId,
                              0, DocControllerAdminId);
                         _unitOfWork.Repository<TaskItem>().Add(taskitem);
                         _unitOfWork.Repository<ApplicationTask>().Update(t);

                         //create task for DocController to follow up with client for selection
                         var taskForSelection = new ApplicationTask((int)EnumTaskType.SelectionFollowupByDocControllerAdmin,
                              dateTimeNow, DocControllerAdminId, DocControllerAdminId, q.OrderId,
                              q.OrderNo, q.OrderItemId, "Follow up for selection of " + candidatedescription +
                              "referred on " + dateTimeNow.Date, dateTimeNow.AddDays(_TargetDays_FollowUpWithClientForSelection),
                              "Open", q.CandidateId, q.CandidateAssessmentId);
                         _unitOfWork.Repository<ApplicationTask>().Add(taskForSelection);
                         dbChanged=true;
                         cvrefs.Add(cvref);
                    }
               }
          
               //todo - do we want to advise the candidate about his referral?
               if (candidateAssessmentChanged || dbChanged) {
                    if (await _unitOfWork.Complete()==0) {
                         if (dbChanged) {
                              dto.ErrorString = "CVs registered as referred to the client, but failed to create tasks in the name of document controller to forward the CV";
                              return dto;
                         }
                    }
               }

               if (!dbChanged) {
                    dto.ErrorString = "No valid data available to register the CV Referrals";
                    return dto;
               }
               
               //var cassessments = await _context.CandidateAssessments.Where(x => CandidateAssessmentIds.Contains(x.Id)).ToListAsync();

               foreach (var c in cvrefs)       //update cvreview.CVRefId             
               {
                    var q = qryOfAssessment.Find(x => x.CandidateId == c.CandidateId && x.OrderItemId == c.OrderItemId);
                    if (q != null) {
                         var candass = q.candassessment;
                         candass.CvRefId = c.Id;
                         _unitOfWork.Repository<CandidateAssessment>().Update(candass);
                    }
               }

               //update Candidate.CandidateStatus="Referred";
               var candidates = await _context.Candidates.Where(x => cvrefs.Select(x => x.CandidateId).ToList().Contains(x.Id)).ToListAsync();
               foreach(var c in candidates)
               {
                    c.CandidateStatus="Referred";
                    _unitOfWork.Repository<Candidate>().Update(c);
               }

               //var cvrefids = cvrefs.Select(x => x.Id).ToList();

               var emailmsgs = await _composeMsgAdmin.ComposeCVFwdMessagesToClient(cvrefs.Select(x => x.Id).ToList(), loggedInUserDto);   //returns object, without saving to DB
               if(emailmsgs==null || emailmsgs.Count ==0) {
                    dto.ErrorString = "CVs referred to client, but failed to compose cv forwarding messages";
                    return dto;
               }
               foreach(var msg in emailmsgs) {
                    _unitOfWork.Repository<EmailMessage>().Add(msg);
               }

               if (await _unitOfWork.Complete() == 0) {
                    dto.ErrorString = "Tasks for Document controller created, but messages were not composed";
                    return dto;
               }
               
               /*
               var filePaths = new List<string>();
               foreach (var msg in emailmsgs)
               {
                    if ((msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmail || 
                              msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmailComposeAndSendSMS ))
                    _emailService.SendEmail(msg, filePaths);
               }
               */

               //update Candidate.candidateStatus
               
               
               
               dto.emailMessages = emailmsgs;
               return dto;
               
               //todo - update the CVRef object with actual date cv sent - call back from email sent                    
          }

          public async Task<bool> DeleteReferral(CVRef cvref)
          {
               if (await _context.Deploys.Where(x => x.CVRefId == cvref.Id).ToListAsync() != null)
               {
                    throw new System.Exception("The referral has related records in Deployments");
               }
               _unitOfWork.Repository<CVRef>().Delete(cvref);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<CVRefPendingDto>> GetCVsPendingReferralsToCustomer()
          {
               var qry = await (from rvw in _context.CVReviews
                    where rvw.CVRefId == 0 && rvw.DocControllerAdminTaskId > 0
                    join i in _context.OrderItems on rvw.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Candidates on rvw.CandidateId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    select new CVRefPendingDto
                    {
                         CandidateId = rvw.CandidateId,
                         CandidateDetails = "App No " + c.ApplicationNo + ", " + c.FullName + " PP No." + c.PpNo,
                         CategoryRef = o.OrderNo + "-" + i.SrNo,
                         ToBeReferredToCustomer = cust.CustomerName,
                         DateCVApprovedToForward =
                              i.NoReviewBySupervisor ? rvw.SubmittedByHRExecOn.Date
                              : rvw.HRMId == 0
                                   ? Convert.ToDateTime(rvw.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(rvw.HRMReviewedOn).Date,
                         CVApprovedByUsername = "",
                         NoReviewBySupervisor = i.NoReviewBySupervisor,
                         HRExecutiveId = (int)i.HrExecId,
                         HRSupId = (int)i.HrSupId,
                         HRMId = (int)i.HrmId
                    })
               .ToListAsync();

               foreach (var item in qry)
               {
                    item.CVApprovedByUsername = await _empService.GetEmployeeNameFromEmployeeId(
                         item.NoReviewBySupervisor ? item.HRExecutiveId : item.HRMId == 0 ? item.HRSupId : item.HRMId);
               }

               return qry;

          }

          public async Task<ICollection<CustomerReferralsPendingDto>> CustomerReferralsPending(int userId)
          {
               if (userId == 0)
               {
                    return await (from r in _context.CVReviews
                         where r.CVRefId == 0 && r.DocControllerAdminTaskId > 0
                         join i in _context.OrderItems on r.OrderItemId equals i.Id
                         join o in _context.Orders on i.OrderId equals o.Id
                         join cat in _context.Categories on i.CategoryId equals cat.Id
                         join cand in _context.Candidates on r.CandidateId equals cand.Id
                         join c in _context.Customers on o.CustomerId equals c.Id
                         select new CustomerReferralsPendingDto
                         {
                              CVReviewId = r.Id,
                              CandidateId = r.CandidateId,
                              ApplicationNo = cand.ApplicationNo,
                              CandidateName = cand.FullName,
                              OrderItemId = r.OrderItemId,
                              CategoryRef = o.OrderNo + "-" + i.SrNo,
                              CustomerName = c.CustomerName,
                              DocControllerAdminTaskId = (int)r.DocControllerAdminTaskId,
                              SentToDocControllerOn = r.NoReviewBySupervisor ? r.SubmittedByHRExecOn.Date :
                                   r.HRMId == 0 ? Convert.ToDateTime(r.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(r.HRMReviewedOn).Date
                         }).ToListAsync();
               }
               else
               {
                    return await (from r in _context.CVReviews
                         where r.CVRefId == 0 && r.DocControllerAdminTaskId > 0 && r.DocControllerAdminEmployeeId == userId
                         join i in _context.OrderItems on r.OrderItemId equals i.Id
                         join o in _context.Orders on i.OrderId equals o.Id
                         join cat in _context.Categories on i.CategoryId equals cat.Id
                         join cand in _context.Candidates on r.CandidateId equals cand.Id
                         join c in _context.Customers on o.CustomerId equals c.Id
                         select new CustomerReferralsPendingDto
                         {
                              CVReviewId = r.Id,
                              CandidateId = r.CandidateId,
                              ApplicationNo = cand.ApplicationNo,
                              CandidateName = cand.FullName,
                              OrderItemId = r.OrderItemId,
                              CategoryRef = o.OrderNo + "-" + i.SrNo,
                              CustomerName = c.CustomerName,
                              DocControllerAdminTaskId = (int)r.DocControllerAdminTaskId,
                              SentToDocControllerOn = r.NoReviewBySupervisor ? r.SubmittedByHRExecOn.Date :
                                   r.HRMId == 0 ? Convert.ToDateTime(r.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(r.HRMReviewedOn).Date
                         }).ToListAsync();
               }
          }
     }
}