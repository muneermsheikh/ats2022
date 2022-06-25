using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class ContractReviewController : BaseApiController
    {
        private readonly IContractReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ContractReviewController(IContractReviewService reviewService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _reviewService = reviewService;
        }


        [Authorize(Roles = "ContractReviewEdit")]
        [HttpPost("createobject/{orderId}")]
        public async Task<ContractReview> CreateContractReviewObject(int orderId)
        {
            var loggedInAppUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
            var cReview = await _reviewService.CreateContractReviewObject(orderId, loggedInAppUser.Id);
            return cReview;
        }

        [Authorize(Roles = "ContractReviewEdit")]
        [HttpPost("CreateContractReviewItemObject/{orderItemId}")]
        public async Task<ContractReviewItem> CreateContractReviewItemObject(int orderItemId)
        {
            var loggedInAppUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
            var loggedInEmployeeId = loggedInAppUser.loggedInEmployeeId;
            return await _reviewService.CreateContractReviewItemObject(orderItemId, loggedInEmployeeId);
        }

        [Authorize(Roles = "ContractReviewEdit")]
        [HttpPut("update")]
        public async Task<ActionResult<EmailMessage>> UpdateContractReview(ContractReview contractReview)
        {
            var msgDto = await _reviewService.EditContractReview(contractReview);
            if (msgDto == null) {
                return BadRequest(new ApiResponse(404, "Failed to update the Contract Review"));
            } else {
                return msgDto.EmailMessage;
            }
        }

       
        [Authorize(Roles = "ContractReviewView, ContractReviewEdit")]
        [HttpGet("reviews")]
        public async Task<Pagination<ContractReview>> GetContractReviews([FromQuery]ContractReviewSpecParams reviewParams)
        {
            var obj = await _reviewService.GetContractReviews(reviewParams);
            return obj;
        }

         [Authorize(Roles ="ContractReviewEdit, ContractReviewView")]
        [HttpGet("reviewdata")]
        public async Task<ICollection<ReviewItemData>> GetReviewItemData()
        {
            return await _reviewService.GetReviewData();
        }
        
        [Authorize(Roles ="ContractReviewEdit, ContractReviewView")]
        [HttpGet("{id}")]
        public async Task<ContractReview> GetContractReviews(int id)
        {
            var loggedInAppUser = await _userManager.FindByEmailFromClaimsPrinciple(User);
            var loggedInEmployeeId = loggedInAppUser.loggedInEmployeeId;
            return await _reviewService.GetOrAddContractReview(id,loggedInEmployeeId);
            
        }


        [Authorize(Roles = "ContractReviewView, ContractReviewEdit")]
        [HttpGet("contractreviewoforderitem")]
        public async Task<ICollection<ContractReviewItemDto>> GetContractReviewItemsDto(ContractReviewItemSpecParams cParams)
        {
            var rvwitem = await _reviewService.GetContractReviewItemsWithOrderDetails(cParams);
            return rvwitem;
        }


        [Authorize(Roles = "ContractReviewEdit")]
        [HttpDelete("{orderid}")]           //deletes contractreview and all children
        public async Task<ActionResult<bool>> DeleteContractReview(int orderid)
        {
            if (!await _reviewService.DeleteContractReview(orderid))  return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

        [Authorize(Roles = "ContractReviewEdit")]
        [HttpDelete("item/{orderitemid}")]      //deletes contractReviewItem and all reviewitems
        public async Task<ActionResult<bool>> DeleteContractReviewItem(int orderitemid)
        {
            if (!await _reviewService.DeleteContractReviewItem(orderitemid))  return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

        [Authorize(Roles = "ContractReviewEdit")]
        [HttpDelete("reviewitem/{id}")]
        public async Task<ActionResult<bool>> DeleteContractReviewReviewItem(int id)
        {
            if (!await _reviewService.DeleteReviewReviewItem(id)) return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

        [Authorize(Roles ="ContractReviewEdit, ContractReviewView")]
        [HttpGet("reviewitem/{orderitemid}")]
        public async Task<ActionResult<ContractReviewItemDto>> GetReviewResults (int orderitemid)
        {
            var results = await _reviewService.GetOrAddReviewResults(orderitemid);

            if(results !=null) return Ok(results);

            return NotFound(new ApiResponse(404, "Not found"));
        }

        [Authorize(Roles ="ContractReviewEdit")]
        [HttpPut("reviewitem")]
        public async Task<ActionResult<bool>> UpdateContractReviewItem(ContractReviewItemDto model)
        {
            var result = await _reviewService.EditContractReviewItem(model);

            if (!result) return BadRequest(new ApiResponse(402, "failed to update the contract review item"));

            return Ok(result);
        }
        
    }
}