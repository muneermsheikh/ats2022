using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Process;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize]     //(Policy = "ProcessEmployeeRole")]
     public class DeployController : BaseApiController
     {
          private readonly IDeployService _deployService;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IEmployeeService _empService;
          private readonly UserManager<AppUser> _userManager;
          public DeployController(IDeployService deployService, IUnitOfWork unitOfWork, 
                UserManager<AppUser> userManager, IEmployeeService empService)
          {
               _userManager = userManager;
               _empService = empService;
               _unitOfWork = unitOfWork;
               _deployService = deployService;
          }

          [Authorize(Roles = "Admin, DocumentControllerAdmin, DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpGet("pending")]
          public async Task<ActionResult<Pagination<DeploymentPendingDto>>> GetPendingDeployments([FromQuery]DeployParams depParam)
          {
     
               var ret = await _deployService.GetPendingDeployments(depParam);
               
               return Ok(ret);
          }


          [Authorize(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpPost("posts")]
          public async Task<ActionResult<ICollection<DeployAddedDto>>> AddDeploymentTransactions(ICollection<DeployPostDto> deployPostsDto)
          {
               var loggedInDto = await GetLoggedInUserDto();
               //if (loggedInDto == null) return BadRequest(new ApiResponse(401, "this option requires logged in User"));
               
               foreach(var dto in deployPostsDto)
               {
                    if(dto.CVRefId == 0 || dto.StageId==0 ) return BadRequest(new ApiResponse(402, "Deploy Id or Status not provided"));
                    if(dto.TransactionDate.Year < 2000) dto.TransactionDate = DateTime.Now;
               }

               var dep = await _deployService.AddDeploymentTransactions(deployPostsDto, loggedInDto.LoggedInEmployeeId);

               if (dep == null) return BadRequest(new ApiResponse(401, "Failed to register the deployment transaction"));

               return Ok(dep);
          }

          [Authorize(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditDeploymentTransaction(Deploy deploy)
          {
               return await _deployService.EditDeploymentTransaction(deploy);
          }

          [Authorize(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]          
          [HttpDelete]
          public async Task<ActionResult<bool>> DeleteDeploymentTransactions(Deploy deploy)
          {
               return await _deployService.DeleteDeploymentTransactions(deploy);
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

          [Authorize]
          [HttpGet("depStatus")]
          public async Task<ActionResult<ICollection<DeployStatusDto>>> GetDeploymentStatus()
          {
               var st = await _deployService.GetDeployStatuses();
               if(st==null) return NotFound(new ApiResponse(404, "No records found"));
               return Ok(st);
          }
     
          [HttpGet("{cvrefid}")]
          public async Task<ActionResult<CVReferredDto>> GetCVRefDto(int cvrefid)
          {
               var dto = await _deployService.GetDeploymentDto(cvrefid);
               if (dto == null) return NotFound(new ApiResponse(404, "Record not found"));
               return Ok(dto);
          }
     }
}