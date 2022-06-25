using System;
using System.Linq;
using core.Entities;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class CandidateForCountSpecs: BaseSpecification<Candidate>
    {
        public CandidateForCountSpecs(CandidateSpecParams candParams)
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
                && (!candParams.AssociateId.HasValue || x.CompanyId == candParams.AssociateId)
            )
        {
        }

        public CandidateForCountSpecs(int id, string dummy ) 
            : base(x => x.Id == id)
        {
        }

  /*
        public CandidateForCountSpecs(int appUserId)
        : base(x => x.AppUserId == appUserId)
        {
        }
  */
    }
}