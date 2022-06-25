using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using core.Entities.EmailandSMS;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace infra.Services
{
     public class EmailService : IEmailService
     {
          private readonly IComposeMessages _composemsg;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IConfiguration _config;
          private readonly ATSContext _context;
          
          public EmailService(ATSContext context, IComposeMessages composemsg, IUnitOfWork unitOfWork, IConfiguration config)
          {
               _context = context;
               _config = config;
               _unitOfWork = unitOfWork;
               _composemsg = composemsg;
          }

          public async Task<Pagination<EmailMessage>> GetEmailMessageOfLoggedinUser(EmailMessageSpecParams msgParams)
          {
               var qryCt = _context.EmailMessages.AsQueryable();
               var qrymsg = _context.EmailMessages.AsQueryable();
               var msgs = new List<EmailMessage>();

               switch(msgParams.Container.ToLower())
               {
                    case "inbox":
                         qryCt = _context.EmailMessages.Where(x => 
                              x.RecipientEmailAddress == msgParams.Username
                              && x.RecipientDeleted==false);

                         qrymsg = qryCt.OrderByDescending(x => x.DateReadOn)
                              .Take(msgParams.PageSize)
                              .Skip((msgParams.PageIndex-1) * msgParams.PageSize);
                         break;
                    case "sent":
                         qryCt = _context.EmailMessages.Where(x => 
                              x.SenderEmailAddress == msgParams.Username
                              && x.SenderDeleted==false && x.MessageSentOn != null );
                         qrymsg = qryCt
                              .OrderByDescending(x => x.MessageSentOn)
                              .Skip((msgParams.PageIndex-1) * msgParams.PageSize)
                              .Take(msgParams.PageSize);
                         break;
                    case "draft":
                         qryCt = _context.EmailMessages.Where(x => x.SenderEmailAddress == msgParams.Username 
                              && x.SenderDeleted==false && x.MessageSentOn==null);
                         qrymsg = qryCt
                              .OrderByDescending(x => x.MessageSentOn)
                              .Skip((msgParams.PageIndex-1) * msgParams.PageSize)
                              .Take(msgParams.PageSize);
                         break;
                    default:
                         break;
               }

          
               var totalItems = await qryCt.CountAsync();
               if (totalItems > 0) {
                    msgs = await qrymsg.ToListAsync();     
               } 

               return new Pagination<EmailMessage>(msgParams.PageIndex, msgParams.PageSize, totalItems, msgs );
          /*
               
               var spec = new EmailMessagesForUserSpecs(msgParams);
               var countSpec = new EmailMessagesForUserCountSpecs(msgParams);

               var totalItems = await _unitOfWork.Repository<EmailMessage>().CountAsync(countSpec);
               var msgs = await _unitOfWork.Repository<EmailMessage>().ListAsync(spec);
               
               if (msgs==null || msgs.Count==0) return null;
          */
               //var data = _mapper.Map<IReadOnlyList<EmailMessageDto>>(msgs);
               
               //return new Pagination<EmailMessage>(msgParams.PageIndex, msgParams.PageSize, totalItems, msgs);
          }


          /*
               public async Task<EmailDto> AcknowledgeApplicationToCandidateByEmail(Candidate candidate)
               {
               if (string.IsNullOrEmpty(candidate.Email)) return null;
               var content = await _composemsg.AckToCandidate(candidate, "mail", "acknowledgetocandidate", true);
               EmailMessage email = new EmailMessage{
                    RecipientName = candidate.FullName,
                    RecipientEmailAddress = candidate.Email,
                    Subject = "Your Application is registered with us, with registration No. " + candidate.ApplicationNo,
                    MessageContent = content
               };

               var result = SendEmail(email);
               if (result) {
                    return new EmailDto {
                         RecipientEmailAddress = email.RecipientEmailAddress,
                         Subject = email.Subject,
                         MessageContent = email.MessageContent
                    };
               } else {
                    return null;
               }
               }

               public Task<PhoneDto> AcknowledgeApplicationToCandidateByPhone(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<SMSDto> AcknowledgeApplicationToCandidateBySMS(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<EmailDto> AdviseApplicationReferralToCandidateByEmail(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<PhoneDto> AdviseApplicationReferralToCandidateByPhone(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<SMSDto> AdviseApplicationReferralToCandidateBySMS(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<EmailDto> AdviseProcessUpdateToCandidateByEmail(Deploy deploy)
               {
               throw new System.NotImplementedException();
               }

               public Task<PhoneDto> AdviseProcessUpdateToCandidateByPhone(Deploy deploy)
               {
               throw new System.NotImplementedException();
               }

               public Task<SMSDto> AdviseProcessUpdateToCandidateBySMS(Deploy deploy)
               {
               throw new System.NotImplementedException();
               }

               public Task<EmailDto> AdviseSelectionRejectionToCandidateByEmail(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<PhoneDto> AdviseSelectionRejectionToCandidateByPhone(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }

               public Task<SMSDto> AdviseSelectionRejectionToCandidateBySMS(Candidate candidate)
               {
               throw new System.NotImplementedException();
               }
          */

          public async Task<EmailMessage> SaveEmailMessage(EmailMessage message)
          {
               if (message.Id == 0) {
                    _unitOfWork.Repository<EmailMessage>().Add(message);
               } else {
                    _unitOfWork.Repository<EmailMessage>().Update(message);
               }
               if(await _unitOfWork.Complete() == 0) {
                    return null;
               } else {
                    return message;
               }
          }
          public EmailMessage SendEmail(EmailMessage msg, ICollection<string> filePaths)
          {
               //save the message in repository
               /* _unitOfWork.Repository<EmailMessage>().Add(msg);
               if (await _unitOfWork.Complete() > 0)
               {
               */
                    var msgToReturn = msg;
                    var smtp =  _config.GetSection("Host").Value;
                    var displayName = _config.GetSection("DisplayName").Value;
                    var fromemailId = _config.GetSection("Mail").Value;
                    var portNo = Convert.ToInt32(_config.GetSection("Port").Value);
                    var password = _config.GetSection("Password").Value;

                    var mailMessage = new MimeMessage();
                    mailMessage.From.Add(new MailboxAddress("", fromemailId));
                    mailMessage.Subject = msg.Subject;
                    mailMessage.Body = new TextPart("html")
                    {
                         Text = msg.Content
                    };

                    string[] arrayTo = msg.RecipientEmailAddress.Split(",");
                    string[] arrayCC = msg.CcEmailAddress.Split(",");
                    string[] arrayBCC = msg.BccEmailAddress.Split(",");
                    foreach(var address in arrayTo){
                         if (!string.IsNullOrWhiteSpace(address)) mailMessage.To.Add(new MailboxAddress("", address.Trim()));
                    }
                    foreach(var address in arrayCC){
                         if (!string.IsNullOrWhiteSpace(address)) mailMessage.Cc.Add(new MailboxAddress(address.Trim()));
                    }
                    mailMessage.Cc.Add(new MailboxAddress("Munir", "munir.sheikh@live.com"));
                    foreach(var address in arrayBCC){
                         if (!string.IsNullOrWhiteSpace(address)) mailMessage.Bcc.Add(new MailboxAddress(address.Trim()));
                    }
                    
                    /* foreach(var filePath in filePaths)
                    {
                         Attachment attachment = new Attachment(filePath);
                         Attachment oAttch = new Attachment(filePath, MailEncoding.Base64);
                    }
                    */
                    using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                    {
                         smtpClient.Connect(smtp, portNo, true);    //port 465 for plain messages
                         smtpClient.Authenticate(fromemailId, password);
                         smtpClient.Send(mailMessage);
                         smtpClient.Disconnect(true);
                    }

                    return msgToReturn;
               //}
               
          }

          public Task SendEmailAsync(EmailMessage emailMessage)
          {
               throw new System.NotImplementedException();
          }

          /*
          public async Task EmailSend(EmailMessage messageModel)
          {
               using (EmailMessage mailMessage = new EmailMessage())
               {
                    mailMessage.From = new MailAddress(_configuration.GetSection("EmailConfiguration").GetSection("FromEmail").Value.ToString(), _configuration.GetSection("EmailConfiguration").GetSection("FromName").Value.ToString(), Encoding.UTF8);
                    mailMessage.Subject = messageModel.Subject;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.Body = messageModel.Content;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyTransferEncoding = TransferEncoding.Base64;
                    mailMessage.To.Add(new MailAddress(messageModel.To));
                    NetworkCredential networkCredential = new NetworkCredential(_configuration.GetSection("EmailConfiguration").GetSection("Username").Value.ToString(), _configuration.GetSection("EmailConfiguration").GetSection("Password").Value.ToString());
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = _configuration.GetSection("EmailConfiguration").GetSection("SmtpServer").Value.ToString();
                    smtpClient.EnableSsl = Convert.ToBoolean(_configuration.GetSection("EmailConfiguration").GetSection("SSL").Value);
                    smtpClient.UseDefaultCredentials = Convert.ToBoolean(_configuration.GetSection("EmailConfiguration").GetSection("UseDefaultCredentials").Value);
                    smtpClient.Port = Convert.ToInt32(_configuration.GetSection("EmailConfiguration").GetSection("Port").Value);
                    smtpClient.Credentials = networkCredential;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtpClient.SendMailAsync(mailMessage);
               }
          }
          */

     }
}