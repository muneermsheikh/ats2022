using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Orders;

namespace core.Entities.HR
{
    public class SelectionDecision: BaseEntity
    {
          public SelectionDecision()
          {
          }

        public SelectionDecision(int cVRefId, int orderItemId, int categoryId, string categoryName, int orderId, 
              int orderNo, string customerName, int applicationNo, int candidateId, string candidateName, DateTime decisionDate, 
              int selectionStatusId, string remarks)
           {
               CVRefId = cVRefId;
               OrderItemId = orderItemId;
               CategoryId = categoryId;
               CategoryName = categoryName;
               OrderId = orderId;
               OrderNo = orderNo;
               CustomerName = customerName;
               ApplicationNo = applicationNo;
               CandidateId = candidateId;
               CandidateName = candidateName;
               DecisionDate = decisionDate;
               SelectionStatusId = selectionStatusId;
               Remarks = remarks;
          }

          public int CVRefId { get; set; }
          public int OrderItemId { get; set; }
          public int CategoryId { get; set; }
          public string CategoryName {get; set;}
          public int OrderId { get; set; }
          public int OrderNo {get; set;}
          public string CustomerName {get; set;}
          public int ApplicationNo {get; set;}
          public int CandidateId {get; set;}
          public string CandidateName {get; set;}
          public DateTime DecisionDate {get; set;}
          public int SelectionStatusId { get; set; }
          [ForeignKey("CVRefId")]
          public virtual CVRef CVRef {get; set;}
          public DateTime SelectedOn { get; set; }
          public int Charges {get; set;}
          public string Remarks {get; set;}
          public Employment Employment {get; set;}
    }
}