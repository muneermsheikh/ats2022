using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.Admin;
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
    [Authorize]
    public class UserHistoryController : BaseApiController
    {
        private readonly IUserHistoryService _userHistoryService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeService _empService;
        public UserHistoryController(IUserHistoryService userHistoryService, UserManager<AppUser> userManager, IEmployeeService empService)
        {
            _empService = empService;
            _userManager = userManager;
            _userHistoryService = userHistoryService;
        }

 
        [HttpGet("byhistoryid/{historyid}")]
        public async Task<ActionResult<UserHistoryDto>> GetUserHistoryDataByCandidateId(int historyid)
        {
            
            var data = await _userHistoryService.GetHistoryFromHistoryId(historyid);
            if (data == null) return NotFound("No record found with selected id");
            if (data != null) return Ok(data);

            return NotFound(new ApiResponse(400, "Your search parameters did not yield any result"));
        }

        [HttpGet("bycandidateid/{candidateid}")]
        public async Task<ActionResult<UserHistory>> GetUserHistoryByCandidateId(int candidateid)
        {
            var cParams = new UserHistoryParams();
            cParams.PersonType="candidate";
            cParams.PersonId = candidateid;
            
            var d = await GetCandidateHistoryFromParams(cParams);
            
            return d.Value;
            
        }

        [HttpGet("prospective/{prospectiveId}")]
        public async Task<ActionResult<UserHistory>> GetUserHistoryByProspectiveCandidateId(int prospectiveId)
        {
            var cParams = new UserHistoryParams();
            cParams.PersonType="prospective";
            cParams.PersonId = prospectiveId;
            
            var d = await GetCandidateHistoryFromParams(cParams);
            
            return d.Value;
            
        }

  
        [HttpGet("dto")]
        public async Task<ActionResult<UserHistory>> GetCandidateHistoryFromParams([FromQuery]UserHistoryParams histParams)
        {
            string err="";
            var hist = new UserHistory();

            if (histParams.PersonType == "prospective") {
                hist = await _userHistoryService.GetOrAddUserHistoryByParams(histParams);
                if (hist==null) return BadRequest(new ApiResponse(400, "failed to retrieve/create history record"));
                return hist;
            }
            var ph = histParams.MobileNo;
            if (!string.IsNullOrEmpty(ph)) {
                if(ph.Substring(0,4) == "0091") ph=ph.Substring(4);
                if(ph.Substring(0,3) == "+91") ph=ph.Substring(3);
                if(ph.Substring(0,1)=="0") ph=ph.Substring(1);
                int l=ph.Length;
                if (l < 10 || l > 15) {
                    ph="";
                    err="mobile no. should be 10 to 15 digits,including country codes";
                }           
            }

            if (!string.IsNullOrEmpty(histParams.MobileNo) &&  string.IsNullOrEmpty(ph)) return BadRequest(new ApiResponse(400, "Invalid mobile no"));
            if(histParams.CreateNewIfNull && (string.IsNullOrEmpty(ph) || string.IsNullOrEmpty(histParams.PersonName) )) 
                return BadRequest(new ApiResponse(404,"when create new record if set to true, then phone number and person name both should be provided"));

            if (!histParams.ApplicationNo.HasValue && string.IsNullOrEmpty(histParams.EmailId) &&
                string.IsNullOrEmpty(ph) && !histParams.PersonId.HasValue) return BadRequest(new ApiResponse(404, "The search object empty" + err));

            histParams.MobileNo=ph;
            if(string.IsNullOrEmpty(histParams.PersonType)) histParams.PersonType="candidate";

            hist = await _userHistoryService.GetOrAddUserHistoryByParams(histParams);
            if (hist==null) return BadRequest(new ApiResponse(400, "failed to retrieve/create history record"));
            return hist;
        }

        [HttpPost("newusercontact")]
        public async Task<ActionResult<UserHistory>> AddNewUserContact(UserHistory userContact)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
            if (loggedInUser == null) return Unauthorized("Access allowed to authorized loggin user only");
            
            //if(userContact.DateOfContact.Year < 2000) userContact.DateOfContact = DateTime.Now;

            var empId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
            if (empId == 0) return Unauthorized("Employee Id not on record");

            if (userContact.PersonId == 0 ) {
                return BadRequest(new ApiResponse(400, "Either Candidate Id or customer official Id should be provided"));
            }
            return await _userHistoryService.AddUserContact(userContact);
        }

        
        [HttpDelete("{userContactId}")]
        public async Task<bool> DeleteUserContactById(int userContactId)
        {
            return await _userHistoryService.DeleteUserContactById(userContactId);
        }


        [HttpGet("contactresults")]
        public async Task<ActionResult<ICollection<ContactResult>>> GetContactResults()
        {
            var results = await _userHistoryService.GetContactResults();
            if (results != null ) return Ok(results);
            return BadRequest(new ApiResponse(404, "No contact result data available"));
        }
        
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UserHistoryReturnDto>> UpdateContactHistory(UserHistory userhistory)
        {
            var returnDto = new UserHistoryReturnDto();
            if (string.IsNullOrEmpty(userhistory.PersonName) && userhistory.PersonId == 0 ) {
                return BadRequest(new ApiResponse(404, "Person Id and name not provided"));
            } else if (userhistory.Id==0) {
                return BadRequest(new ApiResponse(404, "UserHistory Id not available"));
            }

            var dto = await GetLoggedInUserDto();

            returnDto = await _userHistoryService.EditContactHistory(userhistory, dto);
            if (!returnDto.Succeeded) return BadRequest(new ApiResponse(402, "failed to update the contact history"));
            return Ok(returnDto);

        }

        [HttpGet("userItems")]
        public async Task<ActionResult<bool>> UpdateContactHistory(ICollection<UserHistoryItem> userItems)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrinciple(User);   
            if (loggedInUser == null) return Unauthorized(new ApiResponse(404, "unable to retrieve loggedin User"));
            var succeeded = await _userHistoryService.EditContactHistoryItems(userItems, loggedInUser.loggedInEmployeeId);
            if (succeeded) return Ok(true);
            return BadRequest(new ApiResponse(402, "Failed to Update the transactions items"));
        }

        [HttpGet("categoryrefdetails")]
        public async Task<ICollection<CategoryRefDto>> GetCategoryRefDetails()
        {
            var dtos = await _userHistoryService.GetCategoryRefDetails();
            return dtos;
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

    }
}