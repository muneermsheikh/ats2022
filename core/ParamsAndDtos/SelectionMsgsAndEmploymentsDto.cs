using System.Collections.Generic;
using core.Entities.EmailandSMS;

namespace core.ParamsAndDtos
{
     public class SelectionMsgsAndEmploymentsDto
    {
        public ICollection<EmailMessage> EmailMessages {get; set;}
        public ICollection<EmploymentDto> EmploymentDtos {get; set;}
    }
}