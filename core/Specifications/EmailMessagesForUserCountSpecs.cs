using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Params;

namespace core.Specifications
{
    public class EmailMessagesForUserCountSpecs: BaseSpecification<EmailMessage>
    {
         public EmailMessagesForUserCountSpecs(string Container, string username, int pageSize, int pageIndex)
        : base (x =>
                (Container=="Inbox" || x.RecipientEmailAddress.ToLower()==username.ToLower()) &&
                (Container=="Sent" || x.SenderEmailAddress.ToLower()==username.ToLower() ) 
            )
          {
          }
        public EmailMessagesForUserCountSpecs(EmailMessageSpecParams msgParams)
        : base (x =>
            (msgParams.Container=="Inbox" || x.RecipientEmailAddress.ToLower()==msgParams.Username.ToLower()) &&
            (msgParams.Container=="Sent" || x.SenderEmailAddress.ToLower()==msgParams.Username.ToLower() ) 
            )
        {
        }

    }
}
