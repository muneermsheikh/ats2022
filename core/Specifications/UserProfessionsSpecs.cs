using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
    public class UserProfessionsSpecs: BaseSpecification<UserProfession>
    {
        public UserProfessionsSpecs(UserProfessionsSpecParams specParams)
            : base(x => 
                (!specParams.CandidateId.HasValue || x.CandidateId == specParams.CandidateId) &&
                (!specParams.IndustryId.HasValue || x.IndustryId == specParams.IndustryId) &&
                (!specParams.CategoryId.HasValue || x.Id == specParams.CategoryId ) &&
                (!specParams.IsMain.HasValue || x.IsMain == specParams.IsMain)
                )
        {
            ApplyPaging(specParams.PageIndex * (specParams.PageSize - 1), specParams.PageSize);
            AddOrderBy(x => x.CandidateId);
            AddOrderBy(x => x.CategoryId);
        }

        public UserProfessionsSpecs(int candidateId, int[] categoryIds) 
            : base(x => x.CandidateId == candidateId &&  categoryIds.Contains(x.CategoryId))
        {
        }

        public UserProfessionsSpecs(int candidateId) 
            : base(x => x.CandidateId == candidateId)
        {}
    }
}