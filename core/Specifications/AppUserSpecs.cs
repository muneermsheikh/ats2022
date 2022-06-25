using System;
using System.Linq;
using System.Linq.Expressions;
using core.Entities;
using core.Entities.Identity;
using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
     public class AppUserSpecs : BaseSpecification<AppUser>
     {
          public AppUserSpecs(AppUserSpecParams specParams)
            : base(x => 
                /* (!string.IsNullOrEmpty(specParams.City) ||
                  x.Address.City.ToLower().Contains(specParams.City.ToLower())) &&
                (!string.IsNullOrEmpty(specParams.District) ||
                  x.Address.District.ToLower().Contains(specParams.District.ToLower())) &&
                */
                (!string.IsNullOrEmpty(specParams.Email) ||
                  x.Email.ToLower() == specParams.Email.ToLower()) &&
                (!string.IsNullOrEmpty(specParams.Mobile) || x.PhoneNumber == specParams.Mobile)
              )
          {
              //if (specParams.IncludeEntityAddresses) AddInclude(x => x.Address);
              
              ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

              if (!string.IsNullOrEmpty(specParams.Sort)) {
                switch(specParams.Sort.ToLower()) {
                  case "nameasc":
                    AddOrderBy(x => x.UserName);
                    break;
                  case "namddesc":
                    AddOrderByDescending(x => x.UserName);
                    break;
                /*
                  case "cityasc":
                    AddOrderBy(x => x.Address.City);
                    break;
                  case "citydesc":
                    AddOrderByDescending(x => x.Address.City);
                    break;
                
                  case "distasc":
                    AddOrderBy(x => x.Address.District);
                    break;
                
                  case "distdesc":
                    AddOrderByDescending(x => x.Address.District);
                    break;
                  default: AddOrderBy(x => x.Address.FirstName);
                    break;
                */
                }
              }
          }
          
          public AppUserSpecs(int appUserId) 
            : base(x => x.Id == appUserId)
          {
              //AddInclude(x => x.Address);
          }
          
     }
}