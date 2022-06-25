using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class ChecklistService : IChecklistService
     {
        private readonly ATSContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeService _empService;
        private readonly ICVReviewService _cvReviewService;
        private readonly IUserService _userService;
        private readonly ICommonServices _commonService;
        private readonly IMapper _mapper;
        public ChecklistService(ATSContext context, IUnitOfWork unitOfWork, IUserService userService,
            IEmployeeService empService, ICVReviewService cvReviewService, 
          
            ICommonServices commonService, IMapper mapper)
        {
            _mapper = mapper;
            _commonService = commonService;
            _userService = userService;
            _cvReviewService = cvReviewService;
            _empService = empService;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        private List<string> ChecklistErrors(ChecklistHR checklistHR) {

            var errorStrings = new List<string>();
            foreach(var item in checklistHR.ChecklistHRItems)
            {
                if(item.MandatoryTrue && !item.Accepts) errorStrings.Add(item.Parameter + " not accepted");
            }

            if (checklistHR.Charges >=0 && checklistHR.ChargesAgreed != checklistHR.Charges && !Convert.ToBoolean(checklistHR.ExceptionApproved))
                errorStrings.Add("Charges of " + checklistHR.Charges + " not agreed to by the candidate, and Exceptions not approved");
            return errorStrings;
        }


        private async Task<ChecklistHR> AddChecklistHR(int candidateid, int orderitemid, int employeeid, string loggedinusername)
        {
            
            var itemList = new List<ChecklistHRItem>();
            //populate the checklistHRItem
            var data = await _context.ChecklistHRDatas.OrderBy(x => x.SrNo).ToListAsync();
            
            foreach (var item in data)
            {
                itemList.Add(new ChecklistHRItem(item.SrNo, item.Parameter, item.MandatoryTrue));
            }
            var hrTask = new ChecklistHR(candidateid, orderitemid, employeeid, loggedinusername, System.DateTime.Now, itemList);

            _unitOfWork.Repository<ChecklistHR>().Add(hrTask);

            if (await _unitOfWork.Complete() == 0) throw new Exception("Failed to save the Checklist details");
            return hrTask;
        }

        public async Task<ChecklistHR> GetOrCreateNewChecklistHR(int candidateId, int orderItemId, int LoggedInEmployeeId, string LoggedInUserName)
        {
            var checklist = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId).Include(X => X.ChecklistHRItems).FirstOrDefaultAsync();

            if(checklist != null) return checklist;
            
            checklist = await AddChecklistHR(candidateId, orderItemId, LoggedInEmployeeId,LoggedInUserName);
            
            return checklist;
        }

        public async Task<List<string>> EditChecklistHR(ChecklistHRDto model, LoggedInUserDto loggedInUserDto)
        {
            
            var chklst = _mapper.Map<ChecklistHRDto, ChecklistHR>(model);
            var errorList = ChecklistErrors(chklst);
            if(errorList==null || errorList.Count > 0) return errorList;

            var existing = await GetChecklistHRIfEditable(model, loggedInUserDto);     //returns ChecklistHR
            _context.Entry(existing).CurrentValues.SetValues(model);   //saves only the parent, not children

            //the children 
            //Delete children that exist in existing record, but not in the new model order
            foreach (var existingItem in existing.ChecklistHRItems.ToList())
            {
                if (!model.checklistHRItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.ChecklistHRItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            //children that are not deleted,  are either updated or new ones to be added
            foreach (var modelItem in model.checklistHRItems)
            {
                var existingItem = existing.ChecklistHRItems.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)       // Update child
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(modelItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else            //insert children as new record
                {
                    var newItem = new ChecklistHRItem(modelItem.SrNo, modelItem.Parameter, modelItem.MandatoryTrue);
                    existing.ChecklistHRItems.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }
            }
            _context.Entry(existing).State = EntityState.Modified;

            if(await _context.SaveChangesAsync() ==0) errorList.Add("failed to update the Object");
            
            return errorList;
        }

        public async Task<bool> DeleteChecklistHR(ChecklistHRDto checklistHR, LoggedInUserDto loggedInDto)
        {
            var obj = await _context.ChecklistHRs.FindAsync(checklistHR.Id);
            if (obj==null) return false;

            //check dependencies - CandidteAssessment
            var candidateasessmentObject = await _context.CandidateAssessments.Where(x => x.ChecklistHRId == checklistHR.Id).FirstOrDefaultAsync();
            if(candidateasessmentObject != null) return false;
            
            _unitOfWork.Repository<ChecklistHR>().Delete(obj);

            return await _unitOfWork.Complete() > 0;
        }

        public async Task<int> GetChecklistHRId(int candidateid, int orderitemid)
        {
            var checklist = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateid && x.OrderItemId==orderitemid)
                .Select(x => x.Id).FirstOrDefaultAsync();
            return checklist;
        }
        
        public async Task<ChecklistHRDto> GetChecklistHR(int candidateId, int orderItemId, LoggedInUserDto loggedInUserDto)
        {
            var qry = from checklist in _context.ChecklistHRs.Include(t => t.ChecklistHRItems) 
                join orderitem in _context.OrderItems on checklist.OrderItemId equals orderitem.Id
                join candidateX in _context.Candidates on checklist.CandidateId equals candidateX.Id into candidateG
                    from candidate in candidateG.DefaultIfEmpty()
                join order in _context.Orders on orderitem.OrderId equals order.Id
                join customer in _context.Customers on order.CustomerId equals customer.Id
                join cat in _context.Categories on orderitem.CategoryId equals cat.Id
                where checklist.CandidateId == candidateId && checklist.OrderItemId == orderItemId
                orderby orderitem.SrNo, candidate.ApplicationNo
                select new ChecklistHRDto  {
                    Id = checklist.Id, ApplicationNo = candidate.ApplicationNo, OrderItemId = orderItemId,
                    CandidateName = candidate.FullName, CandidateId = candidate.Id,
                    OrderRef =  order.OrderNo + "-" + orderitem.SrNo + "-" + cat.Name,
                    UserLoggedId = loggedInUserDto == null ? 0 : loggedInUserDto.LoggedInEmployeeId, 
                    CheckedOn = checklist.CheckedOn, 
                    HrExecComments = checklist.HrExecComments,
                    checklistHRItems = checklist.ChecklistHRItems,
                    Charges = orderitem.Charges,
                    ChargesAgreed = (int)checklist.ChargesAgreed,
                    ExceptionApproved = checklist.ExceptionApproved,
                    ExceptionApprovedBy = checklist.ExceptionApprovedBy,
                    ExceptionApprovedOn = checklist.ExceptionApprovedOn
                };
            var lst = await qry.FirstOrDefaultAsync();

            return lst;
        }
        
        private async Task<ChecklistHR> GetChecklistHRIfEditable(ChecklistHRDto model, LoggedInUserDto loggedInDto)
        {
            //if cv already forwarded to Sup, then changes not allowed
            var existing = _context.ChecklistHRs.Where(p => p.Id == model.Id)
                .Include(p => p.ChecklistHRItems)
                .AsNoTracking()
                .SingleOrDefault();

            if (existing == null)
            {
                throw new Exception("Checklist record you want edited does not exist");

                /* if (loggedInDto.LoggedInEmployeeId == 0) loggedInDto.LoggedInEmployeeId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInDto.LoggedInAppUserId);
                var hrexecid = await _context.OrderItems.Where(x => x.Id == model.OrderItemId).Select(x => x.HrExecId).FirstOrDefaultAsync();
                if (model.CandidateId==0 || model.OrderItemId == 0) throw new Exception("Candidate or Order Item detail not provided");
                if (hrexecid != loggedInDto.LoggedInEmployeeId) throw new Exception("The LoggedIn user should be the one tasked to work on this category");
                existing = await CreateChecklistHR(isEdit, model.CandidateId, model.OrderItemId, autoSubmit, loggedInDto);
                */
            }

            //var dto = new ChecklistHRDto();

            var submitted = await _context.CVReviews
                .Where(x => x.CandidateId == model.CandidateId && x.OrderItemId == model.OrderItemId 
                    && x.SubmittedByHRExecOn.Year > 2000)
                .Select(x => x.SubmittedByHRExecOn)
                .FirstOrDefaultAsync();

            if (submitted.Year > 2000)
            {
                throw new System.Exception("This Checklist is referred by the HR Executive on " + submitted.Date + " and cannot be edited now");

            }
            
            
            return existing;
        }

      //master data
        public async Task<ChecklistHRData> AddChecklistHRParameter(string checklistParameter, bool mandatory)
        {
            var srno = await _context.ChecklistHRDatas.MaxAsync(x => x.SrNo) + 1;
            var checklist = new ChecklistHRData(srno, checklistParameter, mandatory);
            _unitOfWork.Repository<ChecklistHRData>().Add(checklist);
            if (await _unitOfWork.Complete() > 0) return checklist;
            return null;
        }

        public async Task<bool> DeleteChecklistHRDataAsync(ChecklistHRData checklistHRData)
        {
            _unitOfWork.Repository<ChecklistHRData>().Delete(checklistHRData);
            return (await _unitOfWork.Complete() > 0);
        }
        public async Task<bool> EditChecklistHRDataAsync(ChecklistHRData checklistHRData)
        {
            _unitOfWork.Repository<ChecklistHRData>().Update(checklistHRData);
            return (await _unitOfWork.Complete() > 0);
        }

        public Task<IReadOnlyList<ChecklistHRData>> GetChecklistHRDataListAsync()
        {
            throw new System.NotImplementedException();
        }

        private async Task<bool> AutoSubmitCVReview(int candidateid, int orderitemid, int checklisthrid, LoggedInUserDto loggedInUserDto)
        {
            var hrexectask = await _context.Tasks.Where(x => x.OrderItemId == orderitemid &&
                x.AssignedToId == loggedInUserDto.LoggedInEmployeeId && 
                x.TaskTypeId == (int)EnumTaskType.AssignTaskToHRExec).FirstOrDefaultAsync();
            var orderitem = await _context.OrderItems.Where(x => x.Id == orderitemid)
                .Select(x => new { x.HrSupId, x.Charges, x.NoReviewBySupervisor }).FirstOrDefaultAsync();

            var cv = new CVReviewBySupDto
            {
                    ChecklistHRId = checklisthrid,
                    //TaskId = hrexectask.Id,
                    enumTaskType = core.Entities.Orders.EnumTaskType.SubmitCVToHRSupForReview,
                    TaskOwnerId = hrexectask.TaskOwnerId,
                    AssignedToId = (int)orderitem.HrSupId,
                    NoReviewBySupervisor = orderitem.NoReviewBySupervisor,
                    Charges = orderitem.Charges,
                    OrderItemId = orderitemid,
                    CandidateId = candidateid
            };

            var cvdto = new List<CVReviewBySupDto>();
            cvdto.Add(cv);
            var msgs = await _cvReviewService.CVReviewByHRSup(loggedInUserDto, cvdto);
            if (msgs == null || msgs.Count == 0) throw new Exception("Checklist for HR created, but failed to submit the CV to the Supervisor");
            
            return true;
        }

         
     }
}