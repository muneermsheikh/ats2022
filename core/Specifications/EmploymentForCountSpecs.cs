using System;
using core.Entities.HR;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
     public class EmploymentForCountSpecs: BaseSpecification<Employment>
    {
        public EmploymentForCountSpecs(EmploymentParams cParams)
            : base(x => 
                (!cParams.OrderId.HasValue || x.OrderId == cParams.OrderId) &&
                (!cParams.OrderNo.HasValue || x.OrderNo == cParams.OrderNo) &&
                (!cParams.OrderItemId.HasValue || x.OrderItemId == cParams.OrderItemId) &&
                (!cParams.CategoryId.HasValue || x.CategoryId == cParams.CategoryId) &&
                (string.IsNullOrEmpty(cParams.CategoryName) || x.CategoryName == cParams.CategoryName) &&
                (!cParams.CVRefId.HasValue || x.CVRefId == cParams.CVRefId) &&
                (!cParams.CandidateId.HasValue || x.CandidateId == cParams.CandidateId) &&
                (!cParams.ApplicationNo.HasValue ||  x.ApplicationNo == cParams.ApplicationNo) 
                )
        {
        }

        public EmploymentForCountSpecs(int orderItemId) 
            : base(x => x.OrderItemId == orderItemId)
        {
        }

        public EmploymentForCountSpecs(int orderid, int dummy) 
            : base(x => x.OrderId == orderid)
        {
        }

        public EmploymentForCountSpecs(string candidateId) 
            : base(x => x.CandidateId == Convert.ToInt32(candidateId))
        {
        }
        
        public EmploymentForCountSpecs(string applicationNo, string dummy) 
            : base(x => x.ApplicationNo == Convert.ToInt32(applicationNo))
        {
        }
        
      }
}