using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using AutoMapper;
using core.Entities.HR;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class InterviewFollowupController : BaseApiController
     {
          private readonly IInterviewFollowupService _followupService;
          private readonly IMapper _mapper;
          public InterviewFollowupController(IInterviewFollowupService followupService, IMapper mapper)
          {
               _mapper = mapper;
               _followupService = followupService;
          }

          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, Receptionist")]
          [HttpPost]
          public async Task<ActionResult<ICollection<InterviewItemCandidateFollowup>>> AddNewFollowupToInterviewCandidate(InterviewCandidateFollowupToAddDto followups)
          {
               var fups = await _followupService.AddToInterviewItemCandidatesFollowup(followups);
               if (fups == null) return BadRequest(new ApiResponse(404, "Bad Request"));

               return Ok(fups);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive, Receptionist")]
          [HttpPut]
          public async Task<ActionResult<ICollection<InterviewItemCandidateFollowup>>> EditFollowupToInterviewCandidate(ICollection<InterviewItemCandidateFollowup> followups)
          {
               var fups = await _followupService.EditInterviewItemCandidatesFollowup(followups);
               if (fups == null) return BadRequest(new ApiResponse(404, "Bad Request"));

               return Ok(fups);
          }


          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive, Receptionist")]
          [HttpDelete]
          public async Task<ActionResult<bool>> DeleteCandidateFollowups(ICollection<InterviewItemCandidateFollowup> followups)
          {
               return await _followupService.DeleteInterviewItemCandidatesFollowup(followups);
          }



     }
}