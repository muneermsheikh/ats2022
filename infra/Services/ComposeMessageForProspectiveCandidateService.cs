using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class ComposeMessageForProspectiveCandidateService : IComposeMessageForCandidates
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          public ComposeMessageForProspectiveCandidateService(ATSContext context, IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
               _context = context;
          }

        
          public EmailMessage ComposeMessagesForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " + dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you very much for confirming your interest in the above opening.  As discussed, please send your updated Profile by return " +
                    "to forward same to the client.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, dto.Email,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "",
                    sSubject, content, (int)EnumMessageType.CVAcknowledgementByEMail, 3);
          
               return emailMsg;

          }

          public EmailMessage ComposeMessagesForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {

               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "A customer of ours based in " + dto.City + ", " + dto.Country + " is looking for a " + dto.CategoryName + 
                    "<br><br>We have your details from " + dto.Source + ", and we think it meets with the client requirements.  " +
                    "To discuss the openign with you, we tried many a times to reach you on your numebr " + dto.Phone + 
                    " but failed to reach you.<br><br>If you are interested in the opening, please share with us your correct contact details along with your updated profile " +
                    "so as to submit the same to our Client for their review.<br><br>If you are not interested, kindly respond with a <i>Not Interested</i>" +
                    " message, so as not to bother you again.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, dto.Email,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "",
                    sSubject, content, (int)EnumMessageType.CandidateFollowup, 3);

               return emailMsg;

          }

     }
}