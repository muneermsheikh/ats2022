using core.Entities.EmailandSMS;

namespace core.ParamsAndDtos
{
    public class EmailMessageDto
    {
        public EmailMessage EmailMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}