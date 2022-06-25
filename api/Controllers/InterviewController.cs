using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using core.Entities.HR;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class InterviewController : BaseApiController
     {
          private readonly IInterviewService _interviewService;
          public InterviewController(IInterviewService interviewService)
          {
               _interviewService = interviewService;
          }

          [Authorize(Roles = "Admin, HRManagr, HRSupervisor")]
          [HttpPost]
          public async Task<ActionResult<Interview>> AddInterview(InterviewToAddDto dto)
          {
               if (dto.OrderId == 0 || dto.InterviewDateFrom.Year < 2000 || dto.InterviewDateUpto.Year < 2000 
                    || string.IsNullOrEmpty(dto.InterviewVenue)) return BadRequest(new ApiResponse(404, "incomplete data provided"));
               
               if (dto.InterviewDateUpto < dto.InterviewDateFrom) return BadRequest(new ApiResponse(404, "Interview Starting Date should be earlier than ending date"));
               var intvw = await _interviewService.AddInterview(dto);

               if (intvw == null) return BadRequest(new ApiResponse(404, "Bad Request - failed to save the Interview Data"));

               return intvw;
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPut]
          public async Task<ActionResult<Interview>> EditInterview ([FromQuery] Interview interview)
          {
               var intvw = await _interviewService.EditInterview(interview);

               if (intvw == null) return BadRequest(new ApiResponse(404, "Failed to update the interview data"));

               return intvw;
          }

          [Authorize]
          [HttpGet("interviews")]
          public async Task<ActionResult<Pagination<InterviewBriefDto>>> GetInterviews([FromQuery]InterviewSpecParams specParams )
          {
               var interviews = await _interviewService.GetInterviews(specParams);
               if (interviews == null) return NotFound(new ApiResponse(404, "No interviews found matching the status"));

               return Ok(interviews);
          }
          
          [Authorize]
          [HttpGet("interviewById/{id}")]
          public async Task<ActionResult<Interview>> GetInterviewById(int Id )
          {
               var interviews = await _interviewService.GetInterviewById(Id);
               if (interviews == null) return NotFound(new ApiResponse(404, "No interviews found matching the status"));

               return Ok(interviews);
          }

          //if the Interview data exists in DB, returns the same
          //if it does not exist, creates an Object and returns it
          [Authorize]
          [HttpGet("getorcreateinterview/{id}")]
          public async Task<ActionResult<Interview>> GetOrCreateInterviewByOrderId(int id)
          {
               var interview = await _interviewService.GetOrCreateInterviewByOrderId(id);
               if (interview == null) return NotFound(new ApiResponse(404, "No interviews found matching the status"));

               return Ok(interview);
          }

          [Authorize]
          [HttpGet("openInterviews")]
          public async Task<ActionResult<Pagination<InterviewBriefDto>>> GetAllOpenInterviews()
          {
               var interviews = await _interviewService.GetOpenInterviews();
               if (interviews == null) return NotFound(new ApiResponse(404, "No open interviews found"));

               return Ok(interviews);
          }


          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPost("assigncandidates")]
          public async Task<ActionResult<ICollection<InterviewItemCandidate>>> AssignCandidatesToInterviewItem(AssignCandidatesToAddDto dto)
          {
               if (dto.CandidateIds.Count == 0 ) return BadRequest(new ApiResponse(404, "No data provided"));

               int durationinminutes = 25;

               var candidatesAssigned = await _interviewService.AddCandidatesToInterviewItem(dto.InterviewItemId, dto.ScheduledTimeFrom,
                    durationinminutes, dto.InterviewMode, dto.CandidateIds);

               if (candidatesAssigned == null) return BadRequest(new ApiResponse(404, "failed to assign the candidates"));

               return Ok(candidatesAssigned);

          }
     
          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpDelete("deleteInterviewbyid/{id}")]
          public async Task<ActionResult<bool>> DeleteInterview (int id)
          {
               return await _interviewService.DeleteInterview(id);
          }
          

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpDelete("deleteCandidateAssignments")]
          public async Task<ActionResult<bool>> DeleteCandidateAssignedToInterview ([FromQuery] List<int> candidateInterviewIds)
          {
               if (candidateInterviewIds.Count == 0) return BadRequest(new ApiResponse(404, "Record Ids not provided"));
               return await _interviewService.DeleteFromInterviewItemCandidates(candidateInterviewIds);
          }
          
          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPut("registerCandidateArrived/{candidateInterviewId}")]
          public async Task<ActionResult<bool>> RegisterCandidateArrivedForInterview(int candidateInterviewId, DateTime reportedAt)
          {
               if (candidateInterviewId == 0) return BadRequest(new ApiResponse(404, "Record Id cannot be 0"));

               return await _interviewService.RegisterCandidateReportedForInterview(candidateInterviewId, reportedAt);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPut("registerCandidateInterviewed/{candidateInterviewId}/{interviewMode}/{interviewedAt}")]
          public async Task<ActionResult<bool>> RegisterCandidateAsInterviewed (int candidateInterviewId, 
               string interviewMode, DateTime interviewedAt)
          {
               if (candidateInterviewId == 0) return BadRequest(new ApiResponse(404, "Record Id cannot be 0"));
               return await _interviewService.RegisterCandidateAsInterviewed(candidateInterviewId, interviewMode, interviewedAt);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPut("registerCandidateInterviewedWithResult/{candidateInterviewId}/{interviewMode}/{interviewedAt}/{selectionStatusId}")]
          public async Task<ActionResult<bool>> RegisterCandidateInterviewedWithResult (int candidateInterviewId, 
               string interviewMode, DateTime interviewedAt, int selectionStatusId)
          {
               if (candidateInterviewId == 0) return BadRequest(new ApiResponse(404, "Record Id cannot be 0"));
               return await _interviewService.RegisterCandidateInterviewedWithResult(
                    candidateInterviewId, interviewMode, interviewedAt, selectionStatusId);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpGet("interviewAttendance/{orderId}")]
          public async Task<ActionResult<ICollection<InterviewAttendanceDto>>> GetInterviewAttendance(int orderId, [FromQuery] List<int> attendanceStatusIds)
          {
               if (orderId == 0 || attendanceStatusIds.Count == 0) return BadRequest(new ApiResponse(402, "Bad Request"));
               
               var lst = await _interviewService.GetInterviewAttendanceOfAProject(orderId, attendanceStatusIds);

               if (lst == null) return NotFound(new ApiResponse(400, "No interview records found matching the criteria"));

               return Ok(lst);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpGet("catandcandidates/{interviewItemId}")]
          public async Task<ActionResult<ICollection<InterviewItemCandidateDto>>> GetInterviewCategoryAndAttendance(int interviewItemId)
          {
               
               var lst = await _interviewService.GetInterviewItemAndAttendanceOfInterviewItem(interviewItemId);

               if (lst == null) return NotFound(new ApiResponse(400, "No records found matching the criteria"));

               return Ok(lst);
          }

          [Authorize(Roles = "Admin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpGet("candidatesmatchinginterviewcat")]
          public async Task<ActionResult<ICollection<CandidateBriefDto>>> GetCandidatesMatchingInterviewCategory(InterviewSpecParams interviewParams)
          {
               var cands = await _interviewService.GetCandidatesMatchingInterviewCategory(interviewParams);

               if (cands == null) return NotFound(new ApiResponse(404, "No matching candidates found"));

               return Ok(cands);
          }

     }
}
