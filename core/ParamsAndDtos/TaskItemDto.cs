using System;
using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;
using core.ParamsAndDtos;

namespace core.ParamsAndDtos
{
    public class TaskItemDto
    {
          public TaskItemDto()
          {
          }

        public TaskItemDto(int taskTypeId, DateTime transactionDate, string taskStatus, 
                string taskItemDescription, int employeeId, int orderId, int orderItemId, int orderNo, 
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
          }

          public TaskItemDto(int taskTypeId, int taskId, DateTime transactionDate, string taskStatus, 
                string taskItemDescription, int employeeId, int orderId, int orderItemId, int orderNo, 
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
          }

        public int Id {get; set;}
        public int ApplicationTaskId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TaskTypeId {get; set;}
        public string TaskTypeName {get; set;}
        public string TaskStatus { get; set; }
        public string TaskItemDescription {get; set;}
        public string EmployeeName {get; set;}
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderNo { get; set; }
        public int CandidateId { get; set; }
        public int UserId {get; set;}
        public string UserName {get; set;}
        public DateTime? NextFollowupOn {get; set;}
        public int? NextFollowupById {get; set;}

    }
}