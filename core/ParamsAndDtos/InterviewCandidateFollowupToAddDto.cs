using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
    public class InterviewCandidateFollowupToAddDto
    {
        [Required]
        public DateTime ContactedOn {get; set;}
        [Required]
        public int ContactedById { get; set; }
        [Required]
        public int AttendanceStatusId {get; set;}
        public ICollection<InterviewFollowupItemDto> followupItems {get; set;}
    }

   
}