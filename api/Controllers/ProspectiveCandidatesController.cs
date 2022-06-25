using System.Linq;
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
     public class ProspectiveCandidatesController : BaseApiController
     {
          private readonly IProspectiveCandidateService _prospectiveService;
          private readonly RoleManager<AppRole> _roleManager;
          private readonly ITokenService _tokenService;
          private readonly IUserService _userService;
          private readonly UserManager<AppUser> _userManager;
          
          public ProspectiveCandidatesController(UserManager<AppUser> userManager,
               IProspectiveCandidateService prospectiveService, 
               IUserService userService, 
               ITokenService tokenService,
               RoleManager<AppRole> roleManager)
          {
               _userManager = userManager;
               _userService = userService;
               _tokenService = tokenService;
               _roleManager = roleManager;
               _prospectiveService = prospectiveService;
          }

          [Authorize]
          [HttpGet]
          public async Task<ActionResult<Pagination<ProspectiveCandidate>>> GetProspectiveCandidates([FromQuery]ProspectiveCandidateParams pParams)
          {
              var prospectives = await _prospectiveService.GetProspectiveCandidates(pParams);
              if (prospectives == null) return NotFound(new ApiResponse(404, "Not Found"));
              return Ok(prospectives);
          }

          [Authorize(Roles="CreateCV")]
          [HttpPost]
          public async Task<ActionResult<UserDto>> CreateCandidateFromProspectiveModel (ProspectiveCandidateAddDto prospectiveAddDto)
          {
               //if(await _userManager.FindByEmailAsync(prospectiveAddDto.Email) != null ) return BadRequest("cannot create new candidate record, as the email already exists");

               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               prospectiveAddDto.LoggedInUserId = loggedInUser.loggedInEmployeeId;

               var UserReturned = await _prospectiveService.ConvertProspectiveToCandidate(prospectiveAddDto);

               if (UserReturned == null) return BadRequest(new ApiResponse(402, "Failed to create Candidate record from the propective details"));

               return Ok(UserReturned);
          
          }
     }
}