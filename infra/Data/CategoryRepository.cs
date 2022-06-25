using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities;
using core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace infra.Data
{
     public class CategoryRepository : ICategoryRepository
     {
          private readonly ATSContext _context;
          public CategoryRepository(ATSContext context)
          {
               _context = context;
          }

          public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
          {
               return await _context.Categories.ToListAsync();
          }

          public async Task<Category> GetCategoryByIdAsync(int id)
          {
               return await _context.Categories.FindAsync(id);
          }
     }
}