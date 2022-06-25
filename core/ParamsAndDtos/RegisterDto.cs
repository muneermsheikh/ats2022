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
    public class RegisterDto
    {
        public int Id { get; set; }
        public string UserType {get; set;}
        public string Gender { get; set; }="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required] public string KnownAs { get; set; }
        public int ReferredBy { get; set; }
        public Address Address {get; set;}
        
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
        public string PpNo { get; set; }
        public DateTime PPValidity { get; set; }
        public string Nationality {get; set;}="Indian";
        public int? CompanyId {get; set;}
        public string UserRole { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        public string Position { get; set; }
        public bool NotificationDesired {get; set;}
        public int LoggedInAppUserId { get; set; }
        public int AppUserId {get; set;}
        public bool AppUserIdNotEnforced {get; set;}
        public ICollection<EntityAddress> EntityAddresses {get; set;}
        public ICollection<UserQualification> UserQualifications {get; set;}
        public ICollection<UserExp> UserExperiences {get; set;}
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserPassport> UserPassports {get; set;}
        public ICollection<EmployeeHRSkill> HrSkills {get; set;}        //for employees
        public ICollection<EmployeeOtherSkill> OtherSkills{get; set;}   //for employees
        public ICollection<UserAttachment> UserAttachments {get; set;}
        //public ICollection<IFormFile> UserFormFiles { get; set; }



    }
}