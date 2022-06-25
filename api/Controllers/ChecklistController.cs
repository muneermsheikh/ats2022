using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Interfaces;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text;
using core.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace api.Controllers
{
     public class ChecklistController : BaseApiController
     {
          private readonly IChecklistService _checklistService;
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmployeeService _empService;

          public ChecklistController(IChecklistService checklistService, UserManager<AppUser> userManager,
               IEmployeeService empService)
          {
               _empService = empService;
               _userManager = userManager;
               _checklistService = checklistService;

          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRTrainee")]
          [HttpGet("{candidateid}/{orderitemid}")]
          public async Task<ActionResult<ChecklistHR>> GetOrCreateNewChecklist(int candidateid, int orderitemid)
          {
               var loggedInUserDto = await GetLoggedInUserDto();
               //if (loggedInUserDto == null) return BadRequest(new ApiResponse(404, "this option requires log in"));

               var checklist = await _checklistService.GetOrCreateNewChecklistHR(candidateid, orderitemid, loggedInUserDto.LoggedInEmployeeId, loggedInUserDto.LoggedIAppUsername);
               
               if (checklist == null) return BadRequest(new ApiResponse(404, "failed to save the checklist data"));

               return Ok(checklist);
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRTrainee")]
          [HttpPut("checklisthr")]
          public async Task<ActionResult<List<string>>> EditChecklistHRAsync(ChecklistHRDto checklistHR)
          {
               var loggedInUserDto = await GetLoggedInUserDto();
               //if (loggedInUserDto == null) return BadRequest(new ApiResponse(404, "User not logged in"));
               
               var errorLists = await _checklistService.EditChecklistHR(checklistHR, loggedInUserDto);

               if (errorLists.Count==0 || errorLists==null) return Ok(errorLists);
               return BadRequest(errorLists);
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRTrainee")]
          [HttpGet("checklistid/{candidateid}/{orderitemid}")]
          public async Task<int> GetChecklistHRId(int candidateid, int orderitemid)
          {
               return await _checklistService.GetChecklistHRId(candidateid, orderitemid);
          }
          
          [Authorize]
          [HttpGet("checklisthr/{candidateid}/{orderitemid}")]
          public async Task<ActionResult<ChecklistHRDto>> GetChecklistHR(int candidateid, int orderitemid)
          {
               var loggedInUserDto = await GetLoggedInUserDto();
               //if (loggedInUserDto == null) return BadRequest(new ApiResponse(404, "this option requires log in"));

               var checklist = await _checklistService.GetChecklistHR(candidateid, orderitemid, loggedInUserDto);
               if (checklist == null) return BadRequest(new ApiResponse(404, "No data returned"));

               return Ok(checklist);
          }
          
          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpDelete("hrchecklist")]
          public async Task<ActionResult<bool>> DeleteChecklistHRAsync(ChecklistHRDto checklistHR)
          {
               var loggedInUserDto = await GetLoggedInUserDto();
               return await _checklistService.DeleteChecklistHR(checklistHR, loggedInUserDto);
          }

     //master data
          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpDelete("hrparameter")]
          public async Task<bool> DeleteChecklistHRDataAsync(ChecklistHRData checklistHRData)
          {
               return await _checklistService.DeleteChecklistHRDataAsync(checklistHRData);
          }

          //checklistHR - job card for HR Executives
          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpPost("newhrparameter/{checklist}/{mandatorytrue}")]
          public async Task<ChecklistHRData> AddChecklistHRParameter(string checklist, bool mandatorytrue)
          {
               return await _checklistService.AddChecklistHRParameter(checklist, mandatorytrue);
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpDelete]
          public async Task<bool> DeleteChecklistHRData(ChecklistHRData checklistHRData)
          {
               return await _checklistService.DeleteChecklistHRDataAsync(checklistHRData);
          }
          
          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpPut("hrchecklistdata")]
          public async Task<bool> EditChecklistHRDataAsync(ChecklistHRData checklistHRData)
          {
               return await _checklistService.EditChecklistHRDataAsync(checklistHRData);
          }

          [HttpGet("hrdata")]
          public async Task<IReadOnlyList<ChecklistHRData>> GetChecklistHRDataListAsync()
          {
               return await _checklistService.GetChecklistHRDataListAsync();
          }

          private async Task<LoggedInUserDto> GetLoggedInUserDto()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               if (loggedInUser == null) return null;
               var empId = loggedInUser == null ? 0 :   await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               var loggedInUserDto = new LoggedInUserDto{
                    LoggedIAppUsername = loggedInUser.UserName, LoggedInAppUserEmail=loggedInUser.Email, LoggedInAppUserId = loggedInUser.Id ,
                    LoggedInEmployeeId = empId
               };
               
               return loggedInUserDto;
          }
     }
}