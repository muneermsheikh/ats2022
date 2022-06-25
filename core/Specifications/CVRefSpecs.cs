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
    public class CVRefSpecs: BaseSpecification<CVRef>
    {
        public CVRefSpecs(CVRefSpecParams specParams)
            : base(x => 
                (!specParams.CVRefStatus.HasValue || x.RefStatus == specParams.CVRefStatus) &&
                (!specParams.OrderId.HasValue || x.OrderId == specParams.OrderId) &&
                (!specParams.OrderNo.HasValue || x.OrderNo == specParams.OrderNo) &&
                (!specParams.OrderItemId.HasValue || x.OrderItemId == specParams.OrderItemId) &&
                (!specParams.CategoryId.HasValue || x.CategoryId == specParams.CategoryId) &&
                (string.IsNullOrEmpty(specParams.CategoryName) || x.CategoryName == specParams.CategoryName) &&
                (!specParams.Id.HasValue || x.Id == specParams.Id) &&
                (!specParams.CandidateId.HasValue || x.CandidateId == specParams.CandidateId) &&
                (!specParams.ApplicationNo.HasValue ||  x.ApplicationNo == specParams.ApplicationNo) &&
                (specParams.Ids.Count() == 0 || specParams.Ids.Contains(x.Id))
            )
        {
            if(specParams.IncludeDeployments) AddInclude(x => x.Deploys);
            
            if(specParams.Sort?.ToLower() == "orderid") {
                AddOrderBy(x => x.OrderId);
            } else if (specParams.Sort?.ToLower() == "orderitemid") {
                AddOrderBy(x => x.OrderItemId);
            } else if (specParams.Sort?.ToLower() == "applicationno") {
                AddOrderBy(x => x.ApplicationNo);
            } else if (specParams.Sort?.ToLower() == "categoryid") {
                AddOrderBy(x => x.CategoryId);
            } else {
                AddOrderByDescending(x => x.ReferredOn);
            }
        
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        
        }
  
        public CVRefSpecs(int pageIndex, int pageSize)
            : base(x => x.DeployStageId < (int)EnumDeployStatus.Concluded)
        {
            ApplyPaging(pageSize * (pageIndex - 1), pageSize);
            AddOrderBy(x => x.CustomerName);
            AddOrderBy(x => x.OrderId);
        }
    }
}