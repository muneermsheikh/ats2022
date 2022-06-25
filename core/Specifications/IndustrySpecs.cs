using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class IndustrySpecs: BaseSpecification<Industry>
    {
        public IndustrySpecs(IndustrySpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.IndustryNameLike) || 
                  x.Name.ToLower().Contains(specParams.IndustryNameLike.ToLower())) &&
                (!specParams.IndustryId.HasValue || x.Id == specParams.IndustryId)
            )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderBy(x => x.Name);
        }

        public IndustrySpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}