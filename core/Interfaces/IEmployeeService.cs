using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IEmployeeService
    {
         Task<bool> EditEmployee(Employee employee);
         Task<bool> DeleteEmployee(Employee employee);
         Task<ICollection<Employee>> AddNewEmployees(ICollection<EmployeeToAddDto> employees);
         Task<Pagination<EmployeeBriefDto>> GetEmployeePaginated(EmployeeSpecParams empParams);
         Task<EmployeeDto> GetEmployeeFromIdAsync(int employeeId);
         Task<Employee> GetEmployeeById(int id);
         Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs();
         Task<int> GetEmployeeIdFromAppUserIdAsync(int appUserId);
         Task<EmployeeDto> GetEmployeeBriefAsyncFromAppUserId(int appUserId);
        Task<EmployeeDto> GetEmployeeBriefAsyncFromEmployeeId(int id);
        Task<string> GetEmployeeNameFromEmployeeId(int id);
        Task<ICollection<EmployeePosition>> GetEmployeePositions();
        Task<int> GetEmployeeIdFromEmail(string email);
        Task<AppUser> CreateAppUserForEmployee(int employeeId, string knownas, string email, string password);
    }
}