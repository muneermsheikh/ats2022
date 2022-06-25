using System;
using core.Entities.Tasks;
using core.Params;

namespace core.Specifications
{
    public class TaskForCountSpecs: BaseSpecification<ApplicationTask>
    {
        public TaskForCountSpecs(TaskParams eParams)
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
        }

        public TaskForCountSpecs(string taskStatus) : base (x => x.TaskStatus.ToLower() == taskStatus)
        {
        }

        public TaskForCountSpecs(int taskownerid, bool includeTaskItems) : 
            base (x => x.TaskStatus.ToLower() != "completed" && x.TaskStatus.ToLower() != "canceled" && x.TaskOwnerId == taskownerid)
        {
        }

    }
}