using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly ATSContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(ATSContext context, IUnitOfWork unitOfWork)
        {
                    this._unitOfWork = unitOfWork;
                    this._context = context;
        }

        public async Task<Category> AddCategory(string categoryName)
        {
            var categoryEntity = new Category(categoryName);
            _unitOfWork.Repository<Category>().Add(categoryEntity);
            if (await _unitOfWork.Complete() > 0) return categoryEntity;
            return null;
        }

        public async Task<int> CategoryFromOrderItemId(int orderItemId)
        {
            return await _context.OrderItems.Where(x => x.Id == orderItemId).Select(x => x.CategoryId).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Category>> GetCategoryListAsync(CategorySpecParams specParams)
        {
            var spec = new CategorySpecs(specParams);
            var specCount = new CategoryForCountSpecs(specParams);
            var totalCount = await _unitOfWork.Repository<Category>().CountAsync(specCount);
            var lst = await _unitOfWork.Repository<Category>().ListAsync(spec);
            //var data = _mapper.Map<IReadOnlyList<CategoryToReturnDto>>(lst);

            return new Pagination<Category>(specParams.PageIndex, specParams.PageSize, totalCount, lst);
        }

        public async Task<ICollection<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.OrderBy(x => x.Name).ToListAsync();
        }


        public async Task<bool> EditCategoryAsync(Category category)
        {
            _unitOfWork.Repository<Category>().Update(category);
            return (await _unitOfWork.Complete() > 0);
        }

        public async Task<bool> DeleteCategoryAsync(Category category)
        {
            _unitOfWork.Repository<Category>().Delete(category);
            return (await _unitOfWork.Complete() > 0);
        }

		public Task<Category> GetCategoryById(int id)
		{
			throw new NotImplementedException();
		}

		public Task<ICollection<Category>> GetCategoriesOfAnIndustryAsync(string industryname)
		{
			throw new NotImplementedException();
		}
	}
}