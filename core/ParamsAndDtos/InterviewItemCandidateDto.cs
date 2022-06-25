using System;

namespace core.ParamsAndDtos
{
    public class InterviewItemCandidateDto
    {
        public int InterviewItemId {get; set;}
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string PassportNo { get; set; }
        public DateTime ScheduledFrom { get; set; }
        public string InterviewMode { get; set; }
        public DateTime InterviewedDateTime { get; set; }
        public string SelectionStatusName { get; set; }
        public string Remarks { get; set; }

    }
}