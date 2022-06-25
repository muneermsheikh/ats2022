using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class UserDto
    {
        public int loggedInEmployeeId {get; set;}
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public ICollection<TaskDashboardDto> dashboardTasks {get; set;}
    }
}