using System;
using System.Collections.Generic;

namespace core.ParamsAndDtos
{
    public class AssignCandidatesToAddDto
    {
        public int InterviewItemId { get; set; }
        public DateTime ScheduledTimeFrom {get; set;}
        public DateTime ScheduledTimeUpto {get; set;}
        public string InterviewMode {get; set;}
        public List<int> CandidateIds {get; set;}
    }
}