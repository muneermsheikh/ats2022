using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
    public class ValidateTaskService: IValidateTaskService
    {
          private readonly ATSContext _context;
          public ValidateTaskService(ATSContext context)
          {
               _context = context;
          }

          public async Task<string> ValidateTaskObject(ApplicationTask task)
        {
            string ErrorString = "";

            switch (task.TaskTypeId)
            {
                case (int)EnumTaskType.OrderRegistration:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.ContractReview:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        if (task.OrderItemId == 0) ErrorString = "OrderItem Id not provided";
                        break;
                case (int)EnumTaskType.OrderAssignmentToProjectManager:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.DesignOrderAssessmentQ:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.AssignTaskToHRExec:
                        if (task.OrderItemId == 0) ErrorString = "Order Item Id and Order No not provided";
                        //check for index integrity
                        //check for duplicate key row in object 'dbo.Tasks' for unique index 'IX_Tasks_AssignedToId_OrderItemId_CandidateId_TaskTypeId'. 
                        //The duplicate key value is (1023, 18, 0, 4).

                        break;
                case (int)EnumTaskType.SubmitCVToHRSupForReview:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.SubmitCVToHRMMgrForReview:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.CVForwardToCustomers:
                        //check for uniqueindex violations -'IX_Tasks_AssignedToId_OrderItemId_CandidateId_TaskTypeId
                        //'IX_Tasks_AssignedToId_OrderItemId_CandidateId_TaskTypeId'. The duplicate key value is (2, 5, 4, 17).
                        var taskExists = await _context.Tasks.Where(x => 
                            x.CandidateId == task.CandidateId && x.OrderItemId == task.OrderItemId
                            && x.AssignedToId == task.AssignedToId && x.TaskTypeId == task.TaskTypeId).FirstOrDefaultAsync();
                        if (taskExists != null) {
                            ErrorString = "the task already exists";
                        }
                        if (task.OrderItemId == 0 || task.OrderNo == 0 || task.CandidateId == 0) ErrorString = "Order Item/Candidate details not provided";
                        break;
                case (int)EnumTaskType.SelectionFollowupByDocControllerAdmin:
                        break;
                case (int)EnumTaskType.SelctionRegistrationByDocControllerAdmin:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.MedicalTestsMobilization:
                        if (task.OrderId == 0 || task.OrderNo == 0) ErrorString = "Order Id and Order No not provided";
                        break;
                case (int)EnumTaskType.VisaDocsKSACompilation:
                        break;
                case (int)EnumTaskType.VisaDocsNonKSACompilation:
                        break;
                case (int)EnumTaskType.EmigrationAppLodging:
                        break;
                case (int)EnumTaskType.TravelTicketBooking:
                        break;
                case (int)EnumTaskType.OrderEditedAdvise:
                        break;
            }

            return ErrorString;
        }

    }
}