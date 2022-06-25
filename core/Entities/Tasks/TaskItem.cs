using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.Tasks
{
     public class TaskItem: BaseEntity
    {
          public TaskItem()
          {
          }

        public TaskItem(int taskTypeId, DateTime transactionDate, string taskStatus, 
                string taskItemDescription, int orderId, int orderItemId, int orderNo, 
                int userId, DateTime? nextFollowupOn, int NextFollowupById, 
                int quantity)
          {
               TaskTypeId = taskTypeId;
               TransactionDate = transactionDate;
               TaskStatus = taskStatus;
               TaskItemDescription = taskItemDescription;
               OrderId = orderId;
               OrderItemId = orderItemId;
               OrderNo = orderNo;
               UserId = userId;
               NextFollowupOn = nextFollowupOn;
               Quantity = quantity;
          }

          public TaskItem(int taskTypeId, int taskId, DateTime transactionDate, string taskStatus, 
                string taskItemDescription, int orderId, int orderItemId, int orderNo, 
                int userId, DateTime? nextFollowupOn, int candidateid, int NextFollowupById, 
                int quantity
                )
          {
               TaskTypeId = taskTypeId;
               ApplicationTaskId = taskId;
               TransactionDate = transactionDate;
               TaskStatus = taskStatus;
               TaskItemDescription = taskItemDescription;
               OrderId = orderId;
               OrderItemId = orderItemId;
               OrderNo = orderNo;
               UserId = userId;
               CandidateId = candidateid;
               NextFollowupOn = nextFollowupOn;
               Quantity = quantity;
          }

        [Required]
        public int ApplicationTaskId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        //public string TaskType { get; set; }
        
        [Required]
        public int TaskTypeId {get; set;}
        public string TaskTypeName {get; set;}
        [Required]
        public string TaskStatus { get; set; }
        [Required]
        public string TaskItemDescription {get; set;}
        [Required]
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderNo { get; set; }
        public int CandidateId { get; set; }
        [Required]
        public int UserId {get; set;}
        public string UserName {get; set;}
        public int Quantity { get; set; }=1;
        public DateTime? NextFollowupOn {get; set;}
        public int? NextFollowupById {get; set;}
        //public ApplicationTask ApplicationTask {get; set;}
    }
}