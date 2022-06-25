using System;
using System.Collections.Generic;
using core.Entities.HR;

namespace core.ParamsAndDtos
{
    public class InterviewAttendanceDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string InterviewVenue { get; set; }
        public ICollection<InterviewItemDto> InterviewItems {get; set;}
    }
}