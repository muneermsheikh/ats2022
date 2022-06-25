using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.Tasks;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IOrderAssignmentService
    {
        Task<ICollection<EmailMessage>> DesignOrderAssessmentQs(int orderId, AppUser loggedInUser);
        Task<bool> DeleteHRExecAssignment(int orderItemId);
        Task<bool> OrderItemsNeedAssessment(int orderId);
        Task<bool> EditOrderAssignment(ApplicationTask task);
        Task<bool> SetTaskAsCompleted(int id, string remarks);
        Task<bool> DeleteApplicationTask(int id, string remarks);
        
    }
}