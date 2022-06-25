using System;
using core.Entities.Admin;
using core.Params;

namespace core.Specifications
{
     public class UserContactForCountSpecs : BaseSpecification<UserHistory>
     {
          public UserContactForCountSpecs(UserHistoryParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.PersonName) || (x.PersonName.ToLower().Contains(specParams.PersonName.ToLower())) &&
                (string.IsNullOrEmpty(specParams.EmailId) || x.EmailId == specParams.EmailId) &&
                (string.IsNullOrEmpty(specParams.MobileNo) || x .PhoneNo == specParams.MobileNo) &&
                (!specParams.Id.HasValue || x.Id == (int)specParams.Id) &&
                (!specParams.PersonId.HasValue || x.PersonId == (int)specParams.PersonId)) 
            )
          {
          }
     }
}