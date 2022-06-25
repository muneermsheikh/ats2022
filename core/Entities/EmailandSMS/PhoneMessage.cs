using System;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.EmailandSMS
{
    public class PhoneMessage: BaseEntity
    {
        [Required]
        public string AppUserId { get; set; }
        [Required, MaxLength(14)]   //4 country code, 10 no.
        public string PhoneNo { get; set; }
        [Required]
        public DateTime DateOfAdvise { get; set; }
        [MaxLength(160)]
        public string TextAdvised { get; set; }
        public string CompanyAdvised { get; set; }
        public string OfficialAdvised { get; set; }
        
    }
}