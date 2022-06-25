using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
    public class OrderAssignmentService : IOrderAssignmentService
    {
        private readonly ATSContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskService _taskService;
        private readonly ICommonServices _commonServices;
        //private readonly int _targetDaysForHRExecutivesToSourceCVs = 5;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeService _empService;
        public OrderAssignmentService(ATSContext context, IUnitOfWork unitOfWork, IEmployeeService empService,
            ITaskService taskService, ICommonServices commonServices, UserManager<AppUser> userManager)
        {
            _empService = empService;
            _userManager = userManager;
            _commonServices = commonServices;
            _taskService = taskService;
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public async Task<ICollection<EmailMessage>> DesignOrderAssessmentQs(int orderId, AppUser loggedInAppUser)
        {
            if (!await OrderItemsNeedAssessment(orderId)) throw new Exception("None of the order items need assessment");

            var orderDto = await _commonServices.GetOrderAssignmentDto(orderId);
            //var loggedInAppUser = await _userManager.FindByEmailAsync(loggedInUserEmail);
            var loggedInEmployeeId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInAppUser.Id);

            var task = new ApplicationTask((int)EnumTaskType.DesignOrderAssessmentQ, DateTime.Now, loggedInEmployeeId,
                orderDto.ProjectManagerId, orderDto.OrderId, orderDto.OrderNo, 0, "Design Assessment Questions for Order No. " +
                orderDto.OrderNo + " dated " + orderDto.OrderDate + " for " + orderDto.CustomerName,
                DateTime.Now.AddDays(1), "Open",0, null);

            //emails are composed in ComposeServices and sent in TaskServices
            var msgs = await _taskService.CreateNewApplicationTask(task, loggedInEmployeeId);

            return msgs.emailMessages;

        }


        public async Task<bool> DeleteHRExecAssignment(int id)
        {
            //TODO - if in process, do not allow deletion

            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _unitOfWork.Repository<ApplicationTask>().Delete(task);
                if (await _unitOfWork.Complete() > 0) { return true; } else { return false; }
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> OrderItemsNeedAssessment(int orderId)
        {
            var ItemsNeedAssessment = await _context.OrderItems.Where(x => x.OrderId == orderId
                && x.RequireAssess == true).ToListAsync();

            return (ItemsNeedAssessment != null && ItemsNeedAssessment.Count > 0);
        }

          public Task<bool> EditOrderAssignment(ApplicationTask task)
          {
               throw new NotImplementedException();
          }

          public Task<bool> SetTaskAsCompleted(int id, string remarks)
          {
               throw new NotImplementedException();
          }

          public Task<bool> DeleteApplicationTask(int id, string remarks)
          {
               throw new NotImplementedException();
          }
     }
}