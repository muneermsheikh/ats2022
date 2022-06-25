using System;
using System.Linq;
using core.Entities;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Users;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class SelDecisionForCountSpecs: BaseSpecification<SelectionDecision>
    {
        public SelDecisionForCountSpecs(SelDecisionSpecParams specParams)
            : base(x => 
                (!specParams.OrderId.HasValue || x.OrderId == specParams.OrderId) &&
                (!specParams.OrderNo.HasValue || x.OrderNo == specParams.OrderNo) &&
                (!specParams.OrderItemId.HasValue || x.OrderItemId == specParams.OrderItemId) &&
                (!specParams.CategoryId.HasValue || x.CategoryId == specParams.CategoryId) &&
                (string.IsNullOrEmpty(specParams.CategoryName) || x.CategoryName == specParams.CategoryName) &&
                (!specParams.CVRefId.HasValue || x.CVRefId == specParams.CVRefId) &&
                (!specParams.CandidateId.HasValue || x.CandidateId == specParams.CandidateId) &&
                (!specParams.ApplicationNo.HasValue ||  x.ApplicationNo == specParams.ApplicationNo) &&
                (specParams.CVRefIds.Length==0 || specParams.CVRefIds.Contains(x.CVRefId))
                )
        {
        }

        public SelDecisionForCountSpecs(int orderItemId) 
            : base(x => x.OrderItemId == orderItemId)
        {
        }

        public SelDecisionForCountSpecs(int orderid, int dummy) 
            : base(x => x.OrderId == orderid)
        {
        }

        public SelDecisionForCountSpecs(string candidateId) 
            : base(x => x.CandidateId == Convert.ToInt32(candidateId))
        {
        }
        
        public SelDecisionForCountSpecs(string applicationNo, string dummy) 
            : base(x => x.ApplicationNo == Convert.ToInt32(applicationNo))
        {
        }
        
      }
}