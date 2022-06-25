using System;
using System.Threading.Tasks;
using core.Entities.Tasks;

namespace core.Interfaces
{
    public interface IAssignTaskService
    {
         Task<ApplicationTask> AssignTask(ApplicationTask applicationTask);
    }
}