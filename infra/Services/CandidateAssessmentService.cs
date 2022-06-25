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


namespace infra.Services
    {
        public class CandidateAssessmentService : ICandidateAssessmentService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ATSContext _context;
            private readonly IChecklistService _checklistHRService;
            private readonly ICommonServices _commonServices;
            private readonly ITaskService _taskService;
            public CandidateAssessmentService(IUnitOfWork unitOfWork, ATSContext context, 
                    IChecklistService checklistHRService, ICommonServices commonServices, ITaskService taskService)
            {
                _commonServices = commonServices;
                _checklistHRService = checklistHRService;
                _context = context;
                _unitOfWork = unitOfWork;
                _taskService=taskService;
            }


            //returns error strig if candidate not checklisted
            public async Task<CandidateAssessmentWithErrorStringDto> GetorCreateNewCandidateAssessment(bool requireInternalReview, int candidateId, int orderItemId, int loggedInEmployeeId)
            {
                
                var dto = new CandidateAssessmentWithErrorStringDto();

                var checklistHRid = await _checklistHRService.GetChecklistHRId(candidateId, orderItemId);
                if (checklistHRid == 0)  {
                    dto.ErrorString="Candidate not Checklisted";
                    return dto;      //No HR checklisting done
                }
                
                //check if the assessment already exists
                var candass = await (from ass in _context.CandidateAssessments where ass.CandidateId == candidateId && ass.OrderItemId == orderItemId
                    join e in _context.Employees on ass.AssessedById equals e.Id into emp
                    from empass in emp.DefaultIfEmpty()     //left join
                    select new {ass, empass.KnownAs} 
                    ).FirstOrDefaultAsync();
                
                if (candass != null) {
                    candass.ass.AssessedByName = candass.KnownAs;
                    dto.CandidateAssessment=candass.ass;
                    dto.ErrorString="Candidte already assessed for the same requirement";
                    return dto;
                }

                var candassessment = candass?.ass;
                //var loggedInUserId = await _context.Employees.Where(x => x.AppUserId == loggedInIdentityUserId).Select(x => x.Id).FirstOrDefaultAsync();

                var dateIssued = DateTime.Now;
                string knownas = await _commonServices.GetEmployeeNameFromEmployeeId(loggedInEmployeeId);

                if (!requireInternalReview) {        //the orderassessmentitems will not be set
                    candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInEmployeeId, knownas, dateIssued,false, checklistHRid, EnumCandidateAssessmentResult.NotRequired);
                } else {
                    var itemassessment = await _context.OrderItemAssessments
                            .Where(x => x.OrderItemId == orderItemId)
                            .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                            .FirstOrDefaultAsync();
                    if (itemassessment == null) {
                        dto.ErrorString="Order Category Assessment parameters not defined";
                        return  dto;    //throw new System.Exception("Order Category assessment parameters not defined");                
                    }

                    var items = new List<CandidateAssessmentItem>();
                    foreach(var item in itemassessment.OrderItemAssessmentQs)
                    {
                        items.Add(new CandidateAssessmentItem(item.QuestionNo, item.Subject, item.Question, item.IsMandatory, item.MaxMarks));
                    }
    
                    candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInEmployeeId, knownas, dateIssued,true, checklistHRid, items);
                }

                _unitOfWork.Repository<CandidateAssessment>().Add(candassessment);

                if (await _unitOfWork.Complete() > 0) {

                    var qry = await GetOrderItemDetailForMessage(orderItemId, candidateId);

                    var task = await CreateNextApplicationTaskObject(candassessment.Id, orderItemId, 
                        candidateId, qry.ApplicationNo, qry.FullName, qry.NoReviewBySupervisor, qry.SrNo, 
                        (int)qry.HrSupId, (int)qry.HrmId, qry.FullName, qry.OrderId, qry.OrderNo, qry.OrderDate,
                        qry.CustomerName, qry.CustomerCity, candassessment.AssessedByName, candassessment.AssessedOn,
                        loggedInEmployeeId);

                    task.PostTaskAction=EnumPostTaskAction.OnlyComposeEmailMessage;
                    var msgs = await _taskService.CreateNewApplicationTask(task, loggedInEmployeeId);
                    candassessment.TaskIdDocControllerAdmin = task.Id;
                    dto.CandidateAssessment = candassessment;

                    _unitOfWork.Repository<CandidateAssessment>().Update(candassessment);
                    await _unitOfWork.Complete();
                    return dto;

                } else {
                    dto.ErrorString="Failed to update candidate assessment record";
                    return dto;
                }
          }

            public async Task<ICollection<CandidateAssessedDto>> GetAssessedCandidatesApproved ()
            {
                var exceptList = new List<EnumCandidateAssessmentResult>();
                exceptList.Add(EnumCandidateAssessmentResult.NotAssessed);
                exceptList.Add(EnumCandidateAssessmentResult.Poor);
                exceptList.Add(EnumCandidateAssessmentResult.VeryPoor);
               
                var assessedDto = await (from ass in _context.CandidateAssessments 
                    where (ass.requireInternalReview && !exceptList.Contains(ass.AssessResult) 
                            ||!ass.requireInternalReview) 
                            && ass.CvRefId == 0 
                    join cv in _context.Candidates on ass.CandidateId equals cv.Id
                    join item in _context.OrderItems on ass.OrderItemId equals item.Id
                    join o in _context.Orders on item.OrderId equals o.Id 
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join e in _context.Employees on ass.AssessedById equals e.Id
                    join checklist in _context.ChecklistHRs on new {a= item.Id, b = cv.Id} equals new {a = checklist.OrderItemId, b = checklist.CandidateId } into chklst
                    from chk in chklst.DefaultIfEmpty()
                    orderby ass.OrderItemId
                    select new CandidateAssessedDto{
                        Id = ass.Id, OrderItemId = ass.OrderItemId, CandidateId = cv.Id,
                        AssessedById = ass.AssessedById, requireInternalReview = item.RequireInternalReview,
                        CustomerName = c.CustomerName,
                        AssessedOn = ass.AssessedOn, Remarks = ass.Remarks,
                        OrderItemRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                        CandidateName = cv.FullName,
                        ApplicationNo = cv.ApplicationNo,
                        AssessedByName = e.FirstName + " " + e.FamilyName,
                        AssessedResult = ass.AssessResult.ToString(),
                        Charges = chk == null ? "Undefined" 
                            : item.Charges == 0 ? "FOC"
                                : item.Charges == chk.ChargesAgreed ? "Agreed": chk.ExceptionApproved==true ? "Approved" : "Disparity"
                    }).ToListAsync();
                
                return assessedDto;
            }

           
       
            public async Task<CandidateAssessment> GetNewAssessmentObject(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId)
            {
                    var checklisthrid = await _checklistHRService.GetChecklistHRId(candidateId, orderItemId);
                    if (checklisthrid == 0) return null;

                    //check if the assessment already exists
                    var candassessment = new CandidateAssessment();
                    var knownas = await _commonServices.GetEmployeeNameFromEmployeeId(loggedInIdentityUserId);
                    var dateIssued = DateTime.Now;
                    if (!requireInternalReview) {        //the orderassessmentitems will not be set
                        candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInIdentityUserId, knownas, dateIssued,false, checklisthrid, EnumCandidateAssessmentResult.NotRequired);
                    } else {
                        var itemassessment = await _context.OrderItemAssessments
                                .Where(x => x.OrderItemId == orderItemId)
                                .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                                .FirstOrDefaultAsync();
                        if (itemassessment == null) return null;    //throw new System.Exception("Order Category assessment parameters not defined");                

                        var items = new List<CandidateAssessmentItem>();
                        foreach(var item in itemassessment.OrderItemAssessmentQs)
                        {
                            items.Add(new CandidateAssessmentItem(item.QuestionNo, item.Subject, item.Question, item.IsMandatory, item.MaxMarks));
                        }
        
                        candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInIdentityUserId, knownas, dateIssued,true, checklisthrid, items);
                    }
                    
                    return candassessment;
            }

            public async Task<bool> DeleteCandidateAssessment(int assessmentid)
            {
                var candidateAssessment = await _context.CandidateAssessments.FindAsync(assessmentid);
                if (candidateAssessment == null) return false;
                if(candidateAssessment.TaskIdDocControllerAdmin > 0) {
                    var task = await _context.Tasks.FindAsync(candidateAssessment.TaskIdDocControllerAdmin);
                    _unitOfWork.Repository<ApplicationTask>().Delete(task);
                } else {
                        var task = await _context.Tasks.Where(x => 
                            x.CandidateId == candidateAssessment.CandidateId &&
                            x.OrderItemId == candidateAssessment.OrderItemId)
                            .FirstOrDefaultAsync();
                        if (task != null) _unitOfWork.Repository<ApplicationTask>().Delete(task);
                }
                
                _unitOfWork.Repository<CandidateAssessment>().Delete(candidateAssessment);
                return await _unitOfWork.Complete() > 0;
                
            }
            public async Task<bool> DeleteCandidateAssessmentItem(CandidateAssessmentItem assessmentItem)
            {
                _unitOfWork.Repository<CandidateAssessmentItem>().Delete(assessmentItem);
                return await _unitOfWork.Complete() > 0;
            }

            //once a candidate assessment is approved, escalate to next stage
            //the CV is submitted to either HRSup, HRM or Doc Controller, depending upon order item definitions
            public async Task<MessagesDto> EditCandidateAssessment(CandidateAssessment candidateAssessment, int loggedinEmployeeId)
            {
                var msgsDto = new MessagesDto();

                //update assessmentResult field based upon totals of items
                    var assessResult = new EnumCandidateAssessmentResult();
                    var totalPoints=0;
                    if (candidateAssessment.CandidateAssessmentItems.Count > 0){
                        totalPoints=100* candidateAssessment.CandidateAssessmentItems.Sum(x => x.Points) / candidateAssessment.CandidateAssessmentItems.Sum(x => x.MaxPoints);
                        switch(totalPoints)
                        {
                                case >=90:
                                    assessResult = EnumCandidateAssessmentResult.Excellent;
                                    break;
                                case >=80:
                                    assessResult = EnumCandidateAssessmentResult.VeryGood;
                                    break;
                                case >=70:
                                    assessResult = EnumCandidateAssessmentResult.Good;
                                    break;
                                case >=50:
                                    assessResult = EnumCandidateAssessmentResult.Poor;
                                    break;
                                default:
                                    assessResult = EnumCandidateAssessmentResult.VeryPoor;
                                    break;
                        }
                        candidateAssessment.AssessResult=assessResult;
                    }
                    
                    
                _unitOfWork.Repository<CandidateAssessment>().Update(candidateAssessment);

                    foreach(var item in candidateAssessment.CandidateAssessmentItems)
                    {
                        _unitOfWork.Repository<CandidateAssessmentItem>().Update(item); 
                    }

                    if (await _unitOfWork.Complete() == 0) {
                        msgsDto.ErrorString = "Failed to update Candidate Assessment records";
                        return msgsDto;
                    }
                    
                    var qry = await GetOrderItemDetailForMessage(candidateAssessment.OrderItemId, candidateAssessment.CandidateId);
                        if (qry==null) {
                            msgsDto.ErrorString="Failed to retrieve order item details - this is severe in nature, please inform your developers";
                            return msgsDto;     //this should never happen
                        }
                    var taskdescription="Application No.:" + qry.ApplicationNo + ", Candidate: " + qry.FullName + ", assessed for " + qry.CustomerName +
                            ", " + qry.CustomerCity + "against Order No.:" + qry.OrderNo + " dated " + qry.OrderDate + ", category ref: " + qry.SrNo +
                            "-" + qry.CategoryName + ", assessed by " + candidateAssessment.AssessedByName + " on " + candidateAssessment.AssessedOn;

                    var task = await _context.Tasks.Where(x =>
                            x.OrderItemId == candidateAssessment.OrderItemId && x.CandidateId == candidateAssessment.CandidateId &&
                            x.TaskTypeId == (int)EnumTaskType.CVForwardToCustomers && x.AssignedToId == 4
                        ).FirstOrDefaultAsync();

                    if (task!= null) {
                         if (task.CVReviewId != candidateAssessment.Id) {
                            task.CVReviewId=candidateAssessment.Id;
                            task.TaskDescription=taskdescription;   
                            msgsDto = await _taskService.EditApplicationTask(task, loggedinEmployeeId); //consider to do all db updates here instead
                         } 
                        
                    } else {
                        task = await CreateNextApplicationTaskObject(candidateAssessment.Id, candidateAssessment.OrderItemId, 
                            candidateAssessment.CandidateId, qry.ApplicationNo, qry.FullName, qry.NoReviewBySupervisor, qry.SrNo, 
                            (int)qry.HrSupId, (int)qry.HrmId, qry.FullName, qry.OrderId, qry.OrderNo, qry.OrderDate,
                            qry.CustomerName, qry.CustomerCity, candidateAssessment.AssessedByName, candidateAssessment.AssessedOn,
                            loggedinEmployeeId);
                        
                        msgsDto = await _taskService.CreateNewApplicationTask(task, loggedinEmployeeId);    //*TODO* this saves the data. consider all db changes here in one _unitOFWork.Complete()
                    
                    }

                    //create tasks for DocumentControllerAdmin;
                    
                    
                    if(task != null) candidateAssessment.TaskIdDocControllerAdmin = task.Id;
                    _unitOfWork.Repository<CandidateAssessment>().Update(candidateAssessment);
                    await _unitOfWork.Complete();
                    if (msgsDto==null || msgsDto?.emailMessages?.Count ==0) {
                        msgsDto.ErrorString="Failed to create tasks for the document administrator";
                        return msgsDto;
                    }
                    
                    return msgsDto;
            }

            private async Task<OrderItemDetailForMsgDto> GetOrderItemDetailForMessage(int OrderItemId, int CandidateId)
          {
                var qry = await (from i in _context.OrderItems where i.Id == OrderItemId
                    join cv in _context.Candidates on CandidateId equals cv.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new  OrderItemDetailForMsgDto{
                        SrNo = i.SrNo, HrSupId=(int)i.HrSupId, HrmId=(int)i.HrmId, CategoryName=cat.Name, OrderId=o.Id, 
                        OrderNo=o.OrderNo, OrderDate=o.OrderDate, CustomerName=c.CustomerName, CustomerCity=c.City, 
                        ApplicationNo=cv.ApplicationNo, FullName=cv.FullName, NoReviewBySupervisor=i.NoReviewBySupervisor})
                    .FirstOrDefaultAsync();
                return qry;
          }

            private async Task<ApplicationTask> CreateNextApplicationTaskObject(int candidateassessmentid, int orderitemid,
                int candidateid,int applicationno, string candidatename, bool noreviewbysup, int srno, int hrsupid, int hrmid, 
                string catname, int orderid, int orderno, DateTime orderdate, string customername, string cityname, 
                string assessedbyname, DateTime assessedon, int taskownerid)
        {
               //create tasks for DocumentControllerAdmin;
                string EnvAssignedToId="";
                string EnvPeriodInDays="";
                int intDays = 0;
                int intAssignedToId=0;
                string taskdescription="";
                string assignedtoname="";

                string appdetails="Application No.:" + applicationno +
                    ", Candidate: " + candidatename + ", assessed for " + customername + ", " + cityname +
                    "against Order No.:" + orderno + " dated " + orderdate + ", category ref: " + srno + "-" + catname +
                    ", assessed by " + assessedbyname + " on " + assessedon;

                if (noreviewbysup || (hrsupid ==0 && hrmid ==0)) {
                        EnvAssignedToId="4";  //Environment["EmpDocControllerAdminId"]; // .GetEnvironmentVariable("EmpDocControllerAdminId");
                        EnvPeriodInDays = "1";    //Environment.GetEnvironmentVariable("TargetDays_DocControllerToFwdCV");

                        taskdescription= appdetails + " -  is approved for forwarding to client";
                } else if (hrsupid > 0) {
                        intAssignedToId=hrsupid;
                        EnvPeriodInDays = "2";    //Environment.GetEnvironmentVariable("TargetDays_HRSupToReviewCV");
                        
                        taskdescription= appdetails + " - is submitted to HR Supervisor for review";
                } else if (hrmid > 0) {
                        intAssignedToId=hrmid;
                        EnvPeriodInDays = "2";    // Environment.GetEnvironmentVariable("TargetDays_HRMToReviewCV");
                        
                        taskdescription= appdetails + " - is submitted to HR Manager for review";
                }
                    
                if (!string.IsNullOrEmpty(EnvAssignedToId)) intAssignedToId=Convert.ToInt32(EnvAssignedToId);
                if (!string.IsNullOrEmpty(EnvPeriodInDays)) intDays=Convert.ToInt32(EnvPeriodInDays);
                if(intAssignedToId>0)assignedtoname = await _commonServices.GetEmployeeNameFromEmployeeId(intAssignedToId);


                var newtask = new ApplicationTask((int)EnumTaskType.CVForwardToCustomers, 
                    DateTime.Now, taskownerid, intAssignedToId, orderid, orderno, orderitemid, taskdescription, 
                    DateTime.Now.AddDays(intDays), "Not Started", candidateid, candidateassessmentid);
                
                return newtask;

        }

            public async Task<CandidateAssessment> GetCandidateAssessment(int candidateId, int orderItemId)
          {
              var assess = await _context.CandidateAssessments
                .Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
                .Include(x => x.CandidateAssessmentItems.OrderBy(x => x.QuestionNo))
                .FirstOrDefaultAsync();
            
                return assess;
          }
        
        //IF CHECKLIST does not exist, creates a new one
        public async Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentAndChecklist(int candidateId, int orderItemId, int loggedInEmployeeId, string loggedInUserName)
          {
            
            /*var assessandchecklist = await (from ass in  _context.CandidateAssessments 
                    where ass.CandidateId == candidateId && ass.OrderItemId == orderItemId 
                join checklist in _context.ChecklistHRs on ass.HrChecklistId equals checklist.Id into asschecklist 
                from chklst in asschecklist.DefaultIfEmpty()
                select new CandidateAssessmentAndChecklist {CandidateAssessment = ass, ChecklistHR = chklst})
                .FirstOrDefaultAsync();
            */

            var one = await _context.CandidateAssessments
                .Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
                .Include(x => x.CandidateAssessmentItems)
                .FirstOrDefaultAsync();
            var temp = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateId && x.OrderItemId==orderItemId).Include(x => x.ChecklistHRItems).FirstOrDefaultAsync();
            if (temp == null) {
                temp = await _checklistHRService.GetOrCreateNewChecklistHR(candidateId, orderItemId, loggedInEmployeeId, loggedInUserName);
            }
            
            var twof = await (from chklst in  _context.ChecklistHRs 
                        where chklst.CandidateId == candidateId && chklst.OrderItemId == orderItemId 
                    join chklstItems in _context.ChecklistHRItems on chklst.Id equals chklstItems.ChecklistHRId
                    join item in _context.OrderItems on chklst.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join cv in _context.Candidates on chklst.CandidateId equals cv.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join e in _context.Employees on chklst.UserId equals e.Id into emp
                    from em in emp.DefaultIfEmpty()
            
                    select new ChecklistHRDto {
                        Id = chklst.Id, 
                        ApplicationNo = cv.ApplicationNo,
                        OrderItemId = orderItemId,
                        CandidateName = string.IsNullOrEmpty(cv.FullName) ? "" : cv.FullName,
                        CandidateId = candidateId,
                        UserLoggedId = chklst.UserId, 
                        UserLoggedName = em.KnownAs,
                        CheckedOn = chklst.CheckedOn, 
                        CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                        OrderRef = c.CustomerName,
                        HrExecComments = string.IsNullOrEmpty(chklst.HrExecComments) ? "" : chklst.HrExecComments,
                        Charges = item.Charges, ChargesAgreed = chklst.ChargesAgreed,
                        ExceptionApproved = chklst.ExceptionApproved, 
                        ExceptionApprovedBy = chklst.ExceptionApprovedBy,
                        ExceptionApprovedOn = chklst.ExceptionApprovedOn,
                        checklistHRItems = (ICollection<ChecklistHRItem>)chklstItems
                    }
                ).FirstOrDefaultAsync();
            
            return new CandidateAssessmentAndChecklistDto(one, twof);

          }

       
     }
}