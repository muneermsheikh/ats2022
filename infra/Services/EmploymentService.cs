using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class EmploymentService : IEmploymentService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          public EmploymentService(IUnitOfWork unitOfWork, ATSContext context)
          {
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<Employment> AddEmployment(Employment employment)
          {
               _unitOfWork.Repository<Employment>().Add(employment);
               if (await _unitOfWork.Complete() > 0) {
                   var empParams = new EmploymentParams{CVRefId = employment.CVRefId};
                   var specs = new EmploymentSpecs(empParams);

                   return await _unitOfWork.Repository<Employment>().GetEntityWithSpec(specs);
               } else {
                   return null;
               }
          }

          public async Task<bool> DeleteEmployment(Employment employment)
          {
               _unitOfWork.Repository<Employment>().Delete(employment);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditEmployment(Employment employment)
          {
               // todo - verify object
                _unitOfWork.Repository<Employment>().Update(employment);

               return await _unitOfWork.Complete() > 0;
          }

          public async Task<Employment> GetEmployment(int CVRefId)
          {
               var emp = await _context.Employments.Where(x => x.CVRefId == CVRefId).FirstOrDefaultAsync();
               if (emp == null) {
                    var cvref = await _context.CVRefs.FindAsync(CVRefId);
                    if (cvref == null) return null;
                    emp = new Employment(CVRefId, DateTime.Now , "", 0, 24, false, 0, false, 0, false, 0,0,21,24,cvref.Charges);
                    _unitOfWork.Repository<Employment>().Add(emp);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               return emp;
          }

          public async Task<Employment> GetEmploymentFromSelId(int Id)
          {
               var sel = await _context.SelectionDecisions.Where(x => x.Id == Id).Include(x => x.Employment).FirstOrDefaultAsync();
               var emp = sel.Employment;
               if (emp == null) {
                    var cvref = await _context.CVRefs.FindAsync(sel.CVRefId);
                    if (cvref == null) return null;
                    emp = new Employment(sel.CVRefId, DateTime.Now , "", 0, 24, false, 0, false, 0, false, 0,0,21,24,cvref.Charges);
                    _unitOfWork.Repository<Employment>().Add(emp);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               return emp;
          }

          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoFromOrderNo (int orderNo)
          {
               var emps = await (from emp in _context.Employments where emp.OrderNo == orderNo
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }

          
          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoBetwenDates (DateTime fromDate, DateTime uptoDate)
          {
               var emps = await (from emp in _context.Employments 
                         where (DateTime.Compare(emp.SelectedOn, fromDate) >= 0 
                              && DateTime.Compare(emp.SelectedOn, uptoDate) <= 0)
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }

          
          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoFromCVRefId (int cvrefid)
          {
               var emps = await (from emp in _context.Employments where emp.CVRefId == cvrefid
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }



     }
}