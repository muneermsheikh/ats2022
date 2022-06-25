using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface ITaskService
    {

        Task<ApplicationTask> GetOrCreateTask(ApplicationTask task);
        
        Task<Pagination<ApplicationTask>> GetApplicationPendingTasksOfAUserPaginated(int userid, int pageIndex, int pageSize);
        Task<ICollection<TaskDashboardDto>> GetDashboardTasksOfLoggedInUser(int loggedInEmployeeId);
        Task<MessagesDto> CreateNewApplicationTask(ApplicationTask task, int LoggedInEmployeeId);
        Task<ApplicationTask> CreateNewAppTask(ApplicationTask task, int LoggedInEmployeeId);

        Task<MessagesDto> EditApplicationTask(ApplicationTask task, int employeeId);
        Task<bool> DeleteApplicationTask(ApplicationTask task);
        Task<TaskItem> CreateNewTaskItem(TaskItem taskItem);
        Task<TaskItem> EditTaskItem(TaskItem taskItem);
        Task<bool> DeleteTaskItem(TaskItem taskItem);
        Task<bool> SetApplicationTaskStatus(int ApplicationTaskId, DateTime dateOfStatus, 
               string TaskStatus, string UserName, int AppUserId);
        
        Task<ApplicationTask> GetHRExecTaskForCVCompiling(int orderitemId, int candidateId);   
        Task<ApplicationTask> GetHRSupTaskForCVCompiling(int orderitemId, int candidateId);
        
    }
}