using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.HR;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class InterviewService : IInterviewService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IUserService _userService;
          private readonly IMapper _mapper;
          private readonly ICollection<SelectionStatus> selStatuses;
          public InterviewService(ATSContext context, IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
          {
               _mapper = mapper;
               _userService = userService;
               _unitOfWork = unitOfWork;
               _context = context;
          }

          private async Task<ICollection<SelectionStatus>> GetSelStatuses() {
               return await _context.SelectionStatuses.OrderBy(x => x.Status).ToListAsync();
          }

          private async Task<Interview> CreateInterviewObject(InterviewToAddDto dto) {
               var qry = from o in _context.Orders
                    where o.Id == dto.OrderId
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new
                    {
                         CustomerId = o.CustomerId,
                         CustomerName = c.CustomerName,
                         OrderNo = o.OrderNo,
                         OrderDate = o.OrderDate
                    };

               var qryObj = await qry.FirstOrDefaultAsync();

               var intervwItems = new List<InterviewItem>();
               var orderItems = await _context.OrderItems.Where(x => x.OrderId == dto.OrderId)
                   .Select(x => new { x.Id, x.CategoryId, x.Quantity, x.SourceFrom, x.SrNo })
                   .OrderBy(x => x.SrNo)
                   .ToListAsync();
               if (orderItems == null || orderItems?.Count() == 0) throw new Exception("Invalid order Id");
               foreach (var i in orderItems)
               {
                    intervwItems.Add(new InterviewItem(i.Id, i.CategoryId, dto.InterviewDateFrom.Date, dto.InterviewDateUpto, dto.InterviewMode, dto.CustomerRepresentative));
               }
               var intervw = new Interview(dto.OrderId, qryObj.OrderNo, qryObj.OrderDate.Date, qryObj.CustomerId, qryObj.CustomerName, dto.InterviewVenue,
                   dto.InterviewDateFrom.Date, dto.InterviewDateUpto.Date, dto.InterviewLeaderId,
                   dto.CustomerRepresentative, dto.InterviewMode, intervwItems);

               return intervw;
          }
          public async Task<Interview> AddInterview(InterviewToAddDto dto)
          {
               var intervw = await CreateInterviewObject(dto);
               if (intervw==null) return null;

               _unitOfWork.Repository<Interview>().Add(intervw);

               if (await _unitOfWork.Complete() > 0) return intervw;

               return null;
          }

          public async Task<Interview> AddInterviewCategories(int orderId)
          {
               var intervw = await _context.Interviews.Where(x => x.OrderId == orderId).Include(x => x.InterviewItems).FirstOrDefaultAsync();
               var existingItems = intervw.InterviewItems;
               var orderItems = await _context.OrderItems.Where(x => x.Id == orderId)
                   .Select(x => new { x.Id, x.CategoryId, x.Quantity, x.SourceFrom, x.SrNo })
                   .OrderBy(x => x.SrNo)
                   .ToListAsync();
               var newItems = orderItems.Where(x => !existingItems.Select(x => x.OrderItemId).ToList().Contains(x.Id)).ToList();
               var ItemsToDelete = existingItems.Where(x => !orderItems.Select(x => x.Id).ToList().Contains(x.OrderItemId)).ToList();

               //delete from existing items those orderitemIds that DO NOT exist in orderitems, i.e. itemstodelete.OrderItem
               foreach (var item in existingItems)
               {
                    foreach (var it in ItemsToDelete)
                    {
                         if (item.OrderItemId == item.Id)
                         {
                              _context.Entry(item).State = EntityState.Deleted;
                              break;
                         }
                    }
               }

               if (newItems != null)
               {
                    foreach (var item in newItems)
                    {
                         existingItems.Add(new InterviewItem(item.Id, item.CategoryId, intervw.InterviewDateFrom.Date, intervw.InterviewDateUpto, intervw.InterviewMode, intervw.InterviewerName));
                    }
                    _context.Entry(existingItems).State = EntityState.Modified;
               }

               if (await _context.SaveChangesAsync() > 0)
               {
                    return intervw;
               }
               else
               {
                    return null;
               }
          }

          public async Task<bool> DeleteInterview(int interviewId)
          {
               var interviewItem = await _context.InterviewItems.FindAsync(interviewId);

               if (interviewItem == null) return false;

               _context.Entry(interviewItem).State = EntityState.Deleted;

               return (await _context.SaveChangesAsync() > 0);

          }

          public async Task<bool> DeleteInterviewItem(InterviewItem interviewItem)
          {
               var item = await _context.InterviewItems.FindAsync(interviewItem.Id);
               if (item == null) return false;

               _context.Entry(item).State = EntityState.Deleted;

               return await _context.SaveChangesAsync() > 0;
          }

          public async Task<Interview> EditInterview(Interview modelObj)
          {
               var existingObj = await _context.Interviews
                   .Where(x => x.Id == modelObj.Id)
                   .Include(x => x.InterviewItems)
                   .FirstOrDefaultAsync();
               if (existingObj == null) return null;

               _context.Entry(existingObj).CurrentValues.SetValues(modelObj);

               //delete items that exist in existingObj but not in modelObj
               foreach (var existingItem in existingObj.InterviewItems.ToList())
               {
                    if (!modelObj.InterviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.InterviewItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var item in modelObj.InterviewItems)
               {
                    var existingItem = existingObj.InterviewItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       //update child
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(item);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else      //insert new record
                    {
                         var newItem = new InterviewItem(item.OrderItemId, item.CategoryId,
                             modelObj.InterviewDateFrom.Date, item.InterviewDateUpto, item.InterviewMode, item.InterviewerName);
                         existingObj.InterviewItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }

               _context.Entry(existingObj).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0)
               {
                    return existingObj;
               }

               return null;
          }

          public async Task<Interview> GetInterviewById(int Id)
          {
               return await _context.Interviews
                    .Where(x => x.Id == Id)
                    .Include(x => x.InterviewItems)
                    .ThenInclude(x => x.InterviewItemCandidates)
                    .FirstOrDefaultAsync();
          }

          //GetOrCreateInterviewByOrderId
          //if the Interview data exists in DB, returns the same
          //if it does not exist, creates an Object and returns it
          public async Task<Interview> GetOrCreateInterviewByOrderId(int OrderId)
          {
               var interview = await _context.Interviews
                    .Where(x => x.OrderId == OrderId)
                    .Include(x => x.InterviewItems)
                    .FirstOrDefaultAsync();
               
               if (interview==null) {
                    var dto = new InterviewToAddDto();
                    dto.OrderId = OrderId;
                    interview = await CreateInterviewObject(dto);
               }

               return interview;
          }
          
          
          public async Task<PagedList<InterviewBriefDto>> GetInterviews(InterviewSpecParams specParams)
          {
               int totalItems = 0;
               var qry = _context.Interviews.AsQueryable();

               if(specParams.InterviewId.HasValue) {
                    qry = qry.Where(x => x.Id == specParams.InterviewId);
               } else if (specParams.InterviewItemId.HasValue) {
                    qry = qry.Where(x => x.InterviewItems.Select(x => x.Id == specParams.InterviewItemId).FirstOrDefault());
               } else if (specParams.OrderItemId.HasValue) {
                    qry = qry.Where(x => x.InterviewItems.Select(x => x.OrderItemId == specParams.OrderItemId).FirstOrDefault());
               //} else if (specParams.ApplicationNo.HasValue) {
               //     qry = qry.Where(x => x.InterviewItems.Select(x => x.Id == specParams.InterviewItemId).FirstOrDefault());
               } else if (!string.IsNullOrEmpty(specParams.InterviewStatus)) {
                    qry = qry.Where(x => x.InterviewStatus.ToLower() == specParams.InterviewStatus.ToLower());
               } else if (specParams.CategoryId.HasValue) {
                    qry = qry.Where(x => x.InterviewItems.Select(x => x.CategoryId == specParams.CategoryId).FirstOrDefault());
               } 

               if(specParams.IncludeItems) qry = qry.Include(x => x.InterviewItems);

               totalItems = await qry.CountAsync();

               if (totalItems==0) return null;
               
               var data = await qry.ToListAsync();

               var paged = await PagedList<InterviewBriefDto>.CreateAsync(
                    qry.ProjectTo<InterviewBriefDto>(_mapper.ConfigurationProvider)
                    ,specParams.PageIndex, specParams.PageSize);
               return paged;
          }

          public async Task<ICollection<Interview>> GetInterviewsWithItems(string interviewStatus)
          {
               var interviews = await _context.Interviews
                   .Where(x => x.InterviewStatus.ToLower() == interviewStatus.ToLower())
                   .Include(x => x.InterviewItems)
                   .OrderBy(x => new { x.InterviewStatus, x.InterviewDateFrom.Date })
                   .ToListAsync();
               return interviews;
          }

          public async Task<InterviewItem> EditInterviewItem(InterviewItem interviewItem)
          {
               var existingItem = await _context.InterviewItems.FindAsync(interviewItem.Id);
               _context.Entry(existingItem).CurrentValues.SetValues(interviewItem);

               _context.Entry(existingItem).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0) return existingItem;

               return null;
          }

          public async Task<ICollection<InterviewItemCandidate>> AddCandidatesToInterviewItem(int interviewItemId, DateTime scheduledTime,
              int durationInMinutes, string interviewMode, List<int> CandidateIds)
          {
               const int breakInMinutes = 5;
               var startUpTime = scheduledTime;

               var interviewitem = await _context.InterviewItems.Where(x => x.Id == interviewItemId)
                    .Select(x => new{x.CategoryId, x.InterviewDateFrom, x.InterviewDateUpto}).FirstOrDefaultAsync();

               //select those candidateIds whose Professions match the categoryId of interviewItemId
               //this will exclude all those candidateIds whose professions do not match the categoryId of itneviewItemId
               var CandidateIdsFiltered = await _context.UserProfessions
                   .Where(x => x.CategoryId == interviewitem.CategoryId && CandidateIds.Contains(x.CandidateId))
                   .Select(x => x.CandidateId).ToListAsync();

               if (CandidateIdsFiltered == null || CandidateIdsFiltered.Count() == 0) throw new Exception("Candidates selected do not possess skills as requierd by the Interview Category");

               //check if interview scheduled dates align with interview item interview dates
               if (scheduledTime < interviewitem.InterviewDateFrom || scheduledTime > interviewitem.InterviewDateUpto)
                    throw new Exception("Interview scheduled for the candidate(s) is beyond the interview dates scheduled for the category." +
                         "Interviews are scheduled from " + interviewitem.InterviewDateFrom.Date + " to " + interviewitem.InterviewDateUpto.Date + 
                         " while the candiadtes are scheduled to be itnerviewed at " + scheduledTime);
               
               var candidates = await _context.Candidates.Where(x => CandidateIdsFiltered.Contains(x.Id))
                   .Select(x => new { x.ApplicationNo, x.PpNo, x.FullName, x.Id })
                   .ToListAsync();

               if (candidates == null) throw new Exception("Candidates selected do not possess skills as required by the interview category");

               var lst = new List<InterviewItemCandidate>();

               foreach (var item in candidates)
               {
                    var newCandidate = new InterviewItemCandidate(interviewItemId, item.Id, item.ApplicationNo, item.PpNo, item.FullName, startUpTime,
                        scheduledTime.AddMinutes(durationInMinutes), interviewMode);
                    _context.InterviewItemCandidates.Add(newCandidate);
                    startUpTime.AddMinutes(durationInMinutes + breakInMinutes);
                    lst.Add(newCandidate);
               }

               if (await _context.SaveChangesAsync() > 0) return lst;

               return null;
          }

          public async Task<bool> DeleteFromInterviewItemCandidates(List<int> interviewItemCandidateIds)
          {
               var objs = await _context.InterviewItemCandidates.Where(x => interviewItemCandidateIds.Contains(x.Id)).ToListAsync();

               foreach (var obj in objs)
               {
                    _unitOfWork.Repository<InterviewItemCandidate>().Delete(obj);
               }

               return await _unitOfWork.Complete() > 0;

          }

       
          public async Task<ICollection<InterviewItemCandidateDto>> GetInterviewItemAndAttendanceOfInterviewItem(int interviewItemId)
          {
               var statusNames = await GetSelStatuses();

               var qryCands = await (from itItem in _context.InterviewItems where itItem.Id == interviewItemId
                    join cat in _context.Categories on itItem.CategoryId equals cat.Id
                    join cand in _context.InterviewItemCandidates on itItem.Id equals cand.InterviewItemId into g 
                    from cv in g.DefaultIfEmpty()
                    select new InterviewItemCandidateDto { 
                         SelectionStatusName =  (int)cv.SelectionStatusId == 0 ? "" : statusNames.Where(x => x.Id == cv.SelectionStatusId).Select(x => x.Status).FirstOrDefault(),
                         InterviewItemId = interviewItemId, 
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.CandidateName, 
                         InterviewedDateTime = (DateTime)cv.InterviewedDateTime, 
                         InterviewMode = cv.InterviewMode })
               .ToListAsync();

               return qryCands;

          }

          public async Task<InterviewBriefDto> GetInterviewAttendanceOfAProject(int orderId, List<int> attendanceStatusIds)
          {
               var qryItems = await (from it in _context.Interviews
                    where it.OrderId == orderId
                    join itItem in _context.InterviewItems on it.Id equals itItem.InterviewId
                    join cat in _context.Categories on itItem.CategoryId equals cat.Id
                    join o in _context.Orders on it.OrderId equals o.Id
                    join cand in _context.InterviewItemCandidates on itItem.Id equals cand.InterviewItemId
                    where attendanceStatusIds.Contains((int)cand.AttendanceStatusId)
                    join status in _context.InterviewAttendancesStatus on cand.AttendanceStatusId equals status.Id
                    group status.Status by new
                    {
                    it.Id,
                    itItem.OrderItemId,
                    itItem.CategoryId,
                    cat.Name,
                    itItem.InterviewMode,
                    cand.ApplicationNo,
                    cand.CandidateName,
                    cand.InterviewedDateTime,
                    status.Status
                    } into g
                    select new // InterviewItemDto
                    {
                         InterviewId = g.Key.Id,
                         OrderItemId = g.Key.OrderItemId,
                         CategoryId = g.Key.CategoryId,
                         CategoryName = g.Key.Name,
                         InterviewMode = g.Key.InterviewMode,  
                         ApplicationNo = g.Key.ApplicationNo,
                         CandidateName = g.Key.CandidateName,
                         InterviewedOn = g.Key.InterviewedDateTime,
                         AttendanceStatus = g.Key.Status
                    })
               .ToListAsync();

               if (qryItems == null) return null;

               /*
               var intvItems = new List<InterviewItemDto>();
               foreach (var q in qryItems)
               {
                    //(int interviewId, int orderItemId, int categoryId, string categoryName, DateTime interviewDate, int applicationNo, string candidateName, 
                    //\string interviewMode, string attendanceStatus, string remarks
                    intvItems.Add(new InterviewItemDto(q.InterviewId, q.OrderItemId, q.CategoryId, q.CategoryName, q.InterviewedOn ?? DateTime.Now,
                        q.ApplicationNo, q.CandidateName, q.InterviewMode, q.AttendanceStatus));
               }
               */

               var intervw = await _context.Interviews.Where(x => x.OrderId == orderId).FirstOrDefaultAsync();

               var dtoToReturn = new InterviewBriefDto(intervw.CustomerName, intervw.InterviewVenue, intervw.OrderId, intervw.OrderNo,
                   intervw.OrderDate);
               
               return dtoToReturn;

          }

          public async Task<bool> UpdateCandidateScheduledAttendanceStatus(int interviewItemCandidateId, int attendanceStatusId)
          {
               var item = await _context.InterviewItemCandidates.FindAsync(interviewItemCandidateId);
               if (item == null) return false;

               item.AttendanceStatusId = attendanceStatusId;

               _unitOfWork.Repository<InterviewItemCandidate>().Update(item);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> RegisterCandidateReportedForInterview(int interviewItemCandidateId, DateTime reportedAt)
          {
               var item = await _context.InterviewItemCandidates.FindAsync(interviewItemCandidateId);
               if (item == null) return false;
               
               if (Convert.ToDateTime(item.InterviewedDateTime).Year > 2000)
                    throw new Exception("the candidate has already been interviewed on " + item.InterviewedDateTime);
               if (item.SelectionStatusId > 0) throw new Exception("candidate has bene interviewed and itnerview decision made");
               
               if (item.ScheduledFrom.Day != reportedAt.Day) throw new Exception("candidate was scheduled to report on " + item.ScheduledFrom.Date);

               item.ReportedDateTime = reportedAt;

               _unitOfWork.Repository<InterviewItemCandidate>().Update(item);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> RegisterCandidateAsInterviewed(int candidateInterviewedId, string interviewMode, DateTime interviewedAt)
          {
               var item = await _context.InterviewItemCandidates.FindAsync(candidateInterviewedId);
               if (item == null) return false;
               if (item.InterviewedDateTime != null)
                    throw new Exception("the candidate has already been interviewed on " + item.InterviewedDateTime);
               if (item.SelectionStatusId > 0) throw new Exception("candidate has bene interviewed and itnerview decision made");
               if(item.ScheduledFrom.Date != interviewedAt.Date) throw new Exception("Interviewed Date not as scheduled");

               if (item.ReportedDateTime == null) item.ReportedDateTime = interviewedAt;
               
               item.InterviewMode = interviewMode;
               item.InterviewedDateTime = interviewedAt;
               item.AttendanceStatusId = 6;        //attendance interview status - ATTENDED

               _unitOfWork.Repository<InterviewItemCandidate>().Update(item);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> RegisterCandidateInterviewedWithResult(
               int candidateInterviewedId, string interviewMode, DateTime interviewedAt, int selectionstatusId)
          {
               var item = await _context.InterviewItemCandidates.FindAsync(candidateInterviewedId);
               if (item == null) throw new Exception("Invalid interview item candidate Id");

               if (item.ReportedDateTime  == null) item.ReportedDateTime = interviewedAt;
               item.InterviewMode = interviewMode;
               item.InterviewedDateTime = interviewedAt;
               item.AttendanceStatusId = 6;        //attendance interview status - ATTENDED
               item.SelectionStatusId = selectionstatusId;

               _unitOfWork.Repository<InterviewItemCandidate>().Update(item);

               return await _unitOfWork.Complete() > 0;
          }
          public async Task<ICollection<Interview>> GetOpenInterviews()
          {
               var interviews = await _context.Interviews.Where(x => x.InterviewStatus.ToLower() == "open")
                   .Include(x => x.InterviewItems)
                   .OrderByDescending(x => x.OrderNo)
                   .ToListAsync();
               return interviews;

          }

          public async Task<ICollection<CandidateBriefDto>> GetCandidatesMatchingInterviewCategory(InterviewSpecParams iParams)
          {
               var DisAllowedStatus = new List<string>();
               DisAllowedStatus.Add("Referred");
               DisAllowedStatus.Add("Traveled");
               DisAllowedStatus.Add("Not Available");
               
               var interviewCategoryId = await _context.InterviewItems.Where(x => x.Id == iParams.InterviewItemId)
                    .Select(x => x.CategoryId).FirstOrDefaultAsync();
               
               var qry = from up in _context.UserProfessions where  up.CategoryId == interviewCategoryId
                    join cand in _context.Candidates on up.CandidateId equals cand.Id 
                    where !DisAllowedStatus.Contains(cand.CandidateStatus)
                    join icand in _context.InterviewItemCandidates on cand.Id equals icand.CandidateId 
                         where !DisAllowedStatus.Contains(cand.CandidateStatus)
                    select cand.Id;
               var candidateIds = await qry.ToListAsync();
               

               /* var candidateIds = await _context.UserProfessions.Where(x => x.CategoryId == interviewCategoryId)
                    .Select(x => x.CandidateId).ToListAsync();
               var excludeCandidateId = await _context.InterviewItemCandidates
                    .Where(x => x.InterviewItemId==iParams.InterviewItemId && AllowedCandidateStatus.Contains(x.CandidateId))
                    .Select(x => x.CandidateId).ToListAsync();
               */
               var cands = await _context.Candidates.Where(x => candidateIds.Contains(x.Id) 
                    //&& (AllowedCandidateStatus.Contains((int)x.CandidateStatus)) 
                    )
                    .Include(x => x.UserProfessions)
                    //.Include(x => x.UserPhones)
                    .OrderBy(x => x.ApplicationNo) 
                    .ToListAsync();
               
               
               /*
                var specParams = new CandidateSpecParams { ProfessionId = interviewCategoryId, IncludeUserProfessions = true, IncludeUserPhones = true };
                var specCtParams = new CandidateForCountSpecs(specParams);
                var specs = new CandidateSpecs(specParams);
                var ct = await _unitOfWork.Repository<Candidate>().CountAsync(specs);
                if (ct == 0) return null;

                var cands = await _unitOfWork.Repository<Candidate>().ListAsync(specs);
               */

                var dto = new List<CandidateBriefDto>();
                foreach(var cand in cands)
                {
                    if (cand.UserPhones == null) cand.UserPhones = new List<UserPhone>(); 
                    if (cand.UserProfessions == null) cand.UserProfessions = new List<UserProfession>();
                    dto.Add(new CandidateBriefDto{Id = cand.Id, FullName = cand.FullName,
                        ApplicationNo = cand.ApplicationNo, 
                        //PassportNo = cand.UserPassports?.Where(x => x.IsValid).Select(x => x.PassportNo).FirstOrDefault(),
                        UserProfessions = cand.UserProfessions, ReferredById = cand.ReferredBy,
                        ReferredByName= await _userService.GetCustomerNameFromCustomerId(cand.ReferredBy)
                        });
                }
                
                return dto; //new Pagination<Candidate>(1, 20, ct, dto);

          }

     }
}