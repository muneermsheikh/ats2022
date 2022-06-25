using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize]
     public class TaskController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ITaskService _taskService;
          private readonly IEmailService _emailService;
          private readonly ITaskControlledService _taskControlledService;
          public TaskController(IUnitOfWork unitOfWork, 
               ITaskService taskService, 
               ITaskControlledService taskControlledService,
               IEmailService emailService)
          {
               _emailService = emailService;
               _taskService = taskService;
               _taskControlledService=taskControlledService;
               _unitOfWork = unitOfWork;
          }

          [Authorize]
          [HttpPost("getorcreate")]
          public async Task<ActionResult<ApplicationTask>> GetOrCreateApplicationTask([FromQuery]ApplicationTask task) {
               var t = await _taskService.GetOrCreateTask(task);
               if (t!=null) return Ok(t);
               return BadRequest(new ApiResponse(404, "Failed to get or create new task"));
          }

          [Authorize]
          [HttpPost("create")]
          public async Task<ActionResult<ApplicationTask>> CreateApplicationTask(ApplicationTask task) {
               
               var t = await _taskService.GetOrCreateTask(task);
               if (t!=null) return Ok(t);
               return BadRequest(new ApiResponse(404, "Failed to create new task"));
          }
          
          [Authorize]
          [HttpPost]
          public async Task<ActionResult<ICollection<EmailAndSmsMessagesDto>>> CreateNewApplicationTask(ApplicationTask task)
          {
               var loggedInUser = User.GetUserIdentityUserEmployeeId();
               //verify object data
               if (task.TaskDate.Year < 2000) return BadRequest(new ApiResponse(404, "Task Date not set"));
               if (task.CompleteBy.Year < 2000) return BadRequest(new ApiResponse(404, "Task Completion Date not set"));
               if (task.TaskOwnerId == 0) return BadRequest(new ApiResponse(404, "Bad Request - Task Owner not defined"));
               if (string.IsNullOrEmpty(task.TaskStatus)) return BadRequest(new ApiResponse(404, "Bad Request - task status not provided"));
               if (string.IsNullOrEmpty(task.TaskDescription)) return BadRequest(new ApiResponse(404, "Task Description cannot be blank"));
               if (task.AssignedToId == 0) return BadRequest(new ApiResponse(404, "Task not assigned to any one"));

               // ** TODO ** - verify assignedToId and TaskOwnerId exist
               
               var emailMessages = await _taskService.CreateNewApplicationTask(task, loggedInUser);
               var AttachmentFilePaths = new List<string>();
               if (emailMessages != null &&
                   task.PostTaskAction != EnumPostTaskAction.OnlyComposeEmailAndSMSMessages
                   && task.PostTaskAction != EnumPostTaskAction.OnlyComposeEmailMessage)   //Send involed
               {
                    foreach (var msg in emailMessages.emailMessages)
                    {
                        _emailService.SendEmail(msg, AttachmentFilePaths);
                    }
               }

               return Ok(emailMessages);

          }

          [HttpPut]
          public async Task<ActionResult<MessagesDto>> EditApplicationTask(ApplicationTask task)
          {
               var employeeId = User.GetUserIdentityUserEmployeeId();

               return await _taskService.EditApplicationTask(task, employeeId);
          }

          [HttpDelete]
          public async Task<bool> DeleteApplicationTask(ApplicationTask task)
          {
               return await _taskService.DeleteApplicationTask(task);
          }

          [HttpGet]
          public async Task<ActionResult<Pagination<ApplicationTaskDto>>> GetApplicationTask([FromQuery]TaskParams taskParams)
          {
               var emps = await _taskControlledService.GetApplicationTasksPaginated(taskParams);
               if (emps == null || emps.Count == 0) return BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(emps);
          }
          [HttpGet("wopagination")]
          public async Task<ActionResult<ICollection<ApplicationTaskDto>>> GetApplicationTasksWOPagination([FromQuery]TaskParams taskParams)
          {
               var emps = await _taskControlledService.GetApplicationTasksWOPagination(taskParams);
               if (emps == null || emps.Count == 0) return BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(emps);
          }

          [HttpGet("pendingtasks/{taskstatus}/{pageIndex}/{pageSize}")]
          public async Task<ActionResult<Pagination<ApplicationTaskDto>>> GetPendingTasks(string taskstatus, int pageIndex, int pageSize)
          {
               var emps = await _taskControlledService.GetApplicationPendingTasksPaginated(taskstatus, pageIndex, pageSize);
               if (emps == null || emps.Count == 0) return BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(emps);
          }

          [HttpGet("pendingtasksofauser/{userid}/{pageIndex}/{pageSize}")]
          public async Task<ActionResult<Pagination<ApplicationTask>>> GetPendingTasksOfAUserId(int userid, int pageIndex, int pageSize)
          {
               var emps = await _taskService.GetApplicationPendingTasksOfAUserPaginated(userid, pageIndex, pageSize);
               if (emps == null || emps.Count == 0) return BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(emps);
          }

          [Authorize]
          [HttpPut("applicationtask/{applicationTaskId}/{dateCompleted}")]
          public async Task<ActionResult<bool>> SetApplicationTaskAsCompleted(int applicationTaskId, DateTime dateCompleted)
          {
               var userName = User.GetUsername();
               var userId = User.GetIdentityUserId();
               return await _taskService.SetApplicationTaskStatus(applicationTaskId, dateCompleted, "Completed", userName, userId);
          }

          [Authorize]
          [HttpPut("applicationtask/{applicationTaskId}/{dateOfCancellation}")]
          public async Task<ActionResult<bool>> SetApplicationTaskAsCanceled(int applicationTaskId, DateTime dateOfCancellation)
          {
               var userName = User.GetUsername();
               var userId = User.GetIdentityUserId();
               return await _taskService.SetApplicationTaskStatus(applicationTaskId, dateOfCancellation, "Canceled", userName, userId);
          }

          [HttpPost("item")]
          public async Task<ActionResult<TaskItem>> AddATaskItem(TaskItem taskItem)
          {
               var item = await _taskService.CreateNewTaskItem(taskItem);
               if (item != null) return Ok(item);
               return BadRequest(new ApiResponse(404, "Failed to add the task item"));
          }

          [HttpPut("item")]
          public async Task<ActionResult<TaskItem>> EditTaskItem(TaskItem taskItem)
          {
               var t = await _taskService.EditTaskItem(taskItem);

               if (t == null) return BadRequest(new ApiResponse(404, "Failed to edit the task item"));

               return Ok(t);
          }

          [HttpDelete("item")]
          public async Task<bool> DeleteTaskItem(TaskItem taskItem)
          {
               return await _taskService.DeleteTaskItem(taskItem);
          }


     }
}