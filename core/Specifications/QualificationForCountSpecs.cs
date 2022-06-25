using System;
using System.Linq;
using core.Entities;
using core.Entities.MasterEntities;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class QualificationForCountSpecs: BaseSpecification<Qualification>
    {
        public QualificationForCountSpecs(QualificationSpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.QualificationNameLike) || 
                  x.Name.ToLower().Contains(specParams.QualificationNameLike.ToLower())) &&
                (!specParams.QualificationId.HasValue || x.Id == specParams.QualificationId)
            )
        {
        }
  
    }
}