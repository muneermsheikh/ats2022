using core.Interfaces;
using core.ParamsAndDtos;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class SMSController : BaseApiController
    {
        private readonly ISMSService _smsservice;
        public SMSController(ISMSService smsservice)
        {
            _smsservice = smsservice;
        }

        [HttpPost]
        public string SendSMSMessage(SMSDto dto)
        {
            return _smsservice.sendMessage(dto.PhoneNo, dto.MessageText);
        }
    }
}
