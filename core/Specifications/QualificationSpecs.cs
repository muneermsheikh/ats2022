using System;
using System.Linq;
using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class QualificationSpecs: BaseSpecification<Qualification>
    {
        public QualificationSpecs(QualificationSpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.QualificationNameLike) || 
                  x.Name.ToLower().Contains(specParams.QualificationNameLike.ToLower())) &&
                (!specParams.QualificationId.HasValue || x.Id == specParams.QualificationId)
            )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderBy(x => x.Name);
        }
  
    }
}