using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class CVRvwService : ICVReviewService
     {
          private readonly ITaskService _taskService;
          //private readonly IConfiguration _config;
          private readonly int mTargetDays_HRSupToReviewCV;
          private readonly int mTargetDays_ForwardCVToClient;
          private readonly ICommonServices _commonServices;
          private readonly ATSContext _context;
          private readonly bool gBoolChecklistHRMandatory;
          private readonly int gDocControllerId;
          private readonly IVerifyService _verifyService;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IComposeMessagesForHR _composeMessages;
          private readonly IEmailService _emailService;
          private readonly IComposeMessagesForInternalReviewHR _composeMsgForInternalReviewHR;
          public CVRvwService(ITaskService taskService, IConfiguration config, 
               IComposeMessagesForHR composeMessages, IComposeMessagesForInternalReviewHR composeMsgForInternalReviewHR,
               IUnitOfWork unitOfWork, IEmailService emailService,
               ICommonServices commonServices, ATSContext context, IVerifyService verifyService)
          {
               _emailService = emailService;
               _composeMessages = composeMessages;
               _composeMsgForInternalReviewHR = composeMsgForInternalReviewHR;
               _unitOfWork = unitOfWork;
               _verifyService = verifyService;
               _context = context;
               _commonServices = commonServices;
               _taskService = taskService;
               mTargetDays_HRSupToReviewCV = Convert.ToInt32(config.GetSection("TargetDays_HRSupToReviewCV").Value);
               mTargetDays_ForwardCVToClient = Convert.ToInt32(config.GetSection("TargetDays_DocControllerToFwdCV").Value);
               gBoolChecklistHRMandatory = Convert.ToBoolean(config.GetSection("ChecklistHRMandatory").Value);
               gDocControllerId = Convert.ToInt32(config.GetSection("EmpDocControllerAdminId").Value);
          }

          public async Task<ICollection<EmailMessage>> AddNewCVReview(ICollection<CVReviewSubmitByHRExecDto> cvsSubmitted,LoggedInUserDto loggedInUserDto)
          {
               //checklisthrID, HRExecTaskId, HRMID not poulated in ContractREview
               //validate candidateIds and orderitemIds

               var filtered = new List<CVReviewSubmitByHRExecDto>();
               var commonData = new CommonDataDto();
               DateTime dateTimeNow = DateTime.Now;

               var rvws = new List<CVRvw>();
               var nextTasks = new List<ApplicationTask>();
               foreach(var item in cvsSubmitted)
               {
                    if (await GetCVReviewId(item.CandidateId, item.OrderItemId) > 0) continue;
                    commonData = await _commonServices
                         .CommonDataFromOrderItemCandidateIdWithChecklistId(item.OrderItemId, item.CandidateId);
                    
                    if(commonData == null) continue;
                    //item.AssignedToId = commonData.NoReviewBySupervisor ? gDocControllerId: commonData.HRSupId;
                    item.AssignedToId = commonData.RequireInternalReview 
                         ? commonData.HRSupId==0 ? commonData.HRMId == 0 ? gDocControllerId : commonData.HRMId : commonData.HRSupId 
                         : commonData.HRMId;
                    if(item.AssignedToId == 0) continue;

                    item.CommonDataDto=commonData;

                    var rvw = new CVRvw(commonData.ChecklistHRId, item.CandidateId, commonData.Ecnr, item.OrderItemId,
                         commonData.OrderId, commonData.HRExecId, commonData.HRExecTaskId, commonData.HRSupId, 
                         commonData.HRMId, dateTimeNow, item.ExecRemarks);               
                    _unitOfWork.Repository<CVRvw>().Add(rvw);

                    rvws.Add(rvw);
                    filtered.Add(item);

                    //create task for next assignee
                    var nextTask = new ApplicationTask(commonData.NoReviewBySupervisor 
                         ? (int)EnumTaskType.SubmitCVToDocControllerAdmin
                         : (int)EnumTaskType.SubmitCVToHRSupForReview, 
                         dateTimeNow, commonData.HRExecId, commonData.HRSupId, commonData.OrderId, 
                         commonData.OrderNo, item.OrderItemId, 
                         commonData.NoReviewBySupervisor 
                              ? "cv forwarded by HR Exec to forward to client" + commonData.CandidateDesc
                              : "CV awaiting your review: " + commonData.CandidateDesc,
                         dateTimeNow.AddDays(commonData.NoReviewBySupervisor 
                              ? mTargetDays_ForwardCVToClient
                              : mTargetDays_HRSupToReviewCV), 
                         "Open", item.CandidateId, rvw.Id);
                    //rvw.Id will be updated after rvws are committed to DB
                    _unitOfWork.Repository<ApplicationTask>().Add(nextTask);
                    nextTasks.Add(nextTask);
               }
               
               if (rvws == null || rvws.Count == 0) throw new Exception("No relevant data returned to create CV Review object - possible reasons:" +
                    "the ChecklistHR flag is set to true and checklist has not been executed on the candidate for the order category, " +
                    "or the HR Executive assigned for the category is nto the same as the Loggedin user");

               if (await _unitOfWork.Complete() == 0) return null;

               //update above nextTasks with ReviewId, and nextTask with SupTaskId or DocControllerTAskId
               foreach(var rvw in rvws)
               {
                    var taskUpdate = nextTasks.Where(x => x.OrderItemId==rvw.OrderItemId && 
                         x.CandidateId == rvw.CandidateId && x.TaskDate == dateTimeNow).FirstOrDefault();
                    taskUpdate.CVReviewId=rvw.Id;
                    _unitOfWork.Repository<ApplicationTask>().Update(taskUpdate);
                    if(rvw.NoReviewBySupervisor) {
                         rvw.DocControllerAdminTaskId=taskUpdate.Id;
                         rvw.DocControllerAdminEmployeeId = gDocControllerId;
                    } else {
                         rvw.SupTaskId=taskUpdate.Id;
                    }
                    _unitOfWork.Repository<CVRvw>().Update(rvw);
               }

               
               //advisory to task owners
               var lstMsgs = new List<EmailMessage>();
               var msg = new EmailMessage();
               var assignees = filtered.Select(x => x.AssignedToId).Distinct().ToList();
               foreach (var assignee in assignees)
               {
                    var cvs = filtered.Where(x => x.AssignedToId == assignee).ToList();
                    var cands = cvs.Select(x => x.CommonDataDto).ToList();
                    if(assignee == gDocControllerId) {
                         msg = await _composeMessages.ComposeHTMLToPublish_CVReadiedToForwardToClient(cands, loggedInUserDto, assignee);
                    } else {
                         msg = await _composeMsgForInternalReviewHR.ComposeHTMLToPublish_CVSubmittedToHRSup(cands, loggedInUserDto, assignee);
                    }
                    
                    if (msg!= null)
                    {
                         var attachments = new List<string>();        // TODO - should this be auto-sent?
                         msg = _emailService.SendEmail(msg, attachments);
                    }
                    lstMsgs.Add(msg);
               }
               return lstMsgs;
               
          }

          //CVReviewBySupervisor
          //Called after AddNewCVReview
          //The HR Sup either approves the CV or rejects it
          //if approved, the flow goes to either:
          //   HR Manager (HRMId) if the OrderItem defines it.
          //or,
          //   DocController if no HRM Id is defined.
          
          //if the CV is approved, HR Exec Task (EnumTaskType.AssignTaskToHRExec)
          //appended with the CV details, with count = 1
          //if cv is rejected, the HR Exec task is appended with cv details with count=0
          
          //Update CVReviews table as follows:
          //   SupervisorReviewedOn, SupervisorREvieweId,
          //   HRMId if it si defined by OrderItem
          //   
          public async Task<ICollection<EmailMessage>> CVReviewByHRSup(LoggedInUserDto loggedInDto, ICollection<CVReviewBySupDto> cvsSubmittedDto)
          {
               //if Supervisor not reviewed, then disallow\

               DateTime dateTimeNow = DateTime.Now;
               //update cvsSubmittedDto for use by TaskService
               int enumtasktype = 0;
               int assignedtoid = 0;
               string taskdesc="";
               DateTime completeby;
               var cvReviewIds = new List<int>();      //contains validated CVReview.Id

               foreach (var item in cvsSubmittedDto)
               {
                    var cvrvw = await _context.CVReviews.FindAsync(item.CVReviewId);
                    if (cvrvw==null) continue;
                    if (item.ReviewResultId == (int)EnumSelStatus.NotReviewed) 
                         throw new Exception("CV Id " + item.CandidateId + " not reviewed");
                    
                    //check if already reviewed by Sup.
                    var rvwdBySup = await _context.CVReviews.Where(x => x.Id == item.CVReviewId
                         && x.SupReviewResultId != EnumSelStatus.NotReviewed)
                         .Select(x => x.ReviewedBySupOn).FirstOrDefaultAsync();
                    if (rvwdBySup != null) continue;        //throw new Exception("Already reviewed by Supervisor on " + 
                         //Convert.ToDateTime(rvwdBySup).Date);

                    //retrieve CVREview for updates
                    
                    var commonData = await _commonServices.CommonDataFromOrderDetailIdAndCandidateId(item.CVReviewId);
                    if (commonData == null) continue;
                    commonData.ReviewResultId = (EnumSelStatus)item.ReviewResultId;
                    item.CommonDataDto = commonData;
                    cvReviewIds.Add(item.CVReviewId);
                    if (item.ReviewResultId == (int)EnumSelStatus.Selected)
                    {
                         //Create next task - 
                         //-if no HRMId defined, then next task is for doc controller to send the CV to the client
                         //-if HRMId defined, then next task is for HRManager
                         
                         item.CommonDataDto = commonData;
                         if(commonData.HRMId == 0) {
                              enumtasktype = (int)EnumTaskType.SubmitCVToDocControllerAdmin;
                              assignedtoid = gDocControllerId;
                              taskdesc = "Flg CV is ready to forward to client " + commonData.CandidateDesc;
                              completeby=dateTimeNow.AddDays(mTargetDays_ForwardCVToClient);
                         } else {
                              enumtasktype = (int)EnumTaskType.SubmitCVToHRMMgrForReview;
                              assignedtoid = commonData.HRMId;
                              taskdesc = "Flg CV awaiting your review: " + commonData.CandidateDesc;
                              completeby = dateTimeNow.AddDays(mTargetDays_HRSupToReviewCV);
                         }
                         item.AssignedToId = assignedtoid;
                         
                         var nextTask = new ApplicationTask(enumtasktype,  dateTimeNow, commonData.HRSupId, assignedtoid,
                              commonData.OrderId, commonData.OrderNo, commonData.OrderItemId, taskdesc, completeby, "Open", 
                              commonData.CandidateId, item.CVReviewId);

                         _unitOfWork.Repository<ApplicationTask>().Add(nextTask);     

                         //TODO - set HRExecTask as completed
                         if (cvrvw.HRExecTaskId !=0) {
                              var hrexecTask = await _context.Tasks.FindAsync(cvrvw.HRExecTaskId);
                              hrexecTask.TaskStatus = "Completed";
                              hrexecTask.CompletedOn = dateTimeNow;
                              _unitOfWork.Repository<ApplicationTask>().Update(hrexecTask);

                              var hrExecTaskItem = new TaskItem((int)EnumTaskType.SubmitCVToHRSupForReview,hrexecTask.Id, dateTimeNow, "Completed",
                                   "CV reviewed by Sup", item.CommonDataDto.OrderId, item.CommonDataDto.OrderItemId,
                                   item.CommonDataDto.OrderNo, loggedInDto.LoggedInEmployeeId, dateTimeNow, item.CommonDataDto.CandidateId, item.CommonDataDto.HRSupId,
                                   1  //,hrexecTask
                                   );
                              _unitOfWork.Repository<TaskItem>().Add(hrExecTaskItem);
                         }

                         //UPDATE CVRvw
                         cvrvw.SupTaskId = nextTask.Id;          //this will be 0, bcz not yet committed to db 
                         cvrvw.ReviewedBySupOn=dateTimeNow;
                         cvrvw.SupRemarks=item.SupRemarks;
                         cvrvw.SupReviewResultId=EnumSelStatus.Selected;
                         cvrvw.HRMId = item.CommonDataDto.HRMId;
                    }  else {      //rejected
                         cvrvw.SupRemarks=item.SupRemarks;
                         cvrvw.SupReviewResultId=(EnumSelStatus)item.ReviewResultId;
                         cvrvw.ReviewedBySupOn = dateTimeNow;
                         cvrvw.SupRemarks = item.SupRemarks;
                    }

                    _unitOfWork.Repository<CVRvw>().Update(cvrvw);
               }

               await _unitOfWork.Complete();      //saves the supervisor/DocController tasks

               var lstMsgs = new List<EmailMessage>();
               var suptaskId=0;         //prepare to update CVRvw

               //update SupTaskId fields of CVReviews
               var rvws = await _context.CVReviews.Where(x => cvReviewIds.Contains(x.Id)).ToListAsync();
               foreach(var rvw in rvws)
               {
                    var tasktypeid = rvw.HRMId==0?(int)EnumTaskType.SubmitCVToDocControllerAdmin:(int)EnumTaskType.SubmitCVToHRMMgrForReview;
                    if (rvw.HRMId == 0) {
                         suptaskId = await _context.Tasks.Where(x => 
                              x.CVReviewId==rvw.Id && x.TaskDate==dateTimeNow && x.TaskTypeId == tasktypeid)
                              .Select(x => x.Id).FirstOrDefaultAsync();
                         rvw.DocControllerAdminTaskId=suptaskId;
                         rvw.DocControllerAdminEmployeeId=gDocControllerId;
                    } else {
                         suptaskId = await _context.Tasks.Where(x => 
                              x.CVReviewId==rvw.Id && x.TaskDate==dateTimeNow && x.TaskTypeId == tasktypeid)
                              .Select(x => x.Id).FirstOrDefaultAsync();
                         rvw.HRMTaskId=suptaskId;                         
                    }
                    _unitOfWork.Repository<CVRvw>().Update(rvw);
               }
               await _unitOfWork.Complete();
               
               //advisory to task owners
               var msg = new EmailMessage();
               var assignees = cvsSubmittedDto.Where(x => cvReviewIds.Contains(x.CVReviewId) 
                    && x.ReviewResultId == (int)EnumSelStatus.Selected).Select(x => x.AssignedToId).Distinct().ToList();
               foreach (var assignee in assignees)
               {
                    var cvs = cvsSubmittedDto.Where(x => x.AssignedToId == assignee).ToList();
                    var cands = cvs.Select(x => x.CommonDataDto).ToList();
                    foreach (var cv in cvs)
                    {
                         if(assignee == gDocControllerId && cv.CommonDataDto.HRMId == 0) {
                              msg = await _composeMessages.ComposeHTMLToPublish_CVReadiedToForwardToClient(cands, loggedInDto, cv.AssignedToId);
                         } else {
                              msg = await _composeMsgForInternalReviewHR.ComposeHTMLToPublish_CVReviewedByHRSup(cands, loggedInDto, cv.AssignedToId);
                         }
                         
                         if (msg!= null)
                         {
                              var attachments = new List<string>();        // TODO - should this be auto-sent?
                              msg = _emailService.SendEmail(msg, attachments);
                         }
                         lstMsgs.Add(msg);
                    }
               }
               return lstMsgs;
          }

          public async Task<ICollection<EmailMessage>> CVReviewByHRM(LoggedInUserDto loggedInDto, ICollection<CVReviewByHRMDto> cvsSubmittedDto)
          {
               DateTime dateTimeNow = DateTime.Now;
               //create tasks for the next stage - supervisor or doc controller task
               foreach (var item in cvsSubmittedDto)
               {
                     if (item.HRMReviewResultId == EnumSelStatus.NotReviewed) 
                         throw new Exception("CV Id " + item.CandidateId + " not reviewed by HR Manager");
                    
                    //check if already reviewed by hr manager.
                    var rvwdByHRM = await _context.CVReviews.Where(x => x.Id == item.CVReviewId
                         && x.HRMReviewResultId != EnumSelStatus.NotReviewed)
                         .Select(x => new{x.HRMReviewResultId, x.HRMReviewedOn}).FirstOrDefaultAsync();
                    if (rvwdByHRM != null) throw new Exception("Already reviewed by HR Manager on " +
                         Convert.ToDateTime(rvwdByHRM.HRMReviewedOn).Date);

                    //retrieve CVREview for updates
                    var cvrvw = await _context.CVReviews.FindAsync(item.CVReviewId);
                    var commonData = await _commonServices.CommonDataFromOrderDetailIdAndCandidateId(item.CVReviewId);
                    if (commonData == null) continue;
                    item.CommonDataDto = commonData;
                    item.CommonDataDto.ReviewResultId = item.HRMReviewResultId;

                    if (item.HRMReviewResultId == EnumSelStatus.Selected)
                    {
                         var nextTask = new ApplicationTask();        //task for doc controlelr

                         item.enumTaskType = EnumTaskType.SubmitCVToDocControllerAdmin;
                         item.AssignedToId = gDocControllerId;
                         
                         nextTask = new ApplicationTask((int)EnumTaskType.SubmitCVToDocControllerAdmin,
                              dateTimeNow, commonData.HRMId, gDocControllerId, commonData.OrderId, 
                              commonData.OrderNo, item.CommonDataDto.OrderItemId, 
                              "Pl forward flg CV to respective client: " + commonData.CandidateDesc, 
                              dateTimeNow.AddDays(mTargetDays_ForwardCVToClient), "Open", 
                              item.CommonDataDto.CandidateId, item.CVReviewId);
                         
                         _unitOfWork.Repository<ApplicationTask>().Add(nextTask);    

                         //add task item to HRSup, i.e. parentTask
                         if (cvrvw.SupTaskId > 0) {
                              var hrExecTaskItem = new TaskItem((int)EnumTaskType.SubmitCVToDocControllerAdmin, 
                                   (int)cvrvw.SupTaskId,  dateTimeNow, "Open", 
                                   "CV Forwarded to Doc Controller: " + item.CommonDataDto.CandidateDesc, 
                                   item.CommonDataDto.OrderId, item.OrderItemId, 
                                   item.CommonDataDto.OrderNo, loggedInDto.LoggedInEmployeeId,
                                   dateTimeNow.AddDays(2), item.CandidateId,  item.AssignedToId, 0//, item.ParentTask
                              );
                              //TODO - remove fields such as nextassigned to, and checkbydate
                              _unitOfWork.Repository<TaskItem>().Add(hrExecTaskItem);
                         }

                         //UPDATE CVRvw object
                         cvrvw.HRMReviewedOn = dateTimeNow;
                         cvrvw.HRMRemarks = item.HRMRemarks;
                         cvrvw.HRMReviewResultId = EnumSelStatus.Selected;
                    } else {  //rejected
                         cvrvw.HRMRemarks = item.HRMRemarks;
                         cvrvw.HRMReviewResultId = item.HRMReviewResultId;
                    }
                    _unitOfWork.Repository<CVRvw>().Update(cvrvw);

                    //TODO - set HRMTask as completed
                    //mark sup task as completed
                    var hrmTask = await _context.Tasks.FindAsync(cvrvw.HRMTaskId);
                    if (hrmTask != null) {
                         hrmTask.TaskStatus = "Completed";
                         hrmTask.CompletedOn = dateTimeNow;
                         _unitOfWork.Repository<ApplicationTask>().Update(hrmTask);

                         var hrmtaskitem = new TaskItem();
                         if(item.HRMReviewResultId == EnumSelStatus.Selected) {
                              hrmtaskitem =  new TaskItem((int)EnumTaskType.SubmitCVToHRMMgrForReview, 
                                   hrmTask.Id,  dateTimeNow, "Completed", 
                                   "Candidate approved", item.CommonDataDto.OrderId, item.OrderItemId, 
                                   item.CommonDataDto.OrderNo, loggedInDto.LoggedInEmployeeId,
                                   dateTimeNow.AddDays(2), item.CandidateId,  item.AssignedToId, 0  //, item.ParentTask
                              );
                              _unitOfWork.Repository<TaskItem>().Add(hrmtaskitem);
                         } else {
                              hrmtaskitem =  new TaskItem((int)EnumTaskType.SubmitCVToHRMMgrForReview, 
                                   hrmTask.Id,  dateTimeNow, "Completed", 
                                   "Candidate rejected", 0, item.OrderItemId, 
                                   0, loggedInDto.LoggedInEmployeeId,
                                   dateTimeNow.AddDays(2), item.CandidateId,  item.AssignedToId, 0//, item.ParentTask
                              );
                              _unitOfWork.Repository<TaskItem>().Add(hrmtaskitem);
                         }
                    }
               }
               
               await _unitOfWork.Complete();      //save Supervisor/Doc Controller Task
               
               //UPDATE cvrvw.TaskId
               var rvws = await _context.CVReviews.Where(x => cvsSubmittedDto.Select(x => x.CVReviewId).ToList()
                    .Contains(x.Id)).ToListAsync();
               foreach(var rvw in rvws)
               {
                    var tasktypeid = (int)EnumTaskType.SubmitCVToDocControllerAdmin;
                    var docControllerTaskId = await _context.Tasks.Where(x => 
                         x.CVReviewId==rvw.Id && x.TaskTypeId == tasktypeid && x.TaskDate==dateTimeNow)
                         .Select(x => x.Id).FirstOrDefaultAsync();
                    rvw.DocControllerAdminTaskId=docControllerTaskId;      
                    rvw.DocControllerAdminEmployeeId = gDocControllerId;                   
                    
                    _unitOfWork.Repository<CVRvw>().Update(rvw);
               }
               //await _unitOfWork.Complete();      //isnert cv reviews

               //issue email advisory to task owners
               var msg = new EmailMessage();
               var lstMsgs = new List<EmailMessage>();

               var assignees = cvsSubmittedDto.Select(x => x.AssignedToId).Distinct().ToList();
               foreach(var item in assignees)
               {
                    var cands = cvsSubmittedDto.Where(x => x.AssignedToId == item).Select(x => x.CommonDataDto).ToList();
                    if (cands.Count > 0) {
                         msg = await _composeMsgForInternalReviewHR.ComposeHTMLToPublish_CVReviewedByHRManager(cands, loggedInDto, item);
                         if (msg != null) lstMsgs.Add(msg);
                    }
               }
               
               foreach(var item in lstMsgs)
               {
                    _unitOfWork.Repository<EmailMessage>().Add(item);
                    
                    var attachments = new List<string>();        // TODO - should this be auto-sent?
                    _emailService.SendEmail(item, attachments);
               }
               
               await _unitOfWork.Complete();     //save email messages
               return lstMsgs;
          }
          
          public async Task<bool> DeleteCVSubmittedToHRMForReview(int CVReviewId)
          {
               //CHECK IF already reviewed by HR Sup
               var cvrvw = await _context.CVReviews.Where(x => x.Id == CVReviewId &&
                    x.SupReviewResultId != EnumSelStatus.NotReviewed &&    //REVIEWED BY SUPERVISOR, THAT IS OK    
                    x.HRMReviewResultId == EnumSelStatus.NotReviewed)      //not reviewed by HR Manager
                    .FirstOrDefaultAsync();

               if (cvrvw != null) throw new Exception(
                    "Either the HR Supervisor has not reviewed the CV, or it is reviewed and already passed on to Doc Controller");

               //nullify HR Supervisor reviews
               cvrvw.SupReviewResultId = EnumSelStatus.NotReviewed;
               cvrvw.ReviewedBySupOn = Convert.ToDateTime("01-01-1900");
               
               _unitOfWork.Repository<CVRvw>().Update(cvrvw);

               //delete any task created for HRM
               if (cvrvw.HRMTaskId >0) {
                    var hrmtask = await _context.Tasks.FindAsync(cvrvw.HRMTaskId);
                    if (hrmtask != null) _unitOfWork.Repository<ApplicationTask>().Delete(hrmtask); //cascade delete for taskitems
               }
               
               return await _unitOfWork.Complete() > 0;

          }

          public async Task<bool> UserIsOwnerOfCVReviewBySupObject(int CVReviewBySupId, int loggedInEmpId)
          {
               return await _context.CVReviews.Where(x => x.HRSupId == CVReviewBySupId
                    && x.HRExecutiveId == loggedInEmpId).FirstOrDefaultAsync() != null;
          }
          public async Task<bool> DeleteCVSubmittedToHRSupForReview(int cvreviewId)
          {
               //CHECK IF already reviewed by HR Sup
               var cvrvw = await _context.CVReviews.Where(x => x.Id == cvreviewId 
                    && x.SupReviewResultId != EnumSelStatus.NotReviewed).FirstOrDefaultAsync();

               if (cvrvw != null) throw new Exception("cannot delete the record as Supervisor has already reviewed it");

               //delete the task created in the name of teh HR Supervisor or DOcController
               if(cvrvw.DocControllerAdminTaskId > 0) {
                    var task = await _context.Tasks.FindAsync(cvrvw.DocControllerAdminTaskId);
                    if (task != null)_unitOfWork.Repository<ApplicationTask>().Delete(task);
               }
               //delete the object cvrvw;
               _unitOfWork.Repository<CVRvw>().Delete(cvrvw);

               return await _unitOfWork.Complete() > 0;

          }

          public async Task<ApplicationTask> GetHRExecTaskForCVCompiling(int orderitemId, int assignedToId)
          {
               return await _context.Tasks.Where(x => x.OrderItemId == orderitemId && x.AssignedToId == assignedToId
               && x.TaskTypeId == (int)EnumTaskType.AssignTaskToHRExec).FirstOrDefaultAsync();
          }

          public async Task<ApplicationTask> GetHRSupTaskForCVReview(int orderitemId, int assignedToId)
          {
               return await _context.Tasks.Where(x => x.OrderItemId == orderitemId && x.AssignedToId == assignedToId
               && x.TaskTypeId == (int)EnumTaskType.SubmitCVToHRMMgrForReview).FirstOrDefaultAsync();
          }

          public async Task<CVRvw> GetCVReview(int candidateid, int orderitemid)
          {
               return await _context.CVReviews.Where(x => x.CandidateId == candidateid && x.OrderItemId == orderitemid)
                    .FirstOrDefaultAsync();
          }

          public async Task<ICollection<CVRvw>> GetCVReviews (int orderitemid)
          {
               return await _context.CVReviews.Where(x => x.OrderItemId == orderitemid)
                    .OrderBy(x => x.SubmittedByHRExecOn).ToListAsync();
          }

          public async Task<ICollection<EmailMessage>> CVReviewedByHRM(LoggedInUserDto loggedInDto, ICollection<CVReviewByHRMDto> cvsReviewed)
          {
               DateTime dateTimeNow = DateTime.Now;

               //CREATE TASKS FOR DoccONTROLLER
               foreach(var item in cvsReviewed)
               {
                    var commonData = await _commonServices.CommonDataFromOrderDetailIdAndCandidateId(item.OrderItemId,
                         item.CandidateId);

                    item.CommonDataDto = commonData;

                    var docControllerTask = new ApplicationTask((int)EnumTaskType.SubmitCVToDocControllerAdmin,
                         dateTimeNow, item.HRMId, gDocControllerId, item.CommonDataDto.OrderId, item.CommonDataDto.OrderNo, 
                         item.OrderItemId, "Flg CV ready to forward to client:" + item.CommonDataDto.CandidateDesc,
                         dateTimeNow.AddDays(mTargetDays_ForwardCVToClient), "Open", item.CandidateId, null);
               
                    _unitOfWork.Repository<ApplicationTask>().Add(docControllerTask);
               }
               await _unitOfWork.Complete();      //saves doc controller task

               //Update CReviews and hrmtasks
               foreach(var item in cvsReviewed)
               {
                    var cvrvw = await _context.CVReviews.Where(x => x.OrderItemId == item.OrderItemId
                         && x.CandidateId == item.CandidateId).FirstOrDefaultAsync();
                    var doccontrollertaskId = await _context.Tasks.Where(
                         x => x.CandidateId == item.CandidateId && x.OrderItemId == item.OrderItemId
                         && x.TaskDate == dateTimeNow && x.TaskTypeId == (int)EnumTaskType.SubmitCVToDocControllerAdmin)
                         .Select(x => x.Id) .FirstOrDefaultAsync();
                    cvrvw.HRMReviewResultId = item.HRMReviewResultId;
                    cvrvw.HRMReviewedOn = dateTimeNow;
                    cvrvw.HRMRemarks = item.HRMRemarks;
                    cvrvw.DocControllerAdminTaskId = doccontrollertaskId;
                    cvrvw.DocControllerAdminEmployeeId = gDocControllerId;
                    _unitOfWork.Repository<CVRvw>().Update(cvrvw);

                    var hrmtask = await _context.Tasks.FindAsync(cvrvw.HRMTaskId);
                    hrmtask.TaskStatus = "Completed";
                    hrmtask.CompletedOn = dateTimeNow;
                    _unitOfWork.Repository<ApplicationTask>().Update(hrmtask);
               }

               //publish notifications
               var lstMsgs = new List<EmailMessage>();
               var msg = new EmailMessage();
               var assignees = cvsReviewed.Select(x => x.AssignedToId).Distinct();
               foreach (var assignee in assignees)
               {
                    var cvs = cvsReviewed.Where(x => x.AssignedToId == assignee).ToList();
                    var cands = cvs.Select(x => x.CommonDataDto).ToList();
                    foreach (var cv in cvs)
                    {
                         msg = await _composeMessages.ComposeHTMLToPublish_CVReadiedToForwardToClient(cands, loggedInDto, cv.AssignedToId);
                         if (msg!= null)
                         {
                              var attachments = new List<string>();        // TODO - should this be auto-sent?
                              msg = _emailService.SendEmail(msg, attachments);
                         }
                         lstMsgs.Add(msg);
                    }
               }
               return lstMsgs;
          }

          public async Task<ICollection<CVReviewsPendingDto>>PendingCVReviews()
          {
               return await (from c in _context.CVReviews where 
                    (c.SupReviewResultId == EnumSelStatus.NotReviewed && !c.NoReviewBySupervisor) || 
                    (c.SupReviewResultId !=EnumSelStatus.NotReviewed  && c.HRMReviewResultId == EnumSelStatus.NotReviewed)
               join i in _context.OrderItems on c.OrderItemId equals i.Id
               join o in _context.Orders on i.OrderId equals o.Id
               join cat in _context.Categories on i.CategoryId equals cat.Id
               join cust in _context.Customers on o.CustomerId equals cust.Id
               join emp in _context.Employees on c.HRSupId equals emp.Id
               join emp1 in _context.Employees on c.HRMId equals emp1.Id
               select new CVReviewsPendingDto {
                    CVReviewId = c.Id, CandidateId=c.CandidateId, OrderId = o.Id, OrderItemId = c.OrderItemId,
                    SubmittedByHRExecOn = c.SubmittedByHRExecOn.Date, 
                    ReviewedBySupOn = (int)c.SupReviewResultId != (int)EnumSelStatus.NotReviewed 
                         ? Convert.ToDateTime(c.ReviewedBySupOn).Date : null,
                    SupReviewResultId = (EnumSelStatus)c.SupReviewResultId,
                    ReviewPendingByEmpName = (int)c.SupReviewResultId == 100 ? "HR Sup " + emp.KnownAs : (int)c.HRMReviewResultId == 100 ? "HRM " + emp1.KnownAs: ""
               }).ToListAsync();
          }

          public async Task<ICollection<CVReviewsPendingDto>>PendingCVReviewsByUserIdAsync(int userId)
          {
               return await (from c in _context.CVReviews where 
                    (c.HRSupId == userId && c.SupReviewResultId == EnumSelStatus.NotReviewed 
                         && !c.NoReviewBySupervisor) || 
                    (c.HRMId == userId && c.SupReviewResultId !=EnumSelStatus.NotReviewed  
                         && c.HRMReviewResultId == EnumSelStatus.NotReviewed)
               join i in _context.OrderItems on c.OrderItemId equals i.Id
               join o in _context.Orders on i.OrderId equals o.Id
               join cat in _context.Categories on i.CategoryId equals cat.Id
               join cust in _context.Customers on o.CustomerId equals cust.Id
               select new CVReviewsPendingDto {
                    CVReviewId = c.Id, CandidateId=c.CandidateId, OrderId = o.Id, OrderItemId = c.OrderItemId,
                    SubmittedByHRExecOn = c.SubmittedByHRExecOn, 
                    ReviewedBySupOn = (int)c.SupReviewResultId != (int)EnumSelStatus.NotReviewed 
                         ? Convert.ToDateTime(c.ReviewedBySupOn) : null,
                    SupReviewResultId = (EnumSelStatus)c.SupReviewResultId
                    }).ToListAsync();
          }
          private async Task<int> GetCVReviewId(int candidateId, int orderItemId)
          {
               return await _context.CVReviews.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId).Select(x => x.Id).FirstOrDefaultAsync();
          }

          public Task<int> NextReviewBy(int orderItemId)
          {
               throw new NotImplementedException();
          }
     }
}