using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class InterviewItemDto
    {
        public InterviewItemDto()
        {
        }

          public InterviewItemDto(int interviewId, int orderItemId, int categoryId, string categoryName, DateTime interviewDate, 
            int applicationNo, string candidateName, string interviewMode, string attendanceStatus)
          {
               InterviewId = interviewId;
               OrderItemId = orderItemId;
               CategoryId = categoryId;
               CategoryName = categoryName;
               InterviewDate = interviewDate;
               ApplicationNo = applicationNo;
               CandidateName = candidateName;
               InterviewMode = interviewMode;
               AttendanceStatus = attendanceStatus;
          }

          public int InterviewId { get; set; }
        public int OrderItemId {get; set;}
        public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        public DateTime InterviewDate { get; set; }
        public int ApplicationNo {get; set;}
        public string CandidateName {get; set;}
        public string InterviewMode {get; set;}
        public string AttendanceStatus {get; set;}
        public string Remarks {get; set;}
        //public DateTime AttendedOn {get; set;}
        //public ICollection<InterviewItemCandidateDto> InterviewItemCandidates {get; set;}
    }
}