using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class EmployeeService : IEmployeeService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly UserManager<AppUser> _userManager;
          private readonly IMapper _mapper;
          public EmployeeService(ATSContext context, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
          {
               _mapper = mapper;
               _userManager = userManager;
               _unitOfWork = unitOfWork;
               _context = context;
          }

          public async Task<bool> DeleteEmployee(Employee employee)
          {
               _unitOfWork.Repository<Employee>().Delete(employee);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditEmployee(Employee model)
          {
               //thanks to @slauma of stackoverflow
               var existingEmp = _context.Employees
                   .Where(p => p.Id == model.Id)
                   .Include(x => x.EmployeeAddresses)
                   .Include(x => x.EmployeePhones)
                   .Include(x => x.EmployeeQualifications)
                   .Include(x => x.HrSkills)
                   .Include(x => x.OtherSkills)
                   .AsNoTracking()
                   .SingleOrDefault();

               if (existingEmp == null) return false;

               //ignore any changes to AppUserId that the client might ahve made
               model.AppUserId = existingEmp.AppUserId;      //this cannot be changed by the client

               _context.Entry(existingEmp).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               if (existingEmp.EmployeeAddresses != null) {
                    foreach (var existingAdd in existingEmp.EmployeeAddresses.ToList()) {
                         if (!model.EmployeeAddresses.Any(c => c.Id == existingAdd.Id && c.Id != default(int))) {
                              _context.EmployeeAddresses.Remove(existingAdd);
                              _context.Entry(existingAdd).State = EntityState.Deleted;
                         }
                    }
               }

               if (existingEmp.EmployeePhones != null) {
                    foreach (var existingPh in existingEmp.EmployeePhones.ToList()) {
                         if (!model.EmployeePhones.Any(c => c.Id == existingPh.Id && c.Id != default(int))) {
                              _context.EmployeePhones.Remove(existingPh);
                              _context.Entry(existingPh).State = EntityState.Deleted;
                         }
                    }
               }

               if(existingEmp.EmployeeQualifications !=null) {
                    foreach (var existingQ in existingEmp.EmployeeQualifications.ToList()) {
                         if (!model.EmployeeQualifications.Any(c => c.Id == existingQ.Id && c.Id != default(int)))  {
                              _context.EmployeeQualifications.Remove(existingQ);
                              _context.Entry(existingQ).State = EntityState.Deleted;
                         }
                    }
               }

               if(existingEmp.HrSkills !=null) {
                    foreach (var existingSk in existingEmp.HrSkills.ToList())  {
                         if (!model.HrSkills.Any(c => c.Id == existingSk.Id && c.Id != default(int))) {
                              _context.EmployeeHRSkills.Remove(existingSk);
                              _context.Entry(existingSk).State = EntityState.Deleted;
                         }
                    }
               }

               if (existingEmp.OtherSkills !=null) {
                    foreach (var existingOSk in existingEmp.OtherSkills.ToList())  {
                         if (!model.OtherSkills.Any(c => c.Id == existingOSk.Id && c.Id != default(int))) {
                              _context.EmployeeOtherSkills.Remove(existingOSk);
                              _context.Entry(existingOSk).State = EntityState.Deleted;
                         }
                    }
               }

               //children that are not deleted, are either updated or new ones to be added

               foreach (var item in model.EmployeeAddresses)
               {
                    if (existingEmp.EmployeeAddresses != null) {
                         var existingAdd = existingEmp.EmployeeAddresses.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingAdd != null)  {     // record exists, update it
                              _context.Entry(existingAdd).CurrentValues.SetValues(item);
                              _context.Entry(existingAdd).State = EntityState.Modified;
                         }  else {           //record does not exist, insert a new record
                              var newAdd = new EmployeeAddress(existingEmp.Id, item.AddressType, item.Add, item.StreetAdd,
                                   item.City, item.Pin, item.State, item.District, item.Country, item.IsMain);
                              existingEmp.EmployeeAddresses.Add(newAdd);
                              _context.Entry(newAdd).State = EntityState.Added;
                         }
                    } else {
                         var newAdd = new EmployeeAddress(existingEmp.Id, item.AddressType, item.Add, item.StreetAdd,
                              item.City, item.Pin, item.State, item.District, item.Country, item.IsMain);
                         existingEmp.EmployeeAddresses.Add(newAdd);
                         _context.Entry(newAdd).State = EntityState.Added;
                   }
               }

               foreach (var item in model.EmployeePhones)
               {
                    if (existingEmp.EmployeePhones != null) {
                         var existingPh = existingEmp.EmployeePhones.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingPh != null)  {     // record exists, update it
                              _context.Entry(existingPh).CurrentValues.SetValues(item);
                              _context.Entry(existingPh).State = EntityState.Modified;
                         }  else {           //record does not exist, insert a new record
                              var newPh = new EmployeePhone(existingEmp.Id, item.MobileNo, item.IsOfficial, item.IsMain);
                              existingEmp.EmployeePhones.Add(newPh);
                              _context.Entry(newPh).State = EntityState.Added;
                         }
                    } else {
                         var newPh = new EmployeePhone(existingEmp.Id, item.MobileNo, item.IsOfficial, item.IsMain);
                         existingEmp.EmployeePhones?.Add(newPh);
                         _context.Entry(newPh).State = EntityState.Added;
                   }
               }

               foreach (var item in model.EmployeeQualifications)
               {
                    if (existingEmp.EmployeeQualifications != null) {
                         var existingQ = existingEmp.EmployeeQualifications.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingQ != null)  {      // record exists, update it
                              _context.Entry(existingQ).CurrentValues.SetValues(item);
                              _context.Entry(existingQ).State = EntityState.Modified;
                         }  else {            //record does not exist, insert a new record
                              var newQ = new EmployeeQualification(existingEmp.Id, item.QualificationId, item.IsMain);
                              existingEmp.EmployeeQualifications.Add(newQ);
                              _context.Entry(newQ).State = EntityState.Added;
                         }
                    } else {
                         var newQ = new EmployeeQualification(existingEmp.Id, item.QualificationId, item.IsMain);
                         existingEmp.EmployeeQualifications?.Add(newQ);
                         _context.Entry(newQ).State = EntityState.Added;
                    }
               }

               foreach (var item in model.HrSkills)
               {
                    if (existingEmp.HrSkills !=null) {
                         var existingSk = existingEmp.HrSkills.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingSk != null) {      // record exists, update it
                              _context.Entry(existingSk).CurrentValues.SetValues(item);
                              _context.Entry(existingSk).State = EntityState.Modified;
                         } else {
                              var newSk = new EmployeeHRSkill(existingEmp.Id, item.CategoryId, item.IndustryId, item.SkillLevel);
                              existingEmp.HrSkills?.Add(newSk);
                              _context.Entry(newSk).State = EntityState.Added;
                         }
                    }  else {           //record does not exist, insert a new record
                         var newSk = new EmployeeHRSkill(existingEmp.Id, item.CategoryId, item.IndustryId, item.SkillLevel);
                         existingEmp.HrSkills?.Add(newSk);
                         _context.Entry(newSk).State = EntityState.Added;
                    }
               }

               foreach (var item in model.OtherSkills)
               {
                    if (existingEmp.OtherSkills !=null) {
                         var existingOSk = existingEmp.OtherSkills.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingOSk != null) {       // record exists, update it
                              _context.Entry(existingOSk).CurrentValues.SetValues(item);
                              _context.Entry(existingOSk).State = EntityState.Modified;
                         } else {
                              var newOSk = new EmployeeOtherSkill(existingEmp.Id, item.SkillDataId, item.SkillLevel, item.IsMain);
                              existingEmp.OtherSkills.Add(newOSk);
                              _context.Entry(newOSk).State = EntityState.Added;
                         }
                    }  else {            //record does not exist, insert a new record
                         var newOSk = new EmployeeOtherSkill(existingEmp.Id, item.SkillDataId, item.SkillLevel, item.IsMain);
                         existingEmp.OtherSkills?.Add(newOSk);
                         _context.Entry(newOSk).State = EntityState.Added;
                    }
               }

               _context.Entry(existingEmp).State = EntityState.Modified;

               var succeeded = await _context.SaveChangesAsync() > 0;

               if (!succeeded) return false;

               //check if employee is also registered as identity user
               var user = (await _userManager.FindByIdAsync(model.AppUserId.ToString()) );   //AppUser.Id is integer
               if (user == null )  //employees must have user identity
               {
                    user = await CreateAppUserForEmployee(model.Id, model.FirstName, model.Email, "");
                    existingEmp.AppUserId = user.Id;
                    _context.Entry(existingEmp).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
               }

               return succeeded;
          }

          public async Task<AppUser> CreateAppUserForEmployee(int employeeId, string knownas, string email, string password)
          {
               var user = new AppUser
                    {
                         UserType = "Employee",
                         DisplayName = knownas,
                         Email = email,
                         UserName = email,
                         loggedInEmployeeId = employeeId
                         //, Token = _tokenService.CreateToken
                    };
                    var result = await _userManager.CreateAsync(user, password == "" ? "Pa$$w0rd" : password);
               return user;
          }
          public async Task<ICollection<Employee>> AddNewEmployees(ICollection<EmployeeToAddDto> employees)
          {

               var emps = new List<Employee>();
               var empsDto = new List<EmployeeToAddDto>();

               AppUser user;

               foreach (var employee in employees)
               {
                    var email = employee.Email;
                    if (string.IsNullOrEmpty(email)) continue;
                    if (await _userManager.FindByEmailAsync(email) != null)
                    {
                         //return BadRequest(new ApiValidationErrorResponse { Errors = new[] { "Email address " + email + " is in use" } });
                         continue;
                    }

                    if (string.IsNullOrEmpty(employee.Password))
                    {
                         //return BadRequest(new ApiResponse(400, "Password essential"));
                         continue;
                    }


                    var qualifications = new List<EmployeeQualification>();
                    var hrskills = new List<EmployeeHRSkill>();
                    var otherSkills = new List<EmployeeOtherSkill>();
                    var employeeAddresses = new List<EmployeeAddress>();
                    var emp = new Employee(employee.AppUserId, employee.Gender ?? "M", employee.FirstName, employee.SecondName, employee.FamilyName,
                         employee.KnownAs, employee.DOB, employee.AadharNo, qualifications, employee.DOJ, employee.Department, 
                         employee.Position ?? "Not Available", employee.Email, hrskills, otherSkills, employeeAddresses);
                    //var emp = _mapper.Map<EmployeeToAddDto, Employee>(employee);
                    _unitOfWork.Repository<Employee>().Add(emp);
                    emps.Add(emp);

               }

               if (emps.Count == 0) return null;
               
               if (await _unitOfWork.Complete() == 0) return null;
               
               foreach(var emp in emps)
               {
                    user = await CreateAppUserForEmployee(emp.Id, emp.KnownAs, emp.Email, "");       //update employeeId later
                    //employee.AppUserId = user.Id;
                    emp.AppUserId = user.Id;
                    _unitOfWork.Repository<Employee>().Update(emp);
               }
               await _unitOfWork.Complete();
               return emps;               
          }

          public async Task<Pagination<EmployeeBriefDto>> GetEmployeePaginated(EmployeeSpecParams empParams)
          {
               var spec = new EmployeeSpecs(empParams);
               var countSpec = new EmployeeForCountSpecs(empParams);

               var totalItems = await _unitOfWork.Repository<Employee>().CountAsync(countSpec);
               var emps = await _unitOfWork.Repository<Employee>().ListAsync(spec);

               var data = _mapper.Map<IReadOnlyList<EmployeeBriefDto>>(emps);

               return new Pagination<EmployeeBriefDto>(empParams.PageIndex, empParams.PageSize, totalItems, data);
          }

          public async Task<Employee> GetEmployeeById(int id)
          {
               return await _context.Employees.Where(x => x.Id==id)
                    .Include(x => x.EmployeePhones)
                    .Include(x => x.EmployeeAddresses)
                    .Include(x => x.HrSkills)
                    .Include(x => x.OtherSkills)
                    .Include(x => x.EmployeeQualifications)
                    .FirstOrDefaultAsync();
          }
          
          public async Task<EmployeeDto> GetEmployeeFromIdAsync(int employeeId)
          {
               var emp = await _context.Employees.Where(x => x.Id == employeeId)
                    .Select(x => new
                    {
                         x.FirstName,
                         x.SecondName,
                         x.FamilyName,
                         x.KnownAs,
                         x.Position,
                         x.Email,
                         x.AppUserId,
                         x.EmployeePhones
                    }).FirstOrDefaultAsync();
               if (emp == null) return null;
               var empAppUser = await _userManager.FindByIdAsync(emp.AppUserId.ToString());
               var empusername = empAppUser == null ? "" : empAppUser.Email;
               return new EmployeeDto
               {
                    EmployeeName = emp.FirstName + " " + emp.SecondName + " " + emp.FamilyName,
                    KnownAs = emp.KnownAs,
                    Position = emp.Position,
                    //OfficialPhoneNo = emp.EmployeePhones?.Where(x => x.IsMain && x.IsOfficial && x.IsValid).Select(x => x.MobileNo).FirstOrDefault(),
                    OfficialMobileNo = emp.EmployeePhones?.Where(x => x.IsMain && x.IsValid && x.IsOfficial).Select(x => x.MobileNo).FirstOrDefault(),
                    OfficialEmailAddress = emp.Email,
                    AppUserId = emp.AppUserId,
                    UserName = empusername
               };
          }

          public async Task<int> GetEmployeeIdFromAppUserIdAsync(int appUserId)
          {
               return await _context.Employees.Where(x => x.AppUserId == appUserId).Select(x => x.Id).FirstOrDefaultAsync();
          }

          public async Task<ICollection<EmployeePosition>> GetEmployeePositions()
          {
               var c = await _context.Employees
                    .Select(x => x.Position).Distinct() .ToListAsync();
               var lsts = new List<EmployeePosition>();
               foreach(var lst in c)
               {
                    lsts.Add(new EmployeePosition{Name = lst});
               }
               return lsts;
          }

          public async Task<EmployeeDto> GetEmployeeBriefAsyncFromAppUserId(int appUserId)
          {
               var emp = await _context.Employees.Where(x => x.AppUserId == appUserId)
                    .Select(x => new { x.Id, x.Gender, x.FirstName, x.SecondName, x.FamilyName, x.KnownAs, x.Position, x.Email,
                         x.EmployeePhones })
                    .FirstOrDefaultAsync();
               if (emp != null)
               {
                    return new EmployeeDto
                    {
                         EmployeeId = emp.Id,
                         Gender = emp.Gender,
                         EmployeeName = emp.FirstName + " " + emp.SecondName + " " + emp.FamilyName,
                         Position = emp.Position,
                         KnownAs = emp.KnownAs,
                         UserName = emp.Email,
                         Email = emp.Email,
                         OfficialPhoneNo = emp.EmployeePhones.Where(x => x.IsOfficial && x.IsOfficial).Select(x => x.MobileNo).FirstOrDefault(),
                         OfficialEmailAddress = emp.Email,
                         AppUserId = appUserId
                    };
               }
               else
               {
                    return null;
               }
          }

          public async Task<EmployeeDto> GetEmployeeBriefAsyncFromEmployeeId(int id)
          {
               var emp = await _context.Employees.Where(x => x.Id == id)
                    .Select(x => new { x.AppUserId, x.KnownAs, x.FirstName, x.SecondName, x.FamilyName, x.Position, x.Email,
                         x.EmployeePhones, x.Gender })
                    .FirstOrDefaultAsync();
               if (emp != null)
               {
                    return new EmployeeDto
                    {
                         EmployeeId = id,
                         Gender = emp.Gender,
                         OfficialEmailAddress = emp.Email,
                         OfficialPhoneNo = emp.EmployeePhones.Where(x => x.IsOfficial && x.IsValid).Select(x => x.MobileNo).FirstOrDefault(),
                         OfficialMobileNo = emp.EmployeePhones.Where(x => x.IsOfficial && x.IsValid).Select(x => x.MobileNo).FirstOrDefault(),
                         EmployeeName = emp.FirstName + " " + emp.SecondName + " " + emp.FamilyName,
                         KnownAs = emp.KnownAs,
                         Position = emp.Position,
                         Email = emp.Email,
                         UserName = emp.Email,
                         AppUserId = emp.AppUserId
                    };
               }
               else
               {
                    return null;
               }
          }

          
          public async Task<string> GetEmployeeNameFromEmployeeId(int id)
          {
               return await _context.Employees.Where(x => x.Id == id).Select(x => x.FirstName + " " + x.FamilyName).FirstOrDefaultAsync();
          }
          public async Task<int> GetEmployeeIdFromEmail(string email)
          {
               return await _context.Employees.Where(x => x.Email.ToLower() == email.ToLower()).Select(x => x.Id).FirstOrDefaultAsync();
          }

          public async Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs() {
               
               var emps = await _context.Employees.Select(x => new {x.Id, x.KnownAs}).OrderBy(x => x.KnownAs).ToListAsync();
               var lst = new List<EmployeeIdAndKnownAsDto>();
               foreach(var emp in emps) {
                    lst.Add(new EmployeeIdAndKnownAsDto{Id=emp.Id, KnownAs = emp.KnownAs});
               }
               return lst;
          }

     
     }
}