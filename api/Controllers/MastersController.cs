using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using core.Entities;
using core.Entities.MasterEntities;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize(Roles = "Admin, HRManager, HRSupervisor")]
     public class MastersController : BaseApiController
     {
          private readonly IMastersService _mastersService;
          public MastersController(IMastersService mastersService)
          {
               _mastersService = mastersService;
          }

          
     //industry
          [Authorize]         //(Policy = "MastersAddRole")]
          [HttpPost("addindustry/{industryname}")]
          public async Task<ActionResult<Industry>> AddIndustry(string industryname)
          {
               var industryAdded = await _mastersService.AddIndustry(industryname);
               if (industryAdded==null) return BadRequest(new ApiResponse(400, "failed to add the industry"));
               return Ok(industryAdded);
          }
          
          [Authorize]         //(Policy = "MastersEditRole")]
          [HttpPut("editindustry")]
          public async Task<ActionResult<bool>> EditIndustry(Industry industry)
          {
               var succeeded = await _mastersService.EditIndustryAsync(industry);
               if (!succeeded) return BadRequest(new ApiResponse(400, "failed to edit the industry"));
               return true;
          }

          [Authorize]         //(Policy = "MastersEditRole")]
          [HttpDelete("deleteindustry")]
          public async Task<ActionResult<bool>> DeleteCategory(Industry industry)
          {
               var succeeded = await _mastersService.DeleteIndustryyAsync(industry);
               if (!succeeded) return BadRequest(new ApiResponse(400, "Failed to delete the industry"));
               return true;
          }

          [HttpGet("industry")]
          public async Task<ActionResult<Industry>> GetIndustry(int id)
          {
               var item = await _mastersService.GetIndustry(id);
               if (item!=null) return BadRequest(new ApiResponse(404, "Not Found"));
               return Ok(item);
          }

          [HttpGet("industrylike/{industryname}")]
          public async Task<ActionResult<Industry>> GetIndustryLike(string industryname)
          {
               var item = await _mastersService.GetIndustryListLike(industryname);
               if (item==null) return BadRequest(new ApiResponse(404, "Not Found"));
               return Ok(item);
          }

          [HttpGet("industrylist")]
          public async Task<ActionResult<Industry>> GetIndustryListAsync(IndustrySpecParams industryParams)
          {
               var lst = await _mastersService.GetIndustryListAsync(industryParams);
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

          [HttpGet("industrieslist")]
          public async Task<ActionResult<ICollection<Industry>>> GetIndustriesListAsync()
          {
               var lst = await _mastersService.GetIndustryListWOPaginationAsync();
               return Ok(lst);
          }


     //qualification
          [HttpPost("addqualification/{qualificationname}")]
          public async Task<ActionResult<Qualification>> AddQualification(string qualificationname)
          {
               var qualificationAdded = await _mastersService.AddQualification(qualificationname);
               if (qualificationAdded==null) return BadRequest(new ApiResponse(400, "failed to add the qualification"));
               return Ok(qualificationAdded);
          }

          [HttpPut("editqualification")]
          public async Task<ActionResult<bool>> EditQualification(Qualification qualification)
          {
               var succeeded = await _mastersService.EditQualificationAsync(qualification);
               if (!succeeded) return BadRequest(new ApiResponse(400, "failed to edit the qualification"));
               return true;
          }

          [HttpDelete("deletequalification")]
          public async Task<ActionResult<bool>> DeleteCategory(Qualification qualification)
          {
               var succeeded = await _mastersService.DeleteQualificationAsync(qualification);
               if (!succeeded) return BadRequest(new ApiResponse(400, "Failed to delete the qualification"));
               return true;
          }

          [HttpGet("qlist")]
          public async Task<ActionResult<Qualification>> GetQListAsync()
          {
               var lst = await _mastersService.GetListAsync();
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

          [HttpGet("qualificationlist")]
          public async Task<ActionResult<Qualification>> GetQualificationListAsync(QualificationSpecParams specParams)
          {
               var lst = await _mastersService.GetQualificationListAsync(specParams);
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

     //skilldata
          
          [HttpGet("skilldatalist")]
          public async Task<ActionResult<ICollection<SkillData>>> GetSkillDataListAsync()
          {
               var lst = await _mastersService.GetSkillDataListAsync();
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

     //contract review item data
          [HttpGet("reviewitemdata/{description}/{ismandatory}")]
          public async Task<ReviewItemData> AddReviewItemData(string description, bool ismandatory)
          {
               return await _mastersService.AddReviewItemData(description, ismandatory);
          }

          [HttpDelete("reviewitemdata")]
          public async Task<bool> DeleteItemReviewItemData(ReviewItemData reviewItemData)
          {
               return await _mastersService.DeleteReviewItemDataAsync(reviewItemData);
          }

     
     //Contract Review Item Status

          [HttpGet("reviewitemstatus")]
          public async Task<IReadOnlyList<ReviewItemStatus>> GetReviewItemStatusList()
          {
               return await _mastersService.GetReviewItemStatusListAsync();
          }

          [HttpPost("reviewitemstatus/{itemstatus}")]
          public async Task<ReviewItemStatus> AddReviewItemStatus(string itemstatus)
          {
               return await _mastersService.AddReviewItemStatus(itemstatus);
          }

         [HttpPut("reviewitemstatus")]
         public async Task<bool> EditReviewItemStatus(ReviewItemStatus reviewItemStatus)
         {
               return await _mastersService.EditReviewItemStatusAsync(reviewItemStatus);
         }
     }
}