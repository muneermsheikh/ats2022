using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;

namespace core.ParamsAndDtos
{
    public class MessagesToReturnDto
    {
        public ICollection<EmailMessage> EmailMessages { get; set; }
        public ICollection<SMSMessage> SMSMessages { get; set; }
        public ICollection<SMSMessage> WhatsAppMessages { get; set; }
        
    }
}