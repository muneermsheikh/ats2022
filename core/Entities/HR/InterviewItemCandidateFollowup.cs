using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.HR
{
    public class InterviewItemCandidateFollowup: BaseEntity
    {
          public InterviewItemCandidateFollowup()
          {
          }

          public InterviewItemCandidateFollowup(int interviewItemCandidateId, DateTime contactedOn, int contactedById, string mobileNoCalled, int attendanceStatusId)
          {
               InterviewItemCandidateId = interviewItemCandidateId;
               ContactedOn = contactedOn;
               ContactedById = contactedById;
               MobileNoCalled = mobileNoCalled;
               AttendanceStatusId = attendanceStatusId;
          }

          public int InterviewItemCandidateId {get; set;}
        public DateTime ContactedOn {get; set;}
        public int ContactedById { get; set; }
        [MinLength(10), MaxLength(10)]
        public string MobileNoCalled { get; set; }
        public int AttendanceStatusId {get; set;}
        public bool FollowupConcluded {get; set;}=false;
                
    }
}