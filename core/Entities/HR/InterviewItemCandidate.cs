using System;
using System.Collections.Generic;

namespace core.Entities.HR
{
    public class InterviewItemCandidate: BaseEntity
    {
        public InterviewItemCandidate()
        {
        }

        public InterviewItemCandidate(int interviewItemId, int candidateId, int applicationNo, string passportNo, 
            string candidatename, DateTime scheduledFrom, DateTime scheduledUpto, string interviewMode)
        {
            InterviewItemId = interviewItemId;
            CandidateId = candidateId;
            ApplicationNo = applicationNo;
            PassportNo = passportNo;
            CandidateName = candidatename;
            ScheduledFrom = scheduledFrom;
            ScheduledUpto = scheduledUpto;
            InterviewMode = interviewMode;
        }

        public int InterviewItemId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string PassportNo { get; set; }
        public DateTime ScheduledFrom { get; set; }
        public DateTime ScheduledUpto { get; set; }
        public string InterviewMode { get; set; }
        public DateTime? ReportedDateTime { get; set; }
        public DateTime? InterviewedDateTime { get; set; }
        public int? AttendanceStatusId {get; set;}  //update when candidate attendance/selection concluded
        public int? SelectionStatusId { get; set; }
        public string ConcludingRemarks { get; set; }
        public ICollection<InterviewItemCandidateFollowup> InterviewFollowups { get; set; }
    }
}