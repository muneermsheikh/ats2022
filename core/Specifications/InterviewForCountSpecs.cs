using System;
using System.Linq;
using core.Entities;
using core.Entities.HR;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class InterviewForCountSpecs: BaseSpecification<Interview>
    {
        public InterviewForCountSpecs(InterviewSpecParams IParams)
            :base(x =>
            (!IParams.InterviewId.HasValue || x.Id == IParams.InterviewId) &&
                (!IParams.InterviewItemId.HasValue || 
                  x.InterviewItems.Select(x => x.Id == IParams.InterviewItemId).FirstOrDefault()) 
                && (!IParams.OrderId.HasValue || x.OrderId == IParams.OrderId)
                && (!IParams.OrderItemId.HasValue || 
                  x.InterviewItems.Select(x => x.OrderItemId == IParams.OrderItemId).FirstOrDefault() )
                && (!IParams.CustomerId.HasValue || x.CustomerId==IParams.CustomerId)
                && (!IParams.CategoryId.HasValue || 
                  x.InterviewItems.Select(x => x.CategoryId == IParams.CategoryId).FirstOrDefault())
                && (string.IsNullOrEmpty(IParams.InterviewStatus) || x.InterviewStatus.ToLower() == IParams.InterviewStatus.ToLower())
                && (string.IsNullOrEmpty(IParams.CustomerName) || x.CustomerName.ToLower().Contains(IParams.CustomerName.ToLower()))
            )

        {
        }


  
    }
}