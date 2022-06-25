using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities;

namespace core.Interfaces
{
    public interface ICategoryRepository
    {
         Task<Category> GetCategoryByIdAsync(int id);
         Task<IReadOnlyList<Category>> GetCategoriesAsync();
    }
}