using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Users;

namespace core.Entities.Admin
{
     public class Employee: BaseEntity
    {
          public Employee()
          {
          }

          public Employee(int appUserId, string gender, string firstName, string secondName, string familyName, 
               string knownAs, DateTime dob, string aadharNo, ICollection<EmployeeQualification> qualifications, 
               DateTime dOJ, string department, string position, string email, ICollection<EmployeeHRSkill> hrSkills, //string password,
               ICollection<EmployeeOtherSkill> otherSkills, ICollection<EmployeeAddress> employeeAddresses)
          {
               AppUserId = appUserId; Gender=gender; FirstName = firstName; SecondName=secondName; FamilyName=familyName;
               KnownAs=knownAs; DateOfBirth= dob; AadharNo=aadharNo; Email = email;
               EmployeeQualifications = qualifications;
               DateOfJoining = dOJ; Department = department; HrSkills = hrSkills; OtherSkills = otherSkills;
               Position = position; EmployeeAddresses = employeeAddresses;
          }

          public Employee(string gender, string firstName, string secondName, string familyName, string knownAs, 
               DateTime dob, string aadharNo, int appUserId, DateTime doj, string dept, 
               ICollection<EmployeePhone> userphones,  ICollection<EmployeeQualification> qualifications,  
               ICollection<EmployeeHRSkill> employeeHRSkills, ICollection<EmployeeOtherSkill> empOtherSkills,
               ICollection<EmployeeAddress> employeeAddresses)
          {
               Gender=gender; FirstName = firstName; SecondName=secondName; FamilyName=familyName;
               KnownAs=knownAs; DateOfBirth= dob; AppUserId = appUserId; AadharNo = aadharNo; EmployeeQualifications = qualifications;
               HrSkills = employeeHRSkills; OtherSkills = empOtherSkills; EmployeeAddresses = employeeAddresses;
               EmployeePhones = userphones; DateOfJoining = doj; Department = dept; //Password = password;
          }

        public int AppUserId { get; set; }
        public string Gender { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required]         
        public string KnownAs { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string Nationality {get; set;} = "Indian";
        [EmailAddress]
        public string Email {get; set;}
        public DateTime DateOfJoining {get; set;}
        public string Department { get; set; }
        public DateTime? LastWorkingDay {get; set;}
        public EnumEmployeeStatus Status { get; set; } = EnumEmployeeStatus.Employed;
        public string Remarks { get; set; }        
        public DateTime Created {get; set;}=DateTime.Now;
        public DateTime? LastActive {get; set;}
        public string PhotoUrl { get; set; }
        //public string Password {get; set;}
        public ICollection<EmployeeAddress> EmployeeAddresses {get; set;}
        public ICollection<EmployeeQualification> EmployeeQualifications {get; set;}
        public ICollection<EmployeeHRSkill> HrSkills {get; set;}
        public ICollection<EmployeeOtherSkill> OtherSkills{get; set;}
        public ICollection<EmployeePhone> EmployeePhones {get; set;}
        /* public string Add { get; set; }
        public string Address2 {get; set;}
        public string City { get; set; }
        public string Country {get; set;}
     */
    }
}