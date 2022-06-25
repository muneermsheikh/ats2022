using System.Collections.Generic;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Process;
using core.Entities.Users;
using core.Params;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IEmailService
    {
        EmailMessage SendEmail(EmailMessage emailMessage, ICollection<string> attachmentFiles);
        Task<EmailMessage> SaveEmailMessage(EmailMessage message);
        Task SendEmailAsync(EmailMessage emailMessage);
        Task<Pagination<EmailMessage>> GetEmailMessageOfLoggedinUser(EmailMessageSpecParams msgParams);

    }
}