using System;
using System.Linq;
using System.Linq.Expressions;
using core.Entities;
using core.Entities.HR;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
     public class CandidateSpecs : BaseSpecification<Candidate>
     {
          public CandidateSpecs(CandidateSpecParams candParams)
            : base(x => 
                (!candParams.CandidateId.HasValue || x.Id == candParams.CandidateId)
                && (string.IsNullOrEmpty(candParams.Search) || 
                  x.FirstName.ToLower().Contains(candParams.Search.ToLower()) 
                  || x.SecondName.ToLower().Contains(candParams.Search.ToLower())
                  || x.FamilyName.ToLower().Contains(candParams.Search.ToLower()))
                && (!candParams.ApplicationNoFrom.HasValue && candParams.ApplicationNoUpto.HasValue||
                    x.ApplicationNo == candParams.ApplicationNoFrom) 
                && ((!candParams.ApplicationNoFrom.HasValue && !candParams.ApplicationNoUpto.HasValue)||
                    x.ApplicationNo >= candParams.ApplicationNoFrom &&
                    x.ApplicationNo <= candParams.ApplicationNoUpto) 
               /* && (!candParams.RegisteredFrom.HasValue && candParams.RegisteredUpto.HasValue || 
                    DateTime.Compare(x.Created, Convert.ToDateTime(candParams.RegisteredFrom)) <= 0) 
                && (!candParams.RegisteredFrom.HasValue && !candParams.RegisteredUpto.HasValue ||
                    DateTime.Compare(x.Created, Convert.ToDateTime(candParams.RegisteredFrom)) >= 0 
                    && DateTime.Compare(x.Created, Convert.ToDateTime(candParams.RegisteredUpto)) <= 0  )
                */
                && (string.IsNullOrEmpty(candParams.City) || x.City.ToLower() == candParams.City.ToLower())
                && (!candParams.ProfessionId.HasValue || 
                  x.UserProfessions.Select(x => x.CategoryId).Contains((int)candParams.ProfessionId))
                && (!candParams.AssociateId.HasValue || x.CompanyId == candParams.AssociateId)
             )
          {
              if (candParams.IncludeUserProfessions) AddInclude(x => x.UserProfessions);
              
              if (candParams.PageSize > 0) ApplyPaging(candParams.PageSize * (candParams.PageIndex - 1), candParams.PageSize);

              if (!string.IsNullOrEmpty(candParams.Sort)) {
                switch(candParams.Sort.ToLower()) {
                  case "name":
                    AddOrderBy(x => x.FirstName);
                    break;
                  case "namedesc":
                    AddOrderByDescending(x => x.FirstName);
                    break;
                  /*
                  case "agent":
                    AddOrderBy(x => x.ReferredByName);
                    break;
                  
                  case "agentdesc":
                    AddOrderByDescending(x => x.ReferredByName);
                    break;
                  */
                  case "city":
                    AddOrderBy(x => x.City);
                    break;
                  
                  case "citydesc":
                    AddOrderByDescending(x => x.City);
                    break;

                  default: AddOrderBy(x => x.ApplicationNo);
                    break;
                }
              }
          }

          public CandidateSpecs(int id, string dummy) 
            : base(x => x.Id == id)
          {
              //AddInclude(x => x.EntityAddresses);
              //AddInclude(x => x.UserAttachments);
              //AddInclude(x => x.UserPassports);
              AddInclude(x => x.UserProfessions);
              //AddInclude(x => x.UserPhones);
              //AddInclude(x => x.UserQualifications);
              
              AddOrderBy(x => x.ApplicationNo);
          }
          /*
          public CandidateSpecs(int appUserId) 
            : base(x => x.AppUserId == appUserId)
          {
              //AddInclude(x => x.Addresses);
              
              AddInclude(x => x.UserAttachments);
              AddInclude(x => x.UserPassports);
              AddInclude(x => x.UserProfessions);
              AddInclude(x => x.UserPhones);
              AddInclude(x => x.UserQualifications);
              AddOrderBy(x => x.ApplicationNo);
              
          }
          */
     }
}