using System.Threading.Tasks;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using core.Entities.Process;
using System.Collections.Generic;
using System;
using core.Entities.HR;
using core.Entities.Orders;

namespace infra.Services
{
     public class CommonServices : ICommonServices
     {
          private readonly ATSContext _context;
          public CommonServices(ATSContext context)
          {
               _context = context;
          }

          public async Task<string> CategoryNameFromCategoryId(int categoryId)
          {
               return await _context.Categories.Where(x => x.Id == categoryId).Select(x => x.Name).FirstOrDefaultAsync();
          }
          public async Task<string> CategoryRefFromOrderItemId(int OrderItemId)
          {
               var CatRef = await (from i in _context.OrderItems where i.Id == OrderItemId
                    join o in _context.Orders on i.OrderId equals o.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new {CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + cat.Name + " for " + c.CustomerName })
                    .FirstOrDefaultAsync();

               return CatRef.CategoryRef;
          }
          
          public async Task<string> CustomerNameFromCustomerId(int customerId)
          {
               var qry = await (from c in _context.Customers where c.Id == customerId
                    select c.CustomerName)
                    .FirstOrDefaultAsync();
               return qry;
          }

          public async Task<string> GetEmployeeNameFromEmployeeId(int id)
          {
               return await _context.Employees.Where(x => x.Id == id).Select(x => x.FirstName + " " + x.FamilyName).FirstOrDefaultAsync();
          }
          public async Task<string> GetEmployeePositionFromEmployeeId(int employeeId)
          {
                return await _context.Employees.Where(x => x.Id == employeeId).Select(x => x.Position).FirstOrDefaultAsync();
          }

          public async Task<string> CustomerNameFromOrderDetailId(int orderDetailId)
          {
               var qry = await (from r in _context.OrderItems where r.Id == orderDetailId
                    join o in _context.Orders on r.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select c.CustomerName)
                    .FirstOrDefaultAsync();
               return qry;
          }

          public async Task<CommonDataDto> CommonDataFromCVRefId(int cvrefid)
          {
               var qry = await (from r in _context.CVRefs where r.Id == cvrefid
                    join i in _context.OrderItems on r.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    select (new CommonDataDto {
                         ApplicationNo = cand.ApplicationNo,
                         CandidateName = cand.FullName,
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         OrderNo = ordr.OrderNo,
                         HRExecId = i.HrExecId == null ? 0 : (int)i.HrExecId,
                         HRSupId = i.HrSupId == null ? 0 : (int)i.HrSupId,
                         HRMId = i.HrmId == null ? 0 : (int)i.HrmId,
                         MedicalProcessInchargeEmpId = ordr.MedicalProcessInchargeEmpId == null ? 0 : (int) ordr.MedicalProcessInchargeEmpId,
                         VisaProcessInchargeEmpId = ordr.VisaProcessInchargeEmpId == null ? 0 : (int)ordr.VisaProcessInchargeEmpId,
                         TravelProcessInchargeId = ordr.TravelProcessInchargeId == null ? 0 : (int)ordr.TravelProcessInchargeId
                    })).FirstOrDefaultAsync();

               return qry; 
          }

          public async Task<CommonDataDto> CommonDataFromOrderDetailIdAndCandidateId(int CVReviewId)
          {
               var qry = await (from r in _context.CVReviews where r.Id == CVReviewId
                    join i in _context.OrderItems on r.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    orderby r.SubmittedByHRExecOn
                    select (new CommonDataDto{
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         CategoryId = i.CategoryId,
                         OrderId = ordr.Id,
                         OrderItemId = i.Id,
                         OrderItemSrNo = i.SrNo,
                         OrderNo = ordr.OrderNo,
                         NoReviewBySupervisor = i.NoReviewBySupervisor,
                         HRSupId = (int)i.HrSupId,
                         HRMId = (int)i.HrmId, 
                         HRExecId = (int)i.HrExecId,
                         ApplicationNo = cand.ApplicationNo,
                         CandidateName = cand.FullName,
                         CandidateId = cand.Id,
                         Ecnr = cand.Ecnr
                    })).FirstOrDefaultAsync();
               
               /* return new CommonDataDto{
                    CustomerName = qry.CustomerName, CategoryName = qry.CategoryName, OrderNo = qry.OrderNo,
                    ApplicationNo = qry.ApplicationNo, CandidateName = qry.FullName, CategoryId = qry.CategoryId,
                    NoReviewBySupervisor = qry.NoReviewBySupervisor, HRSupId = (int)qry.HRSupId, 
                    HRMId = (int)qry.HRMId,
                    HRExecId=(int)qry.HRExecId, OrderId = qry.OrderId, OrderItemId = qry.OrderItemId, 
                    OrderItemSrNo = qry.SrNo, CandidateId = qry.CandidateId
               };
               */
               return qry;
          }


          public async Task<ICollection<SelectionDecisionToRegisterDto>> PopulateSelectionDecisionsToRegisterDto(ICollection<SelectionDecisionToRegisterDto> dto)
          {
               var data = dto.Select(x => new {x.SelectionStatusId, x.DecisionDate}).FirstOrDefault();
               int selectionStatusId = data.SelectionStatusId;
               DateTime dt = data.DecisionDate.Date;
               /*join d in dto on cvref.Id equals d.CVRefId
                    join i in _context.OrderItems on cvref.OrderItemId equals i.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on cvref.CandidateId equals cand.Id
               */
               var dtoCVRefIds = dto.Select(x => x.CVRefId).ToList();
               var qry = from c in _context.CVRefs where dtoCVRefIds.Contains(c.Id)
                    select new SelectionDecisionToRegisterDto {
                         CVRefId = c.Id,
                         OrderItemId = c.OrderItemId, 
                         OrderId = c.OrderId,
                         OrderNo = c.OrderNo,
                         CategoryId = c.CategoryId,
                         CategoryName = c.CategoryName,
                         CandidateId = c.CandidateId,
                         ApplicationNo = c.ApplicationNo,
                         CandidateName = c.CandidateName,
                         SelectionStatusId= selectionStatusId,
                         DecisionDate= dt
                    };
               return await qry.ToListAsync();               
          }

          public async Task<Employment> PopulateEmploymentFromCVRefId(int cvrefid, int salary, int charges, DateTime selectedOn)
          {
               var c = await _context.CVRefs.FindAsync(cvrefid);
               var item = await _context.OrderItems.Where(x => x.Id == c.OrderItemId)
                    .Include(x => x.Remuneration)
               .FirstOrDefaultAsync();
               var customerid = await _context.Orders.Where(x => x.Id == item.OrderId).Select(x => x.CustomerId).FirstOrDefaultAsync();
               var emp = new Employment {
                    CVRefId = c.Id, OrderItemId = c.OrderItemId, OrderId = item.OrderId, OrderNo = c.OrderNo,
                    CustomerName = c.CustomerName,  CategoryId = c.CategoryId, CategoryName = c.CategoryName,
                    CandidateId = c.CandidateId, ApplicationNo = c.ApplicationNo, CandidateName = c.CandidateName,
                    SelectedOn = selectedOn, Charges = charges, Salary = salary, CustomerId = customerid
               };
               
               var r = item.Remuneration;
               if (r != null) {
                    emp.SalaryCurrency = r.SalaryCurrency;
                    emp.ContractPeriodInMonths = r.ContractPeriodInMonths;
                    emp.HousingProvidedFree = r.HousingProvidedFree;
                    emp.HousingAllowance = r.HousingAllowance;
                    emp.FoodProvidedFree = r.FoodProvidedFree;
                    emp.FoodAllowance = r.FoodAllowance;
                    emp.TransportProvidedFree = r.TransportProvidedFree;
                    emp.TransportAllowance = r.TransportAllowance;
                    emp.OtherAllowance = r.OtherAllowance;
                    emp.LeavePerYearInDays = r.LeavePerYearInDays;
                    emp.LeaveAirfareEntitlementAfterMonths = r.LeaveAirfareEntitlementAfterMonths;
               }               
               return emp;
          }

          public async Task<CommonDataDto> PendingDeployments()
          {
               /* var tempQury =  from SPListItem customers in _customerList
               group customers by customers["ContractNumber"] into gby 
               select gby.First(); */

               var tempQuery =  from d in _context.Deploys
                    group d by d.CVRefId into dTop 
                    orderby dTop.Key descending
                    select new {
                         Key = dTop.First(),
                         Status = dTop.First()
                    };

               var qry = await (from r in _context.CVRefs 
                    join d in tempQuery on r.Id equals d.Key.CVRefId
                    join i in _context.OrderItems on r.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    select (new CommonDataDto {
                         ApplicationNo = cand.ApplicationNo,
                         CandidateName = cand.FullName,
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         OrderNo = ordr.OrderNo,
                         OrderItemSrNo = i.SrNo,
                         DeployStageId = d.Status.StageId,
                         RequireInternalReview = i.RequireInternalReview,
                         HRSupId = (int)i.HrSupId,
                         HRMId = (int)i.HrmId
                    })).FirstOrDefaultAsync();

               return qry;
               
          }

          public async Task<string> DeploymentStageNameFromStageId(int stageId)
          {
               return await _context.DeployStatus.Where(x => x.StageId == stageId).Select(x => x.StatusName).FirstOrDefaultAsync();
          }

          public async Task<CustomerBriefDto> CustomerBriefDetailsFromCustomerId(int customerId)
          {
               var dto = await _context.Customers.Where(x => x.Id == customerId)
                    .FirstOrDefaultAsync();
               return new CustomerBriefDto{CustomerName = dto.CustomerName, City = dto.City, KnownAs = dto.KnownAs};
          }

          public async Task<OrderAssignmentDto> GetOrderAssignmentDto(int orderId)
          {
               var qry = await (from o in _context.Orders
                where o.Id == orderId
                join c in _context.Customers on o.CustomerId equals c.Id
                join e in _context.Employees on o.ProjectManagerId equals e.Id
                select new
                {
                    OrderId = o.Id, OrderNo = o.OrderNo, OrderDate = o.OrderDate.Date, CustomerName = c.CustomerName, 
                    EmployeeName = e.FirstName + " " + e.FamilyName, Position = e.Position, email = e.Email, 
                    ProjManagerAppUserId = e.AppUserId, ProjectManagerId = o.ProjectManagerId, EmployeeId = e.Id
                }).FirstOrDefaultAsync();

               if (qry == null) throw new Exception("failed to retrieve order particulars - check the project manager is assigned");

               return new OrderAssignmentDto {
                    OrderId = qry.OrderId, OrderNo = qry.OrderNo, OrderDate = qry.OrderDate, CustomerName = qry.CustomerName,
                    ProjectManagerId = qry.EmployeeId, ProjectManagerPosition = qry.Position
                    //, ProjectManagerEmail = qry.email, ProjectManagerAppUserId = qry.ProjManagerAppUserId
               };
          }

          public async Task<CommonDataDto> CommonDataFromOrderDetailIdAndCandidateId(int OrderItemId, int candidateId)
          {
               var qry2 = await _context.Candidates.Where(x => x.Id == candidateId)
                    .Select(x => new {x.ApplicationNo, x.FullName}).FirstOrDefaultAsync();
               if (qry2==null) return null;
               var qry = await (from i in _context.OrderItems where i.Id == OrderItemId 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    select (new {
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         CategoryId = i.CategoryId,
                         OrderId = ordr.Id,
                         OrderItemId= i.Id,
                         OrderNo = ordr.OrderNo,
                         SrNo = i.SrNo,
                         NoReviewBySupervisor = i.NoReviewBySupervisor,
                         HRSupId = i.HrSupId,
                         HRMId = i.HrmId, HRExecId = i.HrExecId
                    })).FirstOrDefaultAsync();
               if (qry==null) return null;
               return new CommonDataDto{
                    CustomerName = qry.CustomerName, CategoryName = qry.CategoryName, OrderNo = qry.OrderNo,
                    ApplicationNo = qry2.ApplicationNo, CandidateName = qry2.FullName, CategoryId = qry.CategoryId,
                    NoReviewBySupervisor = qry.NoReviewBySupervisor, HRSupId = (int)qry.HRSupId, HRMId = (int)qry.HRMId,
                    HRExecId=(int)qry.HRExecId, OrderId = qry.OrderId, OrderItemId = qry.OrderItemId, 
                    OrderItemSrNo = qry.SrNo, CandidateId = candidateId
               };
               
          }

          public async Task<CommonDataForCVRefDto> CommonDataForCVRefFromOrderItemAndCandidateId(int OrderItemId, int candidateId)
          {
               var qry2 = await _context.Candidates.Where(x => x.Id == candidateId)
                    .Select(x => new {x.ApplicationNo, x.PpNo, x.FullName}).FirstOrDefaultAsync();
               if (qry2==null) return null;
               var qry = await (from i in _context.OrderItems where i.Id == OrderItemId 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    select (new {
                         CustomerName = c.CustomerName, 
                         CategoryId = cat.Id,
                         CategoryName = cat.Name, 
                         OrderNo = ordr.OrderNo,
                         SrNo = i.SrNo,
                         OrderId = ordr.Id
                    })).FirstOrDefaultAsync();
               if (qry==null) return null;
               var cvrvwData = await _context.CVReviews.Where(
                    x => x.CandidateId == candidateId && x.OrderItemId == OrderItemId)
                    .Select(x => new{x.DocControllerAdminTaskId, x.Id}).FirstOrDefaultAsync();
               return new CommonDataForCVRefDto{
                    CustomerName = qry.CustomerName, CategoryName = qry.CategoryName, 
                    CategoryRef = qry.OrderNo + "-" + qry.SrNo, OrderId = qry.OrderId, 
                    OrderNo = qry.OrderNo, CandidateDesc = "Application " + qry2.ApplicationNo + " " +
                         qry2.FullName + " PP No." + qry2.PpNo + " referred to " + 
                         qry.CustomerName + " against requirement " + qry.OrderNo + "-" + qry.SrNo,
                    ApplicationNo = qry2.ApplicationNo, CandidateName = qry2.FullName, PPNo = qry2.PpNo,
                    CategoryId = qry.CategoryId, DocControllerAdminTaskId = (int)cvrvwData.DocControllerAdminTaskId,
                    CVReviewId = cvrvwData.Id
               };
               
          }

          public async Task<CommonDataDto> CommonDataFromOrderItemCandidateIdWithChecklistId(int OrderItemId, int candidateId)
          {
               var catMatchesAndChklist = await (from c in _context.ChecklistHRs 
                         where c.CandidateId==candidateId && c.OrderItemId == OrderItemId
                    join i in _context.OrderItems on c.OrderItemId equals i.Id
                    join p in _context.UserProfessions on c.CandidateId equals p.CandidateId where p.CategoryId==i.CategoryId
                    select new {ChecklistId = c.Id, ProfessionId=p.CategoryId}
               ).ToListAsync();

               if (catMatchesAndChklist.Count == 0) return null;

               int chklistId = catMatchesAndChklist.Select(x => x.ChecklistId).FirstOrDefault();
               var qry = await (from i in _context.OrderItems where i.Id == OrderItemId 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    join c in _context.Customers on ordr.CustomerId equals c.Id
                    select (new {
                         CustomerName = c.CustomerName, 
                         CategoryName = cat.Name, 
                         CategoryId = i.CategoryId,
                         OrderId = ordr.Id,
                         OrderItemId= i.Id,
                         OrderNo = ordr.OrderNo,
                         SrNo = i.SrNo,
                         NoReviewBySupervisor = i.NoReviewBySupervisor,
                         HRSupId = i.HrSupId,
                         HRMId = i.HrmId, HRExecId = i.HrExecId
                    })).FirstOrDefaultAsync();
                    
               var hrexecTaskId = await _context.Tasks.Where(x => x.OrderItemId == OrderItemId &&
                    x.AssignedToId == qry.HRExecId && x.TaskTypeId == (int)EnumTaskType.AssignTaskToHRExec)
                    .Select(x => x.Id).FirstOrDefaultAsync();
               if (hrexecTaskId == 0) return null;

               var qry2 = await _context.Candidates.Where(x => x.Id == candidateId)
                    .Select(x => new {x.ApplicationNo, x.FullName}).FirstOrDefaultAsync();
               
               return new CommonDataDto{
                    CustomerName = qry.CustomerName, CategoryName = qry.CategoryName, OrderNo = qry.OrderNo,
                    ApplicationNo = qry2.ApplicationNo, CandidateName = qry2.FullName, CategoryId = qry.CategoryId,
                    NoReviewBySupervisor = qry.NoReviewBySupervisor, HRSupId = (int)qry.HRSupId, HRMId = (int)qry.HRMId,
                    HRExecId=(int)qry.HRExecId, OrderId = qry.OrderId, OrderItemId = qry.OrderItemId, 
                    OrderItemSrNo = qry.SrNo, CandidateId = candidateId, ChecklistHRId = chklistId, 
                    HRExecTaskId = hrexecTaskId
               };
               
          }

          public async Task<string> CandidateNameFromCandidateId(int CandidateId)
          {
               var candidatename = await _context.Candidates.Where(x => x.Id == CandidateId).Select(x => x.FullName).FirstOrDefaultAsync();
               return candidatename;
          }
     }
}