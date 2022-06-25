using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
    public class ComposeMessagesForInternalReviewHR: IComposeMessagesForInternalReviewHR
    {
        private readonly IEmployeeService _empService;
        private readonly ICommonServices _commonServices;
        private readonly ATSContext _context;
        private readonly IComposeMessages _composeMessages;
        public ComposeMessagesForInternalReviewHR(IEmployeeService empService, 
        ICommonServices commonServices, ATSContext context, IComposeMessages composeMessages)
        {
            _empService = empService;
            _context=context;
            _commonServices = commonServices;
            _composeMessages = composeMessages;
        }

        public async Task<ICollection<EmailMessage>> ComposeMessagesToHRExecToSourceCVs(ICollection<OrderAssignmentDto> dtos)
            //, LoggedInUserDto loggedIn )
        {
            //todo - verify the tasks are not already assigned.  If so, advise to conclude existing tasks before assigning to new HR Executives
            /* var hrTasks = await _context.OrderItems.Where(x => orderItemIds.Contains(x.Id))
                .Select(x => new {x.HrExecId, x.Id, x.OrderId, x.CategoryId, x.CategoryName, x.Quantity, x.JobDescription})
                .ToListAsync();

            var order = await _context.Orders.Where(x => x.Id == orderItemIds.Select(x => x.OrderId).FirstOrDefault() )
                .Select(x => new {x.OrderNo, x.OrderDate, x.ProjectManagerId, x.CustomerId, x.CityOfWorking})
                .FirstOrDefaultAsync();               
            */
            var hrExecIds = dtos.Select(x => x.HrExecId).Distinct().ToList();
            var order = dtos.Select(x => new {x.OrderNo, x.CustomerId, x.OrderDate, x.ProjectManagerId, x.CityOfWorking}).FirstOrDefault();
            var msgs = new List<EmailMessage>();

            int projMgrId = order.ProjectManagerId == 0 ? 1 : order.ProjectManagerId;       //todo - correct this
            var projMgr = await _empService.GetEmployeeFromIdAsync(projMgrId);
            var cust = await _commonServices.CustomerBriefDetailsFromCustomerId(order.CustomerId);
            var postAction = dtos.Select(x => x.PostTaskAction).FirstOrDefault();

            foreach(var hrExecId in hrExecIds)
            {
                var hrExec = await _empService.GetEmployeeFromIdAsync((int)hrExecId);

                string msg = DateTime.Now.ToString("dd-MMMM-yyyy", CultureInfo.InvariantCulture)  + "<br><br>" + hrExec.EmployeeName;
                if (!string.IsNullOrEmpty(hrExec.Position)) msg += ", " + hrExec.Position + "<br>";
                msg += "Email: " + hrExec.OfficialEmailAddress + "<br><br>";
                
                msg += "Following tasks are assigned to you for mobilizing candidates as per their job descriptions:<br><br>";
                msg += "<tab><b>Order No.:</b>" + order.OrderNo + " dated " + order.OrderDate.ToString("dd-MMMM-yy", CultureInfo.InvariantCulture) +"</tab>" +
                        "<br><tab><b>Customer:</b>" + cust.CustomerName + ", " + cust.City + "</tab><br>, Place of work: " + order.CityOfWorking;
                msg += "<br><tab>For Job Descriptions, click the relevant link</tab><br>";
                
                var ids = dtos.Where(x => x.HrExecId == hrExecId).Select(x => x.OrderItemId).ToList();
                var orderitems = await _context.OrderItems.Where(x => ids.Contains(x.Id) && x.Remuneration != null)
                        .Include(x => x.Remuneration).ToListAsync();
                
                var tbl = await _composeMessages.TableOfOrderItemsContractReviewedAndApproved(
                        dtos.Where(x => x.HrExecId == hrExecId).Select(x => x.OrderItemId).ToList());
                msg += tbl + "<br><br>Please also check for your task dashboard for these tasks";
                msg += "<br>end of system generated message";

                var emailMsg = new EmailMessage("AssignTasksToHRExec", projMgr.AppUserId, hrExec.AppUserId, 
                        projMgr.OfficialEmailAddress, projMgr.UserName, hrExec.UserName, hrExec.OfficialEmailAddress, "", "", 
                        "Tasks to mobilize suitable candidates - Order No. " + order.OrderNo,
                        msg, (int)EnumMessageType.TaskAssignmentToHRExecToShortlistCV, postAction);
                msgs.Add(emailMsg);
            }
            return msgs;
        }
    
        public async Task<ICollection<EmailMessage>> ComposeMessagesToHRSupToReviewCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn)
        {
            //todo - verify the tasks are not already assigned.  If so, advise to conclude existing tasks before assigning to new HR Executives
            var msgs = new List<EmailMessage>();

            var orderitems = await _context.OrderItems.Where(x => cvsSubmitted.Select(x => x.OrderItemId).ToList().Contains(x.Id))
                .Select(x => new {x.HrExecId, x.Id, x.OrderId, x.HrSupId, x.CategoryId, x.CategoryName, x.Quantity, x.JobDescription})
                .ToListAsync();

            var order = await _context.Orders.Where(x => x.Id == orderitems.Select(x => x.OrderId).FirstOrDefault())
                .Select(x => new {x.Id, x.OrderNo, x.OrderDate, x.ProjectManagerId, x.CustomerId, x.CityOfWorking})
                .FirstOrDefaultAsync();

            var assignedToIds = orderitems.Select(x => x.HrSupId).Distinct().ToList();
            var ownerids = orderitems.Select(x => x.HrExecId).Distinct().ToList();
            var cust = await _commonServices.CustomerBriefDetailsFromCustomerId(order.CustomerId);

            foreach(var ownerid in ownerids)
            {
                var owner = await _empService.GetEmployeeBriefAsyncFromEmployeeId((int)ownerid);
                //select HRExecIds for all records where HRSupId is assignedToId
                var assigneeids = orderitems.Where(x => x.HrExecId == ownerid).Select(x => x.HrSupId).Distinct().ToList();

                foreach(var i in assigneeids)
                {
                        var ownerAndAssignees = cvsSubmitted.Where(x => x.AssignedToId == i && x.TaskOwnerId == ownerid).ToList();
                        var tbl = _composeMessages.TableOfCVsSubmittedByHRExecutives(ownerAndAssignees);

                        var assignee = await _empService.GetEmployeeFromIdAsync((int)i); //task owner

                        var msg = DateTime.Now.Date + "<br><br>" + assignee.EmployeeName + ", " + assignee.Position + "<br>" +
                            "<br>Email: " + assignee.OfficialEmailAddress + "<br><br><b>Task: To review cVs submitted by HR Executives.</b>";
                        msg += "<br><br>Please review following CVs submitted by HR Executives for your approval.<br>";
                        msg += "Order No.:" + order.OrderNo + " dated " + order.OrderDate +
                            "<br>Customer:" + cust.CustomerName + ", " + cust.City + ", Place of work: " + order.CityOfWorking;
                        msg += tbl;
                        msg += "<br><br>Please also check for your task dashboard for these tasks";

                        var emailMsg = new EmailMessage("AssignTasksToHRExec", owner.AppUserId, owner.AppUserId, 
                            owner.OfficialEmailAddress, owner.UserName, assignee.UserName, assignee.OfficialEmailAddress, "", "", 
                            "Tasks to mobilize suitable candidates - Order No. " + order.OrderNo,
                            msg, (int)EnumMessageType.TaskAssignmentToHRExecToShortlistCV, 3);
                        msgs.Add(emailMsg);
                }
            }

            return msgs;
        }

        public async Task<ICollection<EmailMessage>> ComposeMessagesToHRMToReviewCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn)
        {
            //todo - verify the tasks are not already assigned.  If so, advise to conclude existing tasks before assigning to new HR Executives
            var msgs = new List<EmailMessage>();

            var orderitems = await _context.OrderItems.Where(x => cvsSubmitted.Select(x => x.OrderItemId).ToList().Contains(x.Id))
                .Select(x => new {x.Id, x.OrderId, x.HrSupId, x.HrmId, x.CategoryId, x.CategoryName, x.Quantity, x.JobDescription})
                .ToListAsync();

            var order = await _context.Orders.Where(x => x.Id == orderitems.Select(x => x.OrderId).FirstOrDefault())
                .Select(x => new {x.Id, x.OrderNo, x.OrderDate, x.ProjectManagerId, x.CustomerId, x.CityOfWorking})
                .FirstOrDefaultAsync();

            var assignedToIds = orderitems.Select(x => x.HrmId).Distinct().ToList();
            var ownerids = orderitems.Select(x => x.HrSupId).Distinct().ToList();
            var cust = await _commonServices.CustomerBriefDetailsFromCustomerId(order.CustomerId);

            foreach(var ownerid in ownerids)
            {
                var owner = await _empService.GetEmployeeBriefAsyncFromEmployeeId((int)ownerid);
                //select HRExecIds for all records where HRSupId is assignedToId
                var assigneeids = orderitems.Where(x => x.HrSupId == ownerid).Select(x => x.HrmId).Distinct().ToList();

                foreach(var i in assigneeids)
                {
                        var ownerAndAssignees = cvsSubmitted.Where(x => x.AssignedToId == i && x.TaskOwnerId == ownerid).ToList();
                        var tbl = _composeMessages.TableOfCVsSubmittedByHRSup(ownerAndAssignees);

                        var assignee = await _empService.GetEmployeeFromIdAsync((int)i); //task owner

                        var msg = DateTime.Now.Date + "<br><br>" + assignee.EmployeeName + ", " + assignee.Position + "<br>" +
                            "<br>Email: " + assignee.OfficialEmailAddress + "<br><br><b>Task: To review cVs submitted by HR Executives.</b>";
                        msg += "<br><br>Please review following CVs submitted by HR Executives for your approval.<br>";
                        msg += "Order No.:" + order.OrderNo + " dated " + order.OrderDate +
                            "<br>Customer:" + cust.CustomerName + ", " + cust.City + ", Place of work: " + order.CityOfWorking;
                        msg += tbl;
                        msg += "<br><br>Please also check for your task dashboard for these tasks";

                        var emailMsg = new EmailMessage("AssignTasksToHRExec", owner.AppUserId, owner.AppUserId, 
                            owner.OfficialEmailAddress, owner.UserName, assignee.UserName, assignee.OfficialEmailAddress, "", "", 
                            "Tasks to mobilize suitable candidates - Order No. " + order.OrderNo,
                            msg, (int)EnumMessageType.TaskAssignmentToHRExecToShortlistCV,3);
                        msgs.Add(emailMsg);
                }
            }

            return msgs;
        }

        public async Task<EmailMessage> ComposeHTMLToPublish_CVSubmittedToHRSup(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId)
        {
            var emp = await _empService.GetEmployeeBriefAsyncFromAppUserId(loggedInDto.LoggedInAppUserId);
            var recipient = await _empService.GetEmployeeBriefAsyncFromEmployeeId(recipientId);

            var msg = DateTime.Now.Date + "<br><br>"+ emp.EmployeeName + "<br>" + emp.Position + "<br>email:"+ emp.OfficialEmailAddress +
                "Following Applications are submitted to the HR Supervisor for his review:<br><br>" +
                "<table border='1'><tr><th>App No</th><th>Candidate Name</th><th>Category Ref" + 
                "</th><th>Customer</th><th>Submitted by</th></tr>";
            foreach(var item in commonDataDtos)
            {
                msg += "<tr><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + "</td><td>"+item.CategoryName + 
                        "</td><td>" + item.CustomerName + "</td><td>" + emp.EmployeeName + "</td></tr>";
            }

            msg +="</table>Regards<br><br>This is system generated message";

            var email = new EmailMessage("advisory", loggedInDto.LoggedInAppUserId, recipient.AppUserId, loggedInDto.LoggedInAppUserEmail, 
                loggedInDto.LoggedIAppUsername, recipient.KnownAs, recipient.Email, "", "", 
                "Applications submitted to HR Supervisor for review, initiated by " + emp.EmployeeId, msg, 
                (int)EnumMessageType.Publish_CVReviewedByHRSup, 3);
            
            return email;

        }
        public async Task<EmailMessage> ComposeHTMLToPublish_CVReviewedByHRSup(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId)
        {
            var emp = await _empService.GetEmployeeBriefAsyncFromAppUserId(loggedInDto.LoggedInAppUserId);
            var recipient = await _empService.GetEmployeeBriefAsyncFromEmployeeId(recipientId);

            var msg = DateTime.Now.Date + "<br><br>"+ emp.EmployeeName + "<br>" + emp.Position + "<br>email:"+ emp.Email +
                "Following CVs are reviewed by the HR Supervisor:<br><br>" +
                "<table border='1'><tr><th>App No</th><th>Candidate Name</th><th>Category Ref" + 
                "</th><th>Customer</th><th>Submitted by</th><th>Review Result</th></tr>";
            foreach(var item in commonDataDtos)
            {
                msg += "<tr><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + "</td><td>"+item.CategoryName + 
                        "</td><td>" + item.CustomerName + "</td><td>" + emp.EmployeeName + "</td><td>" + item.ReviewResultId + "</tr>";
            }

            msg +="</table><br><br>Regards<br><br>This is system generated message";

            var email = new EmailMessage("advisory", loggedInDto.LoggedInAppUserId, recipient.AppUserId, loggedInDto.LoggedInAppUserEmail, 
                loggedInDto.LoggedIAppUsername, recipient.KnownAs, recipient.Email, "", "", 
                "Applications submitted to HR Supervisor for review, initiated by " + emp.EmployeeId, msg, 
                (int)EnumMessageType.Publish_CVReviewedByHRSup, 3);
            
            return email;
        }

        public async Task<EmailMessage> ComposeHTMLToPublish_CVReviewedByHRManager(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId)
        {
            var emp = await _empService.GetEmployeeBriefAsyncFromAppUserId(loggedInDto.LoggedInAppUserId);
            var recipient = await _empService.GetEmployeeBriefAsyncFromEmployeeId(recipientId);
            if (recipient == null) return null;

            var msg = DateTime.Now.Date + "<br><br>"+ emp.EmployeeName + "<br>" + emp.Position + "<br>email:"+ emp.Email +
                "Following Applications are reviewed by HR Manager and approved for forwarding to respective customers:<br><br>" +
                "<table border='1'><tr><th>App No</th><th>Candidate Name</th><th>Category Ref</th><th>" + 
                "Customer</th><th>Submitted by</th><th>Review Result</th></tr>";
            foreach(var item in commonDataDtos)
            {
                msg += "<tr><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + "</td><td>"+item.CategoryName + 
                        "</td><td>" + item.CustomerName + "</td><td>" + emp.EmployeeName + "</td><td>" + item.ReviewResultId + "</td></tr>";
            }

            msg +="</table><br><br>Regards<br><br>This is system generated message";

            var email = new EmailMessage("advisory", loggedInDto.LoggedInAppUserId, recipient.AppUserId, loggedInDto.LoggedInAppUserEmail, 
                loggedInDto.LoggedIAppUsername, recipient.KnownAs, recipient.Email, "", "", 
                "Applications readied for forwarding to Customers" + emp.EmployeeId, msg, 
                (int)EnumMessageType.Publish_CVReviewedByHRManager, 3);
            
            return email;
        }
    

    }
}