using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.Identity;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class EmployeeExtensions
    {
        public static async Task<string> EmployeeNameByIdAsync(this ATSContext context, int Id)
        {

            var emp = await context.Employees.Where(x => x.Id == Id)
                .Select(x => new {x.FirstName, x.SecondName, x.FamilyName}).FirstOrDefaultAsync();
            return emp == null ? "" : emp.FirstName + " " + emp.SecondName + " " + emp.FamilyName;
        }

        public static async Task<string> EmployeeKnownAsByIdAsync(this ATSContext context, int Id)
        {
            return  await context.Employees.Where(x => x.Id == Id)
                .Select(x => x.KnownAs ).FirstOrDefaultAsync();
        }
    }
}