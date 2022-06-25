using System;
using System.Linq;
using core.Entities;
using core.Entities.HR;
using core.Entities.Process;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Specifications
{
    public class CVRefForCountSpecs: BaseSpecification<CVRef>
    {
        public CVRefForCountSpecs(CVRefSpecParams refParams)
            : base(x => 
                (!refParams.OrderId.HasValue || x.OrderId == refParams.OrderId) &&
                (!refParams.OrderNo.HasValue || x.OrderNo == refParams.OrderNo) &&
                (!refParams.OrderItemId.HasValue || x.OrderItemId == refParams.OrderItemId) &&
                (!refParams.CategoryId.HasValue || x.CategoryId == refParams.CategoryId) &&
                (string.IsNullOrEmpty(refParams.CategoryName) || x.CategoryName == refParams.CategoryName) &&
                (!refParams.Id.HasValue || x.Id == refParams.Id) &&
                (!refParams.CandidateId.HasValue || x.CandidateId == refParams.CandidateId) &&
                (!refParams.ApplicationNo.HasValue ||  x.ApplicationNo == refParams.ApplicationNo) &&
                (refParams.Ids.Count() == 0 || refParams.Ids.Contains(x.Id))
            )
        {
        }
        public CVRefForCountSpecs(int pageIndex, int pageSize)
            : base(x => x.DeployStageId < (int)EnumDeployStatus.Concluded)
        {
        }
    }
}