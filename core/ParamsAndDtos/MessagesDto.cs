using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;

namespace core.ParamsAndDtos
{
    public class MessagesDto
    {
          public MessagesDto()
          {
          }

            public ICollection<EmailMessage> emailMessages { get; set; }
            public string ErrorString {get; set;}
    }
}