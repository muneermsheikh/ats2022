using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using core.Entities;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{    
     //[Authorize(Policy = "Employee")]
     public class CategoriesController : BaseApiController
     {
          private readonly ICategoryService _categoryService;
          
          public CategoriesController(ICategoryService categoryService)
          {
               _categoryService = categoryService;

          }

          
          [HttpGet("{id}")]
          [ProducesResponseType(StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status404NotFound)]
          public async Task<ActionResult<Category>> GetCategory(int id)
          {
               var cat = await _categoryService.GetCategoryById(id);
               if (cat == null) return NotFound(new ApiResponse(404));
               return cat;
          }

          [Authorize]    //(Policy = "MastersAddRole")]
          [HttpPost("addcategory/{categoryname}")]
          public async Task<ActionResult<Category>> AddCategory(string categoryname)
          {
               var categoryAdded = await _categoryService.AddCategory(categoryname);
               if (categoryAdded==null) return BadRequest(new ApiResponse(400, "failed to add the category"));
               return Ok(categoryAdded);
          }

          [Authorize]         //(Policy = "MastersEditRole")]
          [HttpPut("editcategory")]
          public async Task<ActionResult<bool>> EditCategory(Category category)
          {
               var succeeded = await _categoryService.EditCategoryAsync(category);
               if (!succeeded) return BadRequest(new ApiResponse(400, "failed to edit the category"));
               return true;
          }

          [Authorize]         //(Policy = "MastersEditRole")]
          [HttpDelete("deletecategory")]
          public async Task<ActionResult<bool>> DeleteCategory(Category category)
          {
               var succeeded = await _categoryService.DeleteCategoryAsync(category);
               if (!succeeded) return BadRequest(new ApiResponse(400, "Failed to delete the category"));
               return true;
          }

          [Authorize]         //(Policy = "ViewReportsRole")]
          [HttpGet("categorylist")]
          public async Task<ActionResult<Pagination<Category>>> GetCategoryListAsync(CategorySpecParams categoryParams)
          {
               var lst = await _categoryService.GetCategoryListAsync(categoryParams);
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

          [HttpGet("categories")]
          public async Task<ActionResult<ICollection<Category>>> GetCategoriesAsync()
          {
               var lst = await _categoryService.GetCategoriesAsync();
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }

          
          [HttpGet("industry/{industryname}")]
          public async Task<ActionResult<ICollection<Category>>> GetCategoriesOfAnIndustryAsync(string industryname)
          {
               var lst = await _categoryService.GetCategoriesAsync();
               if (lst == null) return BadRequest(new ApiResponse(400, "no data returned"));
               return Ok(lst);
          }
          
     }
}