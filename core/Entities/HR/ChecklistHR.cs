using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Orders;
using core.Entities.Users;

//HR Executives have to deal with candidates to identify their suitability for client requirements
//Certain identified aprameters have to be discussed with candidates, and noted for future references
//this model saves the data in the table
namespace core.Entities.HR
{
    public class ChecklistHR: BaseEntity
    {
         public ChecklistHR()
          {
          }

          public ChecklistHR(int candidateId, int orderItemId, int userId, string loggedinusername, DateTime checkedOn, ICollection<ChecklistHRItem> checklistHRItems)
          {
               CandidateId = candidateId;
               OrderItemId = orderItemId;
               UserId = userId;
               LoggedInUserName=loggedinusername;
               CheckedOn = checkedOn;
               ChecklistHRItems = checklistHRItems;
          }
        [ForeignKey("CandidateId")]
        public int CandidateId { get; set; }
        [ForeignKey("OrderItemId")]
        public int OrderItemId { get; set; }
        public int UserId { get; set; }
        public string LoggedInUserName {get; set;}
        public DateTime CheckedOn {get; set;}
        public String HrExecComments {get; set;}
        public int Charges {get; set;}
        public int ChargesAgreed {get; set;}
        public bool? ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime? ExceptionApprovedOn {get; set;}
        public bool ChecklistedOk {get; set;}
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
        //public ICollection<Candidate> Candidates {get; set;}
        //public ICollection<OrderItem> OrderItems {get; set;}

    }
}