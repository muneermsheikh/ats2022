using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
     public class ApplicationTaskDto
    {
          public ApplicationTaskDto()
          {
          }

          public ApplicationTaskDto(int taskTypeId, DateTime taskDate, int taskOwnerId, int assignedToId, 
                int? orderId, int? orderno, int? orderItemId, string taskDescription, DateTime completeBy, 
                string taskStatus, int? candidateId, ICollection<TaskItemDto> taskItemsDto)
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
               TaskItemsDto= taskItemsDto;
          }
          public ApplicationTaskDto(int taskTypeId, DateTime taskDate, int taskOwnerId, int assignedToId, 
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

        public int Id {get; set;}
        public int TaskTypeId { get; set; }
        public string TaskTypeName {get; set;}
        public int CVReviewId { get; set; }
        public DateTime TaskDate { get; set; }
        public int TaskOwnerId {get; set;}
        public string TaskOwnerName {get; set;}        
        public int AssignedToId {get; set;}
        public string AssignedToName {get; set;}
        public int? OrderId {get; set;}
        public int? OrderNo { get; set; }
        public int? OrderItemId {get; set;}
        public int? ApplicationNo { get; set; }
        public int? CandidateId { get; set; }
        public string PersonType {get; set;}
        public string TaskDescription {get; set;}
        public DateTime CompleteBy {get; set;}
        public string TaskStatus {get; set;}
        public DateTime CompletedOn { get; set; }
        public int HistoryItemId { get; set; }
        public ICollection<TaskItemDto> TaskItemsDto {get; set;}
        public int PostTaskAction { get; set; } 

    }

}