using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.MasterEntities;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class AssessmentQBankService : IAssessmentQBankService
     {
        private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
        public AssessmentQBankService(ATSContext context, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

          public async Task<AssessmentQBank> GetAssessmentQBankByCategoryId(int id)
          {
            var x = await _context.AssessmentQBank.Where(x => x.CategoryId == id)
                .Include(x => x.AssessmentQBankItems.OrderBy(x => x.QNo))
                .FirstOrDefaultAsync();
            return x;
          }

          public async Task<ICollection<AssessmentQBank>> GetAssessmentQBanks()
        {
            var x = await _context.AssessmentQBank.Include(x => x.AssessmentQBankItems.OrderBy(x => x.QNo))
                .OrderBy(x => x.CategoryId)
                .ToListAsync();
            return x;
        }

          public async Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName)
          {
                var x = await _context.AssessmentQBank.Include(x => x.AssessmentQBankItems.OrderBy(x => x.QNo))
                    .Where(x => x.CategoryName.ToLower() == categoryName.ToLower())
                    .FirstOrDefaultAsync();
                
                return x;
          }

          public async Task<ICollection<Category>> GetExistingCategoriesInAssessmentQBank()
        {
            /*    
            var qry = await _context.AssessmentQBanks
                .Join(_context.Categories,
                    a => a.CategoryId,
                    c => c.Id,
                    (a, c) => new {Category = c})
                .Select(x => x.Category)
                .OrderBy(x => x.Name)
                .ToListAsync();
            */
            var qry = await _context.AssessmentQBank   //.GroupBy(x => x.CategoryName)
                .Include(x => x.AssessmentQBankItems)
                .Select (x => new Category{Id = (int)x.CategoryId, Name = x.CategoryName})
                //.GroupBy(x => x.Name)
                .OrderBy(x => x.Name)
                .ToListAsync();
            return (ICollection<Category>)qry;
        }

          public async Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model)
          {
               var q = await _context.AssessmentQBank.Where(x => x.CategoryId == model.CategoryId).FirstOrDefaultAsync();
               if (q != null) return null;

               _unitOfWork.Repository<AssessmentQBank>().Add(model);
               if (await _unitOfWork.Complete() > 0) return model;
               return null;
          }

          public async Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model)
        {
            var existingQ = await _context.AssessmentQBank.Where(x => x.Id == model.Id || x.CategoryId == model.CategoryId).Include(x => x.AssessmentQBankItems).FirstOrDefaultAsync();
            if (existingQ == null) {
                var newItems = new List<AssessmentQBankItem>();
                newItems = (List<AssessmentQBankItem>)model.AssessmentQBankItems;
                var newQ = new AssessmentQBank{
                    CategoryId = model.CategoryId, CategoryName = model.CategoryName, 
                        AssessmentQBankItems = newItems };
                _unitOfWork.Repository<AssessmentQBank>().Add(newQ);
                if (await _unitOfWork.Complete() > 0) {
                    return newQ;
                } else {
                    return null;
                }
            }

            _context.Entry(existingQ).CurrentValues.SetValues(model);

            //items that exist in database, i.e. existingQ, but not in model, are the ones that are deleted
            foreach(var existingItem in existingQ.AssessmentQBankItems.ToList())
            {
                if (model.AssessmentQBankItems.Any(c => c.Id == existingItem.Id && c.Id != default(int) ))
                {
                    _context.AssessmentQBankItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            //insert or update items in model
            var nextSrNo = model.AssessmentQBankItems.Max(x => x.QNo)+1;
            foreach(var item in model.AssessmentQBankItems)
            {
                var existingItem = existingQ.AssessmentQBankItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(item);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert
                    var newItem = new AssessmentQBankItem{
                        AssessmentQBankId = existingQ.Id,
                        AssessmentParameter = item.AssessmentParameter,
                        QNo = nextSrNo++,
                        Question = item.Question,
                        MaxPoints = item.MaxPoints
                    };
                    existingQ.AssessmentQBankItems.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }
            }

            _context.Entry(existingQ).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() > 0) {
                return existingQ;
            } else {
                return null;
            }

        }
     }
}