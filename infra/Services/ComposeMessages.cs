using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;
using core.Entities.Orders;
using core.Interfaces;
using core.Params;
using core.ParamsAndDtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class ComposeMessages : IComposeMessages
     {
          private readonly ATSContext _context;
          private readonly IStatsService _statsService;
          private readonly IEmployeeService _empService;
          

          public ComposeMessages(ATSContext context, IStatsService statsService, ICommonServices commonServices,
                    IConfiguration confg, IEmployeeService empService )
          {
               _empService = empService;
               _statsService = statsService;
               _context = context;
          }

          public string ComposeOrderItems(int orderNo, ICollection<OrderItem> orderItems, bool hasException)
          {
               var ex="";
               string items = "<table border='1'><tr><th>Our reference</th><th>Category</th><th>Quantity</th><th>ECNR</th><th>Source<br>Country</th><th>" +
                    "Work<br>hours</th><th>salary</th><th>Accommodation</th><th>Food</th>Transport</th><th>" +
                    "Other<br>Allowances</th><th>Exceptions</th></tr>";
               foreach (var item in orderItems)
               {
                    items += "<tr><td>" + orderNo + "-" + item.SrNo + "</td><td>" + item.CategoryName + "</td><td>" + item.Quantity;
                    items += "</td><td>" + item.Ecnr + "</td><td>" + item.SourceFrom;
                    items += "</td><td>" + item.Remuneration.WorkHours;
                    items += "</td><td>" + item.Remuneration.SalaryCurrency + item.Remuneration.SalaryMin;
                    items += item.Remuneration.SalaryMax > 0 ? " - " + item.Remuneration.SalaryMax : "";

                    items += item.Remuneration.HousingNotProvided == true ? "Not Provided"
                         : item.Remuneration.HousingProvidedFree == true ? "Free"
                         : item.Remuneration.HousingAllowance == 0 ? "??" : item.Remuneration.HousingAllowance;
                    items += item.Remuneration.FoodNotProvided == true ? "Not Provided"
                         : item.Remuneration.FoodProvidedFree == true ? "Free"
                         : item.Remuneration.FoodAllowance == 0 ? "??" : item.Remuneration.FoodAllowance;
                    items += item.Remuneration.TransportNotProvided == true ? "Not Provided"
                         : item.Remuneration.TransportProvidedFree == true ? "Free"
                         : item.Remuneration.TransportAllowance == 0 ? "??" : item.Remuneration.TransportAllowance;

                    items += "</td><td>" + item.Remuneration.OtherAllowance + "</td><td>";

                    ex += item.Remuneration.WorkHours == 0 ? "<font color='red'><b>Working hours not provided</b><br></font>" : "";
                    ex += item.Remuneration.SalaryMin == 0 ? "<font color='red'><b>Salary not provided</b><br></font>" : "";
                    ex += !item.Remuneration.HousingProvidedFree && item.Remuneration.HousingAllowance == 0
                              && !item.Remuneration.HousingNotProvided ? "<font color='red'><b>Housing provision not provided</b></font><br>" : "";
                    ex += !item.Remuneration.FoodProvidedFree && item.Remuneration.FoodAllowance == 0
                              && !item.Remuneration.HousingNotProvided ? "<font color='red'><b>Food provision not provided</font></b><br>" : "";
                    ex += !item.Remuneration.TransportProvidedFree && item.Remuneration.TransportAllowance == 0
                              && !item.Remuneration.TransportNotProvided ? "<font color='red'><b>Transport provision not provided</b></font><br>" : "";
                    items += ex + "</td></tr>";
                    hasException = ex.Length > 0;
               }
               items += "</table>";
               return items;
          }

          public string GetSelectionDetails(string CandidateName, int ApplicationNo, string CustomerName, string CategoryName, Employment employmt)
          {
               string strToReturn = "";
               strToReturn = "<ul><li><b>Employee Name:</b> " + CandidateName + "(Application No.:" + ApplicationNo + ")</li>" +
                    "<li><b>Employer</b>: " + CustomerName + "</li>" +
                    "<li><b>Selected as:</b> " + CategoryName + 
                    "<li><b>Contract Period:</b>" + employmt.ContractPeriodInMonths + " months</li>" +
                    "<li><b>Basic Salary:</b>" + employmt.SalaryCurrency + " " + employmt.Salary + "</li>" +
                    "<li><b>Housing Provision: </b>";
                    if (employmt.HousingProvidedFree) { strToReturn += "Provided Free"; }
                    else { strToReturn += employmt.HousingAllowance > 0 
                         ? employmt.SalaryCurrency + " " + employmt.HousingAllowance : "Not provided"; }
               strToReturn += "</li>" +
                    "<li><b>Food Provision:</b>";
                    if (employmt.FoodProvidedFree) { strToReturn += "Provided Free"; }
                    else {strToReturn += employmt.FoodAllowance > 0 ? 
                         employmt.SalaryCurrency + " " + employmt.FoodAllowance : "Not Provided"; }
               strToReturn += "</li>" +
                    "<b><li>Transport Provision:</b> ";
                    if (employmt.TransportProvidedFree) { strToReturn += "Provided Free"; }
                    else { strToReturn += employmt.TransportAllowance > 0 
                         ? employmt.SalaryCurrency + " " + employmt.TransportAllowance : "Not provided"; }
               strToReturn += "</li>";
               if (employmt.OtherAllowance > 0) strToReturn += "<li><b>Other Allowances:</b>" + employmt.SalaryCurrency + " " + employmt.OtherAllowance + "</li>";
               return strToReturn + "</ul>";
          }
               
          public async Task<string> TableOfRelevantOpenings(List<int> Ids)
          {
               var stringIds = string.Join(",", Ids);
               var statsParams = new StatsParams { ProfessionIds = stringIds };

               string strToReturn = "";
               var openings = await _statsService.GetCurrentOpenings(statsParams);
               if (openings != null)
               {
                    strToReturn = "<Table border='1'><tr><th>Order No</th><th>Category Name</th><th>Customer</th></tr>";
                    foreach (var item in openings)
                    {
                         strToReturn += "<tr><td>" + item.OrderNo + "</td><td>" + item.ProfessionName + "</td><td>" + item.CustomerName + "</td></tr>";
                    }
               }
               return string.IsNullOrEmpty(strToReturn) 
                    ? "Currently, no opportunities available matching your professions" + "</table>"
                    : strToReturn + "</table";
          }
         
          public async Task<string> TableOfCVsSubmittedByHRExecutives(ICollection<CVsSubmittedDto> cvsSubmitted)
          {
               string strToReturn = "<Table border='1'><tr><th>Category Ref</th><th>Category</th><th>" +
                    "Application No</th><th>Candidate Name</th><th>PP No. </th><th>HR Executive" +
                    "</th><th>Submitted On</th></tr>";

               foreach(var cv in cvsSubmitted)
               {
                    var item = await _context.OrderItems.Where(x => x.Id == cv.OrderItemId).Select(
                              x => new {x.OrderId, x.SrNo, x.CategoryName, x.Quantity, x.RequireAssess}).FirstOrDefaultAsync();
                    var order = await _context.Orders.Where(x => x.Id == item.OrderId).Select(x => x.OrderNo).FirstOrDefaultAsync();

                    var candidate = await _context.Candidates.Where(x => x.Id == cv.CandidateId)
                         .Select(x => new{x.FullName, x.ApplicationNo, x.PpNo}).FirstOrDefaultAsync();
                    var owner = await _empService.GetEmployeeBriefAsyncFromEmployeeId(cv.TaskOwnerId);
                    var categoryName = item.CategoryName;
                    var categorySrNo = item.SrNo;
                    var quantity = item.Quantity;
                    var orderNo = order;
                    strToReturn += "<tr><td>" + order + "-" + item.SrNo + "</td><td>" + item.CategoryName + "</td><td>" +
                         candidate.ApplicationNo + "</td><td>" + candidate.FullName + "</td><td>" +
                         candidate.PpNo + "</td><td>" + owner.EmployeeName + "</td></tr>";
               }
               return strToReturn + "</table>";
               
          }

          public async Task<string> TableOfCVsSubmittedByHRSup(ICollection<CVsSubmittedDto> cvsSubmitted)
          {
               string strToReturn = "<table border='1'><tr><th>Category Ref</th><th>Category</th><th>" +
                    "Application No</th><th>Candidate Name</th><th>PP No. </th><th>HR Executive" +
                    "</th><th>Submitted On</th></tr>";

               foreach(var cv in cvsSubmitted)
               {
                    var item = await _context.OrderItems.Where(x => x.Id == cv.OrderItemId).Select(
                              x => new {x.OrderId, x.SrNo, x.CategoryName, x.Quantity, x.RequireAssess}).FirstOrDefaultAsync();
                    var order = await _context.Orders.Where(x => x.Id == item.OrderId).Select(x => x.OrderNo).FirstOrDefaultAsync();

                    var candidate = await _context.Candidates.Where(x => x.Id == cv.CandidateId)
                         .Select(x => new{x.FullName, x.ApplicationNo, x.PpNo}).FirstOrDefaultAsync();
                    var owner = await _empService.GetEmployeeBriefAsyncFromEmployeeId(cv.TaskOwnerId);
                    var categoryName = item.CategoryName;
                    var categorySrNo = item.SrNo;
                    var quantity = item.Quantity;
                    var orderNo = order;
                    strToReturn += "<tr><td>" + order + "-" + item.SrNo + "</td><td>" + item.CategoryName + "</td><td>" +
                         candidate.ApplicationNo + "</td><td>" + candidate.FullName + "</td><td>" +
                         candidate.PpNo + "</td><td>" + owner.EmployeeName + "</td></tr>";
               }
               return strToReturn + "</table>";
               
          }

          public async Task<OrderItemReviewStatusDto> CumulativeCountForwardedSoFar(int orderitemId)
          {
               return await( _context.CVRefs.Where(x => x.Id == orderitemId)
                    .GroupBy(x => new { x.OrderItemId })
                    .Select(x => new OrderItemReviewStatusDto
                    {
                         OrderItemId = orderitemId,
                         CtOfSelected = x.Count(x => x.RefStatus == (int) EnumCVRefStatus.Selected),
                         CtOfNotReviewed = x.Count(x => x.RefStatus == (int) EnumCVRefStatus.Referred),
                         CtOfRejected = x.Count(x => x.RefStatus != (int)EnumCVRefStatus.Selected &&
                              x.RefStatus != (int) EnumCVRefStatus.Referred)
                    })
               ).FirstOrDefaultAsync();
          }

          public async Task<EnumCandidateAssessmentResult> AssessmentGrade(int candidateId, int orderitemId)
          {
               return await _context.CandidateAssessments
                    .Where(x => x.CandidateId == candidateId && x.OrderItemId == orderitemId)
                    .Select(x => x.AssessResult)
                    .FirstOrDefaultAsync();
          }    
          
          public string GetSelectionDetailsBySMS(SelectionDecision selection)
          {
               string strToReturn = "";
               strToReturn = "Pleased to advise you hv been selected by " + selection.CustomerName + " as " + selection.CategoryName;
               strToReturn += " at a basic salary of " + selection.Employment.SalaryCurrency + " " + selection.Employment.FoodAllowance;
               strToReturn += " plus perks.  Please visit us to review and sign your offer letter and to initiate your joining formalities";
               return strToReturn;
          }

          public async Task<string> TableOfOrderItemsContractReviewedAndApproved(ICollection<int> itemIds)
          {
               string strToReturn = "";
               var orderitems = await _context.OrderItems.Where(x => itemIds.Contains(x.Id) && x.Remuneration != null)
                    .Include(x => x.Remuneration)
                    .OrderBy(x => x.SrNo)
                    .ToListAsync();
               if (orderitems == null || orderitems.Count() == 0) return "";

               strToReturn = "<Table border='1'><tr><th>Sr No</th><th>Category Name</th><th>Quantity</th><th>Source<br>country" +
                    "</th><th>ECNR</th><th>Assessmt<br>Sheet</th><th>Work<br>Hours</th><th>Complete By" +
                    "</th><th>Salary<br>Currency</th><th>Salary</th><th>Food</th><th>Housing</th><th>Transport" +
                    "</th><th>Other<br>Allowance</th><th>Contract Period</th></tr>";

               foreach (var item in orderitems)
               {
                    var catname = string.IsNullOrEmpty(item.CategoryName) ? "undefined" : item.CategoryName;
                    var sourcefrom = string.IsNullOrEmpty(item.SourceFrom) ? "India" : item.SourceFrom;
                    strToReturn += "<tr><td>" + item.SrNo + 
                         "</td><td>" + catname + 
                         "</td><td>" + item.Quantity +
                         "</td><td>" + sourcefrom + "</td><td>";
                    strToReturn += (item.Ecnr ? "Y" : "N") + "</td><td>";
                    strToReturn += item.RequireAssess ? "Y" : "N";

                    if (item.Remuneration != null)
                    {
                         strToReturn += "</td><td>" + item.Remuneration.WorkHours;
                         if (item.CompleteBefore.Year < 2000)
                         {
                              strToReturn += "</td><td>Not Known";
                         }
                         else
                         {
                              strToReturn += "</td><td>" + item.CompleteBefore.ToString("dd-MMMM-yyyy", CultureInfo.InvariantCulture);
                         }
                         strToReturn += "</td><td>" + item.Remuneration.SalaryCurrency + "</td><td>" + item.Remuneration.SalaryMin;
                         if (item.Remuneration.SalaryMax > 0) strToReturn += "-" + item.Remuneration.SalaryMax;

                         if (item.Remuneration.FoodNotProvided)
                         {
                              strToReturn += "</td><td>Not Provided";
                         }
                         else if (item.Remuneration.FoodAllowance > 0)
                         {
                              strToReturn += "</td><td>" + item.Remuneration.FoodAllowance;
                         }
                         else
                         {
                              strToReturn += "</td><td>Provided Free";
                         }

                         if (item.Remuneration.HousingNotProvided)
                         {
                              strToReturn += "</td><td>Not Provided";
                         }
                         else if (item.Remuneration.HousingAllowance > 0)
                         {
                              strToReturn += "</td><td>" + item.Remuneration.HousingAllowance;
                         }
                         else
                         {
                              strToReturn += "</td><td>Provided Free";
                         }

                         if (item.Remuneration.TransportNotProvided)
                         {
                              strToReturn += "</td><td>Not Provided";
                         }
                         else if (item.Remuneration.TransportAllowance > 0)
                         {
                              strToReturn += "</td><td>" + item.Remuneration.TransportAllowance;
                         }
                         else
                         {
                              strToReturn += "</td><td>Provided Free";
                         }
                         
                         if (item.Remuneration.OtherAllowance > 0)
                         { strToReturn += "</td><td>" + item.Remuneration.OtherAllowance; }
                         else
                         { strToReturn += "</td><td>Not Provided"; }

                         strToReturn += "</td><td>" + item.Remuneration.ContractPeriodInMonths + " months</td></tr>";
                    }
               }
               strToReturn += "</table>";
               return strToReturn;
          }

     }
}