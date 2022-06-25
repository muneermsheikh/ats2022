using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
     public class ProspectiveCandidateAddDto
    {
        [Required, Range(1,int.MaxValue)]
        public int ProspectiveId { get; set; }
        [Required]
        public string CandidateName {get; set;}
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string PhoneNo { get; set; }
        public string AlternatePhoneNo {get; set;}
        [EmailAddress, Required]
        public string Email { get; set; }
        [Required, RegularExpression("(?=^.{6,11}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", 
            ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and beween 6 to 11 characters")]
        public string Password { get; set; }
        [Required]
        public string Age {get; set;}
        [Required]
        public string CategoryRef {get; set;}
        [Required]
        public string Source {get; set;}
        public string CurrentLocation {get; set;}
        public int LoggedInUserId { get; set; }
    }
}