using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.ParamsAndDtos;

namespace core.Interfaces
{
    public interface IComposeMessageForCandidates
    {
        EmailMessage ComposeMessagesForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessagesForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        
    }
}