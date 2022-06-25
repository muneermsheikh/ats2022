using System;
using core.Entities.Tasks;
using core.Params;

namespace core.Specifications
{
    public class TaskSpecs: BaseSpecification<ApplicationTask>
    {
        public TaskSpecs(TaskParams eParams)
            : base(x => 
                (!eParams.ApplicationNo.HasValue || x.ApplicationNo == eParams.ApplicationNo) &&
                (!eParams.OrderNo.HasValue || x.OrderNo == eParams.OrderNo) &&
                (!eParams.OrderItemId.HasValue || x.OrderItemId == eParams.OrderItemId) &&
                (!eParams.CandidateId.HasValue || x.CandidateId == eParams.CandidateId) &&
                (!eParams.OrderId.HasValue || x.OrderId == eParams.OrderId) &&
                (!eParams.DateFrom.HasValue && !eParams.DateUpto.HasValue || 
                    Nullable.Compare(x.TaskDate.Date, eParams.DateFrom) >= 0 && 
                    Nullable.Compare(x.TaskDate.Date, eParams.DateUpto) >= 0) &&
                (!eParams.DateFrom.HasValue && eParams.DateUpto.HasValue || 
                    DateTime.Compare(x.TaskDate.Date, (DateTime)eParams.DateFrom) == 0) &&
                (!eParams.CompleteBy.HasValue || 
                    DateTime.Compare(x.CompleteBy.Date, (DateTime)eParams.CompleteBy) <= 0) &&
                (string.IsNullOrEmpty(eParams.TaskStatus) || x.TaskStatus.ToLower() == eParams.TaskStatus.ToLower()) &&
                (!eParams.TaskOwnerId.HasValue || x.TaskOwnerId == eParams.TaskOwnerId) &&
                (!eParams.AssignedToId.HasValue || x.AssignedToId == eParams.AssignedToId) &&
                (!eParams.TaskTypeId.HasValue || x.TaskTypeId == eParams.TaskTypeId) 
                /* &&
                (!eParams.CandidateId.HasValue || x.PersonType.ToLower() == eParams.PersonType.ToLower() && x.CandidateId == eParams.CandidateId) 
                */
            )
        {
            if (eParams.IncludeItems) AddInclude(x => x.TaskItems);
            AddOrderBy(x => x.OrderId);
            AddOrderBy(x => x.OrderItemId);
            AddOrderBy(x => x.TaskDate);
        }
        
        public TaskSpecs(string taskStatus) : base (x => x.TaskStatus.ToLower() == taskStatus)
        {
            AddOrderBy(x => x.OrderId);
            AddOrderBy(x => x.OrderItemId);
            AddOrderBy(x => x.TaskDate);
        }
        public TaskSpecs(int taskownerid, bool includeTaskItems, int pageIndex, int pageSize) : 
            base (x => x.TaskStatus.ToLower() != "completed" && x.TaskStatus.ToLower() != "canceled" && x.TaskOwnerId == taskownerid)
        {
            ApplyPaging(pageSize * (pageIndex -1), pageSize);
            if (includeTaskItems) AddInclude(x => x.TaskItems);
            AddOrderBy(x => x.TaskDate);
            
        }
    }
}