using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.HR;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     //[Authorize]
     public class CandidateAssessmentController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;

          private readonly ICandidateAssessmentService _candidateAssessService;
          private readonly IEmployeeService _empService;
          private readonly ICommonServices _commonServices;
          private readonly ITokenService _tokenService;
          public CandidateAssessmentController(ICandidateAssessmentService candidateAssessService,  ITokenService tokenService,
               UserManager<AppUser> userManager, IEmployeeService empService, 
               ICommonServices commonServices)
          {
               _userManager = userManager;
               _candidateAssessService = candidateAssessService;
               _empService = empService;
               _commonServices = commonServices;
               _tokenService = tokenService;
          }

          [Authorize(Roles ="HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpPost("assess/{requireReview}/{candidateId}/{orderItemId}")]
          //[Authorize(Policy = "HRExecutiveRole, HRSupervisorRole, HRManagerRole")]
          public async Task<ActionResult<CandidateAssessmentWithErrorStringDto>> AssessNewCandidate(bool requireReview, int candidateId, int orderItemId, DateTime dateAdded)
          {
               var userdto = await GetCurrentUser();
               int intEmployeeId = userdto.loggedInEmployeeId;
               var assessed = await _candidateAssessService.GetorCreateNewCandidateAssessment(requireReview, candidateId, orderItemId, intEmployeeId );
               if (!string.IsNullOrEmpty(assessed.ErrorString)) {
                    if(requireReview) {
                         return BadRequest(new ApiResponse(400, "Failed to create new Assessment object. " + assessed.ErrorString ));
                    } else {
                         return BadRequest(new ApiResponse(400, "Failed to shortlist the candidate. " + assessed.ErrorString));
                    }
               };
               return assessed;
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpGet("getorcreatenewcandidateassesment/{requireReview}/{candidateId}/{orderItemId}")]
          //[Authorize(Policy = "HRExecutiveRole, HRSupervisorRole, HRManagerRole")]
          public async Task<ActionResult<CandidateAssessment>> GetNewAssessmentCandidate(bool requireReview, int candidateId, int orderItemId, DateTime dateAdded)
          {
              var userdto = await GetLoggedInUserDto();
               int loggedInEmployeeId = await ApiUserId();

               var dto = await _candidateAssessService.GetorCreateNewCandidateAssessment(requireReview, candidateId, orderItemId, loggedInEmployeeId);
               if (!string.IsNullOrEmpty(dto.ErrorString)) {
                    return BadRequest(new ApiResponse(400, "Failed to create candidate assessment.  " + dto.ErrorString));
               }
               return Ok(dto.CandidateAssessment);
          }

          
          private async Task<int> Identityuserid()
          {
               var email = User.GetIdentityUserEmailId();
               var appuser = await _userManager.FindByEmailAsync(email);
               return appuser.Id;
               //return appuser.IdentityUser();
          }

          private async Task<int> ApiUserId()
          {
               var email = User.GetIdentityUserEmailId();
               if (string.IsNullOrEmpty(email)) return 10;
               return await _empService.GetEmployeeIdFromEmail(email);
          }
          
          [Authorize(Roles = "HRExecutive, HRSupervisor, HRManager")]
          [HttpPut("assess")]
          public async Task<ActionResult<string>> EditCandidateAssessment(CandidateAssessment candidateAssessment)
          {
               var userdto = await GetLoggedInUserDto();
               var loggedInEmpId = userdto.LoggedInEmployeeId;

               var msgs = await _candidateAssessService.EditCandidateAssessment(candidateAssessment, loggedInEmpId);
               if (!string.IsNullOrEmpty(msgs.ErrorString))
               {
                    return BadRequest(new ApiResponse(400, msgs.ErrorString));
               }
               else
               {
                    return Ok(true);
               }
          }

          [Authorize(Roles ="HRManager, HRSupervisor, HRExecutive")]
          [HttpDelete("assess/{assessmentid}")]
          public async Task<ActionResult<bool>> DeleteCandidateAssessment(int assessmentid )
          {
               if (!await _candidateAssessService.DeleteCandidateAssessment(assessmentid))
               {
                    return BadRequest(new ApiResponse(400, "failed to delete the candidate assessment"));
               }
               else
               {
                    return Ok(true);
               }
          }

          [Authorize(Roles ="HRManager, HRSupervisor, HRExecutive")]
          [HttpDelete("assessitem")]
          public async Task<ActionResult<bool>> DeleteCandidateAssessmentItem(CandidateAssessmentItem assessmentItem)
          {
               if (!await _candidateAssessService.DeleteCandidateAssessmentItem(assessmentItem))
               {
                    return BadRequest(new ApiResponse(400, "failed to delete the candidate assessment item"));
               }
               else
               {
                    return Ok();
               }
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpGet("{orderItemId}/{candidateId}")]
          public async Task<ActionResult<CandidateAssessment>> GetCandidateAssessment(int orderItemId, int candidateId)
          {
               var assessment = await _candidateAssessService.GetCandidateAssessment(candidateId, orderItemId);
               if (assessment != null) {
                    return Ok(assessment);
               } else {
                    return null;
               }
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpGet("assessmentandchecklist/{orderItemId}/{candidateId}")]
          public async Task<ActionResult<CandidateAssessmentAndChecklistDto>> GetCandidateAssessmentWithChecklist(int orderItemId, int candidateId)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               var loggedInEmployeeId = loggedInUser.loggedInEmployeeId;
               var assessment = await _candidateAssessService.GetCandidateAssessmentAndChecklist(candidateId, orderItemId, loggedInEmployeeId, loggedInUser.DisplayName);
               if (assessment != null) {
                    if(assessment.Assessed != null) assessment.Assessed.AssessedByName=loggedInUser.DisplayName;
                    return Ok(assessment);
               } else {
                    return null;
               }
          }

          
          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpGet("assessedandapproved")]
          public async Task<ActionResult<CandidateAssessedDto>> GetCandidateAssessedAndApproved()
          {
               var assessed = await _candidateAssessService.GetAssessedCandidatesApproved();
               if (assessed != null) {
                    return Ok(assessed);
               } else {
                    return null;
               }
          }

          private async Task<LoggedInUserDto> GetLoggedInUserDto()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               if (loggedInUser == null) return null;

               var empId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               var loggedInUserDto = new LoggedInUserDto
               {
                    LoggedIAppUsername = loggedInUser.UserName,
                    LoggedInAppUserEmail = loggedInUser.Email,
                    LoggedInAppUserId = loggedInUser.Id,
                    LoggedInEmployeeId = empId,
                    HasAdminPrivilege = User.IsInRole("Admin")
               };
               return loggedInUserDto;
          }

          private async Task<UserDto> GetCurrentUser()
          {
               var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
               if (email==null) email="munir@afreenintl.in";     // return null; // BadRequest("User email not found");
               var user = await _userManager.FindByEmailAsync(email);
               if (user==null) return null;  // BadRequest("User Claim not found");

               return new core.ParamsAndDtos.UserDto
               {
                    loggedInEmployeeId = user.loggedInEmployeeId,
                    Email = user.Email,
                    Token = await _tokenService.CreateToken(user),
                    DisplayName = user.DisplayName
               };
             
          }
          
     }
}