using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     
     
     public class EmployeesController : BaseApiController
     {
          private readonly IGenericRepository<Employee> _empRepo;
          private readonly IEmployeeService _empService;
          private readonly IUnitOfWork _unitOfWork;
          private readonly UserManager<AppUser> _userManager;
          private readonly ATSContext _context;
          private readonly ITokenService _tokenService;
          public EmployeesController(IGenericRepository<Employee> empRepo, IEmployeeService empService, ITokenService tokenService,
               IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ATSContext context)
          {
               _tokenService = tokenService;
               _context = context;
               _userManager = userManager;
               _unitOfWork = unitOfWork;
               _empService = empService;
               _empRepo = empRepo;
          }

          [Authorize]
          [HttpGet("employeepages")]
          public async Task<ActionResult<Pagination<EmployeeBriefDto>>> GetEmployees([FromQuery]EmployeeSpecParams empParams)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               int loggedInEmployeeId = loggedInUser == null ? 0 : await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               
               var emps = await _empService.GetEmployeePaginated(empParams);
               if (emps == null) return NotFound(new ApiResponse(404, "No employees found"));
               return Ok(emps);
          }

          [Authorize]
          [HttpGet("byId/{id}")]
          public async Task<ActionResult<Employee>> GetEmployeeById(int id)
          {
               var claim = this.HttpContext.User.Claims;
               var emp = await _empService.GetEmployeeById(id);
               if (emp == null) return NotFound();
               return Ok(emp);
          }

          [HttpGet("idandknownas")]
          public async Task<ActionResult<ICollection<EmployeeIdAndKnownAsDto>>> GetEmployeIdAndKnownAs() 
          {
               var emps = await _empService.GetEmployeeIdAndKnownAs();

               return Ok(emps);
          }
          [Authorize(Roles = "Admin, HRManager")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditEmployee(Employee employee)
          {
               var email = employee.Email;
               if (string.IsNullOrEmpty(email)) return BadRequest(new
                    ApiResponse(400, "email Id for employee " + employee.FirstName + " " + employee.SecondName + " " + employee.FamilyName +
                    " not provided"));
               /*
               if (user.Email != email)
               {
                    //user email has changed
                    return BadRequest(new ApiResponse(400, "Failed to update the employee - The AppUserId exists, but email Id does not match"));
               } 
               */
               return await _empService.EditEmployee(employee);

          }

          [Authorize(Roles = "Admin, HRManager")]
          [HttpDelete]
          public async Task<ActionResult<bool>> DeleteEmployee(Employee employee)
          {
               await _empService.DeleteEmployee(employee);

               if (await _unitOfWork.Complete() > 0)
               {
                    var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());
                    if (user != null) await _userManager.DeleteAsync(user);
                    return Ok();
               }

               return BadRequest(new ApiResponse(400, "Failed to delete the employee"));
          }

          [Authorize(Roles = "Admin, HRManager")]
          [HttpPost("employees")]
          public async Task<ActionResult<ICollection<UserDto>>> AddNewEmployees(ICollection<EmployeeToAddDto> employees)
          {
               
               var emps = await _empService.AddNewEmployees(employees);

               if (emps == null || emps.Count == 0) return BadRequest(new ApiResponse(402, "Failed to add any employee"));
               
               //return Ok(emps);
               
               var users = new List<UserDto>();
               foreach(var emp in emps)
               {
                    var appuser = await _userManager.FindByIdAsync(emp.AppUserId.ToString());
                    users.Add(new UserDto {
                         loggedInEmployeeId = emp.Id,
                         DisplayName = emp.KnownAs,
                         Token = await _tokenService.CreateToken(appuser),
                         Email = emp.Email
                    });
               }

               return users;
          }

          [Authorize]
          [HttpGet("employeepositions")]
          public async Task<ActionResult<ICollection<EmployeePosition>>> GetEmployeePositions()
          {
               var posn =  await _empService.GetEmployeePositions();

               if (posn == null) return NotFound();

               return Ok(posn);
          
          }
          
     }
}