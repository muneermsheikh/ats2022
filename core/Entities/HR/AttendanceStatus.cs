using System.ComponentModel.DataAnnotations;

namespace core.Entities.HR
{
    public class InterviewAttendanceStatus: BaseEntity
    {
        [MinLength(5)]
        public string Status { get; set; }
    }
}