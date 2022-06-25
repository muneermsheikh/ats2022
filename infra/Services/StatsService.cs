using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.HR;
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
     public class StatsService : IStatsService
     {
          private readonly ATSContext _context;
          private readonly IMapper _mapper;

          private readonly IUnitOfWork _unitOfWork;

          public StatsService(ATSContext context, IMapper mapper, IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
               _mapper = mapper;
               _context = context;
          }


          public async Task<ICollection<OpeningsDto>> GetCurrentOpenings(StatsParams param)
          {
               //first construct queryable by filtering OrderItems.
               var qry = _context.OrderItems
                    .Where(x => x.Status == (int)EnumOrderItemStatus.NotStarted || x.Status == (int)EnumOrderItemStatus.UnderProcess)
                    .Select(x => new { x.Id, x.CategoryId, x.OrderId, x.CompleteBefore, x.Quantity, x.RequireAssess })
                    .AsQueryable();

               if (param.ApplicationNo != 0)
               {
                    //compute professionIds from UserProfessions table
                    var candidateid = await _context.Candidates
                         .Where(x => x.ApplicationNo == param.ApplicationNo)
                         .Select(x => x.Id).FirstOrDefaultAsync();
                    if (candidateid != 0)
                    {
                         var ids = await _context.UserProfessions
                              .Where(x => x.CandidateId == candidateid)
                              .Select(x => x.CategoryId).ToListAsync();
                         qry = qry.Where(x => ids.Contains(x.CategoryId));
                    }
               }
               else if (!string.IsNullOrEmpty(param.ProfessionIds))
               {
                    var ids = param.ProfessionIds.Split(',').Select(x => int.Parse(x));
                    qry = qry.Where(x => ids.Contains(x.CategoryId));
               }

               if (param.OrderNo > 0)
               {
                    var orderid = await _context.Orders.Where(x => x.OrderNo == param.OrderNo)
                         .Select(x => x.Id).FirstOrDefaultAsync();
                    if (orderid > 0)
                    {
                         qry = qry.Where(x => x.OrderId == orderid);
                    }
               }

               var items = await qry.ToListAsync();

               //cosntruct a separate query that would compute selections/rejections made, left join with CVRefs
               //this query wd be searched to updte fields for selections rejectins and balance pending
               var query = _context.CVRefs.Where(x => items.Select(x => x.Id).Contains(x.OrderItemId))
                    .AsQueryable()
                    .GroupBy(x => new { x.OrderItemId })
                    .Select(x => new
                    {
                         OrderItemId = x.Key.OrderItemId,
                         //RefStatusId=x.Key.RefStatusId,
                         //RequireAssess=x.Key.RequireAssess,
                         Selected = x.Count(x => x.RefStatus == (int) EnumCVRefStatus.Selected),
                         Rejected = x.Count(x => x.RefStatus == (int)EnumCVRefStatus.RejectedHighSalaryExpectation ||
                            x.RefStatus == (int) EnumCVRefStatus.RejectedMedicallyUnfit ||
                            x.RefStatus == (int)EnumCVRefStatus.RejectedNoRelevantExp ||
                            x.RefStatus == (int)EnumCVRefStatus.RejectedNotQualified ||
                            x.RefStatus == (int)EnumCVRefStatus.RejectedNotSuitable ||
                            x.RefStatus == (int)EnumCVRefStatus.RejectedOverAge),
                         CanceledAfterSelection = x.Count(x => x.RefStatus == (int)EnumCVRefStatus.NotAvailable),
                         ClBalance = 0,
                         CustomerName = "",
                         OrderNo = 0,
                         OrderDate = "1900-12-12",
                         TargettedDate = "1900-12-12",
                         ProfessionId = 0,
                         ProfessionName = "",
                         Quantity = 0,
                         CompleteBefore = "1900-12-12"
                    });
               var CVRefCounts = await query.ToListAsync();

               var refDtls = await (from r in _context.OrderItems
                                    where items.Select(x => x.Id).Contains(r.Id)
                                    join o in _context.Orders on r.OrderId equals o.Id
                                    join c in _context.Customers on o.CustomerId equals c.Id
                                    join p in _context.Categories on r.CategoryId equals p.Id
                                    select new
                                    {
                                         CustomerName = c.CustomerName,
                                         OrderNo = o.OrderNo
                                         , OrderDate = o.OrderDate
                                         , category = p.Name,
                                         ProfessionId = p.Id,
                                         OrderItemId = r.Id,
                                         Quantity = r.Quantity,
                                         CompleteBefore = r.CompleteBefore,
                                         RequireAssess = r.RequireAssess
                                    }).ToListAsync();

               var temp = await query.ToListAsync();

               var pagedDto = await PagedList<OpeningsDto>.CreateAsync(
                    query.ProjectTo<OpeningsDto>(_mapper.ConfigurationProvider)
                    //query
                    //.AsNoTracking(),1,10)
                    , param.PageIndex, param.PageSize);
               return pagedDto;

               /*
                  // if projectTo DOES NOT WORK, then do flg mapping manually
                  var OpList = new List<OpeningsDto>();
                  var OpListPageHeader = new PaginationHeader();

                  foreach(var item in temp)
                  {
                       var dtl = refDtls.Find(x => x.OrderItemId == item.OrderItemId);
                       if (dtl != null) {
                            OpList.Add(new OpeningsDto(item.OrderItemId, item.Selected, item.Rejected, item.CanceledAfterSelection,
                                 item.ClBalance, dtl.CustomerName, dtl.OrderNo, dtl.OrderDate, 
                                 dtl.CompleteBefore, dtl.ProfessionId, dtl.category, dtl.Quantity)
                            );
                       }
                  }
                  OpList.Skip((param.PageNumber-1)*param.PageSize).Take(param.PageSize);
                  return OpList;
                   */
          }

//HR
//Employee Name  CVs submitted                      CVs approved                        CVs rejected 
//               Within  15 days 30 days >30 days   Within  15 days 30 days >30 days    Within  15 days 30 days >30 days
//Process
//Employee Name  Med Mobilized                      Visa Submitted                      Emig submitted                  Traveled
//               Within  15 days 30 days >30 days   Within  15 days 30 days >30 days    Within  15 days 30 days >30 days

//Admin
//Employee Name  Contract Reviewed                      CVs Forwarded
//               Within  15 days 30 days >30 days       Within  15 days 30 days >30 days

          public async Task<Pagination<EmployeePerformanceDto>> GetEmployeePerformance(EmployeePerfParams empPerf)
          {
               var perfData = await GetEmployeePerfData((int)empPerf.EmployeeId, (DateTime)empPerf.DateFrom, (DateTime)empPerf.DateUpto, empPerf.PerformanceParameters);
               return new Pagination<EmployeePerformanceDto>(empPerf.PageIndex, empPerf.PageSize, perfData.Count(), perfData);
          }

          private async Task<IReadOnlyList<EmployeePerformanceDto>> GetEmployeePerfData(int employeeId, DateTime dateFrom, DateTime dateUpto, 
               ICollection<EnumTaskType> taskTypes)
          {
               var query = await 
                    (from i in _context.TaskItems
                         join t in _context.Tasks on i.ApplicationTaskId equals t.Id
                         where t.TaskOwnerId == employeeId && taskTypes.Contains((EnumTaskType)i.TaskTypeId) &&
                              i.TransactionDate.Date >= dateFrom.Date && i.TransactionDate.Date <= dateUpto.Date
                         orderby i.UserId, i.TaskTypeId
                         select new EmployeePerformanceDto {
                              UserId = i.UserId, TaskType = EnumTaskType.AssignTaskToHRExec,
                              TransactionDate = i.TransactionDate.Date, 
                              Quantity = i.Quantity
                         }
                    ).ToListAsync();
               
               var PerfData = query.GroupBy(x => new {x.UserId, x.TaskType, x.TransactionDate})
                    .Select(g => new EmployeePerformanceDto { 
                         UserId = g.Key.UserId, 
                         TaskType = g.Key.TaskType,
                         TransactionDate = g.Key.TransactionDate,
                         Quantity = g.Sum(x => x.Quantity) }
                    ).ToList();
               return PerfData;
          }

         /*
          public async Task<OrderItemTransDto> GetOrderItemTransactions(StatsTransParams param)
          {
               var query = _context.OrderItems.AsQueryable();
               if (param.OrderItemIds != null && param.OrderItemIds.Count() > 0)
               {
                    query = query.Where(x => param.OrderItemIds.Contains(x.Id));
               }
               if (param.OrderIds != null && param.OrderIds.Count() > 0)
               {
                    query = query.Where(x => param.OrderIds.Contains(x.OrderId));
               }
               if (param.OrderNos != null && param.OrderNos.Count() > 0)
               {
                    query = query.Where(x => param.OrderNos.Contains(x.OrderNo));
               }
               
               query = query.Include(x => x.CVRefs.OrderBy(x => x.Id))
                    .ThenInclude(x => x.Deploys.OrderByDescending(x => x.TransactionDate));

               var qry = await query.ProjectTo<OrderItemTransDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
               return qry;

          }
          */
     }
}