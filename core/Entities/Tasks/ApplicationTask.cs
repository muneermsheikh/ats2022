using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.ParamsAndDtos;

namespace core.Entities.Tasks
{
    public class ApplicationTask: BaseEntity
    {
          public ApplicationTask()
          {
          }

          public ApplicationTask(int taskTypeId, DateTime taskDate, int taskOwnerId, int assignedToId, 
                int? orderId, int? orderno, int? orderItemId, string taskDescription, DateTime completeBy, 
                string taskStatus, int? candidateId, ICollection<TaskItem> taskItems)
          {
               TaskTypeId = taskTypeId;
               TaskDate = taskDate;
               TaskOwnerId = taskOwnerId;
               AssignedToId = assignedToId;
               OrderId = orderId;
               OrderNo = orderno;
               OrderItemId = orderItemId;
               CandidateId = candidateId;
               TaskDescription = taskDescription;
               CompleteBy = completeBy;
               TaskStatus = taskStatus;
               TaskItems = taskItems;
          }
          public ApplicationTask(int taskTypeId, DateTime taskDate, int taskOwnerId, int assignedToId, 
                int? orderId, int? orderno, int? orderItemId, string taskDescription, DateTime completeBy, 
                string taskStatus, int? candidateId, int cvreviewid)
          {
               TaskTypeId = taskTypeId;
               TaskDate = taskDate;
               TaskOwnerId = taskOwnerId;
               AssignedToId = assignedToId;
               OrderId = orderId;
               OrderNo = orderno;
               OrderItemId = orderItemId;
               CandidateId = candidateId;
               TaskDescription = taskDescription;
               CompleteBy = completeBy;
               TaskStatus = taskStatus;
               CVReviewId = cvreviewid;
          }

        [Required]
        public int TaskTypeId { get; set; }
        public int CVReviewId { get; set; }
        public DateTime TaskDate { get; set; } = DateTime.Now;
        [Required]
        public int TaskOwnerId {get; set;}
        public string TaskOwnerName {get; set;}
        [Required]
        public int AssignedToId {get; set;}
        public string AssignedToName {get; set;}
        public int? OrderId {get; set;}
        public int? OrderNo { get; set; }
        public int? OrderItemId {get; set;}
        public int? ApplicationNo { get; set; }
        public int? CandidateId { get; set; }
        public string PersonType {get; set;}
        [Required]
        public string TaskDescription {get; set;}
        [Required]
        public DateTime CompleteBy {get; set;}
        [Required]
        public string TaskStatus {get; set;}="Open";
        public DateTime CompletedOn { get; set; }
        public int HistoryItemId { get; set; }
        public ICollection<TaskItem> TaskItems {get; set;}
        public EnumPostTaskAction PostTaskAction { get; set; } = EnumPostTaskAction.DoNotComposeAnyMessage;

    }

    public enum EnumPostTaskAction
    {
        DoNotComposeAnyMessage = 0,
        OnlyComposeEmailMessage = 1,
        OnlyComposeEmailAndSMSMessages=2,
        ComposeAndSendEmail=3,
        ComposeAndSendEmailComposeAndSendSMS=4
    }
}