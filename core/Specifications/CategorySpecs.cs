using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
    public class CategorySpecs: BaseSpecification<Category>
    {
        public CategorySpecs(CategorySpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.CategoryNameLike) || x.Name.ToLower().Contains(specParams.CategoryNameLike.ToLower())) &&
                (!specParams.CategoryId.HasValue || x.Id == specParams.CategoryId )
                )
        {
            ApplyPaging(specParams.PageIndex * (specParams.PageSize - 1), specParams.PageSize);
            AddOrderBy(x => x.Name);
        }

        public CategorySpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}