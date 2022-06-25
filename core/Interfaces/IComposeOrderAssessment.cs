using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;

namespace core.Interfaces
{
    public interface IComposeOrderAssessment
    {
        Task<ICollection<EmailMessage>> ComposeMessagesToDesignOrderAssessmentQs(int orderId, int LoggedInEmployeeId);
        
    }
}