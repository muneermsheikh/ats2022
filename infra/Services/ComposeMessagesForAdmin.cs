using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
    public class ComposeMessagesForAdmin : IComposeMessagesForAdmin
    {
            private const string _smsNewLine = "<smsbr>";
            private readonly IEmployeeService _empService;
            private readonly ATSContext _context;
            private readonly IComposeMessages _commonMessages;
            private readonly IConfiguration _confg;
            private readonly ICommonServices _commonServices;
            private readonly int _default_HRSupId=4;
            private readonly int _default_ProjManagerId=2;

        public ComposeMessagesForAdmin( IEmployeeService empService, ATSContext context, IComposeMessages commonMessages, IConfiguration confg, ICommonServices commonServices )
        {
               _commonServices = commonServices;
               _confg = confg;
               _commonMessages = commonMessages;
               _context = context;
               _empService = empService;
        }

        public async Task<ICollection<EmailMessage>> AdviseSelectionStatusToCandidateByEmail(ICollection<SelectionDecisionMessageParamDto> selectionsDto)
          {
               var sels = selectionsDto.Select(x => x.SelectionDecision).ToList();
               var qry = (from s in sels 
                    join employ in _context.Employments on s.CVRefId equals employ.CVRefId into employmt 
                    from employ in employmt.DefaultIfEmpty()
                    join c in _context.Candidates on s.CandidateId equals c.Id
                    join i in _context.OrderItems on s.OrderItemId equals i.Id
                    join e in _context.Employees on i.HrExecId equals e.Id into empJoin
                    from emp in empJoin.DefaultIfEmpty()
                    select new {employment = employ, selDecs=s, Gender=c.Gender, candidateEmail=c.Email, candidateKnownAs=c.KnownAs, 
                        HRExecId=i.HrExecId, HRExecEmail=emp?.Email, HRExecKnownAs=emp?.KnownAs })
                    .ToList();

                string msg = "";
                var msgs = new List<EmailMessage>();
                string subject = "";
                string msgToEmail;
                string msgToKnownAs;
                string msgToTitle;
                string candidateTitle="Mr.";
                int msgToEmpId;
               foreach(var sel in qry)
               {
                    //address email to HRSup if HRExecId is not defined
                    if(sel.HRExecId == 0) {
                        var sup = await _context.Employees.FindAsync(_default_HRSupId);
                        if(sup==null) continue;
                        msgToEmpId= _default_HRSupId;
                        msgToKnownAs = sup.KnownAs;
                        msgToEmail = sup.Email;
                        msgToTitle = sup.Gender == "M" ? "Mr. " : "Ms. ";
                    } else {
                        msgToEmpId=(int)sel.HRExecId;
                        if(string.IsNullOrEmpty(sel.HRExecEmail)) continue;
                        msgToKnownAs= sel.HRExecKnownAs;
                        msgToEmail = sel.HRExecEmail;
                        msgToTitle = sel.Gender == "M" ? "Mr. " : "Ms. ";
                    }

                    subject = "<b><u>Subject: </b>Your selection as " + sel.selDecs?.CategoryName + " for " + sel.selDecs?.CustomerName + "</u>";
                    msg = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + 
                         candidateTitle + " " + sel.selDecs.CandidateName + "email: " + msgToEmail + "<br><br>" + 
                         "copy: " + sel.HRExecKnownAs + ", email: " + msgToEmail + "<br><br>Dear " + candidateTitle + " " + sel.selDecs?.CandidateName + ":" +
                         "<br><br>" + subject + "<br><br>";

                    //MessageComposeSources contains collection of static text lines for each type of message.
                    var msgLines = await _context.MessageComposeSources
                         .Where(x => x.MessageType.ToLower() == "selectionadvisetocandidate" && x.Mode == "mail")
                         .OrderBy(x => x.SrNo).ToListAsync();
                    foreach (var m in msgLines)
                    {
                         //if m.LineText equals "<tableofselectiondetails>", then it is a dynamic data, to be
                         //retreived from SelectionDecision, else accept the static data m.LineText
                         if(sel.employment != null) {
                            msg += m.LineText == "<tableofselectiondetails>" 
                            ? _commonMessages.GetSelectionDetails(sel.selDecs?.CandidateName, (int)sel.selDecs?.ApplicationNo, 
                                    sel.selDecs?.CustomerName, sel.selDecs?.CategoryName, sel.employment)
                            : m.LineText;
                            msg += "<br>Best Regards<br>HR Supervisor";

                            var emailMessage = new EmailMessage
                            {
                                RecipientId = msgToEmpId,
                                RecipientUserName = sel.selDecs.CandidateName,
                                RecipientEmailAddress = msgToEmail + ", " + sel.HRExecEmail,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                                CcEmailAddress = sel.HRExecEmail,
                                BccEmailAddress = "",
                                Subject = subject,
                                Content = msg,
                                MessageTypeId = (int)EnumMessageType.SelectionAdvisebyemail
                            };

                            msgs.Add(emailMessage);

                         }
                    }
               }

               if (msgs.Count > 0) return msgs;
               return null;
          }
        public async Task<SMSMessage> AdviseSelectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selectionParam)
          {
               var selection = selectionParam.SelectionDecision;
               var candidate = await (from cvref in _context.CVRefs
                        where cvref.Id == selection.CVRefId
                        join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                        select cand).FirstOrDefaultAsync();

               var candidateName = candidate.KnownAs;
               var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

               string msg = "";

               var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
               if (string.IsNullOrEmpty(mobileNo)) return null;

               msg = "Dear " + title + " " + candidateName + _smsNewLine + _smsNewLine;
               var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "selectionadvisetocandidate" && x.Mode == "sms")
                    .OrderBy(x => x.SrNo).ToListAsync();
               foreach (var m in msgssms)
               {
                    msg += m.LineText == "<tableofselectiondetailssms>" ? 
                        _commonMessages.GetSelectionDetailsBySMS(selection) : m.LineText;
               }
               msg += msg + "<br><br>HR Supervisor";

               var smsMessage = new SMSMessage
               {
                    UserId = candidate.AppUserId,
                    PhoneNo = mobileNo,
                    SMSText = msg
               };

               return smsMessage;
          }

        public ICollection<EmailMessage> AdviseRejectionStatusToCandidateByEmail(ICollection<RejDecisionToAddDto> rejectionsDto)
        {
            //var rejs = rejectionsDto.Select(x => x.SelectionDecision).ToList();
            var qry = (from r in rejectionsDto
                join rf in _context.CVRefs on r.CVRefId equals rf.Id
                join c in _context.Candidates on rf.CandidateId equals c.Id
                join i in _context.OrderItems on rf.OrderItemId equals i.Id
                join e in _context.Employees on i.HrExecId equals e.Id
                select new {rf.CandidateName, rf.CategoryName, rf.CustomerName, r.SelectionStatusId,
                        c.Gender, c.Email, c.KnownAs, c.FullName, 
                        HRExecKnownAs = e.KnownAs, HRExecEmail = e.Email, c.AppUserId, 
                        CandidateEmail = c.Email})
                .ToList();

            var msg = "";
            var msgs = new List<EmailMessage>();
            var subject = "";

            foreach(var rej in qry)
            {
                if (string.IsNullOrEmpty(rej.Email)) continue;

                var title = rej.Gender.ToLower() == "m" ? "Mr. " : "Ms. ";
                subject = "<u><b>Subject</b>: Your candidature for the position of " + rej.CategoryName + 
                        " <b>has not been approved</b> by " + rej.CustomerName + "</u>";
                msg = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + title + " " + 
                        rej.CandidateName +  ", email: " + rej.Email + "<br><br>Dear " + title + " " + rej.CandidateName +
                        "<br><br>" + subject + "<br><br>";
                msg += "We regret to inform you that M/S " + rej.CustomerName + " have not approved of your candidature for the position " +
                        "of " + rej.CategoryName + " giving following reason:<br><ul><li>" + rej.SelectionStatusId + "</li></ul><br>";
                msg += "The rejection by our Customer only indicates your profile did not meet their specific requirements; it " +
                        "does not reflect your suitability for the position in general. We will therefore be continuing to look for " +
                        "alternate opportunities for you and hope to revert to you as soon as possible.<br><br>" + 
                        "In case you do not want us to continue looking for opportunities for you, please do mark yourself as unavailable on our website https://idealsoln.in/candidates/notavailable" + 
                            "so as not to include your profile in our searches<br><br>Best regards/HR Supervisor";
                msg += "<br><br>This is a system generated message";

                var emailMessage = new EmailMessage
                {
                        RecipientId = rej.AppUserId,
                        RecipientUserName = rej.Email,
                        RecipientEmailAddress = rej.Email,
                        CcEmailAddress = "",
                        BccEmailAddress = "",
                        Subject = subject,
                        Content = msg,
                        MessageTypeId = (int)EnumMessageType.RejectionAdviseByMail
                };

                msgs.Add(emailMessage);
            }

            if (msgs.Count > 0) return msgs;
            return null;
            
        }
        public async Task<SMSMessage> AdviseRejectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selectionParam)
        {
            var selection = selectionParam.SelectionDecision;
            var candidate = await (from cvref in _context.CVRefs
                                where cvref.Id == selection.CVRefId
                                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                                select cand).FirstOrDefaultAsync();

            var candidateName = candidate.KnownAs;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string msg = "";

            var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
            if (string.IsNullOrEmpty(mobileNo)) return null;

            msg = "Yr candidature for the position of " + selection.CategoryName + _smsNewLine;
            var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "rejectionadvisetocandidate" && x.Mode == "sms")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgssms)
            {
                msg += m.LineText;
            }
            msg = msg + "<br><br>HR Supervisor";

            var smsMessage = new SMSMessage
            {
                UserId = candidate.AppUserId,
                PhoneNo = mobileNo,
                SMSText = msg
            };

            return smsMessage;
        }

        public async Task<EmailMessage> AckEnquiryToCustomer(OrderMessageParamDto orderMessageDto)
        {
            var order = orderMessageDto.Order;
            var customer = await _context.Customers.Where(x => x.Id == orderMessageDto.Order.CustomerId)
                .Include(x => x.CustomerOfficials).FirstOrDefaultAsync();
            if (customer==null) throw new Exception("failed to retrieve customer data for customer no. " + orderMessageDto.Order.CustomerId);
            var OrderItems = orderMessageDto.Order.OrderItems.OrderBy(x => x.SrNo).ToList();
            var projectManagerId = order.ProjectManagerId == 0 ? _default_ProjManagerId : order.ProjectManagerId;
            EmployeeDto projManager = await _empService.GetEmployeeFromIdAsync(projectManagerId);

            string[] officialDepts = { "main contact", "hr", "accounts", "logistics" };
            CustomerOfficial official = null;
            foreach (var off in officialDepts)
            {
                official = customer.CustomerOfficials.Where(x => x.Divn?.ToLower() == off).FirstOrDefault();
                if (official != null) break;
            }
            bool HasException = false;
            var msg = DateTime.Now.Date + "<br><br>M/S" + customer.CustomerName;
            if (!string.IsNullOrEmpty(customer.Add)) msg += "<br>" + customer.Add;
            if (!string.IsNullOrEmpty(customer.Add2)) msg += "<br>" + customer.Add2;
            msg += "<br>" + customer.City + ", " + customer.Country + "<br><br>";
            msg += official == null ? "" : "Kind Attn : " + official.Title + official.OfficialName + ", " + official.Designation + "<br><br>";
            msg += "Dear " + official?.Gender == "F" ? "Madam:" : "Sir:" + "<br><br>";
            msg += "Thank you very much for your manpower enquiry dated " + order.OrderDate.Date + " for following personnel: ";
            msg += "<br><br>" + _commonMessages.ComposeOrderItems(order.OrderNo, OrderItems, HasException) + "<br><br>";
            msg += HasException == true
                ? "Please note the exceptions mentioned under the column <i>Exceptions</i> and respond ASAP.  " +
                        "We will initiate execution of the wroks at this end on receipt of your clarificatins.<br><br>"
                : "We have initiated the works, and will revert to you soon with our delivery plan.<br><br>";
            msg += "Your point of contact for this order execution shall be the undersigned<br><br>";
            msg += "Please feel free to reach me for any clarification.<br><br>Best regards<br><br>" +
                projManager.EmployeeName + "<br>" + projManager.Position + "<br>" + _confg.GetSection("IdealUserName").Value;
            msg += string.IsNullOrEmpty(projManager.OfficialPhoneNo) == true ? "" : "<br>Phone: " + projManager.OfficialPhoneNo;
            msg += string.IsNullOrEmpty(projManager.OfficialMobileNo) == true ? "" : "<br>Mobile: " + projManager.OfficialMobileNo;
            msg += string.IsNullOrEmpty(projManager.OfficialEmailAddress) == true ? "" : "<br>Email: " + projManager.OfficialEmailAddress;

            var senderEmailAddress = _confg["EmailSenderEmailId"] ?? "";
            var senderUserName = _confg["EmailSenderDisplayName"] ?? "";
            var recipientUserName = customer.CustomerName ?? "";
            var recipientEmailAddress = official?.Email ?? "";
            var ccEmailAddress = _confg["EmailCCandAck"] ?? "";
            var bccEmailAddress = _confg["EmailBCCandAck"] ?? "";
            var subject = "Your enquiry dated " + order.OrderDate.Date + " is registered by us under Serial No. " + order.OrderNo;
            var messageTypeId = (int)EnumMessageType.OrderAcknowledgement;
            
            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = senderEmailAddress,
                SenderUserName = senderUserName,
                RecipientUserName = recipientUserName,
                RecipientEmailAddress = recipientEmailAddress,
                CcEmailAddress = ccEmailAddress,
                BccEmailAddress = bccEmailAddress,
                Subject = subject,
                Content = msg,
                MessageTypeId = messageTypeId
            };

            return emailMessage;
        }

        public async Task<EmailMessage> ForwardEnquiryToHRDept(Order order)
        {
            string msg = "";
            var HRSup = _confg.GetSection("EmpHRSupervisorId").Value;
            int HRSupId = HRSup == null ? 0 : Convert.ToInt32(HRSup);
            var hrObj = await _empService.GetEmployeeFromIdAsync(HRSupId);
            int projMgrId = order.ProjectManagerId == 0 ? 1 : order.ProjectManagerId;       //todo - correct this
            var projMgr = await _empService.GetEmployeeFromIdAsync(projMgrId);
            var cust = await _commonServices.CustomerBriefDetailsFromCustomerId(order.CustomerId);

            msg = DateTime.Now.Date + "<br><br>" + hrObj.EmployeeName + ", " + hrObj.Position + "<br>" +
                "<br>HR Supervisor<br>Email: " + hrObj.OfficialEmailAddress + "<br><br>";
            if (order.ForwardedToHRDeptOn == null || ((DateTime)(order.ForwardedToHRDeptOn)).Date.Year < 200)
            {
                msg += "Following requirement is forwarded to you for execution within time periods shown:<br><br>";
            }
            else
            {
                msg += "Following requirement forwarded to you on " +
                        ((DateTime)order.ForwardedToHRDeptOn).Date + " <b><font color='blue'>is revised</font></b>as follows:<br><br>";
            }
            msg += "Order No.:" + order.OrderNo + " dated " + order.OrderDate +
                "<br>Customer:" + cust.CustomerName + ", " + cust.City + ", Place of work: " + order.CityOfWorking;
            msg += "<br><br>Overall Project Completion target: " + order.CompleteBy.Date + "<br><br>";
            msg += "Requirement details are as follows.  For Job Description, click the relevant link<br>";

            var itemids = await _context.OrderItems.Where(x => x.OrderId == order.Id).Select(x => x.Id).ToListAsync();
            var tbl = _commonMessages.TableOfOrderItemsContractReviewedAndApproved(itemids);
            msg += tbl;
            msg += "<br><br>Task for this requirement is also assigned to you.<br><br>" + projMgr.KnownAs +
                "<br>Project Manager-Order" + order.OrderNo;

            var emailMsg = new EmailMessage("forwardToHR", projMgr.EmployeeId, hrObj.EmployeeId, projMgr.OfficialEmailAddress,
                projMgr.UserName, hrObj.UserName, hrObj.OfficialEmailAddress, "", "", "New Requirement No. " + order.OrderNo,
                msg, (int)EnumMessageType.RequirementForwardToHRDept, 3);
            return emailMsg;
        }
    
        public async Task<ICollection<EmailMessage>> ComposeCVFwdMessagesToClient(ICollection<int> CVRefIds, LoggedInUserDto loggedIn)
        {
            DateTime dateTimeNow = DateTime.Now;
            var emails = new List<EmailMessage>();
            var uniqueorderitemids = await _context.CVRefs.Where(x => CVRefIds.Contains(x.Id)).Select(x => x.OrderItemId).Distinct().ToListAsync();

            var refdata = await ( _context.CVRefs.Where(a => uniqueorderitemids.Contains(a.OrderItemId))
                         .GroupBy(a => a.OrderItemId)
                         .Select(g => new { orderitemid= g.Key, refcount = g.Count() })).ToListAsync();
            var result = await( from r in _context.CVRefs where CVRefIds.Contains(r.Id)
                join i in _context.OrderItems on r.OrderItemId equals i.Id
                join cat in _context.Categories on i.CategoryId equals cat.Id
                join o in _context.Orders on i.OrderId equals o.Id
                join cand in _context.Candidates on r.CandidateId equals cand.Id
                join cust in _context.Customers on o.CustomerId equals cust.Id
                join off in _context.CustomerOfficials on cust.Id equals off.CustomerId 
                        where off.IsValid && off.Divn.ToLower()=="hr"
                join ass in _context.CandidateAssessments on new {a=r.CandidateId, b=r.OrderItemId} equals new {a=ass.CandidateId, b=ass.CandidateId} into g
                from grade in g.DefaultIfEmpty()
                select new CVFwdMsgDto {
                        CustomerId = o.CustomerId, CustomerName = cust.CustomerName, City = cust.City, 
                        OfficialId = off.Id, OfficialTitle = off.Title, OfficialName = off.OfficialName, 
                        Designation = off.Designation, OfficialEmail = off.Email, OfficialUserId = off.AppUserId,
                        OrderNo = o.OrderNo, OrderDated = o.OrderDate.Date, ItemSrNo = i.SrNo, CategoryName = cat.Name,
                        ApplicationNo = cand.ApplicationNo, PPNo = cand.PpNo, CandidateName = cand.FullName,
                        OrderItemId = i.Id,
                        //AssessmentGrade = grade.AssessResult 
                }).ToListAsync();

            foreach(var r in result)
            {
                var data = refdata.Where(x => x.orderitemid == r.OrderItemId).Select(x => x.refcount).FirstOrDefault();
                r.CumulativeSentSoFar = data;

                if (r.OfficialId == 0) {
                    var ids = await _context.CustomerOfficials.Where(x => x.CustomerId == r.CustomerId).ToListAsync();
                    foreach(var id in ids)
                    {
                        switch (id.Divn.ToLower()) {
                            case "admin":
                                r.OfficialId=id.Id;
                                break;
                            case "account":
                                r.OfficialId=id.Id;
                                break;
                            case "logistics":
                                r.OfficialId=id.Id;
                                break;
                            
                        }
                        if (r.OfficialId != 0) break;
                    }
                }
                //r.OfficialId = r.OfficialId == 0 ? 2 : r.OfficialId;
            }
            
            //extract official data from the result set
            var officials = result.Select(x => new 
                {x.OfficialId, x.OfficialTitle, x.OfficialName, x.CustomerName, 
                        x.City, x.OfficialEmail, x.Designation, x.OfficialUserId}).ToList();

            foreach(var off in officials)
            {
                var msg = dateTimeNow.Date + "<br><br>"+  off.OfficialTitle + " " + off.OfficialName + ", " + 
                        off.Designation + "<br>M/S " + off.CustomerName + ", " + off.City + "<br>Email:" + off.OfficialEmail +
                        "We are pleased to enclose following CVs for your consideration agaist your requirements mentioned.<br><br>" +
                        "<table border='1'><tr><th>Order ref and dated</th><th>Category</th>Application No</th><th>Candidate Name</th><th>" + 
                        "Passport No</th><th>Attachments</th><th>Forwarded<br>so far</th><th>Our assessment<br>Grade</th><th>Salary Expectation</th></tr>";
                var filtered = result.Where(x => x.OfficialId == off.OfficialId).ToList();
                int counter=0;
                foreach(var item in filtered)
                {
                        msg += "<tr><td>" + item.OrderNo +"-"+ item.ItemSrNo + "/<br>" + item.OrderDated + "</td><td>" +
                            item.CategoryName + "</td><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + 
                            "</td><td>"+ item.PPNo + "</td><td></td><td>" + item.CumulativeSentSoFar + "</td><td>" + 
                            item.AssessmentGrade + "</td></tr>";
                        counter+=1;
                }

                msg +="</table><br><br>Please review the CVs and provide us your feedback at the very earliest.<br><br>" +
                        "While we try to retain candidates as long as possible, due to the dynamic market conditions, " +
                        "candidates availability becomes volatile, and it is always preferable to keep candidates positively " +
                        "engaged.  While you may take a little longer to make up your minds for selections, the candidates " +
                        "that you are not interested in can be advised to us, so that they may be released for other opportunities.";
                msg += "While rejecting a candidate, if you also advise us reasons for the rejection, it will help us " +
                        "adjust our criteria for shortlistings, which will ultimately help in minimizing rejections at your end.";
                msg +="<br><br>Regards<br><br>This is system generated message";

                var email = new EmailMessage("cv forward", loggedIn.LoggedInAppUserId, off.OfficialUserId, 
                        loggedIn.LoggedInAppUserEmail, loggedIn.LoggedIAppUsername, off.OfficialName, off.OfficialEmail, "", "", 
                        counter + " CVs forwarded against your requirement", msg, (int)EnumMessageType.CVForwardingToClient, 3);
                emails.Add(email);
            }

            return (ICollection<EmailMessage>)emails;
        
        }

    }
}