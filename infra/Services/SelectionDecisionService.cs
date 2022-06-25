using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class SelectionDecisionService : ISelectionDecisionService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IDeployService _deployService;
          private readonly ATSContext _context;
          private readonly IComposeMessages _composeMessages;
          private readonly IEmailService _emailService;
          private readonly IMapper _mapper;
          private readonly IComposeMessagesForAdmin _composeMsgForAdmin;
          private readonly int HRSupEmpTaskId=12;
          public SelectionDecisionService(IUnitOfWork unitOfWork, ATSContext context, ICommonServices commonServices, IComposeMessagesForAdmin composeMsgForAdmin,
          IComposeMessages composeMessages, IDeployService deployService, IEmailService emailService, IMapper mapper)
          {
               _composeMsgForAdmin = composeMsgForAdmin;
               _mapper = mapper;
               _emailService = emailService;
               _composeMessages = composeMessages;
               _context = context;
               _deployService = deployService;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
          }

          public async Task<bool> DeleteSelection(int id)
          {
               var selectionDecision = await _context.SelectionDecisions.FindAsync(id);
               if (selectionDecision == null) return false;
               
               _unitOfWork.Repository<SelectionDecision>().Delete(selectionDecision);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditSelection(SelectionDecision selectionDecision)
          {
               _unitOfWork.Repository<SelectionDecision>().Update(selectionDecision);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<Pagination<SelectionDecision>> GetSelectionDecisions(SelDecisionSpecParams specParams)
          {
               var spec = new SelDecisionSpecs(specParams);
               var specCount = new SelDecisionForCountSpecs(specParams);
               var decisions = await _unitOfWork.Repository<SelectionDecision>().ListAsync(spec);
               var ct = await _unitOfWork.Repository<SelectionDecision>().CountAsync(specCount);

               return new Pagination<SelectionDecision>(specParams.PageIndex, specParams.PageSize, ct, decisions);
          }

          public async Task<SelectionMsgsAndEmploymentsDto> RegisterSelections(ICollection<SelDecisionToAddDto> selDto, int loggedInEmployeeId)
          {
               DateTime dateTimeNow = DateTime.Now;
               var seldecisions = new List<SelectionDecision>();
               var msgs = new List<EmailMessage>();
               var empDtos = new List<EmploymentDto>();
               var cvrefids = selDto.Select(x => x.CVRefId).ToList();

               SelectionDecision selDecision;

               //var cvrefs = await _context.CVRefs.Where(x => cvrefids.Contains(x.Id)).ToListAsync();
               var recAffected=0;
               var details = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                    select new {
                         cvref, CandidateName = cv.FullName, CategoryRef=o.OrderNo + "-" + item.SrNo + cat.Name,
                         CustomerName = c.CustomerName, ApplicationNo = cv.ApplicationNo, Charges=item.Charges,
                         OrderItemId = cvref.OrderItemId, OrderId = o.Id, OrderNo = o.OrderNo, CategoryId = item.CategoryId,
                         CandidateId=cv.Id
                    }).ToListAsync();
               
               foreach(var dtl in details) {
                    var cvref = dtl.cvref;
                    if(cvref.ApplicationNo==0) cvref.ApplicationNo=dtl.ApplicationNo;
                    if(string.IsNullOrEmpty(cvref.CandidateName)) cvref.CandidateName=dtl.CandidateName;
                    if(string.IsNullOrEmpty(cvref.CategoryName)) cvref.CandidateName=dtl.CategoryRef;
                    if(string.IsNullOrEmpty(cvref.CustomerName)) cvref.CustomerName = dtl.CustomerName;
                    if(cvref.Charges==0) cvref.Charges = dtl.Charges;
                    if(cvref.OrderItemId==0) cvref.OrderItemId = dtl.OrderItemId;
                    if(cvref.OrderId==0) cvref.OrderId = dtl.OrderId;
                    if(cvref.OrderNo==0) cvref.OrderNo = dtl.OrderNo;
                    if(cvref.CategoryId==0) cvref.CategoryId = dtl.CategoryId;
               }
               var employmentCVRefIdsAdded = new List<int>();
               foreach (var s in selDto)
               {
                    var cvref = details.Where(x => x.cvref.Id== s.CVRefId).Select(x => x.cvref).FirstOrDefault();
                    if (s.SelectionStatusId == (int)EnumCVRefStatus.Selected)
                    {
                         selDecision = new SelectionDecision(cvref.Id, cvref.OrderItemId, cvref.CategoryId, cvref.CategoryName,
                              cvref.OrderId, cvref.OrderNo, cvref.CustomerName, cvref.ApplicationNo, cvref.CandidateId,
                              cvref.CandidateName, s.DecisionDate, s.SelectionStatusId, s.Remarks);
                         var deployTrans = new Deploy(cvref.Id, s.DecisionDate, EnumDeployStatus.Selected);
                         _unitOfWork.Repository<Deploy>().Add(deployTrans);     
                         recAffected++;

                         seldecisions.Add(selDecision);
                         recAffected++;
                         _unitOfWork.Repository<SelectionDecision>().Add(selDecision);
                         recAffected++;

                         //create employment record
                         var salCurrency = await getSalaryCurrency(cvref.OrderItemId);
                         var emp = await _context.Employments.Where(x => x.CVRefId == cvref.Id).FirstOrDefaultAsync();
                         if(emp==null) {
                              emp = new Employment(cvref.Id, s.DecisionDate,salCurrency,0,24,false,0,false,0,false,0,0,21,24,0 );
                              _unitOfWork.Repository<Employment>().Add(emp);
                              recAffected++;
                              employmentCVRefIdsAdded.Add(cvref.Id);
                         }
                         var HRExecTask = await _context.Tasks.Where(x => x.OrderItemId == cvref.OrderItemId && 
                              x.CandidateId == cvref.CandidateId && x.AssignedToId == (cvref.HRExecId == 0 ? HRSupEmpTaskId : cvref.HRExecId)
                              && x.TaskTypeId == (int)EnumTaskType.OfferLetterAcceptance).FirstOrDefaultAsync();
                         if (HRExecTask==null) {
                              HRExecTask = new ApplicationTask((int)EnumTaskType.OfferLetterAcceptance, dateTimeNow,
                                   loggedInEmployeeId, cvref.HRExecId == 0 ? HRSupEmpTaskId : cvref.HRExecId, cvref.OrderId, cvref.OrderNo, cvref.OrderItemId,
                                   "Get Candidate's acceptance of the selection term " + "Application No. " + cvref.ApplicationNo + ", " +
                                   cvref.CandidateName + " selected for " + cvref.CustomerName + " on " + dateTimeNow.Date,
                                        dateTimeNow.AddDays(2), "Open", cvref.CandidateId, 0);
                              _unitOfWork.Repository<ApplicationTask>().Add(HRExecTask);
                              recAffected++;
                         }
                    
                         cvref.DeployStageId = (int)EnumDeployStatus.Selected;
                         cvref.DeployStageDate = dateTimeNow;
                    }
                    //update CVRef
                    cvref.RefStatus = s.SelectionStatusId;
                    
                    _unitOfWork.Repository<CVRef>().Update(cvref);
                    recAffected++;

                    //update candidateStatus
                    
                    //update doc controller task for selection

                    var docTask = await _context.Tasks.Where(x => x.CandidateId == cvref.CandidateId &&
                         x.OrderItemId == cvref.OrderItemId && x.TaskTypeId == (int)EnumTaskType.SelectionFollowupByDocControllerAdmin)
                         .FirstOrDefaultAsync();
                    if (docTask != null)
                    {
                         docTask.TaskStatus = "Completed";
                         docTask.CompletedOn = dateTimeNow;
                         _unitOfWork.Repository<ApplicationTask>().Update(docTask);
                    }
               }

               var recordsAffected = await _unitOfWork.Complete();
               if (recordsAffected > 0)
               {
                    var selDecDto = new List<SelectionDecisionMessageParamDto>();
                    //var selected = seldecisions.Where(x => x.SelectionStatusId == (int)EnumSelStatus.Selected).ToList();
                    if (seldecisions != null && seldecisions.Count > 0)
                    {
                         foreach (var s in seldecisions)
                         {
                              selDecDto.Add(new SelectionDecisionMessageParamDto { SelectionDecision = s, DirectlySendMessage = false });
                         }

                         var msg = await _composeMsgForAdmin.AdviseSelectionStatusToCandidateByEmail(selDecDto);
                         if (msg != null)
                         {
                              foreach (var m in msg) { msgs.Add(m); }
                         }
                    }
                    var rejectionDto = selDto.Where(x => x.SelectionStatusId != (int)EnumSelStatus.Selected).ToList();
                    var rejected = _mapper.Map<ICollection<SelDecisionToAddDto>, ICollection<RejDecisionToAddDto>>(rejectionDto);
                    
                    //var rejected = new selDto.Where(x => x.SelectionStatusId != (int)EnumSelStatus.Selected).ToList();
                    if (rejected.Count > 0)
                    {
                         var msg = _composeMsgForAdmin.AdviseRejectionStatusToCandidateByEmail(rejected);
                         if (msg != null)
                         { foreach (var m in msg) { msgs.Add(m); } }
                    }
                    var attachments = new List<string>();
                    foreach (var msg in msgs)
                    {
                         //_emailService.SendEmail(msg, attachments);      //**TODO** compose msg in OUtlook
                    }
                    var emps = await _context.Employments.Where(x => employmentCVRefIdsAdded.Contains(x.CVRefId)).ToListAsync();
                    var empdtos = _mapper.Map<ICollection<Employment>, ICollection<EmploymentDto>>(emps);

                    //update candidate.candiadtestatus
                    var candidates = await _context.Candidates.Where(x => details.Select(x => x.CandidateId).ToList().Contains(x.Id)).ToListAsync();
                    
                    var qry = (from r in selDto 
                         join cvref in _context.CVRefs on r.CVRefId equals cvref.Id
                         join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                         select new {cand, r.SelectionStatusId}).ToList();
                    foreach(var q in qry) {
                         q.cand.CandidateStatus=q.SelectionStatusId==(int)EnumSelStatus.Selected ? "Selected": "Available";
                         _unitOfWork.Repository<Candidate>().Update(q.cand);
                    }
                    return new SelectionMsgsAndEmploymentsDto{EmailMessages = msgs, EmploymentDtos=empdtos};

               }
               else
               {
                    return null;
               }
          }

          private async Task<string> getSalaryCurrency(int orderItemId)
          {
              var remuneration = await _context.Remunerations.Where(x => x.OrderItemId == orderItemId).Select(x => x.SalaryCurrency).FirstOrDefaultAsync();
              if (string.IsNullOrEmpty(remuneration)) return "Undefined";
              return remuneration;

          }
          public async Task<ICollection<SelectionStatus>> GetSelectionStatus()
          {
               return await _context.SelectionStatuses.OrderBy(x => x.Status).ToListAsync();
          }

          public async Task<Pagination<SelectionsPendingDto>> GetPendingSelections(CVRefSpecParams specParams)
          {
          
               var qry2 = (from r in _context.CVRefs
                    where r.RefStatus == (int)EnumCVRefStatus.Referred
                    join i in _context.OrderItems on r.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join p in _context.Categories on i.CategoryId equals p.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    orderby r.ReferredOn
                    select new SelectionsPendingDto
                    {
                         CVRefId = r.Id,
                         OrderItemId = r.OrderItemId,
                         OrderNo = o.OrderNo,
                         CandidateName = cand.FullName,
                         ApplicationNo = cand.ApplicationNo,
                         CategoryName = p.Name,
                         CustomerName = c.CustomerName,
                         ReferredOn = r.ReferredOn.Date,
                         RefStatus = r.RefStatus
                    })
                    .AsQueryable();
               var totalcount = await qry2.CountAsync();
               var data = await qry2.Skip((specParams.PageIndex -1)*specParams.PageSize).Take(specParams.PageSize)
                    .ToListAsync();

               /*
               specParams.CVRefStatus=(int)EnumCVRefStatus.Referred;

               var specs = new CVRefSpecs(specParams);
               var countSpec = new CVRefForCountSpecs(specParams);
               var totalItems = await _unitOfWork.Repository<CVRef>().CountAsync(countSpec);
               var cvs = await _unitOfWork.Repository<CVRef>().ListAsync(specs);

               //*PROJECTION HERE ?? **todo**
               */
               var dtos = new List<SelectionsPendingDto>();
               

               if(totalcount > 0) {

                    var refdetails = await (from i in _context.OrderItems where data.Select(x => x.OrderItemId).ToList().Contains(i.Id)
                         join o in _context.Orders on i.OrderId equals o.Id 
                         join c in _context.Customers on o.CustomerId equals c.Id 
                         join cat in _context.Categories on i.CategoryId equals cat.Id 
                         select new {CustomerName = c.CustomerName, OrderNo=o.OrderNo, orderitemid=i.Id,
                              CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + cat.Name}
                    ).ToListAsync();
                    
                    foreach(var dto in data) {
                         if (string.IsNullOrEmpty(dto.CustomerName)) dto.CustomerName= 
                              refdetails.Find(x => x.orderitemid == dto.OrderItemId).CustomerName;
                         if (string.IsNullOrEmpty(dto.CategoryName)) dto.CategoryName= 
                              refdetails.Find(x => x.orderitemid == dto.OrderItemId).CategoryRef;

                         dtos.Add(new SelectionsPendingDto {
                              CVRefId = dto.CVRefId,
                              OrderItemId = dto.OrderItemId,
                              OrderNo = dto.OrderNo,
                              CandidateName = dto.CandidateName,
                              ApplicationNo = dto.ApplicationNo,
                              CategoryName = dto.CategoryName,
                              CustomerName = dto.CustomerName,
                              ReferredOn = dto.ReferredOn.Date,
                              RefStatus = dto.RefStatus
                         });
                    }
               }
               
               return new Pagination<SelectionsPendingDto>(specParams.PageIndex, specParams.PageSize, totalcount, dtos);

          }
     }
}         