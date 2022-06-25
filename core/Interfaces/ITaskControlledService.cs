using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Tasks;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface ITaskControlledService
    {
        Task<Pagination<ApplicationTask>> GetApplicationTasksPaginated(TaskParams taskParams);
        Task<ICollection<ApplicationTask>> GetApplicationTasksWOPagination(TaskParams taskParams);
        Task<Pagination<ApplicationTask>> GetApplicationPendingTasksPaginated(string taskStatus, int pageIndex, int pageSize);
        Task<ICollection<EmailMessage>> CreateTaskForHRExecOnOrderItemIds(ICollection<OrderAssignmentDto> assignments, int loggedInUserEmployeeId);
    }
}