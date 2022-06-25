using System;
using System.ComponentModel.DataAnnotations;

namespace core.ParamsAndDtos
{
    public class CustomerOfficialToCreateDto
    {
        public bool LogInCredential {get; set;}
        public string Gender { get; set; }="M";
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required]
        public string KnownAs {get; set;}
        public string Designation { get; set; }
        public string Nationality {get; set;}
        public string ImageUrl { get; set; }
        public int CompanyId {get; set;}
        public string PhoneNo {get; set;}
        public string Mobile { get; set; }
        public DateTime DOB { get; set; }
        public string Add { get; set; }
        public string StreetAdd { get; set; }
        public string Location { get; set; }
        [Required]
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string Country { get; set; }
        public int AppUserId { get; set; }
        public string DisplayName { get; set; }
        public string UserName {get; set;}
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", 
            ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
     }
}