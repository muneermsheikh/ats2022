using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Admin;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace core.ParamsAndDtos
{
    public class RegisterEmployeeDto
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required] 
        public string KnownAs { get; set; }
        //public Address Address {get; set;}
        public string DisplayName { get; set; }
        public string UserName {get; set;}
        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", 
            ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
        public string Password { get; set; }

        [Required] 
        [EmailAddress]
        public string Email { get; set; }
        public DateTime DOB {get; set;}
        public string PlaceOfBirth { get; set; }
        public string Department {get; set;}    //for employees
        public DateTime DOJ {get; set;}         //for employees
        public string AadharNo { get; set; }    //for employees
        public string Nationality {get; set;}="Indian";
        public string UserRole { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        public string Position { get; set; }
        public int LoggedInAppUserId { get; set; }
        public int AppUserId {get; set;}
        public string PhotoUrl { get; set; }
        public ICollection<EmployeeAddress> EmployeeAddresses {get; set;}
        public ICollection<EmployeeQualification> EmployeeQualifications {get; set;}
        public ICollection<EmployeePhone> EmployeePhones {get; set;}
        public ICollection<EmployeeHRSkill> HrSkills {get; set;}    
        public ICollection<EmployeeOtherSkill> OtherSkills{get; set;}  

    }
}