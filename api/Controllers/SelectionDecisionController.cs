using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class SelectionDecisionController : BaseApiController
     {
          private readonly ISelectionDecisionService _service;
          private readonly IEmployeeService _empService;
          private readonly IMapper _mapper;
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmploymentService _employmentService;
          public SelectionDecisionController(ISelectionDecisionService service, IMapper mapper,
               UserManager<AppUser> userManager, IEmployeeService empService, IEmploymentService employmentService)
          {
               _empService = empService;
               _userManager = userManager;
               _mapper = mapper;
               _service = service;
               _employmentService = employmentService;
          }

          [Authorize]
          [HttpGet]
          public async Task<ActionResult<Pagination<SelectionDecision>>> GetSelectionDecisions(SelDecisionSpecParams selDecisionParams)
          {
               var loggedInDto = await GetLoggedInUserDto();
               
               var decs = await _service.GetSelectionDecisions(selDecisionParams);
               if (decs != null) return Ok(decs);
               return NotFound(new ApiResponse(404, "no records found"));
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpPost]
          public async Task<ActionResult<SelectionMsgsAndEmploymentsDto>> RegisterSelectionDecisions(ICollection<SelDecisionToAddDto> dtos)
          {
               var loggedInDto = await GetLoggedInUserDto();
              
               var decs = await _service.RegisterSelections(dtos, loggedInDto.LoggedInEmployeeId);

               if (decs != null) return Ok(decs);

               return BadRequest(new ApiResponse(400, "failed to update the selections"));
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditSelectionDecision(SelectionDecision selectionDecision)
          {
               return await _service.EditSelection(selectionDecision);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpDelete("{id}")]
          public async Task<ActionResult<bool>> DeleteSelectionDecision(int id)
          {
               return await _service.DeleteSelection(id);
          }

          [Authorize]
          [HttpGet("pendingseldecisions")]
          [Authorize]
          public async Task<ActionResult<Pagination<SelectionsPendingDto>>> SelectionDecisionPending([FromQuery] CVRefSpecParams selParams)
          {
               var data = await _service.GetPendingSelections(selParams);
               if (data==null && data.Count == 0) return NotFound(new ApiResponse(404, "No referral decisions found pending as of now"));
               
               return Ok(data);
               //return Ok(new Pagination<SelectionsPendingDto>(selParams.PageIndex, selParams.PageSize, data.Count, (IReadOnlyList<SelectionsPendingDto>)data));
          }
          
          [Authorize]
          [HttpGet("selectionstatus")]
          public async Task<ActionResult<ICollection<SelectionStatus>>> GetSelectionStatus()
          {
               var st = await _service.GetSelectionStatus();    
               return Ok(st);
          }

          //HttpGetAttribute()
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

          

     }
}