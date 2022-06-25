using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class CategoryForCountSpecs: BaseSpecification<Category>
    {
        public CategoryForCountSpecs(CategorySpecParams catParams)
            : base(x => 
                (string.IsNullOrEmpty(catParams.Search) || 
                  x.Name.ToLower().Contains(catParams.Search.ToLower()))
                )
                
        {
        }

        public CategoryForCountSpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}