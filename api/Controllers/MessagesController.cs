using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class MessagesController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IMapper _mapper;
          private readonly IComposeMessagesForAdmin _composeMsgAdmin;
          private readonly IEmailService _emailService;
          private readonly ISMSService _smsService;
          private readonly UserManager<AppUser> _userManager;
          private readonly IComposeMessagesForHR _composeMsgHR;
          private readonly IComposeMessagesForProcessing _composeMsgProcess;
          public MessagesController(IMapper mapper, IUnitOfWork unitOfWork, IComposeMessagesForAdmin composeMsgAdmin, 
               IComposeMessagesForHR composeMsgHR, IComposeMessagesForProcessing composeMsgProcess,
               IEmailService emailService, ISMSService smsService, UserManager<AppUser> userManager)
          {
               _composeMsgProcess = composeMsgProcess;
               _composeMsgHR = composeMsgHR;
               _userManager = userManager;
               _smsService = smsService;
               _emailService = emailService;
               _composeMsgAdmin = composeMsgAdmin;
               _mapper = mapper;
               _unitOfWork = unitOfWork;
          }

          [Authorize]
          [HttpPost]
          public async Task<ActionResult<MessageDto>> SendNewMessage(EmailMessage message)
          {
               //if (User == null) return BadRequest(new ApiResponse(400, "the user must log in to invoke this function"));
               var sender = await _userManager.FindByEmailAsync(message.SenderEmailAddress);
               if(sender == null) return BadRequest(new ApiResponse(400, "invalid sender user id"));
               /*var recipient = await _userManager.FindByEmailAsync(message.RecipientEmailAddress);
               if(recipient == null) return BadRequest(new ApiResponse(400, "invalid recipient user id"));
               */
               message.SenderId = sender.Id;      //User.GetIdentityUserId();
               message.SenderUserName = sender.UserName; 
               //message.Sender = sender;
               
               //message.RecipientId = recipient.Id;
              // message.RecipientUserName=recipient.UserName;
               var AttachmentFilePaths = new List<string>();
               //message.Recipient = recipient;

               
               var sentMsg =  _emailService.SendEmail(message, AttachmentFilePaths);       //not async
               
               if (sentMsg == null) {
                    return BadRequest(new ApiResponse(400, "message saved, but Failed to send it"));
               } else {
                    DateTime dtTimeNow;
                    dtTimeNow = DateTime.Now;
                    
                    message.MessageSentOn = dtTimeNow;
                    _unitOfWork.Repository<EmailMessage>().Update(message);
                    await _unitOfWork.Complete();
                    return new MessageDto {
                         Id = sentMsg.Id, 
                         //SenderId = sentMsg.SenderId, 
                         SenderUsername = sentMsg.SenderUserName,
                         //RecipientId = sentMsg.RecipientId, 
                         RecipientUsername = sentMsg.RecipientUserName,
                         MessageSent= dtTimeNow, DateRead = sentMsg.DateReadOn,
                         //SenderDeleted = sentMsg.SenderDeleted, RecipientDeleted = sentMsg.RecipientDeleted,
                         Content = sentMsg.Content
                    };
               }
          }

          [HttpGet("savemessage")]
          public async Task<ActionResult<EmailMessage>> SaveMessage(EmailMessage message) {
               var msg =  await _emailService.SaveEmailMessage(message);
               if (msg==null) return BadRequest(new ApiResponse(400, "failed to save email message"));
               return msg;
          }
          
          [Authorize]
          [HttpGet("loggedInUser")]
          public async Task<ActionResult<Pagination<EmailMessage>>> GetMessagesForLoggedInUser([FromQuery] EmailMessageSpecParams messageParams)
          {

               if (!"inboxsentdraft".Contains(messageParams.Container.ToLower())) 
                    return BadRequest(new ApiResponse(400, "invalid Params container"));

               var user = await _userManager.FindByEmailAsync(User.GetIdentityUserEmailId());
               messageParams.Username=user.UserName;

               /* 
               var specs = new EmailMessagesForUserSpecs(messageParams.Container, messageParams.Username, messageParams.PageSize, messageParams.PageIndex);
               var countSpecs = new EmailMessagesForUserCountSpecs(messageParams.Container, messageParams.Username, messageParams.PageSize, messageParams.PageIndex);
               var totalItems = await _unitOfWork.Repository<EmailMessage>().CountAsync(specs);
               var messages = await _unitOfWork.Repository<EmailMessage>().ListAsync(specs);
               */

               var messages = await _emailService.GetEmailMessageOfLoggedinUser(messageParams);
               //if (messages == null) return NotFound(new ApiResponse(404,"No " + messageParams.Container=="Inbox" ? " incoming " : " outgoing " + "email messages in record for the logged in user"));
               return Ok(messages);
          }


/*
          [HttpDelete("{id}")]
          public async Task<ActionResult> DeleteMessage(int id)
          {
               var username = User.GetUsername();

               var message = await _unitOfWork.Repository<EmailMessage>().GetByIdAsync(id);

               if (!message.SenderId.Equals(message.RecipientId))
                    return Unauthorized();

               if (message.SenderUserName.Equals(username)) message.SenderDeleted = true;

               if (message.RecipientUserName.Equals(username)) message.RecipientDeleted = true;

               if (message.SenderDeleted && message.RecipientDeleted)
                    _unitOfWork.Repository<EmailMessage>().Delete(message);

               if (await _unitOfWork.Complete() > 0) return Ok();

               return BadRequest("Problem deleting the message");
          }
*/
          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpGet("ComposeCVAcknToCandidateByEmail")]
          public async Task<EmailMessage> ComposeCVAcknEmailMessage(CandidateMessageParamDto paramDto)
          {
               var msg = await _composeMsgHR.ComposeHTMLToAckToCandidateByEmail(paramDto);
               var msgToReturn = new EmailMessage();
               var AttachmentFilePaths = new List<string>();
               if (paramDto.DirectlySendMessage)
               {
                    msgToReturn = _emailService.SendEmail(msg, AttachmentFilePaths);
                    //msgToReturn.MessageSentOn = System.DateTime.Now;
               }

               return msgToReturn;
          }
          
          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpGet("ComposeSelAdvToCandidateByEmail")]
          public async Task<ICollection<EmailMessage>> ComposeSelAdviseToCandidateByemail(ICollection<SelectionDecisionMessageParamDto> paramDto)
          {
               var msgs = await _composeMsgAdmin.AdviseSelectionStatusToCandidateByEmail(paramDto);
               var AttachmentFilePaths = new List<string>();
               foreach(var msg in msgs)
               {
                    _emailService.SendEmail(msg, AttachmentFilePaths);                    
               }

               return msgs;
          }

          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpGet("ComposeRejAdvToCandidateByEmail")]
          public ICollection<EmailMessage> ComposeRejAdviseToCandidateByemail(ICollection<RejDecisionToAddDto> paramDto)
          {
               var msgs = _composeMsgAdmin.AdviseRejectionStatusToCandidateByEmail(paramDto);
               var AttachmentFilePaths = new List<string>();
               foreach(var msg in msgs)
               {
                    _emailService.SendEmail(msg, AttachmentFilePaths);
               }

               return msgs;
          }

          [Authorize(Roles = "DocumentControllerProcess, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpGet("ComposeProcessAdvToCandidateByEmail")]
          public async Task<EmailMessage> ComposeProcessAdviseToCandidateByemail(DeployMessageParamDto paramDto)
          {
               var msg = await _composeMsgProcess.AdviseProcessTransactionUpdatesToCandidateByEmail(paramDto);
               var msgToReturn = new EmailMessage();
               var AttachmentFilePaths = new List<string>();
               if (paramDto.DirectlySendMessage)
               {
                    msgToReturn = _emailService.SendEmail(msg, AttachmentFilePaths);
                    //msgToReturn.MessageSentOn = System.DateTime.Now;
               }
               return msgToReturn;
          }

          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpGet("ComposeCVAcknToCandidateBySMS")]
          public async Task<SMSMessage> ComposeCVAcknBySMS(CandidateMessageParamDto paramDto)
          {
               var msg = await _composeMsgHR.ComposeMsgToAckToCandidateBySMS(paramDto);
               if (paramDto.DirectlySendMessage)
               {
                    _smsService.sendMessage(msg.PhoneNo, msg.SMSText);
                    msg.DeliveryResult = "success";
               }

               return msg;
          }

          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpGet("ComposeSelAdvToCandidateBySMS")]
          public async Task<SMSMessage> ComposeSelAdviseToCandidateBySMS(SelectionDecisionMessageParamDto paramDto)
          {
               var msg = await _composeMsgAdmin.AdviseSelectionStatusToCandidateBySMS(paramDto);
               if (paramDto.DirectlySendMessage)
               {
                    _smsService.sendMessage(msg.PhoneNo, msg.SMSText);
                    msg.DeliveryResult = "success";
               }

               return msg;
          }

          [Authorize(Roles = "HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpGet("ComposeRejAdvToCandidateBySMS")]
          public async Task<SMSMessage> ComposeRejAdviseToCandidateBySMS(SelectionDecisionMessageParamDto paramDto)
          {
               var msg = await _composeMsgAdmin.AdviseRejectionStatusToCandidateBySMS(paramDto);
               if (paramDto.DirectlySendMessage)
               {
                    _smsService.sendMessage(msg.PhoneNo, msg.SMSText);
                    msg.DeliveryResult = "success";
               }

               return msg;
          }

          [Authorize(Roles = "DocumentControllerProcess, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpGet("ComposeProcessAdvToCandidateBySMS")]
          public async Task<SMSMessage> ComposeProcessAdviseToCandidateBySMS(DeployMessageParamDto paramDto)
          {
               var msg = await _composeMsgProcess.AdviseProcessTransactionUpdatesToCandidateBySMS(paramDto);
               if (paramDto.DirectlySendMessage)
               {
                    _smsService.sendMessage(msg.PhoneNo, msg.SMSText);
                    msg.DeliveryResult = "success";
               }

               return msg;
          }


     }
}