using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Tasks;

namespace core.Interfaces
{
    public interface IValidateTaskService
    {
        Task<string> ValidateTaskObject(ApplicationTask task);
    }
}