using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Interfaces;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class ComposeOrderAssessment : IComposeOrderAssessment
     {
        private readonly IEmployeeService _empService;
        private readonly ATSContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ComposeOrderAssessment(IEmployeeService empService, ATSContext context, 
            UserManager<AppUser> userManager)
        {
               _userManager = userManager;
               _context = context;
               _empService = empService;
        }

        public async Task<ICollection<EmailMessage>> ComposeMessagesToDesignOrderAssessmentQs(int orderId, int loggedinEmployeeId )
        {
            //Order.OrderItems can have different HR Supervisors who are tasked with designing assessment Q
            //So there will be one email message for each HR Supervisor
            //So the object to return will be collection of EmailMessage
            
            //compile data for use in the email message
            var loggedInUserObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(loggedinEmployeeId);

            //details of the ORder Items for use in the email message table
            var orderItemDetails = await _context.OrderItems.Where(x => x.OrderId == orderId && x.RequireAssess==true)
                .Select(x => new {
                        x.HrSupId, x.Id, x.OrderNo, x.SrNo, x.CategoryId, x.CategoryName, x.Quantity,
                        x.Ecnr, x.IndustryName, x.JobDescription})
                .OrderBy(x => x.HrSupId).ThenBy(x => x.SrNo).ToListAsync();

            //find distinct HR Supervisors, each value will have a email message
            var RecipientIds = orderItemDetails.Select(x => x.HrSupId).Distinct().ToList();
            var ord = await _context.Orders.Where(x => x.Id == orderId)
                .Select(x => new { x.OrderDate, x.OrderNo, x.CustomerId, x.CityOfWorking, x.CustomerName }).FirstOrDefaultAsync();

            var categoriesToDesignQ = new List<OrderAssessmentQObj>();       //the order categories for which assessment Q to design
            foreach (var recipientId in RecipientIds)
            {
                var assessmentDetails = new List<OrderAssessmentQDetails>();
                var dtls = orderItemDetails.Where(x => x.HrSupId == recipientId).ToList();

                foreach (var d in dtls)
                {
                        assessmentDetails.Add(new OrderAssessmentQDetails {
                            OrderItemId = d.Id,CategoryName = d.CategoryName, Quantity = d.Quantity, JDUrl = "todo"
                        });
                }
                var hruser = await _empService.GetEmployeeBriefAsyncFromEmployeeId((int)recipientId);
                if (hruser==null) throw new Exception("HR Supervisor Id value for the category not defined");

                var hrUserObj = await _userManager.FindByIdAsync(hruser.AppUserId.ToString());

                categoriesToDesignQ.Add(new OrderAssessmentQObj{
                        EmployeeId = (int)recipientId, AppUserId = hrUserObj.Id, AppUserEmail = hrUserObj.Email, AppUserName = hrUserObj.UserName,
                        EmployeeFullName = hruser.EmployeeName + ", " + hruser.Position, OrderId = orderId,
                        OrderDate = ord.OrderDate.Date, OrderNo = ord.OrderNo, CustomerName = ord.CustomerName,  
                        City = ord.CityOfWorking, AssessmentQDetails = assessmentDetails
                });
            };

            
            var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(loggedinEmployeeId);
            var emailmessages = new List<EmailMessage>();
            
            foreach (var item in categoriesToDesignQ)
            {
                var content = DateTime.Now.Date + "<br><br>" + item.EmployeeFullName + "<br><br>" +
                        "Following categories require Shortlisted Candidate Assessment to accompany their profiles.  " +
                        "Please design questions for the same for approval of the undersigned before entrusting the HR Executives " +
                        "to assess shortlisted candidates accordingly.<br>";
                content += "Customer: " + item.CustomerName + "<br>City of employment: " + item.City +
                        "<br>Order No.: " + item.OrderNo + " dated " + item.OrderDate + "<br>";
                
                content += "<table border='1'><tr><th>Sr No</th><th>Category Name</th><th> Qnty</th><th>Job Desc Url</th></tr>";
                foreach (var i in item.AssessmentQDetails)
                {
                        content += "<tr><td>" + i.SrNo + "</td><td>" + i.CategoryName + "</td><td>" + i.Quantity +
                            "</td><td>" + i.JDUrl + "</td></tr>";
                }
                content += "</table><br><br>For any clarification, refer the Job Description available at the above given url or consult the undersigned" +
                        "<br><br>Regards<br><br>" + senderObj.EmployeeName + "<br>" + senderObj.Position;
                var emailMsg = new EmailMessage("HR", loggedinEmployeeId, item.AppUserId, senderObj.Email,
                        senderObj.EmployeeName, item.AppUserName, item.AppUserEmail, "", "",
                        "Task to design assessment Questions",
                        content, (int)EnumMessageType.OrderAssessmentQDesigning, 3);
                emailmessages.Add(emailMsg);
            }

            return emailmessages;

        }


     }
}