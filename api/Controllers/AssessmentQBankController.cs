using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Errors;
using core.Entities;
using core.Entities.MasterEntities;
using core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    
    public class AssessmentQBankController : BaseApiController
    {
        private readonly ILogger<AssessmentQBankController> _logger;
          private readonly IAssessmentQBankService _service;

        public AssessmentQBankController(ILogger<AssessmentQBankController> logger, 
            IAssessmentQBankService service)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("existingqbankcategories")]
        public async Task<ICollection<Category>> ExistingQBankCategories()
        {
            return await _service.GetExistingCategoriesInAssessmentQBank();
        }

        [HttpGet("assessmentbankqs")]
        public async Task<ICollection<AssessmentQBank>> GetAssessmentBankQs()
        {
            return await _service.GetAssessmentQBanks();
        }

        [HttpGet("catqs/{categoryName}")]
        public async Task<AssessmentQBank> GetAssessmentQsOfCategoryByName(string categoryName)
        {
            return await _service.GetAssessmentQsOfACategoryByName(categoryName);
        }

        [HttpGet("byid/{id}")]
        public async Task<AssessmentQBank> GetAssessmentQBankByCategoryId(int id)
        {
            var q = await _service.GetAssessmentQBankByCategoryId(id);
            return q;
        }

        
        [HttpPost]
        [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
        public async Task<ActionResult<AssessmentQBank>> InsertAssessmentQ(AssessmentQBank qbank)
        {
            var q = await _service.InsertAssessmentQBank(qbank);
            if (q == null) return BadRequest(new ApiResponse(400, "Bad Request - this probably means the Assessment Question for the chosen category already exists"));
            return Ok(q);
        }
        [Authorize(Roles ="Admin, HRManager, HRSupervisor")]
        [HttpPut]
        public async Task<AssessmentQBank> UpdateAssessmentQ(AssessmentQBank qBank)
        {
            var success = await _service.UpdateAssessmentQBank(qBank);
            return success;
        }

    }
}