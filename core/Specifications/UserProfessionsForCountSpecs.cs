using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
    public class UserProfessionsForCountSpecs: BaseSpecification<UserProfession>
    {
        public UserProfessionsForCountSpecs(UserProfessionsSpecParams specParams)
            : base(x => 
                (!specParams.CandidateId.HasValue || x.CandidateId == specParams.CandidateId) &&
                (!specParams.IndustryId.HasValue || x.IndustryId == specParams.IndustryId) &&
                (!specParams.CategoryId.HasValue || x.Id == specParams.CategoryId ) &&
                (!specParams.IsMain.HasValue || x.IsMain == specParams.IsMain)
                )
        {
        }
        
        public UserProfessionsForCountSpecs(int candidateId, int[] categoryIds) 
            : base(x => x.CandidateId == candidateId &&  categoryIds.Contains(x.CategoryId))
        {
        }
 
    }
}