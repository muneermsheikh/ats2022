using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     public class DLForwardController : BaseApiController
     {
          private readonly IDLForwardService _dlfwdService;
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmployeeService _empService;
          public DLForwardController(IDLForwardService dlfwdService, UserManager<AppUser> userManager, IEmployeeService empService)
          {
               _empService = empService;
               _userManager = userManager;
               _dlfwdService = dlfwdService;
          }

          [Authorize(Roles = "DocumentControllerAdmin, HRSupervisor, HRExecutive, HRTrainee" )]
          [HttpPost]
          public async Task<ActionResult<bool>> ForwardDLToAgents(OrderItemsAndAgentsToFwdDto ItemsAndAgents)
          {
               if (ItemsAndAgents.Agents==null) return BadRequest(new ApiResponse(400, "bad request - object contains nulls"));
               var loggedInEmployeeId = 0;
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
               loggedInEmployeeId = loggedInUser == null ? 0 : await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               
               var succeeded = await _dlfwdService.ForwardDLToAgents(ItemsAndAgents, loggedInEmployeeId);

               if (succeeded) return Ok(true);

               return BadRequest(new ApiResponse(404, "Failed to forward the DL to agents"));
          }
     }
}