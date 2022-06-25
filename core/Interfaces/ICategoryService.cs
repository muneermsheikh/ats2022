using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
	public interface ICategoryService
    {
        Task<Category> AddCategory(string categoryName);
        Task<bool> EditCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Category category);
        Task<Pagination<Category>> GetCategoryListAsync(CategorySpecParams categoryParams);
        Task<ICollection<Category>> GetCategoriesAsync();
        Task<ICollection<Category>> GetCategoriesOfAnIndustryAsync(string industryname);
        Task<Category> GetCategoryById(int id);
        Task<int> CategoryFromOrderItemId (int orderItemId);

    }
}