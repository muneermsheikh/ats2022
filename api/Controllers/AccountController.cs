using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTOs;
using api.Errors;
using api.Extensions;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.ParamsAndDtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using core.Params;
using infra.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Net.Http;
using core.Entities.Attachments;
using System.Security.Claims;

namespace api.Controllers
{
     public class AccountController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;
          private readonly SignInManager<AppUser> _signInManager;
          private readonly ITokenService _tokenService;
          private readonly IMapper _mapper;
          private readonly IUserService _userService;
          private readonly AppIdentityDbContext _identityContext;
          private readonly ITaskService _taskService;
          private readonly ITaskControlledService _taskControlledService;
          private readonly IEmployeeService _empService;
          private readonly IConfiguration _config;
          private readonly RoleManager<AppRole> _roleManager;
          
          public AccountController(
               UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
               ITokenService tokenService, RoleManager<AppRole> roleManager,
               ITaskControlledService taskControlledService,
               IMapper mapper, IUserService userService, AppIdentityDbContext identityContext,
               ITaskService taskService, IEmployeeService empService, IConfiguration config)
          {
               _userService = userService ?? throw new ArgumentNullException(nameof(userService));
               _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
               _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
               _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
               _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
               _identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
               _taskControlledService=taskControlledService;
               _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
               _empService = empService ?? throw new ArgumentNullException(nameof(empService));
               _config = config ?? throw new ArgumentNullException(nameof(config));
               _roleManager = roleManager;
          }


          //[Authorize]
          [HttpGet]
          public async Task<ActionResult<UserDto>> GetCurrentUser()
          {
               
               /*
               var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
               if (email==null) return BadRequest("User email not found");
               var user = await _userManager.FindByEmailAsync(email);
               if (user==null) return BadRequest("User Claim not found");
               */
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               if (loggedInUser==null) return BadRequest("User email not found");
               return new UserDto
               {
                    loggedInEmployeeId = loggedInUser.loggedInEmployeeId,
                    Email = loggedInUser.Email,
                    Token = await _tokenService.CreateToken(loggedInUser),
                    DisplayName = loggedInUser.DisplayName 
               };  
          }
          

          [HttpGet("emailexists")]
          public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }
          
          [HttpGet("ppexists")]
          public async Task<ActionResult<string>> CheckPPNumberExistsAsync([FromQuery] string ppnumber)
          {
               var pp = await _userService.CheckPPNumberExists(ppnumber);
               return pp;
          }
          
          
          [HttpGet("aadahrexists/{aadharno}")]
          public async Task<ActionResult<bool>> CheckAadharNoExistsAsync([FromQuery] string aadharno)
          {
               return await _userService.CheckAadharNoExists(aadharno);
          }

          
          [HttpPost("login")]
          public async Task<ActionResult<core.ParamsAndDtos.UserDto>> Login(LoginDto loginDto)
          {
               //var user = await _userManager.FindByEmailAsync(loginDto.Email);
               var user = await _userManager.Users.Where(x => x.Email == loginDto.Email)
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync();
               if (user == null) return Unauthorized(new ApiResponse(401));
               var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
               if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
          
               //authorization
               var claims = new List<Claim>();
               claims.Add(new Claim("username", user.UserName));
               claims.Add(new Claim("displayname", user.DisplayName));
               
               // Add roles as multiple claims
               if (user.UserRoles != null) {
                    foreach(var role in user.UserRoles) 
                    {
                         claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                    }
               }
               // Optionally add other app specific claims as needed
               //claims.Add(new Claim("UserState", UserState.ToString()));
               var loggedInEmployeeId=user.loggedInEmployeeId;
               if(user.loggedInEmployeeId == 0) {
                    loggedInEmployeeId = await _empService.GetEmployeeIdFromAppUserIdAsync(user.Id);
               }
          
               //var taskParams = new TaskParams{TaskOwnerId = loggedInEmployeeId, AssignedToId = loggedInEmployeeId, TaskStatus = "Open"};
               var tasksOfLoggedInUser = new List<TaskDashboardDto>();
               if(loggedInEmployeeId != 0) {
                    tasksOfLoggedInUser = (List<TaskDashboardDto>)await _taskService.GetDashboardTasksOfLoggedInUser(loggedInEmployeeId);
               }
               
               var userdto = new core.ParamsAndDtos.UserDto
               {
                    loggedInEmployeeId = loggedInEmployeeId,
                    dashboardTasks = tasksOfLoggedInUser,
                    Email = user.Email,
                    Token = await _tokenService.CreateToken(user),
                    DisplayName = user.DisplayName
               };

               return userdto;
          }

          [Authorize]
          [HttpGet("users")]
          public async Task<ActionResult<ICollection<UserDto>>> GetUsers (AppUserSpecParams userParams)
          {
               var users = await _userManager.Users.ToListAsync();
               return  Ok(_mapper.Map<ICollection<UserDto>>(users));
          }

          
          //registers individuals. For customers and vendors, it will register the users for customers that exist
          //the IFormFile collection has following prefixes to filenames:
          //pp: passport; ph: photo, ec: educational certificates, qc: qualification certificates
          [HttpPost("register")]
          public async Task<ActionResult<core.ParamsAndDtos.UserDto>> Register(RegisterDto registerDto)  //, ICollection<IFormFile> UserFormFiles )
          {

               //check if the entity already exists - check fields that have unique values - pp, aadhar, 
               var errorstring = await _userService.CheckIfObjectExists(registerDto);
               if(!string.IsNullOrEmpty(errorstring)) {
                    return BadRequest(new ApiResponse(404, "the object already exists: " + errorstring));
               }

               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               string phoneno="";
               var objPP = new UserPassport();
               var lstPP = new List<UserPassport>();
               
               switch (registerDto.UserType.ToLower())
               {
                    case "candidate":
                         registerDto.UserRole = "Candidate";
                         if (registerDto.UserPhones != null && registerDto.UserPhones.Count() > 0)
                         {
                              foreach (var ph in registerDto.UserPhones)
                              {
                                   if (string.IsNullOrEmpty(ph.MobileNo)) return BadRequest(new ApiResponse(400, "mobile no cannot be blank"));
                              }
                         }
                         phoneno = registerDto.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault();
                         objPP = string.IsNullOrEmpty(registerDto.PpNo) 
                              ? null 
                              : new UserPassport(registerDto.PpNo, registerDto.Nationality, registerDto.PPValidity);
                         if(objPP != null) {
                              lstPP.Add(objPP);
                              registerDto.UserPassports = lstPP;
                         }
                         break;

                    case "employee":
                         if(loggedInUser==null) return Unauthorized("Log-in required!");
                         if(string.IsNullOrEmpty(registerDto.AadharNo)) return BadRequest(new ApiResponse(400, "for employees, Aadhar number is mandatory"));
                         break;
                    case "official":
                         if(loggedInUser==null) return Unauthorized("Log-in required!");
                         if((int)registerDto.CompanyId==0) return BadRequest(new ApiResponse(400, "For officials, customer Id is essential"));
                         break;
                    default:
                         break;
               }
               
               var userForDto = await _userManager.FindByEmailAsync(registerDto.Email);
               if (userForDto ==null) {
                    userForDto = new AppUser
                    {
                         UserType = registerDto.UserType,
                         DisplayName = registerDto.KnownAs, // registerDto.DisplayName,
                         KnownAs = registerDto.KnownAs,
                         Gender = registerDto.Gender,
                         Email = registerDto.Email,
                         UserName = registerDto.Email
                    };
                    registerDto.DisplayName = registerDto.DisplayName ?? userForDto.DisplayName;
                    registerDto.PlaceOfBirth = registerDto.PlaceOfBirth ?? "";
                    userForDto.PhoneNumber = phoneno;

                    var result = await _userManager.CreateAsync(userForDto, registerDto.Password);
                    if (!result.Succeeded) return BadRequest(result.Errors);
               }

               int loggedInEmployeeId = loggedInUser == null ? 0 : loggedInUser.loggedInEmployeeId;
               registerDto.LoggedInAppUserId = loggedInEmployeeId;

               //validate passport obj
               var objAddress = registerDto.EntityAddresses;
                    
               //the plain dto object to return, irrespective of type of user, i.e. whether candidate, employee or customer
               var userDtoToReturn = new core.ParamsAndDtos.UserDto
               {
                    DisplayName = userForDto.DisplayName,
                    Token = await _tokenService.CreateToken(userForDto),
                    Email = userForDto.Email
               };

               //user registered. 


               registerDto.AppUserId = userForDto.Id;

                    //*** flg not working..
                    /*
                    if (registerDto.UserPhones != null)
                    {    //ensure no duplicate user phones in the collection
                         var qry = (from p in registerDto.UserPhones
                                   group p by p.MobileNo into g
                                   where g.Count() > 1
                                   select g.Key);
                         if (qry != null) registerDto.UserPhones = null;     //disallow if any duplicate numbers
                    }
                    */

               //depending upon usertype, create other entities
                    switch (registerDto.UserType.ToLower())
                    {
                         case "candidate":
                              //var userCreated = await _userService.CreateCandidateAsync(registerDto, UserFormFiles, loggedInEmployeeId);
                              var cand = await _userService.CreateCandidateAsync(registerDto, loggedInEmployeeId);
                              if (cand != null) userDtoToReturn.loggedInEmployeeId=cand.Id;
                              break;
                         case "customerofficial":
                         case "vendorofficial":
                              var off = await _userService.CreateCustomerOfficialAsync(registerDto);
                              if (off != null) userDtoToReturn.loggedInEmployeeId=off.Id;
                              break;
                         default:
                              break;
                    }
               //return
                    
                    return userDtoToReturn;
               /*
               */
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpPost("registeremployee")]
          public async Task<ActionResult<core.ParamsAndDtos.UserDto>> RegisterEmployee(RegisterEmployeeDto registerDto )
          {
               if(string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.AadharNo)) return BadRequest("Email ID and Aadhar Card both are mandatory to register an employee");
               
               //var loggedInUser = await GetCurrentUser();   // _userManager.FindByEmailFromClaimsPrinciple(User);

               //if (loggedInUser == null) return BadRequest(new ApiResponse(401, "Unauthorized"));
               //registerDto.LoggedInAppUserId = loggedInUser.
                          
               //int loggedInEmployeeId = loggedInUser == null ? 0 : await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               //check if user email already on record
               if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
               {
                    return new BadRequestObjectResult(new ApiValidationErrorResponse { Errors = new[] { "Email address is in use" } });
               }

               //for customer and vendor official, customer Id is mandatory
                    if (string.IsNullOrEmpty(registerDto.AadharNo))
                    {
                         return BadRequest(new ApiResponse(400, "for employees, Aadhar number is mandatory"));
                    }

                    if (registerDto.EmployeePhones != null && registerDto.EmployeePhones.Count() > 0)
                    {
                         foreach (var ph in registerDto.EmployeePhones)
                         {
                              if (string.IsNullOrEmpty(ph.MobileNo)) return BadRequest(new ApiResponse(400, "mobile no cannot be blank"));
                         }
                    }

               //update address 
               //registerDto.Address.
               //create and save AppUser IdentityObject
                    var user = new AppUser
                    {
                         UserType = "Employee",
                         DisplayName = registerDto.KnownAs, // registerDto.DisplayName,
                         //Address = registerDto.Address,
                         
                         Email = registerDto.Email,
                         UserName = registerDto.Email
                    };
                    registerDto.DisplayName = registerDto.DisplayName ?? user.DisplayName;
                    registerDto.PlaceOfBirth = registerDto.PlaceOfBirth ?? "";
                    var result = await _userManager.CreateAsync(user, registerDto.Password);
                    if (!result.Succeeded) return BadRequest(result.Errors);

                    if(!string.IsNullOrEmpty(registerDto.UserRole)) {
                         var roleResult = await _userManager.AddToRoleAsync(user, registerDto.UserRole);
                         if (!result.Succeeded) return BadRequest(result.Errors);
                    }
               //the plain dto object to return, irrespective of type of user, i.e. whether candidate, employee or customer

                    //var userAdded = await _userManager.FindByEmailAsync(registerDto.Email);
                    //no need to retreive obj from DB - the object user can be used for the same
                    var userAdded = user;
               //user registered. 

               //now save the objects in DataContext database
                    registerDto.AppUserId = userAdded.Id;
                    //*** flg not working..
                    /*
                    if (registerDto.UserPhones != null)
                    {    //ensure no duplicate user phones in the collection
                         var qry = (from p in registerDto.UserPhones
                                   group p by p.MobileNo into g
                                   where g.Count() > 1
                                   select g.Key);
                         if (qry != null) registerDto.UserPhones = null;     //disallow if any duplicate numbers
                    }
                    */

                    var added = await _userService.CreateEmployeeAsync(registerDto);
                    var userDtoToReturn = new core.ParamsAndDtos.UserDto
                         {
                              loggedInEmployeeId = added.Id,
                              DisplayName = user.DisplayName,
                              Token = await _tokenService.CreateToken(user),
                              Email = user.Email
                         };
               //return
                    return userDtoToReturn;
               /*
               */
          }

         private async Task<string> WriteFile(IFormFile file)
          {
               //bool isSaveSuccess = false;
               string fileName;
               try
               {
                    var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                    if (!Directory.Exists(pathBuilt))
                    {
                         Directory.CreateDirectory(pathBuilt);
                    }

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                         await file.CopyToAsync(stream);
                    }

                    //isSaveSuccess = true;
               }
               catch (Exception e)
               {
                    return e.Message;
               }

               return "";
          }


          [Authorize(Roles ="Admin, HRManager")]
          [HttpDelete("user/{useremail}")]
          public async Task<ActionResult<bool>> DeleteIdentityUser (string useremail)
          {
               var user = await _userManager.FindByEmailAsync(useremail);
               if (user==null) {
                    return BadRequest(new ApiResponse(400, "no user with the selected email exists"));
               }
               var result = await _userManager.DeleteAsync(user);

               return result.Succeeded;
          }
     
          private async Task<bool> UserExists(string username)
          {
               return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
          }
     }
}