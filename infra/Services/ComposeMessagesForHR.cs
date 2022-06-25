using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
    public class ComposeMessagesForHR: IComposeMessagesForHR
{
        private const string _smsNewLine = "<smsbr>";
        private const string _WAPNewLine = "<smsbr>";
        private readonly IEmployeeService _empService;
        private readonly ATSContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommonServices _commonServices;
        private readonly IComposeMessages _composeMessages;
        private readonly IConfiguration _confg;
        private readonly IUnitOfWork _unitOfWork;
        public ComposeMessagesForHR(IEmployeeService empService, ATSContext context, 
            UserManager<AppUser> userManager, ICommonServices commonServices, 
            IComposeMessages composeMessages, IConfiguration confg, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _confg = confg;
            _composeMessages = composeMessages;
            _commonServices = commonServices;
            _userManager = userManager;
            _context = context;
            _empService = empService;
        }

        
        
        public Task<ICollection<EmailMessage>> ComposeMessagesToDocControllerAdminToForwardCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn)
        {
            throw new NotImplementedException();
        }
        public async Task<EmailMessage> ComposeHTMLToAckToCandidateByEmail(CandidateMessageParamDto candidateParam)
        {
            var candidate = candidateParam.Candidate;
            string msg = "";
            var candidateName = candidate.FullName;
            var email = candidate.Email;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string subject = "";

            subject = "Your Application is registered with us under Sr. No. " + candidate.ApplicationNo;
            msg = DateTime.Today + "<br><br>" + title + " " + candidateName + "<br><br>Dear " + title + " " + candidateName;
            msg += "Dear " + title + " " + candidateName + "<br><br>" + subject + "<br><br>";
            var msgs = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "acknowledgementtocandidate" && x.Mode == "mail")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgs)
            {
                msg += m.LineText == "tableofrelevantopenings"
                        ? await _composeMessages.TableOfRelevantOpenings(
                            candidate.UserProfessions.Select(x => x.CategoryId).ToList())
                        : m.LineText;
            }
            msg = msg + "<br><br>HR Supervisor";

            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = _confg["EmailSenderEmailId"],
                SenderUserName = _confg["EmailSenderDisplayName"],
                RecipientUserName = candidate.KnownAs,
                RecipientEmailAddress = email,
                CcEmailAddress = _confg["EmailCCandAck"],
                BccEmailAddress = _confg["EmailBCCandAck"],
                Subject = subject,
                Content = msg,
                MessageTypeId = (int)EnumMessageType.CVAcknowledgementByEMail
            };

            return emailMessage;
        }

        public async Task<SMSMessage> ComposeMsgToAckToCandidateBySMS(CandidateMessageParamDto candidateParam)
          {
               var candidate = candidateParam.Candidate;
               string msg = "";
               var candidateName = candidate.KnownAs;
               var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

               string subject = "";

               var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
               if (string.IsNullOrEmpty(mobileNo)) return null;

               subject = "Your Application registered with us under Sr. No. " + candidate.ApplicationNo;
               msg = DateTime.Today + _smsNewLine + _smsNewLine + title + " " + candidateName + _smsNewLine + _smsNewLine + "Dear " + title + " " + candidateName;
               var sms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "acknowledgementtocandidate" && x.Mode == "sms")
                    .OrderBy(x => x.SrNo).ToListAsync();
               foreach (var m in sms)
               {
                    msg += m;
               }
               msg += _smsNewLine + _smsNewLine + "HR Supervisor";

               var smsMessage = new SMSMessage
               {
                    UserId = candidate.AppUserId,
                    PhoneNo = mobileNo,
                    SMSText = msg
               };

               return smsMessage;
          }

        public async Task<EmailMessage> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId)
          {
               var emp = await _empService.GetEmployeeBriefAsyncFromAppUserId(loggedInDto.LoggedInAppUserId);
               var recipient = await _empService.GetEmployeeBriefAsyncFromEmployeeId(recipientId);

               var msg = DateTime.Now.Date + "<br><br>"+ emp.EmployeeName + "<br>" + emp.Position + "<br>email:"+ emp.Email +
                    "Following Applications are cleared for forwarding to respective clients by the Document Controller:<br><br>" +
                    "<table border='1'><tr><th>App No</th><th>Candidate Name</th><th>Category Ref</th><th>Customer</th><th>Submitted by</th></tr>";
               foreach(var item in commonDataDtos)
               {
                    msg += "<tr><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + "</td><td>" + 
                         item.CategoryName+"</td><td>" + item.CustomerName + "</td><td>" + emp.EmployeeName + "</td></tr>";
               }

               msg +="</table>Regards<br><br>This is system generated message";

               var email = new EmailMessage("advisory", loggedInDto.LoggedInAppUserId, recipient.AppUserId, loggedInDto.LoggedInAppUserEmail, 
                    loggedInDto.LoggedIAppUsername, recipient.KnownAs, recipient.Email, "", "", 
                    "Applications readied to forward to client - initiated by " + emp.EmployeeName, msg, 
                    (int)EnumMessageType.CVForwardingToDocControllerToFwdCVToClient,3);
               
               return email;
          }

       
        
        private ICollection<EmailMessage> ComposeHTMLForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents,  EmployeeDto senderObj, int loggedInUserId)
        {
            
            var catTable = ComposeCategoryTableForDLFwd(itemsAndAgents.Items);
            
            var msgs = new List<EmailMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = DateTime.Now + "<br><br>" + agent.Title + " " + agent.CustomerOfficialName;
                if(!string.IsNullOrEmpty(agent.OfficialDesignation)) hdr +=", " + agent.OfficialDesignation;
                hdr +="<br>" + agent.CustomerName + "<br>" + agent.CustomerCity;
                hdr += "email: " + agent.OfficialEmailId + "<br><br>";
                hdr += "Sub.: Manpower requirement for " + agent.CustomerCity;
                hdr += "<br><br>We have following manpower requirement.  If you have interested candidates, please refer them to us<br><br>";
                hdr += catTable + "<br><br>Regards<br><br>" + senderObj.EmployeeName + "<br>" + senderObj.Position;

                var message = new EmailMessage("DLFwdToAgent", loggedInUserId,
                        agent.OfficialId, agent.OfficialEmailId,
                        senderObj.OfficialEmailAddress, agent.CustomerName,
                        agent.OfficialEmailId, "","", "Requirement", hdr, 
                        (int)EnumMessageType.DLForwardToAgents,0 );
                msgs.Add(message);
            }

            return msgs;
        }

        private ICollection<SMSMessage> ComposeSMSForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents, EmployeeDto senderObj, int loggedInUserId)
        {
            var catTable = ComposeCategoryTableForDLFwdBySMS(itemsAndAgents.Items);
            string deliveryresult ="";
            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(loggedInUserId, itemsAndAgents.DateForwarded, agent.Phoneno, hdr, deliveryresult);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            
            return null;
        }

        private ICollection<SMSMessage> ComposeWhatsAppForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents, EmployeeDto senderObj, int loggedInUserId)
        {
            var catTable = ComposeCategoryTableForDLFwdByWhatsApp(itemsAndAgents.Items);
            string deliveryresult ="";
            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(loggedInUserId, itemsAndAgents.DateForwarded, agent.Phoneno, hdr, deliveryresult);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            return null;
        }

        public async Task<MessagesToReturnDto> ComposeHTMLToForwardEnquiryToAgents(OrderItemsAndAgentsToFwdDto itemsAndagents, int LoggedInUserId)
        {
            var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(
                itemsAndagents.Items.Select(x => x.ProjectManagerId).FirstOrDefault());
            if (senderObj == null) return null;

            var emailmsgs = new List<EmailMessage>();
            var smsmsgs = new List<SMSMessage>();
            var whatsappmsgs= new List<SMSMessage>();
            
            var msgsToReturnDto = new MessagesToReturnDto();

            var orderitemsandagentstofwd = new OrderItemsAndAgentsToFwdDto();
            var agentsByEmail = itemsAndagents.Agents.Where(x => x.Checked==true).ToList();
            if (agentsByEmail != null && agentsByEmail.Count > 0) {
                orderitemsandagentstofwd.Agents = agentsByEmail;
                orderitemsandagentstofwd.Items = itemsAndagents.Items;
                emailmsgs  = (List<EmailMessage>)ComposeHTMLForwards(orderitemsandagentstofwd, senderObj, LoggedInUserId);
                if (emailmsgs !=null && emailmsgs.Count() > 0) msgsToReturnDto.EmailMessages = emailmsgs;
            }
            
            var agentsBySMS = itemsAndagents.Agents.Where(x => x.CheckedPhone==true).ToList();
            if (agentsBySMS != null && agentsBySMS.Count > 0) {
                orderitemsandagentstofwd = new OrderItemsAndAgentsToFwdDto();
                orderitemsandagentstofwd.Agents = agentsBySMS;
                orderitemsandagentstofwd.Items = itemsAndagents.Items;
                smsmsgs  = (List<SMSMessage>)ComposeSMSForwards(orderitemsandagentstofwd, senderObj, LoggedInUserId);
                if (smsmsgs != null && smsmsgs.Count() > 0) msgsToReturnDto.SMSMessages = smsmsgs;
            }

            var agentsByWhatsApp = itemsAndagents.Agents.Where(x => x.CheckedMobile==true).ToList();
            if (agentsByWhatsApp != null && agentsByWhatsApp.Count > 0) {
                orderitemsandagentstofwd = new OrderItemsAndAgentsToFwdDto();
                orderitemsandagentstofwd.Agents = agentsByWhatsApp;
                orderitemsandagentstofwd.Items = itemsAndagents.Items;
                smsmsgs  = (List<SMSMessage>)ComposeWhatsAppForwards(orderitemsandagentstofwd, senderObj, LoggedInUserId);
                if (smsmsgs !=null && smsmsgs.Count() > 0) msgsToReturnDto.WhatsAppMessages = smsmsgs;
            }
            return msgsToReturnDto;
        }
        public async Task<ICollection<SMSMessage>> ForwardEnquiryToAgentsBySMS(OrderItemsAndAgentsToFwdDto itemsAndagents, int LoggedInUserId)
        {
            var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(
                itemsAndagents.Items.Select(x => x.ProjectManagerId).FirstOrDefault());
            if (senderObj == null) return null;

            var catTable = ComposeCategoryTableForDLFwdBySMS(itemsAndagents.Items);
            string deliveryresult ="";
            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndagents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(LoggedInUserId, itemsAndagents.DateForwarded, agent.Phoneno, hdr, deliveryresult);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            
            if (await _unitOfWork.Complete() > 0) return (ICollection<SMSMessage>)msgs;
            return null;
        }

        private string ComposeCategoryTableForDLFwd(ICollection<OrderItemToFwdDto> orderitems) {
               
               var para = "<Table border='1'><tr><th>Order Ref</th><th>Customer</th>" +
                    "<th>Category</th><th>Quantity</th><th>Basic Salary</th>" +
                    "<th>Job Description</th>" +
                    "<th>Remuneration and Facilities</th></tr>";
               
               foreach(var item in orderitems ) {
                    para += "<tr><td>" + item.CategoryRef + "</td><td>" + item.CustomerName + "</td><td>" +
                         item.CategoryName + "</td><td>" + item.Quantity + "</td><td>" +
                         item.SalaryCurrency + " " + 
                         item.BasicSalary  + "<td><td>" +
                         item.JobDescriptionURL + "</td><td>" + 
                         item.RemunerationURL + "</td></tr>";
               }
               para += "</table>";

               return para;
          }

        private string ComposeCategoryTableForDLFwdBySMS(ICollection<OrderItemToFwdDto> orderitems) {
               
            int srno=0;              
            string para="";
            foreach(var item in orderitems ) {
                para += ++srno + ". Ref:" + item.CategoryRef + item.CategoryName + _smsNewLine +
                        "Customer: " + item.CustomerName + _smsNewLine +
                        "Qnty: " + item.Quantity + _smsNewLine +
                        "Salary: " + item.SalaryCurrency + " " + item.BasicSalary  + _smsNewLine +
                        "Job Description: " + item.JobDescriptionURL + _smsNewLine + 
                        "Remuneration: " + item.RemunerationURL  + _smsNewLine;
            }

            return para;
        }

        private string ComposeCategoryTableForDLFwdByWhatsApp(ICollection<OrderItemToFwdDto> orderitems) {
               
            int srno=0;              
            string para="";
            foreach(var item in orderitems ) {
                para += ++srno + ". Ref:" + item.CategoryRef + item.CategoryName + _WAPNewLine +
                        "Customer: " + item.CustomerName + _WAPNewLine +
                        "Qnty: " + item.Quantity + _WAPNewLine +
                        "Salary: " + item.SalaryCurrency + " " + item.BasicSalary  + _WAPNewLine +
                        "Job Description: " + item.JobDescriptionURL + _WAPNewLine + 
                        "Remuneration: " + item.RemunerationURL  + _WAPNewLine;
            }

            return para;
        }

    }
    
}