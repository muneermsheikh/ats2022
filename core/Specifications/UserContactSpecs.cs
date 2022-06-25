using System;
using System.Linq;
using core.Entities.Admin;
using core.Params;

namespace core.Specifications
{
     public class UserContactSpecs: BaseSpecification<UserHistory>
    {
        public UserContactSpecs(UserHistoryParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.PersonName) || (x.PersonName.ToLower().Contains(specParams.PersonName.ToLower())) &&
                (string.IsNullOrEmpty(specParams.EmailId) || x.EmailId == specParams.EmailId) &&
                (string.IsNullOrEmpty(specParams.MobileNo) || x .PhoneNo == specParams.MobileNo) &&
                (!specParams.Id.HasValue || x.Id == (int)specParams.Id) &&
                (!specParams.PersonId.HasValue || x.PersonId == (int)specParams.PersonId)) 
            )

        {
            //ApplyPaging(specParams.PageIndex * (specParams.PageIndex - 1), specParams.PageSize);
            AddInclude(x => x.UserHistoryItems.OrderByDescending(x => x.DateOfContact));
        }
    }
}
