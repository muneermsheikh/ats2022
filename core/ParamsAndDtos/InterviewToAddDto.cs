using System;
using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
    public class InterviewToAddDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string InterviewVenue { get; set; }
        [Required]
        public string InterviewMode {get; set;}
        [Required]
        public DateTime InterviewDateFrom { get; set; }
        [Required]
        public DateTime InterviewDateUpto { get; set; }
        [Required]
        public int InterviewLeaderId { get; set; }
        public string CustomerRepresentative { get; set; }
    }
}