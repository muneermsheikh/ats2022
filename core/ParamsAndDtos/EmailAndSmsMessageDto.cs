using System.Collections.Generic;
using core.Entities.EmailandSMS;
using core.Entities.Tasks;

namespace core.ParamsAndDtos
{
    public class EmailAndSmsMessagesDto
    {
        public ApplicationTask ApplicationTask {get; set;}
        public ICollection<EmailMessage> EmailMessages { get; set; }
        public ICollection<SMSMessage> SMSMessages { get; set; }
    }
}