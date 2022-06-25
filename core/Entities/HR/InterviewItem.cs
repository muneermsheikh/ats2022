using System;
using System.Collections.Generic;

namespace core.Entities.HR
{
    public class InterviewItem: BaseEntity
    {
        public InterviewItem()
        {
        }

        public InterviewItem(int orderItemId, int categoryId, DateTime interviewDatefrom, DateTime interviewDateupto, string interviewMode, string interviewerName)
        {
            OrderItemId = orderItemId;
            CategoryId = categoryId;
            InterviewDateFrom = interviewDatefrom;
            InterviewDateUpto = interviewDateupto;
            InterviewMode = interviewMode;
            InterviewerName = interviewerName;
        }

        public int InterviewId { get; set; }
        public int OrderItemId { get; set; }
        public int CategoryId { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto {get; set;}
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewStatus {get; set;}
        public string ConcludingRemarks {get; set;}
        public ICollection<InterviewItemCandidate> InterviewItemCandidates {get; set;}

    }
}