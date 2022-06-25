using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
     public class AdminController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;
          private readonly RoleManager<AppRole> _roleManager;
          public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
          {
               _roleManager = roleManager;
               _userManager = userManager;
          }

          [Authorize(Roles = "Admin")]
          [HttpGet("photos-to-moderate")]
          public ActionResult GetPhotosForModeration()
          {
               return Ok("Admins or moderators can see this");
          }

     /*
          
          [Authorize]
          [HttpGet("users-with-roles")]
          public async Task<ActionResult> GetUsersWithRoles()
          {
               var users = await _userManager.Users
                   .Include(x => x.UserRoles)
                   .ThenInclude(x => x.Role)
                   .OrderBy(x => x.UserName)
                   .Select(s => new
                   {
                        s.Id,
                        UserName = s.UserName,
                        Roles = s.UserRoles.Select(r => r.Role.Name).ToList()
                   })
                   .ToListAsync();
               if (users == null) return NotFound(new ApiResponse(401, "No users found with Roles"));
               return Ok(users);
          }
     */
     

          [Authorize(Roles="Admin, HRManager" )]
          [HttpPost("edit-roles/{useremail}")]
          public async Task<ActionResult> EditRoles(string useremail, [FromQuery] string roles)
          {
               var lst = roles.Split(",").ToArray();
               var selectedRoles = new List<string>();
               foreach (var item in lst)
               {
                    if (await _roleManager.RoleExistsAsync(item))
                    {
                         selectedRoles.Add(item.Trim());
                    }
               }

               if (selectedRoles.Count() == 0) return BadRequest(new ApiResponse(404, "none of the roles exist in Identity Roles"));

               var user = await _userManager.FindByEmailAsync(useremail);
               if (user == null) return NotFound("Could not find user");
               var userRoles = await _userManager.GetRolesAsync(user);

               //IdentityResult result;
               var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
               if (!result.Succeeded) return BadRequest("Failed to add to roles");
               result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

               if (!result.Succeeded) return BadRequest("Failed to remove from roles");

               return Ok(await _userManager.GetRolesAsync(user));
          }

          [Authorize(Roles ="Admin, HRManager")]
          [HttpPut("userrole/{userEmail}/{oldRoleName}/{newRoleName}")]
          public async Task<ActionResult<bool>> EditUserRole(string userEmail, string oldRoleName, string newRoleName)
          {
               var user = await _userManager.FindByEmailAsync(userEmail);
               if (user == null)
               {
                    return BadRequest(new ApiResponse(400, "no user with the selected email exists"));
               }
               var roleExists = await _roleManager.RoleExistsAsync(newRoleName);
               if (!roleExists) return BadRequest(new ApiResponse(400, "the role " + newRoleName + " does not exist"));

               var roleAdded = await _userManager.RemoveFromRoleAsync(user, oldRoleName);
               if (roleAdded.Succeeded) await _userManager.AddToRoleAsync(user, newRoleName);

               return roleAdded.Succeeded;
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpGet("userswithgivenrole/{rolename}")]
          public async Task<ActionResult<IReadOnlyList<AppUser>>> GetIdentityUsersWithARole(string roleName)
          {
               var users = await _userManager.GetUsersInRoleAsync(roleName);
               if (users == null) return NotFound(new ApiResponse(404, "No users found with role '" + roleName + "'"));
               return Ok(users);
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpGet("userwithroles/{useremail}")]
          public async Task<ActionResult<IReadOnlyList<AppUserRole>>> GetIdentityUserWithRoles(string useremail)
          {
               var user = await _userManager.FindByEmailAsync(useremail);
               if (user == null) return NotFound(new ApiResponse(404, "User not found"));
               var roles = await _userManager.GetRolesAsync(user);

               return Ok(roles);
          }

          [Authorize]
          [HttpGet("userhastherole/{useremail}/{rolename}")]
          public async Task<ActionResult<bool>> UserHasTheRole(string useremail, string roleName)
          {
               var user = await _userManager.FindByEmailAsync(useremail);
               if (user == null) return NotFound(new ApiResponse(404, "user not found"));
               return await _userManager.IsInRoleAsync(user, roleName);
          }

          [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpGet("deleteuserrole/{useremail}/{rolename}")]
          public async Task<ActionResult<bool>> DeleteUserRole(string useremail, string roleName)
          {
               var user = await _userManager.FindByEmailAsync(useremail);
               if (user == null) return NotFound(new ApiResponse(404, "user not found"));
               var result = await _userManager.RemoveFromRoleAsync(user, roleName);
               return result.Succeeded;
          }

               
//Roles
          [Authorize]
          [HttpGet("identityroles")]
          public async Task<ActionResult<IReadOnlyList<String>>> GetIdentityRoles()
          {
               var iroles =  await _roleManager.Roles.OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
               return iroles;
          }

          [Authorize(Roles ="Admin, HRManager")]
          [HttpPut("role/{existingRoleName}/{newRoleName}")]
          public async Task<ActionResult<bool>> EditRole(string existingRoleName, string newRoleName)
          {
               var role = await _roleManager.FindByNameAsync(existingRoleName);
               if (role==null) return BadRequest(new ApiResponse(400, "The requested role does not exist"));
               role.Name=newRoleName;
               if (await _roleManager.UpdateAsync(role) == null) {
                    return BadRequest(new ApiResponse(404, "failed to update the role " + existingRoleName));
               } else {
                    return Ok();
               }
          }

          [Authorize(Roles ="Admin, HRManager")]
          [HttpPost("role/{newRole}")]
          public async Task<ActionResult<bool>> AddNewRole(string newRole)
          {
                var roleExists = await _roleManager.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new AppRole{Name = newRole});
                    if (!result.Succeeded) return BadRequest("failed to add role");
                    return Ok(true);

                } else {
                     return BadRequest(new ApiResponse(404, "the role '" + newRole + "' already exists"));
                }
          }

          [Authorize(Roles ="Admin, HRManager")]
          [HttpDelete("role/{rolename}")]
          public async Task<ActionResult<bool>> DeleteIdentityRole(string rolename)
          {
               var role = await _roleManager.FindByNameAsync(rolename);
               if (role==null) return NotFound(new ApiResponse(404, "Role not found"));

               var result = await _roleManager.DeleteAsync(role);

               return result.Succeeded;
          }

          //[Authorize(Roles = "Admin")]
          //[Authorize (Policy = "RequireAdminRole")]
          [HttpGet("users-with-roles")]
          public async Task<ActionResult> GetUsersWithRoles()
          {
               var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    DisplayName = u.DisplayName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

               return Ok(users);
          }

        [Authorize(Roles= "Admin")]
        [HttpPost("edit-roles")]
        public async Task<ActionResult> EditUserRoles([FromQuery]string roles)
        {
               //CSV, first part is user name
               var selectedRoles = roles.Split(",").ToArray();
               var username = selectedRoles[0];
               selectedRoles = selectedRoles.Where((source, index) =>index != 0).ToArray();
               //var user = await _userManager.FindByNameAsync(username);
               var user = await _userManager.FindByEmailAsync(username);

               if (user == null) return NotFound("Could not find user");

               var userRoles = await _userManager.GetRolesAsync(user);

               var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

               if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }
    
     }
}