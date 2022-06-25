using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class EmploymentController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IEmploymentService _employmentService;
          public EmploymentController(IUnitOfWork unitOfWork, IEmploymentService employmentService)
          {
               _employmentService = employmentService;
               _unitOfWork = unitOfWork;
          }

          [Authorize(Roles = "Admn, HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin, DocumentControllerProcess")]
          [HttpGet]
          public async Task<ActionResult<Pagination<Employment>>> GetEmployments(EmploymentParams employmentParams)
          {
               var spec = new EmploymentSpecs(employmentParams);
               var specCount = new EmploymentForCountSpecs(employmentParams);
               var emps = await _unitOfWork.Repository<Employment>().ListAsync(spec);
               var ct = await _unitOfWork.Repository<Employment>().CountAsync(specCount);

               return Ok(new Pagination<Employment>(employmentParams.PageIndex,
                    employmentParams.PageSize, ct, emps));
          }

          [Authorize(Roles = "Admn, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpPost]
          public async Task<ActionResult<Employment>> AddEmployment(Employment employment)
          {
              // todo - verify object
              var sel = await _unitOfWork.Repository<SelectionDecision>().GetByIdAsync(employment.SelectionDecisionId);
              if (sel == null) return null;

              _unitOfWork.Repository<Employment>().Add(employment);

              if (await _unitOfWork.Complete() > 0) {
                  return await _unitOfWork.Repository<Employment>().GetEntityWithSpec(
                      new EmploymentSpecs(new EmploymentParams{CVRefId=employment.CVRefId})
                  );
              } else {
                  return null;
              }
          }

        [Authorize]
        [HttpGet("employment/{cvrefid}")]
        public async Task<Employment> GetEmployment (int cvrefid)
        {
            return await _employmentService.GetEmployment(cvrefid);
        }

        [Authorize]
        [HttpGet("employmentsbyorderno/{orderno}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromOrderNo(int orderno)
        {
            return await _employmentService.GetEmploymentDtoFromOrderNo(orderno);
        }
        
        [Authorize]
        [HttpGet("employmentsbydates/{datefrom}/{dateupto}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromOrderNo(DateTime dateform, DateTime dateupto)
        {
            return await _employmentService.GetEmploymentDtoBetwenDates(dateform, dateupto);
        }

        [Authorize]
        [HttpGet("employmentsbycvref/{cvref}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromCVRef(int cvref)
        {
            return await _employmentService.GetEmploymentDtoFromCVRefId(cvref);
        }


        [Authorize]
        [HttpGet("employmentfromselid/{id}")]
        public async Task<Employment> GetEmploymentFromSelId (int id)
        {
            return await _employmentService.GetEmploymentFromSelId(id);
        }

        [Authorize(Roles = "Admn, HRManager, HRSupervisor, DocumentControllerAdmin")]
        [HttpPut("employment")]
        public async Task<bool> UpdateEmployment(Employment employment)
        {
            return await _employmentService.EditEmployment(employment);
        }



     }
}