using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class CVReviewsController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmployeeService _empService;
          private readonly ICVReviewService _cvreviewService;
          private readonly IVerifyService _verifyServices;
          public CVReviewsController(UserManager<AppUser> userManager, IVerifyService verifyServices, 
                IEmployeeService empService, ICVReviewService cvreviewService)
          {
               _verifyServices = verifyServices;
               _cvreviewService = cvreviewService;
               _empService = empService;
               _userManager = userManager;
          }

          [Authorize(Roles ="HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpPost("newCVReviews")]
          public async Task<ActionResult<CVRvw>> AddNewCVReview(ICollection<CVReviewSubmitByHRExecDto> cvsSubmitted)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));
               
               //check if the order item requires internal reviews, or cv can be directly sent to the customer
               
               var rvws = await _cvreviewService.AddNewCVReview(cvsSubmitted, loggedInDto);
               if (rvws == null || rvws.Count == 0) return BadRequest(new ApiResponse(404, "the CV Review result for the candidate/orderitem is not created"));
                   
               return Ok(rvws);
          }
          
          [Authorize(Roles ="HRSupervisor, HRManager, HRExecutive, HRTrainee, DocumentControllerAdmin")]
          [HttpGet("{candidateid}/{orderitemid}")]
          public async Task<ActionResult<CVRvw>> GetCVReview(int candidateid, int orderitemid)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));
               
               var rvw = await _cvreviewService.GetCVReview(candidateid, orderitemid);
               if (rvw == null) return BadRequest(new ApiResponse(404, "the CV Review result for the candidate/orderitem is not created"));
               if(!User.UserHasTheRole("Admin") && rvw.HRExecutiveId != loggedInDto.LoggedInEmployeeId)
                    return Unauthorized("The user must have admin role or be the user who created the CV Review file");

               return Ok(rvw);
          }

          [Authorize(Roles = "HRExecutive, HRTrainee")]
          [HttpPost("submitToSup")]
          public async Task<ActionResult<ICollection<EmailMessage>>> SubmitCVToSupervisorForReview(ICollection<CVReviewSubmitByHRExecDto> cvsSubmittedDto)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));

               var msgs = await _cvreviewService.AddNewCVReview(cvsSubmittedDto, loggedInDto);
               if (msgs == null || msgs.Count == 0) return BadRequest(new ApiResponse(400, "No tasks could be created"));
               return Ok(msgs);
          }

          [Authorize(Roles ="HRExecutive, HRTrainee")]
          [HttpDelete("hrsup/{id}")]
          public async Task<ActionResult<bool>> DeleteCVSubmittedToHRSup(int id)
          {
               var loggedInDto = await GetLoggedInUserDto();
               //check if the user is the one that created the cv review to sup record
               if (! await _cvreviewService.UserIsOwnerOfCVReviewBySupObject(id, loggedInDto.LoggedInEmployeeId))
                    return BadRequest(new ApiResponse(404, "Only the user that created the CV Review record can delete it"));
               return await _cvreviewService.DeleteCVSubmittedToHRSupForReview(id);
          }

          [Authorize(Roles = "HRSupervisor")]
          [HttpPost("sup")]
          public async Task<ActionResult<ICollection<EmailMessage>>> CVReviewByHRSup(ICollection<CVReviewBySupDto> cvsSubmittedDto)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));

               var msgs =  await _cvreviewService.CVReviewByHRSup(loggedInDto, cvsSubmittedDto);
               if (msgs == null || msgs.Count ==0) return BadRequest(new ApiResponse(400, "failed to submit the cv for review"));

               return Ok(msgs);
          }


          [Authorize(Roles ="HRManager")]
          [HttpPost("hrm")]
          public async Task<ActionResult<ICollection<EmailMessage>>> CVsReviewByHRM(ICollection<CVReviewByHRMDto> cvsReviewed)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));

               var msgs =  await _cvreviewService.CVReviewByHRM(loggedInDto, cvsReviewed);
               if (msgs == null || msgs.Count ==0) return BadRequest(new ApiResponse(400, "failed to submit the cv for review"));

               return Ok(msgs);
          }

          [Authorize(Roles = "HRSupervisor")]
          [HttpDelete("submittedtohrm/{id}")]
          public async Task<ActionResult<bool>> DeleteCVSubmittedToHRM(int id)
          {
               var loggedInDto = await GetLoggedInUserDto();
               //check if the user is the one that created the cv review to sup record
               if (! await _cvreviewService.UserIsOwnerOfCVReviewBySupObject(id, loggedInDto.LoggedInEmployeeId))
                    return BadRequest(new ApiResponse(404, "Only the user that created the CV Review record can delete it"));
               return await _cvreviewService.DeleteCVSubmittedToHRSupForReview(id);
          }
         
          [HttpGet("reviewsofitemid/{orderitemid}")]
          public async Task<ICollection<CVRvw>> GetCVReviewsOfAnOrderItemId(int orderitemid)
          {
               return await _cvreviewService.GetCVReviews(orderitemid);
          }

          [Authorize]
          [HttpGet("PendingRvwsOfLoggedInUser")]
          public async Task<ActionResult<ICollection<CVReviewsPendingDto>>> GetPendingReviewsOfLoggedInUser()
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return BadRequest(new ApiResponse(400, "this option requires logged in User"));
               var pendings = await _cvreviewService.PendingCVReviewsByUserIdAsync(loggedInDto.LoggedInEmployeeId);
               if (pendings==null || pendings.Count == 0) return NotFound(new ApiResponse(400, "The logged in User does not have any pending CV Reviews"));
               return Ok(pendings);
          }

          [Authorize]
          [HttpGet("PendingRvws")]
          public async Task<ActionResult<ICollection<CVReviewsPendingDto>>> GetPendingReviews()
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return Unauthorized(new ApiResponse(401, "this option requires logged in User"));
               //if (!loggedInDto.HasAdminPrivilege) return Unauthorized(new ApiResponse(404, "Only admin role has access to this feature"));
               var pendings = await _cvreviewService.PendingCVReviews();
               if (pendings==null || pendings.Count == 0) return NotFound(new ApiResponse(400, "No reviews pending"));
               return Ok(pendings);

          }

          private async Task<LoggedInUserDto> GetLoggedInUserDto()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               if (loggedInUser == null) return null;
               
               var empId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               var loggedInUserDto = new LoggedInUserDto{
                    LoggedIAppUsername = loggedInUser.UserName, LoggedInAppUserEmail=loggedInUser.Email, LoggedInAppUserId = loggedInUser.Id,
                    LoggedInEmployeeId = empId, HasAdminPrivilege = User.IsInRole("Admin")
               };
               return loggedInUserDto;
          }

     }
}